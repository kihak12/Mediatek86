using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Mediatek86.metier;
using Mediatek86.controleur;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;

namespace Mediatek86.vue
{
    public partial class FrmMediatek : Form
    {

        #region Variables globales

        private readonly Controle controle;
        const string ETATNEUF = "00001";

        private readonly BindingSource bdgLivresListe = new BindingSource();
        private readonly BindingSource bdgDvdListe = new BindingSource();
        private readonly BindingSource bdgCommandeLivresListe = new BindingSource();
        private readonly BindingSource bdgCommandesLivresListeBdd = new BindingSource();
        private readonly BindingSource bdgCommandeDvdsListe = new BindingSource();
        private readonly BindingSource bdgCommandeDvdsListeBdd = new BindingSource();
        private readonly BindingSource bdgGenres = new BindingSource();
        private readonly BindingSource bdgPublics = new BindingSource();
        private readonly BindingSource bdgRayons = new BindingSource();
        private readonly BindingSource bdgGenresModif = new BindingSource();
        private readonly BindingSource bdgPublicsModif = new BindingSource();
        private readonly BindingSource bdgRayonsModif = new BindingSource();
        private readonly BindingSource bdgRevuesListe = new BindingSource();
        private readonly BindingSource bdgCommandeRevueListe = new BindingSource();
        private readonly BindingSource bdgExemplairesListe = new BindingSource();
        private List<Livre> lesLivres = new List<Livre>();
        private List<Commande> lesCommandesLivres = new List<Commande>();
        private List<Commande> lesCommandesDvds = new List<Commande>();
        private List<Dvd> lesDvd = new List<Dvd>();
        private List<Revue> lesRevues = new List<Revue>();
        private List<Exemplaire> lesExemplaires = new List<Exemplaire>();
        private int start = 0;
        private int userDroits = 0;

        #endregion


        internal FrmMediatek(Controle controle, int droits)
        {
            InitializeComponent();
            this.controle = controle;
            setDroits(droits);
        }

        /// <summary>
        /// Modifie l'accessibilité au panel selon les droits
        /// </summary>
        /// <param name="droits"></param>
        public void setDroits(int droits)
        {
            if(droits == 2)
            {
                ((Control)this.tabCommandeLivres).Enabled = false;
                ((Control)this.tabCommandeDvd).Enabled = false;
                ((Control)this.tabCommandeRevues).Enabled = false;
                groupBox1.Enabled = false;
                groupBox2.Enabled = false;
                groupBox3.Enabled = false;
            }
            userDroits = droits;
        }


        #region modules communs

        /// <summary>
        /// Rempli un des 3 combo (genre, public, rayon)
        /// </summary>
        /// <param name="lesCategories"></param>
        /// <param name="bdg"></param>
        /// <param name="cbx"></param>
        public void RemplirComboCategorie(List<Categorie> lesCategories, BindingSource bdg, ComboBox cbx)
        {
            bdg.DataSource = lesCategories;
            cbx.DataSource = bdg;
            if (cbx.Items.Count > 0)
            {
                cbx.SelectedIndex = -1;
            }
        }

        #endregion


        #region Revues
        //-----------------------------------------------------------
        // ONGLET "Revues"
        //------------------------------------------------------------

        /// <summary>
        /// Ouverture de l'onglet Revues : 
        /// appel des méthodes pour remplir le datagrid des revues et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabRevues_Enter(object sender, EventArgs e)
        {
            lesRevues = controle.GetAllRevues();
            RemplirComboCategorie(controle.GetAllGenres(), bdgGenres, cbxRevuesGenres);
            RemplirComboCategorie(controle.GetAllPublics(), bdgPublics, cbxRevuesPublics);
            RemplirComboCategorie(controle.GetAllRayons(), bdgRayons, cbxRevuesRayons);

            RemplirComboCategorie(controle.GetAllGenres(), bdgGenresModif, cbxRevuesGenresModif);
            RemplirComboCategorie(controle.GetAllPublics(), bdgPublicsModif, cbxRevuesPublicsModif);
            RemplirComboCategorie(controle.GetAllRayons(), bdgRayonsModif, cbxRevuesRayonsModif);

            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        private void RemplirRevuesListe(List<Revue> revues)
        {
            bdgRevuesListe.DataSource = revues;
            dgvRevuesListe.DataSource = bdgRevuesListe;
            dgvRevuesListe.Columns["empruntable"].Visible = false;
            dgvRevuesListe.Columns["idRayon"].Visible = false;
            dgvRevuesListe.Columns["idGenre"].Visible = false;
            dgvRevuesListe.Columns["idPublic"].Visible = false;
            dgvRevuesListe.Columns["image"].Visible = false;
            dgvRevuesListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvRevuesListe.Columns["id"].DisplayIndex = 0;
            dgvRevuesListe.Columns["titre"].DisplayIndex = 1;
        }


        /// <summary>
        /// Recherche et affichage de la revue dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbRevuesNumRecherche.Text.Equals(""))
            {
                txbRevuesTitreRecherche.Text = "";
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
                Revue revue = lesRevues.Find(x => x.Id.Equals(txbRevuesNumRecherche.Text));
                if (revue != null)
                {
                    List<Revue> revues = new List<Revue>();
                    revues.Add(revue);
                    RemplirRevuesListe(revues);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirRevuesListeComplete();
                }
            }
            else
            {
                RemplirRevuesListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des revues dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbRevuesTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbRevuesTitreRecherche.Text.Equals(""))
            {
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
                txbRevuesNumRecherche.Text = "";
                List<Revue> lesRevuesParTitre;
                lesRevuesParTitre = lesRevues.FindAll(x => x.Titre.ToLower().Contains(txbRevuesTitreRecherche.Text.ToLower()));
                RemplirRevuesListe(lesRevuesParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxRevuesGenres.SelectedIndex < 0 && cbxRevuesPublics.SelectedIndex < 0 && cbxRevuesRayons.SelectedIndex < 0
                    && txbRevuesNumRecherche.Text.Equals(""))
                {
                    RemplirRevuesListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations de la revue sélectionné
        /// </summary>
        /// <param name="revue"></param>
        private void AfficheRevuesInfos(Revue revue)
        {
            txbRevuesPeriodicite.Text = revue.Periodicite;
            chkRevuesEmpruntable.Checked = revue.Empruntable;
            txbRevuesImage.Text = revue.Image;
            txbRevuesDateMiseADispo.Text = revue.DelaiMiseADispo.ToString();
            txbRevuesNumero.Text = revue.Id;
            txbRevuesGenre.Text = revue.Genre;
            txbRevuesPublic.Text = revue.Public;
            txbRevuesRayon.Text = revue.Rayon;
            txbRevuesTitre.Text = revue.Titre;
            txbRevuesNumeroModif.Text = revue.Id;
            txbRevuesTitreModif.Text = revue.Titre;
            txbRevuesPeriodiciteModif.Text = revue.Periodicite;
            txbRevuesDateMiseADispoModif.Text = revue.DelaiMiseADispo.ToString();
            chkRevuesEmpruntableModif.Checked = revue.Empruntable;
            txbRevuesImageModif.Text = revue.Image;
            cbxRevuesPublicsModif.Text = "";
            cbxRevuesRayonsModif.Text = "";
            cbxRevuesGenresModif.Text = "";
            cbxRevuesPublicsModif.SelectedText = revue.Public;
            cbxRevuesRayonsModif.SelectedText = revue.Rayon;
            cbxRevuesGenresModif.SelectedText = revue.Genre;

            string image = revue.Image;
            try
            {
                pcbRevuesImage.Image = Image.FromFile(image);
            }
            catch 
            { 
                pcbRevuesImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations de la reuve
        /// </summary>
        private void VideRevuesInfos()
        {
            txbRevuesPeriodicite.Text = "";
            chkRevuesEmpruntable.Checked = false;
            txbRevuesImage.Text = "";
            txbRevuesDateMiseADispo.Text = "";
            txbRevuesNumero.Text = "";
            txbRevuesGenre.Text = "";
            txbRevuesPublic.Text = "";
            txbRevuesRayon.Text = "";
            txbRevuesTitre.Text = "";
            pcbRevuesImage.Image = null;
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesGenres.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Genre genre = (Genre)cbxRevuesGenres.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesPublics.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Public lePublic = (Public)cbxRevuesPublics.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesRayons.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxRevuesRayons.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations de la revue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvRevuesListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvRevuesListe.CurrentCell != null)
            {
                try
                {
                    Revue revue = (Revue)bdgRevuesListe.List[bdgRevuesListe.Position];
                    AfficheRevuesInfos(revue);
                }
                catch
                {
                    VideRevuesZones();
                }
            }
            else
            {
                VideRevuesInfos();
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des revues
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirRevuesListeComplete()
        {
            RemplirRevuesListe(lesRevues);
            VideRevuesZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideRevuesZones()
        {
            cbxRevuesGenres.SelectedIndex = -1;
            cbxRevuesRayons.SelectedIndex = -1;
            cbxRevuesPublics.SelectedIndex = -1;
            txbRevuesNumRecherche.Text = "";
            txbRevuesTitreRecherche.Text = "";
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvRevuesListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideRevuesZones();
            string titreColonne = dgvRevuesListe.Columns[e.ColumnIndex].HeaderText;
            List<Revue> sortedList = new List<Revue>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesRevues.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesRevues.OrderBy(o => o.Titre).ToList();
                    break;
                case "Periodicite":
                    sortedList = lesRevues.OrderBy(o => o.Periodicite).ToList();
                    break;
                case "DelaiMiseADispo":
                    sortedList = lesRevues.OrderBy(o => o.DelaiMiseADispo).ToList();
                    break;
                case "Genre":
                    sortedList = lesRevues.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesRevues.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesRevues.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirRevuesListe(sortedList);
        }

        private void BtnRevuesAjout_Click(object sender, EventArgs e)
        {
            string rayonId = getRayonId(cbxRevuesRayonsModif.Text);
            string genreId = getGenresId(cbxRevuesGenresModif.Text);
            string publicId = getPublicId(cbxRevuesPublicsModif.Text);
            Revue revue = new Revue(
                txbRevuesNumeroModif.Text,
                txbRevuesTitreModif.Text,
                txbRevuesImageModif.Text,
                genreId,
                cbxDvdGenresModif.Text,
                publicId,
                cbxDvdPublicsModif.Text,
                rayonId,
                cbxDvdRayonsModif.Text,
                chkRevuesEmpruntableModif.Checked,
                txbRevuesPeriodiciteModif.Text,
                int.Parse(txbRevuesDateMiseADispoModif.Text)
                );

            if (controle.CreerRevue(revue))
            {
                MessageBox.Show("Revue ajouté avec success");
            }
            else
            {
                MessageBox.Show("Une revue porte déjà ce numéro");
            }
            lesRevues = controle.GetAllRevues();
            RemplirRevuesListeComplete();
        }

        private void BtnRevueSuppr_Click(object sender, EventArgs e)
        {
            if ((MessageBox.Show("Êtes vous sur de vouloir supprimé cette revue ?", "Suppression d'une revue", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes))
            {

                if (controle.SupprimerBdd(txbRevuesNumero.Text, "revue"))
                {
                    MessageBox.Show("Revue supprimé avec success");
                }
                else
                {
                    MessageBox.Show("Echec de la supression, Cette revue posséde une commande ou un exemplaire.");
                }
            }
            lesRevues = controle.GetAllRevues();
            RemplirRevuesListeComplete();
        }

        private void BtnRevuesModif_Click(object sender, EventArgs e)
        {
            string rayonId = getRayonId(cbxRevuesRayonsModif.Text);
            string genreId = getGenresId(cbxRevuesGenresModif.Text);
            string publicId = getPublicId(cbxRevuesPublicsModif.Text);
            Revue revue = new Revue(
                txbRevuesNumero.Text,
                txbRevuesTitreModif.Text,
                txbRevuesImageModif.Text,
                genreId,
                cbxDvdGenresModif.Text,
                publicId,
                cbxDvdPublicsModif.Text,
                rayonId,
                cbxDvdRayonsModif.Text,
                chkRevuesEmpruntableModif.Checked,
                txbRevuesPeriodiciteModif.Text,
                int.Parse(txbRevuesDateMiseADispoModif.Text)
                );
            if ((MessageBox.Show("Êtes vous sur de vouloir Modifier cette revue ?\nSi Le numéro de document a été modifier, il ne sera pas pris en compte, il conservera celui d'origine.", "Modification d'une revue", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes))
            {
                if (controle.UpdateRevue(revue))
                {
                    MessageBox.Show("Revue modifier avec success");
                }
                else
                {
                    MessageBox.Show("Erreur lors de la modification");
                }
            }
            lesRevues = controle.GetAllRevues();
            RemplirRevuesListeComplete();
        }

        #endregion


        #region Livres

        //-----------------------------------------------------------
        // ONGLET "LIVRES"
        //-----------------------------------------------------------

        /// <summary>
        /// Ouverture de l'onglet Livres : 
        /// appel des méthodes pour remplir le datagrid des livres et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabLivres_Enter(object sender, EventArgs e)
        {
            lesLivres = controle.GetAllLivres();
            RemplirComboCategorie(controle.GetAllGenres(), bdgGenres, cbxLivresGenres);
            RemplirComboCategorie(controle.GetAllPublics(), bdgPublics, cbxLivresPublics);
            RemplirComboCategorie(controle.GetAllRayons(), bdgRayons, cbxLivresRayons);
            RemplirComboCategorie(controle.GetAllGenres(), bdgGenresModif, cbxLivresGenresModif);
            RemplirComboCategorie(controle.GetAllPublics(), bdgPublicsModif, cbxLivresPublicsModif);
            RemplirComboCategorie(controle.GetAllRayons(), bdgRayonsModif, cbxLivresRayonsModif);
            RemplirLivresListeComplete();

            if(userDroits != 2)
            {
                FrmAbonnement frmAbonnement = new FrmAbonnement();
                if (start == 0 && frmAbonnement.affect())
                {
                    frmAbonnement.ShowDialog();
                    start = 1;
                }
            }

        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        private void RemplirLivresListe(List<Livre> livres)
        {
            bdgLivresListe.DataSource = livres;
            dgvLivresListe.DataSource = bdgLivresListe;
            dgvLivresListe.Columns["isbn"].Visible = false;
            dgvLivresListe.Columns["idRayon"].Visible = false;
            dgvLivresListe.Columns["idGenre"].Visible = false;
            dgvLivresListe.Columns["idPublic"].Visible = false;
            dgvLivresListe.Columns["image"].Visible = false;
            dgvLivresListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvLivresListe.Columns["id"].DisplayIndex = 0;
            dgvLivresListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Recherche et affichage du livre dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbLivresNumRecherche.Text.Equals(""))
            {
                txbLivresTitreRecherche.Text = "";
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
                Livre livre = lesLivres.Find(x => x.Id.Equals(txbLivresNumRecherche.Text));
                if (livre != null)
                {
                    List<Livre> livres = new List<Livre>();
                    livres.Add(livre);
                    RemplirLivresListe(livres);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirLivresListeComplete();
                }
            }
            else
            {
                RemplirLivresListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des livres dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxbLivresTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbLivresTitreRecherche.Text.Equals(""))
            {
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
                txbLivresNumRecherche.Text = "";
                List<Livre> lesLivresParTitre;
                lesLivresParTitre = lesLivres.FindAll(x => x.Titre.ToLower().Contains(txbLivresTitreRecherche.Text.ToLower()));
                RemplirLivresListe(lesLivresParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxLivresGenres.SelectedIndex < 0 && cbxLivresPublics.SelectedIndex < 0 && cbxLivresRayons.SelectedIndex < 0 
                    && txbLivresNumRecherche.Text.Equals(""))
                {
                    RemplirLivresListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations du livre sélectionné
        /// </summary>
        /// <param name="livre"></param>
        private void AfficheLivresInfos(Livre livre)
        {
            txbLivresAuteur.Text = livre.Auteur;
            txbLivresCollection.Text = livre.Collection;
            txbLivresImage.Text = livre.Image;
            txbLivresIsbn.Text = livre.Isbn;
            txbLivresNumero.Text = livre.Id;
            txbLivresGenre.Text = livre.Genre;
            txbLivresPublic.Text = livre.Public;
            txbLivresRayon.Text = livre.Rayon;
            txbLivresTitre.Text = livre.Titre;   
            string image = livre.Image;

            txbLivresAuteurModif.Text = livre.Auteur;
            txbLivresCollectionModif.Text = livre.Collection;
            txbLivresImageModif.Text = livre.Image;
            txbLivresIsbnModif.Text = livre.Isbn;
            txbLivresNumeroModif.Text = livre.Id;
            txbLivresTitreModif.Text = livre.Titre;

            cbxLivresPublicsModif.Text = "";
            cbxLivresRayonsModif.Text = "";
            cbxLivresGenresModif.Text = "";
            cbxLivresPublicsModif.SelectedText = livre.Public;
            cbxLivresRayonsModif.SelectedText = livre.Rayon;
            cbxLivresGenresModif.SelectedText = livre.Genre;

            try
            {
                pcbLivresImage.Image = Image.FromFile(image);
            }
            catch 
            {
                pcbLivresImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations du livre
        /// </summary>
        private void VideLivresInfos()
        {
            txbLivresAuteur.Text = "";
            txbLivresCollection.Text = "";
            txbLivresImage.Text = "";
            txbLivresIsbn.Text = "";
            txbLivresNumero.Text = "";
            txbLivresGenre.Text = "";
            txbLivresPublic.Text = "";
            txbLivresRayon.Text = "";
            txbLivresTitre.Text = "";
            pcbLivresImage.Image = null;
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresGenres.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Genre genre = (Genre)cbxLivresGenres.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirLivresListe(livres);
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresPublics.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Public lePublic = (Public)cbxLivresPublics.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirLivresListe(livres);
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresRayons.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxLivresRayons.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirLivresListe(livres);
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations du livre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvLivresListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvLivresListe.CurrentCell != null)
            {
                try
                {
                    Livre livre = (Livre)bdgLivresListe.List[bdgLivresListe.Position];
                    AfficheLivresInfos(livre);
                }
                catch
                {
                    VideLivresZones();
                }
            }
            else
            {
                VideLivresInfos();
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des livres
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirLivresListeComplete()
        {
            RemplirLivresListe(lesLivres);
            VideLivresZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideLivresZones()
        {
            cbxLivresGenres.SelectedIndex = -1;
            cbxLivresRayons.SelectedIndex = -1;
            cbxLivresPublics.SelectedIndex = -1;
            txbLivresNumRecherche.Text = "";
            txbLivresTitreRecherche.Text = "";
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvLivresListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideLivresZones();
            string titreColonne = dgvLivresListe.Columns[e.ColumnIndex].HeaderText;
            List<Livre> sortedList = new List<Livre>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesLivres.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesLivres.OrderBy(o => o.Titre).ToList();
                    break;
                case "Collection":
                    sortedList = lesLivres.OrderBy(o => o.Collection).ToList();
                    break;
                case "Auteur":
                    sortedList = lesLivres.OrderBy(o => o.Auteur).ToList();
                    break;
                case "Genre":
                    sortedList = lesLivres.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesLivres.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesLivres.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirLivresListe(sortedList);
        }

        private void BtnLivresAjout_Click(object sender, EventArgs e)
        {
            string rayonId = getRayonId(cbxLivresRayonsModif.Text);
            string genreId = getGenresId(cbxLivresGenresModif.Text);
            string publicId = getPublicId(cbxLivresPublicsModif.Text);
            Livre livre = new Livre(
                txbLivresNumeroModif.Text,
                txbLivresTitreModif.Text,
                txbLivresImageModif.Text,
                txbLivresIsbnModif.Text,
                txbLivresAuteurModif.Text,
                txbLivresCollectionModif.Text,
                genreId,
                cbxLivresGenresModif.Text,
                publicId,
                cbxLivresPublicsModif.Text,
                rayonId,
                cbxLivresRayonsModif.Text
                );
            if (controle.CreerLivre(livre))
            {
                MessageBox.Show("Livre ajouté avec success");
            }
            else
            {
                MessageBox.Show("Un livre porte déjà cet Id");
            }

            lesLivres = controle.GetAllLivres();
            RemplirLivresListeComplete();

        }

        private void BtnLivresSuppr_Click(object sender, EventArgs e)
        {

            if ((MessageBox.Show("Êtes vous sur de vouloir supprimé ce livre ?", "Suppression d'un livre", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes))
            {

                if (controle.SupprimerBdd(txbLivresNumero.Text, "livre"))
                {
                    MessageBox.Show("Livre supprimé avec success");
                }
                else
                {
                    MessageBox.Show("Echec de la supression, ce livre doit posséder une commande.");
                }

            }
            lesLivres = controle.GetAllLivres();
            RemplirLivresListeComplete();
        }

        private void BtnLivresModif_Click(object sender, EventArgs e)
        {
            string rayonId = getRayonId(cbxLivresRayonsModif.Text);
            string genreId = getGenresId(cbxLivresGenresModif.Text);
            string publicId = getPublicId(cbxLivresPublicsModif.Text);
            Livre livre = new Livre(
                txbLivresNumero.Text,
                txbLivresTitreModif.Text,
                txbLivresImageModif.Text,
                txbLivresIsbnModif.Text,
                txbLivresAuteurModif.Text,
                txbLivresCollectionModif.Text,
                genreId,
                cbxLivresGenresModif.Text,
                publicId,
                cbxLivresPublicsModif.Text,
                rayonId,
                cbxLivresRayonsModif.Text
                );
            if ((MessageBox.Show("Êtes vous sur de vouloir Modifier ce livre ?\nLe numéro de document saisie ne sera pas pris en compte, il conservera celui d'origine.", "Modification d'un livre", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes))
            {
                if (controle.UpdateLivre(livre))
                {
                    MessageBox.Show("Livre Modifier avec success");
                }
                else
                {
                    MessageBox.Show("Erreur lors de la modification");
                }
            }
            lesLivres = controle.GetAllLivres();
            RemplirLivresListeComplete();

        }

        #endregion


        #region Dvd
        //-----------------------------------------------------------
        // ONGLET "DVD"
        //-----------------------------------------------------------

        /// <summary>
        /// Ouverture de l'onglet Dvds : 
        /// appel des méthodes pour remplir le datagrid des dvd et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabDvd_Enter(object sender, EventArgs e)
        {
            lesDvd = controle.GetAllDvd();
            RemplirComboCategorie(controle.GetAllGenres(), bdgGenres, cbxDvdGenres);
            RemplirComboCategorie(controle.GetAllPublics(), bdgPublics, cbxDvdPublics);
            RemplirComboCategorie(controle.GetAllRayons(), bdgRayons, cbxDvdRayons);
            RemplirComboCategorie(controle.GetAllGenres(), bdgGenresModif, cbxDvdGenresModif);
            RemplirComboCategorie(controle.GetAllPublics(), bdgPublicsModif, cbxDvdPublicsModif);
            RemplirComboCategorie(controle.GetAllRayons(), bdgRayonsModif, cbxDvdRayonsModif);
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        private void RemplirDvdListe(List<Dvd> Dvds)
        {
            bdgDvdListe.DataSource = Dvds;
            dgvDvdListe.DataSource = bdgDvdListe;
            dgvDvdListe.Columns["idRayon"].Visible = false;
            dgvDvdListe.Columns["idGenre"].Visible = false;
            dgvDvdListe.Columns["idPublic"].Visible = false;
            dgvDvdListe.Columns["image"].Visible = false;
            dgvDvdListe.Columns["synopsis"].Visible = false;
            dgvDvdListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvDvdListe.Columns["id"].DisplayIndex = 0;
            dgvDvdListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Recherche et affichage du Dvd dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbDvdNumRecherche.Text.Equals(""))
            {
                txbDvdTitreRecherche.Text = "";
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
                Dvd dvd = lesDvd.Find(x => x.Id.Equals(txbDvdNumRecherche.Text));
                if (dvd != null)
                {
                    List<Dvd> Dvd = new List<Dvd>();
                    Dvd.Add(dvd);
                    RemplirDvdListe(Dvd);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirDvdListeComplete();
                }
            }
            else
            {
                RemplirDvdListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des Dvd dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbDvdTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbDvdTitreRecherche.Text.Equals(""))
            {
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
                txbDvdNumRecherche.Text = "";
                List<Dvd> lesDvdParTitre;
                lesDvdParTitre = lesDvd.FindAll(x => x.Titre.ToLower().Contains(txbDvdTitreRecherche.Text.ToLower()));
                RemplirDvdListe(lesDvdParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxDvdGenres.SelectedIndex < 0 && cbxDvdPublics.SelectedIndex < 0 && cbxDvdRayons.SelectedIndex < 0
                    && txbDvdNumRecherche.Text.Equals(""))
                {
                    RemplirDvdListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations du dvd sélectionné
        /// </summary>
        /// <param name="dvd"></param>
        private void AfficheDvdInfos(Dvd dvd)
        {
            txbDvdRealisateur.Text = dvd.Realisateur;
            txbDvdSynopsis.Text = dvd.Synopsis;
            txbDvdImage.Text = dvd.Image;
            txbDvdDuree.Text = dvd.Duree.ToString() ;
            txbDvdNumero.Text = dvd.Id;
            txbDvdGenre.Text = dvd.Genre;
            txbDvdPublic.Text = dvd.Public;
            txbDvdRayon.Text = dvd.Rayon;
            txbDvdTitre.Text = dvd.Titre;
            string image = dvd.Image;

            txbDvdRealisateurModif.Text = dvd.Realisateur;
            txbDvdSynopsisModif.Text = dvd.Synopsis;
            txbDvdImageModif.Text = dvd.Image;
            txbDvdDureeModif.Text = dvd.Duree.ToString();
            txbDvdNumeroModif.Text = dvd.Id;
            txbDvdTitreModif.Text = dvd.Titre;

            cbxDvdPublicsModif.Text = "";
            cbxDvdRayonsModif.Text = "";
            cbxDvdGenresModif.Text = "";
            cbxDvdPublicsModif.SelectedText = dvd.Public;
            cbxDvdRayonsModif.SelectedText = dvd.Rayon;
            cbxDvdGenresModif.SelectedText = dvd.Genre;

            try
            {
                pcbDvdImage.Image = Image.FromFile(image);
            }
            catch 
            {
                pcbDvdImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations du dvd
        /// </summary>
        private void VideDvdInfos()
        {
            txbDvdRealisateur.Text = "";
            txbDvdSynopsis.Text = "";
            txbDvdImage.Text = "";
            txbDvdDuree.Text = "";
            txbDvdNumero.Text = "";
            txbDvdGenre.Text = "";
            txbDvdPublic.Text = "";
            txbDvdRayon.Text = "";
            txbDvdTitre.Text = "";
            pcbDvdImage.Image = null;
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdGenres.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Genre genre = (Genre)cbxDvdGenres.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdPublics.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Public lePublic = (Public)cbxDvdPublics.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdRayons.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxDvdRayons.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations du dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDvdListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDvdListe.CurrentCell != null)
            {
                try
                {
                    Dvd dvd = (Dvd)bdgDvdListe.List[bdgDvdListe.Position];
                    AfficheDvdInfos(dvd);
                }
                catch
                {
                    VideDvdZones();
                }
            }
            else
            {
                VideDvdInfos();
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des Dvd
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirDvdListeComplete()
        {
            RemplirDvdListe(lesDvd);
            VideDvdZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideDvdZones()
        {
            cbxDvdGenres.SelectedIndex = -1;
            cbxDvdRayons.SelectedIndex = -1;
            cbxDvdPublics.SelectedIndex = -1;
            txbDvdNumRecherche.Text = "";
            txbDvdTitreRecherche.Text = "";
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDvdListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideDvdZones();
            string titreColonne = dgvDvdListe.Columns[e.ColumnIndex].HeaderText;
            List<Dvd> sortedList = new List<Dvd>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesDvd.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesDvd.OrderBy(o => o.Titre).ToList();
                    break;
                case "Duree":
                    sortedList = lesDvd.OrderBy(o => o.Duree).ToList();
                    break;
                case "Realisateur":
                    sortedList = lesDvd.OrderBy(o => o.Realisateur).ToList();
                    break;
                case "Genre":
                    sortedList = lesDvd.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesDvd.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesDvd.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirDvdListe(sortedList);
        }

        private void BtnDvdAjout_Click(object sender, EventArgs e)
        {
            string rayonId = getRayonId(cbxDvdRayonsModif.Text);
            string genreId = getGenresId(cbxDvdGenresModif.Text);
            string publicId = getPublicId(cbxDvdPublicsModif.Text);
            Dvd dvd = new Dvd(
                txbDvdNumeroModif.Text,
                txbDvdTitreModif.Text,
                txbDvdImageModif.Text,
                int.Parse(txbDvdDureeModif.Text),
                txbDvdRealisateurModif.Text,
                txbDvdSynopsisModif.Text,
                genreId,
                cbxDvdGenresModif.Text,
                publicId,
                cbxDvdPublicsModif.Text,
                rayonId,
                cbxDvdRayonsModif.Text
                );
            if (controle.CreerDvd(dvd))
            {
                MessageBox.Show("Dvd ajouté avec success");
            }
            else
            {
                MessageBox.Show("Un Dvd porte déjà ce numéro");
            }
            lesDvd = controle.GetAllDvd();
            RemplirDvdListeComplete();
        }

        private void BtnDvdSuppr_Click(object sender, EventArgs e)
        {
            if ((MessageBox.Show("Êtes vous sur de vouloir supprimé ce Dvd ?", "Suppression d'un Dvd",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes))
            {

                if (controle.SupprimerBdd(txbDvdNumero.Text, "dvd"))
                {
                    MessageBox.Show("Dvd supprimé avec success");
                }
                else
                {
                    MessageBox.Show("Echec de la supression, ce dvd doit posséder une commande.");
                }
            }
            lesDvd = controle.GetAllDvd();
            RemplirDvdListeComplete();
        }

        private void BtnDvdModif_Click(object sender, EventArgs e)
        {
            string rayonId = getRayonId(cbxDvdRayonsModif.Text);
            string genreId = getGenresId(cbxDvdGenresModif.Text);
            string publicId = getPublicId(cbxDvdPublicsModif.Text);
            Dvd dvd = new Dvd(
                txbDvdNumeroModif.Text,
                txbDvdTitreModif.Text,
                txbDvdImageModif.Text,
                int.Parse(txbDvdDureeModif.Text),
                txbDvdRealisateurModif.Text,
                txbDvdSynopsisModif.Text,
                genreId,
                cbxDvdGenresModif.Text,
                publicId,
                cbxDvdPublicsModif.Text,
                rayonId,
                cbxDvdRayonsModif.Text
                );
            if ((MessageBox.Show("Êtes vous sur de vouloir Modifier ce Dvd ?\nSi Le numéro de document a été modifier, il ne sera pas pris en compte, il conservera celui d'origine.", "Modification d'un Dvd", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes))
            {
                if (controle.UpdateDvd(dvd))
                {
                    MessageBox.Show("Dvd Modifier avec success");
                }
                else
                {
                    MessageBox.Show("Erreur lors de la modification (Max 20 caractère pour le réalisateur)");
                }
            }
            lesDvd = controle.GetAllDvd();
            RemplirDvdListeComplete();
        }

        #endregion


        #region Réception Exemplaire de presse
        //-----------------------------------------------------------
        // ONGLET "RECEPTION DE REVUES"
        //-----------------------------------------------------------

        /// <summary>
        /// Ouverture de l'onglet : blocage en saisie des champs de saisie des infos de l'exemplaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabReceptionRevue_Enter(object sender, EventArgs e)
        {
            lesRevues = controle.GetAllRevues();
            accesReceptionExemplaireGroupBox(false);
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        private void RemplirReceptionExemplairesListe(List<Exemplaire> exemplaires)
        {
            bdgExemplairesListe.DataSource = exemplaires;
            dgvReceptionExemplairesListe.DataSource = bdgExemplairesListe;
            dgvReceptionExemplairesListe.Columns["idEtat"].Visible = false;
            dgvReceptionExemplairesListe.Columns["idDocument"].Visible = false;
            dgvReceptionExemplairesListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvReceptionExemplairesListe.Columns["numero"].DisplayIndex = 0;
            dgvReceptionExemplairesListe.Columns["dateAchat"].DisplayIndex = 1;
        }

        /// <summary>
        /// Recherche d'un numéro de revue et affiche ses informations
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionRechercher_Click(object sender, EventArgs e)
        {
            if (!txbReceptionRevueNumero.Text.Equals(""))
            {
                Revue revue = lesRevues.Find(x => x.Id.Equals(txbReceptionRevueNumero.Text));
                if (revue != null)
                {
                    AfficheReceptionRevueInfos(revue);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    VideReceptionRevueInfos();
                }
            }
            else
            {
                VideReceptionRevueInfos();
            }
        }

        /// <summary>
        /// Si le numéro de revue est modifié, la zone de l'exemplaire est vidée et inactive
        /// les informations de la revue son aussi effacées
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbReceptionRevueNumero_TextChanged(object sender, EventArgs e)
        {
            accesReceptionExemplaireGroupBox(false);
            VideReceptionRevueInfos();
        }

        /// <summary>
        /// Affichage des informations de la revue sélectionnée et les exemplaires
        /// </summary>
        /// <param name="revue"></param>
        private void AfficheReceptionRevueInfos(Revue revue)
        {
            // informations sur la revue
            txbReceptionRevuePeriodicite.Text = revue.Periodicite;
            chkReceptionRevueEmpruntable.Checked = revue.Empruntable;
            txbReceptionRevueImage.Text = revue.Image;
            txbReceptionRevueDelaiMiseADispo.Text = revue.DelaiMiseADispo.ToString();
            txbReceptionRevueNumero.Text = revue.Id;
            txbReceptionRevueGenre.Text = revue.Genre;
            txbReceptionRevuePublic.Text = revue.Public;
            txbReceptionRevueRayon.Text = revue.Rayon;
            txbReceptionRevueTitre.Text = revue.Titre;         
            string image = revue.Image;
            try
            {
                pcbReceptionRevueImage.Image = Image.FromFile(image);
            }
            catch 
            {
                pcbReceptionRevueImage.Image = null;
            }
            // affiche la liste des exemplaires de la revue
            afficheReceptionExemplairesRevue();
            // accès à la zone d'ajout d'un exemplaire
            accesReceptionExemplaireGroupBox(true);
        }

        private void afficheReceptionExemplairesRevue()
        {
            string idDocuement = txbReceptionRevueNumero.Text;
            lesExemplaires = controle.GetExemplairesRevue(idDocuement);
            RemplirReceptionExemplairesListe(lesExemplaires);
        }

        /// <summary>
        /// Vide les zones d'affchage des informations de la revue
        /// </summary>
        private void VideReceptionRevueInfos()
        {
            txbReceptionRevuePeriodicite.Text = "";
            chkReceptionRevueEmpruntable.Checked = false;
            txbReceptionRevueImage.Text = "";
            txbReceptionRevueDelaiMiseADispo.Text = "";
            txbReceptionRevueGenre.Text = "";
            txbReceptionRevuePublic.Text = "";
            txbReceptionRevueRayon.Text = "";
            txbReceptionRevueTitre.Text = "";
            pcbReceptionRevueImage.Image = null;
            lesExemplaires = new List<Exemplaire>();
            RemplirReceptionExemplairesListe(lesExemplaires);
            accesReceptionExemplaireGroupBox(false);
        }

        /// <summary>
        /// Vide les zones d'affichage des informations de l'exemplaire
        /// </summary>
        private void VideReceptionExemplaireInfos()
        {
            txbReceptionExemplaireImage.Text = "";
            txbReceptionExemplaireNumero.Text = "";
            pcbReceptionExemplaireImage.Image = null;
            dtpReceptionExemplaireDate.Value = DateTime.Now;
        }

        /// <summary>
        /// Permet ou interdit l'accès à la gestion de la réception d'un exemplaire
        /// et vide les objets graphiques
        /// </summary>
        /// <param name="acces"></param>
        private void accesReceptionExemplaireGroupBox(bool acces)
        {
            VideReceptionExemplaireInfos();
            grpReceptionExemplaire.Enabled = acces;
        }

        /// <summary>
        /// Recherche image sur disque (pour l'exemplaire)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionExemplaireImage_Click(object sender, EventArgs e)
        {
            string filePath = "";
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "Files|*.jpg;*.bmp;*.jpeg;*.png;*.gif";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog.FileName;
            }
            txbReceptionExemplaireImage.Text = filePath;         
            try
            {
                pcbReceptionExemplaireImage.Image = Image.FromFile(filePath);
            }
            catch 
            {
                pcbReceptionExemplaireImage.Image = null;
            }
        }

        /// <summary>
        /// Enregistrement du nouvel exemplaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionExemplaireValider_Click(object sender, EventArgs e)
        {
            if (!txbReceptionExemplaireNumero.Text.Equals(""))
            {
                try
                {
                    int numero = int.Parse(txbReceptionExemplaireNumero.Text);
                    DateTime dateAchat = dtpReceptionExemplaireDate.Value;
                    string photo = txbReceptionExemplaireImage.Text;
                    string idEtat = ETATNEUF;
                    string idDocument = txbReceptionRevueNumero.Text;
                    Exemplaire exemplaire = new Exemplaire(numero, dateAchat, photo, idEtat, idDocument);
                    if (controle.CreerExemplaire(exemplaire))
                    {
                        VideReceptionExemplaireInfos();
                        afficheReceptionExemplairesRevue();
                    }
                    else
                    {
                        MessageBox.Show("numéro de publication déjà existant", "Erreur");
                    }
                }catch
                {
                    MessageBox.Show("le numéro de parution doit être numérique", "Information");
                    txbReceptionExemplaireNumero.Text = "";
                    txbReceptionExemplaireNumero.Focus();
                }
            }
            else
            {
                MessageBox.Show("numéro de parution obligatoire", "Information");
            }
        }

        /// <summary>
        /// Tri sur une colonne
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvExemplairesListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dgvReceptionExemplairesListe.Columns[e.ColumnIndex].HeaderText;
            List<Exemplaire> sortedList = new List<Exemplaire>();
            switch (titreColonne)
            {
                case "Numero":
                    sortedList = lesExemplaires.OrderBy(o => o.Numero).Reverse().ToList();
                    break;
                case "DateAchat":
                    sortedList = lesExemplaires.OrderBy(o => o.DateAchat).Reverse().ToList();
                    break;
                case "Photo":
                    sortedList = lesExemplaires.OrderBy(o => o.Photo).ToList();
                    break;
            }
            RemplirReceptionExemplairesListe(sortedList);
        }

        /// <summary>
        /// Sélection d'une ligne complète et affichage de l'image sz l'exemplaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvReceptionExemplairesListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvReceptionExemplairesListe.CurrentCell != null)
            {
                Exemplaire exemplaire = (Exemplaire)bdgExemplairesListe.List[bdgExemplairesListe.Position];
                string image = exemplaire.Photo;
                try
                {
                    pcbReceptionExemplaireRevueImage.Image = Image.FromFile(image);
                }
                catch
                {
                    pcbReceptionExemplaireRevueImage.Image = null;
                }
            }
            else
            {
                pcbReceptionExemplaireRevueImage.Image = null;
            }
        }


        #endregion


        #region utils
        /// <summary>
        /// Recherche le label du Rayon par son Id
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        private string getRayonId(string sender)
        {
            List<Categorie> rayons = controle.GetAllRayons();
            foreach (Categorie item in rayons)
            {
                if (item.Libelle == sender)
                {
                    return item.Id;
                }
            }
            return null;
        }

        /// <summary>
        /// Recherche le label du Public par son Id
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        private string getPublicId(string sender)
        {
            List<Categorie> rayons = controle.GetAllPublics();
            foreach (Categorie item in rayons)
            {
                if (item.Libelle == sender)
                {
                    return item.Id;
                }
            }
            return null;
        }

        /// <summary>
        /// Recherche le label du Genre par son Id
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        private string getGenresId(string sender)
        {
            List<Categorie> rayons = controle.GetAllGenres();
            foreach (Categorie item in rayons)
            {
                if (item.Libelle == sender)
                {
                    return item.Id;
                }
            }
            return null;
        }

        /// <summary>
        /// Génère l'Id d'une commande aléatoirement
        /// </summary>
        /// <returns></returns>
        private int getRandomOrderId()
        {
            Random rnd = new Random();
            int num = rnd.Next(10000, 20000);
            return num;
        }


        #endregion


        #region commande livre

        //-----------------------------------------------------------
        // ONGLET "COMMANDE LIVRE"
        //-----------------------------------------------------------

        /// <summary>
        /// Ouverture de l'onglet Commandes de livres : 
        /// appel des méthodes pour remplir le datagrid des livres et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabCommandeLivres_Enter_1(object sender, EventArgs e)
        {
            lesLivres = controle.GetAllLivres();
            lesCommandesLivres = controle.GetAllCommandeLivresDvd("livre");
            RemplirCommandesLivresListeComplete();
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        private void RemplirCommanbandesLivresListe(List<Commande> lesCommandesLivres)
        {
            bdgCommandeLivresListe.DataSource = lesCommandesLivres;
            dgvCommandesLivresListe.DataSource = bdgCommandeLivresListe;
            dgvCommandesLivresListe.Columns["IdSuivi"].Visible = false;
            dgvCommandesLivresListe.Columns["Id"].HeaderText = "Numéro de commande";
            dgvCommandesLivresListe.Columns["IdLivreDvd"].HeaderText = "Numéro du document";
            dgvCommandesLivresListe.Columns["Montant"].HeaderText = "Montant(€)";
            dgvCommandesLivresListe.Columns["nbExemplaire"].HeaderText = "Nombre d'exemplaire(s)";
            dgvCommandesLivresListe.Columns["DateCommande"].HeaderText = "Date de la commande";
            dgvCommandesLivresListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvCommandesLivresListe.Columns["Id"].DisplayIndex = 0;
            dgvCommandesLivresListe.Columns["Etat"].DisplayIndex = 7;
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        private void RemplirCommanbandesLivresListeBdd(List<Livre> livres)
        {
            bdgCommandesLivresListeBdd.DataSource = livres;
            dgvCommandesLivresListeBdd.DataSource = bdgCommandesLivresListeBdd;
            dgvCommandesLivresListeBdd.Columns["isbn"].Visible = false;
            dgvCommandesLivresListeBdd.Columns["idRayon"].Visible = false;
            dgvCommandesLivresListeBdd.Columns["idGenre"].Visible = false;
            dgvCommandesLivresListeBdd.Columns["idPublic"].Visible = false;
            dgvCommandesLivresListeBdd.Columns["image"].Visible = false;
            dgvCommandesLivresListeBdd.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvCommandesLivresListeBdd.Columns["id"].DisplayIndex = 0;
            dgvCommandesLivresListeBdd.Columns["titre"].DisplayIndex = 1;
        }


        /// <summary>
        /// Affichage de la liste complète des commandes des livres
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirCommandesLivresListeComplete()
        {
            RemplirCommanbandesLivresListe(lesCommandesLivres);
            RemplirCommanbandesLivresListeBdd(lesLivres);
            VideLivresZones();
        }

        private void dgvCommandesLivresListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCommandesLivresListe.CurrentCell != null)
            {
                try
                {
                    Commande commande = (Commande)bdgCommandeLivresListe.List[bdgCommandeLivresListe.Position];
                    AfficheCommandeLivresInfos(commande);
                }
                catch
                {
                    VideCommandeLivresInfos();
                }
            }
            else
            {
                VideCommandeLivresInfos();
            }
        }

        /// <summary>
        /// Affichage de la liste complète des commandes des livres
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void AfficheCommandeLivresInfos(Commande commande)
        {
            Livre livre = controle.selectLivreById(commande.IdLivreDvd);
            txbLivresNumeroDocCommande.Text = commande.IdLivreDvd;
            txbLivresNumeroCommande.Text = commande.Id.ToString();
            txbLivresTitreCommande.Text = livre.Titre;
            txbLivresAuteurCommande.Text = livre.Auteur;
            txbLivresCollectionCommande.Text = livre.Collection;
            txbLivresGenreCommande.Text = livre.Genre;
            txbLivresPublicCommande.Text = livre.Public;
            txbLivresRayonCommande.Text = livre.Rayon;
            txbLivresIsbnCommande.Text = livre.Isbn;
            txbLivresDateCommande.Text = commande.DateCommande.ToString("D");
            txbLivresRelanceCommande.Text = commande.DateCommande.AddDays(3).ToString("D");
            txbLivresNombreCommande.Text = commande.NbExemplaire.ToString();
            txbLivresEtatCommande.Text = commande.Etat;

            switch (commande.IdSuivi)
            {
                case (1):
                    btnLivresCommandeSetEnCours.Enabled = false;
                    btnLivresCommandeSetLivree.Enabled = true;
                    btnLivresCommandeSetReglee.Enabled = false;
                    btnLivresCommandeSetRelancee.Enabled = true;
                    btnLivresCommandeSetDelete.Enabled = true;
                    break;
                case (2):
                case (3):
                    btnLivresCommandeSetEnCours.Enabled = false;
                    btnLivresCommandeSetLivree.Enabled = true;
                    btnLivresCommandeSetReglee.Enabled = true;
                    btnLivresCommandeSetRelancee.Enabled = false;
                    btnLivresCommandeSetDelete.Enabled = false;
                    break;
                case (4):
                    btnLivresCommandeSetEnCours.Enabled = true;
                    btnLivresCommandeSetLivree.Enabled = false;
                    btnLivresCommandeSetReglee.Enabled = false;
                    btnLivresCommandeSetRelancee.Enabled = true;
                    btnLivresCommandeSetDelete.Enabled = true;
                    break;
            }
            string image = livre.Image;
            try{ pcbLivresImageCommande.Image = Image.FromFile(image);}
            catch{pcbLivresImageCommande.Image = null;}
        }


        /// <summary>
        /// Affichage de la liste complète des commandes des livres
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void AfficheCommandeLivresBddInfos(Livre livre)
        {
            txbLivresNumeroCommandeCreate.Text = livre.Id;
            txbLivresTitreCommandeCreate.Text = livre.Titre;
            txbLivresAuteurCommandeCreate.Text = livre.Auteur;
            txbLivresCollectionCommandeCreate.Text = livre.Collection;
            txbCommandeLivresGenres.Text = livre.Genre;
            txbCommandeLivresPublics.Text = livre.Public;
            txbCommandeLivresRayons.Text = livre.Rayon;
            txbLivresIsbnCommandeCreate.Text = livre.Isbn;

        }


        /// <summary>
        /// Affichage de la liste complète des commandes des livres
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void VideCommandeLivresInfos()
        {
            txbLivresNumeroDocCommande.Text = "";
            txbLivresTitreCommande.Text = "";
            txbLivresAuteurCommande.Text = "";
            txbLivresCollectionCommande.Text = "";
            txbLivresGenreCommande.Text = "";
            txbLivresPublicCommande.Text = "";
            txbLivresRayonCommande.Text = "";
            txbLivresIsbnCommande.Text = "";

            txbLivresDateCommande.Text = "";
            txbLivresNombreCommande.Text = "";
            txbLivresEtatCommande.Text = "";
        }

        private void dgvCommandesLivresListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideLivresZones();
            string titreColonne = dgvCommandesLivresListe.Columns[e.ColumnIndex].HeaderText;
            List<Commande> sortedList = new List<Commande>();
            switch (titreColonne)
            {
                case "Numéro de commande":
                    sortedList = lesCommandesLivres.OrderBy(o => o.Id).ToList();
                    break;
                case "Numéro du document":
                    sortedList = lesCommandesLivres.OrderBy(o => o.IdLivreDvd).ToList();
                    break;
                case "Nombre d'exemplaire(s)":
                    sortedList = lesCommandesLivres.OrderBy(o => o.NbExemplaire).ToList();
                    break;
                case "Date de la commande":
                    sortedList = lesCommandesLivres.OrderBy(o => o.DateCommande).ToList();
                    break;
                case "Montant(€)":
                    sortedList = lesCommandesLivres.OrderBy(o => o.Montant).ToList();
                    break;
                case "Titre":
                    sortedList = lesCommandesLivres.OrderBy(o => o.Titre).ToList();
                    break;
                case "Etat":
                    sortedList = lesCommandesLivres.OrderBy(o => o.IdSuivi).ToList();
                    break;
            }
            RemplirCommanbandesLivresListe(sortedList);
        }

        private void btnLivresNumCommandeRecherche_Click(object sender, EventArgs e)
        {
            if (!txbLivresNumCommandeRecherche.Text.Equals(""))
            {
                Commande commande = lesCommandesLivres.Find(x => x.IdLivreDvd.Equals(txbLivresNumCommandeRecherche.Text));
                if (commande != null)
                {
                    List<Commande> commandes = new List<Commande>();
                    commandes.Add(commande);
                    RemplirCommanbandesLivresListe(commandes);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirLivresListeComplete();
                }
            }
            else
            {
                RemplirLivresListeComplete();
            }
            lesCommandesLivres = controle.GetAllCommandeLivresDvd("livre");
            RemplirCommandesLivresListeComplete();
        }

        private void btnLivresCommandeSetEnCours_Click(object sender, EventArgs e)
        {
            Commande commande = lesCommandesLivres.Find(x => x.Id.Equals(int.Parse(txbLivresNumeroCommande.Text)));
            if (commande != null)
            {
                if (commande.Etat != "en cours")
                {
                    List<Commande> commandes = new List<Commande>();
                    commandes.Add(commande);
                    if (controle.ModifyEtatCommande(commande, "1"))
                    {
                        MessageBox.Show("Etat modifier avec succès");
                    }
                    else
                    {
                        MessageBox.Show("Erreur lors de la modification");
                    }
                }
                else
                {
                    MessageBox.Show("Erreur cette commmande est déjà en cours");
                }
            }
            else
            {
                MessageBox.Show("Erreur lors de la modification");
            }
            lesCommandesLivres = controle.GetAllCommandeLivresDvd("livre");
            RemplirCommandesLivresListeComplete();
        }

        private void btnLivresCommandeSetLivree_Click(object sender, EventArgs e)
        {
            Commande commande = lesCommandesLivres.Find(x => x.Id.Equals(int.Parse(txbLivresNumeroCommande.Text)));
            if (commande != null)
            {
                if (commande.Etat != "livrée")
                {
                    List<Commande> commandes = new List<Commande>();
                    commandes.Add(commande);
                    if (controle.ModifyEtatCommande(commande, "2"))
                    {
                        MessageBox.Show("Etat modifier avec succès");
                    }
                    else
                    {
                        MessageBox.Show("Erreur lors de la modification");
                    }
                }
                else
                {
                    MessageBox.Show("Erreur cette commmande est déjà livrée");
                }
            }
            else
            {
                MessageBox.Show("Erreur lors de la modification");
            }
            lesCommandesLivres = controle.GetAllCommandeLivresDvd("livre");
            RemplirCommandesLivresListeComplete();
        }

        private void btnLivresCommandeSetReglee_Click(object sender, EventArgs e)
        {
            Commande commande = lesCommandesLivres.Find(x => x.Id.Equals(int.Parse(txbLivresNumeroCommande.Text)));
            if (commande != null)
            {
                if (commande.Etat != "réglée")
                {
                    List<Commande> commandes = new List<Commande>();
                    commandes.Add(commande);
                    if (controle.ModifyEtatCommande(commande, "3"))
                    {
                        MessageBox.Show("Etat modifier avec succès");
                    }
                    else
                    {
                        MessageBox.Show("Erreur lors de la modification");
                    }
                }
                else
                {
                    MessageBox.Show("Erreur cette commmande est déjà réglée");
                }
            }
            else
            {
                MessageBox.Show("Erreur lors de la modification");
            }
            lesCommandesLivres = controle.GetAllCommandeLivresDvd("livre");
            RemplirCommandesLivresListeComplete();
        }

        private void btnLivresCommandeSetRelancee_Click(object sender, EventArgs e)
        {
            Commande commande = lesCommandesLivres.Find(x => x.Id.Equals(txbLivresNumeroDocCommande.Text));
            if (commande != null)
            {
                if (commande.Etat != "relancée")
                {
                    List<Commande> commandes = new List<Commande>();
                    commandes.Add(commande);
                    if (controle.ModifyEtatCommande(commande, "4"))
                    {
                        MessageBox.Show("Etat modifier avec succès");
                    }
                    else
                    {
                        MessageBox.Show("Erreur lors de la modification");
                    }
                }
                else
                {
                    MessageBox.Show("Erreur cette commmande est déjà relancée");
                }
            }
            else
            {
                MessageBox.Show("Erreur lors de la modification");
            }
            lesCommandesLivres = controle.GetAllCommandeLivresDvd("livre");
            RemplirCommandesLivresListeComplete();
        }

        private void btnLivresCommandeCommander_Click(object sender, EventArgs e)
        {
            if (nudLivresPrixCommandeCreate.Value == 0 && nudLivresNombreCommandeCreate.Value == 0)
            {
                MessageBox.Show("Veuillez saisir le nombre d'exemplaire a commander ainsi que le prix.");
            }
            else
            {
                string rayonId = getRayonId(txbCommandeLivresRayons.Text);
                string genreId = getGenresId(txbCommandeLivresGenres.Text);
                string publicId = getPublicId(txbCommandeLivresPublics.Text);

                Commande commande = new Commande(
                    getRandomOrderId(),
                    DateTime.Now,
                    (double)nudLivresPrixCommandeCreate.Value * (double)nudLivresNombreCommandeCreate.Value,
                    (int)nudLivresNombreCommandeCreate.Value,
                    txbLivresNumeroCommandeCreate.Text,
                    1,
                    "en cours",
                    txbLivresTitreCommandeCreate.Text
                    );

                Livre livre = new Livre(
                    txbLivresNumeroCommandeCreate.Text,
                    txbLivresTitreCommandeCreate.Text,
                    txbLivresImageCommandeCreate.Text,
                    txbLivresIsbnCommandeCreate.Text,
                    txbLivresAuteurCommandeCreate.Text,
                    txbLivresCollectionCommandeCreate.Text,
                    genreId,
                    txbCommandeLivresGenres.Text,
                    publicId,
                    txbCommandeLivresPublics.Text,
                    rayonId,
                    txbCommandeLivresRayons.Text
                    );

                if (controle.CreerCommande(commande, livre))
                {
                    MessageBox.Show("Commande créé avec success");
                }
                else
                {
                    MessageBox.Show("Une erreur est survenu lors de la création de la commande.");
                }
            }


            lesLivres = controle.GetAllLivres();
            lesCommandesLivres = controle.GetAllCommandeLivresDvd("livre");
            RemplirCommandesLivresListeComplete();
        }

        private void btnLivresCommandeSetDelete_Click(object sender, EventArgs e)
        {
            Commande commande = lesCommandesLivres.Find(x => x.IdLivreDvd.Equals(txbLivresNumeroDocCommande.Text));
            if (commande != null)
            {
                if (commande.Etat != "livrée")
                {
                    if ((MessageBox.Show("Êtes vous sur de vouloir supprimé cette commande ?", "Suppression d'une commande"
                        , MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) 
                        == System.Windows.Forms.DialogResult.Yes))
                    {
                        List<Commande> commandes = new List<Commande>();
                        commandes.Add(commande);
                        if (controle.DeleteCommande(commande))
                        {
                            MessageBox.Show("Commande supprimée avec succès");
                        }
                        else
                        {
                            MessageBox.Show("Erreur lors de la suppression de la commande");
                        }
                    }
                    
                }
                else
                {
                    MessageBox.Show("Erreur Impossible de supprimer une commande livrée");
                }
            }
            else
            {
                MessageBox.Show("Erreur lors de la suppression");
            }
            lesCommandesLivres = controle.GetAllCommandeLivresDvd("livre");
            RemplirCommandesLivresListeComplete();
        }

        private void dgvCommandesLivresListeBdd_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCommandesLivresListeBdd.CurrentCell != null)
            {
                try
                {
                    Livre livre = (Livre)bdgCommandesLivresListeBdd.List[bdgCommandesLivresListeBdd.Position];
                    AfficheCommandeLivresBddInfos(livre);
                }
                catch
                {
                    VideCommandeLivresInfos();
                }
            }
            else
            {
                VideCommandeLivresInfos();
            }
        }

        #endregion


        #region commande dvd


        private void tabOngletsApplication_Enter(object sender, EventArgs e)
        {
            lesDvd = controle.GetAllDvd();
            lesCommandesDvds = controle.GetAllCommandeLivresDvd("dvd");
            RemplirCommandesDvdsListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des commandes des livres
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirCommandesDvdsListeComplete()
        {
            RemplirCommanbandesDvdsListe(lesCommandesDvds);
            RemplirCommanbandesDvdsListeBdd(lesDvd);
            VideLivresZones();
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        private void RemplirCommanbandesDvdsListe(List<Commande> lesCommandesDvds)
        {
            bdgCommandeDvdsListe.DataSource = lesCommandesDvds;
            dgvCommandesDvdsListe.DataSource = bdgCommandeDvdsListe;
            dgvCommandesDvdsListe.Columns["IdSuivi"].Visible = false;
            dgvCommandesDvdsListe.Columns["Id"].HeaderText = "Numéro de commande";
            dgvCommandesDvdsListe.Columns["IdLivreDvd"].HeaderText = "Numéro du document";
            dgvCommandesDvdsListe.Columns["Montant"].HeaderText = "Montant(€)";
            dgvCommandesDvdsListe.Columns["nbExemplaire"].HeaderText = "Nombre d'exemplaire(s)";
            dgvCommandesDvdsListe.Columns["DateCommande"].HeaderText = "Date de la commande";
            dgvCommandesDvdsListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvCommandesDvdsListe.Columns["Id"].DisplayIndex = 0;
            dgvCommandesDvdsListe.Columns["Etat"].DisplayIndex = 7;
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        private void RemplirCommanbandesDvdsListeBdd(List<Dvd> dvd)
        {
            bdgCommandeDvdsListeBdd.DataSource = dvd;
            dgvCommandesDvdsListeBdd.DataSource = bdgCommandeDvdsListeBdd;
            dgvCommandesDvdsListeBdd.Columns["idRayon"].Visible = false;
            dgvCommandesDvdsListeBdd.Columns["idGenre"].Visible = false;
            dgvCommandesDvdsListeBdd.Columns["idPublic"].Visible = false;
            dgvCommandesDvdsListeBdd.Columns["image"].Visible = false;
            dgvCommandesDvdsListeBdd.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvCommandesDvdsListeBdd.Columns["id"].DisplayIndex = 0;
            dgvCommandesDvdsListeBdd.Columns["titre"].DisplayIndex = 1;
        }

        private void dgvCommandesDvdsListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dgvCommandesDvdsListe.Columns[e.ColumnIndex].HeaderText;
            List<Commande> sortedList = new List<Commande>();
            switch (titreColonne)
            {
                case "Numéro de commande":
                    sortedList = lesCommandesDvds.OrderBy(o => o.Id).ToList();
                    break;
                case "Numéro du document":
                    sortedList = lesCommandesDvds.OrderBy(o => o.Titre).ToList();
                    break;
                case "Nombre d'exemplaire(s)":
                    sortedList = lesCommandesDvds.OrderBy(o => o.NbExemplaire).ToList();
                    break;
                case "Date de la commande":
                    sortedList = lesCommandesDvds.OrderBy(o => o.DateCommande).ToList();
                    break;
                case "Montant(€)":
                    sortedList = lesCommandesDvds.OrderBy(o => o.Montant).ToList();
                    break;
                case "Titre":
                    sortedList = lesCommandesDvds.OrderBy(o => o.Titre).ToList();
                    break;
                case "Etat":
                    sortedList = lesCommandesDvds.OrderBy(o => o.IdSuivi).ToList();
                    break;
            }
            RemplirCommanbandesDvdsListe(sortedList);
        }

        private void btnDvdsNumCommandeRecherche_Click(object sender, EventArgs e)
        {
            if (!txbDvdsNumCommandeRecherche.Text.Equals(""))
            {
                Commande commande = lesCommandesDvds.Find(x => x.IdLivreDvd.Equals(txbDvdsNumCommandeRecherche.Text));
                if (commande != null)
                {
                    List<Commande> commandes = new List<Commande>();
                    commandes.Add(commande);
                    RemplirCommanbandesLivresListe(commandes);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirLivresListeComplete();
                }
            }
            else
            {
                RemplirLivresListeComplete();
            }
            lesCommandesLivres = controle.GetAllCommandeLivresDvd("dvd");
            RemplirCommandesLivresListeComplete();
        }

        private void dgvCommandesDvdsListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCommandesDvdsListe.CurrentCell != null)
            {
                try
                {
                    Commande commande = (Commande)bdgCommandeDvdsListe.List[bdgCommandeDvdsListe.Position];
                    AfficheCommandeDvdsInfos(commande);
                }
                catch
                {
                    VideCommandeLivresInfos();
                }
            }
            else
            {
                VideCommandeLivresInfos();
            }
        }

        /// <summary>
        /// Affichage de la liste complète des commandes des livres
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void AfficheCommandeDvdsInfos(Commande commande)
        {
            Dvd dvd = controle.selectDvdById(commande.IdLivreDvd);
            txbDvdsNumeroCommande.Text = commande.Id.ToString();
            txbDvdsNumeroDocCommande.Text = dvd.Id;
            txbDvdsTitreCommande.Text = dvd.Titre;
            txbDvdsRealisateurCommande.Text = dvd.Realisateur;
            txbDvdsSynopsisCommande.Text = dvd.Synopsis;
            txbDvdsGenreCommande.Text = dvd.Genre;
            txbDvdsPublicCommande.Text = dvd.Public;
            txbDvdsRayonCommande.Text = dvd.Rayon;
            txbDvdsDureeCommande.Text = dvd.Duree.ToString();

            txbDvdsDateCommande.Text = commande.DateCommande.ToString("D");
            txbDvdsRelanceCommande.Text = commande.DateCommande.AddDays(3).ToString("D");
            txbDvdsNombreCommande.Text = commande.NbExemplaire.ToString();
            txbDvdsEtatCommande.Text = commande.Etat;

            switch (commande.IdSuivi)
            {
                case (1):
                    btnDvdsCommandeSetEnCours.Enabled = false;
                    btnDvdsCommandeSetLivree.Enabled = true;
                    btnDvdsCommandeSetReglee.Enabled = false;
                    btnDvdsCommandeSetRelancee.Enabled = true;
                    btnDvdsCommandeSetDelete.Enabled = true;
                    break;
                case (2):
                case (3):
                    btnDvdsCommandeSetEnCours.Enabled = false;
                    btnDvdsCommandeSetLivree.Enabled = true;
                    btnDvdsCommandeSetReglee.Enabled = true;
                    btnDvdsCommandeSetRelancee.Enabled = false;
                    btnDvdsCommandeSetDelete.Enabled = false;
                    break;
                case (4):
                    btnDvdsCommandeSetEnCours.Enabled = true;
                    btnDvdsCommandeSetLivree.Enabled = false;
                    btnDvdsCommandeSetReglee.Enabled = false;
                    btnDvdsCommandeSetRelancee.Enabled = true;
                    btnDvdsCommandeSetDelete.Enabled = true;
                    break;
            }

            string image = dvd.Image;
            try
            {
                pcbLivresImageCommande.Image = Image.FromFile(image);
            }
            catch
            {
                pcbLivresImageCommande.Image = null;
            }

        }

        private void dgvCommandesDvdsListeBdd_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCommandesDvdsListeBdd.CurrentCell != null)
            {
                try
                {
                    Dvd dvd = (Dvd)bdgCommandeDvdsListeBdd.List[bdgCommandeDvdsListeBdd.Position];
                    AfficheCommandeDvdsBddInfos(dvd);
                }
                catch
                {
                    VideCommandeLivresInfos();
                }
            }
            else
            {
                VideCommandeLivresInfos();
            }
        }

        /// <summary>
        /// Affichage de la liste complète des commandes des livres
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void AfficheCommandeDvdsBddInfos(Dvd dvd)
        {
            txbDvdsNumeroCommandeCreate.Text = dvd.Id;
            txbDvdsTitreCommandeCreate.Text = dvd.Titre;
            txbDvdsRealisateurCommande.Text = dvd.Realisateur;
            txbDvdsSynopsisCommandeCreate.Text = dvd.Synopsis;
            txbCommandeDvdsGenres.Text = dvd.Genre;
            txbCommandeDvdsPublics.Text = dvd.Public;
            txbCommandeDvdsRayons.Text = dvd.Rayon;
            txbDvdsDureeCommandeCreate.Text = dvd.Duree.ToString();
        }

        private void btnDvdsCommandeSetEnCours_Click(object sender, EventArgs e)
        {
            Commande commande = lesCommandesDvds.Find(x => x.Id.Equals(int.Parse(txbDvdsNumeroCommande.Text)));
            if (commande != null)
            {
                if (commande.Etat != "en cours")
                {
                    List<Commande> commandes = new List<Commande>();
                    commandes.Add(commande);
                    if (controle.ModifyEtatCommande(commande, "1"))
                    {
                        MessageBox.Show("Etat modifier avec succès");
                    }
                    else
                    {
                        MessageBox.Show("Erreur lors de la modification");
                    }
                }
                else
                {
                    MessageBox.Show("Erreur cette commmande est déjà en cours");
                }
            }
            else
            {
                MessageBox.Show("Erreur lors de la modification");
            }
            lesCommandesDvds = controle.GetAllCommandeLivresDvd("dvd");
            RemplirCommandesDvdsListeComplete();
        }

        private void btnDvdsCommandeSetLivree_Click(object sender, EventArgs e)
        {
            Commande commande = lesCommandesDvds.Find(x => x.Id.Equals(int.Parse(txbDvdsNumeroCommande.Text)));
            if (commande != null)
            {
                if (commande.Etat != "livrée")
                {
                    List<Commande> commandes = new List<Commande>();
                    commandes.Add(commande);
                    if (controle.ModifyEtatCommande(commande, "2"))
                    {
                        MessageBox.Show("Etat modifier avec succès");
                    }
                    else
                    {
                        MessageBox.Show("Erreur lors de la modification");
                    }
                }
                else
                {
                    MessageBox.Show("Erreur cette commmande est déjà livrée");
                }
            }
            else
            {
                MessageBox.Show("Erreur lors de la modification");
            }
            lesCommandesDvds = controle.GetAllCommandeLivresDvd("dvd");
            RemplirCommandesDvdsListeComplete();
        }

        private void btnDvdsCommandeSetReglee_Click(object sender, EventArgs e)
        {
            Commande commande = lesCommandesDvds.Find(x => x.Id.Equals(int.Parse(txbDvdsNumeroCommande.Text)));
            if (commande != null)
            {
                if (commande.Etat != "réglée")
                {
                    List<Commande> commandes = new List<Commande>();
                    commandes.Add(commande);
                    if (controle.ModifyEtatCommande(commande, "3"))
                    {
                        MessageBox.Show("Etat modifier avec succès");
                    }
                    else
                    {
                        MessageBox.Show("Erreur lors de la modification");
                    }
                }
                else
                {
                    MessageBox.Show("Erreur cette commmande est déjà réglée");
                }
            }
            else
            {
                MessageBox.Show("Erreur lors de la modification");
            }
            lesCommandesDvds = controle.GetAllCommandeLivresDvd("dvd");
            RemplirCommandesDvdsListeComplete();
        }

        private void btnDvdsCommandeSetRelancee_Click(object sender, EventArgs e)
        {
            Commande commande = lesCommandesDvds.Find(x => x.Id.Equals(int.Parse(txbDvdsNumeroCommande.Text)));
            if (commande != null)
            {
                if (commande.Etat != "relancée")
                {
                    List<Commande> commandes = new List<Commande>();
                    commandes.Add(commande);
                    if (controle.ModifyEtatCommande(commande, "4"))
                    {
                        MessageBox.Show("Etat modifier avec succès");
                    }
                    else
                    {
                        MessageBox.Show("Erreur lors de la modification");
                    }
                }
                else
                {
                    MessageBox.Show("Erreur cette commmande est déjà relancée");
                }
            }
            else
            {
                MessageBox.Show("Erreur lors de la modification");
            }
            lesCommandesDvds = controle.GetAllCommandeLivresDvd("dvd");
            RemplirCommandesDvdsListeComplete();
        }

        private void btnDvdsCommandeSetDelete_Click(object sender, EventArgs e)
        {
            Commande commande = lesCommandesDvds.Find(x => x.Id.Equals(int.Parse(txbDvdsNumeroCommande.Text)));
            if (commande != null)
            {
                if (commande.Etat != "livrée")
                {
                    if ((MessageBox.Show("Êtes vous sur de vouloir supprimé cette commande ?", "Suppression d'une commande", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes))
                    {
                        List<Commande> commandes = new List<Commande>();
                        commandes.Add(commande);
                        if (controle.DeleteCommande(commande))
                        {
                            MessageBox.Show("Commande supprimée avec succès");
                        }
                        else
                        {
                            MessageBox.Show("Erreur lors de la suppression de la commande");
                        }
                    }

                }
                else
                {
                    MessageBox.Show("Erreur Impossible de supprimer une commande livrée");
                }
            }
            else
            {
                MessageBox.Show("Erreur lors de la suppression");
            }
            lesCommandesDvds = controle.GetAllCommandeLivresDvd("dvd");
            RemplirCommandesDvdsListeComplete();
        }

        private void btnDvdsCommandeCommander_Click(object sender, EventArgs e)
        {
            if (nudDvdsPrixCommandeCreate.Value == 0 && nudDvdsNombreCommandeCreate.Value == 0)
            {
                MessageBox.Show("Veuillez saisir le nombre d'exemplaire a commander ainsi que le prix.");
            }
            else
            {
                string rayonId = getRayonId(txbCommandeDvdsRayons.Text);
                string genreId = getGenresId(txbCommandeDvdsGenres.Text);
                string publicId = getPublicId(txbCommandeDvdsPublics.Text);

                Commande commande = new Commande(
                    getRandomOrderId(),
                    DateTime.Now,
                    (double)nudDvdsPrixCommandeCreate.Value * (double)nudDvdsNombreCommandeCreate.Value,
                    (int)nudDvdsNombreCommandeCreate.Value,
                    txbDvdsNumeroCommandeCreate.Text,
                    1,
                    "en cours",
                    txbDvdsTitreCommandeCreate.Text
                    );

                Dvd dvd = new Dvd(
                    txbDvdsNumeroCommandeCreate.Text,
                    txbDvdsTitreCommandeCreate.Text,
                    txbDvdsImageCommandeCreate.Text,
                    int.Parse(txbDvdsDureeCommandeCreate.Text),
                    txbDvdsRealisateurCommandeCreate.Text,
                    txbDvdsSynopsisCommandeCreate.Text,
                    genreId,
                    txbCommandeLivresGenres.Text,
                    publicId,
                    txbCommandeLivresPublics.Text,
                    rayonId,
                    txbCommandeLivresRayons.Text
                    );

                if (controle.CreerCommandeDvd(commande, dvd))
                {
                    MessageBox.Show("Commande créé avec success");
                }
                else
                {
                    MessageBox.Show("Une erreur est survenu lors de la création de la commande.");
                }
            }

            lesDvd = controle.GetAllDvd();
            lesCommandesDvds = controle.GetAllCommandeLivresDvd("dvd");
            RemplirCommandesDvdsListeComplete();
        }

        #endregion



        #region commande Revues

        private void tabCommandeRevues_Enter(object sender, EventArgs e)
        {
            RevueAbonnementRefresh();
        }

        /// <summary>
        /// Actualise les champs revue après une modification dans la bdd
        /// </summary>
        private void RevueAbonnementRefresh()
        {
            lesRevues = controle.GetAllRevues();
            RemplirRevuesListeCompleteBdd();
            txbRevuesNumeroCommandeCreate.Text = getRandomOrderId().ToString();
        }


        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        private void RemplirCommandeRevuesListe(List<Abonnement> lesCommandesRevues)
        {
            bdgCommandeRevueListe.DataSource = lesCommandesRevues;
            dgvRevuesListeCommande.DataSource = bdgCommandeRevueListe;
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        private void RemplirRevuesListeBdd(List<Revue> revues)
        {
            bdgRevuesListe.DataSource = revues;
            dgvRevuesListeBdd.DataSource = bdgRevuesListe;
            dgvRevuesListeBdd.Columns["empruntable"].Visible = false;
            dgvRevuesListeBdd.Columns["idRayon"].Visible = false;
            dgvRevuesListeBdd.Columns["idGenre"].Visible = false;
            dgvRevuesListeBdd.Columns["idPublic"].Visible = false;
            dgvRevuesListeBdd.Columns["image"].Visible = false;
            dgvRevuesListeBdd.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvRevuesListeBdd.Columns["id"].DisplayIndex = 0;
            dgvRevuesListeBdd.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Affichage de la liste complète des revues
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirRevuesListeCompleteBdd()
        {
            RemplirRevuesListeBdd(lesRevues);
        }

        private void dgvRevuesListeBdd_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dgvRevuesListeBdd.Columns[e.ColumnIndex].HeaderText;
            List<Revue> sortedList = new List<Revue>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesRevues.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesRevues.OrderBy(o => o.Titre).ToList();
                    break;
                case "Periodicite":
                    sortedList = lesRevues.OrderBy(o => o.Periodicite).ToList();
                    break;
                case "DelaiMiseADispo":
                    sortedList = lesRevues.OrderBy(o => o.DelaiMiseADispo).ToList();
                    break;
                case "Genre":
                    sortedList = lesRevues.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesRevues.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesRevues.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirRevuesListeBdd(sortedList);
        }

        private void dgvRevuesListeBdd_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvRevuesListeBdd.CurrentCell != null)
            {
                try
                {
                    Revue revue = (Revue)bdgRevuesListe.List[bdgRevuesListe.Position];
                    AfficheRevuesCommande(revue);
                    AfficheRevuesInfosCommande(revue);
                }
                catch
                {
                    VideRevuesZones();
                }
            }
        }

        /// <summary>
        /// Affiche les commande de la revue reçut en paramètre
        /// </summary>
        /// <param name="revue"></param>
        private void AfficheRevuesCommande(Revue revue)
        {
            List<Abonnement> abonnements = controle.GetRevueCommande(revue);
            RemplirCommandeRevuesListe(abonnements);
        }

        private void btnRevuesNumRechercheCommande_Click(object sender, EventArgs e)
        {
            if (!txbRevuesNumRechercheCommande.Text.Equals(""))
            {
                Revue revue = lesRevues.Find(x => x.Id.Equals(txbRevuesNumRechercheCommande.Text));
                if (revue != null)
                {
                    List<Revue> revues = new List<Revue>();
                    revues.Add(revue);
                    RemplirRevuesListeBdd(revues);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirRevuesListeCompleteBdd();
                }
            }
            else
            {
                RemplirRevuesListeCompleteBdd();
            }
        }

        /// <summary>
        /// Affichage des informations de la revue sélectionné
        /// </summary>
        /// <param name="revue"></param>
        private void AfficheRevuesInfosCommande(Revue revue)
        {
            txbRevuesPeriodiciteCommande.Text = revue.Periodicite;
            chkRevuesEmpruntableCommande.Checked = revue.Empruntable;
            txbRevuesImageCommande.Text = revue.Image;
            txbRevuesDelaiCommande.Text = revue.DelaiMiseADispo.ToString();
            txbRevuesNumeroCommande.Text = revue.Id;
            txbRevuesGenreCommande.Text = revue.Genre;
            txbRevuesPublicCommande.Text = revue.Public;
            txbRevuesRayonCommande.Text = revue.Rayon;
            txbRevuesTitreCommande.Text = revue.Titre;
            txbRevuesTitreCommandeCreate.Text = revue.Titre;

            string image = revue.Image;
            try
            {
                pcbRevuesImageCommande.Image = Image.FromFile(image);
            }
            catch
            {
                pcbRevuesImageCommande.Image = null;
            }
        }

        /// <summary>
        /// Affichage des informations de la revue sélectionné
        /// </summary>
        /// <param name="revue"></param>
        private void AfficheRevuesInfosCommandeUpdate(Abonnement abonnement)
        {
            txbRevuesNumeroCommandeUpdate.Text = abonnement.Id.ToString();
            txbRevuesTitreCommandeUpdate.Text = txbRevuesTitreCommande.Text;
            nudRevuesPrixAbonnementUpdate.Value = (decimal)abonnement.Montant;
            dtpRevuesDateAbonnementUpdate.Value = abonnement.DateExpiration;
        }

        private void btnRevuesAbonnementCreate_Click(object sender, EventArgs e)
        {
            if (nudRevuesPrixAbonnementCreate.Value == 0)
            {
                MessageBox.Show("Veuillez saisir le prix de l'abonnement par mois");
            }
            else if (dtpRevuesDateAbonnementCreate.Value < DateTime.Now)
            {
                MessageBox.Show("Veuillez saisir la date de fin d'abonnement");
            }
            else
            {
                Abonnement abonnement = new Abonnement(
                    int.Parse(txbRevuesNumeroCommandeCreate.Text),
                    DateTime.Now,
                    DateTime.Parse(dtpRevuesDateAbonnementCreate.Text),
                    (double)nudRevuesPrixAbonnementCreate.Value,
                    txbRevuesNumeroCommande.Text
                    );

                if (controle.CreerAbonnementRevue(abonnement))
                {
                    MessageBox.Show("Abonnement créé avec success");
                }
                else
                {
                    MessageBox.Show("Une erreur est survenu lors de la création de la l'abonnement.");
                }
            }
            RevueAbonnementRefresh();
        }

        private void dgvRevuesListeCommande_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvRevuesListeCommande.CurrentCell != null)
            {
                try
                {
                    Abonnement abonnement = (Abonnement)bdgCommandeRevueListe.List[bdgCommandeRevueListe.Position];
                    AfficheRevuesInfosCommandeUpdate(abonnement);
                }
                catch
                {
                    VideRevuesZones();
                }
            }
        }
        private void btnRevuesAbonnementUpdate_Click(object sender, EventArgs e)
        {
            if (nudRevuesPrixAbonnementUpdate.Value == 0)
            {
                MessageBox.Show("Veuillez saisir le prix de l'abonnement par mois");
            }
            else if (dtpRevuesDateAbonnementUpdate.Value < DateTime.Now)
            {
                MessageBox.Show("Veuillez saisir la date de fin d'abonnement");
            }
            else
            {
                Abonnement oldAbonnement = (Abonnement)bdgCommandeRevueListe.List[bdgCommandeRevueListe.Position];

                Abonnement abonnement = new Abonnement(
                    int.Parse(txbRevuesNumeroCommandeUpdate.Text),
                    oldAbonnement.DateCommande,
                    DateTime.Parse(dtpRevuesDateAbonnementUpdate.Text),
                    (double)nudRevuesPrixAbonnementUpdate.Value,
                    txbRevuesNumeroCommande.Text
                    );

                if (controle.UpdateAbonnementRevue(abonnement))
                {
                    MessageBox.Show("Abonnement modifié avec success");
                }
                else
                {
                    MessageBox.Show("Une erreur est survenu lors de la modification de la l'abonnement.");
                }
            }
            RevueAbonnementRefresh();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Abonnement abonnement = (Abonnement)bdgCommandeRevueListe.List[bdgCommandeRevueListe.Position];
            if (!controle.ParutionDansAbonnement(abonnement.DateCommande, abonnement.DateExpiration, DateTime.Now))
            {
                if ((MessageBox.Show("Êtes vous sur de vouloir supprimé cette commande ?", "Suppression d'une commande",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes))
                {
                    if (controle.DeleteAbonnement(abonnement))
                    {
                        MessageBox.Show("Abonnement supprimée avec succès");
                    }
                    else
                    {
                        MessageBox.Show("Erreur lors de la suppression de l'abonnement");
                    }
                }
            }
            else
            {
                MessageBox.Show("Impossible de supprimer cet abonnement, il est toujours en cours");
            }
            RevueAbonnementRefresh();

        }


        #endregion


    }
}
