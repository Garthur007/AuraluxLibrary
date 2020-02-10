using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuraluxLibrary
{
	public class GénérerSoldatsArgs : EventArgs
	{
		public string Message { get; set; }
		public int nbSoldatsActuel { get; set; }
	}
	public class AttaquerArgs : EventArgs
	{
		public string Message { get; set; }
		public int nbSoldatsPourAtq { get; set; }
	}
	public class DefenseArgs : EventArgs
	{
		public string Message { get; set; }
		public int nbSoldatsPourLaDéfense { get; set; }
	}
	public class UpradeLevelArgs : EventArgs
	{
		public string Message { get; set; }
		public int nouveauNiveau { get; set; }
	}

	public abstract class EventEntiy
	{
		//Pour l'appeler, planet.OnNBSoldatsChanged += (s,d) => {Le comportement attendu}
		public EventHandler<GénérerSoldatsArgs> OnNBSoldatsChanged;
		public EventHandler<AttaquerArgs> OnAttack;
		public EventHandler<DefenseArgs> OnDefense;
		public EventHandler<UpradeLevelArgs> OnUpdrade;



		public bool EstNeutre { get; set; }
		public bool SeFaitAttaquer { get; set; }
		public string Id { get; private set; } //Le id de la planète

		public string IdDuPropriétaire { get; set; }
		public int NiveauDeSanté
		{
			get { return NiveauDeSanté; }
			set
			{
				if (value <= 0)
					this.EstNeutre = true;
				NiveauDeSanté = value;

			}
		}
		public string JoueurEnContrôle { get; private set; } //Le nom ou le id du jouer qui contrôle la planète
		public int NbDeSoldats
		{
			get { return NbDeSoldats; }
			private set
			{
				NbDeSoldats = value;
			}
		}
		private int NbDeSoldatParGénération
		{
			get { return NbDeSoldatParGénération; }
			set
			{
				NbDeSoldatParGénération = (NiveauActuel + 1) * 10;
			}
		}
		public int MaxNiveauDeSanté { get; private set; } //Le niveau de santé maximal selon le niveau de la planète
		public bool Conquérable { get; private set; }  //Est-ce que la planète est conquérable ou non?
		public int NbDeSoldatsPourConquérir { get { return NbDeSoldatsPourConquérir; } set { NbDeSoldatsPourConquérir = NbDeSoldats+ (int)(NbDeSoldats/3); } }  //Le nombre de soldats ennemis néccessaire pour conquérir la planète
		private int NbPoint { get; set; }  //Le nombre de point actuel
		public int NiveauActuel //Le niveau actuel
		{
			get { return NiveauActuel; }
			set
			{
				NiveauActuel = value;

			}

		}
		private int NbDePtsAvantProchainNiveau { get; set; } //Le nombre de points avant d'atteindre le prochain niveau
		private int NiveauMax { get { return NiveauMax; } set { NiveauMax = 3; } } //Le niveau maximal

		public void GénérerSoldats()
		{
			NbDeSoldats += NbDeSoldatParGénération;
			OnNBSoldatsChanged?.Invoke(this, new GénérerSoldatsArgs()
			{ Message = "Nouvelle Génération", nbSoldatsActuel = NbDeSoldats });
		}


		//Méthode pour incrémenter de niveau
		//Cette méthode s'assure que le niveau actuel ne dépasse pas le niveau maximal
		public void IncrémenterNiveau() 
		{
			NiveauActuel = NiveauActuel<NiveauMax? NiveauActuel + 1 : NiveauActuel;
			OnUpdrade?.Invoke(this, new UpradeLevelArgs() { Message = "Niveau supérieur", nouveauNiveau = NiveauActuel });
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

		public void Défendre(int nombreDeSoldatsPourDéfense)
		{
			NbDeSoldats -= nombreDeSoldatsPourDéfense;
			OnDefense?.Invoke(this, new DefenseArgs() 
			{ Message = "Il faut se défendre", nbSoldatsPourLaDéfense = nombreDeSoldatsPourDéfense });
		}

		public EventEntiy(string id)
		{
			Id = id;
			SeFaitAttaquer = false;
			Conquérable = true;
			NiveauDeSanté = 100;
		}
		
		public void Reset()
		{
			Conquérable = true;
			SeFaitAttaquer = false;
			NbDeSoldats = 0;
			EstNeutre = true;
		}
		public void ToggleConquérable() => Conquérable = !Conquérable;

    }
}
