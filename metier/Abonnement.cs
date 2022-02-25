using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mediatek86.metier
{
    public class Abonnement
    {
        public Abonnement(int id, DateTime dateCommande, DateTime dateExpiration, double montant, string idRevue)
        {
            this.Id = id;
            this.DateCommande = dateCommande;
            this.DateExpiration = dateExpiration;
            this.Montant = montant;
            this.IdRevue = idRevue;
        }

        public int Id { get; set; }
        public DateTime DateCommande { get; set; }
        public DateTime DateExpiration { get; set; }
        public double Montant { get; set; }
        public string IdRevue { get; set; }
    }
}
