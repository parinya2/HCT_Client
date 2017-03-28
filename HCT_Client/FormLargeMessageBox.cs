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
    public enum MessageBoxIcon
    { 
        Null,
        WarningSign,
        CameraIcon
    }

    public partial class FormLargeMessageBox : Form
    {
        public string errorCode;
        public Form callerForm;
        public Button leftButton;
        public Button rightButton;
        public Label messageLabel;
        public PictureBox iconPictureBox;
        public MessageBoxIcon iconType;
        int mode;

        public FormLargeMessageBox(int mode, MessageBoxIcon icon)
        {
            InitializeComponent();

            int SCREEN_WIDTH = SystemInformation.VirtualScreen.Width;
            int SCREEN_HEIGHT = SystemInformation.VirtualScreen.Height;

            this.mode = mode;
            this.Width = 770;
            this.Height = 460;
            this.iconType = icon;


            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

            RenderUI();
        }

        void RenderUI()
        {
            iconPictureBox = new PictureBox();

            if (iconType == MessageBoxIcon.Null)
            {
                iconPictureBox.Image = null;
                iconPictureBox.Width = 1;
                iconPictureBox.Height = 1;
            }
            else
            {
                iconPictureBox.Width = 250;
                iconPictureBox.Height = 120;
                string iconName = "";
                if (iconType == MessageBoxIcon.WarningSign) iconName = "WarningSign.png";
                if (iconType == MessageBoxIcon.CameraIcon) iconName = "CameraIcon.png";
                Bitmap iconBitmap = Util.GetImageFromImageResources(iconName);
                iconPictureBox.Image = iconBitmap;
            }
            iconPictureBox.Location = new Point((this.Width - iconPictureBox.Width) / 2, 30);
            iconPictureBox.BackColor = Color.Transparent;
            iconPictureBox.SizeMode = PictureBoxSizeMode.Zoom;

            int buttonOffsetX = 30;
            int buttonOffsetY = 30;

            leftButton = new Button();
            leftButton.Width = 200;
            leftButton.Height = 100;
            leftButton.Font = new Font(this.Font.FontFamily, 26);
            leftButton.ForeColor = Color.White;
            leftButton.BackColor = GlobalColor.redColor;

            rightButton = new Button();
            rightButton.Width = leftButton.Width;
            rightButton.Height = leftButton.Height;
            rightButton.Font = new Font(this.Font.FontFamily, 26);
            rightButton.ForeColor = Color.White;
            rightButton.BackColor = GlobalColor.redColor;

            switch (this.mode)
            { 
                case 0 :
                    leftButton.Visible = false;
                    rightButton.Visible = true;
                    rightButton.Location = new Point(this.Width - rightButton.Width - buttonOffsetX,
                                this.Height - rightButton.Height - buttonOffsetY);
                    break;
                case 1 :
                    leftButton.Visible = true;
                    rightButton.Visible = true;

                    rightButton.Location = new Point(this.Width / 2 + buttonOffsetX,
                                                    this.Height - rightButton.Height - buttonOffsetY);
                    leftButton.Location = new Point(this.Width / 2 - buttonOffsetX - leftButton.Width,
                                                    rightButton.Location.Y);
                    break;
                case -1 :
                    leftButton.Visible = false;
                    rightButton.Visible = false;
                    break;
            }

            int messageLabelOffsetX = 50;
            int messageLabelOffsetY = 30;

            messageLabel = new Label();
            messageLabel.Width = this.Width - (messageLabelOffsetX * 2);
            messageLabel.Height = this.Height - iconPictureBox.Height - rightButton.Height - messageLabelOffsetY * 2;
            messageLabel.Location = new Point(messageLabelOffsetX, iconPictureBox.Location.Y + iconPictureBox.Height);
            messageLabel.Font = new Font(this.Font.FontFamily, 24);

            messageLabel.TextAlign = ContentAlignment.MiddleCenter;

            this.Controls.Add(messageLabel);
            this.Controls.Add(leftButton);
            this.Controls.Add(rightButton);
            this.Controls.Add(iconPictureBox);
        }

        public void ShowMessageBoxAtLocation(Point location)
        {
            FormsManager.GetFormFadeView().Visible = true;
            this.BringToFront();
            this.Visible = true;
            this.Location = location;  
        }    
    }
}
