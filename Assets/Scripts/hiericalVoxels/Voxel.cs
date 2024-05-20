using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;

public class Voxel
{
    static int MAX_DEPTH = 10;
    static int NUM_CHILDREN = 8;

    static int CAPACITY = 10;

    Voxel parent;
    Voxel[] children;

    VoxelBoundary boundary;

    //int processorNum;
    string processor_file_path;
    int nodeDepth;
    int index;
    int flatten_index;
    List<Point> points;

    bool leafVoxel = true;

    float value = float.MinValue;

    //This constructor is only for the initial Root voxel
    public Voxel(Vector3 startCoords, float w, float h, float d, string pfp){
        this.parent = null;
        this.children = new Voxel[NUM_CHILDREN];

        this.boundary = new VoxelBoundary(startCoords, w, h, d);

        this.processor_file_path = pfp;
        this.nodeDepth = 0;
        this.index = 0;
        this.flatten_index = 0;
        this.points = new List<Point>();
    }

    //This is the constructor that should be used for any voxel that is a child of another voxel
    //Parent: the parent of this voxel (ie the voxel that has depth - 1)
    //Boundary: The bottom-left coordinate of the voxel with the width, height and depth included
    //D: the depth of this voxel
    //Index: The index this voxel takes in the flattened 1d array for this depth.
    public Voxel(Voxel parent, VoxelBoundary boundary, string pfp, int d, int index){
        this.parent = parent;
        this.children = new Voxel[NUM_CHILDREN];

        this.boundary = boundary;

        this.processor_file_path = pfp;
        this.nodeDepth = d;
        this.index = index;
        this.flatten_index = -1;
        this.points = new List<Point>();
    }

    public VoxelBoundary getBoundary(){
        return this.boundary;
    }

    //This method should only be called if you are certain this point should be added into this voxel.
    public void addPoint(Point p){
        this.points.Add(p);

        //We only want to subdivide this voxel if all of the following statements are true:
        //1. The amount of points that are now in this voxel is sufficiently large enough to want to subdivide
        //2. This voxel has not already been subdivided (it is still a leaf voxel)
        //3. The depth of the tree is not too large. We only want to suvdivide the space a maximum number of times (TODO: check if we don't want to subdivide further)
        if((this.points.Count > CAPACITY) && leafVoxel && (MAX_DEPTH > this.nodeDepth)){
            subdivide();
        }

        //If this is not the leaf voxel we want to add the point to one of its children as well.
        if(!leafVoxel){

            //Loop through all children to determine in which child this point is located
            for(int j = 0; j < NUM_CHILDREN; ++j){
                Voxel child = this.children[j];
                if(child.contains(p.position)){
                    child.addPoint(p);
                    break;
                }
            }
        }
    }

    public bool contains(Vector3 coord){
        return this.boundary.contains(coord);
    }

    
    
    public int getStartIndexForDepth(int child_index, int depth){

        int side_length = (int)Math.Pow(2, depth);
        int depth_side_length = (int)Math.Pow(2, depth - this.nodeDepth - 1);
        int half_cube_size = side_length * side_length * depth_side_length;

        int[] index_array = {0, 
                             0 + depth_side_length, 
                             0 + (side_length * depth_side_length), 
                             0 + (side_length * depth_side_length) + depth_side_length,
                             0 + half_cube_size, 
                             0 + half_cube_size + depth_side_length, 
                             0 + half_cube_size + (side_length * depth_side_length), 
                             0 + half_cube_size + (side_length * depth_side_length) + depth_side_length
                            };

        if(parent == null){
            return index_array[child_index];
        }

        return parent.getStartIndexForDepth(index, depth) + index_array[child_index];
    }
    
    private void subdivide(){

        VoxelBoundary[] newboundary = boundary.subdivide();
        int newDepth = this.nodeDepth + 1; 

        int start_index = 0;
        if(parent != null){
            start_index = parent.getStartIndexForDepth(index, newDepth);
        }

        int side_length = (int)Math.Pow(2, newDepth);
        int depth_side_length = (int)Math.Pow(2, newDepth - this.nodeDepth - 1); //Should always equal 1, but we'll keep the math to make sure.
        int half_cube_size = side_length * side_length * depth_side_length;

        int[] index_array = {start_index, 
                             start_index + depth_side_length, 
                             start_index + (side_length * depth_side_length), 
                             start_index + (side_length * depth_side_length) + depth_side_length,
                             start_index + half_cube_size, 
                             start_index + half_cube_size + depth_side_length, 
                             start_index + half_cube_size + (side_length * depth_side_length), 
                             start_index + half_cube_size + (side_length * depth_side_length) + depth_side_length
                            };
        
        
        //Initialize the new children 
        for(int i = 0; i < NUM_CHILDREN; ++i){
            this.children[i] = new Voxel(this, newboundary[i], this.processor_file_path, newDepth, i);
            this.children[i].flatten_index = index_array[i];
        }

        //All the points in this node also need to be put into the children nodes
        for(int i = 0; i < this.points.Count; ++i){
            Point p = this.points[i];

            //Loop through all children to determine in which child this point is located
            for(int j = 0; j < NUM_CHILDREN; ++j){
                Voxel child = this.children[j];
                if(child.contains(p.position)){
                    child.addPoint(p);
                    break;
                }
            }
        }

        leafVoxel = false;
    }

    public int minDepth(){
        if(leafVoxel){
            return nodeDepth;
        }
        else {
            int min_depth = 10000000;
            for(int j = 0; j < NUM_CHILDREN; ++j){
                int d = this.children[j].minDepth();
                if(d < min_depth){
                    min_depth = d;
                }
            }

            return min_depth;
        }
    }

    public int maxDepth(){
        if(leafVoxel){
            return nodeDepth;
        }
        else{
            int max_depth = 0;
            for(int j = 0; j < NUM_CHILDREN; ++j){
                int d = this.children[j].maxDepth();
                if(d > max_depth){
                    max_depth = d;
                }
            }

            return max_depth;
        }
    }
    
    public int nodesAtDepth(int d){
        if (nodeDepth == d){
            return 1;
        }
        else {
            if(!leafVoxel){
                int total = 0;
                for(int j = 0; j < NUM_CHILDREN; ++j){
                    total += this.children[j].nodesAtDepth(d);
                }
                return total;
            }
            else {
                return 0;
            }
        }
    }

    public bool insideBuilding(OBJData objData){

        
        int num_intersections = 0;
        // if(flatten_index == 50){
        // if(flatten_index >= 536 && flatten_index < 537){
        //     num_intersections = objData.insideBuilding(this.boundary, false, true);
        //     this.DrawCenter(Color.cyan);
        //     Debug.Log(num_intersections);
        //     //Debug.Log(objData.checkPoint);
        //     //DebugDraw.DrawCross(objData.checkPoint, 100, 100, 100, Color.cyan);
        //     //Debug.DrawLine(objData.checkPoint, this.boundary.getCenter(), Color.black, 1000);
        // }
        // else {
        num_intersections = objData.insideBuilding(this.boundary, false, false);
        // }
        if((num_intersections % 2) == 1){ //If there are an odd amount of intersections then it is inside a building
            return true;
        }
        return false;
    }

    public void insideBuildingWrite(int depth, OBJData OBJData, ref int[]inside_building_output){

        if(depth == this.nodeDepth){
            //If this voxel is inside a building in some other OBJ file, we can skip any checks.
            //Once inside a building, always inside a building!

            if(inside_building_output[flatten_index] == 1){
                return;
            }
            if(insideBuilding(OBJData)){
                inside_building_output[flatten_index] = 1;
            }
            else{
                inside_building_output[flatten_index] = 0;
            }
            return;
        }

        if(leafVoxel){
            this.subdivide();
        }

        for(int i = 0; i < NUM_CHILDREN; ++i){
            this.children[i].insideBuildingWrite(depth, OBJData, ref inside_building_output);
        }

    }

    public bool insideBuildingDebug(int checkDepth){

        if(checkDepth == this.nodeDepth){
            Color drawColor = Color.green;

            //if(flatten_index == 271){
            //if((flatten_index == 271) || (flatten_index == 272)){

            int[] in_building = BuildingInfo.getInsideBuildingAtDepth(this.processor_file_path, this.nodeDepth);
            if(in_building[flatten_index] == 1){
                drawColor = Color.red;
                this.DrawCenter(drawColor);
                return true;
            }
            this.DrawCenter(drawColor);
            return false;
            //}
            //return false;
        }

        if(leafVoxel){
            this.subdivide();
        }
        
        for(int j = 0; j < NUM_CHILDREN; ++j){
            this.children[j].insideBuildingDebug(checkDepth);
        }
        return false;
    }

    //This start the flattening process for the voxels
    //This should only ever be called on the root voxel
    public float[] start_flatten(int depth, string key){

        double side_length = Math.Pow(2, depth);
        double size = side_length * side_length * side_length;
        
        //Debug.Log(String.Format("The size of the 1d array is: {0}", size));
        float[] values = new float[(int)size];
        
        this.flatten(ref values, depth, key);
       
        return values;
    }

    //New Implimantation:
    //Actually subdivide the nodes, remove children nodes after the values have been filled
    public void flatten(ref float[] values, int returnDepth, string key) {
               
        if(this.nodeDepth == returnDepth) {
            float value = 0.0f;
            value = this.getValue(key);
            // int[] inside_building_input = BuildingInfo.getInsideBuildingAtDepth(this.processor_file_path, returnDepth);
            // if(inside_building_input[flatten_index] == 0){
            //     value = this.getValue();
            // }
            values[flatten_index] = value;
            return;
        }

        if(leafVoxel){
            this.subdivide();
        }

        for(int j = 0; j < NUM_CHILDREN; ++j){
            this.children[j].flatten(ref values, returnDepth, key);
        }
    }

    //public float getValue(OBJData OBJData, ref int inside_building_check){
    public float getValue(string key){

        //If we have already calculated the value once, simply return that value.
        // if(this.value != float.MinValue){
        //     DebugDraw.num_already++;
        //     return this.value;
        // }

        
        float numPoints = this.points.Count;
        if(numPoints <= 0){
            //If this is the root node and there are still no points in it, return 0
            if(this.nodeDepth == 0){
                DebugDraw.num_root++;
                return 0;
            }
            return parent.getValue(key);
        }


        float total = 0.0f;
        for(int i = 0; i < numPoints; ++i){
            Point p = this.points[i];
            total += p.getValue(key);
        }
        this.value = total / numPoints;
        DebugDraw.calculated_value++;

        return this.value;
    }

    
    public static Voxel createRootVoxel(string pfp, PointCloud pointCloud){

        Vector3 minC = pointCloud.getMinCoords();
        Vector3 maxC = pointCloud.getMaxCoords();

        float width = maxC.x - minC.x;
        float height = maxC.y - minC.y;
        float depth = maxC.z - minC.z;

        Voxel root = new Voxel(minC, width, height, depth, pfp);

        for(int i = 0; i < pointCloud.numPoints(); ++i){
            root.addPoint(pointCloud.getPoint(i));
        }

        return root;
    }
    
    public void DrawCenter(Color color){

        Vector3 cellCenter = this.boundary.getCenter();

        float half_width = this.boundary.width / 4;
        float half_height = this.boundary.height / 4;
        float half_depth = this.boundary.depth / 4;

        // Debug.Log("Drawing cross");
        // Debug.Log(cellCenter);

        DebugDraw.DrawCross(cellCenter, half_width, half_height, half_depth, color);

    }
    
    
    public void DrawDebug(int debugDrawDepth, bool drawPoints)
    {
       


        Color[] debugColors = {Color.black, Color.red, Color.green, Color.yellow, Color.cyan, Color.blue, Color.red, Color.green};
        Color[] inBuildingColors = {Color.green, Color.red};

        int[] in_building = BuildingInfo.getInsideBuildingAtDepth(this.processor_file_path, this.nodeDepth);

        
        //Color based on Depth
        //Color display_color = debugColors[nodeDepth];

        //Color basedon in building
        Color display_color = inBuildingColors[in_building[flatten_index]];

        DebugDraw.DrawBox(boundary.startCoord, boundary.width, boundary.height, boundary.depth, display_color);

        //Debug.Log(String.Format("Drawing node at depth {0}", this.nodeDepth));

        if(nodeDepth < debugDrawDepth){
            // recursively show children
            if (!leafVoxel)
            {
                for(int j = 0; j < NUM_CHILDREN; ++j){
                    this.children[j].DrawDebug(debugDrawDepth, drawPoints);
                }
            }
        }

        // recursively show children
        // if (!leafVoxel)
        // {
        //     for(int j = 0; j < NUM_CHILDREN; ++j){
        //         this.children[j].DrawDebug(debugDrawDepth, drawPoints);
        //     }
        // }

        if(drawPoints){
            // draw actual points
            for (int i = 0, length = points.Count; i < length; i++)
            {
                Point point = points[i];
                Debug.DrawRay(new Vector3(point.position.x, point.position.y, point.position.z), Vector3.up * 0.1f, Color.white, 100);
                Debug.DrawRay(new Vector3(point.position.x, point.position.y, point.position.z), -Vector3.up * 0.1f, Color.white, 100);

                Debug.DrawRay(new Vector3(point.position.x, point.position.y, point.position.z), Vector3.forward * 0.1f, Color.white, 100);
                Debug.DrawRay(new Vector3(point.position.x, point.position.y, point.position.z), -Vector3.forward * 0.1f, Color.white, 100);

                Debug.DrawRay(new Vector3(point.position.x, point.position.y, point.position.z), Vector3.right * 0.1f, Color.white, 100);
                Debug.DrawRay(new Vector3(point.position.x, point.position.y, point.position.z), -Vector3.right * 0.1f, Color.white, 100);
            }
        }

    }
}


// //This returns the values of voxels at a specific depth in a 1d array
//     //This 1d array is needed to use the VolumeRendering
//     public void flatten_old(ref float[] values, int returnDepth, int startIndex, ref float[]values_filled) {
        
        
//         if(this.nodeDepth == returnDepth) {
//             // Debug.Log(String.Format("Inserting value into index {0}", startIndex));
//             float value = this.getValue();
//             //Debug.Log(String.Format("Value to write: {0}", value));
//             values[startIndex] = value;
//             values_filled[startIndex] = startIndex; //Old debugging code, should remove the value_filled array, might keep around for now
//             return;
//         }

//         int side_length = (int)Math.Pow(2, returnDepth);
//         int depth_side_length = (int)Math.Pow(2, returnDepth - this.nodeDepth - 1);

//         if(leafVoxel){
//             //TODO: populate the values its children should have populated if they existed
//             int nextDepth = this.nodeDepth + 1;

//             // Debug.Log(String.Format("For start index {0} there are not more children, but we need a lower level", startIndex));

//             float value = this.getValue();
//             flatten_dummy(ref values, returnDepth, startIndex, nextDepth, value, ref values_filled);
//             flatten_dummy(ref values, returnDepth, startIndex + depth_side_length, nextDepth, value, ref values_filled);
//             flatten_dummy(ref values, returnDepth, startIndex + (side_length * depth_side_length), nextDepth, value, ref values_filled);
//             flatten_dummy(ref values, returnDepth, startIndex + (side_length * depth_side_length) + depth_side_length, nextDepth, value, ref values_filled);

            
//             int half_cube_size = side_length * side_length * depth_side_length;
            
//             flatten_dummy(ref values, returnDepth, startIndex + half_cube_size, nextDepth, value, ref values_filled);
//             flatten_dummy(ref values, returnDepth, startIndex + half_cube_size + depth_side_length, nextDepth, value, ref values_filled);
//             flatten_dummy(ref values, returnDepth, startIndex + half_cube_size + (side_length * depth_side_length), nextDepth, value, ref values_filled);
//             flatten_dummy(ref values, returnDepth, startIndex + half_cube_size + (side_length * depth_side_length) + depth_side_length, nextDepth, value, ref values_filled);
//         }
//         else {
//             this.children[0].flatten_old(ref values, returnDepth, startIndex, ref values_filled);
//             this.children[1].flatten_old(ref values, returnDepth, startIndex + depth_side_length, ref values_filled);
//             this.children[2].flatten_old(ref values, returnDepth, startIndex + (side_length * depth_side_length), ref values_filled);
//             this.children[3].flatten_old(ref values, returnDepth, startIndex + (side_length * depth_side_length) + depth_side_length, ref values_filled);

//             int half_cube_size = side_length * side_length * depth_side_length;

//             this.children[4].flatten_old(ref values, returnDepth, startIndex + half_cube_size, ref values_filled);
//             this.children[5].flatten_old(ref values, returnDepth, startIndex + half_cube_size + depth_side_length, ref values_filled);
//             this.children[6].flatten_old(ref values, returnDepth, startIndex + half_cube_size + (side_length * depth_side_length), ref values_filled);
//             this.children[7].flatten_old(ref values, returnDepth, startIndex + half_cube_size + (side_length * depth_side_length) + depth_side_length, ref values_filled);
//         }
//     }


    // public void flatten_dummy(ref float[] values, int returnDepth, int startIndex, int currentDepth, float value, ref float[] values_filled)
    // {
    //     //Debug.Log(String.Format("Current Depth is: {0}", currentDepth));
    //     if(currentDepth == returnDepth){
    //         //Debug.Log(String.Format("Setting value for index: {0}", startIndex));
    //         //Debug.Log(String.Format("Value to write in dummy: {0}", value));
    //         values[startIndex] = value;
    //         values_filled[startIndex] = startIndex;
    //         return;
    //     }
    //     else {
    //         int side_length = (int)Math.Pow(2, returnDepth);
    //         int depth_side_length = (int)Math.Pow(2, returnDepth - currentDepth - 1);

    //         int nextDepth = currentDepth + 1;
    //         flatten_dummy(ref values, returnDepth, startIndex, nextDepth, value, ref values_filled);
    //         flatten_dummy(ref values, returnDepth, startIndex + depth_side_length, nextDepth, value, ref values_filled);
    //         flatten_dummy(ref values, returnDepth, startIndex + (side_length * depth_side_length), nextDepth, value, ref values_filled);
    //         flatten_dummy(ref values, returnDepth, startIndex + (side_length * depth_side_length) + depth_side_length, nextDepth, value, ref values_filled);

            
    //         int half_cube_size = side_length * side_length * depth_side_length;
            
    //         flatten_dummy(ref values, returnDepth, startIndex + half_cube_size, nextDepth, value, ref values_filled);
    //         flatten_dummy(ref values, returnDepth, startIndex + half_cube_size + depth_side_length, nextDepth, value, ref values_filled);
    //         flatten_dummy(ref values, returnDepth, startIndex + half_cube_size + (side_length * depth_side_length), nextDepth, value, ref values_filled);
    //         flatten_dummy(ref values, returnDepth, startIndex + half_cube_size + (side_length * depth_side_length) + depth_side_length, nextDepth, value, ref values_filled);
    //     }
    // }