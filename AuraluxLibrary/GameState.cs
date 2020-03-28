using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuraluxLibrary
{
	public class GameState
	{
		const int NBP = 5;
		public List<Player> listeDesJoueurs;
		public List<Planète> listeDesPlanètes;
		public List<Planète> planètesNeutres;

		public List<AttackInfo> listeDesAttaques;

		public List<Player> ListeDesJoueurs { get { return listeDesJoueurs; } set { listeDesJoueurs = value; } }
		public List<Planète> ListeDesPlanètes { get { return listeDesPlanètes; } set { listeDesPlanètes = value; } }
		public List<Planète> PlanètesNeutres { get { return planètesNeutres; } set { planètesNeutres = value; } }

		public List<AttackInfo> ListeDesAttaques { get { return listeDesAttaques; } set { listeDesAttaques = value; } }
		
		public GameState(List<Player> lDesJoueurs, List<Planète> lPLanètes)
		{
			ListeDesJoueurs = new List<Player>();
			foreach (Player p in lDesJoueurs)
				ListeDesJoueurs.Add(p);

			ListeDesPlanètes = new List<Planète>();
			foreach (Planète p in lPLanètes)
				ListeDesPlanètes.Add(p);

			PlanètesNeutres = new List<Planète>();
			foreach (Planète p in ListeDesPlanètes)
				if (p.EstNeutre)
					PlanètesNeutres.Add(p);
		}


		public bool GameOver(){
			///
			return listeDesJoueurs[0].ListeDePlanètesControllées.Count == 5||
				 listeDesJoueurs[1].ListeDePlanètesControllées.Count == 5 || listeDesJoueurs[0].ListeDePlanètesControllées.Count == 0 ||
				 listeDesJoueurs[1].ListeDePlanètesControllées.Count == 0;

		}
		public int Score(Player a)
		{
			if (!GameOver())
				return 0;

			if (a.ListeDePlanètesControllées.Count == listeDesPlanètes.Count)
				return 10;
			else
				return -10;
		}


		public static GameState NextState(GameState gs, List<AttackInfo> attaques)
		{
			int nbJ = gs.listeDesJoueurs.Count();
			int nbP = gs.listeDesPlanètes.Count();
			//Copy
			var nJ = new List<Player>();
			foreach (Player p in gs.ListeDesJoueurs)
				nJ.Add(p);

			var nP = new List<Planète>();
			foreach (Planète p in gs.ListeDesPlanètes)
				nP.Add(p);

			//Ici on applique les changements causés au jeu suite aux attaques lancer par les joeurs;
			var planètesTemporaire = gs.ListeDesPlanètes;

			foreach(AttackInfo atq in attaques)
				if(!atq.SelfAtq)
					for(int i = 0; i < nbJ; ++i)
						if (atq.JoueurLancantAtq == gs.ListeDesJoueurs[i].ID && atq.AtqRéuissie)
						{
							//On a trouvé quel joeur avait lancé l'attaque.
							var planèteSubissantAtq = nP.Find(e => e.Id == atq.objectif);
							var nouvelÉtatPlanet = planèteSubissantAtq;
							nouvelÉtatPlanet.EstNeutre = false;
								

							nP.Remove(planèteSubissantAtq);
								//Pour le joueur lancant l'attaque
								var a = nJ.ElementAt(i);
							
								a.ListeDePlanètesControllées.Add(nouvelÉtatPlanet);
								nJ.RemoveAt(i);
								var nouvelÉtatDuJoueur = new Player(a.ID,a.ListeDePlanètesControllées);
								nouvelÉtatDuJoueur.ListeDePlanètesControllées.Add(planèteSubissantAtq);
							/////////////////////////////
							nouvelÉtatPlanet.JoueurEnContrôle = a.ID;

							//Pour le joueur qui se fait attaquer
							var def = nJ.Find(e => e.ID == atq.JoueurSeDéfendant);

								var nouvelÉtatDuJoueur2 = new Player(def.ID, def.ListeDePlanètesControllées);
								nouvelÉtatDuJoueur.ListeDePlanètesControllées.Remove(planèteSubissantAtq);

								//Pour les planètes

								nP.Add(nouvelÉtatPlanet);
							
							break;
						}
			
			return new GameState(nJ, nP);
			//return new GameState(nbJ, nbP, gs.listeDesJoueurs, gs.listeDesPlanètes);
		}
	
		
	}
}
