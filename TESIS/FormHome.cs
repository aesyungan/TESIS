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
using System.IO;

namespace TESIS
{
    public partial class FormHome : MetroFramework.Forms.MetroForm
    {
        public Usuarios usuario { get; set; }
        public Archivos archivoSelected = new Archivos();
        public Archivos archivoInsert = new Archivos();
        public bool isSelected;
        //path de l archivo a enviar 
        static string pathFileSend = "";
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
        public void showSpinner(bool estado)
        {
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
            //codigo para q descarge el archo del socket


            //  byte[] dataToSend = File.ReadAllBytes(@"" + path);
            // FileStream fileStream = new FileStream(pathFileSend, FileMode.Create, FileAccess.ReadWrite);
             Stream myStream ;//del socket

            //dialog para guardar file
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            string namFile = "data.png";
            saveFileDialog1.FileName = namFile;
            saveFileDialog1.Filter = @"All Files|*.txt;*.docx;*.doc;*.pdf*.xls;*.xlsx;*.pptx;*.ppt|Text File (.txt)|*.txt|Word File (.docx ,.doc)|*.docx;*.doc|PDF (.pdf)|*.pdf|Spreadsheet (.xls ,.xlsx)|  *.xls ;*.xlsx|Presentation (.pptx ,.ppt)|*.pptx;*.ppt";
            // saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;
            saveFileDialog1.DefaultExt = "." + GetClearExtension(pathFileSend);
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {

                if ((myStream = saveFileDialog1.OpenFile()) != null)
                {
                    // Code to write the stream goes here.
                    // MessageBox.Show(saveFileDialog1.FileName);
                    myStream.Close();
                    showSpinner(false);
                }

            }
            else
            {
                showSpinner(false);
            }


        }
        public static string GetClearExtension(string filePath)
        {
            return String.IsNullOrEmpty(filePath)
                ? null
                : Path.GetExtension(filePath).Substring(1).ToLower();
        }
        public void guardarStream(Stream stream, string destPath)
        {
            using (var fileStream = new FileStream(destPath, FileMode.Create, FileAccess.Write))
            {
                stream.CopyTo(fileStream);
            }
        }

        private void btnCambiar_Click(object sender, EventArgs e)
        {
            showSpinner(false);
        }
        //selecciona el dirrectorio del archivo 
        private void archivoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            if (openFile.ShowDialog() == DialogResult.OK)
            {//directorio del archivo a enviar
                pathFileSend = openFile.FileName;
                showSpinner(true);
                ////dataBase
                //archivoInsert.fecha= DateTime.Now.ToString("M/d/yyyy");
                //archivoInsert.nombre = openFile.SafeFileName;
                //archivoInsert.ubicacion = openFile.FileName;
                //archivoInsert.usuarios.id = usuario.id;
                //LNArchivos.Instance.Insertar(archivoInsert);


                //codigo para enviar por socket aqui


                CargarDatos(usuario);
                showSpinner(false);
            }
        }

    }
}
