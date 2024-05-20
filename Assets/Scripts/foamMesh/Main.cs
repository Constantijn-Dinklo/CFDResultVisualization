using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using GeoProc;


//using namespace GK;

public class Main : MonoBehaviour
{

    public Material polyMeshMaterial;
    private MeshContainer meshContainer;

    PointCloud pc;

    Vector3 minCoords;
    float new_width, new_height, new_depth;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("In Main");

        // GK.ConvexHullCalculator calc = new GK.ConvexHullCalculator();
        // var verts = new List<Vector3>();
        // var tris = new List<int>();
        // var normals = new List<Vector3>();
        meshContainer = new MeshContainer();

        pc = new PointCloud();

        // numCells();
        //findCenter();
        //findBounds();

        minCoords = new Vector3();
        new_width = 0;
        new_height = 0;
        new_depth = 0;


        // string constant_point_file_path = "Assets/Resources/small_ascii/constant/polyMesh/points";
        // string constant_face_file_path = "Assets/Resources/small_ascii/constant/polyMesh/faces";
        // string constant_boundary_file_path = "Assets/Resources/small_ascii/constant/polyMesh/boundary";
        
        // ReadPointFile(constant_point_file_path, constant_mesh_index);
        // ReadFaceFile(constant_face_file_path, constant_mesh_index);
        // ReadBoundaryFile(constant_boundary_file_path, constant_mesh_index);


        //TODO: This should all be concluded into a 'parse' function in the CustomMesh class
        //Every mesh should have these 5 files.
        // string two_point_file_path = "Assets/Resources/small_ascii/2/polyMesh/points";
        // string two_face_file_path = "Assets/Resources/small_ascii/2/polyMesh/faces";
        // string two_boundary_file_path = "Assets/Resources/small_ascii/2/polyMesh/boundary";
        // string two_owner_file_path = "Assets/Resources/small_ascii/2/polyMesh/owner";
        // string two_neighbour_file_path = "Assets/Resources/small_ascii/2/polyMesh/neighbour";

        // string two_point_file_path = "Assets/Resources/windAroundBuildings/constant/polyMesh/points";
        // string two_face_file_path = "Assets/Resources/windAroundBuildings/constant/polyMesh/faces";
        // string two_boundary_file_path = "Assets/Resources/windAroundBuildings/constant/polyMesh/boundary";
        // string two_owner_file_path = "Assets/Resources/windAroundBuildings/constant/polyMesh/owner";
        // string two_neighbour_file_path = "Assets/Resources/windAroundBuildings/constant/polyMesh/neighbour";

        // string two_point_file_path = "Assets/Resources/large_case/processorsASCII/processor0/2/polyMesh/points";
        // string two_face_file_path = "Assets/Resources/large_case/processorsASCII/processor0/2/polyMesh/faces";
        // string two_boundary_file_path = "Assets/Resources/large_case/processorsASCII/processor0/2/polyMesh/boundary";
        // string two_owner_file_path = "Assets/Resources/large_case/processorsASCII/processor0/2/polyMesh/owner";
        // string two_neighbour_file_path = "Assets/Resources/large_case/processorsASCII/processor0/2/polyMesh/neighbour";


        // int variable_mesh_index_two = meshContainer.AddMesh();
        // CustomMesh mesh_two = meshContainer.GetMesh(variable_mesh_index_two);

        // mesh_two.ReadPointFile(two_point_file_path);
        // mesh_two.drawMinMaxVer(Color.green, Color.red);
        // mesh_two.ReadFaceFile(two_face_file_path);
        // mesh_two.ReadBoundaryFile(two_boundary_file_path);
        // mesh_two.ReadOwnerFile(two_owner_file_path);
        // mesh_two.ReadNeighbourFile(two_neighbour_file_path);

        // Debug.Log(mesh_two.getCells().Count);

        // mesh_two.updateMesh(0, mesh_two.getNumFaces(), -1, 'k');
        // mesh_two.GetMesh();

        

        // DrawMesh(variable_mesh_index_two, 0, mesh_two.getNumFaces(), -1, 'k');

        // //Debug.Log(mesh_two.getNumCells());

        // string two_cell_center_file_path = "Assets/Resources/large_case/processorsASCII/processor0/2/polyMesh/cellCenters";
        // mesh_two.CenterOfCells(two_cell_center_file_path);

        //mesh_two.ReadValueFile(two_k_file_path, 'k');

        // int variable_mesh_index_two = meshContainer.AddMesh();
        // CustomMesh mesh_three = meshContainer.GetMesh(variable_mesh_index_two);

        // mesh_three.ReadPointFile(two_point_file_path);
        // mesh_three.ReadFaceFile(two_face_file_path);
        // mesh_three.ReadBoundaryFile(two_boundary_file_path);
        // mesh_three.ReadOwnerFile(two_owner_file_path);
        // mesh_three.ReadNeighbourFile(two_neighbour_file_path);

        // mesh_three.ReadValueFile(two_k_file_path, 'k');


        
        // //this.polyMesh.DrawMesh(0, this.polyMeshMaterial);
        // DrawMesh(constant_mesh_index, 0 , -1, -1);
        //DrawMesh(variable_mesh_index, 0, 0, -1, 'k', "Mesh_1");
        //DrawMesh(variable_mesh_index_two, 0, 0, 6, 'k', "Mesh_2");

        // writeProcessorBounds();
        // for(int lodi = 0; lodi < 6; ++lodi){
        //     string basePathError = "Assets/Resources/ErrorData/region245Error/";
        //     // int lodi = 2;
        //     double sideLengthi = Math.Pow(2, lodi);
        //     double pointAmounti = sideLengthi * sideLengthi * sideLengthi;

        //     string errorPathP = basePathError + String.Format("pError{0}", lodi);
        //     StreamWriter pWriter = new StreamWriter(errorPathP);
        //     for(int i = 0; i < pointAmounti; ++i){
        //         pWriter.WriteLine(0);
        //     }
        //     pWriter.Close();

        //     string errorPathV = basePathError + String.Format("UError{0}", lodi);
        //     StreamWriter UWriter = new StreamWriter(errorPathV);
        //     for(int i = 0; i < pointAmounti; ++i){
        //         UWriter.WriteLine(0 + " " + 0 + " " + 0);
        //     }
        //     UWriter.Close();
        // }

        int region = 0;

        findMissingCells(region, 5);
        return;

        // for(int lod = 0; lod < 6; ++lod){
        //     initializeErrorFiles(region, lod);
        // }
        // return;

        int processor = 0;

        string two_point_file_path = String.Format("Assets/Resources/large_case/processorsASCII/processor{0}/2/polyMesh/points", processor);
        string two_face_file_path = String.Format("Assets/Resources/large_case/processorsASCII/processor{0}/2/polyMesh/faces", processor);
        string two_boundary_file_path = String.Format("Assets/Resources/large_case/processorsASCII/processor{0}/2/polyMesh/boundary", processor);
        string two_owner_file_path = String.Format("Assets/Resources/large_case/processorsASCII/processor{0}/2/polyMesh/owner", processor);
        string two_neighbour_file_path = String.Format("Assets/Resources/large_case/processorsASCII/processor{0}/2/polyMesh/neighbour", processor);
        string pValuePath = String.Format("Assets/Resources/large_case/processorsASCII/processor{0}/1455/pPlain", processor);
        string uValuePath = String.Format("Assets/Resources/large_case/processorsASCII/processor{0}/1455/UPlain", processor);


        int variable_mesh_index_two = meshContainer.AddMesh();
        CustomMesh mesh_two = meshContainer.GetMesh(variable_mesh_index_two);

        mesh_two.ReadPointFile(two_point_file_path);
        mesh_two.drawMinMaxVer(Color.green, Color.red);
        mesh_two.ReadFaceFile(two_face_file_path);
        mesh_two.ReadBoundaryFile(two_boundary_file_path);
        mesh_two.ReadOwnerFile(two_owner_file_path);
        mesh_two.ReadNeighbourFile(two_neighbour_file_path);
        mesh_two.ReadPlainValueFileScalar(pValuePath, 'p');
        mesh_two.ReadPlainValueFileVector(uValuePath, 'U');

        // Debug.Log(mesh_two.getCells()[100].getScalarValue('p'));
        // Debug.Log(mesh_two.getCells()[100].getVectorValue('U'));

        //Calculate the boudning box of each cell before continuing
        List<Cell> cells = mesh_two.getCells();
        for(int i = 0; i < cells.Count; ++i){
            Cell cell = cells[i];
            cell.calculateBoundingBox(mesh_two.GetVertices());
        }


        string basePath = String.Format("Assets/Resources/ErrorData/region{0}/", region);
        for(int lod = 0; lod < 6; ++lod){
            // int lod = 5;
            double sideLength = Math.Pow(2, lod);
            double pointAmount = sideLength * sideLength * sideLength;
            
            Vector3[] points = new Vector3[(int)pointAmount];
            string pointPath = basePath + String.Format("points{0}", lod);
            StreamReader pointReader = new StreamReader(pointPath);
            for(int i = 0; i < pointAmount; ++i){
                string[] pointParts = pointReader.ReadLine().Split(' ');
                try {
                    Vector3 point = new Vector3(float.Parse(pointParts[0]), float.Parse(pointParts[1]), float.Parse(pointParts[2]));
                    points[i] = point;
                }
                catch(FormatException e){
                    Debug.Log(e);
                }
            }


            float[] scalarValues = new float[(int)pointAmount];
            string scalarFilePath = basePath + string.Format("{0}/voxel{1}", "p", lod);
            using (FileStream fileStream = File.OpenRead(scalarFilePath))
            {
                byte[] data = new byte[fileStream.Length];
                int bytesRead = fileStream.Read(data, 0, data.Length);

                int floatCount = data.Length / 8;
                // Debug.Log(floatCount);
                for (int i = 0; i < floatCount; i++)
                {
                    scalarValues[i] = (float)BitConverter.ToDouble(data, i * 8);
                }
                fileStream.Close();
            }


            Vector3[] vectorValues = new Vector3[(int)pointAmount];
            string vectorFilePath = basePath + string.Format("{0}/voxel{1}", "U", lod);
            using (FileStream fileStream = File.OpenRead(vectorFilePath))
            {
                byte[] data = new byte[fileStream.Length];
                int bytesRead = fileStream.Read(data, 0, data.Length);

                int floatCount = data.Length / 8;
                int counter = 0;
                for (int i = 0; i < floatCount; i+= 3)
                {
                    float x = (float)BitConverter.ToDouble(data, i * 8);
                    float z = (float)BitConverter.ToDouble(data, (i+1) * 8);
                    float y = (float)BitConverter.ToDouble(data, (i+2) * 8);
                    Vector3 value = new Vector3(x,y,z);
                    vectorValues[counter] = value;
                    counter++;
                }
                fileStream.Close();
            }

            
            //Error values
            string basePathError = String.Format("Assets/Resources/ErrorData/region{0}Error/", region);
            string errorPathP = basePathError + String.Format("pErrorPercentage{0}", lod);
            string errorPathV = basePathError + String.Format("UErrorPercentage{0}", lod);
            string errorPathVMag = basePathError + String.Format("UErrorMagPercentage{0}", lod);

            float[] pErrorPercentageValues = new float[(int)pointAmount];
            StreamReader pErrorPercentageReader = new StreamReader(errorPathP);
            for(int i = 0; i < pointAmount; ++i){
                try {
                    pErrorPercentageValues[i] = float.Parse(pErrorPercentageReader.ReadLine());
                }
                catch(FormatException e){
                    Debug.Log(e);
                }
            }
            pErrorPercentageReader.Close();

            Vector3[] uErrorPercentageValues = new Vector3[(int)pointAmount];
            StreamReader uErrorPercentageReader = new StreamReader(errorPathV);
            for(int i = 0; i < pointAmount; ++i){
                string[] valueParts = uErrorPercentageReader.ReadLine().Split(' ');
                try {
                    Vector3 value = new Vector3(float.Parse(valueParts[0]), float.Parse(valueParts[1]), float.Parse(valueParts[2]));
                    uErrorPercentageValues[i] = value;
                }
                catch(FormatException e){
                    Debug.Log(e);
                }
            }
            uErrorPercentageReader.Close();

            float[] uErrorMagPercentageValues = new float[(int)pointAmount];
            StreamReader uErrorMagPercentageReader = new StreamReader(errorPathVMag);
            for(int i = 0; i < pointAmount; ++i){
                try {
                    uErrorMagPercentageValues[i] = float.Parse(uErrorMagPercentageReader.ReadLine());
                }
                catch(FormatException e){
                    Debug.Log(e);
                }
            }
            uErrorMagPercentageReader.Close();
            
            
            
            for(int p = 0; p < points.Length; ++p){
                if(pErrorPercentageValues[p] != 0){
                    continue;
                }
                // bool cellFound = false;
                Vector3 point = points[p];
                for(int i = 0; i < cells.Count; ++i){
                    Cell cell = cells[i];
                    bool inside = cell.insideCell(point, mesh_two.GetVertices());
                    if(inside){
                        pErrorPercentageValues[p] = getScalarError(scalarValues[p], cell.getScalarValue('p'));
                        uErrorPercentageValues[p] = getVectorError(vectorValues[p], cell.getVectorValue('U'));
                        uErrorMagPercentageValues[p] = getVectorMagError(vectorValues[p], cell.getVectorValue('U'));

                        if(pErrorPercentageValues[p] == 0){
                            pErrorPercentageValues[p] = -1000000;
                        }

                        if(uErrorPercentageValues[p].x == 0 && uErrorPercentageValues[p].y == 0 && uErrorPercentageValues[p].z == 0){
                            uErrorPercentageValues[p] = new Vector3(-1000000, -1000000, -1000000);
                        }

                        if(uErrorMagPercentageValues[p] == 0){
                            uErrorMagPercentageValues[p] = -1000000;
                        }

                        // if(Math.Abs(uErrorMagPercentageValues[p]) > 1){
                        //     Debug.Log(vectorValues[p].x + " " + vectorValues[p].y + " " + vectorValues[p].z);
                        //     Debug.Log(cell.getVectorValue('U').x + " " + cell.getVectorValue('U').y + " " + cell.getVectorValue('U').z);
                            // DebugDraw.DrawCross(point, 100, 100, 100, Color.black, 1000);
                            // return;
                        // }
                       
                        // cellFound = true;
                    }
                }
                // if(!cellFound){
                //     Debug.Log(String.Format("Point {0} did not find a cell", p));
                // }
            }


            StreamWriter pWriter = new StreamWriter(errorPathP);
            for(int i = 0; i < pointAmount; ++i){
                pWriter.WriteLine(pErrorPercentageValues[i]);
            }
            pWriter.Close();

            StreamWriter UWriter = new StreamWriter(errorPathV);
            for(int i = 0; i < pointAmount; ++i){
                UWriter.WriteLine(uErrorPercentageValues[i].x + " " + uErrorPercentageValues[i].y + " " + uErrorPercentageValues[i].z);
            }
            UWriter.Close();

            StreamWriter UMagWriter = new StreamWriter(errorPathVMag);
            for(int i = 0; i < pointAmount; ++i){
                // if(Math.Abs(uErrorMagPercentageValues[i]) > 1){
                //     DebugDraw.DrawCross(points[i], 100, 100, 100, Color.black, 1000);
                // }
                UMagWriter.WriteLine(uErrorMagPercentageValues[i]);
            }
            UMagWriter.Close();
        }

        // Vector3 minCoordP = mesh_two.minVertex;
        // Vector3 maxCoordP = mesh_two.maxVertex;
        // DebugDraw.DrawBox(minCoordP, maxCoordP.x - minCoordP.x, maxCoordP.y - minCoordP.y, maxCoordP.z - minCoordP.z, Color.red, 1000);


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void initializeErrorFiles(int region, int lod){
        double sideLength = Math.Pow(2, lod);
        double pointAmount = sideLength * sideLength * sideLength;

        string basePathError = String.Format("Assets/Resources/ErrorData/region{0}Error/", region);
        string errorPathP = basePathError + String.Format("pErrorPercentage{0}", lod);
        StreamWriter pWriter = new StreamWriter(errorPathP);
        for(int i = 0; i < pointAmount; ++i){
            pWriter.WriteLine(0);
        }
        pWriter.Close();

        string errorPathV = basePathError + String.Format("UErrorPercentage{0}", lod);
        StreamWriter UWriter = new StreamWriter(errorPathV);
        for(int i = 0; i < pointAmount; ++i){
            UWriter.WriteLine(0 + " " + 0 + " " + 0);
        }
        UWriter.Close();

        string errorPathVMag = basePathError + String.Format("UErrorMagPercentage{0}", lod);
        StreamWriter UMagWriter = new StreamWriter(errorPathVMag);
        for(int i = 0; i < pointAmount; ++i){
            UMagWriter.WriteLine(0);
        }
        UMagWriter.Close();
    }

    public void findMissingCells(int region, int lod){
        double sideLength = Math.Pow(2, lod);
        double pointAmount = sideLength * sideLength * sideLength;

        string basePath = String.Format("Assets/Resources/ErrorData/region{0}/", region);
        string pointPath = basePath + String.Format("points{0}", lod);
        Vector3[] points = readPoints(pointPath, (int)pointAmount);
        

        string basePathError = String.Format("Assets/Resources/ErrorData/region{0}Error/", region);
        string errorPathP = basePathError + String.Format("pErrorPercentage{0}", lod);
        float[] pErrorValues = readScalarErrorValues(errorPathP, (int)pointAmount);

        StreamWriter missingPointsWriter = new StreamWriter(basePathError + "missingPoints");

        for(int i = 0; i < pointAmount; ++i){
            if(pErrorValues[i] == 0){
                missingPointsWriter.WriteLine(points[i].x + " " + points[i].y + " " + points[i].z);
            }
        }

        missingPointsWriter.Close();
       
    }

    public float getScalarError(float calculated, float actual){
        if(actual == 0){
            actual = 1;
        }
        return (calculated - actual) / actual;
    }

    public Vector3 getVectorError(Vector3 calculated, Vector3 actual){
        float xError = getScalarError(calculated.x, actual.x);
        float yError = getScalarError(calculated.y, actual.y);
        float zError = getScalarError(calculated.z, actual.z);
        return new Vector3(xError, yError, zError);
    }

    public float getVectorMagError(Vector3 calculated, Vector3 actual){
        return getScalarError(calculated.magnitude, actual.magnitude);
    }

    public void numCells(){
        int start = 0;
        int amount = 48;

        int number_of_cells = 0;
        for(int i = start; i < start + amount; ++i){
            string base_string = String.Format("Assets/Resources/large_case/processorsASCII/processor{0}/1455/k", i);

            StreamReader reader = new StreamReader(base_string);

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
            number_of_cells += number_of_points;
        }
        Debug.Log(number_of_cells);
    }

    public void findCenter(){
        int start = 45;
        int amount = 3;
        DateTime before = DateTime.Now;
        for(int i = start; i < start + amount; ++i){
            //Debug.Log(i);
            string base_string = String.Format("Assets/Resources/large_case/processorsASCII/processor{0}/2/polyMesh/", i);
            
            string point_file_path = base_string + "points";
            string face_file_path = base_string + "faces";
            string boundary_file_path = base_string + "boundary";
            string owner_file_path = base_string + "owner";
            string neighbour_file_path = base_string + "neighbour";

            int variable_mesh_index = meshContainer.AddMesh();
            CustomMesh cMesh = meshContainer.GetMesh(variable_mesh_index);

            cMesh.ReadPointFile(point_file_path);
            cMesh.ReadFaceFile(face_file_path);
            cMesh.ReadBoundaryFile(boundary_file_path);
            cMesh.ReadOwnerFile(owner_file_path);
            cMesh.ReadNeighbourFile(neighbour_file_path);

            string cell_center_file_path = base_string + "cellCentersNew";
            cMesh.CenterOfCells(cell_center_file_path);
        }
        DateTime after = DateTime.Now; 
        TimeSpan duration = after.Subtract(before);
        Debug.Log("Finding all cell centers took in milliseconds: " + duration.TotalMilliseconds);
    }

    public void findBounds(){
        int start = 0;
        int amount = 48;
        Color[] colors = {Color.red, Color.black, Color.blue, Color.green, Color.gray, Color.cyan};
        for(int i = start; i < start + amount; ++i){
            string base_string = String.Format("Assets/Resources/large_case/processorsASCII/processor{0}/2/polyMesh/", i);
            
            string point_file_path = base_string + "points";
            ReadPointFile(point_file_path);

            // int variable_mesh_index = meshContainer.AddMesh();
            // CustomMesh cMesh = meshContainer.GetMesh(variable_mesh_index);

            //cMesh.ReadPointFile(point_file_path);

            //cMesh.drawMinMaxVer(colors[i - start+ 2], colors[i - start+ 4]);

            //DebugDraw.DrawCross(cMesh.minVertex, 100, 100, 100, colors[i - start]);
            //DebugDraw.DrawCross(cMesh.maxVertex, 100, 100, 100, colors[i - start]);

            // string cell_center_file_path = base_string + "Bounds";
            // cMesh.WriteBounds(cell_center_file_path);
        }

        Debug.Log(String.Format("The minimum point is: {0}", pc.getMinCoords()));
        Debug.Log(String.Format("The maximum point is: {0}", pc.getMaxCoords()));

        string result_base_path = "Assets/Resources/large_case/processorsComplete/";

        string bound_file_path = result_base_path + "bounds";
        StreamWriter writer = new StreamWriter(bound_file_path);

        Vector3 minVertex = pc.getMinCoords();
        Vector3 maxVertex = pc.getMaxCoords();

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

    public void splitArea(){

        string base_path = "Assets/Resources/large_case/";
        string base_path_processors = base_path + "processors400/";
        string path = base_path_processors + "bounds";
        StreamReader reader = new StreamReader(path);

        string[] minCoordString = reader.ReadLine().Split(' ');
        string[] dimString = reader.ReadLine().Split(' ');

        Vector3 minCoord = new Vector3(float.Parse(minCoordString[0]), float.Parse(minCoordString[1]), float.Parse(minCoordString[2]));
        float width = float.Parse(dimString[0]);
        float height = float.Parse(dimString[1]);
        float depth = float.Parse(dimString[2]);


        float width_split_counter = Mathf.Ceil(width / 400);
        float height_split_counter = Mathf.Ceil(height / 400);
        float depth_split_counter = Mathf.Ceil(depth / 400);

        string counterString = width_split_counter + " " + height_split_counter + " " + depth_split_counter;


        float new_width = width / width_split_counter;
        float new_height = height / height_split_counter;
        float new_depth = depth / depth_split_counter;

        string newDimString = new_width + " " + new_height + " " + new_depth;


        string processors_path = base_path_processors + "processors/";
        if(!Directory.Exists(processors_path)){
            Directory.CreateDirectory(processors_path);
        }

        string processor_dim_path = processors_path + "dimensions";
        StreamWriter writer_processor_dim = new StreamWriter(processor_dim_path);
        writer_processor_dim.WriteLine(counterString);
        writer_processor_dim.WriteLine(newDimString);
        writer_processor_dim.Close();


        Debug.Log(String.Format("The dimensions are: {0}, {1}, {2}", width, height, depth));
        Debug.Log(String.Format("The number of splits are: {0}, {1}, {2}", width_split_counter, height_split_counter, depth_split_counter));
        Debug.Log(String.Format("The new dimensions are: {0}, {1}, {2}", new_width, new_height, new_depth));


        int index = 0;
        for(int d = 0; d < depth_split_counter; ++d){
            float depth_point = minCoord.z + (new_depth * d);
            for(int h = 0; h < height_split_counter; ++h){
                float height_point = minCoord.y + (new_height * h);
                for(int w = 0; w < width_split_counter; ++w){
                    float width_point = minCoord.x + (new_width * w);

                    string folder_path = processors_path + string.Format("processor{0}/", index);
                    if(!Directory.Exists(folder_path)){
                        Directory.CreateDirectory(folder_path);
                    }

                    string file_path_bounds = folder_path + "bounds";
                    StreamWriter writer_bounds = new StreamWriter(file_path_bounds);

                    string coordsString = width_point + " " + height_point + " " + depth_point;
                    writer_bounds.WriteLine(coordsString);
                    writer_bounds.WriteLine(newDimString);

                    writer_bounds.Close();

                    index++;
                }
            }
        }
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
        //this.setNumVertices(number_of_points);

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
                pc.addPoint(point);
                //this.addVertice(i, point);
            }
            catch(FormatException e){
                Debug.Log(e);
            }
        }
        Debug.Log(String.Format("Finished reading {0} points", number_of_points));
        reader.Close();
    }

    void DrawMesh(int index, int start_face, int num_faces, int num_boundaries, char key, string obj_name = ""){
        //string face_name = "Face_" + i.ToString();
        // polyMesh.updateMesh();
        //Not sure why 'Material' cannot be added to the game object. At the same time I can't see what it does, so we can leave it out.
        //GameObject obj = new GameObject(mesh_name, typeof(Material), typeof(MeshRenderer), typeof(MeshFilter));

        CustomMesh meshToDraw = this.meshContainer.GetMesh(index);
        meshToDraw.updateMesh(start_face, num_faces, num_boundaries, key);

        string mesh_name = "Mesh_" + index.ToString();
        //GameObject obj = new GameObject(obj_name, typeof(MeshRenderer), typeof(MeshFilter), typeof(LODGroup));
        GameObject obj = new GameObject(mesh_name, typeof(MeshRenderer), typeof(MeshFilter)); 
        obj.GetComponent<Renderer>().material = this.polyMeshMaterial;
        obj.GetComponent<MeshFilter>().mesh = meshToDraw.GetMesh();
        
        
        // LODGroup lodGroup = obj.GetComponent<LODGroup>();
        // LOD lod = new LOD();
        // LOD[] lods = lodGroup.GetLODs();
        // int lod_count = lodGroup.lodCount;
        // Debug.Log(String.Format("This LOD has {0} levels", lod_count));

    }


    public void writeProcessorBounds() {
         for(int i = 0; i < 48; ++i){
            string pointsFilePath = String.Format("Assets/Resources/large_case/processorsASCII/processor{0}/2/polyMesh/points", i);
            int meshIndex = meshContainer.AddMesh();
            CustomMesh mesh_two = meshContainer.GetMesh(meshIndex);

            mesh_two.ReadPointFile(pointsFilePath);
            // mesh_two.drawMinMaxVer(Color.green, Color.red);
            
            string boundpath = String.Format("Assets/Resources/large_case/processorsASCII/processor{0}/bound", i);
            StreamWriter boundWriter = new StreamWriter(boundpath);
            boundWriter.WriteLine(mesh_two.minVertex.x.ToString() + " " + mesh_two.minVertex.y.ToString() + " " + mesh_two.minVertex.z.ToString());
            boundWriter.WriteLine(mesh_two.maxVertex.x.ToString() + " " + mesh_two.maxVertex.y.ToString() + " " + mesh_two.maxVertex.z.ToString());
            boundWriter.Close();
        }
    }




    public Vector3[] readPoints(string path, int numPoints){

        Vector3[] points = new Vector3[(int)numPoints];
        StreamReader pointReader = new StreamReader(path);
        for(int i = 0; i < numPoints; ++i){
            string[] pointParts = pointReader.ReadLine().Split(' ');
            try {
                Vector3 point = new Vector3(float.Parse(pointParts[0]), float.Parse(pointParts[1]), float.Parse(pointParts[2]));
                points[i] = point;
            }
            catch(FormatException e){
                Debug.Log(e);
            }
        }
        return points;
    }


    public float[] readScalarErrorValues(string path, int numPoints){
        float[] pErrorValues = new float[numPoints];
        StreamReader pErrorReader = new StreamReader(path);
        for(int i = 0; i < numPoints; ++i){
            try {
                pErrorValues[i] = float.Parse(pErrorReader.ReadLine());
            }
            catch(FormatException e){
                Debug.Log(e);
            }
        }
        pErrorReader.Close();
        return pErrorValues;
    }

}


//---------------------------------------OLD CODE-----------------------------------------------//


// string grabLine(ref BinaryReader binReader){
// var line = "";

// while(true){
//     byte b = binReader.ReadByte();
//     char c = (char)b;
//     line += c;

//     if(c == '\n'){
//         Debug.Log("NEW LINE");
//         break;
//     }
// }

// return line;
// }

// string grabPoint(ref BinaryReader binaryReader){
// string point = "";

// Encoding iso = Encoding.GetEncoding("ISO-8859-1");
// Encoding utf8 = Encoding.UTF8;

// byte b = binaryReader.ReadByte();
// Debug.Log(b);
// byte[] bs = {b};

// //byte[] utfBytes = iso.GetBytes(bs);
// byte[] isoBytes = Encoding.Convert(iso, utf8, bs);
// string msg = utf8.GetString(isoBytes);
// Debug.Log(msg);
// // char c = (char)i;
// // Debug.Log(c);

// return point;
// }

    // void ReadBinaryFile(){

    //     string path = "Assets/Resources/small_case/constant/polyMesh/points";
    //     BinaryReader binReader = new BinaryReader(File.Open(path, FileMode.Open));

    //     string line = "";
    //     int number_of_points = 0;
    //     while(true){
    //         line = grabLine(ref binReader);

    //         try{
    //             number_of_points = Int32.Parse(line);
    //             break;
    //         }
    //         catch(FormatException e){
    //             //Debug.Log(e);
    //             continue;
    //         }
    //     }
    //     Debug.Log(line);

    //     for(int i = 0; i < 10; ++i){
    //         line = grabPoint(ref binReader);
    //         //Debug.Log(line);
    //     }

    //     // for(int i = 0; i < 100; ++i){
    //     //     byte line = binReader.ReadByte();
    //     //     char c = (char)line;
    //     //     Debug.Log(line);
    //     //     Debug.Log(c);

    //     //     if(c == '\n'){
    //     //         Debug.Log("NEW LINE");
    //     //     }

    //     //     result += c;
    //     // }

    //     // string line2 = binReader.ReadString();
    //     // Debug.Log(line2);

    //     // string line3 = binReader.ReadString();
    //     // Debug.Log(line3);

    //     // string line4 = binReader.ReadString();
    //     // Debug.Log(line4);

    //     // string line5 = binReader.ReadString();
    //     // Debug.Log(line5);
    // }
    
