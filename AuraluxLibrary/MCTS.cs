using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuraluxLibrary
{
	public class MCTS
	{
		int nb = 0;
		public  Random generateur;
		public Random Generateur { get { return generateur; } set { generateur = value; } }
		public bool Stop { get { return nb >= 100; } }

		public Node rootNode;
		public Node RootNode { get { return rootNode; } set { rootNode = value; } }

		public MCTS(GameStateMCTS gs)
		{
			rootNode = new Node(gs);
			Generateur = new Random();
		}
		/**  Actions
		 * Attaquer planète neutre, planète ennemi
		 * Attendre
		 * S'upgrade
		 * */
		

	   public Node Greatest_UCB1(Node n)
	   {
		   if (Stop) return null;

			double greatestValue = n.ChildNodes[0].UCB1;
		   Node nodeToReturn = n.ChildNodes[0];
		   for (int i = 1; i < 2; ++i) 
			   if (n.ChildNodes[i].UCB1>greatestValue)
			   {
				   greatestValue = n.ChildNodes[i].UCB1;
				   nodeToReturn = n.ChildNodes[i];
			   }
		   return nodeToReturn;
	   }
	   public void Execution(Node rNode)
	   {
			nb++;
		   if (Stop) return;

		   ExplorerNode(rNode);
		   var highestUCB1Node = Greatest_UCB1(rNode);
		   Iterate(highestUCB1Node);
	   }
	   public void Iterate(Node temporaryRootNode)
	   {
		   if (Stop) return;

			
		   if (temporaryRootNode.N == 0)
		   {
			   temporaryRootNode.T += GameSimulation(temporaryRootNode.NodeGameState, 0);
				temporaryRootNode.N++;
				BackPropagation(temporaryRootNode);
				Execution(RootNode);
			}
			else
			{
				Execution(temporaryRootNode);
			}
		   
	   }
	   public void BackPropagation(Node a)
	   {
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
		   if (Stop) return;

		   var gs = t.NodeGameState;
		   var planètesNeutres = gs.ListeDesPlanètes.Where(p => p.EstNeutre);
		   var jA = gs.ListeDesJoueurs.Find(j => j.Id == "EnemyTag");

		   string idPlanèteToAttack = planètesNeutres.Count() == 0 ? gs.ListeDesJoueurs[1].PlanètesControllées.ElementAt(0) :
			   planètesNeutres.ElementAt(0).Id;

		   var childGameState1 = GameStateMCTS.NextGameState(gs,
			   new AttackInfo("EnemyTag", jA.NbSoldats, idPlanèteToAttack, "Player"));

		   var childGameState2 = GameStateMCTS.NextGameState(gs,
			   new AttackInfo("EnemyTag", jA.NbSoldats, jA.PlanètesControllées.Where(a => gs.ListeDesPlanètes.Find(x => x.Id == a).Niveau < 4).ElementAt(0), "EnemyTag"));

		   var childGameState3 = GameStateMCTS.NextGameState(gs,
			   new AttackInfo(null, 0,null,null));

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
		public int GameSimulation(GameStateMCTS gs, int n)
		{

			int nbSoldatsPNeutre = 10;
			int nbSoldatsPEnnemi = 20;

			var nn = n;
			if (n >= 100) return -1; else nn++;
			if (gs.IsGameOver(gs)) return gs.FinaleScore(gs);

			var planètesNeutres = gs.ListeDesPlanètes.Where(p => p.EstNeutre == true);
			int choix = Generateur.Next(0,4);



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
	}
}
