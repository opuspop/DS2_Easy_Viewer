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
            box.Image = Image.FromFile(filename);
            box.SizeMode = PictureBoxSizeMode.Zoom;
            //FileInfo fi2 = new FileInfo(filename);
            //image_Path = UncPath(fi2);
            //image_Path = GetUNCByDrive(filename);
            
            string drivePrefix = image_Path.Substring(0, 1);
            string imageRenommee = "\\\\Ds-Master\\" + drivePrefix + image_Path.Remove(0, 2);
            chemin.Text = imageRenommee;
        }

        public string GetUNCByDrive(string sPath)
        {
            string sReturn = string.Empty;
            string sDrive = null;
            if (sPath.Length > 2)
            {
                sDrive = sPath.Substring(0, 2);
                sPath = sPath.Substring(2);
            }
            else
            {
                sDrive = sPath;
                sPath = string.Empty;
            }
            try
            {
                System.Management.ManagementObjectSearcher searcher = new System.Management.ManagementObjectSearcher("root\\CIMV2", "SELECT Providername FROM Win32_LogicalDisk WHERE Name = '" + sDrive + "'");
                System.Management.ManagementObjectCollection oResult = searcher.Get();
                if (oResult == null)
                {
                    return sDrive + sPath;
                }
                foreach (System.Management.ManagementObject oManagmentObject in oResult)
                {
                    sReturn = oManagmentObject.GetPropertyValue("Providername") + sPath;

                }
            }
            catch (System.Management.ManagementException err)
            {

                MessageBox.Show("An error occurred while querying for WMI data: " + err.Message);
            }
            if (string.IsNullOrEmpty(sReturn))
            {
                return sDrive + sPath;
            }
            else
            {
                return sReturn;

            }

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
                UdpClient udpClient = new UdpClient();
                //udpClient.Send(commande, commande.Length, "192.168.0.100", 2209);
                udpClient.Send(commande, commande.Length, "127.0.0.1", 2209);
            }
            catch (Exception)
            {

            }
        }

        private void envoyer_Click(object sender, EventArgs e)
        {
            Byte[] commande;
            //fadeOutToutLeMonde();
            commande = Ds2Command("Text Add \"AllSky_1\" "+ image_Path + " 0 0 Local 90 0 0 1 1 ");
            envoyerCommande(commande);
            Thread.Sleep(100);
            commande = Ds2Command("Text Locate \"AllSky_1\" 0 0 90 0 180 180 ");
            envoyerCommande(commande);
            Thread.Sleep(50);
            commande = Ds2Command("Text View \"AllSky_1\"  0 100 100 100 100");
            envoyerCommande(commande);
            Thread.Sleep(50);
        }
    }
}
