using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Collections.Generic;
using System.Net.Sockets;
using System;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Management;
using System.Linq;

namespace DS2_Easy_Viewer
{
    public partial class Form1 : Form
    {

        public static List<imageBox> imageBoxList = new List<imageBox>();
        public static int boiteSelectionnee;

        public Form1()
        {
            InitializeComponent();
            Rotation_value_txt.Text = "0";
            Azimuth_value_txt.Text = "90";
            Elevation_value_txt.Text = "90";
            Height_value_txt.Text = "180";
            Width_value_txt.Text = "180";
            Opacite_value_txt.Text = "100";
            //MessageBox.Show("Text Locate \"AllSky_1\" " + string.Join(" ", textLOCATEParamListdeListe[0].ToArray()));

            for (int i = 0; i < 10; i++)
            {
                imageBox box = new imageBox(this, i);
                imageBoxList.Add(box);
            }
        }

        public void slider_Height_Scroll(object sender, EventArgs e)
        {
            try
            {
                Byte[] commande;

                imageBoxList[boiteSelectionnee].textLocateParameters[5] = slider_Height.Value;
                commande = Ds2Command("Text Locate \"AllSky_" + (boiteSelectionnee) + "\" " + string.Join(" ", imageBoxList[boiteSelectionnee].textLocateParameters.ToArray()) + " \"");
                //MessageBox.Show("Text Locate \"AllSky_" + (boiteSelectionnee + 1) + "\" " + string.Join(" ", imageBoxList[boiteSelectionnee].textLocateParameters.ToArray()) + " \"");
                envoyerCommande(commande);
                Height_value_txt.Text = slider_Height.Value.ToString();
            }
            catch (Exception) { }
        }
        private void slider_Width_Scroll(object sender, EventArgs e)
        {
            try
            {
                Byte[] commande;
                imageBoxList[boiteSelectionnee].textLocateParameters[4] = slider_Width.Value;
                commande = Ds2Command("Text Locate \"AllSky_" + (boiteSelectionnee) + "\" " + string.Join(" ", imageBoxList[boiteSelectionnee].textLocateParameters.ToArray()) + " \"");
                envoyerCommande(commande);
                Width_value_txt.Text = slider_Width.Value.ToString();
            }
            catch (Exception) { }
        }
        private void slider_Opacite_Scroll(object sender, EventArgs e)
        {
            Byte[] commande;
            // commande = Ds2Command("Text View \"AllSky_" + imageSelectionnee + "\" 0 " + textLOCATEParamListdeListe[3][6] + " 100 100 100");
            // MessageBox.Show("Text View \"AllSky_" + imageSelectionnee + "\" 0 " + textLOCATEParamListdeListe[3][6] + " 100 100 100");
            // envoyerCommande(commande);
        }
        private void slider_Elevation_Scroll(object sender, EventArgs e)
        {
            try
            {
                Byte[] commande;
                imageBoxList[boiteSelectionnee].textLocateParameters[2] = slider_Elevation.Value;
                commande = Ds2Command("Text Locate \"AllSky_" + (boiteSelectionnee) + "\" " + string.Join(" ", imageBoxList[boiteSelectionnee].textLocateParameters.ToArray()) + " \"");
                envoyerCommande(commande);
                Elevation_value_txt.Text = slider_Elevation.Value.ToString();
            }
            catch (Exception) { }
        }
        private void slider_Azimuth_Scroll(object sender, EventArgs e)
        {
            try
            {
                Byte[] commande;
                imageBoxList[boiteSelectionnee].textLocateParameters[1] = slider_Azimuth.Value;
                commande = Ds2Command("Text Locate \"AllSky_" + (boiteSelectionnee) + "\" " + string.Join(" ", imageBoxList[boiteSelectionnee].textLocateParameters.ToArray()) + " \"");
                envoyerCommande(commande);
                Azimuth_value_txt.Text = slider_Azimuth.Value.ToString();
            }
            catch (Exception) { }
        }
        private void rotationImage(object sender, EventArgs e)
        {
            try
            {
                Byte[] commande;
                imageBoxList[boiteSelectionnee].textLocateParameters[3] = slider_rotation.Value;
                commande = Ds2Command("Text Locate \"AllSky_" + (boiteSelectionnee) + "\" " + string.Join(" ", imageBoxList[boiteSelectionnee].textLocateParameters.ToArray()) + " \"");
                envoyerCommande(commande);
                Rotation_value_txt.Text = slider_rotation.Value.ToString();
            }
            catch (Exception) { }
        }
        private void slider_Height_Scroll_1(object sender, EventArgs e)
        {

        }
        private void Rotation_value_txt_TextChanged(object sender, EventArgs e) // quand on change manuellement la valeur 
        {
            try
            {
                Byte[] commande;
                imageBoxList[boiteSelectionnee].textLocateParameters[3] = Convert.ToInt16(Rotation_value_txt.Text);
                slider_rotation.Value = Convert.ToInt16(Rotation_value_txt.Text);
                commande = Ds2Command("Text Locate \"AllSky_" + (boiteSelectionnee) + "\" " + string.Join(" ", imageBoxList[boiteSelectionnee].textLocateParameters.ToArray()) + " \"");
                envoyerCommande(commande);
                Rotation_value_txt.Text = slider_rotation.Value.ToString();
            }
            catch (Exception) { }
        }
        private void Opacite_value_txt_TextChanged(object sender, EventArgs e)
        {
            int myInt = int.Parse(Rotation_value_txt.Text);
            slider_rotation.Value = myInt;
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

        private void Select_Multi_btn_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Multiselect = true;
            openFileDialog1.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.tif;*.tiff*,.tga...";
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {
                int increment = 0;
                foreach (string files in openFileDialog1.FileNames)
                {
                    imageBoxList[increment].loadImage(files);
                    increment += 1;
                }


            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(719, 429);
            this.Name = "Form1";
            this.ResumeLayout(false);

        }
    }
    public class imageBox
    {

        public string image_Path;
        TextBox chemin = new TextBox();
        PictureBox box = new PictureBox();
        Button envoyer = new Button();
        Panel panneau = new Panel();
        public int boxIndex;
        public List<int> textAddParameters = new List<int> { 0, 0, 0, 90, 0, 0, 1, 1, 1 };  // crée une liste de listes des paramètres de text add 
        public List<int> textLocateParameters = new List<int> { 0, 0, 90, 0, 180, 180 };     // crée une liste de listes des paramètres de text locate /// le dernier paramètre est l'opacite de textview
        // [0] RateTime     [1] Azimuth     [2] Elevation   [3] Rotation  [4] Width    [5] height

        public imageBox(Form Form1, int index)
        {

            panneau.BackgroundImage = DigitalSky_EasyView.Properties.Resources.Panel_Off_2;
            panneau.Size = new Size(166, 218);
            if (index < 5)
            {
                panneau.Location = new Point(80 + (index * 190), 12);
            }
            else
            {
                panneau.Location = new Point(80 + ((index - 5) * 190), 250);
            }
            this.boxIndex = index;
            panneau.Controls.Add(box);
            box.Click += new EventHandler(imageBox_Click);
            box.DoubleClick += new System.EventHandler(imageBox_DoubleClick);
            box.Name = "box_1";
            box.Location = new Point(13, 40);
            box.Size = new Size(140, 140);
            box.BorderStyle = BorderStyle.FixedSingle;
            envoyer.Size = new Size(140, 22);
            envoyer.Location = new Point(13, 185);
            envoyer.BackgroundImage = DigitalSky_EasyView.Properties.Resources.Envoyer_btn;
            envoyer.Text = "";
            envoyer.FlatStyle = FlatStyle.Popup;
            envoyer.Click += new EventHandler(envoyer_Click);
            chemin.Text = "";
            chemin.Size = new Size(140, 22);
            chemin.BackColor = Color.FromArgb(40, 40, 40);
            chemin.ForeColor = Color.White;
            chemin.BorderStyle = BorderStyle.None;
            chemin.Location = new Point(13, 10);
            chemin.Font = new Font("Calibri", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            panneau.Controls.Add(chemin);
            panneau.Controls.Add(envoyer);
            Form1.Controls.Add(panneau);

        }

        private void imageBox_Click(object sender, EventArgs e)
        {
            if (image_Path != null)
            {
                foreach (imageBox boite in DigitalSky_EasyView.Form1.imageBoxList)
                {
                    boite.panneau.BackgroundImage = DigitalSky_EasyView.Properties.Resources.Panel_Off_2;
                }
                panneau.BackgroundImage = DigitalSky_EasyView.Properties.Resources.Panel_On_2;
                DigitalSky_EasyView.Form1.boiteSelectionnee = boxIndex;
                Form1.slider_Azimuth.Value = textLocateParameters[1];
                Form1.slider_Elevation.Value = textLocateParameters[2];
                Form1.slider_rotation.Value = textLocateParameters[3];
                Form1.slider_Width.Value = textLocateParameters[4];
                Form1.slider_Height.Value = textLocateParameters[5];

            }

        }
        private void imageBox_DoubleClick(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.tif;*.tiff*,.tga...";
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {
                loadImage(openFileDialog1.FileName);

            }
        }

        public void loadImage(string filename)
        {
            box.Image = Image.FromFile(filename);
            box.SizeMode = PictureBoxSizeMode.Zoom;
            FileInfo fi2 = new FileInfo(filename);
            //image_Path = UncPath(fi2);
            image_Path = GetUNCByDrive(filename);
            chemin.Text = image_Path;
            foreach (imageBox boite in DigitalSky_EasyView.Form1.imageBoxList)
            {
                boite.panneau.BackgroundImage = DigitalSky_EasyView.Properties.Resources.Panel_Off_2;
            }
            panneau.BackgroundImage = DigitalSky_EasyView.Properties.Resources.Panel_On_2;
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


        public static string UncPath(FileInfo fileInfo)
        {
            string filePath = fileInfo.FullName;

            //if (filePath.StartsWith(@"\\"))
            //return filePath;

            //if (new DriveInfo(Path.GetPathRoot(filePath)).DriveType != DriveType.Network)
            //return filePath;

            string drivePrefix = Path.GetPathRoot(filePath).Substring(0, 2);
            //MessageBox.Show("drivePrefix = " +drivePrefix);
            string uncRoot;

            using (var managementObject = new ManagementObject())
            {

                var managementPath = string.Format("Win32_LogicalDisk='{0}'", drivePrefix);
                //MessageBox.Show("Si on se rend ici, c'est que le ManagementObject a été créé");
                managementObject.Path = new ManagementPath(managementPath);
                uncRoot = (string)managementObject["ProviderName"];
                //MessageBox.Show("unRoot = " + uncRoot) ;
            }

            return filePath.Replace(drivePrefix, uncRoot);
        }

        private void envoyer_Click(object sender, EventArgs e)
        {
            Byte[] commande;
            //fadeOutToutLeMonde();
            commande = Ds2Command("Text Add \"AllSky_" + boxIndex + "\" \"" + image_Path + "\"" + string.Join(" ", textAddParameters.ToArray()) + " \"");
            envoyerCommande(commande);
            Thread.Sleep(100);
            commande = Ds2Command("Text Locate \"AllSky_" + boxIndex + "\" " + string.Join(" ", textLocateParameters.ToArray()) + " \"");
            envoyerCommande(commande);
            Thread.Sleep(50);
            commande = Ds2Command("Text View \"AllSky_" + boxIndex + "\" 0 100 100 100 100");
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
                UdpClient udpClient = new UdpClient();
                //udpClient.Send(commande, commande.Length, "192.168.0.100", 2209);
                udpClient.Send(commande, commande.Length, "127.0.0.1", 2209);
            }
            catch (Exception)
            {

            }
        }

    }

}