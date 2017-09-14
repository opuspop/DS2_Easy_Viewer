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
          
            
        }
    }
}
