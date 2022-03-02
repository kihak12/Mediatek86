using Mediatek86.metier;
using System.Collections.Generic;
using System.Windows.Forms;
using Mediatek86.modele;

namespace Mediatek86.vue
{
    public partial class FrmAbonnement : Form
    {
        private readonly BindingSource bdgCommandeRevueListe = new BindingSource();

        public FrmAbonnement()
        {
            InitializeComponent();
            RemplirDgv();
        }

        /// <summary>
        /// Search for subscriptions ending in less than 30 days
        /// </summary>
        /// <returns></returns>
        public bool affect()
        {
            List<Abonnement> abonnements = Dao.GetAllAbonnementEpiration();
            if (abonnements.Count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Remplie le DataGridView avec les abonnement arrivant a expiration
        /// </summary>
        private void RemplirDgv()
        {
            List<Abonnement> abonnements = Dao.GetAllAbonnementEpiration();
            int y = 0;

            foreach (Abonnement o in abonnements)
            {
                Revue revue = Dao.selectRevueById(o.IdRevue);
                abonnements[y].IdRevue = revue.Titre;
                y++;
            }

            bdgCommandeRevueListe.DataSource = abonnements;
            dgvAbonnementExpirationListe.DataSource = bdgCommandeRevueListe;
            dgvAbonnementExpirationListe.Columns["idRevue"].DisplayIndex = 0;
            dgvAbonnementExpirationListe.Columns["id"].DisplayIndex = 4;
            dgvAbonnementExpirationListe.Columns["IdRevue"].HeaderText = "Titre de la revue";
            dgvAbonnementExpirationListe.Columns["Id"].HeaderText = "Numéro de commande";
            dgvAbonnementExpirationListe.Columns["Montant"].HeaderText = "Montant(€)";
            dgvAbonnementExpirationListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        }

        /// <summary>
        /// Ferme la fenêtre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }
    }
}
