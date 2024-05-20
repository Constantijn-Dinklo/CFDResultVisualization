using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoProc
{
    public class GeoPoint
    {
        
		public double x {get;set;}		
		public double y {get;set;}
		public double z {get;set;}

		public GeoPoint(){}
		
		public GeoPoint(double x, double y, double z)		
		{
			this.x=x;
			this.y=y;
			this.z=z;	
		}	

		public static GeoPoint operator +(GeoPoint p0, GeoPoint p1) 
		{
			return new GeoPoint(p0.x + p1.x, p0.y + p1.y, p0.z + p1.z);
		}
    }
}
