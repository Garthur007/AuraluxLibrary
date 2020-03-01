using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuraluxLibrary
{
	public class Player
	{
		string id;
		int nbSoldats;
		int nbPlanèteContrôlée;


		public string ID { get { return id; } set { id = value; } }
		public int NbSoldats { get { return nbSoldats; } set { nbSoldats = value; } }
		public int NbPlanèteContrôlée { get { return nbPlanèteContrôlée; } set { nbPlanèteContrôlée = value; } }

		public Player()
		{

		}
	}
}
