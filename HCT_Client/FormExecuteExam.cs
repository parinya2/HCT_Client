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
    public partial class FormExecuteExam : FixedSizeForm
    {
        Panel monitorPanel;
        Panel examPanel;
        BaseTextLabel usernameLabel;
        BasePhotoLabel photoLabel;
        BaseTextLabel timerLabel;
        Stopwatch stopwatch;

        public FormExecuteExam()
        {
            InitializeComponent();
            RenderUI();
        }

        void RenderUI()
        {
            monitorPanel = new Panel();
            monitorPanel.Width = (int)(SCREEN_WIDTH * 0.3);
            monitorPanel.Height = SCREEN_HEIGHT;
            monitorPanel.BackColor = Color.Black;
            monitorPanel.Location = new Point(0, 0);

            photoLabel = new BasePhotoLabel();
            photoLabel.Location = new Point(40, 40);

            usernameLabel = new BaseTextLabel();
            usernameLabel.Text = "ดัสกร ทองเหลา";
            usernameLabel.Location = new Point(photoLabel.Location.X + photoLabel.Width + 20 + 10, 
                                               photoLabel.Location.Y);

            timerLabel = new BaseTextLabel();
            timerLabel.Text = "";
            timerLabel.Location = new Point(photoLabel.Location.X, 
                                            photoLabel.Location.Y + photoLabel.Height + 20);
            timerLabel.Width = monitorPanel.Width - (photoLabel.Location.X * 2);

            monitorPanel.Controls.Add(photoLabel);
            monitorPanel.Controls.Add(usernameLabel);
            monitorPanel.Controls.Add(timerLabel);

            this.Controls.Add(monitorPanel);

            examPanel = new Panel();
            examPanel.Width = SCREEN_WIDTH - monitorPanel.Width;
            examPanel.Height = SCREEN_HEIGHT;
            examPanel.BackColor = Color.LightGray;
            examPanel.Location = new Point(monitorPanel.Width, 0);
            this.Controls.Add(examPanel);

            stopwatch = new Stopwatch(100);
            stopwatch.TheTimeChanged += new Stopwatch.TimerTickHandler(TimeHasChanged);
        }

        protected void TimeHasChanged(string newTime)
        {
            timerLabel.Text = newTime;
        }
    }
}
