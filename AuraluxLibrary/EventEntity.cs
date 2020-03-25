using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuraluxLibrary
{
	public class GénérerSoldatsArgs 
	{
		public string Message { get; set; }
		public int nbTotalDeSoldat { get; set; }
		public int nbNouveauxSoldats { get; set; }
	}
	public class AttaquerArgs : EventArgs
	{
		public string Message { get; set; }
		public int nbSoldatsPourAtq { get; set; }
	}
	public class DefenseArgs : EventArgs
	{
		public string Message { get; set; }
		public int NiveauDeSanté { get; set; }
	}
	public class UpradeLevelArgs : EventArgs
	{
		public string Message { get; set; }
		public int nouveauNiveau { get; set; }
	}
	public class NeutreArgs
	{
		public string Message { get; set; }
	}

	public abstract class EventEntiy
	{
		//Pour l'appeler, planet.OnNBSoldatsChanged += (s,d) => {Le comportement attendu}
		public EventHandler<GénérerSoldatsArgs> OnNBSoldatsChanged;
		public EventHandler<AttaquerArgs> OnAttack;
		public EventHandler<DefenseArgs> OnDefense;
		public EventHandler<UpradeLevelArgs> OnUpdrade;
		public EventHandler<NeutreArgs> OnDevientNeutre;


	
		public bool estNeutre;
		public bool seFaitAttaquer;
		public bool conquérable;
		public string id;
		public string idDuPropriétaire;
		public int niveauDeSanté;
		public int nbDeSoldats;
		public int nbDeSoldatParGénération;
		public int maxNiveauDeSanté;
		public int nbDeSoldatsPourConquérir;
		public int nbPts;
		public int niveauActuel;
		public int nbDePtsAvantProchainNiveau;
		public int niveauMax;



		public bool EstNeutre
		{
			get { return estNeutre; }
			set { estNeutre = value; OnDevientNeutre?.Invoke(this, new NeutreArgs() { Message = "Je suis neutre" }); }
		}
		public bool SeFaitAttaquer { get { return seFaitAttaquer; } set { seFaitAttaquer = value; } }
		public string Id { get { return id; }  set { id = value; } } //Le id de la planète

		public string JoueurEnContrôle { get { return idDuPropriétaire; } set { idDuPropriétaire = value; } }


		public int NiveauDeSanté
		{
			get { return niveauDeSanté; }
			set
			{
				if (value <= 0)
				{
					if (!EstNeutre)
					{
						EstNeutre = true;
						OnDevientNeutre?.Invoke(this, new NeutreArgs()
						{ Message = "On est devenue neutre" });
					}
				}
				niveauDeSanté = value;

			}
		}
		public int NbDeSoldats
		{
			get { return nbDeSoldats; }
			set
			{
				int temp = nbDeSoldats;
				nbDeSoldats = value;
				OnNBSoldatsChanged?.Invoke(this, new GénérerSoldatsArgs()
				{ Message = "Nouvelle Génération", nbTotalDeSoldat = nbDeSoldats, nbNouveauxSoldats = nbDeSoldats - temp });
			}
		}
		
		
		private int NbDeSoldatParGénération
		{
			get { return nbDeSoldatParGénération; }
			set
			{
				nbDeSoldatParGénération = (NiveauActuel + 1) * 10;
			}
		}


		public int MaxNiveauDeSanté { get { return maxNiveauDeSanté; } private set { maxNiveauDeSanté = value * (niveauActuel+1); } } //Le niveau de santé maximal selon le niveau de la planète
		public bool Conquérable { get { return conquérable; } private set { conquérable = value; } }  //Est-ce que la planète est conquérable ou non?
		public int NbDeSoldatsPourConquérir { get { return nbDeSoldatsPourConquérir; } set { nbDeSoldatsPourConquérir = value; } }  //Le nombre de soldats ennemis néccessaire pour conquérir la planète
		private int NbPoint { get { return nbPts; } set { nbPts = value; } }  //Le nombre de point actuel
		public int NiveauActuel //Le niveau actuel
		{
			get { return niveauActuel; }
			set
			{
				niveauActuel = value;
			}

		}
		private int NbDePtsAvantProchainNiveau { get { return nbDePtsAvantProchainNiveau; } set { NbDePtsAvantProchainNiveau = value; } } //Le nombre de points avant d'atteindre le prochain niveau
		private int NiveauMax { get { return niveauMax; } set { niveauMax = 3; } } //Le niveau maximal

		public void GénérerSoldats()
		{
			if(!estNeutre)
				NbDeSoldats += NbDeSoldatParGénération;
		}


		//Méthode pour incrémenter de niveau
		//Cette méthode s'assure que le niveau actuel ne dépasse pas le niveau maximal
		public void IncrémenterNiveau()
		{
			nbDePtsAvantProchainNiveau += (niveauActuel + 2)* 500;
			NiveauActuel = NiveauActuel < NiveauMax ? NiveauActuel + 1 : NiveauActuel;
			OnUpdrade?.Invoke(this, new UpradeLevelArgs() { Message = "Niveau supérieur", nouveauNiveau = NiveauActuel });
		}
		public void AutoGuérison()
		{
			if(niveauDeSanté < maxNiveauDeSanté)
			{
				niveauDeSanté++;
			}else if (nbDePtsAvantProchainNiveau > 0)
			{
				nbDePtsAvantProchainNiveau--;
			}
			else
			{
				IncrémenterNiveau();
			}
		}


		//Cette méthode est pour attaquer avec tous nos soldats
		public void AttaquerAvecTousNosSoldats() => Attaquer(NbDeSoldats);

		//On appelle cette méthode lorsqu'une planète doit se séparer de certains soldat
		public void Attaquer(int nombreDeSoldatsPourAtq)
		{
			NbDeSoldats -= nombreDeSoldatsPourAtq;
			OnAttack?.Invoke(this, new AttaquerArgs()
			{ Message = "Debut d'une attaque", nbSoldatsPourAtq = nombreDeSoldatsPourAtq });
		}

		public void Défendre(int ptsDeVie)
		{
			nbDeSoldats -= 1;
			if(nbDeSoldats==0)
				NiveauDeSanté -= ptsDeVie;
			OnDefense?.Invoke(this, new DefenseArgs()
			{ Message = "Il faut se défendre", NiveauDeSanté = NiveauDeSanté });
		}

		public EventEntiy()
		{

			
			SeFaitAttaquer = false;
			nbDeSoldatParGénération = 5;
			Conquérable = true;
			NiveauDeSanté = 100;
			maxNiveauDeSanté = 100;
			niveauActuel = 0;
			nbDePtsAvantProchainNiveau = 200;

		}

		public void Reset()
		{
			Conquérable = true;
			SeFaitAttaquer = false;
			NbDeSoldats = 0;
			EstNeutre = true;
			
		}
		public void Set()
		{
			SeFaitAttaquer = false;
			nbDeSoldatParGénération = 5;
			Conquérable = true;
			NiveauDeSanté = 100;
			EstNeutre = false;
			NbDeSoldatParGénération = 0;

		}
		public void ToggleConquérable() => Conquérable = !Conquérable;
		
    }
}
