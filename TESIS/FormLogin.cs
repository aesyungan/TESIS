using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Entidades;
using LN;

namespace TESIS
{
    public partial class FormLogin : MetroFramework.Forms.MetroForm
    {
        public FormLogin()
        {
            InitializeComponent();
            //pictureBoxHeader.Image = new System.Drawing.Bitmap(@"\img\LATERALSUPERIOR.png");
            txtClave.PasswordChar = '*';
        }



        private void btnCancelar_Click(object sender, EventArgs e)
        {
            txtCedula.Text = "";
            txtClave.Text = "";
        }

        private void btnLogear_Click(object sender, EventArgs e)
        {
            try
            {
                Usuarios usuarios = new Usuarios();
                usuarios.cedula = txtCedula.Text;
                usuarios.clave = txtClave.Text;
                usuarios = LNUsuarios.Instance.Logear(usuarios);
                if (usuarios != null)
                {
                    FormHome frm = new FormHome(usuarios);
                    frm.Show();
                    this.Visible = false;
                }
                else
                {
                    MessageBox.Show("Cédula o Clave Incorrectos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
          


        }
        //salir
        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //DialogResult x = MessageBox.Show("desea salir", "programa", MessageBoxButtons.YesNo);
            //if (x == DialogResult.Yes)
            //{
            //    this.Close();
            //}

            Application.Exit();
        }
        //cerca
        private void acercaDeToolStripMenuItem_Click(object sender, EventArgs e)
        {

            FormAcerca frm = new FormAcerca();

            // Show Form

            frm.ShowDialog();
        }

       
    }
}
