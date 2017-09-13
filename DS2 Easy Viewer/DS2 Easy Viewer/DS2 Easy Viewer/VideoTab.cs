using System;
using System.Windows.Forms;
using Microsoft.DirectX.AudioVideoPlayback;

namespace DS2_Easy_Viewer
{
    public partial class VideoTab : Form
    {
        public static VideoTab Instance { get; set; }

        private Video video;
        private string videoPath = "";


        public VideoTab()
        {
            InitializeComponent();

        }

        private void GotToPhotoTab_btn_Click(object sender, EventArgs e)
        {
            if (Form1.Instance == null)//Check if Form1 has already been created
            {
                //if not: go create a new one !
                Form1.Instance = new Form1();
            }
            //Instance of Form1 is already created => open that one            
            Form1.Instance.Show();
            this.Hide();
        }
    }
}
