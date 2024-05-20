using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoProc
{
    class GeoVector
    {
        GeoPoint p0; // vector begin point
		GeoPoint p1; // vector end point
        
		public double x {get{return (this.p1.x - this.p0.x);}} // vector x axis projection value
		public double y {get{return (this.p1.y - this.p0.y);}} // vector y axis projection value
		public double z {get{return (this.p1.z - this.p0.z);}} // vector z axis projection value 
	
		public GeoVector() {}
			
		public GeoVector(GeoPoint p0, GeoPoint p1)
		{
			this.p0 = p0;
			this.p1 = p1;			
		}		

		public static GeoVector operator *(GeoVector u, GeoVector v) 
		{
			double x = u.y * v.z - u.z * v.y;
			double y = u.z * v.x - u.x * v.z;
			double z = u.x * v.y - u.y * v.x;
			
			GeoPoint p0 = v.p0;
			GeoPoint p1 = p0 + new GeoPoint(x, y, z);

			return new GeoVector(p0, p1);
		}
		
    }
}
