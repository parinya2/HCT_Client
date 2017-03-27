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
        private Label headerTextLabel;
        private PictureBox dltLogoPictureBox;
        private Label simulatorTextLabel;        
        int headerLineGap = 20;

        public FixedSizeFormWithHeader()
        {
            InitializeComponent();

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

            headerTextLabel = new Label();
            headerTextLabel.ForeColor = Color.White;
            headerTextLabel.BackColor = GlobalColor.purpleColor;
            headerTextLabel.Width = SCREEN_WIDTH - dltLogoPictureBox.Width - dltLogoPictureBox.Location.X - headerLineGap * 3;
            headerTextLabel.Height = isSimulatorMode ? (int)(dltLogoPictureBox.Height * 0.7) : dltLogoPictureBox.Height;
            headerTextLabel.TextAlign = ContentAlignment.MiddleLeft;
            headerTextLabel.Font = new Font(this.Font.FontFamily, 22);
            headerTextLabel.Text = "โรงเรียนสอนขับรถ หาดใหญ่ คาร์ เทรนเนอร์ | Electronics Exam";
            headerTextLabel.Location = new Point(dltLogoPictureBox.Location.X + dltLogoPictureBox.Width + headerLineGap, 
                                                 dltLogoPictureBox.Location.Y);

            headerBackgroundPanel = new Panel();
            headerBackgroundPanel.Width = SCREEN_WIDTH;
            headerBackgroundPanel.Height = dltLogoPictureBox.Location.Y + dltLogoPictureBox.Height + 10;
            headerBackgroundPanel.BackColor = GlobalColor.purpleColor;
            
            this.Controls.Add(headerTextLabel);
            //this.Controls.Add(dltLogoPictureBox);
            this.Controls.Add(headerBackgroundPanel);
            //this.Controls.Add(headerLineLabel);

            if (isSimulatorMode)
            {
                simulatorTextLabel = new Label();
                simulatorTextLabel.ForeColor = Color.Red;
                simulatorTextLabel.Width = SCREEN_WIDTH - dltLogoPictureBox.Width - dltLogoPictureBox.Location.X - headerLineGap * 3;
                simulatorTextLabel.Height = dltLogoPictureBox.Height - headerTextLabel.Height;
                simulatorTextLabel.TextAlign = ContentAlignment.MiddleLeft;
                simulatorTextLabel.Font = new Font(this.Font.FontFamily, 14);
                simulatorTextLabel.Text = "โปรแกรมนี้เป็นแบบจำลองการทำข้อสอบเท่านั้น ไม่ใช่การทำข้อสอบจริง";
                simulatorTextLabel.Location = new Point(dltLogoPictureBox.Location.X + dltLogoPictureBox.Width + headerLineGap,
                                                     headerTextLabel.Location.Y + headerTextLabel.Height + 5);
                this.Controls.Add(simulatorTextLabel);
            }
        }

       
    }
}
