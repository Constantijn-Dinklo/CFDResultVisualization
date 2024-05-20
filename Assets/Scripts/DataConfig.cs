using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DataType
{
    P       = 0,
    U       = 1,
    // EPSILON = 0,
    // NUT     = 2,
    // K       = 3,
    // PHI     = 4,
}

public static class DataConfig
{
    public static DataType dataType = DataType.P;

    public static void setDataType(DataType newType)
    {
        dataType = newType;
    }

    public static DataType getDataType()
    {
        return dataType;
    }

    public static DataType stringToDataType(string key)
    {
        switch (key)
        {
            case "p":
                return DataType.P;
            case "U":
                return DataType.U;
            // case "epsilon":
            //     return DataType.EPSILON;
            // case "nut":
            //     return DataType.NUT;
            // case "p":
            //     return DataType.P;
            // case "phi":
            //     return DataType.PHI;
            default:
                return DataType.P;

        } 
    
    }

    public static string dataTypeToString(DataType dataType)
    {
        switch(dataType)
        {

            case DataType.P:
                return "p";
            case DataType.U:
                return "U";
            // case DataType.EPSILON:
            //     return "epsilon";
            // case DataType.NUT:
            //     return "nut";
            // case DataType.P:
            //     return "p";
            // case DataType.PHI:
            //     return "phi";

        }

        return "";
    }

    public static int dataTypeToInt(DataType dataType){
        switch(dataType)
        {
            case DataType.P:
                return 0;
            case DataType.U:
                return 1;
        }
        return 0;
    }
}