using Entidades;
using LN;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TESIS
{
    public partial class TestDescargar : Form
    {
        public TestDescargar()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Usuarios usuario = new Usuarios();
            usuario.id = 3;
            SocketApp socketApp = new SocketApp("127.0.0.1", 5656, usuario);
            //para q actualize cuando envie todo
            //socketApp.dataGridViewArchivos = dataGridViewArchivos;
            //  socketApp.progresSpinnerLoad = ProgresSpinnerLoad;
            Archivos archivoSElect = new Archivos();
            archivoSElect.id = 79;
            archivoSElect = LNArchivos.Instance.ListarId(archivoSElect);
            //Console.WriteLine(archivoSElect.nombre);
           // socketApp.descargarArchivo(archivoSElect.nombre);
        }
    }
}
