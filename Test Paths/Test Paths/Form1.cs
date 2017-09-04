using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Test_Paths
{
    public partial class Form1 : Form
    {
        public string image_Path = "";
        public string imageRenommee = "";
        public string ipAdresse = "";
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.tif;*.tiff*,.tga...";
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {

                image_Path = openFileDialog1.FileName;
                loadImage(openFileDialog1.FileName);
            }
        }

        public void loadImage(string filename)
        {
            
            //FileInfo fi2 = new FileInfo(filename);
            //image_Path = UncPath(fi2);
            //image_Path = GetUNCByDrive(filename);
            
            string drivePrefix = image_Path.Substring(0, 1);
            imageRenommee = "\\\\Ds-Master\\" + drivePrefix + image_Path.Remove(0, 2);
            chemin.Text = imageRenommee;
            try
            {
                box.Image = Image.FromFile(imageRenommee);
                box.SizeMode = PictureBoxSizeMode.Zoom;
            }
            catch (Exception) { }
        }

        private void envoyer_Click(object sender, EventArgs e)
        {
            Byte[] commande;
            //fadeOutToutLeMonde();
            commande = Ds2Command("Text Add \"AllSky_1\" \"" + imageRenommee + "\" 0 0 Local 90 0 0 1 1 ");
            //MessageBox.Show("Text Add \"AllSky_1\" \"" + imageRenommee + "\" 0 0 Local 90 0 0 1 1 ");
            envoyerCommande(commande);
            Thread.Sleep(100);
            commande = Ds2Command("Text Locate \"AllSky_1\" 0 0 90 0 180 180 ");
           // MessageBox.Show("Text Locate \"AllSky_1\" 0 0 90 0 180 180 ");
            envoyerCommande(commande);
            Thread.Sleep(50);
            commande = Ds2Command("Text View \"AllSky_1\"  0 100 100 100 100");
            //MessageBox.Show("Text View \"AllSky_1\"  0 100 100 100 100");
            envoyerCommande(commande);
            Thread.Sleep(50);
        }

        private Byte[] Ds2Command(string command)
        {
            Byte[] sendBytes = Encoding.ASCII.GetBytes("\x02" + "DSTA00000000" + "\x07" + "DIRECT" + "\x07" + command + "\x03");
            return sendBytes;
        }

        private void envoyerCommande(Byte[] commande)
        {
            try
            {
                ipAdresse = textBox1.Text;
                UdpClient udpClient = new UdpClient();
                udpClient.Send(commande, commande.Length, ipAdresse, 2209);
                //udpClient.Send(commande, commande.Length, "127.0.0.1", 2209);
            }
            catch (Exception)
            {

            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
          
        }
    }
}
