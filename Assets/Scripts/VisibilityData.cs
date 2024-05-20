using System;
using System.Collections.Generic;
using UnityEngine;

public static class VisibilityData 
{

    public static Vector2 scalarVisibilityWindow = new Vector2(0, 1);

    public static Vector2 xVisibilityWindow = new Vector2(0, 1);
    public static Vector2 yVisibilityWindow = new Vector2(0, 1);
    public static Vector2 zVisibilityWindow = new Vector2(0, 1);

    public static Vector2 magVisibilityWindow = new Vector2(0, 1);

    public static float scaleValue = 1.0f;

    public static bool destroy = false;


    public static bool isVisible(Vector3 value){
        string dataType = DataConfig.dataTypeToString(DataConfig.dataType);

        float minXValue = DataStatisticsVector.getMinXValue(dataType);
        float curMinXValue = minXValue + xVisibilityWindow.x * DataStatisticsVector.getXRange(dataType);
        float curMaxXValue = minXValue + xVisibilityWindow.y * DataStatisticsVector.getXRange(dataType);
        if(value.x < curMinXValue || value.x > curMaxXValue){
            return false;
        }

        float minYValue = DataStatisticsVector.getMinYValue(dataType);
        float curMinYValue = minYValue + yVisibilityWindow.x * DataStatisticsVector.getYRange(dataType);
        float curMaxYValue = minYValue + yVisibilityWindow.y * DataStatisticsVector.getYRange(dataType);
        if(value.y < curMinYValue || value.y > curMaxYValue){
            return false;
        }

        float minZValue = DataStatisticsVector.getMinZValue(dataType);
        float curMinZValue = minZValue + zVisibilityWindow.x * DataStatisticsVector.getZRange(dataType);
        float curMaxZValue = minZValue + zVisibilityWindow.y * DataStatisticsVector.getZRange(dataType);
        if(value.z < curMinZValue || value.z > curMaxZValue){
            return false;
        }

        float minMag = DataStatisticsVector.getMinMag(dataType);
        float curMinMagValue = minMag + magVisibilityWindow.x * DataStatisticsVector.getMagRange(dataType);
        float curMaxMagValue = minMag + magVisibilityWindow.y * DataStatisticsVector.getMagRange(dataType);
        if(value.magnitude < curMinMagValue || value.magnitude > curMaxMagValue){
            return false;
        }

        return true;
    }
}