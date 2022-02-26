
namespace Mediatek86.vue
{
    partial class FrmAbonnement
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dgvAbonnementExpirationListe = new System.Windows.Forms.DataGridView();
            this.label6 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAbonnementExpirationListe)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvAbonnementExpirationListe
            // 
            this.dgvAbonnementExpirationListe.AllowUserToAddRows = false;
            this.dgvAbonnementExpirationListe.AllowUserToDeleteRows = false;
            this.dgvAbonnementExpirationListe.AllowUserToResizeColumns = false;
            this.dgvAbonnementExpirationListe.AllowUserToResizeRows = false;
            this.dgvAbonnementExpirationListe.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAbonnementExpirationListe.Location = new System.Drawing.Point(15, 37);
            this.dgvAbonnementExpirationListe.MultiSelect = false;
            this.dgvAbonnementExpirationListe.Name = "dgvAbonnementExpirationListe";
            this.dgvAbonnementExpirationListe.ReadOnly = true;
            this.dgvAbonnementExpirationListe.RowHeadersVisible = false;
            this.dgvAbonnementExpirationListe.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvAbonnementExpirationListe.Size = new System.Drawing.Size(681, 215);
            this.dgvAbonnementExpirationListe.TabIndex = 4;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(12, 9);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(364, 16);
            this.label6.TabIndex = 2;
            this.label6.Text = "Abonnement(s) arrivant a expiration dans moins de 30 jours :";
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(312, 267);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(90, 30);
            this.button1.TabIndex = 5;
            this.button1.Text = "Fermer";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // FrmAbonnement
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(714, 310);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.dgvAbonnementExpirationListe);
            this.Name = "FrmAbonnement";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Rappel abonnement";
            ((System.ComponentModel.ISupportInitialize)(this.dgvAbonnementExpirationListe)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvAbonnementExpirationListe;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button button1;
    }
}