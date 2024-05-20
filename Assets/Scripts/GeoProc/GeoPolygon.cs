using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoProc
{
    public class GeoPolygon
    {
         // Vertices of the 3D polygon
		List<GeoPoint> v;
		
		// Vertices Index
		List<int> idx;

        // Number of vertices
        public int N { get { return this.v.Count; } }

        public List<GeoPoint> V { get { return this.v; } }

        public List<int> I { get { return this.idx; } }

		public GeoPolygon(){}

        public GeoPolygon(List<GeoPoint> p)
		{			
			this.v = new List<GeoPoint>();

			this.idx = new List<int>();

			for(int i=0;i<p.Count;i++)
			{
				this.v.Add(p[i]);
				this.idx.Add(i);
			}		
		}      
     
    }
}
