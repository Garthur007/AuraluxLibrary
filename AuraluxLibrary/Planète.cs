using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace AuraluxLibrary
{
	
	public class Planète : EventEntiy
	{
		public double x;
		public double y;
		public double z;
		public double X { get { return x; } set { x = value; } }
		public double Y { get { return y; } set { y = value; } }
		public double Z { get { return z; } set { z = value; } }
		public int Rayon
		{
			get;set;
		}
		public Planète(string id, int rayon, double x, double y, double z) : base(id)
		{
			X = x;
			Y = y;
			Z = z;
			Rayon = rayon;
		}
	}
}
