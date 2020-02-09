using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuraluxLibrary
{
	class Planet : EventEntiy
	{
		public int Rayon
		{
			get;set;
		}
		public Planet(string id, int rayon) : base(id)
		{
			Rayon = rayon;
		}
	}
}
