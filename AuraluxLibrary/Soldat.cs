using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AuraluxLibrary
{
	public class Soldat
	{
		string idEntitÉnControlle;
		public string IdEntitÉnControlle { get { return idEntitÉnControlle; } set { idEntitÉnControlle = value; } }

		double posX, posY, posZ, desX, desY, desZ;
		public string Id { get; set; }
		
		public double PosX { get { return posX; } set { posX = value; } }
		public double PosY { get { return posY; } set { posY = value; } }
		public double PosZ { get { return posZ; } set { posZ = value; } }

		public double DesX { get { return desX; } set { desX = value; } }
		public double DesY { get { return desY; } set { desY = value; } }
		public double DesZ { get { return desZ; } set { desZ = value; } }

		public Soldat(string idControlleur)
		{
			IdEntitÉnControlle = idEntitÉnControlle;
		}
		public void DéfinirPosition(double a, double b, double c)
		{
			PosX = a;
			PosY = b;
			PosZ = c;
		}
		public void DéfinirDestination(double a, double b, double c)
		{
			DesX = a;
			DesY = b;
			DesZ = c;
		}

		public override string ToString()
		{
			return $"{IdEntitÉnControlle}|{PosX}:{PosY}:{PosZ}&{DesX}:{DesY}:{DesZ}";
		}
	
	}
}
