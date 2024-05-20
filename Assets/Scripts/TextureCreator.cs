using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ExampleEditorScript : MonoBehaviour
{
    [MenuItem("CreateExamples/3DTexture")]
    static void CreateTexture3D()
    {
        // Configure the texture
        int size = 32;
        int numVoxels = size * size * size;
        TextureFormat format = TextureFormat.RGBA32;
        TextureWrapMode wrapMode =  TextureWrapMode.Clamp;

        // Create the texture and apply the configuration
        Texture3D texture = new Texture3D(size, size, size, format, false);
        texture.wrapMode = wrapMode;

        // Create a 3-dimensional array to store color data
        Color[] colors = new Color[numVoxels];

        int center = size / 2;

        
        Debug.Log(numVoxels);
        int regionNum = 245;
        string key = "U";
        int depth = 5;
        Vector3[] values = readVectorValues(regionNum, key, depth);
        Debug.Log(values.Length);

        // for(int i = 0; i < numVoxels; ++i){
        //     colors[i] = new Color(values[i].x, values[i].y, values[i].z);
        // }

        // Populate the array so that the x, y, and z values of the texture will map to red, blue, and green colors
        // for(int i = 0; i < numVoxels; ++i){
        //     float x = UnityEngine.Random.Range(0.0f, 1.0f);
        //     float y = UnityEngine.Random.Range(0.0f, 1.0f);
        //     float z = UnityEngine.Random.Range(0.0f, 1.0f);
        //     colors[i] = new Color(x, y, z);
        // }

        float inverseResolution = 1.0f / (size - 1.0f);
        for (int z = 0; z < size; z++)
        {
            int zOffset = z * size * size;
            for (int y = 0; y < size; y++)
            {
                int yOffset = y * size;
                for (int x = 0; x < size; x++)
                {

                    // float valueX = ((Mathf.Abs(center - x) * -1.0f) + center) / center;
                    // float valueY = ((Mathf.Abs(center - y) * -1.0f) + center) / center;
                    // float valueZ = ((Mathf.Abs(center - z) * -1.0f) + center) / center;
                    
                    // colors[x + yOffset + zOffset] = new Color(valueX, valueX, valueX, valueX);
                    



                    colors[x + yOffset + zOffset] = new Color(x * inverseResolution,
                        y * inverseResolution, z * inverseResolution, 1.0f);
                }
            }
        }

        // Copy the color values to the texture
        texture.SetPixels(colors);

        // Apply the changes to the texture and upload the updated texture to the GPU
        texture.Apply();        

        // Save the texture to your Unity Project
        // string saveFileName = String.Format("Assets/Resources/Texture3DRegion{0}.asset", regionNum);
        string saveFileName = String.Format("Assets/Resources/RandomInverse.asset");
        AssetDatabase.CreateAsset(texture, saveFileName);
    }

    public static Vector3[] readVectorValues(int regionNum, string key, int depth){
        // Debug.Log(String.Format("Depth: {0}", depth));

        double sideLength = Math.Pow(2, depth);
        double size = sideLength * sideLength * sideLength;

        Vector3[] voxelValues = new Vector3[(int)size];

        int splitSize = 400;

        string filePath = string.Format("Assets/Resources/Case_One/region{0}/regions/region{1}/{2}/voxel{3}", splitSize, regionNum, key, depth);
        using (FileStream fileStream = File.OpenRead(filePath))
        {
            byte[] data = new byte[fileStream.Length];
            int bytesRead = fileStream.Read(data, 0, data.Length);

            int floatCount = data.Length / 8;
            int counter = 0;
            for (int i = 0; i < floatCount; i+= 3)
            {
                float x = (float)BitConverter.ToDouble(data, i * 8);
                float y = (float)BitConverter.ToDouble(data, (i+1) * 8);
                float z = (float)BitConverter.ToDouble(data, (i+2) * 8);
                Vector3 value = new Vector3(x,y,z);
                DataStatisticsVector.updateValue(key, value);
                voxelValues[counter] = value;
                counter++;
            }
            fileStream.Close();
        }

        // Debug.Log(String.Format("The processor {0} has Min X Value is: {1}", id, DataStatisticsVector.getMinXValue(key)));

        return voxelValues;
    }
}