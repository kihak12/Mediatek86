using System.Collections.Generic;
using Mediatek86.modele;
using Mediatek86.metier;
using Mediatek86.vue;


namespace Mediatek86.controleur
{
    internal class Controle
    {
        private readonly List<Livre> lesLivres;
        private readonly List<Dvd> lesDvd;
        private readonly List<Revue> lesRevues;
        private readonly List<Categorie> lesRayons;
        private readonly List<Categorie> lesPublics;
        private readonly List<Categorie> lesGenres;

        /// <summary>
        /// Ouverture de la fenêtre
        /// </summary>
        public Controle()
        {
            lesLivres = Dao.GetAllLivres();
            lesDvd = Dao.GetAllDvd();
            lesRevues = Dao.GetAllRevues();
            lesGenres = Dao.GetAllGenres();
            lesRayons = Dao.GetAllRayons();
            lesPublics = Dao.GetAllPublics();
            FrmMediatek frmMediatek = new FrmMediatek(this);
            frmMediatek.ShowDialog();
        }

        /// <summary>
        /// getter sur la liste des genres
        /// </summary>
        /// <returns>Collection d'objets Genre</returns>
        public List<Categorie> GetAllGenres()
        {
            return lesGenres;
        }

        /// <summary>
        /// getter sur la liste des livres
        /// </summary>
        /// <returns>Collection d'objets Livre</returns>
        public List<Livre> GetAllLivres()
        {
            return Dao.GetAllLivres();
        }

        /// <summary>
        /// getter sur la liste des commandes livres
        /// </summary>
        /// <returns>Collection d'objets Livre</returns>
        public Livre selectLivreById(string id)
        {
            return Dao.selectLivreById(id);
        }

        /// <summary>
        /// getter sur la liste des commandes livres
        /// </summary>
        /// <returns>Collection d'objets Livre</returns>
        public List<Commande> GetAllCommandeLivresDvd(string type)
        {
            return Dao.GetAllCommandeLivresDvd(type);
        }

        /// <summary>
        /// getter sur la liste des Dvd
        /// </summary>
        /// <returns>Collection d'objets dvd</returns>
        public List<Dvd> GetAllDvd()
        {
            return Dao.GetAllDvd();
        }

        /// <summary>
        /// getter sur la liste des revues
        /// </summary>
        /// <returns>Collection d'objets Revue</returns>
        public List<Revue> GetAllRevues()
        {
            return Dao.GetAllRevues();
        }

        /// <summary>
        /// getter sur les rayons
        /// </summary>
        /// <returns>Collection d'objets Rayon</returns>
        public List<Categorie> GetAllRayons()
        {
            return lesRayons;
        }

        /// <summary>
        /// getter sur les publics
        /// </summary>
        /// <returns>Collection d'objets Public</returns>
        public List<Categorie> GetAllPublics()
        {
            return lesPublics;
        }

        /// <summary>
        /// récupère les exemplaires d'une revue
        /// </summary>
        /// <returns>Collection d'objets Exemplaire</returns>
        public List<Exemplaire> GetExemplairesRevue(string idDocuement)
        {
            return Dao.GetExemplairesRevue(idDocuement);
        }

        /// <summary>
        /// Crée un exemplaire d'une revue dans la bdd
        /// </summary>
        /// <param name="exemplaire">L'objet Exemplaire concerné</param>
        /// <returns>True si la création a pu se faire</returns>
        public bool CreerExemplaire(Exemplaire exemplaire)
        {
            return Dao.CreerExemplaire(exemplaire);
        }

        /// <summary>
        /// Ajouté un livre dans la bdd
        /// </summary>
        /// <param name="exemplaire">L'objet Exemplaire concerné</param>
        /// <returns>True si la création a pu se faire</returns>
        public bool CreerLivre(Livre livre)
        {
            return Dao.CreerLivre(livre);
        }

        /// <summary>
        /// Modifier un livre dans la bdd
        /// </summary>
        /// <param name="livre">L'objet Exemplaire concerné</param>
        /// <returns>True si la création a pu se faire</returns>
        public bool UpdateLivre(Livre livre)
        {
            return Dao.UpdateLivre(livre);
        }

        /// <summary>
        /// Ajouté un Dvd dans la bdd
        /// </summary>
        /// <param name="dvd">L'objet Exemplaire concerné</param>
        /// <returns>True si la création a pu se faire</returns>
        public bool CreerDvd(Dvd dvd)
        {
            return Dao.CreerDvd(dvd);
        }

        /// <summary>
        /// Modifier un dvd dans la bdd
        /// </summary>
        /// <param name="dvd">L'objet Exemplaire concerné</param>
        /// <returns>True si la création a pu se faire</returns>
        public bool UpdateDvd(Dvd dvd)
        {
            return Dao.UpdateDvd(dvd);
        }

        /// <summary>
        /// Ajouté une revue dans la bdd
        /// </summary>
        /// <param name="revue">L'objet Exemplaire concerné</param>
        /// <returns>True si la création a pu se faire</returns>
        public bool CreerRevue(Revue revue)
        {
            return Dao.CreerRevue(revue);
        }

        /// <summary>
        /// Modifier une revue dans la bdd
        /// </summary>
        /// <param name="revue">L'objet Exemplaire concerné</param>
        /// <returns>True si la création a pu se faire</returns>
        public bool UpdateRevue(Revue revue)
        {
            return Dao.UpdateRevue(revue);
        }

        /// <summary>
        /// getter sur la liste des commandes livres
        /// </summary>
        /// <returns>Collection d'objets Livre</returns>
        public bool ModifyEtatCommande(Commande commande, string etat)
        {
            return Dao.ModifyEtatCommande(commande, etat);
        }

        /// <summary>
        /// getter sur la liste des commandes livres
        /// </summary>
        /// <returns>Collection d'objets Livre</returns>
        public bool CreerCommande(Commande commande, Livre livre)
        {
            return Dao.CreerCommande(commande, livre);
        }

        /// <summary>
        /// Supprimer une commande pas encore livrée
        /// </summary>
        /// <returns>Collection d'objets Livre</returns>
        public bool DeleteCommande(Commande commande)
        {
            return Dao.DeleteCommande(commande);
        }


        public bool SupprimerBdd(string Id, string table)
        {
            return Dao.SupprimerBdd(Id, table);
        }

    }

}

