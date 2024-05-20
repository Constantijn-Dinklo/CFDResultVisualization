using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelBoundary
{
    public Vector3 startCoord;

    public float width, height, depth; 

    public VoxelBoundary(float x, float y, float z, float w, float h, float d){
        startCoord = new Vector3(x, y, z);
        
        this.width = w;
        this.height = h;
        this.depth = d;
    }

    public VoxelBoundary(Vector3 startCoord, float w, float h, float d){
        this.startCoord = startCoord;

        this.width = w;
        this.height = h;
        this.depth = d;
    }

    public float sizeSqr() {
        float size = (this.width + this.height + this.depth) / 3.0f;
        return size * size;
    }

    public bool contains(Vector3 point){

        bool contains = ((point.x >= startCoord.x) && (point.x < (startCoord.x + width)) && 
                         (point.y >= startCoord.y) && (point.y < (startCoord.y + height)) &&
                         (point.z >= startCoord.z) && (point.z < (startCoord.z + depth)));

        return contains;
    }

    public bool intersectsXZ(Vector3 p1, Vector3 p2, Vector3 p3){
        
        if(p1.x < startCoord.x && p2.x < startCoord.x && p3.x < startCoord.x){
            return false;
        }

        if((p1.x > (startCoord.x + height)) && (p2.x > (startCoord.x + height)) && (p3.x > (startCoord.x + height))){
            return false;
        }

        if(p1.z < startCoord.z && p2.z < startCoord.z && p3.z < startCoord.z){
            return false;
        }

        if((p1.z > (startCoord.z + depth)) && (p2.z > (startCoord.z + depth)) && (p3.z > (startCoord.z + depth))){
            return false;
        }

        return true;
    }

    public VoxelBoundary[] subdivide(){

        VoxelBoundary[] newBounds = new VoxelBoundary[8];

        float x = startCoord.x;
        float y = startCoord.y;
        float z = startCoord.z;

        float half_width = this.width / 2;
        float half_height = this.height / 2;
        float half_depth = this.depth / 2;

        
        newBounds[0] = new VoxelBoundary(x, y, z, half_width, half_height, half_depth);
        newBounds[1] = new VoxelBoundary(x + half_width, y, z, half_width, half_height, half_depth);
        newBounds[2] = new VoxelBoundary(x, y + half_height, z, half_width, half_height, half_depth);
        newBounds[3] = new VoxelBoundary(x + half_width, y + half_height, z, half_width, half_height, half_depth);

        newBounds[4] = new VoxelBoundary(x, y, z + half_depth, half_width, half_height, half_depth);
        newBounds[5] = new VoxelBoundary(x + half_width, y, z + half_depth, half_width, half_height, half_depth);
        newBounds[6] = new VoxelBoundary(x, y + half_height, z + half_depth, half_width, half_height, half_depth);
        newBounds[7] = new VoxelBoundary(x + half_width, y + half_height, z + half_depth, half_width, half_height, half_depth);

        return newBounds;
    }

    public Vector3 getCenter(){

        Vector3 center = new Vector3(this.startCoord.x, this.startCoord.y, this.startCoord.z);

        float half_width = this.width / 2;
        float half_height = this.height / 2;
        float half_depth = this.depth / 2;

        center.x += half_width;
        center.y += half_height;
        center.z += half_depth;

        return center;
    }

}
