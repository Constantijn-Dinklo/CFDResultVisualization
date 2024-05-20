using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

using UnityVolumeRendering;


public class Data : MonoBehaviour
{
    public Camera cam;


    public int splitSize = 0;

    public int startNum = 0;

    public int amount = -1;



    public DataType dataOption = DataType.P;

    private string dataOptionStr = "p";



    public int numDepths = 0;
    public int[] regionDepths = {};


    //Scalar Options
    public float[] transitionDepths = {};
    public float[] depthLoadRatioScalar = {};
    public float[] depthDestroyRatioScalar = {};



    //Vector Options
    public float[] depthLoadRatioVector = {};
    public float[] depthViewRatioVector = {};
    public float[] depthDestroyRatioVector = {};




    public Color[] depthColor = {};

    public Dictionary<int, Dictionary<int, VolumeRenderedObject>> regionVolumes;

    public GameObject arrowPrefab;
    public GameObject depth5Prefab;
    // public GameObject windPrefab;

    Dictionary<int, GameObject> regionObjects;
    List<Processor> regions;

    bool destroying = false;



    // Start is called before the first frame update
    void Start()
    {
        regionVolumes = new Dictionary<int, Dictionary<int, VolumeRenderedObject>>();

        regionObjects = new Dictionary<int, GameObject>();
        regions = new List<Processor>();

        string base_path = "Assets/Resources/Case_One/";

        Debug.Log(dataOption);
        DataConfig.setDataType(dataOption);
        dataOptionStr = DataConfig.dataTypeToString(dataOption);        

        readBounds(base_path + "bounds");

        string data_base_path = base_path + String.Format("region{0}/", splitSize);
        string processors_base_path = data_base_path + "regions/";

        //Determine the number of processors
        string processors_dim_file_path = data_base_path + "dimensions";
        StreamReader dim_reader = new StreamReader(processors_dim_file_path);
        
        string[] num_processors_string = dim_reader.ReadLine().Split(' ');

        int num_processors_width = int.Parse(num_processors_string[0]);
        int num_processors_height = int.Parse(num_processors_string[1]);
        int num_processors_depth = int.Parse(num_processors_string[2]);

        int num_processors = num_processors_width * num_processors_height * num_processors_depth;
        Debug.Log(num_processors);

        dim_reader.Close();

        if(amount == -1)
        {
            amount = num_processors;
        }

        for(int regionNum = startNum; regionNum < startNum + amount; ++regionNum){

            GameObject regionObj = new GameObject(String.Format("Region_{0}", regionNum));
            
            Processor region = regionObj.AddComponent<Processor>();
            region.initialize(regionNum, numDepths, regionDepths);
            
            string file_path_processor = processors_base_path + string.Format("region{0}/", regionNum);
            region.setFolderPath(file_path_processor);

            string region_bound_file_path = file_path_processor + string.Format("bounds");
            region.readBoundary(region_bound_file_path);

            regions.Add(region);
            regionVolumes[regionNum] = new Dictionary<int, VolumeRenderedObject>();


            regionObj.AddComponent<LODGroup>();
            regionObj.transform.position = region.bounds.getCenter();

            regionObjects[regionNum] = regionObj;
        }


        EditVolumeGUI.ShowWindowAll(dataOption);
    }

    // Update is called once per frame
    void Update()
    {
        // return;
        if(destroying){
            return;
        }

        if(VisibilityData.destroy){
            for(int regionNumber = 0; regionNumber < regions.Count; ++regionNumber){
                Processor region = regions[regionNumber];
                region.destroy();
            }
            destroying = true;
        }
        
        Vector3 camPos = cam.transform.position;
        Vector3 camRot = cam.transform.forward;
        
        // Debug.Log(regions.Count);
        //Loop over all regions
        for(int regionNumber = 0; regionNumber < regions.Count; ++regionNumber){
            Processor region = regions[regionNumber];
            Vector3 areaCoord = region.processorCenter;
            
            Vector3 targetDir = areaCoord - camPos;
            double sqrDistance = targetDir.sqrMagnitude;

            // float angle = Vector3.Angle(targetDir, camRot);
            // if(angle < 60) {

            VoxelBoundary regionBounds = region.bounds;
            float sizeSqr = regionBounds.sizeSqr();

            region.updateCurrentDepth(0);
            for(int depthIndex = numDepths - 1; depthIndex >= 0; --depthIndex){
                int depth = regionDepths[depthIndex];

                if(dataOptionStr == "p"){

                    if(sqrDistance < sizeSqr * depthLoadRatioScalar[depthIndex]){
                        region.updateCurrentDepth(depth);
                        // DebugDraw.DrawBox(regionBounds.startCoord, regionBounds.width, regionBounds.height, regionBounds.depth, depthColor[depthIndex]);
                        // DebugDraw.DrawCross(region.processorCenter, 100, 100, 100, depthColor[depthIndex], 1);

                        if(!region.scalarDataLoaded[dataOptionStr][depth] && !region.scalarDataLoading[dataOptionStr][depth]){
                            StartCoroutine(region.readVoxelDataAsync("p", regionDepths[depthIndex]));
                        }

                        if(region.scalarDataLoaded[dataOptionStr][depth] && (!region.scalarDataAssigned[dataOptionStr][depth] || DataStatistics.dataChanged)){
                            region.setDataForLODGroup(numDepths, regionDepths, depth, transitionDepths, regionObjects[region.id]);
                        }
                    }

                    // if(sqrDistance > sizeSqr * depthDestroyRatioScalar[depthIndex]){
                    //     if(region.scalarDataLoaded[dataOptionStr][depth]){
                    //         region.freeRegionScalar(depth);
                    //     }
                    // }
                }
                else if(dataOptionStr == "U"){


                    if(sqrDistance < sizeSqr * depthLoadRatioVector[depthIndex]){
                        // DebugDraw.DrawBox(regionBounds.startCoord, regionBounds.width, regionBounds.height, regionBounds.depth, depthColor[depthIndex]);
                        // DebugDraw.DrawCross(region.processorCenter, 100, 100, 100, depthColor[depthIndex], 1);
                        if(!region.vectorDataLoaded[dataOptionStr][depth] && !region.vectorDataLoading[dataOptionStr][depth]){
                            StartCoroutine(region.readVectorValues(dataOptionStr, regionDepths[depthIndex]));
                        }
                        if(region.vectorDataLoaded[dataOptionStr][depth]){
                            if(!region.vectorGameObjectCreated[dataOptionStr][depth] && !region.vectorGameObjectCreating[dataOptionStr][depth]){
                                StartCoroutine(region.createArrowForDepth(dataOptionStr, depth, arrowPrefab, regionObjects[region.id], depth5Prefab));
                            }
                        }
                    }

                    if(sqrDistance <  sizeSqr * depthViewRatioVector[depthIndex]){
                        region.updateCurrentDepth(depth);
                    }

                    if(sqrDistance > sizeSqr * depthDestroyRatioVector[depthIndex]){
                        if(region.vectorDataLoaded[dataOptionStr][depth]){
                            region.freeRegionVector(depth);
                        }
                    }
                }

            }
            DataStatistics.dataChanged = false;
        }
    }


    public void readBounds(string file_path){

        StreamReader reader = new StreamReader(file_path);

        string[] minCoordString = reader.ReadLine().Split(' ');
        string[] dimString = reader.ReadLine().Split(' ');

        Vector3 minCoord = new Vector3(float.Parse(minCoordString[0]), float.Parse(minCoordString[1]), float.Parse(minCoordString[2]));
        float width = float.Parse(dimString[0]);
        float height = float.Parse(dimString[1]);
        float depth = float.Parse(dimString[2]);

        DebugDraw.DrawBox(minCoord, width, height, depth, Color.white, 1000);
    }


}
