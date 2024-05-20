using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System;


namespace UnityVolumeRendering
{
    /// <summary>
    /// Rutnime (play mode) GUI for editing a volume's visualisation.
    /// </summary>
    public class EditVolumeGUI : MonoBehaviour
    {
        public VolumeRenderedObject targetObject;

        //private VolumeRenderedObject[] targetObjects;
        private List<VolumeRenderedObject> targetObjects;

        private Rect windowRect = new Rect(150, 0, WINDOW_WIDTH, WINDOW_HEIGHT);

        private const int WINDOW_WIDTH = 500;
        private const int WINDOW_HEIGHT = 500;

        private int selectedRenderModeIndex = 0;
        private int selectedDataType = 0;
        private string valueStr;
        private Vector3 rotation;

        private static EditVolumeGUI instance;

        private int windowID;

        private void Awake()
        {
            // Fetch a unique ID for our window (see GUI.Window)
            windowID = WindowGUID.GetUniqueWindowID();
        }

        private void Start()
        {
            //rotation = targetObject.transform.rotation.eulerAngles;
            valueStr = "0";
        }

        // public static void ShowWindow(VolumeRenderedObject volRendObj)
        public static void ShowWindow()
        {
            if(instance != null)
                GameObject.Destroy(instance);

            //string old_text = $"EditVolumeGUI_{volRendObj.name}";
            string new_text = "EditVolumeGUI";
            GameObject obj = new GameObject(new_text);
            instance = obj.AddComponent<EditVolumeGUI>();
            //instance.targetObject = volRendObj;
        }

        //public static void ShowWindowAll(VolumeRenderedObject[] volRendObj)
        public static void ShowWindowAll(DataType dataType)
        {
            if(instance != null)
                GameObject.Destroy(instance);

            GameObject obj = new GameObject($"EditVolumeGUI");
            instance = obj.AddComponent<EditVolumeGUI>();
            instance.targetObjects = new List<VolumeRenderedObject>();

            instance.selectedDataType = DataConfig.dataTypeToInt(dataType);

        }

        public static void addTargetObject(VolumeRenderedObject volRendObj)
        {
            if(instance != null){
                instance.targetObjects.Add(volRendObj);
            }
        }

        private void OnGUI()
        {
            //string old_text = $"Edit volume ({targetObject.dataset.datasetName})";
            string new_text = "Edit volume";
            windowRect = GUI.Window(windowID, windowRect, UpdateWindow, new_text);
        }

        private void UpdateWindow(int windowID)
        {
            GUI.DragWindow(new Rect(0, 0, 15000, 20));

            GUILayout.BeginVertical();



            string dataType = DataConfig.dataTypeToString(DataConfig.dataType);
            GUILayout.Label("Data Type:");
            // selectedDataType = GUILayout.SelectionGrid(selectedDataType, new string[] { "Epsilon", "k", "nut", "p", "phi", "U" }, 6);
            selectedDataType = GUILayout.SelectionGrid(selectedDataType, new string[] { "p", "U" }, 2);

            //p
            if(selectedDataType == 0){
                if(targetObjects.Count > 0)
                {
                    // Render mode
                    GUILayout.Label("Render mode");
                    selectedRenderModeIndex = GUILayout.SelectionGrid(selectedRenderModeIndex, new string[] { "Direct volume rendering", "Maximum intensity projection", "Isosurface rendering", "Isosurface 1 value" }, 3);
                    VOConfig.setRenderMode((RenderMode)selectedRenderModeIndex);


                    //GUILayout.BeginHorizontal();
                    
                    //GUILayout.EndHorizontal();

                    // Visibility window
                    GUILayout.Label("Visibility window (min - max visible values)");
                    GUILayout.BeginHorizontal();
                    //Vector2 visibilityWindow = targetObject.GetVisibilityWindow();

                    //Min Vertical
                    GUILayout.BeginVertical();
                    //float minValue = targetObject.dataset.GetMinDataValue();
                    float minValue = DataStatistics.getMinValue(dataType);
                    GUILayout.Label("min: " + minValue.ToString());

                    if(VOConfig.getRenderMode() != RenderMode.IsosurfaceRendering1Value){
                        VisibilityData.scalarVisibilityWindow.x = GUILayout.HorizontalSlider(VisibilityData.scalarVisibilityWindow.x, 0.0f, VisibilityData.scalarVisibilityWindow.y, GUILayout.Width(150.0f));
                    }
                    else{
                        VisibilityData.scalarVisibilityWindow.x = GUILayout.HorizontalSlider(VisibilityData.scalarVisibilityWindow.x, 0.0f, 1.0f, GUILayout.Width(150.0f));
                    }
                    //float curMinValue = minValue + visibilityWindow.x * targetObject.dataset.GetDataRange();
                    float curMinValue = minValue + VisibilityData.scalarVisibilityWindow.x * DataStatistics.getRange(dataType);
                    GUILayout.Label("Current value: " + curMinValue.ToString());

                    string oldMinvaluestr = curMinValue.ToString();
                    string newValue = GUILayout.TextField(curMinValue.ToString(), GUILayout.Width(150.0f));
                    if(oldMinvaluestr != newValue){
                        if(newValue != ""){
                            float minTextValue = float.Parse(newValue);
                            minTextValue = (minTextValue - minValue);
                            VisibilityData.scalarVisibilityWindow.x = minTextValue / DataStatistics.getRange(dataType);
                            // VisibilityData.scalarVisibilityWindow.y = (minTextValue + VOConfig.Isosurface1ValueOffset) / targetObject.dataset.GetDataRange();
                        }
                    }

                    GUILayout.EndVertical();


                    
                    //Max Vertical
                    GUILayout.BeginVertical();
                    //float maxValue = targetObject.dataset.GetMaxDataValue();
                    float maxValue = DataStatistics.getMaxValue(dataType);
                    GUILayout.Label("max: " + maxValue.ToString());

                    float curMaxValue = 0.0f;
                    if(VOConfig.getRenderMode() != RenderMode.IsosurfaceRendering1Value){
                        VisibilityData.scalarVisibilityWindow.y = GUILayout.HorizontalSlider(VisibilityData.scalarVisibilityWindow.y, VisibilityData.scalarVisibilityWindow.x, 1.0f, GUILayout.Width(150.0f));
                        //curMaxValue = minValue + visibilityWindow.y * targetObject.dataset.GetDataRange();
                        curMaxValue = minValue + VisibilityData.scalarVisibilityWindow.y * DataStatistics.getRange(dataType);
                    }
                    else{
                        curMaxValue = (curMinValue + VOConfig.Isosurface1ValueOffset);
                        //visibilityWindow.y = (curMaxValue - minValue) / targetObject.dataset.GetDataRange();
                        VisibilityData.scalarVisibilityWindow.y = (curMaxValue - minValue) / DataStatistics.getRange(dataType);
                    }
                    GUILayout.Label("Current value: " + curMaxValue.ToString());
                    
                    string oldMaxvaluestr = curMaxValue.ToString();
                    string newMaxValue = GUILayout.TextField(curMaxValue.ToString(), GUILayout.Width(150.0f));
                    if(oldMaxvaluestr != newMaxValue){
                        if(newMaxValue != ""){
                            float maxTextValue = float.Parse(newMaxValue);
                            maxTextValue = (maxTextValue - minValue);
                            VisibilityData.scalarVisibilityWindow.y = maxTextValue / DataStatistics.getRange(dataType);
                            // VisibilityData.scalarVisibilityWindow.y = (minTextValue + VOConfig.Isosurface1ValueOffset) / targetObject.dataset.GetDataRange();
                        }
                    }
                    GUILayout.EndVertical();

                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();

                    if(VOConfig.getRenderMode() != RenderMode.IsosurfaceRendering1Value){
                        
                        // visibilityWindow.y = GUILayout.HorizontalSlider(visibilityWindow.y, visibilityWindow.x, 1.0f, GUILayout.Width(150.0f));
                        // curMaxValue = minValue + visibilityWindow.y * targetObject.dataset.GetDataRange();
                    }
                    else{
                        
                        string oldStr = this.valueStr;
                        this.valueStr = GUILayout.TextField(this.valueStr, GUILayout.Width(150.0f));

                        if(oldStr != this.valueStr){
                            if(this.valueStr != ""){
                                float minTextValue = float.Parse(this.valueStr);
                                minTextValue = (minTextValue - minValue);
                                VisibilityData.scalarVisibilityWindow.x = minTextValue / DataStatistics.getRange(dataType);
                                VisibilityData.scalarVisibilityWindow.y = (minTextValue + VOConfig.Isosurface1ValueOffset) / targetObject.dataset.GetDataRange();
                            }
                        }
                        
                    }

                

                    GUILayout.EndHorizontal();

                    //Debug.Log(string.Format("The min value is {0} and the max value is {1}", visibilityWindow.x, visibilityWindow.y));


                    //Since not all the processors have datasets with the same value range. Each processor needs to display a different section of their values.
                    Vector2 targetVisibilityWindow = new Vector2(0.0f, 0.0f);
                    for(int i = 0; i < targetObjects.Count; ++i){
                        
                        float minTargetValue = targetObjects[i].dataset.GetMinDataValue();
                        float maxTargetValue = targetObjects[i].dataset.GetMaxDataValue();

                        float actualMinValue = minValue + (DataStatistics.getRange(dataType) * VisibilityData.scalarVisibilityWindow.x);
                        float actualMaxValue = minValue + (DataStatistics.getRange(dataType) * VisibilityData.scalarVisibilityWindow.y);

                        targetVisibilityWindow.x = (actualMinValue - minTargetValue) / targetObjects[i].dataset.GetDataRange();
                        targetVisibilityWindow.x = Mathf.Max(targetVisibilityWindow.x, 0); //We never want it below 0.
                        // Debug.Log(String.Format("X: {0}", targetVisibilityWindow.x));

                        targetVisibilityWindow.y = 1 - (maxTargetValue - actualMaxValue) / targetObjects[i].dataset.GetDataRange(); 
                        targetVisibilityWindow.y = Mathf.Min(targetVisibilityWindow.y, 1); //Should never be greather then 1.
                        // Debug.Log(String.Format("Y: {0}", targetVisibilityWindow.y));
                        
                        targetObjects[i].SetVisibilityWindow(targetVisibilityWindow);
                        //targetObjects[i].SetVisibilityWindow(visibilityWindow);
                    }

                    // Rotation
                    // GUILayout.Label("Rotation");
                    // rotation.x = GUILayout.HorizontalSlider(rotation.x, 0.0f, 360.0f);
                    // rotation.y = GUILayout.HorizontalSlider(rotation.y, 0.0f, 360.0f);
                    // rotation.z = GUILayout.HorizontalSlider(rotation.z, 0.0f, 360.0f);
                    //targetObject.transform.rotation = Quaternion.Euler(rotation);

                    // Load transfer function
                    // if(GUILayout.Button("Load transfer function", GUILayout.Width(150.0f)))
                    // {
                    //     RuntimeFileBrowser.ShowOpenFileDialog(OnLoadTransferFunction);
                    // }
                }
            }
            //U
            else if(selectedDataType == 1){
                //X Horizontal
                {
                    GUILayout.BeginHorizontal();

                    //Min X Vertical
                    GUILayout.BeginVertical();
                    float minXValue = DataStatisticsVector.getMinXValue(dataType);
                    GUILayout.Label("Min X: " + minXValue.ToString());

                    VisibilityData.xVisibilityWindow.x = GUILayout.HorizontalSlider(VisibilityData.xVisibilityWindow.x, 0.0f, VisibilityData.xVisibilityWindow.y, GUILayout.Width(150.0f));
                    float curMinXValue = minXValue + VisibilityData.xVisibilityWindow.x * DataStatisticsVector.getXRange(dataType);
                    GUILayout.Label("Current value: " + curMinXValue.ToString());
                    GUILayout.EndVertical();

                    //Max X Vertical
                    GUILayout.BeginVertical();
                    float maxXValue = DataStatisticsVector.getMaxXValue(dataType);
                    GUILayout.Label("Max X: " + maxXValue.ToString());

                    VisibilityData.xVisibilityWindow.y = GUILayout.HorizontalSlider(VisibilityData.xVisibilityWindow.y, VisibilityData.xVisibilityWindow.x, 1.0f, GUILayout.Width(150.0f));
                    float curMaxXValue = minXValue + VisibilityData.xVisibilityWindow.y * DataStatisticsVector.getXRange(dataType);
                    GUILayout.Label("Current value: " + curMaxXValue.ToString());
                    GUILayout.EndVertical();

                    GUILayout.EndHorizontal();
                }

                //Y Horizontal
                {
                    GUILayout.BeginHorizontal();

                    //Min Y Vertical
                    GUILayout.BeginVertical();
                    float minYValue = DataStatisticsVector.getMinYValue(dataType);
                    GUILayout.Label("Min Y: " + minYValue.ToString());

                    VisibilityData.yVisibilityWindow.x = GUILayout.HorizontalSlider(VisibilityData.yVisibilityWindow.x, 0.0f, VisibilityData.yVisibilityWindow.y, GUILayout.Width(150.0f));
                    float curMinYValue = minYValue + VisibilityData.yVisibilityWindow.x * DataStatisticsVector.getYRange(dataType);
                    GUILayout.Label("Current value: " + curMinYValue.ToString());
                    GUILayout.EndVertical();

                    //Max Y Vertical
                    GUILayout.BeginVertical();
                    float maxYValue = DataStatisticsVector.getMaxYValue(dataType);
                    GUILayout.Label("Max Y: " + maxYValue.ToString());

                    VisibilityData.yVisibilityWindow.y = GUILayout.HorizontalSlider(VisibilityData.yVisibilityWindow.y, VisibilityData.yVisibilityWindow.x, 1.0f, GUILayout.Width(150.0f));
                    float curMaxYValue = minYValue + VisibilityData.yVisibilityWindow.y * DataStatisticsVector.getYRange(dataType);
                    GUILayout.Label("Current value: " + curMaxYValue.ToString());
                    GUILayout.EndVertical();

                    GUILayout.EndHorizontal();
                }

                //Z Horizontal
                {
                    GUILayout.BeginHorizontal();

                    //Min Z Vertical
                    GUILayout.BeginVertical();
                    float minZValue = DataStatisticsVector.getMinZValue(dataType);
                    GUILayout.Label("Min Z: " + minZValue.ToString());

                    VisibilityData.zVisibilityWindow.x = GUILayout.HorizontalSlider(VisibilityData.zVisibilityWindow.x, 0.0f, VisibilityData.zVisibilityWindow.y, GUILayout.Width(150.0f));
                    float curMinZValue = minZValue + VisibilityData.zVisibilityWindow.x * DataStatisticsVector.getZRange(dataType);
                    GUILayout.Label("Current value: " + curMinZValue.ToString());
                    GUILayout.EndVertical();

                    //Max Z Vertical
                    GUILayout.BeginVertical();
                    float maxZValue = DataStatisticsVector.getMaxZValue(dataType);
                    GUILayout.Label("Max Z: " + maxZValue.ToString());

                    VisibilityData.zVisibilityWindow.y = GUILayout.HorizontalSlider(VisibilityData.zVisibilityWindow.y, VisibilityData.zVisibilityWindow.x, 1.0f, GUILayout.Width(150.0f));
                    float curMaxZValue = minZValue + VisibilityData.zVisibilityWindow.y * DataStatisticsVector.getZRange(dataType);
                    GUILayout.Label("Current value: " + curMaxZValue.ToString());
                    GUILayout.EndVertical();

                    GUILayout.EndHorizontal();
                }

                //Mag Horizontal
                {
                    GUILayout.BeginHorizontal();

                    //Min Mag Vertical
                    GUILayout.BeginVertical();
                    float minMagValue = DataStatisticsVector.getMinMag(dataType);
                    GUILayout.Label("Min Mag: " + minMagValue.ToString());

                    VisibilityData.magVisibilityWindow.x = GUILayout.HorizontalSlider(VisibilityData.magVisibilityWindow.x, 0.0f, VisibilityData.magVisibilityWindow.y, GUILayout.Width(150.0f));
                    float curMinMagValue = minMagValue + VisibilityData.magVisibilityWindow.x * DataStatisticsVector.getMagRange(dataType);
                    GUILayout.Label("Current value: " + curMinMagValue.ToString());
                    GUILayout.EndVertical();

                    //Max X Vertical
                    GUILayout.BeginVertical();
                    float maxMagValue = DataStatisticsVector.getMaxMag(dataType);
                    GUILayout.Label("Max Mag: " + maxMagValue.ToString());

                    VisibilityData.magVisibilityWindow.y = GUILayout.HorizontalSlider(VisibilityData.magVisibilityWindow.y, VisibilityData.magVisibilityWindow.x, 1.0f, GUILayout.Width(150.0f));
                    float curMaxXValue = minMagValue + VisibilityData.magVisibilityWindow.y * DataStatisticsVector.getMagRange(dataType);
                    GUILayout.Label("Current value: " + curMaxXValue.ToString());
                    GUILayout.EndVertical();

                    GUILayout.EndHorizontal();
                }

                //Scale Horizontal
                {
                    GUILayout.BeginHorizontal();

                    //Min Mag Vertical
                    GUILayout.BeginVertical();
                    GUILayout.Label("Scale: ");
                    VisibilityData.scaleValue = GUILayout.HorizontalSlider(VisibilityData.scaleValue, 0.1f, 10.0f, GUILayout.Width(150.0f));
                    GUILayout.Label("Current value: " +  VisibilityData.scaleValue.ToString());
                    GUILayout.EndVertical();


                    GUILayout.EndHorizontal();
                }

            }
            

            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if(GUILayout.Button("Destroy")){
                VisibilityData.destroy = true;
            }

            // Show close button
            if (GUILayout.Button("Close"))
            {
                GameObject.Destroy(this.gameObject);
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        private void OnLoadTransferFunction(RuntimeFileBrowser.DialogResult result)
        {
            if(!result.cancelled)
            {
                string extension = Path.GetExtension(result.path);
                if(extension == ".tf")
                {
                    TransferFunction tf = TransferFunctionDatabase.LoadTransferFunction(result.path);
                    if (tf != null)
                    {
                        targetObject.transferFunction = tf;
                        targetObject.SetTransferFunctionMode(TFRenderMode.TF1D);
                    }
                }
                if (extension == ".tf2d")
                {
                    TransferFunction2D tf = TransferFunctionDatabase.LoadTransferFunction2D(result.path);
                    if (tf != null)
                    {
                        targetObject.transferFunction2D = tf;
                        targetObject.SetTransferFunctionMode(TFRenderMode.TF2D);
                    }
                }
            }
        }
    }
}
