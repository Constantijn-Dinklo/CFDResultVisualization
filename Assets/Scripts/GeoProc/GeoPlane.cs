using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoProc
{
    class GeoPlane
    {
        
		// Plane Equation: a * x + b * y + c * z + d = 0

		double a;
		double b;
		double c;
	    double d;

		public GeoPlane() {}
		
		public GeoPlane(double a, double b, double c, double d)
		{
			this.a = a;
			this.b = b;
			this.c = c;
			this.d = d;
		}

		public GeoPlane(GeoPoint p0, GeoPoint p1, GeoPoint p2)
		{
			
			GeoVector v = new GeoVector(p0, p1);

			GeoVector u = new GeoVector(p0, p2);

			GeoVector n = u * v;

			// normal vector		
			double a = n.x;
			double b = n.y;
			double c = n.z;				
			double d = -(a * p0.x + b * p0.y + c * p0.z);
			
			this.a = a;
			this.b = b;
			this.c = c;
			this.d = d;
			
		}

        public double A { get { return this.a; } }
        public double B { get { return this.b; } }
        public double C { get { return this.c; } }
        public double D { get { return this.d; } }

		public static GeoPlane operator -(GeoPlane pl) 
		{
			return new GeoPlane(-pl.a, -pl.b, -pl.c, -pl.d);
		}

		public static double operator *(GeoPoint pt, GeoPlane pl) 
		{
			return (pt.x * pl.a + pt.y * pl.b + pt.z * pl.c + pl.d);
		}
    }
}
