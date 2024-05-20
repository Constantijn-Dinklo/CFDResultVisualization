using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;


public class SplitArea : MonoBehaviour
{
    public int splitSize = 0;
    // Start is called before the first frame update
    void Start()
    {
        DateTime before = DateTime.Now;
        splitArea(splitSize);
        DateTime after = DateTime.Now; 
        TimeSpan duration = after.Subtract(before);
        Debug.Log("Splitting duration in milliseconds: " + duration.TotalMilliseconds);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void splitArea(int size){

        DateTime before = DateTime.Now;
        Debug.Log("Start splitting area");

        string base_path = "Assets/Resources/large_case/";
        string bounds_path = base_path + "bounds";
        StreamReader reader = new StreamReader(bounds_path);

        string[] minCoordString = reader.ReadLine().Split(' ');
        string[] dimString = reader.ReadLine().Split(' ');

        Vector3 minCoord = new Vector3(float.Parse(minCoordString[0]), float.Parse(minCoordString[1]), float.Parse(minCoordString[2]));
        float width = float.Parse(dimString[0]);
        float height = float.Parse(dimString[1]);
        float depth = float.Parse(dimString[2]);


        int width_split_counter = Mathf.RoundToInt(width / size);
        int height_split_counter = Mathf.RoundToInt(height / size);
        int depth_split_counter = Mathf.RoundToInt(depth / size);

        int num_processors = width_split_counter * height_split_counter * depth_split_counter;

        string counterString = width_split_counter + " " + height_split_counter + " " + depth_split_counter;


        float new_width = width / width_split_counter;
        float new_height = height / height_split_counter;
        float new_depth = depth / depth_split_counter;

        string newDimString = new_width + " " + new_height + " " + new_depth;

        string base_path_processors = base_path + String.Format("processors{0}/", size);
        if(!Directory.Exists(base_path_processors)){
            Directory.CreateDirectory(base_path_processors);
        }


        string processors_path = base_path_processors + "processors/";
        if(!Directory.Exists(processors_path)){
            Directory.CreateDirectory(processors_path);
        }

        string num_processors_path = base_path_processors + "num_processors";
        StreamWriter num_processors_writer = new StreamWriter(num_processors_path);
        num_processors_writer.WriteLine(num_processors);
        num_processors_writer.Close();

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

                    string folder_path_ep = folder_path + "epsilon/";
                    if(!Directory.Exists(folder_path_ep)){
                        Directory.CreateDirectory(folder_path_ep);
                    }
                    string folder_path_k = folder_path + "k/";
                    if(!Directory.Exists(folder_path_k)){
                        Directory.CreateDirectory(folder_path_k);
                    }
                    string folder_path_nut = folder_path + "nut/";
                    if(!Directory.Exists(folder_path_nut)){
                        Directory.CreateDirectory(folder_path_nut);
                    }
                    string folder_path_p = folder_path + "p/";
                    if(!Directory.Exists(folder_path_p)){
                        Directory.CreateDirectory(folder_path_p);
                    }
                    string folder_path_phi = folder_path + "phi/";
                    if(!Directory.Exists(folder_path_phi)){
                        Directory.CreateDirectory(folder_path_phi);
                    }
                    string folder_path_U = folder_path + "U/";
                    if(!Directory.Exists(folder_path_U)){
                        Directory.CreateDirectory(folder_path_U);
                    }

                    index++;
                }
            }
        }

        Debug.Log("Finish splitting area");
        DateTime after = DateTime.Now; 
        TimeSpan duration = after.Subtract(before);
        Debug.Log("Splitting duration in milliseconds: " + duration.TotalMilliseconds);
    }
}
