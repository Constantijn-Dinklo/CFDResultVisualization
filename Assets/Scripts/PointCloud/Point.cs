using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point 
{

    public Vector3 position;

    public Dictionary<string, float> values;

    public string UValue = "";

    public Point(Vector3 pos){
        this.position = pos;
        values = new Dictionary<string, float>();
        values["epsilon"] = 0.0f;
        values["k"] = 0.0f;
        values["nut"] = 0.0f;
        values["p"] = 0.0f;
        values["phi"] = 0.0f;
        values["U"] = 0.0f;
    }

    
    public void setValue(string key, float v){
        this.values[key] = v;
    }

    public float getValue(string key)
    {
        return this.values[key];
    }

    public bool inArea(Vector3 startCoord, float width, float height, float depth)
    {
        if( (this.position.x < startCoord.x) ||
            (this.position.y < startCoord.y) ||
            (this.position.z < startCoord.z))
        {
            return false;
        }

        if( (this.position.x > (startCoord.x + width)) ||
            (this.position.y > (startCoord.y + height)) ||
            (this.position.z > (startCoord.z + depth)))
        {
            return false;
        }

        return true;
    }

}
