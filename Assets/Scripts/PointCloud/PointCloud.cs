using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointCloud 
{

    Vector3 minCoords;
    Vector3 maxCoords;

    private List<Point> points;

    public PointCloud(){
        this.minCoords = new Vector3(10000000, 10000000, 10000000);
        this.maxCoords = new Vector3(-10000000, -10000000, -10000000);

        points = new List<Point>();

    }

    //All other 'addPoint' methods should call this method as it does extra calculations.
    public void addPoint(Point p){
        this.points.Add(p);

        if(this.minCoords.x > p.position.x){
            this.minCoords.x = p.position.x;
        }
        if(this.maxCoords.x < p.position.x){
            this.maxCoords.x = p.position.x;
        }
    
        if(this.minCoords.y > p.position.y){
            this.minCoords.y = p.position.y;
        }
        if(this.maxCoords.y < p.position.y){
            this.maxCoords.y = p.position.y;
        }

        if(this.minCoords.z > p.position.z){
            this.minCoords.z = p.position.z;
        }
        if(this.maxCoords.z < p.position.z){
            this.maxCoords.z = p.position.z;
        }
    }

    public void addPoint(Vector3 coords){
        Point p = new Point(coords);
        this.addPoint(p);
    }

    public void addPoint(float x, float y, float z){
        Vector3 coords = new Vector3(x, y, z);
        Point p = new Point(coords);
        this.addPoint(p);
    }

    public int numPoints(){
        return points.Count;
    }

    public Point getPoint(int index){
        return points[index];
    }

    public List<Point> getPoints()
    {
        return points;
    }

    public List<Point> getPointsInArea(Vector3 startCoord, float width, float height, float depth)
    {
        List<Point> returnPoints = new List<Point>();

        for(int i = 0; i < this.points.Count; ++i)
        {
            if(this.points[i].inArea(startCoord, width, height, depth))
            {
                returnPoints.Add(this.points[i]);
            }
        }
        return returnPoints;
    }
    
    public bool insidePointCloud(Vector3 point)
    {
        //If the point is less then any of the min coords it can't be inside this pointcloud
        if( (point.x < minCoords.x) ||
            (point.y < minCoords.y) ||
            (point.z < minCoords.z))
        {
                return false;
        }

        //If the point is greather then any of the max coords it can't be inside the pointcloud
        if( (point.x > maxCoords.x) ||
            (point.y > maxCoords.y) ||
            (point.z > maxCoords.z))
        {
            return false;
        }

        return true;
    }       
    
    public float getWidth(){
        return Mathf.Abs(maxCoords.x - minCoords.x);
    }

    public float getHeight(){
        return Mathf.Abs(maxCoords.y - minCoords.y);
    }

    public float getDepth(){
        return Mathf.Abs(maxCoords.z - minCoords.z);
    }
    
    public Vector3 getMinCoords(){
        return this.minCoords;
    }

    public Vector3 getMaxCoords(){
        return this.maxCoords;
    }

    public float getMinValue(string key){
        float minValue = float.MaxValue;
        for(int i = 0; i < points.Count; ++i){
            Point p = this.points[i];
            if(p.values[key] < minValue){
                minValue = p.values[key];
            }
        }

        return minValue;
    }

    public float getMaxValue(string key){
        float maxValue = float.MinValue;
        for(int i = 0; i < points.Count; ++i){
            Point p = this.points[i];
            if(p.values[key] > maxValue){
                maxValue = p.values[key];
            }
        }

        return maxValue;
    }

}
