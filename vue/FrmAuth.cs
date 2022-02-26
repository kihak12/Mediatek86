using Mediatek86.controleur;
using Mediatek86.metier;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mediatek86.vue
{
    public partial class FrmAuth : Form
    {
        private Controle controle;
        private readonly BindingSource bdgDroits = new BindingSource();

        internal FrmAuth(Controle controleP)
        {
            InitializeComponent();
            controle = controleP;
            RemplirComboBox();
        }

        private void RemplirComboBox(){
            bdgDroits.DataSource = controle.getAllPostes();
            cbxPoste.DataSource = bdgDroits;
            if (cbxPoste.Items.Count > 0)
            {
                cbxPoste.SelectedIndex = -1;
            }
        }

        private static void runMediatek(Controle controle, int result)
        {
            FrmMediatek frmMediatek = new FrmMediatek(controle, result);
            frmMediatek.ShowDialog();
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string pseudo = cbxPoste.Text;
            string pass = txbPassword.Text;

            if (pseudo != "" && pass != "")
            {
                int result = controle.checkUserLogin(pseudo, pass);

                if (result == 5)
                {
                    MessageBox.Show("Identifiants incorrects");
                }
                else if (result == 4)
                {
                    MessageBox.Show("Vos droits ne sont pas suffisants pour accéder à cette application");
                    this.Close();
                }
                else
                {
                    this.Hide();
                    runMediatek(controle, result);
                }
            }
            else
            {
                MessageBox.Show("Veuillez saisir votre poste ainsi que votre mot de passe");
            }
        }
    }
}
