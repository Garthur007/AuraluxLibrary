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
			ListeDesJoueurs = new List<Player>();
			ListeDesPlanètes = new List<Planète>();

			foreach (Player p in lDesJoueurs)
				ListeDesJoueurs.Add(p.Clone());

			foreach (Planète p in lPLanètes)
				ListeDesPlanètes.Add(p.Clone());
		
		}


		public bool GameOver() => listeDesJoueurs[0].ListeDePlanètesControllées.Count == 5 ||
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
			GameState nextGameState = new GameState(gs.ListeDesJoueurs, gs.ListeDesPlanètes);

			List<Player> listeJoueur = nextGameState.ListeDesJoueurs;
			List<Planète> listePlanète = nextGameState.ListeDesPlanètes;

			Player joueurAttackant = listeJoueur.Find(player => player.ID == atq.JoueurLancantAtq);
			Player joueurDef = listeJoueur.Find(player => player.ID == atq.JoueurSeDéfendant);

			Planète planèteSubissantAtq = listePlanète.Find(planet => planet.Id == atq.Objectif);
			Planète planèteSubissantAtq2 = joueurDef.ListeDePlanètesControllées.Find(planet => planet.Id == atq.Objectif);

			if(atq.AtqRéuissie && !atq.SelfAtq)
			{
				planèteSubissantAtq.EstNeutre = false;
				planèteSubissantAtq.idDuPropriétaire = joueurAttackant.ID;
				joueurAttackant.ListeDePlanètesControllées.Add(planèteSubissantAtq);
				
				if(joueurDef.ListeDePlanètesControllées.Exists(planet => planet.Id == atq.Objectif))
				{
					joueurDef.ListeDePlanètesControllées.RemoveAll(planet => planet.Id == atq.Objectif);
				}


			}


			return nextGameState;
		}
	}
}
