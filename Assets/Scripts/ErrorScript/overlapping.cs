using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class overlapping : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // pointInsideProcessor();
        // return;

        int region = 0;

        string basePath = "Assets/Resources/ErrorData/";

        StreamReader boundsReader = new StreamReader(String.Format(basePath + "region{0}/bounds", region));

        string[] minCoordString = boundsReader.ReadLine().Split(' ');
        string[] dimString = boundsReader.ReadLine().Split(' ');

        Vector3 minCoord = new Vector3(float.Parse(minCoordString[0]), float.Parse(minCoordString[1]), float.Parse(minCoordString[2]));
        float width = float.Parse(dimString[0]);
        float height = float.Parse(dimString[1]);
        float depth = float.Parse(dimString[2]);
        Vector3 maxCoord = new Vector3(minCoord.x + width, minCoord.y + height, minCoord.z + depth);

        DebugDraw.DrawBox(minCoord, width, height, depth, Color.white, 1000);


        StreamWriter overlapping = new StreamWriter(String.Format(basePath + "region{0}/overlapping", region));
        for(int i = 0; i < 48; ++i){

            string boundpath = String.Format("Assets/Resources/large_case/processorsASCII/processor{0}/bound", i);
            StreamReader boundWriter = new StreamReader(boundpath);
            
            string[] minCoordStringP = boundWriter.ReadLine().Split(' ');
            string[] maxCoordStringP = boundWriter.ReadLine().Split(' ');

            Vector3 minCoordP = new Vector3(float.Parse(minCoordStringP[0]), float.Parse(minCoordStringP[1]), float.Parse(minCoordStringP[2]));
            Vector3 maxCoordP = new Vector3(float.Parse(maxCoordStringP[0]), float.Parse(maxCoordStringP[1]), float.Parse(maxCoordStringP[2]));

            if(overlap(minCoord, maxCoord, minCoordP, maxCoordP)){
                overlapping.WriteLine(i.ToString());
                DebugDraw.DrawBox(minCoordP, maxCoordP.x - minCoordP.x, maxCoordP.y - minCoordP.y, maxCoordP.z - minCoordP.z, Color.red, 1000);

            }

        }
        overlapping.Close();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    bool overlap(Vector3 a1, Vector3 b1, Vector3 a2, Vector3 b2){

        if(a1.x > b2.x || a2.x > b1.x){
            return false;
        }

        if(a1.y > b2.y || a2.y > b1.y){
            return false;
        }

        if(a1.z > b2.z || a2.z > b1.z){
            return false;
        }

        return true;
    }

    public void pointInsideProcessor(){

        int pointAmount = 1151;
        Vector3[] points = new Vector3[pointAmount];
        string basePathError = "Assets/Resources/ErrorData/region245Error/";
        StreamReader pointReader = new StreamReader(basePathError + "missingPoints");
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



        string basePath = "Assets/Resources/ErrorData/";
        StreamWriter overlapping = new StreamWriter(basePath + "region245/overlapping");
        for(int i = 0; i < 48; ++i){

            string boundpath = String.Format("Assets/Resources/large_case/processorsASCII/processor{0}/bound", i);
            StreamReader boundWriter = new StreamReader(boundpath);
            
            string[] minCoordStringP = boundWriter.ReadLine().Split(' ');
            string[] maxCoordStringP = boundWriter.ReadLine().Split(' ');

            Vector3 minCoordP = new Vector3(float.Parse(minCoordStringP[0]), float.Parse(minCoordStringP[1]), float.Parse(minCoordStringP[2]));
            Vector3 maxCoordP = new Vector3(float.Parse(maxCoordStringP[0]), float.Parse(maxCoordStringP[1]), float.Parse(maxCoordStringP[2]));

            for(int p = 0; p < points.Length; ++p){
                if(insideBoundingBox(points[p], minCoordP, maxCoordP)){
                    Debug.Log(String.Format("Point {0} is inside bounding box {1}", p, i));
                }
            }

            // if(overlap(minCoord, maxCoord, minCoordP, maxCoordP)){
            //     overlapping.WriteLine(i.ToString());
            //     DebugDraw.DrawBox(minCoordP, maxCoordP.x - minCoordP.x, maxCoordP.y - minCoordP.y, maxCoordP.z - minCoordP.z, Color.red, 1000);

            // }

        }
        overlapping.Close();
    }


     public bool insideBoundingBox(Vector3 point, Vector3 minCoord, Vector3 maxCoord){
        if(point.x > maxCoord.x || point.x < minCoord.x){
            return false;
        }
        if(point.y > maxCoord.y || point.y < minCoord.y){
            return false;
        }
        if(point.z > maxCoord.z || point.z < minCoord.z){
            return false;
        }
        return true;
    }
}
