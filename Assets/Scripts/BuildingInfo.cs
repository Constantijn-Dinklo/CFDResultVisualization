using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;


public static class BuildingInfo
{

    public static string inside_building_path = "";

    static Dictionary<string, Dictionary<int, int[]>> inside_building_input = new Dictionary<string, Dictionary<int, int[]>>();


    public static int[] getInsideBuildingAtDepth(string file_path, int depth)
    {
        //Check if the file_path to all processors is already in the dictionary.
        if(!inside_building_input.ContainsKey(file_path)){
            inside_building_input.Add(file_path, new Dictionary<int, int[]>());
        }
        //If we have already loaded this data, simply return that data
        if(inside_building_input[file_path].ContainsKey(depth)){
            return inside_building_input[file_path][depth];
        }

        Debug.Log("Reading building info file for depth " + depth.ToString());

        double side_length = Math.Pow(2, depth);
        double size = side_length * side_length * side_length;

        int[] new_info = new int[(int)size];

        //Read in the data from file
        //string file_name = "Assets/Resources/windAroundBuildings/constant/triSurface/insideBuilding_" + depth.ToString();
        string file_name = file_path + inside_building_path + string.Format("insideBuilding_{0}", depth);
        
        //if the file does not exist, simply fill it with 0's (not insdie a building)
        if(!System.IO.File.Exists(file_name)){
            for(int i = 0; i < new_info.Length; ++i){
                new_info[i] = 0;
            }
            inside_building_input[file_path].Add(depth, new_info);
            return new_info;
        }


        StreamReader inside_building_file = new StreamReader(file_name);
        for(int i = 0; i < new_info.Length; ++i){
            string line = inside_building_file.ReadLine();
            int value = Int32.Parse(line);
            new_info[i] = value;
        }
        inside_building_input[file_path].Add(depth, new_info);
        
        return inside_building_input[file_path][depth];
    }

}
