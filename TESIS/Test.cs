using Entidades;
using LN;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TESIS
{
    public partial class Test : Form
    {
        public static int cont = 0;

        public Test()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            if (openFile.ShowDialog() == DialogResult.OK)
            {//directorio del archivo a enviar
                string nombreFile = CopyFile(openFile.FileName, openFile.SafeFileName);
                Console.WriteLine(nombreFile);
                InitSocket(nombreFile,3);
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

        public void InitSocket(string nomreArchivoSelect, int id_user )
        {
            Console.WriteLine("Inicio");
            DirectoryInfo di = new DirectoryInfo(@"envios\planos-medianos");
            Console.WriteLine("Ingrese el número de archivos a enviar:");
            //int cont = Convert.ToInt32(Console.ReadLine());
            int cont = 1;
            //Crear objeto StreamWriter que sera el que nos ayude a escribir sobre archivo
            //StreamWriter escrito = File.CreateText("pruebas3/Pruebas1.txt");
            //String q = ("Nro." + "," + "Tamaño del Archivo" + "," + "Tiempo de Envio de Clave" + "," + "Tiempo de envio de Criptograma" + "," + "Tiempo de Envio Cifrado de Criptograma" + "," + "Tiempo de Descifrado de Criptograma");

            ////Escribimos en el archivo con el metodo write 
            //escrito.WriteLine(q);
            //escrito.Flush();

            for (int i = 0; i < cont; i++)
            {
                Console.WriteLine("\nArchivo N° " + (i + 1));
                Console.WriteLine("Envio archivo: " + di.GetFiles()[i].Name);
                enviarArchivo(nomreArchivoSelect,id_user);

            }
            Console.WriteLine("!Proceso completado con éxito!!\n");
            Console.ReadLine();
        }


        public void enviarArchivo(String filename, int id_user )
        {
            String p;
            long sz;
            IPEndPoint ipEnd = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5656);
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            sock.Connect(ipEnd);

            String filedirectory = "envios/";
            String filenamekey = "clave.crypt";
            FileEncryption fe = new FileEncryption();
            fe.makeKey();
            fe.saveKey("claves/" + filenamekey, "public.pem");
            //tiempo  de cifrado del archivo
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            //DateTime tiempo5 = DateTime.Now;
            //Console.WriteLine("INICIO tiempo: " + tiempo5.Ticks);
            fe.EncryptFile(filedirectory + "planos-medianos/" + filename, filedirectory + "cifrados/" + filename);
            //tiempo de finalizacion del cifrado archivo
            stopwatch.Stop();
            double tECrip = stopwatch.ElapsedMilliseconds;
            //DateTime tiempo6 = DateTime.Now;
            //  Console.WriteLine("FIN tiempo: " + tiempo6.Ticks);
            //double tECrip = (tiempo6 - tiempo5).TotalMilliseconds;
            //TimeSpan tECrip = new TimeSpan(tiempo6.Ticks - tiempo5.Ticks);
            Console.WriteLine("TIEMPO DE CIFRADO DE ARCHIVO:  {0} ms", tECrip);
            //  Console.WriteLine("TIEMPO DE CIFRADO DE ARCHIVO:  {0} ms", tECrip.TotalMilliseconds);

            sock.Send(Encoding.ASCII.GetBytes(filenamekey));

            int bytesReceived = 1;
            var buffer = new byte[8192];

            do
            {
                bytesReceived = sock.Receive(buffer);
            } while (!(bytesReceived > 0));

            String ack = Encoding.UTF8.GetString(buffer, 0, bytesReceived);

            Console.WriteLine("Recibido: " + ack);

            FileStream fileStream = new FileStream("claves/" + filenamekey, FileMode.Open);
            byte[] buff = new byte[1024];
            int bytesRead = 0;
            //envio de la clave
            var stopwatch1 = new Stopwatch();
            stopwatch1.Start();
            //DateTime tiempo1 = DateTime.Now;
            //Console.WriteLine("INICIO tiempo: " + tiempo1.Ticks);
            do
            {
                bytesRead = fileStream.Read(buff, 0, buff.Length);
                if (bytesRead > 0)
                {
                    sock.Send(buff, bytesRead, SocketFlags.None);
                }
            } while (bytesRead > 0);
            fileStream.Close();
            //tiempo de finalizacion de envio clave
            stopwatch1.Stop();
            //DateTime tiempo2 = DateTime.Now;
            //Console.WriteLine("INICIO fin: " + tiempo2.Ticks);
            double tEClave = stopwatch1.ElapsedMilliseconds;
            Console.WriteLine("TIEMPO DE ENVIO DE CLAVE:  {0} ms", tEClave);
            //  TimeSpan tEClave = new TimeSpan(tiempo2.Ticks - tiempo1.Ticks);
            // Console.WriteLine("TIEMPO DE ENVIO DE CLAVE:  {0} ms", tEClave.TotalMilliseconds);

            //TimeSpan tEClave = (tiempo2.TimeOfDay);
            //TimeSpan tEClave1 = (tiempo1.TimeOfDay);
            //Console.WriteLine("TIEMPO DE ENVIO DE CLAVE: {0:fff} ", (tEClave.TotalMilliseconds - tEClave1.TotalMilliseconds) + " milisegundos");

            //Recibe confirmación
            buff = new byte[1024];
            bytesReceived = 1;
            //confirmacion de recepcion de la clave

            do
            {
                bytesReceived = sock.Receive(buff);
            } while (!(bytesReceived > 0) && bytesReceived > buff.Length);

            ack = Encoding.UTF8.GetString(buff, 0, bytesReceived);

            Console.WriteLine("Recibido clave: " + ack);

            //proceso envio de archivo cifrado

            sock.Send(Encoding.ASCII.GetBytes(filename));

            bytesReceived = 1;
            buffer = new byte[8192];

            do
            {
                bytesReceived = sock.Receive(buffer);
            } while (!(bytesReceived > 0));

            ack = Encoding.UTF8.GetString(buffer, 0, bytesReceived);
            Console.WriteLine("Recibido: " + ack);

            fileStream = new FileStream(filedirectory + "cifrados/" + filename, FileMode.Open);

            sock.Send(Encoding.ASCII.GetBytes(fileStream.Length.ToString())); //Envio de tamano
            do
            {
                bytesReceived = sock.Receive(buffer);
            } while (!(bytesReceived > 0));

            ack = Encoding.UTF8.GetString(buffer, 0, bytesReceived);
            Console.WriteLine("Recibido Tamaño:" + ack);

            //fileStream = new FileStream(filedirectory + "cifrados/" + filename, FileMode.Open);
            sz = fileStream.Length;
            buff = new byte[8192];
            bytesRead = 0;
            var stopwatch2 = new Stopwatch();
            stopwatch2.Start();
            // DateTime tiempo3 = DateTime.Now;
            //Console.WriteLine("INICIO tiempo: " + tiempo3.Ticks);
            //envio de criptograma
            do
            {
                bytesRead = fileStream.Read(buff, 0, buff.Length);
                if (bytesRead > 0)
                {
                    //sock.Send(buff);
                    sock.Send(buff, bytesRead, SocketFlags.None);
                }
            } while (bytesRead > 0);
            fileStream.Close();

            bytesReceived = 1;
            buffer = new byte[8192];

            do
            {
                bytesReceived = sock.Receive(buffer);
            } while (!(bytesReceived > 0));

            ack = Encoding.UTF8.GetString(buffer, 0, bytesReceived);

            // long tiempo4 = Convert.ToInt64(ack);
            stopwatch2.Stop();
            //DateTime tiempo4 = DateTime.Now;
            //Console.WriteLine("FIN tiempo: " + tiempo4.Ticks);
            //TimeSpan tCifradoClave = new TimeSpan(tiempo4.Ticks - tiempo3.Ticks);
            double tCifradoClave = stopwatch2.ElapsedMilliseconds;
            Console.WriteLine("TIEMPO DE ENVIO DE CRIPTOGRAMA:  {0} ms", tCifradoClave);
            //Console.WriteLine("TIEMPO DE ENVIO DE CRIPTOGRAMA: {0} ms", tCifradoClave.TotalMilliseconds);

            //TimeSpan tCifradoClave = (tiempo4.TimeOfDay);
            //TimeSpan tCifradoClave1 = (tiempo3.TimeOfDay);
            //Console.WriteLine("TIEMPO DE ENVIO DE CRIPTOGRAMA: {0:fff} " + (tCifradoClave.TotalMilliseconds - tCifradoClave1.TotalMilliseconds) + " milisegundos");

            bytesReceived = 1;
            buffer = new byte[8192];

            do
            {
                bytesReceived = sock.Receive(buffer);
            } while (!(bytesReceived > 0));

            ack = Encoding.UTF8.GetString(buffer, 0, bytesReceived);
            cont++;
            Console.WriteLine("Recibido " + cont + ": " + ack + " ms");
            //--------------------------------------------------------------------------------
            //envia id Del Usuario Q Inserta 
           
            Console.WriteLine("Envio Id Usuario:" + id_user);
            sock.Send(Encoding.ASCII.GetBytes(id_user.ToString())); //Envio de usuario q Inserta
            //fin envia Id Usuario
            //--------------------------------------------------------------------------------
            sock.Close();
            sock = null;
            Console.WriteLine("Proceso concluido");

            //Concatenar variables de tiempos para guardar en un string
            p = (cont + "," + sz + "," + tEClave + "," + tCifradoClave + "," + tECrip + "," + ack);

            //Escribimos en el archivo con el metodo write 
            //guardar.WriteLine(p);
            // guardar.Flush();
            // escribir.close();
        }


    }
}
