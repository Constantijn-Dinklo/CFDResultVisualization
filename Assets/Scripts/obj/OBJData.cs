using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;


public class OBJData
{

    //private List<Vector3> points;
    private PointCloud points;

    private List<OBJFace> faces;

    Mesh mesh;

    public Vector3 checkPoint;

    public OBJData(){
        this.mesh = new Mesh();

        points = new PointCloud();
        faces = new List<OBJFace>();

        //TODO: calculate this point based on the OJBdata, and not have a constant value
        checkPoint = new Vector3(0, 0, 0); //For now we grab a constant point that is outside the OBJ area, this point should be determined by looking at the obj data.
    }


    public int insideBuilding(VoxelBoundary voxelBoundary, bool showFacesInBox, bool showFacesIntersecting){

        int num_intersections = 0;

        Vector3 voxelCenter = voxelBoundary.getCenter();
        
        checkPoint.x = voxelCenter.x;
        checkPoint.z = voxelCenter.z;

        if(!points.insidePointCloud(voxelCenter)){
            //Debug.Log("Did not check this obj");
            return num_intersections;
        }

        //Debug.Log("Did check this obj");

        //DebugDraw.DrawCross(checkPoint, 100, 100, 100, Color.cyan);

        //Debug.Log(checkPoint);

        for(int i = 0; i < this.faces.Count; ++i){
            OBJFace face = this.faces[i];

            //Debugging statements to only include some of the faces.
            // if(i < 680310 || i > 681319){
            //     continue;
            // }

            // if(i < 680815 || i > 681067){
            //     continue;
            // }

            // if(i < 680815 || i > 680941){
            //     continue;
            // }

            // if(i < 680878 || i > 680941){
            //     continue;
            // }

            // if(i < 680895 || i > 680896){
            //     continue;
            // }
            // if(i != 680897){
            //     continue;
            // }


            Vector3 p1 = this.points.getPoint(face.vertices_index[0]).position;
            Vector3 p2 = this.points.getPoint(face.vertices_index[1]).position;
            Vector3 p3 = this.points.getPoint(face.vertices_index[2]).position;

            //Our raycast goes in a straight line along the X direction.
            //If that face is outside the box created by the YZ boundary of this voxel it is not possible to intersect with this face
            //Skip it, it will save a lot of time 
            if(voxelBoundary.intersectsXZ(p1, p2, p3)){

                if(showFacesInBox){
                    Debug.DrawLine(p1, p2, Color.blue, 1000);
                    Debug.DrawLine(p2, p3, Color.blue, 1000);
                    Debug.DrawLine(p3, p1, Color.blue, 1000);
                }
                //Debug.Log(i);

                if(intersectFace(face, checkPoint, voxelCenter)){
                    if(showFacesIntersecting){
                        Debug.DrawLine(p1, p2, Color.red, 1000);
                        Debug.DrawLine(p2, p3, Color.red, 1000);
                        Debug.DrawLine(p3, p1, Color.red, 1000);
                    }
                    num_intersections++;
                } 
            }
            //}
        }

        //Debug.Log(num_intersections);
        return num_intersections;
    }


    public int insideBuilding(Vector3 point){

        int num_intersections = 0;
        for(int i = 0; i < this.faces.Count; ++i){
            if(intersectFace(faces[i], checkPoint, point)){
                num_intersections++;
                //return -1;
            }
        }
        //Debug.Log(num_intersections);

        return num_intersections;
    }


    //Check if the line created by l0 and l
    //   face:  The face against which to check if the line intersecs
    //   l0:    The start point of the line
    //   l:     The direction (normalized) of the line
    public bool intersectFace(OBJFace face, Vector3 startPoint, Vector3 endPoint){

        Vector3 faceNormal = face.getNormal();
        Vector3 facePoint = this.points.getPoint(face.vertices_index[0]).position; //Always simply grab the first point of the face as it doesn't matter which point we grab for the plane equation.

        Vector3 lineLength = endPoint - startPoint;
        Vector3 direction = lineLength;
        direction.Normalize();

        float distance = lineLength.y / direction.y;

        float t = 0;
        if(intersectPlane(faceNormal, facePoint, startPoint, direction, ref t)){
            Vector3 p1 = this.points.getPoint(face.vertices_index[0]).position;
            Vector3 p2 = this.points.getPoint(face.vertices_index[1]).position;
            Vector3 p3 = this.points.getPoint(face.vertices_index[2]).position;

            //return true;

            if(t > distance){
                return false;
            }

            // Debug.DrawLine(p1, p2, Color.green, 1000);
            // Debug.DrawLine(p2, p3, Color.yellow, 1000);
            // Debug.DrawLine(p3, p1, Color.red, 1000);
            
            Vector3 intersection_point = startPoint + direction * t;

            //DebugDraw.DrawCross(intersection_point, 10, 10, 10, Color.black);

            if(sameSide(p1, p2, intersection_point, p3) && sameSide(p2, p3, intersection_point, p1) && sameSide(p3, p1, intersection_point, p2)){
                // Debug.DrawLine(p1, p2, Color.red, 1000);
                // Debug.DrawLine(p2, p3, Color.red, 1000);
                // Debug.DrawLine(p3, p1, Color.red, 1000);
                
                return true;
            }
        }

        return false;
    }

    //Check if points p and v are on the same side of the line as l0 -> l1.
    //Params:
    //  l0: The start point of the line of the triangle
    //  l1: The end point of the line of the triangle
    //  p: The point to check if it is on the same side as v
    //  v: The point of the triangle we want to varify p against
    public bool sameSide(Vector3 l0, Vector3 l1, Vector3 p, Vector3 v){

        Vector3 pSide = Vector3.Cross(l1-l0, p-l0);
        Vector3 vSide = Vector3.Cross(l1-l0, v-l0);

        return Vector3.Dot(pSide, vSide) >= 0;

    }

    
    public bool intersectPlane(Vector3 n, Vector3 p0, Vector3 l0, Vector3 l, ref float t){

        float denom = Vector3.Dot(n, l); 
        //Debug.Log(denom);
        if(Math.Abs(denom) > 1e-6){
            Vector3 p0l0 = p0 - l0;
            t = Vector3.Dot(p0l0, n) / denom; 
            //Debug.Log(t); 
            return (t >= 0);
        }

        return false;
    }

    
    
    public void readFile(string fileName){
        StreamReader obj_reader = new StreamReader(fileName);

        //string[] lines = new string[100];

        //Assign values to all the points in the point cloud
        int index = 0;
        while(obj_reader.Peek() != -1){

            string line = obj_reader.ReadLine();
            // lines[index] = line;

            // if(line.StartsWith("//")){
            //     break;
            // }

            if(line.StartsWith("v")){
                string[] split_line = line.Split(' ');

                Vector3 point_pos = new Vector3(float.Parse(split_line[1]), float.Parse(split_line[3]), float.Parse(split_line[2]));
                Point p = new Point(point_pos);
                points.addPoint(p);
            }

            if(line.StartsWith("f")){
                string[] split_line = line.Split(' ');

                int v1 = Int32.Parse(split_line[1]) -1;
                int v2 = Int32.Parse(split_line[2]) - 1;
                int v3 = Int32.Parse(split_line[3]) - 1;

                OBJFace new_face = new OBJFace(v1, v2, v3);
                new_face.calculateNormal(this.points);
                faces.Add(new_face);
            }

            index++;
        }

        obj_reader.Close();

        this.checkPoint.y = points.getMinCoords().y - 100; //Set the check point to always be 100 behind the total data set
        
        // StreamWriter obj_writer = new StreamWriter(fileName);
        // for(int i = 0; i < index; ++i){
        //     obj_writer.Write(lines[i]);
        // }
        // obj_writer.Close();

        Debug.Log(String.Format("Finished reading OBJ with {0} points and {1} faces", this.points.numPoints(), this.faces.Count));
        //Debug.Log(String.Format("The c"))
        //DebugDraw.DrawBox(points.getMinCoords(), points.getWidth(), points.getHeight(), points.getDepth(), Color.black);
    }

    public Mesh GetMesh(){

        this.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        Vector3[] vertices = new Vector3[this.points.numPoints()];
        for(int i = 0; i < this.points.numPoints(); ++i){
            vertices[i] = this.points.getPoint(i).position;
        }

        int[] triangles = new int[this.faces.Count * 3];
        for(int i = 0; i < this.faces.Count; ++i){
            OBJFace face = this.faces[i];

            int v1 = face.vertices_index[0];
            int v2 = face.vertices_index[1];
            int v3 = face.vertices_index[2];

            triangles[i*3] = face.vertices_index[0];
            triangles[i*3 + 1] = face.vertices_index[1];
            triangles[i*3 + 2] = face.vertices_index[2];
        }

        this.mesh.vertices = vertices;
        this.mesh.triangles = triangles;

        this.mesh.RecalculateNormals();

        return this.mesh;

    }

}
