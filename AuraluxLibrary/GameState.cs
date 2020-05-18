using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuraluxLibrary
{
	//Classe pour l'état de jeu 
	public class GameState
	{
		
		private List<Player> listeDesJoueurs;
		private List<Planète> listeDesPlanètes;
		
		public List<Player> ListeDesJoueurs { get { return listeDesJoueurs; } set { listeDesJoueurs = value; } }
		public List<Planète> ListeDesPlanètes { get { return listeDesPlanètes; } set { listeDesPlanètes = value; } }
		
		public GameState(List<Player> lDesJoueurs, List<Planète> lPLanètes)
		{
			ListeDesJoueurs = new List<Player>(lDesJoueurs);
			ListeDesPlanètes = new List<Planète>(lPLanètes);
			
		}
		public bool GameOver()
		{
			for (int i = 1; i < ListeDesPlanètes.Count(); ++i)
				if (ListeDesPlanètes.ElementAt(i-1).idDuPropriétaire != ListeDesPlanètes.ElementAt(i).idDuPropriétaire)
					return false;
			return true;
		}
		public int Score(Player a)
		{
			if (!GameOver()) return 0;
			else if (a.ListeDePlanètesControllées.Count == ListeDesPlanètes.Count)
				return 10;
			else
				return -10;
		}

		public static GameState NextState(GameState g, AttackInfo atq)
		{
			return null;
		}

	
	}


	//Toutes les classes qui finissent par MCTS sont exlusivement pour l'arbre de recherche de Monte Carlo
	public class PlanèteMCTS
	{
		string id;
		string joueurEnControlle;
		
		int nbPts;


		public string Id { get { return id; } set { id = value; } }
		public string JoueurEnControlle { get { return joueurEnControlle; } set { joueurEnControlle = value; } }
		public int Niveau
		{
			get 
			{
				if (NbPts < 50)
					return 0;
				else if (NbPts < 100)
					return 1;
				else 
					return 2;
			}

		}
		public int NbUnitPerGen { get { return (Niveau + 1) * 10; } }
		public bool EstNeutre { get { return JoueurEnControlle == null || JoueurEnControlle == ""; } }
		public int NbPts { get { return nbPts; } set { nbPts = value; } }
		public PlanèteMCTS(string i, string jEnControlle = null)
		{
			Id = i;
			NbPts = 0;
			JoueurEnControlle = jEnControlle;
		}

	}
	public class JoueurMCTS
	{
		string id;
		List<string> planètesControllées;
		int nbSoldats;

		public string Id { get { return id; } set { id = value; } }
		public List<string> PlanètesControllées { get { return planètesControllées; } set { planètesControllées = value; } }
		public int NbSoldats { get {return nbSoldats; } set { nbSoldats = value; } }
		public JoueurMCTS(string i)
		{
			Id = i;
			PlanètesControllées = new List<string>();
		}
		public void AjouterPlanète(string p) => PlanètesControllées.Add(p);
		public void EnleverPlanète(string p) => PlanètesControllées.Remove(p);
		
	}
	public class GameStateMCTS
	{

		List<JoueurMCTS> listeDesJoueurs;
		List<PlanèteMCTS> listeDesPlanètes;
		public List<JoueurMCTS> ListeDesJoueurs { get { return listeDesJoueurs; } set { listeDesJoueurs = value; } }
		public List<PlanèteMCTS> ListeDesPlanètes { get { return listeDesPlanètes; } set { listeDesPlanètes = value; } }

		public GameStateMCTS(List<JoueurMCTS> lDesJoueurs, List<PlanèteMCTS> lPLanètes)
		{
			ListeDesJoueurs = new List<JoueurMCTS>(lDesJoueurs);
			ListeDesPlanètes = new List<PlanèteMCTS>(lPLanètes);
		}
		public bool IsGameOver(GameStateMCTS gs)
		{
			for (int i = 1; i < gs.ListeDesPlanètes.Count(); ++i)
				if (gs.ListeDesPlanètes[i - 1].JoueurEnControlle != gs.ListeDesPlanètes[i].JoueurEnControlle)
					return false;
			return true;
		}
		public  int FinaleScore(GameStateMCTS gs)
		{
			var n = 0;
			foreach (PlanèteMCTS p in gs.ListeDesPlanètes)
				if (p.JoueurEnControlle == gs.ListeDesJoueurs[0].Id)
					n++;
			if (n == gs.ListeDesPlanètes.Count())
				return 10;
			else
				return -10;

		}

		public static GameStateMCTS NextGameState(GameStateMCTS gs, AttackInfo a)
		{
			var lplanète = gs.ListeDesPlanètes;
			var lplayer = gs.ListeDesJoueurs;
			var listeDesPlanètes = new List<PlanèteMCTS>();
			var listeDesJoueurs = new List<JoueurMCTS>();


			foreach (JoueurMCTS j in lplayer)
			{
				var playerToAdd = new JoueurMCTS(j.Id);
				playerToAdd.NbSoldats = j.NbSoldats;
				playerToAdd.PlanètesControllées = new List<string>();
				foreach (string s in j.PlanètesControllées)
					playerToAdd.PlanètesControllées.Add(s);

				listeDesJoueurs.Add(playerToAdd);
			}
			foreach(PlanèteMCTS p in lplanète)
			{
				var planetToAdd = new PlanèteMCTS(p.Id);
				planetToAdd.JoueurEnControlle = p.JoueurEnControlle;
				planetToAdd.NbPts = p.NbPts;
				listeDesPlanètes.Add(planetToAdd);
			}

			if (a.PlanèteCible != null)  //Aucune atq
			{

				var planète = listeDesPlanètes.Find(p => p.Id == a.PlanèteCible);
				var attaquant = listeDesJoueurs.Find(j => j.Id == a.JoueurLancantAtq);


				if (a.JoueurLancantAtq == a.JoueurSeDéfendant)  //Atq à sois même
				{

					planète.NbPts += a.NombreDeUnits;
				}
				else
				{
					planète.JoueurEnControlle = a.JoueurLancantAtq;
					attaquant.AjouterPlanète(a.PlanèteCible);
					if (!listeDesPlanètes.Find(p => p.Id == a.PlanèteCible).EstNeutre) //Atq à une pNeutre
					{
						listeDesJoueurs.Find(j => j.Id == a.JoueurSeDéfendant).EnleverPlanète(a.PlanèteCible);
					}

				}

			}

			//Le code pour spawn les nouveaux soldats
			foreach (JoueurMCTS j in listeDesJoueurs)
			{
				foreach (string s in j.PlanètesControllées)
				{
					var p = listeDesPlanètes.Find(b => b.Id == s);
					j.NbSoldats += p.NbUnitPerGen;
				}
				j.NbSoldats -= a.NombreDeUnits;
			}

			return new GameStateMCTS(listeDesJoueurs, listeDesPlanètes);
		}
	}

}
