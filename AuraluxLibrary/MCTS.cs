using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuraluxLibrary
{
	public class MCTS
	{
		public bool Stop { get; set; }

		public Node rootNode;
		public Node RootNode { get { return rootNode; } set { rootNode = value; } }

		public MCTS(GameState gs)
		{
			Stop = false;
			rootNode = new Node(gs);
		}
		/**  Actions
		 * Attaquer planète neutre, planète ennemi
		 * Attendre
		 * S'upgrade
		 * */

	
		public Node Greatest_UCB1(Node n)
		{
			if (Stop)
				return null;
			double greatestValue = n.ChildNodes[0].UCB1(0);
			Node nodeToReturn = n.ChildNodes[0];
			for (int i = 1; i < 2; ++i) 
				if (n.ChildNodes[i].UCB1(0)>greatestValue)
				{
					greatestValue = n.ChildNodes[i].UCB1(0);
					nodeToReturn = n.ChildNodes[i];
				}
			return nodeToReturn;
		}

		public void Execution(Node rNode)
		{
			if (Stop) return;
				
			ExplorerNode(rNode);
			var highestUCB1Node = Greatest_UCB1(rNode);
			Iterate(highestUCB1Node);
		}
		public void Iterate(Node temporaryRootNode)
		{
			if (Stop) return;

			if (temporaryRootNode.n == 0)
			{
				temporaryRootNode.t += GameSimulation(temporaryRootNode.NodeGameState, 0);
				BackPropagation(temporaryRootNode);
			}
			else
			{
				//Execution(temporaryRootNode);
			}
		}
		public void BackPropagation(Node a)
		{
			if (Stop || a.ParentNode == null)
				return;
			else
			{
				a.ParentNode.t += a.t;
				a.ParentNode.n++;
				BackPropagation(a.ParentNode);
			}
		}
		public void ExplorerNode(Node t)
		{
			a.ParentNode == null

			if (t == null)
				throw new ArgumentNullException("t n'existe pas");

			var gs = t.NodeGameState;
			var planètesNeutres = gs.ListeDesPlanètes.Where(p => p.EstNeutre == true);
			var jA = gs.ListeDesJoueurs.Find(j => j.ID == "EnemyTag");

			string idPlanèteToAttack = planètesNeutres.Count() == 0 ? gs.ListeDesJoueurs[1].ListeDePlanètesControllées.ElementAt(0).Id :
				planètesNeutres.ElementAt(0).Id;



			var childGameState1 = GameState.NextState(gs,
				new AttackInfo(jA.ID, jA.NbSoldats, idPlanèteToAttack, "Player"));
			var childGameState2 = GameState.NextState(gs,
				new AttackInfo(jA.ID, jA.NbSoldats, jA.ListeDePlanètesControllées.Where(a => a.MaxOut == false).ElementAt(0).Id, "Player"));
			var childGameState3 = GameState.NextState(gs,
				new AttackInfo());

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
		public int GameSimulation(GameState gs, int n)
		{
			if (Stop)
				return 0;
			var nn = n;
			if (n >= 10) return -1; else nn++;
			if (gs.GameOver()) return gs.Score(gs.ListeDesJoueurs[0]);



			AttackInfo atq = new AttackInfo();
			var planètesNeutres = gs.ListeDesPlanètes.Where(p => p.EstNeutre == true);
			int choix = new Random().Next(0, 3);
			if (gs.ListeDesJoueurs[0].ListeDePlanètesControllées.Count != 0 && (n % 2 == 0 || gs.ListeDesJoueurs[1].ListeDePlanètesControllées.Count == 0))
			{
				if ((choix == 0 || planètesNeutres.Count() == 0) && gs.ListeDesJoueurs[1].ListeDePlanètesControllées.Count != 0)
					atq = new AttackInfo("EnemyTag", 20, gs.ListeDesJoueurs[1].ListeDePlanètesControllées.ElementAt(0).Id, "Player");
				else if ((choix == 1 || choix == 2 || gs.ListeDesJoueurs[1].ListeDePlanètesControllées.Count == 0) && planètesNeutres.Count() != 0)
					atq = new AttackInfo("EnemyTag", 20, planètesNeutres.ElementAt(0).Id, "Player");
				else
					atq = new AttackInfo("EnemyTag", 20, gs.ListeDesJoueurs[0].ListeDePlanètesControllées.Where(a => a.MaxOut == false).ElementAt(0).Id, "EnemyTag");
			}
			if (gs.ListeDesJoueurs[1].ListeDePlanètesControllées.Count != 0 && (n % 2 != 0 || gs.ListeDesJoueurs[0].ListeDePlanètesControllées.Count == 0))
			{
				if ((choix == 0 || planètesNeutres.Count() == 0) && gs.ListeDesJoueurs[0].ListeDePlanètesControllées.Count != 0)
					atq = new AttackInfo("Player", 20, gs.ListeDesJoueurs[0].ListeDePlanètesControllées.ElementAt(0).Id, "EnemyTag");
				else if ((choix == 1 || choix == 2 || gs.ListeDesJoueurs[0].ListeDePlanètesControllées.Count == 0) && planètesNeutres.Count() != 0)
					atq = new AttackInfo("Player", 25, planètesNeutres.ElementAt(0).Id, "EnemyTag");
				//else
				//atq = new AttackInfo(true, "Player", "Player", gs.ListeDesJoueurs[1].ListeDePlanètesControllées.Where(a => a.MaxOut == false).ElementAt(0).Id, 30);
			}

			return GameSimulation(GameState.NextState(gs, atq), nn);
		}
	}
}
