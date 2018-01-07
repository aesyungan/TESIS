using Entidades;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LN;
namespace TESIS
{
    public partial class FormHome : MetroFramework.Forms.MetroForm
    {
        public Usuarios usuario { get; set; }
        public Archivos archivoSelected = new Archivos();
        public bool isSelected;
        public FormHome(Usuarios usuarios)
        {
            InitializeComponent();
            usuario = usuarios;
            // MessageBox.Show(usuario.nombre);
            CargarDatos(usuario);
            activeBtnDetails(isSelected);
            //details
            ShowDetails();
            //user log
            lbUserName.Text = usuario.nombre;
            //spinner
            showSpinner(false);
        
        }


        public void CargarDatos(Usuarios item)
        {
            dataGridViewArchivos.DataSource = null;
            dataGridViewArchivos.DataSource = LNArchivos.Instance.Listar(item);
            if (dataGridViewArchivos.Rows.Count > 0)
            {//oculta columnas
                dataGridViewArchivos.Columns[0].Visible = false;
                dataGridViewArchivos.Columns[1].Visible = false;
                dataGridViewArchivos.Columns[4].Visible = false;
            }
            // dataGridViewArchivos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            dataGridViewArchivos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;//100% content
        }
        public void showSpinner(bool estado){
            ProgresSpinnerLoad.Visible = estado;
            
        }
        private void acercaDeToolStripMenuItem_Click(object sender, EventArgs e)
        {

            FormAcerca frm = new FormAcerca();

            // Show Form

            frm.ShowDialog();
        }

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
            //this.Close();
        }

        private void cerrarSessiónToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormLogin frm = new FormLogin();
            frm.Show();
        }



        private void clickRowItem(object sender, DataGridViewCellMouseEventArgs e)
        {
            int ID = Convert.ToInt32(dataGridViewArchivos.Rows[e.RowIndex].Cells[0].Value.ToString());//optiene ID
            archivoSelected.id = ID;
            archivoSelected = LNArchivos.Instance.ListarId(archivoSelected);
            isSelected = true;
            activeBtnDetails(isSelected);
            ShowDetails();
            // MessageBox.Show(archivoSelected.nombre);
        }

        public void ShowDetails()
        {
            lnNombre.Text = archivoSelected.nombre;
            lbFecha.Text = archivoSelected.fecha;
            lbUbicacion.Text = archivoSelected.ubicacion;
        }
        public void activeBtnDetails(bool estado)
        {
            btnCambiar.Enabled = estado;
            btnDescargar.Enabled = estado;
            btnEliminar.Enabled = estado;
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            LNArchivos.Instance.Eliminar(archivoSelected);
            CargarDatos(usuario);
            activeBtnDetails(false);
            MessageBox.Show("Eliminación Correcta", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnDescargar_Click(object sender, EventArgs e)
        {
            showSpinner(true);
        }

        private void btnCambiar_Click(object sender, EventArgs e)
        {
            showSpinner(false);
        }
    }
}
