using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace HCT_Client
{
    public partial class FormLoadingView : FormFadeView
    {
        PictureBox loadingPictureBox;
        public FormLoadingView()
        {
            InitializeComponent();

            this.Enabled = true;
            this.Opacity = 0.7;
            this.BackColor = Color.Black;

            RenderUI();
        }

        private void RenderUI()
        {
            loadingPictureBox = new PictureBox();
            loadingPictureBox.Width = 150;
            loadingPictureBox.Height = loadingPictureBox.Width;
            loadingPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            loadingPictureBox.Location = new Point((SCREEN_WIDTH - loadingPictureBox.Width) / 2 ,
                                                    (SCREEN_HEIGHT - loadingPictureBox.Height) / 2);

            System.Reflection.Assembly myAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            string path = "HCT_Client.Images.DLT_Loading.gif";
            Stream myStream = myAssembly.GetManifestResourceStream(path);
            Image gifImage = Image.FromStream(myStream);

            loadingPictureBox.Image = gifImage;

            this.Controls.Add(loadingPictureBox);
        }

        public void ShowLoadingView(bool flag)
        {
            this.Visible = flag;
            if (flag)
            {
                this.BringToFront();
            }
                
        }
    }
}
