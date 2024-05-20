using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class PointsToArea : MonoBehaviour
{

    public int splitSize = 0;

    public int startNum = 0;
    public int amount = -1;


    // string base_path;
    // string base_path_data;
    // string base_path_processors;


    // Vector3 minCoord;

    // int num_width, num_height, num_depth;

    // float width, height, depth;

    // Start is called before the first frame update
    void Start()
    {
        // pointsToArea(splitSize, startNum, amount);
        pointsToAreaSection(splitSize, startNum, amount);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void pointsToArea(int split_size, int start_num, int OP_amount)
    {
        
        DateTime before = DateTime.Now;
        Debug.Log("Start points to area");
        Dictionary<int, List<Point>> processorPoints = new Dictionary<int, List<Point>>();

        string base_path = "Assets/Resources/large_case/";
        string base_path_data = base_path + String.Format("processors{0}/", split_size);
        string base_path_processors = base_path_data + "processors/";

        string data_bounds = base_path + "bounds";
        StreamReader reader = new StreamReader(data_bounds);

        string[] minCoordString = reader.ReadLine().Split(' ');

        Vector3 minCoord = new Vector3(float.Parse(minCoordString[0]), float.Parse(minCoordString[1]), float.Parse(minCoordString[2]));
        //Debug.Log(String.Format("The min vertex is: {0}", minCoord));

        string processors_dimensions_path = base_path_processors + "dimensions";
        StreamReader dim_reader = new StreamReader(processors_dimensions_path);
        string[] counterString = dim_reader.ReadLine().Split(' ');
        string[] dimString = dim_reader.ReadLine().Split(' ');

        int num_width = int.Parse(counterString[0]);
        int num_height = int.Parse(counterString[1]);
        int num_depth = int.Parse(counterString[2]);

        float width = float.Parse(dimString[0]);
        float height = float.Parse(dimString[1]);
        float depth = float.Parse(dimString[2]);


        string processors_base_path = base_path + "processorsASCII/";

        if(OP_amount == -1)
        {
            string num_processors_path = processors_base_path + "num_processors";
            StreamReader num_processors_reader = new StreamReader(num_processors_path);
            OP_amount = int.Parse(num_processors_reader.ReadLine());
        }

        //Debug.Log(OP_amount);

        string[] keys = {"epsilon", "k", "nut", "p", "phi", "U"};
        //string[] keys = {"k"};

        
        for(int i = start_num; i < start_num + OP_amount; ++i)
        {

            string cell_center_fp = processors_base_path + String.Format("processor{0}/2/polyMesh/cellCenters", i);
            StreamReader cell_center_reader = new StreamReader(cell_center_fp, true);
            Dictionary<string, StreamReader> input_files = new Dictionary<string, StreamReader>();
            for(int k = 0; k < keys.Length; ++k)
            {
                string value_file_path = processors_base_path + String.Format("processor{0}/1455/{1}Plain", i, keys[k]);
                StreamReader value_writer = new StreamReader(value_file_path, true);
                input_files[keys[k]] = value_writer;
            }

            while(  (cell_center_reader.Peek() != -1) &&
                    (input_files["epsilon"].Peek() != -1) && 
                    (input_files["k"].Peek() != -1) &&
                    (input_files["nut"].Peek() != -1) &&
                    (input_files["p"].Peek() != -1) &&
                    (input_files["phi"].Peek() != -1) &&
                    (input_files["U"].Peek() != -1)){
            
                    string[] parts = cell_center_reader.ReadLine().Split(' ');
                    Vector3 coords = new Vector3(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]));
                    Point p = new Point(coords);
            
                    for(int k = 0; k < keys.Length; ++k)
                    {
                        string cell_value_string = input_files[keys[k]].ReadLine();
                        if(keys[k] == "U"){
                            p.UValue = cell_value_string;
                        }
                        else{
                            float cell_value = float.Parse(cell_value_string);
                            p.setValue(keys[k], cell_value);
                        }
                       
                    }

                    int index = determineProcessorNum(p.position, minCoord, width, height, depth, num_width, num_height, num_depth);
                    if(!processorPoints.ContainsKey(index)){
                        processorPoints[index] = new List<Point>();
                    }
                    processorPoints[index].Add(p);
            }

            for(int k = 0; k < keys.Length; ++k)
            {
                input_files[keys[k]].Close();
            }
        }
        // Debug.Log(String.Format("The dictionary has {0} keys", processorPoints.Keys.Count));

        // DateTime after = DateTime.Now; 
        // TimeSpan duration = after.Subtract(before);
        // Debug.Log("PtA duration in milliseconds: " + duration.TotalMilliseconds);

        //return;

        //string[] keys = {"epsilon", "k", "nut", "p", "phi", "U"};

        foreach(KeyValuePair<int, List<Point>> entry in processorPoints)
        {
            int index = entry.Key;
            List<Point> points = entry.Value;

            string processor_folder_path = base_path_processors + String.Format("processor{0}/", index);
            
            string cell_center_file_path_p = processor_folder_path + "cell_centers";
            StreamWriter cell_center_writer = new StreamWriter(cell_center_file_path_p, true);

            Dictionary<string, StreamWriter> files = new Dictionary<string, StreamWriter>();
            for(int k = 0; k < keys.Length; ++k)
            {
                string value_file_path = processor_folder_path + String.Format("{0}/{0}Value", keys[k]);
                //string value_file_path = processor_folder_path + String.Format("{0}Value", keys[k]);
                StreamWriter value_writer = new StreamWriter(value_file_path, true);
                files[keys[k]] = value_writer;
            }

            for(int i = 0; i < points.Count; ++i){
                Point p = points[i];

                string coord_string = p.position[0] + " " + p.position[1] + " " + p.position[2];
                cell_center_writer.WriteLine(coord_string);

                for(int k = 0; k < keys.Length; ++k)
                {
                    if(keys[k] == "U"){
                        files[keys[k]].WriteLine(p.UValue);
                    }
                    else{
                        files[keys[k]].WriteLine(p.getValue(keys[k]));
                    }
                }
            }

            cell_center_writer.Close();
            for(int k = 0; k < keys.Length; ++k)
            {
                files[keys[k]].Close();
            }

        }
        Debug.Log("Finish points to area");

        DateTime after = DateTime.Now; 
        TimeSpan duration = after.Subtract(before);
        Debug.Log("PtA duration in milliseconds: " + duration.TotalMilliseconds);
    }

    public static void pointsToAreaSection(int split_size, int start_num, int OP_amount)
    {
        
        DateTime before = DateTime.Now;
        Debug.Log("Start points to area");
        Dictionary<int, List<Point>> processorPoints = new Dictionary<int, List<Point>>();

        string base_path = "Assets/Resources/large_case/";
        string base_path_data = base_path + String.Format("processors{0}/", split_size);
        string base_path_processors = base_path_data + "processors/";

        string data_bounds = base_path + "bounds";
        StreamReader reader = new StreamReader(data_bounds);
 
        string[] minCoordString = reader.ReadLine().Split(' ');

        Vector3 minCoord = new Vector3(float.Parse(minCoordString[0]), float.Parse(minCoordString[1]), float.Parse(minCoordString[2]));
        //Debug.Log(String.Format("The min vertex is: {0}", minCoord));

        string processors_dimensions_path = base_path_processors + "dimensions";
        StreamReader dim_reader = new StreamReader(processors_dimensions_path);
        string[] counterString = dim_reader.ReadLine().Split(' ');
        string[] dimString = dim_reader.ReadLine().Split(' ');

        int num_width = int.Parse(counterString[0]);
        int num_height = int.Parse(counterString[1]);
        int num_depth = int.Parse(counterString[2]);

        float width = float.Parse(dimString[0]);
        float height = float.Parse(dimString[1]);
        float depth = float.Parse(dimString[2]);


        string processors_base_path = base_path + "processorsASCII/";

        if(OP_amount == -1)
        {
            string num_processors_path = processors_base_path + "num_processors";
            StreamReader num_processors_reader = new StreamReader(num_processors_path);
            OP_amount = int.Parse(num_processors_reader.ReadLine());
        }

        //Debug.Log(OP_amount);

        string[] keys = {"epsilon", "k", "nut", "p", "phi", "U"};
        //string[] keys = {"k"};

        
        for(int a = start_num; a < 48; ++a)
        {
            start_num = a;
            Debug.Log("Starting a set of 5");
            DateTime before_5 = DateTime.Now;
            processorPoints = new Dictionary<int, List<Point>>();
            
            for(int i = start_num; (i < start_num + OP_amount) && (i < 48); ++i)
            {
                string cell_center_fp = processors_base_path + String.Format("processor{0}/2/polyMesh/cellCenters", i);
                StreamReader cell_center_reader = new StreamReader(cell_center_fp, true);
                Dictionary<string, StreamReader> input_files = new Dictionary<string, StreamReader>();
                for(int k = 0; k < keys.Length; ++k)
                {
                    string value_file_path = processors_base_path + String.Format("processor{0}/1455/{1}Plain", i, keys[k]);
                    StreamReader value_writer = new StreamReader(value_file_path, true);
                    input_files[keys[k]] = value_writer;
                }

                while(  (cell_center_reader.Peek() != -1) &&
                        (input_files["epsilon"].Peek() != -1) && 
                        (input_files["k"].Peek() != -1) &&
                        (input_files["nut"].Peek() != -1) &&
                        (input_files["p"].Peek() != -1) &&
                        (input_files["phi"].Peek() != -1) &&
                        (input_files["U"].Peek() != -1)){
                
                        string[] parts = cell_center_reader.ReadLine().Split(' ');
                        Vector3 coords = new Vector3(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]));
                        Point p = new Point(coords);
                
                        for(int k = 0; k < keys.Length; ++k)
                        {
                            string cell_value_string = input_files[keys[k]].ReadLine();
                            if(keys[k] == "U"){
                                p.UValue = cell_value_string;
                            }
                            else{
                                float cell_value = float.Parse(cell_value_string);
                                p.setValue(keys[k], cell_value);
                            }
                        
                        }

                        int index = determineProcessorNum(p.position, minCoord, width, height, depth, num_width, num_height, num_depth);
                        if(!processorPoints.ContainsKey(index)){
                            processorPoints[index] = new List<Point>();
                        }
                        processorPoints[index].Add(p);
                }

                cell_center_reader.Close();
                for(int k = 0; k < keys.Length; ++k)
                {
                    input_files[keys[k]].Close();
                }
                a = i;
            }

            int point_counter = 0;
            foreach(KeyValuePair<int, List<Point>> entry in processorPoints)
            {
                int index = entry.Key;
                List<Point> points = entry.Value;

                string processor_folder_path = base_path_processors + String.Format("processor{0}/", index);
                
                string cell_center_file_path_p = processor_folder_path + "cell_centers";
                StreamWriter cell_center_writer = new StreamWriter(cell_center_file_path_p, true);

                Dictionary<string, StreamWriter> files = new Dictionary<string, StreamWriter>();
                for(int k = 0; k < keys.Length; ++k)
                {
                    string value_file_path = processor_folder_path + String.Format("{0}/{0}Value", keys[k]);
                    //string value_file_path = processor_folder_path + String.Format("{0}Value", keys[k]);
                    StreamWriter value_writer = new StreamWriter(value_file_path, true);
                    files[keys[k]] = value_writer;
                }

                for(int i = 0; i < points.Count; ++i){
                    Point p = points[i];

                    string coord_string = p.position[0] + " " + p.position[1] + " " + p.position[2];
                    cell_center_writer.WriteLine(coord_string);

                    for(int k = 0; k < keys.Length; ++k)
                    {
                        if(keys[k] == "U"){
                            files[keys[k]].WriteLine(p.UValue);
                        }
                        else{
                            files[keys[k]].WriteLine(p.getValue(keys[k]));
                        }
                    }
                    point_counter++;
                }

                cell_center_writer.Close();
                for(int k = 0; k < keys.Length; ++k)
                {
                    files[keys[k]].Close();
                }

            }
        
            Debug.Log(String.Format("Number of points: {0}", point_counter));
            DateTime after_5 = DateTime.Now; 
            TimeSpan duration_5 = after_5.Subtract(before_5);
            Debug.Log("Set of 5 duration: " + duration_5.TotalMilliseconds);
        }
        Debug.Log("Finish points to area");

        DateTime after = DateTime.Now; 
        TimeSpan duration = after.Subtract(before);
        Debug.Log("PtA duration in milliseconds: " + duration.TotalMilliseconds);
    }

    public static int determineProcessorNum(Vector3 coord, Vector3 minCoord, float width, float height, float depth, int num_width, int num_height, int num_depth)
    {
        int width_index = Mathf.FloorToInt((coord.x - minCoord.x) / width);
        int height_index = Mathf.FloorToInt((coord.y - minCoord.y) / height);
        int depth_index = Mathf.FloorToInt((coord.z - minCoord.z) / depth);

        int index = width_index + (height_index * num_width) + (depth_index * (num_width * num_height));

        return index;
    }
}
