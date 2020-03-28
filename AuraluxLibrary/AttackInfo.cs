using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuraluxLibrary
{
	public class AttackInfo
	{
		public string joueurLancantAtq;
		public string objectif;
		public int nombreDeUnits;
		public bool atqRéussie;
		public bool selfAtq;
		public string joueurSeDéfendant;

		public string JoueurLancantAtq { get { return joueurLancantAtq; } set { joueurLancantAtq = value; } }
		public string JoueurSeDéfendant { get { return joueurSeDéfendant; } set { joueurSeDéfendant = value; } }
		public string Objectif { get { return objectif; } set { objectif = value; } }
		public int NombreDeUnits { get { return nombreDeUnits; } set { nombreDeUnits = value; } }
		public bool AtqRéuissie { get { return atqRéussie; } set { atqRéussie = value; } }
		public bool SelfAtq { get { return selfAtq; } set { selfAtq = value; } }


		public AttackInfo(bool attaqueÀSoisMême,string IdAttaquant, string IdDefendant, string IdObjectif,int nbs)
		{
			SelfAtq = attaqueÀSoisMême;
			joueurLancantAtq = IdAttaquant;
			Objectif = IdObjectif;
			NombreDeUnits = nbs;
			AtqRéuissie = joueurLancantAtq == IdDefendant;
			JoueurSeDéfendant = IdDefendant;
			if (!AtqRéuissie)
				AtqRéuissie = NombreDeUnits >= 20;
		}
	}
}
