using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugDraw
{

    public static int num_root = 0;
    public static int num_already = 0;
    public static int calculated_value = 0;

    public static int inside_building = 0;
    
    
    public static void DrawCross(Vector3 center, float width, float height, float depth, Color color, int seconds)
    {
        Vector3 widthMinus = new Vector3(center.x - width, center.y, center.z);
        Vector3 widthPlus = new Vector3(center.x + width, center.y, center.z);

        Vector3 heightMinus = new Vector3(center.x, center.y - height, center.z);
        Vector3 heightPlus = new Vector3(center.x, center.y + height, center.z);

        Vector3 depthMinus = new Vector3(center.x, center.y, center.z - depth);
        Vector3 depthPlus = new Vector3(center.x, center.y, center.z + depth);

        Debug.DrawLine(widthMinus, widthPlus, color, seconds);
        Debug.DrawLine(heightMinus, heightPlus, color, seconds);
        Debug.DrawLine(depthMinus, depthPlus, color, seconds);
    }

    public static void DrawCross(Vector3 center, float width, float height, float depth, Color color)
    {
        DebugDraw.DrawCross(center, width, height, depth, color, 1000);
    }

    public static void DrawBox(Vector3 bottomLeftCoords, float width, float height, float depth, Color display_color, int seconds = 1)
    {
        var bottomLeftFront = new Vector3(bottomLeftCoords.x, bottomLeftCoords.y, bottomLeftCoords.z);
        var bottomRightFront = new Vector3(bottomLeftCoords.x + width, bottomLeftCoords.y, bottomLeftCoords.z);
        var topLeftFront = new Vector3(bottomLeftCoords.x, bottomLeftCoords.y + height, bottomLeftCoords.z);
        var topRigthFront = new Vector3(bottomLeftCoords.x  + width, bottomLeftCoords.y + height, bottomLeftCoords.z);

        var bottomLeftBack = new Vector3(bottomLeftCoords.x, bottomLeftCoords.y, bottomLeftCoords.z + depth);
        var bottomRightBack = new Vector3(bottomLeftCoords.x + width, bottomLeftCoords.y, bottomLeftCoords.z  + depth);
        var topLeftBack = new Vector3(bottomLeftCoords.x, bottomLeftCoords.y + height, bottomLeftCoords.z  + depth);
        var topRigthBack = new Vector3(bottomLeftCoords.x  + width, bottomLeftCoords.y  + height, bottomLeftCoords.z  + depth);

        Debug.DrawLine(bottomLeftBack, bottomLeftFront, display_color, seconds);
        Debug.DrawLine(bottomRightBack, bottomRightFront, display_color, seconds);
        Debug.DrawLine(bottomLeftBack, bottomRightBack, display_color, seconds);
        Debug.DrawLine(bottomLeftFront, bottomRightFront, display_color, seconds);

        Debug.DrawLine(topLeftBack, topLeftFront, display_color, seconds);
        Debug.DrawLine(topRigthBack, topRigthFront, display_color, seconds);
        Debug.DrawLine(topLeftBack, topRigthBack, display_color, seconds);
        Debug.DrawLine(topLeftFront, topRigthFront, display_color, seconds);

        Debug.DrawLine(bottomLeftBack, topLeftBack, display_color, seconds);
        Debug.DrawLine(bottomLeftFront, topLeftFront, display_color, seconds);
        Debug.DrawLine(bottomRightBack, topRigthBack, display_color, seconds);
        Debug.DrawLine(bottomRightFront, topRigthFront, display_color, seconds);
    }

}
