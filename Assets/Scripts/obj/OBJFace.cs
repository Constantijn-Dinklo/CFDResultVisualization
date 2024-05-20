using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OBJFace
{

    public int[] vertices_index = new int[3];
    
    private Vector3 normal;


    public OBJFace(int v1, int v2, int v3){
        this.vertices_index[0] = v3;
        this.vertices_index[1] = v2;
        this.vertices_index[2] = v1;
    }

    public void calculateNormal(PointCloud pc){

        Vector3 p1 = pc.getPoint(vertices_index[0]).position;
        Vector3 p2 = pc.getPoint(vertices_index[1]).position;
        Vector3 p3 = pc.getPoint(vertices_index[2]).position;

        Vector3 p2p1 = p2 - p1;
        Vector3 p3p1 = p3 - p1;

        normal = Vector3.Cross(p2p1, p3p1);
        normal.Normalize();
    }

    public Vector3 getNormal(){
        return this.normal;
    }
}
