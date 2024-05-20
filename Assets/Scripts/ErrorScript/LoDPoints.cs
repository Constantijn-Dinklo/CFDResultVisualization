using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class LoDPoints : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        int region = 0;
        string basePath = "Assets/Resources/ErrorData/";

        StreamReader boundsReader = new StreamReader(String.Format(basePath + "region{0}/bounds", region));

        string[] minCoordString = boundsReader.ReadLine().Split(' ');
        string[] dimString = boundsReader.ReadLine().Split(' ');

        Vector3 minCoord = new Vector3(float.Parse(minCoordString[0]), float.Parse(minCoordString[1]), float.Parse(minCoordString[2]));
        float width = float.Parse(dimString[0]);
        float height = float.Parse(dimString[1]);
        float depth = float.Parse(dimString[2]);

        for(int lod = 0; lod < 6; ++lod){
            // int lod = 1;
            double sizeLength = Math.Pow(2, lod);

            float voxelWidth = width / (float)sizeLength;
            float voxelHeight = height / (float)sizeLength;
            float voxelDepth = depth / (float)sizeLength;

            // Debug.Log(String.Format("V Width: {0}, V Height: {1}, V Depth: {2}", voxelWidth, voxelHeight, voxelDepth));

            StreamWriter writeLoD = new StreamWriter(String.Format(basePath + "region{0}/points{1}", region, lod));
            Vector3 startCoord = new Vector3(minCoord.x + (voxelWidth / 2), minCoord.y + (voxelHeight / 2), minCoord.z + (voxelDepth / 2));
            for(int d = 0; d < sizeLength; d++){
                float newDepth = startCoord.z + d * voxelDepth;
                string depthStr = newDepth.ToString();
                for(int h = 0; h < sizeLength; h++){
                    float newHeight = startCoord.y + h * voxelHeight;
                    string heightStr = newHeight.ToString();
                    for(int w = 0; w < sizeLength; w++){
                        float newWidth = startCoord.x + w * voxelWidth;
                        string widthStr = newWidth.ToString();
                        

                        writeLoD.WriteLine(widthStr + " " + heightStr + " " + depthStr);
                    }
                }
            }

            writeLoD.Close();
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
