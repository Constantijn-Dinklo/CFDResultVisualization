using System.Collections.Generic;
using UnityEngine;

public class MeshContainer
{
    private List<CustomMesh> meshes;
    public MeshContainer(){
        this.meshes = new List<CustomMesh>();
    }

    public int AddMesh(){
        int mesh_id = meshes.Count;
        CustomMesh newMesh = new CustomMesh(mesh_id);
        meshes.Add(newMesh);

        return mesh_id;
    }

    public CustomMesh GetMesh(int index){
        return meshes[index];
    }

    public void DrawMesh(int index, Material mat){
        //string face_name = "Face_" + i.ToString();
        CustomMesh meshToDraw = meshes[index];

        string polyMesh_name = "Mesh_" + index.ToString();
        GameObject obj = new GameObject(polyMesh_name, typeof(Material), typeof(MeshRenderer), typeof(MeshFilter));
        obj.GetComponent<Renderer>().material = mat;
        obj.GetComponent<MeshFilter>().mesh = meshToDraw.GetMesh();
    }
}