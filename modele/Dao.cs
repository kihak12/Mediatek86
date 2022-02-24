using Mediatek86.metier;
using System.Collections.Generic;
using Mediatek86.bdd;
using System;
using System.Windows.Forms;

namespace Mediatek86.modele
{
    public static class Dao
    {

        private static readonly string server = "localhost";
        private static readonly string userid = "root";
        private static readonly string password = "";
        private static readonly string database = "mediatek86";
        private static readonly string connectionString = "server="+server+";user id="+userid+";password="+password+";database="+database+";SslMode=none";

        /// <summary>
        /// Retourne tous les genres à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Genre</returns>
        public static List<Categorie> GetAllGenres()
        {
            List<Categorie> lesGenres = new List<Categorie>();
            string req = "Select * from genre order by libelle";

            BddMySql curs = BddMySql.GetInstance(connectionString);
            curs.ReqSelect(req, null);

            while (curs.Read())
            {
                Genre genre = new Genre((string)curs.Field("id"), (string)curs.Field("libelle"));
                lesGenres.Add(genre);
            }
            curs.Close();
            return lesGenres;
        }

        /// <summary>
        /// Retourne tous les rayons à partir de la BDD
        /// </summary>
        /// <returns>Collection d'objets Rayon</returns>
        public static List<Categorie> GetAllRayons()
        {
            List<Categorie> lesRayons = new List<Categorie>();
            string req = "Select * from rayon order by libelle";

            BddMySql curs = BddMySql.GetInstance(connectionString);
            curs.ReqSelect(req, null);

            while (curs.Read())
            {
                Rayon rayon = new Rayon((string)curs.Field("id"), (string)curs.Field("libelle"));
                lesRayons.Add(rayon);
            }
            curs.Close();
            return lesRayons;
        }

        /// <summary>
        /// Retourne toutes les catégories de public à partir de la BDD
        /// </summary>
        /// <returns>Collection d'objets Public</returns>
        public static List<Categorie> GetAllPublics()
        {
            List<Categorie> lesPublics = new List<Categorie>();
            string req = "Select * from public order by libelle";

            BddMySql curs = BddMySql.GetInstance(connectionString);
            curs.ReqSelect(req, null);

            while (curs.Read())
            {
                Public lePublic = new Public((string)curs.Field("id"), (string)curs.Field("libelle"));
                lesPublics.Add(lePublic);
            }
            curs.Close();
            return lesPublics;
        }

        /// <summary>
        /// Retourne toutes les livres à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Livre</returns>
        public static List<Livre> GetAllLivres()
        {
            List<Livre> lesLivres = new List<Livre>();
            string req = "Select l.id, l.ISBN, l.auteur, d.titre, d.image, l.collection, ";
            req += "d.idrayon, d.idpublic, d.idgenre, g.libelle as genre, p.libelle as public, r.libelle as rayon ";
            req += "from livre l join document d on l.id=d.id ";
            req += "join genre g on g.id=d.idGenre ";
            req += "join public p on p.id=d.idPublic ";
            req += "join rayon r on r.id=d.idRayon ";
            req += "order by titre ";

            BddMySql curs = BddMySql.GetInstance(connectionString);
            curs.ReqSelect(req, null);

            while (curs.Read())
            {
                string id = (string)curs.Field("id");
                string isbn = (string)curs.Field("ISBN");
                string auteur = (string)curs.Field("auteur");
                string titre = (string)curs.Field("titre");
                string image = (string)curs.Field("image");
                string collection = (string)curs.Field("collection");
                string idgenre = (string)curs.Field("idgenre");
                string idrayon = (string)curs.Field("idrayon");
                string idpublic = (string)curs.Field("idpublic");
                string genre = (string)curs.Field("genre");
                string lepublic = (string)curs.Field("public");
                string rayon = (string)curs.Field("rayon");
                Livre livre = new Livre(id, titre, image, isbn, auteur, collection, idgenre, genre, 
                    idpublic, lepublic, idrayon, rayon);
                lesLivres.Add(livre);
            }
            curs.Close();

            return lesLivres;
        }

        /// <summary>
        /// Retourne toutes les commandes de livres à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Livre</returns>
        public static List<Commande> GetAllCommandeLivresDvd(string type)
        {
            List<Commande> lesCommandesLivres = new List<Commande>();
            string req = "SELECT commandedocument.*, commande.dateCommande, commande.montant, suivi.id AS id_suivi, suivi.Etat, document.titre FROM `commandedocument` INNER JOIN commande ON commande.id=commandedocument.id INNER JOIN suivi ON commandedocument.idSuivi=suivi.id INNER JOIN document ON document.id=commandedocument.idLivreDvd WHERE commandedocument.type=\"" + type + "\"";

            BddMySql curs = BddMySql.GetInstance(connectionString);
            curs.ReqSelect(req, null);

            while (curs.Read())
            {
                string id = (string)curs.Field("id");
                DateTime dateCommande = (DateTime)curs.Field("dateCommande");
                double montant = (double)curs.Field("montant");
                int nbExemplaire = (int)curs.Field("nbExemplaire");
                string idLivreDvd = (string)curs.Field("idLivreDvd");
                int idSuivi = (int)curs.Field("id_suivi");
                string etat = (string)curs.Field("Etat");
                string titre = (string)curs.Field("titre");

                int idFinal = int.Parse(id);

                Commande commande = new Commande(idFinal, dateCommande, montant, nbExemplaire, idLivreDvd, idSuivi, etat, titre);
                lesCommandesLivres.Add(commande);
            }
            curs.Close();

            return lesCommandesLivres;
        }

        /// <summary>
        /// Retourne toutes les dvd à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Dvd</returns>
        public static List<Dvd> GetAllDvd()
        {
            List<Dvd> lesDvd = new List<Dvd>();
            string req = "Select l.id, l.duree, l.realisateur, d.titre, d.image, l.synopsis, ";
            req += "d.idrayon, d.idpublic, d.idgenre, g.libelle as genre, p.libelle as public, r.libelle as rayon ";
            req += "from dvd l join document d on l.id=d.id ";
            req += "join genre g on g.id=d.idGenre ";
            req += "join public p on p.id=d.idPublic ";
            req += "join rayon r on r.id=d.idRayon ";
            req += "order by titre ";

            BddMySql curs = BddMySql.GetInstance(connectionString);
            curs.ReqSelect(req, null);

            while (curs.Read())
            {
                string id = (string)curs.Field("id");
                int duree = (int)curs.Field("duree");
                string realisateur = (string)curs.Field("realisateur");
                string titre = (string)curs.Field("titre");
                string image = (string)curs.Field("image");
                string synopsis = (string)curs.Field("synopsis");
                string idgenre = (string)curs.Field("idgenre");
                string idrayon = (string)curs.Field("idrayon");
                string idpublic = (string)curs.Field("idpublic");
                string genre = (string)curs.Field("genre");
                string lepublic = (string)curs.Field("public");
                string rayon = (string)curs.Field("rayon");
                Dvd dvd = new Dvd(id, titre, image, duree, realisateur, synopsis, idgenre, genre,
                    idpublic, lepublic, idrayon, rayon);
                lesDvd.Add(dvd);
            }
            curs.Close();

            return lesDvd;
        }

        /// <summary>
        /// Retourne toutes les revues à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Revue</returns>
        public static List<Revue> GetAllRevues()
        {
            List<Revue> lesRevues = new List<Revue>();
            string req = "Select l.id, l.empruntable, l.periodicite, d.titre, d.image, l.delaiMiseADispo, ";
            req += "d.idrayon, d.idpublic, d.idgenre, g.libelle as genre, p.libelle as public, r.libelle as rayon ";
            req += "from revue l join document d on l.id=d.id ";
            req += "join genre g on g.id=d.idGenre ";
            req += "join public p on p.id=d.idPublic ";
            req += "join rayon r on r.id=d.idRayon ";
            req += "order by titre ";

            BddMySql curs = BddMySql.GetInstance(connectionString);
            curs.ReqSelect(req, null);

            while (curs.Read())
            {
                string id = (string)curs.Field("id");
                bool empruntable = (bool)curs.Field("empruntable");
                string periodicite = (string)curs.Field("periodicite");
                string titre = (string)curs.Field("titre");
                string image = (string)curs.Field("image");
                int delaiMiseADispo = (int)curs.Field("delaimiseadispo");
                string idgenre = (string)curs.Field("idgenre");
                string idrayon = (string)curs.Field("idrayon");
                string idpublic = (string)curs.Field("idpublic");
                string genre = (string)curs.Field("genre");
                string lepublic = (string)curs.Field("public");
                string rayon = (string)curs.Field("rayon");
                Revue revue = new Revue(id, titre, image, idgenre, genre,
                    idpublic, lepublic, idrayon, rayon, empruntable, periodicite, delaiMiseADispo);
                lesRevues.Add(revue);
            }
            curs.Close();

            return lesRevues;
        }

        /// <summary>
        /// Retourne les exemplaires d'une revue
        /// </summary>
        /// <returns>Liste d'objets Exemplaire</returns>
        public static List<Exemplaire> GetExemplairesRevue(string idDocument)
        {
            List<Exemplaire> lesExemplaires = new List<Exemplaire>();
            string req = "Select e.id, e.numero, e.dateAchat, e.photo, e.idEtat ";
            req += "from exemplaire e join document d on e.id=d.id ";
            req += "where e.id = @id ";
            req += "order by e.dateAchat DESC";
            Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@id", idDocument}
                };

            BddMySql curs = BddMySql.GetInstance(connectionString);
            curs.ReqSelect(req, parameters);

            while (curs.Read())
            {
                string idDocuement = (string)curs.Field("id");
                int numero = (int)curs.Field("numero");
                DateTime dateAchat = (DateTime)curs.Field("dateAchat");
                string photo = (string)curs.Field("photo");
                string idEtat = (string)curs.Field("idEtat");
                Exemplaire exemplaire = new Exemplaire(numero, dateAchat, photo, idEtat, idDocuement);
                lesExemplaires.Add(exemplaire);
            }
            curs.Close();

            return lesExemplaires;
        }

        /// <summary>
        /// ecriture d'un exemplaire en base de données
        /// </summary>
        /// <param name="exemplaire"></param>
        /// <returns>true si l'insertion a pu se faire</returns>
        public static bool CreerExemplaire(Exemplaire exemplaire)
        {
            try
            {
                string req = "insert into exemplaire values (@idDocument,@numero,@dateAchat,@photo,@idEtat)";
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@idDocument", exemplaire.IdDocument},
                    { "@numero", exemplaire.Numero},
                    { "@dateAchat", exemplaire.DateAchat},
                    { "@photo", exemplaire.Photo},
                    { "@idEtat",exemplaire.IdEtat}
                };
                BddMySql curs = BddMySql.GetInstance(connectionString);
                curs.ReqUpdate(req, parameters);
                curs.Close();
                return true;
            }catch{
                return false;
            }
        }

        /// <summary>
        /// Création d'un livre dans la base de donnée
        /// </summary>
        /// <param name="livre"></param>
        /// <returns>true si l'insertion a pu se faire</returns>
        public static bool CreerLivre(Livre livre)
        {
            try
            {
                string req = "insert into document values (@id, @titre, @image, @idRayon, @idPublic, @idGenre)";
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@id", livre.Id},
                    { "@titre", livre.Titre},
                    { "@image", livre.Image},
                    { "@idRayon", livre.IdRayon},
                    { "@idPublic", livre.IdPublic},
                    { "@idGenre", livre.IdGenre},
                };
                BddMySql curs = BddMySql.GetInstance(connectionString);
                curs.ReqUpdate(req, parameters);
                curs.Close();

                req = "insert into livres_dvd values (@id)";
                parameters = new Dictionary<string, object>
                {
                    { "@id", livre.Id},
                };
                curs = BddMySql.GetInstance(connectionString);
                curs.ReqUpdate(req, parameters);
                curs.Close();

                req = "insert into livre values (@id, @ISBN, @auteur, @collection)";
                parameters = new Dictionary<string, object>
                {
                    { "@id", livre.Id},
                    { "@ISBN", livre.Isbn},
                    { "@auteur", livre.Auteur},
                    { "@collection", livre.Collection},
                };
                curs = BddMySql.GetInstance(connectionString);
                curs.ReqUpdate(req, parameters);
                curs.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Suppression d'un livre dans la base de donnée
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="table"></param>
        /// <returns>true si la supression a pu se faire</returns>
        public static bool SupprimerBdd(string Id, string table)
        {
            try
            {
                if (checkCommand(Id) && checkExemplaire(Id))
                {
                    string req = "DELETE FROM " + table + " WHERE id = (@id)";
                    Dictionary<string, object> parameters = new Dictionary<string, object>
                    {
                        { "@id", Id}
                    };
                    BddMySql curs = BddMySql.GetInstance(connectionString);
                    curs.ReqUpdate(req, parameters);
                    curs.Close();

                    if (table != "revue")
                    {
                        req = "DELETE FROM livres_dvd WHERE id = (@id)";
                        parameters = new Dictionary<string, object>
                        {
                            { "@id", Id}
                        };
                        curs = BddMySql.GetInstance(connectionString);
                        curs.ReqUpdate(req, parameters);
                        curs.Close();

                        req = "DELETE FROM document WHERE id = (@id)";
                        parameters = new Dictionary<string, object>
                        {
                            { "@id", Id}
                        };
                        curs = BddMySql.GetInstance(connectionString);
                        curs.ReqUpdate(req, parameters);
                        curs.Close();

                        return true;
                    }
                    else
                    {
                        req = "DELETE FROM revue WHERE id = (@id)";
                        parameters = new Dictionary<string, object>
                        {
                            { "@id", Id}
                        };
                        curs = BddMySql.GetInstance(connectionString);
                        curs.ReqUpdate(req, parameters);
                        curs.Close();

                        req = "DELETE FROM document WHERE id = (@id)";
                        parameters = new Dictionary<string, object>
                        {
                            { "@id", Id}
                        };
                        curs = BddMySql.GetInstance(connectionString);
                        curs.ReqUpdate(req, parameters);
                        curs.Close();

                        return true;
                    }
                }
                else
                {
                    return false;
                }
                
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Modification d'un livre dans la base de donnée
        /// </summary>
        /// <param name="livre"></param>
        /// <returns>true si l'insertion a pu se faire</returns>
        public static bool UpdateLivre(Livre livre)
        {
            try
            {
                string req = "UPDATE document SET titre=(@titre), image=(@image), idRayon=(@idRayon), idPublic=(@idPublic), idGenre=(@idGenre) WHERE id=(@id)";
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@titre", livre.Titre},
                    { "@image", livre.Image},
                    { "@idRayon", livre.IdRayon},
                    { "@idPublic", livre.IdPublic},
                    { "@idGenre", livre.IdGenre},
                    { "@id", livre.Id},
                };
                BddMySql curs = BddMySql.GetInstance(connectionString);
                curs.ReqUpdate(req, parameters);
                curs.Close();

                req = "UPDATE livre SET ISBN=@ISBN, auteur=@auteur, collection=@collection WHERE id=(@id)";
                parameters = new Dictionary<string, object>
                {
                    { "@ISBN", livre.Isbn},
                    { "@auteur", livre.Auteur},
                    { "@collection", livre.Collection},
                    { "@id", livre.Id},
                };
                curs = BddMySql.GetInstance(connectionString);
                curs.ReqUpdate(req, parameters);
                curs.Close();

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Création d'un Dvd dans la base de donnée
        /// </summary>
        /// <param name="dvd"></param>
        /// <returns>true si l'insertion a pu se faire</returns>
        public static bool CreerDvd(Dvd dvd)
        {
            try
            {
                string req = "insert into document values (@id, @titre, @image, @idRayon, @idPublic, @idGenre)";
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@id", dvd.Id},
                    { "@titre", dvd.Titre},
                    { "@image", dvd.Image},
                    { "@idRayon", dvd.IdRayon},
                    { "@idPublic", dvd.IdPublic},
                    { "@idGenre", dvd.IdGenre},
                };
                BddMySql curs = BddMySql.GetInstance(connectionString);
                curs.ReqUpdate(req, parameters);
                curs.Close();

                req = "insert into livres_dvd values (@id)";
                parameters = new Dictionary<string, object>
                {
                    { "@id", dvd.Id},
                };
                curs = BddMySql.GetInstance(connectionString);
                curs.ReqUpdate(req, parameters);
                curs.Close();

                req = "insert into dvd values (@id, @synopsis, @realisateur, @duree)";
                parameters = new Dictionary<string, object>
                {
                    { "@id", dvd.Id},
                    { "@synopsis", dvd.Synopsis},
                    { "@realisateur", dvd.Realisateur},
                    { "@duree", dvd.Duree},
                };
                curs = BddMySql.GetInstance(connectionString);
                curs.ReqUpdate(req, parameters);
                curs.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Modification d'un Dvd dans la base de donnée
        /// </summary>
        /// <param name="dvd"></param>
        /// <returns>true si la modification a pu se faire</returns>
        public static bool UpdateDvd(Dvd dvd)
        {
            try
            {
                string req = "UPDATE document SET titre=(@titre), image=(@image), idRayon=(@idRayon), idPublic=(@idPublic), idGenre=(@idGenre) WHERE id=(@id)";
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@titre", dvd.Titre},
                    { "@image", dvd.Image},
                    { "@idRayon", dvd.IdRayon},
                    { "@idPublic", dvd.IdPublic},
                    { "@idGenre", dvd.IdGenre},
                    { "@id", dvd.Id},
                };
                BddMySql curs = BddMySql.GetInstance(connectionString);
                curs.ReqUpdate(req, parameters);
                curs.Close();

                req = "UPDATE dvd SET synopsis=@synopsis, realisateur=@realisateur, duree=@duree WHERE id=(@id)";
                parameters = new Dictionary<string, object>
                {
                    { "@synopsis", dvd.Synopsis},
                    { "@realisateur", dvd.Realisateur},
                    { "@duree", dvd.Duree},
                    { "@id", dvd.Id},
                };
                curs = BddMySql.GetInstance(connectionString);
                curs.ReqUpdate(req, parameters);
                curs.Close();

                return true;
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// Création d'une revue dans la base de donnée
        /// </summary>
        /// <param name="revue"></param>
        /// <returns>true si l'insertion a pu se faire</returns>
        public static bool CreerRevue(Revue revue)
        {
            try
            {
                string req = "insert into document values (@id, @titre, @image, @idRayon, @idPublic, @idGenre)";
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@id", revue.Id},
                    { "@titre", revue.Titre},
                    { "@image", revue.Image},
                    { "@idRayon", revue.IdRayon},
                    { "@idPublic", revue.IdPublic},
                    { "@idGenre", revue.IdGenre},
                };
                BddMySql curs = BddMySql.GetInstance(connectionString);
                curs.ReqUpdate(req, parameters);
                curs.Close();

                int check = 0;
                if (revue.Empruntable)
                {
                    check = 1;
                }

                req = "insert into revue values (@id, @empruntable, @periodicite, @delaiMiseADispo)";
                parameters = new Dictionary<string, object>
                {
                    { "@id", revue.Id},
                    { "@empruntable", check},
                    { "@periodicite", revue.Periodicite},
                    { "@delaiMiseADispo", revue.DelaiMiseADispo},
                };
                curs = BddMySql.GetInstance(connectionString);
                curs.ReqUpdate(req, parameters);
                curs.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Modification d'une Revue dans la base de donnée
        /// </summary>
        /// <param name="revue"></param>
        /// <returns>true si la modification a pu se faire</returns>
        public static bool UpdateRevue(Revue revue)
        {
            try
            {
                string req = "UPDATE document SET titre=(@titre), image=(@image), idRayon=(@idRayon), idPublic=(@idPublic), idGenre=(@idGenre) WHERE id=(@id)";
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@titre", revue.Titre},
                    { "@image", revue.Image},
                    { "@idRayon", revue.IdRayon},
                    { "@idPublic", revue.IdPublic},
                    { "@idGenre", revue.IdGenre},
                    { "@id", revue.Id},
                };
                BddMySql curs = BddMySql.GetInstance(connectionString);
                curs.ReqUpdate(req, parameters);
                curs.Close();

                int check = 0;
                if (revue.Empruntable)
                {
                    check = 1;
                }

                req = "UPDATE revue SET empruntable=@empruntable, periodicite=@periodicite, delaiMiseADispo=@delaiMiseADispo WHERE id=(@id)";
                parameters = new Dictionary<string, object>
                {
                    { "@empruntable", check},
                    { "@periodicite", revue.Periodicite},
                    { "@delaiMiseADispo", revue.DelaiMiseADispo},
                    { "@id", revue.Id},
                };
                curs = BddMySql.GetInstance(connectionString);
                curs.ReqUpdate(req, parameters);
                curs.Close();

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Modification de l'etat de livraison dans la base de donnée
        /// </summary>
        /// <param name="commande"></param>
        /// <returns>true si la modification a pu se faire</returns>
        public static bool ModifyEtatCommande(Commande commande, string etat)
        {
            try
            {
                string req = "UPDATE commandedocument SET idsuivi=(@idsuivi) WHERE id=(@id)";
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@idsuivi", etat},
                    { "@id", commande.Id},
                };
                BddMySql curs = BddMySql.GetInstance(connectionString);
                curs.ReqUpdate(req, parameters);
                curs.Close();

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Création d'une revue dans la base de donnée
        /// </summary>
        /// <param name="revue"></param>
        /// <returns>true si l'insertion a pu se faire</returns>
        public static bool CreerCommande(Commande commande, Livre livre)
        {
            try
            {
                string req = "insert into commande values (@id, @dateCommande, @montant)";
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@id", commande.Id},
                    { "@dateCommande", commande.DateCommande},
                    { "@montant", commande.Montant},
                };
                BddMySql curs = BddMySql.GetInstance(connectionString);
                curs.ReqUpdate(req, parameters);
                curs.Close();

                req = "insert into commandedocument values (@id, @nbExemplaire, @idLivreDvd, @idSuivi, @type)";
                parameters = new Dictionary<string, object>
                {
                    { "@id", commande.Id},
                    { "@nbExemplaire", commande.NbExemplaire},
                    { "@idLivreDvd", livre.Id},
                    { "@idSuivi", commande.IdSuivi},
                    { "@type", "livre"},
                };
                curs = BddMySql.GetInstance(connectionString);
                curs.ReqUpdate(req, parameters);
                curs.Close();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool checkCommand(string Id)
        {
            int y = 0; 
            string req = "Select * from commandedocument WHERE idLivreDvd=(@id)";
            Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@id", Id},
                };
            BddMySql curs = BddMySql.GetInstance(connectionString);
            curs.ReqSelect(req, parameters);

            while (curs.Read())
            {
                y++;
            }

            curs.Close();
            if (y == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool checkAbonnement(string Id)
        {
            int y = 0;
            string req = "Select * from abonnement WHERE idRevue=(@id)";
            Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@id", Id},
                };
            BddMySql curs = BddMySql.GetInstance(connectionString);
            curs.ReqSelect(req, parameters);

            while (curs.Read())
            {
                y++;
            }

            curs.Close();
            if (y == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool checkExemplaire(string Id)
        {
            int y = 0;
            string req = "Select * from exemplaire WHERE id=(@id)";
            Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@id", Id},
                };
            BddMySql curs = BddMySql.GetInstance(connectionString);
            curs.ReqSelect(req, parameters);

            while (curs.Read())
            {
                y++;
            }

            curs.Close();
            if (y == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static Livre selectLivreById(string Id)
        {
            Livre livre = new Livre("", "", "", "", "", "", "", "", "", "","", "");
            string req = "Select l.id, l.ISBN, l.auteur, d.titre, d.image, l.collection, d.idrayon, d.idpublic, d.idgenre, g.libelle as genre, p.libelle as public, r.libelle as rayon from livre l join document d on l.id=d.id join genre g on g.id=d.idGenre join public p on p.id=d.idPublic join rayon r on r.id=d.idRayon WHERE l.id="+Id+" order by titre;";

            BddMySql curs = BddMySql.GetInstance(connectionString);
            curs.ReqSelect(req, null);

            if (curs.Read())
            {
                string id = (string)curs.Field("id");
                string isbn = (string)curs.Field("ISBN");
                string auteur = (string)curs.Field("auteur");
                string titre = (string)curs.Field("titre");
                string image = (string)curs.Field("image");
                string collection = (string)curs.Field("collection");
                string idgenre = (string)curs.Field("idgenre");
                string idrayon = (string)curs.Field("idrayon");
                string idpublic = (string)curs.Field("idpublic");
                string genre = (string)curs.Field("genre");
                string lepublic = (string)curs.Field("public");
                string rayon = (string)curs.Field("rayon");
                livre = new Livre(id, titre, image, isbn, auteur, collection, idgenre, genre,
                    idpublic, lepublic, idrayon, rayon);
            }

            curs.Close();
            return livre;
        }

        /// <summary>
        /// Suppression d'un livre dans la base de donnée
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="table"></param>
        /// <returns>true si la supression a pu se faire</returns>
        public static bool DeleteCommande(Commande commande)
        {
            try
            {
                string req = "DELETE FROM commandedocument WHERE id = (@id)";
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@id", commande.Id}
                };
                BddMySql curs = BddMySql.GetInstance(connectionString);
                curs.ReqUpdate(req, parameters);
                curs.Close();

                req = "DELETE FROM commande WHERE id = (@id)";
                parameters = new Dictionary<string, object>
                {
                    { "@id", commande.Id}
                };
                curs = BddMySql.GetInstance(connectionString);
                curs.ReqUpdate(req, parameters);
                curs.Close();

                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
