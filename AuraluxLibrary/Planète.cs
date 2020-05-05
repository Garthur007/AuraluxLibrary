using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace AuraluxLibrary
{
	
	public class Planète : EventEntiy
	{
	
		public Planète() : base()
		{
			
		}
		public Planète Clone()
		{
			bool neutreC = EstNeutre;
			bool seFaitAtqC = SeFaitAttaquer;
			string idClone = Id;
			string idOwnerClone = idDuPropriétaire;
			int niveauDeSClone = NiveauDeSanté;
			int nbSoldatsClone = NbDeSoldats;
			int nbSoldatsParGénérationClone = nbDeSoldatParGénération;
			int nbDeSoldatsPourConquérirClone = nbDeSoldatsPourConquérir;
			int nbPtsClone = nbPts;
			int niveauActuelClone = NiveauActuel;
			int nbDePtsAvantProchainNiveauClone = nbDePtsAvantProchainNiveau;
			int niveauMaxClone = niveauMax;
			bool maxOutClone = maxOut;

			bool b = false;
			if (EstNeutre == true)
				b = true;
			else
				b = false;


			var copie = new Planète();
			copie.EstNeutre = b;
			copie.SeFaitAttaquer = seFaitAtqC;
			copie.Id = this.Id;
			copie.idDuPropriétaire = this.idDuPropriétaire;
			copie.NiveauDeSanté = niveauDeSClone;
			copie.NbDeSoldats = nbSoldatsClone;
			copie.nbDeSoldatParGénération = nbSoldatsParGénérationClone;
			copie.nbDeSoldatsPourConquérir = nbDeSoldatsPourConquérirClone;
			copie.nbPts = this.nbPts;
			copie.NiveauActuel = niveauActuelClone;
			copie.nbDePtsAvantProchainNiveau = nbDePtsAvantProchainNiveauClone;
			copie.niveauMax = niveauMaxClone;
			copie.MaxOut = maxOutClone;
			return copie;
		}
	}
}
