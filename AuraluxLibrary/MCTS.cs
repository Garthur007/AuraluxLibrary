using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuraluxLibrary
{
	//La classe pour l'arbre de recherche de Monte Carlo
	public class MCTS
	{
		int nb = 0;
		public Random generateur;  
		public Random Generateur { get { return generateur; } set { generateur = value; } }  
		public bool Stop { get { return nb >= 500; } } // On met une limite de 500 exécution
		public Node rootNode; 
		public Node RootNode { get { return rootNode; } set { rootNode = value; } } //Le node au sommet de l'arbre
		public MCTS(GameStateMCTS gs)
		{
			rootNode = new Node(gs);
			Generateur = new System.Random();
		}
		public Node Greatest_UCB1(Node n)
		{
			//Cette méthode sert à déterminer quel node enfant a le plus haut score/UCB1
			//Si un node n'a jamais été explorer, on va le privilégié et lui donner un UCB1 d'au moins 1000000

			if (Stop || n.ChildNodes.Count() == 0) return null;

			double greatestValue = n.ChildNodes[0].UCB1;
			Node nodeToReturn = n.ChildNodes[0];
			for (int i = 1; i < 3; ++i)
				if (n.ChildNodes[i].UCB1 > greatestValue)
				{
					greatestValue = n.ChildNodes[i].UCB1;
					nodeToReturn = n.ChildNodes[i];
				}
			return nodeToReturn;
		}
		public void Execution(Node rNode)
		{
			//Si le node qui a été reçus en paramètre n'a jamais été exploré,
			//on l'explore et l'agrandit (on lui ajoute des nodes enfants )

			//Ensuite on détermine lequel des nodes enfants a le UCB1 le plus élevé et on itère à traves ce node
			nb++;
			if (Stop) return;
			

			if (rNode.ChildNodes.Count() == 0)
				ExplorerNode(rNode);
			var highestUCB1Node = Greatest_UCB1(rNode);
			Iterate(highestUCB1Node);
		}
		public void Iterate(Node temporaryRootNode)
		{
			
			if (Stop || temporaryRootNode == null) return;
			

			//On commence par déterminer si le Node a déjà été visité
			if (temporaryRootNode.N == 0)
			{
				//Si le Node n'a pas encore été visité, 
				//On simule une partie qui va donnner soit une victoire ou une défaite et 
				//on met à jour le score UCB1 par rapport à la simulation de jeu, dans le Node recu en paramètre,
				//et dans tous les nodes parents.

				//Ensuite, on fait refait l'éxécution à partir du même Node reçu en paramètre.

				var add = GameSimulation(
					new GameStateMCTS(temporaryRootNode.NodeGameState.ListeDesJoueurs,
					temporaryRootNode.NodeGameState.ListeDesPlanètes), 0);


				temporaryRootNode.T += add;
				temporaryRootNode.N++;
				BackPropagation(temporaryRootNode);
				Execution(RootNode);
			}
			else
			{
				//Sinon, on fait fait l'éxécution à partir du même Node reçu en paramètre.

				Execution(temporaryRootNode);
			}

		}
		public void BackPropagation(Node a)
		{
			//Cette méthode sert à mettre à jour l'état de tous les Nodes parent 
			//du Node reçu en paramètre,

			//C'est une méthode récursive qui ne s'arrête que lorsqu'on a atteint le node de base.

			if (Stop || a.ParentNode == null)
				return;
			else
			{
				a.ParentNode.T += a.T;
				a.ParentNode.N++;
				BackPropagation(a.ParentNode);
			}
		}
		public void ExplorerNode(Node t)
		{
			//Cette méthode sert à créer des nodes enfants pour le nodes reçu en paramètre.
			//C'est dans cette méthode que l'arbre s'agrandit.

			if (Stop) return;
			if (t.NodeGameState.IsGameOver(t.NodeGameState)) return;
			var l = new List<JoueurMCTS>(t.NodeGameState.ListeDesJoueurs);  //La liste des joueurs
			var pp = new List<PlanèteMCTS>(t.NodeGameState.ListeDesPlanètes);  //La liste des planètes
			GameStateMCTS gs = new GameStateMCTS(l, pp);
			var planètesNeutres = gs.ListeDesPlanètes.Where(plan => plan.EstNeutre); 
			var jA = gs.ListeDesJoueurs.Find(j => j.Id == "EnemyTag");  //Joueur lancant l'attaque



			string idPlanèteToAttack; 
			//Avant de pouvoir attaquer la planète d'un adversaire, il ne faut plus qu'il aille des planètes neutre
			if (planètesNeutres.Count() == 0)
				idPlanèteToAttack = gs.ListeDesJoueurs[1].PlanètesControllées.ElementAt(0);
			else
				idPlanèteToAttack = planètesNeutres.ElementAt(0).Id;

			//On a trois actions possibles, attaquer une planète, s'améliorer ou attendre. 
			GameStateMCTS childGameState1, childGameState2, childGameState3;

			if (jA.NbSoldats < pp.Find(plan => plan.Id == idPlanèteToAttack).NbPts)
			{
				//Si on a pas assez de soldat pour attaquer, on attend...
				childGameState1 = GameStateMCTS.NextGameState(gs,
				new AttackInfo(null, 0, null, null));
				childGameState2 = GameStateMCTS.NextGameState(gs,
				 new AttackInfo(null, 0, null, null));
			}
			else
			{
				//Sinon, on peut attaquer une planète neutre ou s'améliorer
				childGameState1 = GameStateMCTS.NextGameState(gs,
				   new AttackInfo("EnemyTag", jA.NbSoldats, idPlanèteToAttack, "Player"));

				childGameState2 = GameStateMCTS.NextGameState(gs,
				   new AttackInfo("EnemyTag", jA.NbSoldats, jA.PlanètesControllées.Where(a => gs.ListeDesPlanètes.Find(x => x.Id == a).Niveau < 4).ElementAt(0), "EnemyTag"));
			}
			//Le 3ieme choix est d'attendre
			childGameState3 = GameStateMCTS.NextGameState(gs,
			   new AttackInfo(null, 0, null, null));

			Node childNodeA = new Node(childGameState1);
			Node childNodeB = new Node(childGameState2);
			Node childNodeC = new Node(childGameState3);

			t.Add_Child(childNodeA);
			childNodeA.ParentNode = t;
			t.Add_Child(childNodeB);
			childNodeB.ParentNode = t;
			t.Add_Child(childNodeC);
			childNodeC.ParentNode = t;
		}
		public int GameSimulation(GameStateMCTS g, int n)
		{

			//Cette méthode simule une partie de manière aléatoire
			//En cas de victoire, le score de 10 est retourné et en cas de défaite, un score de -10 est retourné
			//Si la simulation est trop lourde (i.e. le nombre de récursion de la simulation atteint 100), on retourne 0

			var gs = new GameStateMCTS(g.ListeDesJoueurs, g.ListeDesPlanètes);
			int nbSoldatsPNeutre = 10;
			int nbSoldatsPEnnemi = 20;

			var nn = n;
			if (n >= 100) return 0; else nn++;
			if (gs.IsGameOver(gs)) return gs.FinaleScore(gs);

			var planètesNeutres = gs.ListeDesPlanètes.Where(p => p.EstNeutre == true);
			int choix = Generateur.Next(0, 4);



			AttackInfo atq = new AttackInfo(null, 0, null, null);

			if (n % 2 == 0 && gs.ListeDesJoueurs[0].PlanètesControllées.Count() != 0)
			{
				if (planètesNeutres.Count() == 0)
				{

					if (choix == 0 && gs.ListeDesJoueurs[0].NbSoldats >= 20)
						atq = new AttackInfo("EnemyTag", nbSoldatsPEnnemi, gs.ListeDesJoueurs[1].PlanètesControllées.ElementAt(0), "Player");
					else if (choix == 1 && gs.ListeDesJoueurs[0].NbSoldats >= 10)
						atq = new AttackInfo("EnemyTag", nbSoldatsPNeutre, gs.ListeDesJoueurs[0].PlanètesControllées.Where(a => gs.ListeDesPlanètes.Find(x => x.Id == a).Niveau < 4).ElementAt(0), "EnemyTag");
					else
						atq = new AttackInfo(null, 0, null, null);
				}
				else
				{
					if (gs.ListeDesJoueurs[1].PlanètesControllées.Count() == 0 && gs.ListeDesJoueurs[0].NbSoldats >= 10)
						atq = new AttackInfo("EnemyTag", nbSoldatsPNeutre, planètesNeutres.ElementAt(0).Id, "Player");
					else
					{
						if (choix == 0 && gs.ListeDesJoueurs[0].NbSoldats >= 10) //Attaque d'une planète neutre
							atq = new AttackInfo("EnemyTag", nbSoldatsPNeutre, planètesNeutres.ElementAt(0).Id, "Player");
						else if (choix == 1) //Self upgrade
							atq = new AttackInfo("EnemyTag", gs.ListeDesJoueurs[0].NbSoldats, gs.ListeDesJoueurs[0].PlanètesControllées.Where(a => gs.ListeDesPlanètes.Find(x => x.Id == a).Niveau < 4).ElementAt(0), "EnemyTag");
						else if (choix == 2 && gs.ListeDesJoueurs[0].NbSoldats >= 20) //Attaque planète ennemi
							atq = new AttackInfo("EnemyTag", nbSoldatsPEnnemi, gs.ListeDesJoueurs[1].PlanètesControllées.ElementAt(0), "Player");
						else//On attent
							atq = new AttackInfo(null, 0, null, null);//Debug.Log(".................................................");
					}
				}
			}
			else if (gs.ListeDesJoueurs[1].PlanètesControllées.Count() != 0)
			{
				if (planètesNeutres.Count() == 0)
				{

					if (choix == 0 && gs.ListeDesJoueurs[1].NbSoldats >= 20)
						atq = new AttackInfo("Player", nbSoldatsPEnnemi, gs.ListeDesJoueurs[0].PlanètesControllées.ElementAt(0), "EnemyTag");
					else if (choix == 1)
						atq = new AttackInfo("Player", gs.ListeDesJoueurs[1].NbSoldats, gs.ListeDesJoueurs[1].PlanètesControllées.Where(a => gs.ListeDesPlanètes.Find(x => x.Id == a).Niveau < 4).ElementAt(0), "Player");
					else
						atq = new AttackInfo(null, 0, null, null);
				}
				else
				{
					if (gs.ListeDesJoueurs[0].PlanètesControllées.Count() == 0 && gs.ListeDesJoueurs[1].NbSoldats >= 10)
						atq = new AttackInfo("Player", nbSoldatsPNeutre, planètesNeutres.ElementAt(0).Id, "EnemyTag");
					else
					{
						if (choix == 0 && gs.ListeDesJoueurs[1].NbSoldats >= 10)
							atq = new AttackInfo("Player", nbSoldatsPNeutre, planètesNeutres.ElementAt(0).Id, "EnemyTag");
						else if (choix == 1)
							atq = new AttackInfo("Player", gs.ListeDesJoueurs[1].NbSoldats, gs.ListeDesJoueurs[1].PlanètesControllées.Where(a => gs.ListeDesPlanètes.Find(x => x.Id == a).Niveau < 4).ElementAt(0), "Player");
						else if (choix == 2 && gs.ListeDesJoueurs[1].NbSoldats >= 20)
							atq = new AttackInfo("Player", nbSoldatsPEnnemi, gs.ListeDesJoueurs[0].PlanètesControllées.ElementAt(0), "EnemyTag");
						else
							atq = new AttackInfo(null, 0, null, null); //Debug.Log(".................................................");
					}
				}
			}
			return GameSimulation(GameStateMCTS.NextGameState(gs, atq), nn);
		}
		public static GameStateMCTS GameStateConverter(GameState g)
		{

			//Cette méthode sert à convertir un GameState traditionel,
			//à un GameStateMCTS
			var l = new List<JoueurMCTS>();
			foreach (Player x in g.ListeDesJoueurs)
			{
				var joueur = new JoueurMCTS(x.ID);
				foreach (Planète q in x.ListeDePlanètesControllées)
					joueur.AjouterPlanète(q.Id);
				joueur.NbSoldats = x.NbSoldats;
				l.Add(joueur);
			}
			var p = new List<PlanèteMCTS>();
			foreach (Planète x in g.ListeDesPlanètes)
			{
				var plan = new PlanèteMCTS(x.Id);
				plan.JoueurEnControlle = x.idDuPropriétaire;
				if (x.NiveauActuel == 0)
					plan.NbPts = 25;
				else if (x.NiveauActuel == 1)
					plan.NbPts = 75;
				else
					plan.NbPts = 151;

				p.Add(plan);
			}
			return new GameStateMCTS(l, p);
		}
	}
}
