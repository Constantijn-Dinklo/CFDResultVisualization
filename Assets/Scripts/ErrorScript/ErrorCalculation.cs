using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Data.Common;

public class ErrorCalculation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        int region = 0;
        for(int lod = 0; lod < 6; ++lod){
            // int lod = 1;
            double sideLength = Math.Pow(2, lod);
            double pointAmount = sideLength * sideLength * sideLength;

            string basePathError = String.Format("Assets/Resources/ErrorData/region{0}Error/", region);
            string errorPathP = basePathError + String.Format("pErrorPercentage{0}", lod);
            float[] pErrorValues = readScalarErrorValues(errorPathP, (int)pointAmount);

            string errorPathU = basePathError + String.Format("UErrorPercentage{0}", lod);
            Vector3[] UErrorValues = readPoints(errorPathU, (int)pointAmount);

            string errorPathUMag = basePathError + String.Format("UErrorMagPercentage{0}", lod);
            float[] UMagErrorValues = readScalarErrorValues(errorPathUMag, (int)pointAmount);

            float averagePError = 0;
            Vector3 averageUError = new Vector3(0, 0, 0);
            float averageUMagError = 0;
            for(int i = 0; i < pointAmount; ++i){
                if(pErrorValues[i] != -1000000){
                    averagePError += pErrorValues[i];
                }
                if(UErrorValues[i].x != - 1000000){
                    averageUError += UErrorValues[i];
                }
                if(UMagErrorValues[i] != -1000000){
                    averageUMagError += UMagErrorValues[i];
                }
            }

            averagePError /= (float)pointAmount;
            averageUError /= (float)pointAmount;
            averageUMagError /= (float)pointAmount;

            double standardDeviationP = 0;
            double standardDeviationUMag = 0;

            for(int i = 0; i < pointAmount; ++i){
                float pErrorValue = pErrorValues[i];
                float UMagErrorValue = UMagErrorValues[i];
                if(pErrorValue == -1000000){
                    pErrorValue = 0;
                }
                if(UMagErrorValue == -1000000){
                    UMagErrorValue = 0;
                }

                standardDeviationP += Mathf.Pow((pErrorValue - averagePError), 2);
                standardDeviationUMag += Mathf.Pow((UMagErrorValue - averageUMagError), 2);
            }
            // Debug.Log(lod);
            // Debug.Log(standardDeviationP);
            // Debug.Log(standardDeviationUMag);

            standardDeviationP /= (float)pointAmount;
            standardDeviationUMag /= (float)pointAmount;
            // Debug.Log(standardDeviationP);
            // Debug.Log(standardDeviationUMag);

            // standardDeviationP = Math.Sqrt(standardDeviationP);
            // standardDeviationUMag = Math.Sqrt(standardDeviationUMag);
            // Debug.Log(standardDeviationP);
            // Debug.Log(standardDeviationUMag);

            Debug.Log(String.Format("The P value of LoD {0} has an average error of {1} with a sd of {2}", lod, averagePError, standardDeviationP));
            Debug.Log(String.Format("Lod {0} has an U error of x: {1}, y: {2}, z: {3}", lod, averageUError.x, averageUError.y, averageUError.z));
            Debug.Log(String.Format("The U Mag value of LoD {0} has an average error of {1} with a sd of {2}", lod, averageUMagError, standardDeviationUMag));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
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
