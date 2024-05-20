using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;


public class WriteVoxelValues : MonoBehaviour
{
    public int splitSize = 0;

    public int startNum = 0;
    public int amount = -1;

    public int[] depths = {};
    // Start is called before the first frame update
    void Start()
    {
        DateTime before = DateTime.Now;
        // writeVoxelValues(splitSize, startNum, amount, depths);
        DateTime after = DateTime.Now; 
        TimeSpan duration = after.Subtract(before);
        Debug.Log("Voxelization duration in milliseconds: " + duration.TotalMilliseconds);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // public static void writeVoxelValues(int split_size, int start_num, int new_amount, int[] depths)
    // {
    //     DateTime before = DateTime.Now;
    //     Debug.Log("Start writing voxel values");

    //     string base_path = "Assets/Resources/large_case/";
    //     string data_base_path = base_path + String.Format("processors{0}/", split_size);
    //     string processors_base_path = data_base_path + "processors/";

    //     if(new_amount == -1)
    //     {
    //         string num_processors_path = data_base_path + "num_processors";
    //         StreamReader num_processors_reader = new StreamReader(num_processors_path);
    //         new_amount = int.Parse(num_processors_reader.ReadLine());
    //     }

    //     string[] keys = {"epsilon", "k", "nut", "p", "phi"};
    //     //string[] keys = {"epsilon", "k"};
    //     //string[] keys = {"k"};

    //     for(int processor_num = start_num; processor_num < start_num + new_amount; ++processor_num){
    //         string file_path_processor = processors_base_path + string.Format("processor{0}/", processor_num);
        
    //         int[] input = {1};
    //         Processor processor =  new Processor(1, input);//processorObj.AddComponent<Processor>();

    //         processor.setID(processor_num);
    //         processor.setFolderPath(file_path_processor);

    //         string processor_bound_file_path = file_path_processor + string.Format("bounds");
    //         processor.readBoundary(processor_bound_file_path);

    //         string file_path_center_of_cells = processor.folderPath + string.Format("cell_centers");
    //         processor.readCellCenters(file_path_center_of_cells); //Reads the points for in the point cloud for this processor/area
            
    //         for(int k = 0; k < keys.Length; ++k)
    //         {
    //             string key = keys[k];
    //             string file_path_cell_values = processor.folderPath + string.Format("{0}/{0}Value", key);
    //             processor.readCellValues(file_path_cell_values, key); //Reads the value that should be associated with each point in the point cloud.
    //         }

    //         processor.initializeVoxel();


    //         for(int d = 0; d < depths.Length; ++d)
    //         {
    //             int depth = depths[d];

    //             for(int k = 0; k < keys.Length; ++k)
    //             {
    //                 string key = keys[k];

    //                 float[] values = processor.root.start_flatten(depth, key);
                    
    //                 string voxel_data_file_path = file_path_processor + String.Format("{0}/voxel{1}", key, depth);
    //                 StreamWriter voxel_writer = new StreamWriter(voxel_data_file_path);

    //                 // //Debug.Log(string.Format("There are {0} values", values.Length));
    //                 for(int i = 0; i < values.Length; ++i){
    //                 // for(int i = 0; i < 100; ++i){
    //                 //     Debug.Log(values[i]);
    //                     voxel_writer.WriteLine(values[i].ToString());
    //                 }

    //                 voxel_writer.Close();
    //             }
    //         }
        
    //     }

    //     Debug.Log("Finished writing voxel values");
    //     DateTime after = DateTime.Now; 
    //     TimeSpan duration = after.Subtract(before);
    //     Debug.Log("Voxelization duration in milliseconds: " + duration.TotalMilliseconds);
    // }
}
