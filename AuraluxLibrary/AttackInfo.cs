using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuraluxLibrary
{
	public class AttackInfo
	{

		private string joueurLancantAtq;  
		private string planèteCible;
		private int nombreDeUnits;
		private bool selfAtq;
		private string joueurSeDéfendant;
		public bool Attendre { get; set; }

		public string JoueurLancantAtq { get { return joueurLancantAtq; } set { joueurLancantAtq = value; } }
		public string JoueurSeDéfendant { get { return joueurSeDéfendant; } set { joueurSeDéfendant = value; } }
		public string PlanèteCible { get { return planèteCible; } set { planèteCible = value; } }
		public int NombreDeUnits { get { return nombreDeUnits; } set { nombreDeUnits = value; } }
		public bool SelfAtq { get { return selfAtq; } set { selfAtq = value; } }


		public AttackInfo(string IdAttaquant, int nbSoldat,  string planèteCible, string IdDefendant , bool wait = false)
		{
			JoueurLancantAtq = IdAttaquant;  //Le joueur qui lance l'attaque
			JoueurSeDéfendant = IdDefendant;  //Le jouer qui se défend de l'attaque
			PlanèteCible = planèteCible;  //La cible de l'attaque
			NombreDeUnits = nbSoldat;    //Nombre d'unité de l'attaque
			SelfAtq = JoueurLancantAtq == JoueurSeDéfendant;
			Attendre = JoueurLancantAtq == null || JoueurLancantAtq == "";
		}
		public AttackInfo()
		{

		}

		/*
		 Qu'est-ce qui rend une attaque successfull??
		 1-  Le nombre de soldat avec laquelle tu attaques?  
		 2-  Le nombre de soldat déjà déployer sur ta cible?  ==>> pour lancer une attaque vers une planète,
																	il faut s'assurer d'avoir au moins autant de soldat
																	que le nombre de soldat déjà déployé.

	2w

		 */
	}
}
