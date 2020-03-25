using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuraluxLibrary
{
	public class GameState
	{
		


		public Player[] listeDesJoueurs;
		public Planète[] listeDesPlanètes;
		public Planète[] planètesNeutres;

		public List<AttackInfo> listeDesAttaques;

		public Player[] ListeDesJoueurs { get { return listeDesJoueurs; } set { listeDesJoueurs = value; } }
		public Planète[] ListeDesPlanètes { get { return listeDesPlanètes; } set { listeDesPlanètes = value; } }
		public Planète[] PlanètesNeutres { get { return planètesNeutres; } set { planètesNeutres = value; } }

		public List<AttackInfo> ListeDesAttaques { get { return listeDesAttaques; } set { listeDesAttaques = value; } }
		public GameState(int nombreDeJoueur, int nombreDePlanètes,
			List<Player> listeDesJoueurs, List<Planète> lPLanètes)
		{
			if (nombreDeJoueur == listeDesJoueurs.Count() && nombreDePlanètes == lPLanètes.Count())
			{
				ListeDesJoueurs = new Player[nombreDeJoueur];
				ListeDesPlanètes = new Planète[nombreDePlanètes];
				UpdateGameState(listeDesJoueurs, lPLanètes);
			}

		}

		public  bool GameOver() => PlanètesNeutres.Count() == 0;
		public int Score(Player a)
		{
			if (!GameOver())
				return 0;
			string winnerID = "";
			int score = -10;
			foreach(Player p in ListeDesJoueurs)
			{
				if(p.ListeDePlanètesControllées.Count() == listeDesPlanètes.Length)
				{
					winnerID = p.ID;
					break;
				}	
			}
			if (winnerID == a.ID)
				score = 10;
			return score;
		}

		public void UpdateGameState(List<Player> listeDesJoueurs, List<Planète> lPLanètes)
		{//Liste de tous les joeurs actifs du jeu
			
			for (int i = 0; i < ListeDesJoueurs.Length; ++i)
				ListeDesJoueurs[i] = listeDesJoueurs[i];


			//Liste de toutes les planètes du jeu
			
			for (int i = 0; i < ListeDesPlanètes.Length; ++i)
				ListeDesPlanètes[i] = lPLanètes[i];

			//Liste de toutes les planètes neutres
			var pNeutres = ListeDesPlanètes.Where(e => e.EstNeutre == true).ToList();
			PlanètesNeutres = new Planète[pNeutres.Count()];
			for (int i = 0; i < pNeutres.Count(); ++i)
				PlanètesNeutres[i] = pNeutres.ElementAt(i);
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
			var planètesTemporaire = gs.ListeDesPlanètes.Clone();

			foreach(AttackInfo atq in attaques)
				if(!atq.SelfAtq)
					for(int i = 0; i < gs.listeDesJoueurs.Length; ++i)
						if (atq.JoueurLancantAtq == gs.ListeDesJoueurs[i].ID)
						{
							//On a trouvé quel joeur avait lancé l'attaque.
							if (atq.AtqRéuissie)
							{
								var planèteSubissantAtq = nP.Find(e => e.Id == atq.objectif);
								var nouvelÉtatPlanet = planèteSubissantAtq;
								

								nP.Remove(planèteSubissantAtq);
								//Pour le joueur lancant l'attaque
								var a = nJ.ElementAt(i);
								nJ.Remove(a);
								var nouvelÉtatDuJoueur = new Player(a.ID,a.ListeDePlanètesControllées);
								nouvelÉtatDuJoueur.ListeDePlanètesControllées.Add(planèteSubissantAtq);
								/////////////////////////////
								nouvelÉtatPlanet.EstNeutre = false;
								nouvelÉtatPlanet.JoueurEnContrôle = a.ID;

								//Pour le joueur qui se fait attaquer
								var def = nJ.Find(e => e.ID == atq.JoueurSeDéfendant);

								var nouvelÉtatDuJoueur2 = new Player(def.ID, def.ListeDePlanètesControllées);
								nouvelÉtatDuJoueur.ListeDePlanètesControllées.Remove(planèteSubissantAtq);

								//Pour les planètes

								nP.Add(nouvelÉtatPlanet);
							}
							break;
						}
			
			GameState next = new GameState(nbJ, nbP, nJ, nP);
			return next;
		}
	}
	
	

}
