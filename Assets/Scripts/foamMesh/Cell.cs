using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell {
    
    private int id;

    Vector3 minCoord;
    Vector3 maxCoord;

    //A list of all faces surrounding this cell in no particular order.
    private List<Face> faces;

    private Dictionary<char, float> scalarValues;

    private Dictionary<char, Vector3> vectorValues;

    public Cell(int id){
        this.id = id;
        this.faces = new List<Face>();
        
        minCoord = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        maxCoord = new Vector3(float.MinValue, float.MinValue, float.MinValue);


        this.scalarValues = new Dictionary<char, float>();
        this.vectorValues = new Dictionary<char, Vector3>();
    }

    public void addFace(Face face){
        faces.Add(face);
    }

    public void calculateBoundingBox(Vector3[] vertices){
        List<Vector3> points = this.getUniquePointList(vertices);
        for(int i = 0; i < points.Count; ++i){
            Vector3 point = points[i];

            this.minCoord.x = Mathf.Min(this.minCoord.x, point.x);
            this.minCoord.y = Mathf.Min(this.minCoord.y, point.y);
            this.minCoord.z = Mathf.Min(this.minCoord.z, point.z);

            this.maxCoord.x = Mathf.Max(this.maxCoord.x, point.x);
            this.maxCoord.y = Mathf.Max(this.maxCoord.y, point.y);
            this.maxCoord.z = Mathf.Max(this.maxCoord.z, point.z);


        }
    }

    public void setScalarValue(char key, float value){
        this.scalarValues[key] = value;
    }

    public float getScalarValue(char key){
        if(!this.scalarValues.ContainsKey(key)){
            Debug.Log(String.Format("No Scalar value for key {0}", key));
            return 1.0f;
        }
        return this.scalarValues[key];
    }

    public void setVectorValue(char key, Vector3 value){
        this.vectorValues[key] = value;
    }

    public Vector3 getVectorValue(char key){
        if(!this.vectorValues.ContainsKey(key)){
            Debug.Log(String.Format("No Vector value for key {0}", key));
            return Vector3.one;
        }
        return this.vectorValues[key];
    }

    public Vector3 getCenter(Vector3[] vertices){

        Vector3 center = new Vector3(0.0f, 0.0f, 0.0f);

        HashSet<int> pointSet = new HashSet<int>();

        for(int i = 0; i < faces.Count; ++i){
            Face f = faces[i];

            for(int j = 0; j < f.triangles.Count; ++j){
                pointSet.Add(f.triangles[j]);
            }
        }


        foreach (int i in pointSet)
        {
            Vector3 point = vertices[i];
            center += point;
        }

        center = center / pointSet.Count;
        //Debug.Log(center);

        return center;
    }

    public HashSet<int> getUniquePointIndexes() {
        HashSet<int> pointSet = new HashSet<int>();

        for(int i = 0; i < faces.Count; ++i){
            Face f = faces[i];

            for(int j = 0; j < f.triangles.Count; ++j){
                pointSet.Add(f.triangles[j]);
            }
        }

        return pointSet;
    }

    public List<Vector3> getUniquePointList(Vector3[] vertices){
        List<Vector3> points = new List<Vector3>();

        HashSet<int> pointSet = getUniquePointIndexes();
        foreach (int i in pointSet)
        {
            Vector3 point = vertices[i];
            points.Add(point);
        }

        return points;
    }



    public bool insideCell(Vector3 point, Vector3[] vertices){

        if(!insideBoundingBox(point)){
            return false;
        }

        Vector3 endPoint = new Vector3(point.x, 1000, point.z);

        int numIntersections = 0;
        for(int i = 0; i < faces.Count; ++i){
            if(intersectFace(faces[i], point, endPoint, vertices)){
                numIntersections++;
            }
        }
        if(numIntersections % 2 != 0){
            return true;
        }
        return false;
    }

    public bool insideBoundingBox(Vector3 point){
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

    public bool intersectFace(Face face, Vector3 startPoint, Vector3 endPoint, Vector3[] vertices){

        for(int i = 0; i < face.num_triangles; ++i){
             Vector3 faceNormal = face.calculateNormal(i, vertices);
            Vector3 facePoint = vertices[face.triangles[0]]; //Always simply grab the first point of the face as it doesn't matter which point we grab for the plane equation.

            Vector3 lineLength = endPoint - startPoint;
            Vector3 direction = lineLength;
            direction.Normalize();

            float distance = lineLength.y / direction.y;

            float t = 0;
            if(intersectPlane(faceNormal, facePoint, startPoint, direction, ref t)){
                 Vector3 p1 = vertices[face.triangles[i * 3]];
                Vector3 p2 = vertices[face.triangles[i * 3 + 1]];
                Vector3 p3 = vertices[face.triangles[i * 3 + 2]];

                //return true;

                if(t > distance){
                    return false;
                }

                // Debug.DrawLine(p1, p2, Color.green, 1000);
                // Debug.DrawLine(p2, p3, Color.yellow, 1000);
                // Debug.DrawLine(p3, p1, Color.red, 1000);
                
                Vector3 intersection_point = startPoint + direction * t;

                //DebugDraw.DrawCross(intersection_point, 10, 10, 10, Color.black);

                if(sameSide(p1, p2, intersection_point, p3) && sameSide(p2, p3, intersection_point, p1) && sameSide(p3, p1, intersection_point, p2)){
                    // Debug.DrawLine(p1, p2, Color.red, 1000);
                    // Debug.DrawLine(p2, p3, Color.red, 1000);
                    // Debug.DrawLine(p3, p1, Color.red, 1000);
                    
                    return true;
                }
            }
        }
       

        return false;
    }

    public bool intersectPlane(Vector3 n, Vector3 p0, Vector3 l0, Vector3 l, ref float t){

        float denom = Vector3.Dot(n, l); 
        //Debug.Log(denom);
        if(Math.Abs(denom) > 1e-6){
            Vector3 p0l0 = p0 - l0;
            t = Vector3.Dot(p0l0, n) / denom; 
            //Debug.Log(t); 
            return (t >= 0);
        }

        return false;
    }

    public bool sameSide(Vector3 l0, Vector3 l1, Vector3 p, Vector3 v){

        Vector3 pSide = Vector3.Cross(l1-l0, p-l0);
        Vector3 vSide = Vector3.Cross(l1-l0, v-l0);

        return Vector3.Dot(pSide, vSide) >= 0;

    }

}