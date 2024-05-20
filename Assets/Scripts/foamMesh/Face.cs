using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FaceType{
    Boundary,
    Internal
}


public class Face{

    public int face_id;
    public int num_triangles;
    public List<int> triangles;

    public FaceType faceType;

    public Vector3 normal;

    //TODO: Potentially make this a reference to the cell class?
    public Cell owner_cell;
    public Cell neighbour_cell;

    public Face(int face_id){
        this.face_id = face_id;
        this.num_triangles = 0;
        this.triangles = new List<int>();

        this.faceType = FaceType.Boundary;

        this.owner_cell = null;
        this.neighbour_cell = null;
    }

    public void addTriangle(int v1, int v2, int v3){
        this.triangles.Add(v1);
        this.triangles.Add(v2);
        this.triangles.Add(v3);

        this.num_triangles += 1;
    }

    public Vector3 calculateNormal(int traingleIndex, Vector3[] vertices){

        Vector3 p1 = vertices[triangles[traingleIndex * 3]];
        Vector3 p2 = vertices[triangles[traingleIndex * 3 + 1]];
        Vector3 p3 = vertices[triangles[traingleIndex * 3 + 2]];

        Vector3 p2p1 = p2 - p1;
        Vector3 p3p1 = p3 - p1;

        normal = Vector3.Cross(p2p1, p3p1);
        normal.Normalize();
        return normal;
    }
}


// public class Face
// {
//     int v1_index, v2_index, v3_index, v4_index;

//     public Mesh mesh;

//     public Face(int v1_index, int v2_index, int v3_index, int v4_index){
//         this.v1_index = v1_index;
//         this.v2_index = v2_index;
//         this.v3_index = v3_index;
//         this.v4_index = v4_index;
//     }

//     public void calculateMesh(ref Data data){
//         mesh = new Mesh();
//         //mesh.name = "Face";
//         mesh.vertices = new [] { data.getPoint(this.v1_index), 
//                                  data.getPoint(this.v2_index), 
//                                  data.getPoint(this.v3_index), 
//                                  data.getPoint(this.v4_index) };
//         // mesh.vertices = new [] { new Vector3(-250, 0, -200), 
//         //                          new Vector3(-150, 0, -200), 
//         //                          new Vector3(-150, -200, -200), 
//         //                          new Vector3(-250, -200, -200) };
//         //mesh.uv = [Vector2 (0, 0), Vector2 (0, 1), Vector2(1, 1), Vector2 (1, 0)];
//         mesh.triangles = new [] {0, 1, 2, 0, 2, 3};
//         //mesh.RecalculateNormals();
//     }

//     public Mesh GetMesh(){
//         return this.mesh;
//     }
// }
