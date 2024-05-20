using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;

using UnityVolumeRendering;


public class Processor : MonoBehaviour
{

    public int id;

    public string folderPath;

    public Vector3 processorCenter;

    public VoxelBoundary bounds;

    int currentVisibleDepth;


    //Scalar Variables
    public Dictionary<int, VolumeRenderedObject> regionVolumes;
    Dictionary<string, Dictionary<int, float[]>> scalarVoxelData;


    public Dictionary<string, Dictionary<int, bool>> scalarDataLoaded;
    public Dictionary<string, Dictionary<int, bool>> scalarDataLoading;
    public Dictionary<string, Dictionary<int, bool>> scalarDataAssigned;




    //Vector Variables
    Dictionary<int, GameObject> depthGameObjects;
    Dictionary<string, Dictionary<int, Vector3[]>> vectorVoxelData;
    
    public Dictionary<string, Dictionary<int, bool>> vectorDataLoaded;
    public Dictionary<string, Dictionary<int, bool>> vectorDataLoading;
    public Dictionary<string, Dictionary<int, bool>> vectorGameObjectCreated;
    public Dictionary<string, Dictionary<int, bool>> vectorGameObjectCreating;


    Dictionary<int, List<ArrowScript>> arrows;

    public void initialize(int id, int numLODs, int[] depths){
        this.id = id;

        
        regionVolumes = new Dictionary<int, VolumeRenderedObject>();
        depthGameObjects = new Dictionary<int, GameObject>();

        scalarDataLoaded = new Dictionary<string, Dictionary<int,bool>>();
        scalarDataLoading = new Dictionary<string, Dictionary<int,bool>>();
        scalarDataAssigned = new Dictionary<string, Dictionary<int,bool>>();

        
        vectorDataLoaded = new Dictionary<string, Dictionary<int, bool>>();
        vectorDataLoading = new Dictionary<string, Dictionary<int, bool>>();
        vectorGameObjectCreated = new Dictionary<string, Dictionary<int, bool>>();
        vectorGameObjectCreating = new Dictionary<string, Dictionary<int, bool>>();
        
        
        string[] keys = {"p", "U"};
        for(int k = 0; k < keys.Length; ++k){

            scalarDataLoaded[keys[k]] = new Dictionary<int, bool>();
            scalarDataLoading[keys[k]] = new Dictionary<int, bool>();
            scalarDataAssigned[keys[k]] = new Dictionary<int, bool>();

            vectorDataLoaded[keys[k]] = new Dictionary<int, bool>();
            vectorDataLoading[keys[k]] = new Dictionary<int, bool>();
            vectorGameObjectCreated[keys[k]] = new Dictionary<int, bool>();
            vectorGameObjectCreating[keys[k]] = new Dictionary<int, bool>();

            for(int i = 0; i < numLODs; ++i){
                scalarDataLoaded[keys[k]].Add(depths[i], false);
                scalarDataLoading[keys[k]].Add(depths[i], false);
                scalarDataAssigned[keys[k]].Add(depths[i], false);

                vectorDataLoaded[keys[k]].Add(depths[i], false);
                vectorDataLoading[keys[k]].Add(depths[i], false);
                vectorGameObjectCreated[keys[k]].Add(depths[i], false);
                vectorGameObjectCreating[keys[k]].Add(depths[i], false);
            }



        }
        
        scalarVoxelData = new Dictionary<string, Dictionary<int, float[]>>();
        vectorVoxelData = new Dictionary<string, Dictionary<int, Vector3[]>>();

        arrows = new Dictionary<int, List<ArrowScript>>();
    }

    void Update(){
        foreach(KeyValuePair<int,GameObject> depthGameObject in depthGameObjects)
        {
            int depth = depthGameObject.Key;
            if(depth == currentVisibleDepth){
                depthGameObject.Value.SetActive(true);
            }
            else {
                depthGameObject.Value.SetActive(false);
            }
        }

        if(arrows.ContainsKey(currentVisibleDepth)){
            for(int i = 0; i < arrows[currentVisibleDepth].Count; ++i){
                ArrowScript arrow = arrows[currentVisibleDepth][i];

                if(VisibilityData.isVisible(arrow.getValue())){
                    arrow.gameObject.SetActive(true);
                    arrow.transform.localScale = new Vector3(VisibilityData.scaleValue * arrow.getValue().magnitude, VisibilityData.scaleValue * arrow.getValue().magnitude, VisibilityData.scaleValue * arrow.getValue().magnitude);
                }
                else {
                    arrow.gameObject.SetActive(false);
                }
            }
        }
    }

    public void setFolderPath(string folder_path){
        this.folderPath = folder_path;
    }

    public void readBoundary(string path)
    {
        StreamReader reader = new StreamReader(path);

        string[] minCoordString = reader.ReadLine().Split(' ');
        string[] dimString = reader.ReadLine().Split(' ');

        Vector3 minCoord = new Vector3(float.Parse(minCoordString[0]), float.Parse(minCoordString[1]), float.Parse(minCoordString[2]));
        float width = float.Parse(dimString[0]);
        float height = float.Parse(dimString[1]);
        float depth = float.Parse(dimString[2]);

        this.bounds = new VoxelBoundary(minCoord, width, height, depth);
        this.processorCenter = this.bounds.getCenter();
        
    }

    public bool hasData(string key, int depth){
        if(scalarVoxelData.ContainsKey(key) && scalarVoxelData[key].ContainsKey(depth)){
            return true;
        }
        return false;
    }
    
    public void updateCurrentDepth(int depth){
        currentVisibleDepth = depth;
    }
    
    
    
    
    public IEnumerator readVoxelDataAsync(string key, int depth){

        this.scalarDataLoading[key][depth] = true;
        double side_length = Math.Pow(2, depth);
        double size = side_length * side_length * side_length;

        float[] voxel_values = new float[(int)size];

        string filePath = folderPath + string.Format("{0}/voxel{1}", key, depth);
        using (FileStream fileStream = File.OpenRead(filePath))
        {
            byte[] data = new byte[fileStream.Length];
            int bytesRead = fileStream.Read(data, 0, data.Length);

            int floatCount = data.Length / 8;
            // Debug.Log(floatCount);
            for (int i = 0; i < floatCount; i++)
            {
                voxel_values[i] = (float)BitConverter.ToDouble(data, i * 8);
                if(voxel_values[i] < DataStatistics.getMinValue(key))
                {
                    DataStatistics.setMinValue(key, voxel_values[i]);
                }
                if(voxel_values[i] > DataStatistics.getMaxValue(key))
                {
                    DataStatistics.setMaxValue(key, voxel_values[i]);
                }
            }
            fileStream.Close();
        }


        // Rendering error values
        // StreamReader errorReader = new StreamReader(String.Format("Assets/Resources/ErrorData/region{0}Error/pErrorPercentage{1}", id, depth));
        // // StreamReader errorReader = new StreamReader(String.Format("Assets/Resources/ErrorData/region{0}Error/UErrorMagPercentage{1}", id, depth));
        // for(int i = 0; i < size; ++i){
        //     try {
        //         voxel_values[i] = Math.Abs(float.Parse(errorReader.ReadLine()));
        //         // voxel_values[i] = float.Parse(errorReader.ReadLine());
        //         if(voxel_values[i] == 1000000){
        //             voxel_values[i] = 0;
        //         }
        //         // Debug.Log(voxel_values[i]);
        //         if(voxel_values[i] < DataStatistics.getMinValue(key))
        //         {
        //             DataStatistics.setMinValue(key, voxel_values[i]);
        //         }
        //         if(voxel_values[i] > DataStatistics.getMaxValue(key))
        //         {
        //             DataStatistics.setMaxValue(key, voxel_values[i]);
        //         }
        //     }
        //     catch(FormatException e){
        //         Debug.Log(e);
        //     }
        // }
        // errorReader.Close();

        int int_side_length = (int)side_length;
        for(int d = 0; d < side_length; ++d){
            for(int h = 0; h < side_length; ++h){
                for(int w = 0; w < side_length; ++w){
                    int oldIndex = w + h * int_side_length + d * (int_side_length * int_side_length);
                    int newIndex = d + h * int_side_length + w * (int_side_length * int_side_length);

                    float temp = voxel_values[oldIndex];
                    voxel_values[oldIndex] = voxel_values[newIndex];
                    voxel_values[newIndex] = temp;
                }
            }
        }

        if(!scalarVoxelData.ContainsKey(key)){
            scalarVoxelData[key] = new Dictionary<int, float[]>();
        }
        scalarVoxelData[key][depth] = voxel_values;

        this.scalarDataLoading[key][depth] = false;
        this.scalarDataLoaded[key][depth] = true;
        yield return null;
    }
    
    public float[] getVoxelData(string key, int depth){
        if(!scalarVoxelData.ContainsKey(key) || !scalarVoxelData[key].ContainsKey(depth)){
            Debug.Log(String.Format("Data {0} at depth {1} is not yet available for processor {2}", key, depth, id.ToString()));
        }
        return scalarVoxelData[key][depth];
    }
    
    public VolumeDataset createDatasetAtDepth(int depth) {
        //We first flatten the voxel structure to the depth required
        float[] values = getVoxelData("p", depth);
        int num_voxels_per_side = (int)Math.Pow(2, depth);

        // for(int i = 0; i < values.Length; ++i){
        //     Debug.Log(values[i]);
        // }

        //Make the dataset from which the rendering will happen.
        VolumeDataset dataset = new VolumeDataset();

        dataset.datasetName = depth.ToString();
        dataset.dimX = num_voxels_per_side;
        dataset.dimY = num_voxels_per_side;
        dataset.dimZ = num_voxels_per_side;
        // dataset.dataDimX = Mathf.FloorToInt(processor.root.getBoundary().width);
        // dataset.dataDimY = Mathf.FloorToInt(processor.root.getBoundary().height);
        // dataset.dataDimZ = Mathf.FloorToInt(processor.root.getBoundary().depth);
        dataset.dataScaleX = bounds.width;
        dataset.dataScaleY = bounds.height;
        dataset.dataScaleZ = bounds.depth;
        dataset.data = values;

        return dataset;     
    }
    
    public void setDataForLODGroup(int numVolumes, int[] volumeDepths, int curDepth, float[] transitionDepths, GameObject parentObj){

        // GameObject lodGroupObj = regionObjects[processor.id];

        // //Set up the number of volumes we want to show and the depths of those volumes
        VolumeRenderedObject[] volRenObj = new VolumeRenderedObject[numVolumes];

        //Creating rendering objects
        int numDepthData = 0;
        for(int i = 0; i < numVolumes; ++i){
            int depth = volumeDepths[i];
            if(hasData("p", depth)){

                VolumeRenderedObject volObj;
                if(regionVolumes.ContainsKey(depth)){
                    // Debug.Log("Updated Region texture");
                    volObj = regionVolumes[depth];
                    volObj.meshRenderer.sharedMaterial.SetTexture("_DataTex", volObj.dataset.GetDataTexture(true));
                }
                else{
                    VolumeDataset dataset = createDatasetAtDepth(depth);
                    volObj = VolumeObjectFactory.CreateObject(dataset);
                    volObj.transform.position = parentObj.transform.position;
                    volObj.transform.parent = parentObj.transform;
                    regionVolumes[depth] = volObj;
                    EditVolumeGUI.addTargetObject(volObj);
                }
                volRenObj[i] = volObj;
                numDepthData++;
            }
        }

        //Setup the lod regions
        LOD[] lods = new LOD[numDepthData];
        int currentLOD = 0;
        for(int i = 0; i < numVolumes; ++i){
            int depth = volumeDepths[i];
            if(hasData("p", depth)){ 
                Renderer[] renderers = new Renderer[1];
                renderers[0] = volRenObj[i].meshRenderer;

                lods[currentLOD] = new LOD(transitionDepths[i], renderers);
                currentLOD++;
            }
        }

        LODGroup lodGroup = parentObj.GetComponent<LODGroup>();

        lodGroup.SetLODs(lods);
        lodGroup.RecalculateBounds();

        // lodGroup.transform.position = processor.bounds.getCenter();
        scalarDataLoaded["p"][curDepth] = true;
        scalarDataAssigned["p"][curDepth] = true;
        return;
    }


    public void freeRegionScalar(int depth){
        Debug.Log(String.Format("Destroying region {0} scalar at depth {1}", id, depth));

        GameObject.Destroy(regionVolumes[depth].gameObject);
        regionVolumes.Remove(depth);

        scalarVoxelData["p"].Remove(depth);

        scalarDataLoaded["p"][depth] = false;
        scalarDataAssigned["p"][depth] = false;
    }


    //Vector Functions
    public IEnumerator readVectorValues(string key, int depth){
        vectorDataLoading[key][depth] = true;

        double sideLength = Math.Pow(2, depth);
        double size = sideLength * sideLength * sideLength;

        Vector3[] voxelValues = new Vector3[(int)size];

        string filePath = folderPath + string.Format("{0}/voxel{1}", key, depth);
        using (FileStream fileStream = File.OpenRead(filePath))
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
                DataStatisticsVector.updateValue(key, value);
                voxelValues[counter] = value;
                counter++;
            }
            fileStream.Close();
        }

        int intSideLength = (int)sideLength;
        for(int d = 0; d < sideLength; ++d){
            for(int h = 0; h < sideLength; ++h){
                for(int w = 0; w < sideLength; ++w){
                    int oldIndex = w + h * intSideLength + d * (intSideLength * intSideLength);
                    int newIndex = d + h * intSideLength + w * (intSideLength * intSideLength);

                    Vector3 temp = voxelValues[oldIndex];
                    voxelValues[oldIndex] = voxelValues[newIndex];
                    voxelValues[newIndex] = temp;
                }
            }
        }


        if(!vectorVoxelData.ContainsKey(key)){
            vectorVoxelData[key] = new Dictionary<int, Vector3[]>();
        }
        vectorVoxelData[key][depth] = voxelValues;

        vectorDataLoading[key][depth] = false;
        vectorDataLoaded[key][depth] = true;
        yield return null;
    }

    public IEnumerator createArrowForDepth(string key, int depth, GameObject prefab, GameObject parentObj, GameObject depth5Prefab){
        vectorGameObjectCreating[key][depth] = true;

        if(!depthGameObjects.ContainsKey(depth)){
            GameObject gameObject;
            if(depth == 5){
                gameObject = GameObject.Instantiate(depth5Prefab);
            }
            else {
                gameObject = new GameObject(String.Format("Depth_{0}", depth));
            }
            gameObject.transform.parent = parentObj.transform;

            depthGameObjects[depth] = gameObject;
        }
        arrows[depth] = new List<ArrowScript>();

        GameObject depthObj = depthGameObjects[depth];

        int sideLength = (int)Math.Pow(2, depth);

        float voxelWidth = bounds.width / sideLength;
        float voxelHeight = bounds.height / sideLength;
        float voxelDepth = bounds.depth / sideLength;

        Vector3[] voxelValues = vectorVoxelData["U"][depth];
        

        Vector3 startPos = new Vector3(bounds.startCoord.x +  voxelWidth / 2, bounds.startCoord.y + voxelHeight / 2, bounds.startCoord.z + voxelDepth / 2);
        int counter = 0;
        for(int d = 0; d < sideLength; ++d){
            float curZ = startPos.z + voxelDepth * d;
            for(int h = 0; h < sideLength; ++h){
                float curY = startPos.y + voxelHeight * h;
                for(int w = 0; w < sideLength; ++w){
                    float curX = startPos.x + voxelWidth * w;
                    Vector3 currentPos = new Vector3(curX, curY, curZ);
                    Vector3 currentValue = voxelValues[counter];

                    GameObject windObject;
                    if(depth == 5){
                        windObject = depthObj.transform.GetChild(counter).gameObject;
                    }
                    else {
                        windObject = GameObject.Instantiate(prefab);
                    }
                     
                    windObject.transform.position = currentPos;
                    windObject.transform.rotation = Quaternion.LookRotation(currentValue.normalized);
                    windObject.transform.parent = depthObj.transform;

                    ArrowScript arrowScript = windObject.GetComponent<ArrowScript>();
                    arrowScript.setValue(currentValue);

                    arrows[depth].Add(arrowScript);

                    counter++;
                }
            }  
        }
        // }
        

        vectorGameObjectCreating[key][depth] = false;
        vectorGameObjectCreated[key][depth] = true;
        yield return null;
    }

    
    public void freeRegionVector(int depth){
        Debug.Log(String.Format("Destroying region {0} vector at depth {1}", id, depth));
        
        if(arrows.ContainsKey(depth)){
            List<ArrowScript> arrowList = arrows[depth];
            for(int i = 0; i < arrowList.Count; ++i){
                GameObject.Destroy(arrowList[i].gameObject);
            }
            arrows[depth] = new List<ArrowScript>();
        }

        vectorVoxelData["U"][depth] = new Vector3[0];

        vectorDataLoaded["U"][depth] = false;
        vectorGameObjectCreated["U"][depth] = false;
    }




    public void destroy(){
        foreach(KeyValuePair<int,List<ArrowScript>> arrowsDepth in arrows)
        {
            List<ArrowScript> arrowList = arrowsDepth.Value;
            for(int i = 0; i < arrowList.Count; ++i){
                GameObject.Destroy(arrowList[i].gameObject);
            }
        }
    }


    public void spawnParticles(string key, int depth, GameObject prefab, GameObject parentObj){
        int numParticlesPerRow = 5;

        float widthOffset = bounds.width / numParticlesPerRow;
        float heightOffset = bounds.height / numParticlesPerRow;
        float depthOffset = bounds.depth / numParticlesPerRow;

        int halfParticlesPerRow = Mathf.FloorToInt(numParticlesPerRow / 2);

        for(int x = 0; x < numParticlesPerRow; ++x){
            int xValue = x - halfParticlesPerRow;
            for(int y = 0; y < numParticlesPerRow; ++y){
                int yValue = y - halfParticlesPerRow;
                for(int z = 0; z < numParticlesPerRow; ++z){
                    int zValue = z - halfParticlesPerRow;

                    GameObject windObject = GameObject.Instantiate(prefab);
                    windObject.transform.position = processorCenter + new Vector3(xValue * widthOffset, yValue * heightOffset, zValue * depthOffset);
                    windObject.transform.localScale = new Vector3(10, 10, 10);
                    windObject.transform.parent = parentObj.transform;
                    
                    // WindFlow windParticle = windObject.GetComponent<WindFlow>();
                    // windParticles.Add(windParticle);


                }
            } 
        }

        scalarDataAssigned[key][depth] = true;
    }



    public void drawRaysForDepth(int depth, float scale){

        int sideLength = (int)Math.Pow(2, depth);

        float voxelWidth = bounds.width / sideLength;
        float voxelHeight = bounds.height / sideLength;
        float voxelDepth = bounds.depth / sideLength;


        Vector3[] voxelValues = vectorVoxelData["U"][depth];

        Vector3 startPos = new Vector3(bounds.startCoord.x +  voxelWidth / 2, bounds.startCoord.y + voxelHeight / 2, bounds.startCoord.z + voxelDepth / 2);
        int counter = 0;
        for(int d = 0; d < sideLength; ++d){
            float curZ = startPos.z + voxelDepth * d;
            for(int h = 0; h < sideLength; ++h){
                float curY = startPos.y + voxelHeight * h;
                for(int w = 0; w < sideLength; ++w){
                    float curX = startPos.x + voxelWidth * w;
                    Vector3 currentPos = new Vector3(curX, curY, curZ);
                    
                    Vector3 currentValue = voxelValues[counter];
                    if(VisibilityData.isVisible(currentValue)){
                        Vector3 norCurValue = currentValue.normalized;
                        Gizmos.color = new Color(Math.Abs(norCurValue.x), Math.Abs(norCurValue.y), Math.Abs(norCurValue.z));
                        Vector3 displayValue = currentValue * scale;
                        Gizmos.DrawRay(currentPos, displayValue);
                    }
                    counter++;
                }
            }  
        }
    }
}