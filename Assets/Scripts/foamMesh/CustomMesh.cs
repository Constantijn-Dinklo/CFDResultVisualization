using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;


public struct Boundary{

    public int start_face;
    public int num_faces;
    public Boundary(int start_face, int num_faces){
        this.start_face = start_face;
        this.num_faces = num_faces;
    }
}

public class CustomMesh
{
    private int id;

    private Mesh mesh;

   
    private int num_vert;
    private Vector3[] vertices;

    public Vector3 minVertex;
    public Vector3 maxVertex;

    private List<Boundary> boundaries;

    private List<Face> faces;

    private List<Cell> cells;
    
    //TODO: Possibly give this mesh a reference to the point cloud in which all points are stored,
    //      then determine the start and end index of the points that make up this mesh.
    //      Only copy the vertices over if we need to show this mesh.
    //private PointCloud pointCloud; //This is the point cloud in which the vertices are kept
    // private int pointCloudStartVertice;
    // private int numberOfVertices;

    public CustomMesh(int id){
        this.id = id;

        this.mesh = new Mesh();
        this.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        minVertex = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        maxVertex = new Vector3(float.MinValue, float.MinValue, float.MinValue);

        
        this.boundaries = new List<Boundary>();
        this.faces = new List<Face>();
        this.cells = new List<Cell>();
    }

    public void setNumVertices(int num_vertices){
        this.num_vert = num_vertices;
        this.vertices = new Vector3[num_vertices + 6];
        //this.numberOfVertices = num_vertices;
    }

    public void addVertice(int index, Vector3 vertex){
        this.vertices[index] = vertex;

        this.minVertex.x = Mathf.Min(this.minVertex.x, vertex.x);
        this.minVertex.y = Mathf.Min(this.minVertex.y, vertex.y);
        this.minVertex.z = Mathf.Min(this.minVertex.z, vertex.z);

        this.maxVertex.x = Mathf.Max(this.maxVertex.x, vertex.x);
        this.maxVertex.y = Mathf.Max(this.maxVertex.y, vertex.y);
        this.maxVertex.z = Mathf.Max(this.maxVertex.z, vertex.z);
    }

    public void addFace(int[] vertices_indexes){
        int num_v = vertices_indexes.Length;
        int face_id = this.faces.Count;

        Face newFace;

        if(num_v == 3){
            newFace = new Face(face_id);
            newFace.addTriangle(vertices_indexes[0], vertices_indexes[1], vertices_indexes[2]);
        }
        else if(num_v == 4){
            newFace = new Face(face_id);
            newFace.addTriangle(vertices_indexes[0], vertices_indexes[1], vertices_indexes[2]);
            newFace.addTriangle(vertices_indexes[0], vertices_indexes[2], vertices_indexes[3]);

        }
        else if(num_v == 5){
            newFace = new Face(face_id);
            newFace.addTriangle(vertices_indexes[0], vertices_indexes[1], vertices_indexes[2]);
            newFace.addTriangle(vertices_indexes[0], vertices_indexes[2], vertices_indexes[4]);
            newFace.addTriangle(vertices_indexes[2], vertices_indexes[3], vertices_indexes[4]);
        }
        else if (num_v == 6){
            newFace = new Face(face_id);
            newFace.addTriangle(vertices_indexes[0], vertices_indexes[1], vertices_indexes[2]);
            newFace.addTriangle(vertices_indexes[0], vertices_indexes[2], vertices_indexes[4]);
            newFace.addTriangle(vertices_indexes[2], vertices_indexes[3], vertices_indexes[4]);
            newFace.addTriangle(vertices_indexes[0], vertices_indexes[4], vertices_indexes[5]);
        }
        else if (num_v == 7){
            newFace = new Face(face_id);
            newFace.addTriangle(vertices_indexes[0], vertices_indexes[1], vertices_indexes[2]);
            newFace.addTriangle(vertices_indexes[0], vertices_indexes[2], vertices_indexes[3]);
            newFace.addTriangle(vertices_indexes[0], vertices_indexes[3], vertices_indexes[4]);
            newFace.addTriangle(vertices_indexes[0], vertices_indexes[4], vertices_indexes[5]);
            newFace.addTriangle(vertices_indexes[0], vertices_indexes[5], vertices_indexes[6]);
        }
        else{
            //Debug.Log(num_v);
            newFace = new Face(face_id);
            // newFace.addTriangle(vertices_indexes[0], vertices_indexes[1], vertices_indexes[2]);
            // newFace.addTriangle(vertices_indexes[0], vertices_indexes[2], vertices_indexes[3]);
            // newFace.addTriangle(vertices_indexes[0], vertices_indexes[3], vertices_indexes[4]);
            // newFace.addTriangle(vertices_indexes[0], vertices_indexes[4], vertices_indexes[5]);
            // newFace.addTriangle(vertices_indexes[0], vertices_indexes[5], vertices_indexes[6]);
            // newFace.addTriangle(vertices_indexes[0], vertices_indexes[6], vertices_indexes[7]);

            for(int i = 0; i < num_v - 2; ++i){
                newFace.addTriangle(vertices_indexes[0], vertices_indexes[i + 1], vertices_indexes[i + 2]);
            }

        }
        faces.Add(newFace);
    }

    public void addBoundary(int start_face, int num_faces){
        Boundary newBoundary = new Boundary(start_face, num_faces);
        boundaries.Add(newBoundary);
    }
    
    public void updateMesh(int start_face, int num_faces, int num_boundaries, char key){
        this.mesh.Clear();

        List<int> triangles_to_show = new List<int>();

        Color[] colors = new Color[this.vertices.Length];

        float max_k = 0;
        float min_k = 1;


        if(num_faces > 0){
            int end_face = start_face + num_faces;
            for(int i = start_face; i < end_face; ++i){
                
                Face face = this.faces[i];
                for(int k = 0; k < face.num_triangles; ++k){
                    int v1 = face.triangles[k*3];
                    int v2 = face.triangles[k*3 + 1];
                    int v3 = face.triangles[k*3 + 2];

                    triangles_to_show.Add(v1);
                    triangles_to_show.Add(v2);
                    triangles_to_show.Add(v3);

                    Cell owner_cell = face.owner_cell;
                    float k_value = owner_cell.getScalarValue(key);

                    if(k_value > max_k){
                        max_k = k_value;
                    }
                    if(k_value < min_k){
                        min_k = k_value;
                    }
                }
            }
        }

        if(num_boundaries == -1){
            num_boundaries = boundaries.Count;
        }
        // else{
        //     num_boundaries = boundary_ids_to_show.Length;
        // }
        // boundaries_to_show = new Boundary[num_boundaries];

        int num_triangles_showing = 0;
        for(int i = 0; i < num_boundaries; ++i){
            Boundary boundary = boundaries[i];

            int start_face_offset = 0;

            int end_boundary_face = boundary.start_face + start_face_offset + boundary.num_faces;
            for(int j = boundary.start_face + start_face_offset; j < end_boundary_face; ++j){

                Face face = this.faces[j];
                for(int k = 0; k < face.num_triangles; ++k){
                    int v1 = face.triangles[k*3];
                    int v2 = face.triangles[k*3 + 1];
                    int v3 = face.triangles[k*3 + 2];

                    triangles_to_show.Add(v1);
                    triangles_to_show.Add(v2);
                    triangles_to_show.Add(v3);

                    Cell owner_cell = face.owner_cell;
                    float k_value = owner_cell.getScalarValue(key);

                    if(k_value > max_k){
                        max_k = k_value;
                    }
                    if(k_value < min_k){
                        min_k = k_value;
                    }

                    // Debug.Log(string.Format("The vertice index for face {0}:", j));
                    // Debug.Log(string.Format("The coordinates of vertex {0} : {1} {2} {3}", v1, this.vertices[v1].x, this.vertices[v1].y, this.vertices[v1].z));
                    // Debug.Log(string.Format("The coordinates of vertex {0} : {1} {2} {3}", v2, this.vertices[v2].x, this.vertices[v2].y, this.vertices[v2].z));
                    // Debug.Log(string.Format("The coordinates of vertex {0} : {1} {2} {3}", v3, this.vertices[v3].x, this.vertices[v3].y, this.vertices[v3].z));

                
                    // float v1_num = this.vertices[v1].z;
                    // float v2_num = this.vertices[v2].z;
                    // float v3_num = this.vertices[v3].z;

                    // if (v1_num < min_z){
                    //     min_z = v1_num;
                    // }
                    // if (v2_num < min_z){
                    //     min_z = v2_num;
                    // } 
                    // if (v3_num < min_z){
                    //     min_z = v3_num;
                    // }

                    // if(v1_num > max_z){
                    //     max_z = v1_num;
                    // }
                    // if(v2_num > max_z){
                    //     max_z = v2_num;
                    // }
                    // if(v3_num > max_z){
                    //     max_z = v3_num;
                    // }
                
                }
                num_triangles_showing += 1;
                
            }
        }

        float k_diff = max_k - min_k;

        
        if(num_faces > 0){
            int end_face = start_face + num_faces;
            for(int i = start_face; i < end_face; ++i){
                
                Face face = this.faces[i];
                for(int k = 0; k < face.num_triangles; ++k){
                    int v1 = face.triangles[k*3];
                    int v2 = face.triangles[k*3 + 1];
                    int v3 = face.triangles[k*3 + 2];

                    Cell owner_cell = face.owner_cell;
                    float k_value = owner_cell.getScalarValue(key);

                    float display_k = (k_value - min_k) / k_diff;

                    colors[v1] = Color.Lerp(Color.red, Color.green, display_k);
                    colors[v2] = Color.Lerp(Color.red, Color.green, display_k);
                    colors[v3] = Color.Lerp(Color.red, Color.green, display_k);
                }
            }
        }
        
        
        for(int i = 0; i < num_boundaries; ++i){
            Boundary boundary = boundaries[i];

            int start_face_offset = 0;

            int end_boundary_face = boundary.start_face + start_face_offset + boundary.num_faces;
            for(int j = boundary.start_face + start_face_offset; j < end_boundary_face; ++j){

                Face face = this.faces[j];
                for(int k = 0; k < face.num_triangles; ++k){
                    int v1 = face.triangles[k*3];
                    int v2 = face.triangles[k*3 + 1];
                    int v3 = face.triangles[k*3 + 2];

                    Cell owner_cell = face.owner_cell;
                    float k_value = owner_cell.getScalarValue(key);

                    float display_k = (k_value - min_k) / k_diff;

                    colors[v1] = Color.Lerp(Color.red, Color.green, display_k);
                    colors[v2] = Color.Lerp(Color.red, Color.green, display_k);
                    colors[v3] = Color.Lerp(Color.red, Color.green, display_k);
                }
            }
        }
        
        

        int[] triangles_showing = new int[triangles_to_show.Count];
        //Debug.Log("Triangles To Showing:");
        for(int i = 0; i < triangles_to_show.Count; ++i){
            //Debug.Log(string.Format("{0}", triangles_to_show[i]));
            triangles_showing[i] = (int)triangles_to_show[i];
            //Debug.Log(string.Format("{0}", triangles_showing[i]));
        }

        this.mesh.vertices = this.vertices;
        this.mesh.triangles = triangles_showing;
        // this.mesh.colors = colors;

        Debug.Log(String.Format("A total of {0} barriers were included", num_boundaries));
        Debug.Log(String.Format("Showing mesh {0} with {1} triangles", this.id, triangles_to_show.Count));

        this.mesh.RecalculateNormals();
    }

    public Mesh GetMesh(){
        return this.mesh;
    }

    public Vector3[] GetVertices(){
        return this.vertices;
    }

    public int getNumCells(){
        return this.cells.Count;
    }

    public List<Cell> getCells() {
        return this.cells;
    }
    
    public void CenterOfCells(string file_path){

        StreamWriter writer = new StreamWriter(file_path);

        for(int i = 0; i < this.cells.Count; ++i){
            Vector3 center = this.cells[i].getCenter(this.vertices);

            //Debug.Log(center);

            string center_write = center[0] + " " + center[1] + " " + center[2];
            //Debug.Log(center_write);

            writer.WriteLine(center_write);
        }        
        
        writer.Close();
    }

    
    public void drawMinMaxVer(Color cmin, Color cmax)
    {
        Debug.Log(this.minVertex);
        Debug.Log(this.maxVertex);
        DebugDraw.DrawCross(this.minVertex, 100, 100, 100, cmin, 1000);
        DebugDraw.DrawCross(this.maxVertex, 100, 100, 100, cmax, 1000);
        // for(int i = 0; i < this.vertices.Length; ++i)
        // {
        //     if( (this.vertices[i].x == this.minVertex.x) ||
        //         (this.vertices[i].y == this.minVertex.y) ||
        //         (this.vertices[i].z == this.minVertex.z)) {
        //             DebugDraw.DrawCross(this.vertices[i], 100, 100, 100, cmin, 1000);
        //         }
        //      if((this.vertices[i].x == this.maxVertex.x) ||
        //         (this.vertices[i].y == this.maxVertex.y) ||
        //         (this.vertices[i].z == this.maxVertex.z)) {
        //             DebugDraw.DrawCross(this.vertices[i], 100, 100, 100, cmax, 1000);
        //         }
        // }
    }

    public int getNumFaces(){
        return this.faces.Count;
    }

    public void WriteBounds(string file_path)
    {
        StreamWriter writer = new StreamWriter(file_path);

        float width = maxVertex.x - minVertex.x;
        float height = maxVertex.y - minVertex.y;
        float depth = maxVertex.z - minVertex.z;

        Debug.Log(String.Format("The min vertex is: {0}", minVertex));
        Debug.Log(String.Format("The max vertex is: {0}", maxVertex));

        string minValue = minVertex.x + " " + minVertex.y + " " + minVertex.z;
        string demString = width + " " + height + " " + depth;

        writer.WriteLine(minValue);
        writer.WriteLine(demString);

        writer.Close();
    }
    
    public void ReadPointFile(string file_path){
        
        StreamReader reader = new StreamReader(file_path);

        //Read past header of file
        string line = "";
        while (reader.Peek() != -1){
            line = reader.ReadLine();
            if (line.StartsWith("//")){
                break;
            }
        }

        //Determine the number of points that this file contains
        int number_of_points = 0;
        while (reader.Peek() != -1){
            line = reader.ReadLine();
            if (line != ""){
                try{
                    number_of_points = Int32.Parse(line);
                    break;
                }
                catch(FormatException e){
                    Debug.Log(e);
                }
            }
        }
        this.setNumVertices(number_of_points);

        reader.ReadLine();

        //Read the points for this mesh
        string point_line = "";
        for(int i = 0; i < number_of_points; ++i){
            point_line = reader.ReadLine();

            point_line = point_line.Replace("(", "");
            point_line = point_line.Replace(")", "");

            string[] coords = point_line.Split(' ');

            try {
                //Vector3 point = this.pointCloud.addPoint(float.Parse(coords[0]), float.Parse(coords[1]), float.Parse(coords[2]));
                Vector3 point = new Vector3(float.Parse(coords[0]), float.Parse(coords[2]), float.Parse(coords[1]));
                this.addVertice(i, point);
            }
            catch(FormatException e){
                Debug.Log(e);
            }
        }
        // Debug.Log(String.Format("Finished reading {0} points", number_of_points));
        reader.Close();
    }

    public void ReadFaceFile(string file_path){

        StreamReader reader = new StreamReader(file_path);

        //Read past header of file
        string line = "";
        while (reader.Peek() != -1){
            line = reader.ReadLine();
            if (line.StartsWith("//")){
                break;
            }
        }

        //Determine the number of faces that this file contains
        int number_of_faces = 0;
        while (reader.Peek() != -1){
            line = reader.ReadLine();
            if (line != ""){
                try{
                    number_of_faces = Int32.Parse(line);
                    break;
                }
                catch(FormatException e){
                    Debug.Log(e);
                }
            }
        }

        reader.ReadLine();

        //Read the faces for this mesh
        string face_line = "";
        for(int i = 0; i < number_of_faces; ++i){
            face_line = reader.ReadLine();
            string original_line = face_line;
            int bracket_index = face_line.IndexOf("(");
            if(bracket_index > 1){
                Debug.Log(bracket_index);
            }
            //Debug.Log(bracket_index);
            string num_point_on_face_str = face_line.Substring(0, bracket_index);
            face_line = face_line.Substring(bracket_index);
            face_line = face_line.Replace("(", "");
            face_line = face_line.Replace(")", "");

            if(bracket_index > 1){
                Debug.Log(face_line);
            }

            string[] point_ids = face_line.Split(' ');

            try {
                int num_point_on_face = Int32.Parse(num_point_on_face_str);
                if(num_point_on_face <= 1){
                    Debug.Log(original_line);
                }

                int[] vertices = new int[num_point_on_face];

                for(int j = 0; j < num_point_on_face; ++j){
                    int vi = Int32.Parse(point_ids[j]);
                    vertices[j] = vi;
                }
                this.addFace(vertices);
            }
            catch(FormatException e){
                Debug.Log(e);
            }
            
        }

        // Debug.Log(String.Format("Finished reading {0} faces", number_of_faces));
        reader.Close();

    }

    public void ReadBoundaryFile(string file_path){

        StreamReader reader = new StreamReader(file_path);

        //Read past header of file
        string line = "";
        while (reader.Peek() != -1){
            line = reader.ReadLine();
            if (line.StartsWith("//")){
                break;
            }
        }

        //Determine the number of boundaries that this file contains
        int number_of_boundaries = 0;
        while (reader.Peek() != -1){
            line = reader.ReadLine();
            if (line != ""){
                try{
                    number_of_boundaries = Int32.Parse(line);
                    break;
                }
                catch(FormatException e){
                    Debug.Log(e);
                }
            }
        }

        //Read up until the opening bracket.
        line = reader.ReadLine().Trim(); //This should be the opening bracket '(' of the list of boundaries
        while(line != "("){
            line = reader.ReadLine().Trim();
        }

        //Read the boundaries for this mesh
        for(int i = 0; i < number_of_boundaries; ++i){

            line = reader.ReadLine().Trim();
            if(line == ""){
                continue;
            }

            //We have found a boundary structure
            string boundaryName = line;

            line = reader.ReadLine().Trim(); //SHould be the opening bracket of the boundary '{'

            int start_face = 0;
            int num_faces = 0;
            while(line != "}"){
                line = reader.ReadLine().Trim();

                if(line.StartsWith("nFaces")){
                    int semi_index = line.IndexOf(';');
                    line = line.Remove(semi_index);
                    string[] line_parts = line.Split(' ');
                    string num_faces_str = line_parts[line_parts.Length - 1];
                    
                    try{
                        num_faces = Int32.Parse(num_faces_str);
                    }
                    catch (Exception e){
                        Debug.Log(e);
                    }
                }

                if(line.StartsWith("startFace")){
                    int semi_index = line.IndexOf(';');
                    line = line.Remove(semi_index);
                    string[] line_parts = line.Split(' ');
                    string start_face_str = line_parts[line_parts.Length - 1];

                    try{
                        start_face = Int32.Parse(start_face_str);
                    }
                    catch (Exception e){
                        Debug.Log(e);
                    }
                }
            }
            this.addBoundary(start_face, num_faces);
        }

        // Debug.Log(String.Format("Finished reading the boundary file with {0} boundaries", number_of_boundaries));
        reader.Close();

    }

    public void ReadOwnerFile(string file_path){
        StreamReader reader = new StreamReader(file_path);

        //Read past header of file
        string line = "";
        while (reader.Peek() != -1){
            line = reader.ReadLine();
            if (line.StartsWith("//")){
                break;
            }
        }

        //Determine the number of enteries that this file contains
        int number_of_owners = 0;
        while (reader.Peek() != -1){
            line = reader.ReadLine();
            if (line != ""){
                try{
                    number_of_owners = Int32.Parse(line);
                    break;
                }
                catch(FormatException e){
                    Debug.Log(e);
                }
            }
        }

        reader.ReadLine(); // The (

        //Read which cell is the owner of each face.
        string owner_line = "";
        for(int i = 0; i < number_of_owners; ++i){
            owner_line = reader.ReadLine();
            try{
                int cell_id = Int32.Parse(owner_line);

                if(this.cells.Count <= cell_id){
                    for(int y = this.cells.Count; y <= cell_id; ++y){
                        Cell new_cell = new Cell(y);
                        this.cells.Add(new_cell);
                    }
                }

                Cell owner_cell = this.cells[cell_id];
                owner_cell.addFace(faces[i]);
                
                faces[i].owner_cell = owner_cell;
            }
            catch(FormatException e){
                Debug.Log(e);
            }
        }

        // Debug.Log(String.Format("Finished reading the owner file with {0} enteries", number_of_owners));
        reader.Close();
    }

    public void ReadNeighbourFile(string file_path){
        StreamReader reader = new StreamReader(file_path);

        //Read past header of file
        string line = "";
        while (reader.Peek() != -1){
            line = reader.ReadLine();
            if (line.StartsWith("//")){
                break;
            }
        }

        //Determine the number of enteries that this file contains
        int number_of_neighbours = 0;
        while (reader.Peek() != -1){
            line = reader.ReadLine();
            if (line != ""){
                try{
                    number_of_neighbours = Int32.Parse(line);
                    break;
                }
                catch(FormatException e){
                    Debug.Log(e);
                }
            }
        }

        reader.ReadLine(); // The (
        
        //Read which cell is the neighbour of each internal face.
        string neighbour_line = "";
        for(int i = 0; i < number_of_neighbours; ++i){
            neighbour_line = reader.ReadLine();
            try{
                int cell_id = Int32.Parse(neighbour_line);

                if(this.cells.Count <= cell_id){
                    for(int y = this.cells.Count; y <= cell_id; ++y){
                        Cell new_cell = new Cell(y);
                        this.cells.Add(new_cell);
                    }
                }

                Cell neighbour_cell = this.cells[cell_id];

                neighbour_cell.addFace(faces[i]);
                
                faces[i].neighbour_cell = neighbour_cell;
                faces[i].faceType = FaceType.Internal;
            }
            catch(FormatException e){
                Debug.Log(e);
            }
        }
        
        // Debug.Log(String.Format("Finished reading the neighbour file with {0} enteries", number_of_neighbours));
        reader.Close();
    }

    public void ReadValueFile(string file_path, char key){
        StreamReader reader = new StreamReader(file_path);

        //Read past header of file
        string line = "";
        while (reader.Peek() != -1){
            line = reader.ReadLine();
            if (line.StartsWith("//")){
                break;
            }
        }

        //Read till the dimensions field
        while (reader.Peek() != -1){
            line = reader.ReadLine();
            if(line.StartsWith("dimensions")){
                //TODO: do something with the dimensions
                break;
            }
        }

        //Read till the internal field
        while (reader.Peek() != -1){
            line = reader.ReadLine();
            if(line.StartsWith("internalField")){
                //TODO: do something with the internal field
                break;
            }
        }

        //Read till the number of values 
        int number_of_values = 0;
        while (reader.Peek() != -1){
            line = reader.ReadLine();
            if (line != ""){
                try{
                    number_of_values = Int32.Parse(line);
                    break;
                }
                catch(FormatException e){
                    Debug.Log(e);
                }
            }
        }

        reader.ReadLine(); // The (

        //Read the k value of each cell
        string value_line = "";
        for(int i = 0; i < number_of_values; ++i){
            value_line = reader.ReadLine();
            try{
                float value = float.Parse(value_line);

                Cell cell = this.cells[i];
                cell.setScalarValue(key, value);
            }
            catch(FormatException e){
                Debug.Log(e);
            }
        }

    }


    public void ReadPlainValueFileScalar(string file_path, char key){
        StreamReader reader = new StreamReader(file_path);

        for(int i = 0; i < cells.Count; ++i){
            string value_line = reader.ReadLine();
            try{
                float value = float.Parse(value_line);
                cells[i].setScalarValue(key, value);
            }
            catch(FormatException e){
                Debug.Log(e);
            }
        }
    }

    public void ReadPlainValueFileVector(string file_path, char key){
        StreamReader reader = new StreamReader(file_path);

        for(int i = 0; i < cells.Count; ++i){
            string[] vectorParts = reader.ReadLine().Replace("(", "").Replace(")", "").Split(' ');
            // for(int j = 0; j < vectorParts.Length; ++j){
            //     Debug.Log(vectorParts[j]);
            // }
            try {
                //Vector3 point = this.pointCloud.addPoint(float.Parse(coords[0]), float.Parse(coords[1]), float.Parse(coords[2]));
                Vector3 vector = new Vector3(float.Parse(vectorParts[0]), float.Parse(vectorParts[2]), float.Parse(vectorParts[1]));
                cells[i].setVectorValue(key, vector);
            }
            catch(FormatException e){
                Debug.Log(e);
            }
        }
    }
}

