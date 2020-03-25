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

		List<Planète> listeDeplanètesControllées;
		List<Soldat> listesDesSoldats;

		public string ID { get { return id; } set { id = value; } }
		public int NbSoldats { get { return nbSoldats; } set { nbSoldats = value; } }
		public int NbPlanèteContrôlée { get { return nbPlanèteContrôlée; } set { nbPlanèteContrôlée = value; } }
		public List<Soldat> ListesDesSoldats { get { return listesDesSoldats; } set { listesDesSoldats = value; } }
		public List<Planète> ListeDePlanètesControllées { get { return listeDeplanètesControllées; } set { listeDeplanètesControllées = value; } }
		public Player(string id, List<Planète> p)
		{
			ID = id;
			NbSoldats = 0;
			ListeDePlanètesControllées = p;
		}
	}
}
