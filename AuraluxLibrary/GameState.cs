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
		private List<Player> listeDesJoueurs;
		private List<Planète> listeDesPlanètes;
		
		public List<Player> ListeDesJoueurs { get { return listeDesJoueurs; } set { listeDesJoueurs = value; } }
		public List<Planète> ListeDesPlanètes { get { return listeDesPlanètes; } set { listeDesPlanètes = value; } }
		
		public GameState(List<Player> lDesJoueurs, List<Planète> lPLanètes)
		{
			ListeDesJoueurs = new List<Player>(lDesJoueurs);
			ListeDesPlanètes = new List<Planète>(lPLanètes);
			/*
			foreach (Player p in lDesJoueurs)
				ListeDesJoueurs.Add(p.Clone());

			foreach (Planète p in lPLanètes)
				ListeDesPlanètes.Add(p.Clone());*/
		}

		
		public bool GameOver() =>
			listeDesJoueurs[0].ListeDePlanètesControllées.Count == 5 ||
			listeDesJoueurs[1].ListeDePlanètesControllées.Count == 5;
		public int Score(Player a)
		{
			if (!GameOver())
				return 0;

			if (a.ListeDePlanètesControllées.Count == ListeDesPlanètes.Count)
				return 10;
			else
				return -10;
		}

		public static GameState NextState(GameState gs, AttackInfo atq)
		{
			bool selfAttack = atq.SelfAtq;
			


			if (atq.JoueurLancantAtq == "" || atq.JoueurLancantAtq == null)
				return new GameState(gs.ListeDesJoueurs, gs.ListeDesPlanètes);

			List<Player> listeJoueur = new List<Player>();
			List<Planète> listePlanète = new List<Planète>(gs.listeDesPlanètes);

			foreach (Player p in gs.ListeDesJoueurs)
				listeJoueur.Add(p.Clone());
			//foreach (Planète p in gs.listeDesPlanètes)
			//listePlanète.Add(p.Clone());


			

			Player joueurAttackant = listeJoueur.Find(player => player.ID == atq.JoueurLancantAtq);
			Player joueurDef = listeJoueur.Find(player => player.ID == atq.JoueurSeDéfendant);

			Planète planèteSubissantAtq = listePlanète.Find(planet => planet.Id == atq.PlanèteCible);
			Planète planèteSubissantAtq2 = joueurDef.ListeDePlanètesControllées.Find(planet => planet.Id == atq.PlanèteCible);
			Planète planèteSubissantAtq3 = joueurAttackant.ListeDePlanètesControllées.Find(planet => planet.Id == atq.PlanèteCible);

			if (selfAttack)
			{
				//joueurAttackant.ListeDePlanètesControllées.Remove(planèteSubissantAtq3);
				int nombreDeSoldatPourATK = atq.NombreDeUnits;
				for(int i = 0; i< nombreDeSoldatPourATK; ++i)
				{
					joueurAttackant.ListeDePlanètesControllées.Find(planet => planet.Id == atq.PlanèteCible).AutoGuérison();
				}
			}

			if(!atq.SelfAtq)
			{
				if (!planèteSubissantAtq.EstNeutre)
					joueurDef.ListeDePlanètesControllées.Remove(planèteSubissantAtq2);
				planèteSubissantAtq.EstNeutre = false;
				planèteSubissantAtq.idDuPropriétaire = joueurAttackant.ID;
				joueurAttackant.ListeDePlanètesControllées.Add(planèteSubissantAtq);
				if(joueurDef.ListeDePlanètesControllées.Exists(planet => planet.Id == atq.PlanèteCible))
				{
					joueurDef.ListeDePlanètesControllées.RemoveAll(planet => planet.Id == atq.PlanèteCible);
				}
			}
			
			return new GameState(listeJoueur, listePlanète);
		}
	}
}
