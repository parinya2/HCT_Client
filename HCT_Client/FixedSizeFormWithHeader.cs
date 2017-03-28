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
    public partial class FixedSizeFormWithHeader : FixedSizeForm
    {
        private Panel headerBackgroundPanel;
        public Label headerLineLabel;
        private Label headerTextLabel1;
        private Label headerTextLabel2;
        private PictureBox dltLogoPictureBox;
        private Label simulatorTextLabel;        
        int headerLineGap = 20;

        public FixedSizeFormWithHeader()
        {
            InitializeComponent();

            Color bgColor = GlobalColor.purpleColor;
            bool isSimulatorMode = WebServiceManager.webServiceMode == WebServiceMode.SimulatorMode;

            dltLogoPictureBox = new PictureBox();
            dltLogoPictureBox.Width = 150;
            dltLogoPictureBox.Height = 150;
            dltLogoPictureBox.Location = new Point(70, 30);
            dltLogoPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            //TODO: Temporary
            dltLogoPictureBox.Image = null; //Util.GetImageFromImageResources("DLT_Logo.png");

            headerLineLabel = new Label();
            headerLineLabel.BackColor = Color.Black;
            headerLineLabel.Width = SCREEN_WIDTH - headerLineGap * 2;
            headerLineLabel.Height = 3;
            headerLineLabel.Location = new Point(headerLineGap, dltLogoPictureBox.Location.Y + dltLogoPictureBox.Height + 10);

            headerTextLabel1 = new Label();
            headerTextLabel1.ForeColor = Color.White;
            headerTextLabel1.BackColor = bgColor;
            headerTextLabel1.Width = 700;
            headerTextLabel1.Height = isSimulatorMode ? (int)(dltLogoPictureBox.Height * 0.7) : dltLogoPictureBox.Height;
            headerTextLabel1.TextAlign = ContentAlignment.MiddleLeft;
            headerTextLabel1.Font = new Font(this.Font.FontFamily, 28);
            headerTextLabel1.Text = "โรงเรียนสอนขับรถ หาดใหญ่ คาร์ เทรนเนอร์ |";
            headerTextLabel1.Location = new Point(dltLogoPictureBox.Location.X + dltLogoPictureBox.Width + headerLineGap, 
                                                 dltLogoPictureBox.Location.Y);

            headerTextLabel2 = new Label();
            headerTextLabel2.ForeColor = GlobalColor.orangeColor;
            headerTextLabel2.BackColor = bgColor;
            headerTextLabel2.Width = 450;
            headerTextLabel2.Height = headerTextLabel1.Height;
            headerTextLabel2.TextAlign = ContentAlignment.MiddleLeft;
            headerTextLabel2.Font = headerTextLabel1.Font;
            headerTextLabel2.Text = " Electronics Exam";
            headerTextLabel2.Location = new Point(headerTextLabel1.Location.X + headerTextLabel1.Width,
                                                 headerTextLabel1.Location.Y);

            headerBackgroundPanel = new Panel();
            headerBackgroundPanel.Width = SCREEN_WIDTH;
            headerBackgroundPanel.Height = dltLogoPictureBox.Location.Y + dltLogoPictureBox.Height + 10;
            headerBackgroundPanel.BackColor = bgColor;           

            if (isSimulatorMode)
            {
                simulatorTextLabel = new Label();
                simulatorTextLabel.ForeColor = GlobalColor.yellowColor;
                simulatorTextLabel.BackColor = bgColor;
                simulatorTextLabel.Width = SCREEN_WIDTH - dltLogoPictureBox.Width - dltLogoPictureBox.Location.X - headerLineGap * 3;
                simulatorTextLabel.Height = dltLogoPictureBox.Height - headerTextLabel1.Height;
                simulatorTextLabel.TextAlign = ContentAlignment.MiddleLeft;
                simulatorTextLabel.Font = new Font(this.Font.FontFamily, 20);
                simulatorTextLabel.Text = "โปรแกรมนี้เป็นแบบจำลองการทำข้อสอบเท่านั้น ไม่ใช่การทำข้อสอบจริง";
                simulatorTextLabel.Location = new Point(dltLogoPictureBox.Location.X + dltLogoPictureBox.Width + headerLineGap,
                                                     headerTextLabel1.Location.Y + headerTextLabel1.Height + 5);
                this.Controls.Add(simulatorTextLabel);
            }

            this.Controls.Add(headerTextLabel1);
            this.Controls.Add(headerTextLabel2);
            //this.Controls.Add(dltLogoPictureBox);
            this.Controls.Add(headerBackgroundPanel);
            //this.Controls.Add(headerLineLabel);
        }       
    }
}
