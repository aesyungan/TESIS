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
        string hostServidor= Properties.Settings.Default.HostServer;
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
            try
            {
                showSpinner(true);
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
                showSpinner(false);
            }
            catch (Exception ex)
            {
                showSpinner(false);
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
       
        }
        public void showSpinner(bool estado)
        {
            ProgresSpinnerLoad.Visible = estado;
            if (estado == false)
                ShowDetailsBorrar();
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
        public void ShowDetailsBorrar()
        {
            lnNombre.Text = "";
            lbFecha.Text = "";
            lbUbicacion.Text = "";
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
            try
            {
                showSpinner(true);
                LNArchivos.Instance.Eliminar(archivoSelected);
                CargarDatos(usuario);
                activeBtnDetails(false);
                MessageBox.Show("Eliminación Correcta", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ShowDetailsBorrar();
                showSpinner(false);
            }
            catch (Exception ex)
            {
                showSpinner(false);
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
          
        }

        private void btnDescargar_Click(object sender, EventArgs e)
        {
            try
            {
                showSpinner(true);
                Usuarios usuario = new Usuarios();
                usuario.id = 3;
                SocketApp socketApp = new SocketApp(hostServidor, 5656, usuario);
                //para q actualize cuando envie todo
                //socketApp.dataGridViewArchivos = dataGridViewArchivos;
                //  socketApp.progresSpinnerLoad = ProgresSpinnerLoad;

                //archivoSElect.id = 79;
                //archivoSElect = LNArchivos.Instance.ListarId(archivoSElect);
                //Console.WriteLine(archivoSElect.nombre);
                socketApp.progresSpinnerLoad = ProgresSpinnerLoad;
                socketApp.descargarArchivo(archivoSelected.nombre);
                //activeBtnDetails(false);
            }
            catch (Exception ex)
            {
                showSpinner(false);
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            try
            {
                OpenFileDialog openFile = new OpenFileDialog();
                showSpinner(true);
                if (openFile.ShowDialog() == DialogResult.OK)
                {//directorio del archivo a enviar

                    string nombreFile = CopyFile(openFile.FileName, openFile.SafeFileName);
                    Console.WriteLine(nombreFile);
                    SocketApp socketApp = new SocketApp(hostServidor, 5656, usuario);
                    //para q actualize cuando envie todo
                    socketApp.dataGridViewArchivos = dataGridViewArchivos;
                    socketApp.progresSpinnerLoad = ProgresSpinnerLoad;
                    socketApp.UpdateArchivo(nombreFile, archivoSelected.id);
                    //InitSocket(nombreFile);

                    // CargarDatos(usuario);
                    // showSpinner(false);
                    ShowDetailsBorrar();
                    activeBtnDetails(false);
                }
                else
                {
                    showSpinner(false);
                }
            }
            catch (Exception ex)
            {
                showSpinner(false);
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
          
            
        }
        //selecciona el dirrectorio del archivo 
        private void archivoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFile = new OpenFileDialog();
                showSpinner(true);
                if (openFile.ShowDialog() == DialogResult.OK)
                {//directorio del archivo a enviar

                    string nombreFile = CopyFile(openFile.FileName, openFile.SafeFileName);
                    Console.WriteLine(nombreFile);
                    SocketApp socketApp = new SocketApp(hostServidor, 5656, usuario);
                    //para q actualize cuando envie todo
                    socketApp.dataGridViewArchivos = dataGridViewArchivos;
                    socketApp.progresSpinnerLoad = ProgresSpinnerLoad;
                    socketApp.InitSocketEnviar(nombreFile, usuario.id);
                    //InitSocket(nombreFile);

                    // CargarDatos(usuario);
                    // showSpinner(false);
                    activeBtnDetails(false);
                    ShowDetailsBorrar();
                }
                else
                {
                    showSpinner(false);
                }
            }
            catch (Exception ex)
            {
                showSpinner(false);
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
           


        }
        public string CopyFile(string pathOrigen, string nombreFile)
        {
            string pathProyect = Directory.GetCurrentDirectory();
            long milliseconds = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            nombreFile = milliseconds.ToString() + nombreFile;
            //Console.WriteLine(milliseconds.ToString());
            pathProyect = pathProyect + "\\envios\\planos-medianos\\" + nombreFile;
            File.Copy(@"" + pathOrigen, @"" + pathProyect);
            return nombreFile;
        }

    }
}
