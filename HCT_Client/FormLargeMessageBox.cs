using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HCT_Client
{
    public partial class FormLargeMessageBox : Form
    {
        public Button leftButton;
        public Button rightButton;
        int mode;

        public FormLargeMessageBox(int mode)
        {
            InitializeComponent();

            int SCREEN_WIDTH = SystemInformation.VirtualScreen.Width;
            int SCREEN_HEIGHT = SystemInformation.VirtualScreen.Height;

            this.mode = mode;
            this.Width = 600;
            this.Height = 300;

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

            RenderUI();
        }

        void RenderUI()
        {
            int buttonOffsetX = 30;
            int buttonOffsetY = 30;

            leftButton = new Button();
            leftButton.Width = 150;
            leftButton.Height = 80;
            leftButton.Location = new Point(leftButton.Width + buttonOffsetX,
                                 this.Height - leftButton.Height - buttonOffsetY);
            
            rightButton = new Button();
            rightButton.Width = 150;
            rightButton.Height = 80;
            rightButton.Location = new Point(this.Width - rightButton.Width - buttonOffsetX,
                                             this.Height - rightButton.Height - buttonOffsetY);

            switch (this.mode)
            { 
                case 0 :
                    leftButton.Visible = false;
                    rightButton.Visible = true;
                    break;
                case 1 :
                    leftButton.Visible = true;
                    rightButton.Visible = true;
                break;
            }

            this.Controls.Add(leftButton);
            this.Controls.Add(rightButton);
        }
    }
}
