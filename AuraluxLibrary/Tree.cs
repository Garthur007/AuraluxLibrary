using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuraluxLibrary
{
	public class Node
	{
		public bool isRoot { get { return parentNode == null;  } }
		public bool isLeaf { get { return ChildNodes.Count() == 0; } }


		GameState nodeGameState;
		public GameState NodeGameState { get { return nodeGameState; } set { nodeGameState = value; } }
		List<Node> childNodes;
		public List<Node> ChildNodes { get { return childNodes; } set { childNodes = value; } }

		Node parentNode;
		public Node ParentNode { get { return parentNode; } set { parentNode = value; } }
		int t;  //score
		public int T { get { return t; }set { t = value; } }
		int n; //nombre de fois que ca a été visité
		public int N { get { return n; } set { n = value; } }
		//public float UCB1 { get; set; } //UCB1 = moyenne_des_valeurs_finales + 2 X sqrt(ln(nombre_de_fois_que_So_est_visité)/)

		public Node(GameState gs)
		{
			NodeGameState = new GameState(gs.ListeDesJoueurs, gs.ListeDesPlanètes);
			ChildNodes = new List<Node>();
			T = 0;
			N = 0;
		}
		public void Add_Child(Node n) => ChildNodes.Add(n);
		public double UCB1(int ni)
		{
			if (ni == 0)
				return 1000000;
			return (double)T + 2 * Math.Sqrt(Math.Log((double)parentNode.N)/N);
		}
	}
}
