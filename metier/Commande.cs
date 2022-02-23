using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mediatek86.metier
{
    public class Commande
    {
        public Commande(int id, DateTime dateCommande, double montant, int nbExemplaire, string idLivreDvd, int idSuivi, string etat, string titre)
        {
            this.Id = id;
            this.DateCommande = dateCommande;
            this.Montant = montant;
            this.NbExemplaire = nbExemplaire;
            this.IdLivreDvd = idLivreDvd;
            this.IdSuivi = idSuivi;
            this.Etat = etat;
            this.Titre = titre;
        }

        public int Id { get; set; }
        public DateTime DateCommande { get; set; }
        public double Montant { get; set; }
        public int NbExemplaire { get; set; }
        public string IdLivreDvd { get; set; }
        public int IdSuivi { get; set; }
        public string Etat { get; set; }
        public string Titre { get; set; }


    }
}
