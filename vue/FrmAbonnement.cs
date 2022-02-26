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

        private void button1_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }
    }
}
