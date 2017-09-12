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

        public Form1()
        {
            //imageBox box = new imageBox(this, 1);
            //imageBoxList.Add(box);
        }
    }
     
    public class imageBox
    {
        public string image_Path;
        TextBox chemin = new TextBox();
        PictureBox box = new PictureBox();
        Button envoyer = new Button();
        Panel panneauParametres = new System.Windows.Forms.Panel(); Panel panneauRotation = new System.Windows.Forms.Panel(); Panel panneauAzimuth = new System.Windows.Forms.Panel();
        Panel panneauElevation = new System.Windows.Forms.Panel(); Panel panneauWidth = new System.Windows.Forms.Panel(); Panel panneauHeight = new System.Windows.Forms.Panel();
        Panel panneauImage = new Panel();
        public int boxIndex;
        public List<int> textAddParameters = new List<int> { 0, 0, 0, 90, 0, 0, 1, 1, 1 };  // crée une liste de listes des paramètres de text add 
        public List<int> textLocateParameters = new List<int> { 0, 0, 90, 0, 180, 180 };     // crée une liste de listes des paramètres de text locate /// le dernier paramètre est l'opacite de textview
        // [0] RateTime     [1] Azimuth     [2] Elevation   [3] Rotation  [4] Width    [5] height

        public imageBox(Form Form1, int index)
        {
            panneauParametres.BackgroundImage = global::DS2_Easy_Viewer.Properties.Resources.Params_Outline_2;
            panneauParametres.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            //panneauParametres.Controls.Add(this.panel2);
            //panneauParametres.Controls.Add(this.Rotation_Panel);
            panneauParametres.Location = new System.Drawing.Point(909, 12);
            panneauParametres.Name = "panel1";
            panneauParametres.Size = new System.Drawing.Size(246, 560);
            panneauParametres.TabIndex = 0;
            Form1.Controls.Add(panneauParametres);
        }

    }
}