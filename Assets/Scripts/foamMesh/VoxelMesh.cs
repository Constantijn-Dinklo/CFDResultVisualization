using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelMesh
{

    private string id;


    private float voxel_size; //For now we use squares

    private float xmin = 1000000, ymin = 1000000, zmin = 1000000;
    private float xmax = -1000000, ymax = -1000000, zmax = -1000000;
 
    public VoxelMesh(CustomMesh mesh){

        this.determineBoundary(mesh.GetVertices());
    }

    public void determineBoundary(Vector3[] vertices){

        for(int i = 0; i < vertices.Length; ++i){
            if(vertices[i][0] <= xmin){
                xmin = vertices[i][0];
            }
            if(vertices[i][2] <= ymin){
                ymin = vertices[i][0];
            }
            if(vertices[i][1] <= zmin){
                zmin = vertices[i][0];
            }
            if(vertices[i][0] >= xmax){
                xmax = vertices[i][0];
            }
            if(vertices[i][2] >= ymax){
                ymax = vertices[i][0];
            }
            if(vertices[i][1] >= zmax){
                zmax = vertices[i][0];
            }
        }

        Debug.Log(string.Format("Min boundary: ({0}, {1}, {2})", xmin, ymin, zmin));
        Debug.Log(string.Format("Max boundary: ({0}, {1}, {2})", xmax, ymax, zmax));
    }
}
