using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoProc
{
    public class GeoPolygonProc
    {
        double MaxUnitMeasureError = 0.001;

        // Polygon Boundary
		double X0, X1, Y0, Y1, Z0, Z1;	

		// Polygon faces
		List<GeoFace> Faces;

		// Polygon face planes
		List<GeoPlane> FacePlanes;

		// Number of faces
		int NumberOfFaces;	

		// Maximum point to face plane distance error, point is considered in the face plane if its distance is less than this error
		double MaxDisError;

		public GeoPolygonProc(){}		
			
#region public methods

		public GeoPolygonProc(GeoPolygon polygonInst)
        {
            //int MaxFacesCount = polygonInst.GetMaxFaceCount();

            List<GeoFace> faces = new List<GeoFace>();

            List<GeoPlane> facePlanes = new List<GeoPlane>();

            int numberOfFaces = 0;

            double x0 = 0, x1 = 0, y0 = 0, y1 = 0, z0 = 0, z1 = 0;

            // Get boundary 
            this.Get3DPolygonBoundary(polygonInst, ref x0, ref x1, ref y0, ref y1, ref z0, ref z1);

            // Get maximum point to face plane distance error, point is considered in the face plane if its distance is less than this error
            double maxDisError = this.Get3DPolygonUnitError(polygonInst);

            // Get face planes		
            this.GetConvex3DFaces(polygonInst, maxDisError, faces, facePlanes, ref numberOfFaces);

            // Set data members
            this.X0 = x0;
            this.X1 = x1;
            this.Y0 = y0;
            this.Y1 = y1;
            this.Z0 = z0;
            this.Z1 = z1;
            this.Faces = faces;
            this.FacePlanes = facePlanes;
            this.NumberOfFaces = numberOfFaces;
            this.MaxDisError = maxDisError;
        }

		public void GetBoundary(ref double xmin, ref double xmax, ref double ymin, ref double ymax, ref double zmin, ref double zmax)
		{
            xmin = this.X0;
            xmax = this.X1;
            ymin = this.Y0;
            ymax = this.Y1;
            zmin = this.Z0;
            zmax = this.Z1;
		}
	
		public bool PointInside3DPolygon(double x, double y, double z)
        {
            GeoPoint P = new GeoPoint(x, y, z);

            return this.PointInside3DPolygon(P, this.FacePlanes, this.NumberOfFaces);	
        }

#endregion

#region private methods    

        double Get3DPolygonUnitError(GeoPolygon polygon)
        {
            List<GeoPoint> vertices = polygon.V;
            int n = polygon.N;

            double measureError = 0;

            double xmin = 0, xmax = 0, ymin = 0, ymax = 0, zmin = 0, zmax =0;

            this.Get3DPolygonBoundary(polygon, ref xmin, ref xmax, ref ymin, ref ymax, ref zmin, ref zmax);

            measureError = ((Math.Abs(xmax) + Math.Abs(xmin) + Math.Abs(ymax) + Math.Abs(ymin) + Math.Abs(zmax) + Math.Abs(zmin)) / 6 * MaxUnitMeasureError);

            return measureError;
        }

		void Get3DPolygonBoundary(GeoPolygon polygon, ref double xmin, ref double xmax, ref double ymin, ref double ymax, ref double zmin, ref double zmax)
        {
            List<GeoPoint> vertices = polygon.V;

            int n = polygon.N;

            xmin = xmax = vertices[0].x;
            ymin = ymax = vertices[0].y;
            zmin = zmax = vertices[0].z;

            for (int i = 1; i < n; i++)
            {
                if (vertices[i].x < xmin) xmin = vertices[i].x;
                if (vertices[i].y < ymin) ymin = vertices[i].y;
                if (vertices[i].z < zmin) zmin = vertices[i].z;
                if (vertices[i].x > xmax) xmax = vertices[i].x;
                if (vertices[i].y > ymax) ymax = vertices[i].y;
                if (vertices[i].z > zmax) zmax = vertices[i].z;
            }		
        }
	
		bool PointInside3DPolygon(double x, double y, double z, List<GeoPlane> facePlanes, int numberOfFaces)
        {
            GeoPoint P = new GeoPoint(x, y, z);

            return this.PointInside3DPolygon(P, facePlanes, numberOfFaces);	
        }

		bool PointInside3DPolygon(GeoPoint P, List<GeoPlane> facePlanes, int numberOfFaces)
        {
            for (int i = 0; i < numberOfFaces; i++)
            {

                double dis = P * facePlanes[i];

                // If the point is in the same half space with normal vector for any facet of the cube, then it is outside of the 3D polygon		
                if (dis > 0)
                {
                    return false;
                }

            }

            // If the point is in the opposite half space with normal vector for all 6 facets, then it is inside of the 3D polygon
            return true;
        }
	
        // Input: polgon, maxError
        // Return: faces, facePlanes, numberOfFaces
		void GetConvex3DFaces(GeoPolygon polygon, double maxError, List<GeoFace> faces, List<GeoPlane> facePlanes, ref int numberOfFaces)
        {
            // vertices of 3D polygon
            List<GeoPoint> vertices = polygon.V;
		  
		    int n = polygon.N;
	
            // vertice indexes for all faces
            // vertice index is the original index value in the input polygon
            List<List<int>> faceVerticeIndex = new List<List<int>>();
			
            // face planes for all faces
		    List<GeoPlane> fpOutward = new List<GeoPlane>();	
            	
		    for(int i=0; i< n; i++)
		    { 
			    // triangle point 1
			    GeoPoint p0 = vertices[i];

			    for(int j= i+1; j< n; j++)
			    {
				    // triangle point 2
				    GeoPoint p1 = vertices[j];

				    for(int k = j + 1; k<n; k++)
				    {
					    // triangle point 3
					    GeoPoint p2 = vertices[k];
		
					    GeoPlane trianglePlane = new GeoPlane(p0, p1, p2);
					
					    int onLeftCount = 0;
					    int onRightCount = 0;

                        // indexes of points that lie in same plane with face triangle plane
                        List<int> pointInSamePlaneIndex = new List<int>();
		    
					    for(int l = 0; l < n ; l ++)
					    {
						    if(l != i && l != j && l != k) // any point other than the 3 triangle points
						    {
							    GeoPoint p = vertices[l];

							    double dis = p * trianglePlane;                                

							    if(Math.Abs(dis) < maxError) // next point is in the triangle plane
							    {							
								    pointInSamePlaneIndex.Add(l);								    
							    }
							    else
							    {
								    if(dis < 0)
								    {
									    onLeftCount ++;								
								    }
								    else
								    {
									    onRightCount ++;
								    }

							    }
						    }
					    }
				

					    if(onLeftCount == 0 || onRightCount == 0) // This is a face for a CONVEX 3d polygon. For a CONCAVE 3d polygon, this maybe not a face.
					    {
                            List<int> verticeIndexInOneFace = new List<int>();
						   
						    // triangle plane
                            verticeIndexInOneFace.Add(i);
                            verticeIndexInOneFace.Add(j);
                            verticeIndexInOneFace.Add(k);
                            
                            int m = pointInSamePlaneIndex.Count;

						    if(m > 0) // there are other vetirces in this triangle plane
						    {
							    for(int p = 0; p < m; p ++)
							    {
                                    verticeIndexInOneFace.Add(pointInSamePlaneIndex[p]);
							    }						
						    }

                            // if verticeIndexInOneFace is a new face, add it in the faceVerticeIndex list, add the trianglePlane in the face plane list fpOutward
                            if (!Utility.ContainsList(faceVerticeIndex, verticeIndexInOneFace))
                            {
                                faceVerticeIndex.Add(verticeIndexInOneFace);

                                if (onRightCount == 0)
                                {
                                    fpOutward.Add(trianglePlane);
                                }
                                else if (onLeftCount == 0)
                                {
                                    fpOutward.Add(-trianglePlane);
                                }
                            }

					    }
					    else
					    {					
						    // possible reasons:
						    // 1. the plane is not a face of a convex 3d polygon, it is a plane crossing the convex 3d polygon.
						    // 2. the plane is a face of a concave 3d polygon
					    }

				    } // k loop
			    } // j loop		
		    } // i loop                        

            // return number of faces
            numberOfFaces = faceVerticeIndex.Count;
            
            for (int i = 0; i < numberOfFaces; i++)
            {                
                // return face planes
                facePlanes.Add(new GeoPlane(fpOutward[i].A, fpOutward[i].B, fpOutward[i].C, fpOutward[i].D));

                List<GeoPoint> gp = new List<GeoPoint>();

                List<int> vi = new List<int>();
                
                for (int j = 0; j < faceVerticeIndex[i].Count; j++)
                {
                    vi.Add(faceVerticeIndex[i][j]);
                    gp.Add( new GeoPoint(vertices[vi[j]].x, vertices[vi[j]].y, vertices[vi[j]].z));
                }

                // return faces
                faces.Add(new GeoFace(gp, vi));
            }                  		  
        }

#endregion

	
    }
}
