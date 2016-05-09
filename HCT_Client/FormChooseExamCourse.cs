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
    public class SingleExamCoursePanel : Panel
    {
        public PictureBox examCourseIconPictureBox;
        public BaseTextLabel examCourseTextLabel;

        public SingleExamCoursePanel(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            RenderUI();
        }

        void RenderUI()
        {
            int margin = 30;           

            examCourseIconPictureBox = new PictureBox();
            examCourseIconPictureBox.Location = new Point(margin, margin);
            examCourseIconPictureBox.Width = this.Height - margin * 2;
            examCourseIconPictureBox.Height = examCourseIconPictureBox.Width;
            examCourseIconPictureBox.BackColor = Color.Transparent;

            examCourseTextLabel = new BaseTextLabel();
            examCourseTextLabel.Location = new Point(examCourseIconPictureBox.Location.X + examCourseIconPictureBox.Width,
                                                     examCourseIconPictureBox.Location.Y);
            examCourseTextLabel.Width = this.Width - examCourseIconPictureBox.Width - margin * 2;
            examCourseTextLabel.Height = examCourseIconPictureBox.Height;
            examCourseTextLabel.BackColor = Color.Transparent;
            examCourseTextLabel.TextAlign = ContentAlignment.MiddleCenter;
            examCourseTextLabel.ForeColor = Color.Black;
            examCourseTextLabel.Font = new Font(this.Font.FontFamily, 24);

            this.Controls.Add(examCourseIconPictureBox);
            this.Controls.Add(examCourseTextLabel);
        }
    }

    public partial class FormChooseExamCourse : FixedSizeForm
    {
        public SingleExamCoursePanel examCourseType1panel;
        public SingleExamCoursePanel examCourseType2panel;
        public BaseTextLabel examCourseTopicLabel;
        private BlinkButtonSignalClock blinkButtonSignalClock;

        public FormChooseExamCourse()
        {
            InitializeComponent();

            RenderUI();

            blinkButtonSignalClock = new BlinkButtonSignalClock();
            blinkButtonSignalClock.TheTimeChanged += new BlinkButtonSignalClock.BlinkButtonSignalClockTickHandler(BlinkButtonSignalClockHasChanged);
        }

        private void RenderUI()
        {
            examCourseTopicLabel = new BaseTextLabel();
            examCourseTopicLabel.Width = SCREEN_WIDTH;
            examCourseTopicLabel.Height = 150;
            examCourseTopicLabel.TextAlign = ContentAlignment.MiddleCenter;
            examCourseTopicLabel.ForeColor = Color.Black;
            examCourseTopicLabel.Font = new Font(this.Font.FontFamily, 26);
            examCourseTopicLabel.Location = new Point(0, 60);

            int buttonGapX = 240;
            int buttonGapY = 30;

            int width = SCREEN_WIDTH - buttonGapX * 2;
            int height = 160;
            examCourseType1panel = new SingleExamCoursePanel(width, height);
            examCourseType1panel.Location = new Point(buttonGapX, 
                                                        examCourseTopicLabel.Location.Y + examCourseTopicLabel.Height + buttonGapY * 2);
            examCourseType1panel.Click += new EventHandler(ExamCourseType1ButtonClicked);
            examCourseType1panel.examCourseIconPictureBox.Click += new EventHandler(ExamCourseType1ButtonClicked);
            examCourseType1panel.examCourseTextLabel.Click += new EventHandler(ExamCourseType1ButtonClicked);

            examCourseType2panel = new SingleExamCoursePanel(width, height);
            examCourseType2panel.Width = examCourseType1panel.Width;
            examCourseType2panel.Height = examCourseType1panel.Height;
            examCourseType2panel.Location = new Point(examCourseType1panel.Location.X,
                                                       examCourseType1panel.Location.Y + examCourseType1panel.Height + buttonGapY);
            examCourseType2panel.Click += new EventHandler(ExamCourseType2ButtonClicked);
            examCourseType2panel.examCourseIconPictureBox.Click += new EventHandler(ExamCourseType2ButtonClicked);
            examCourseType2panel.examCourseTextLabel.Click += new EventHandler(ExamCourseType2ButtonClicked);

            System.Reflection.Assembly myAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            Stream myStream = myAssembly.GetManifestResourceStream("HCT_Client.CarIcon.png");
            Bitmap carBitmap = new Bitmap(myStream);
            examCourseType1panel.examCourseIconPictureBox.Image = carBitmap;
            examCourseType1panel.examCourseIconPictureBox.SizeMode = PictureBoxSizeMode.Zoom;

            myStream = myAssembly.GetManifestResourceStream("HCT_Client.MotorcycleIcon.png");
            Bitmap motocycleBitmap = new Bitmap(myStream);
            examCourseType2panel.examCourseIconPictureBox.Image = motocycleBitmap;
            examCourseType2panel.examCourseIconPictureBox.SizeMode = PictureBoxSizeMode.Zoom;


            this.Controls.Add(examCourseTopicLabel);
            this.Controls.Add(examCourseType1panel);
            this.Controls.Add(examCourseType2panel);

            RefreshUI();
        }

        public void RefreshUI()
        {
            examCourseTopicLabel.Text = LocalizedTextManager.GetLocalizedTextForKey("FormChooseExamCourse.Topic");
            examCourseType1panel.examCourseTextLabel.Text = LocalizedTextManager.GetLocalizedTextForKey("FormChooseExamCourse.Button.1");
            examCourseType2panel.examCourseTextLabel.Text = LocalizedTextManager.GetLocalizedTextForKey("FormChooseExamCourse.Button.2");
        }

        void ExamCourseType1ButtonClicked(object sender, EventArgs e)
        {
            GoToNextForm();
        }

        void ExamCourseType2ButtonClicked(object sender, EventArgs e)
        {
            GoToNextForm();
        }

        void GoToNextForm()
        {
            FormExecuteExam instanceFormExecuteExam = FormsManager.GetFormExecuteExam();
            instanceFormExecuteExam.LoadExamData();
            instanceFormExecuteExam.Visible = true;
            instanceFormExecuteExam.Enabled = true;
            instanceFormExecuteExam.RefreshUI();
            instanceFormExecuteExam.BringToFront();
            this.Visible = false;
        }

        protected void BlinkButtonSignalClockHasChanged(int state)
        {
            examCourseType1panel.BackColor = Util.GetButtonBlinkColorAtSignalState(state);
            examCourseType2panel.BackColor = Util.GetButtonBlinkColorAtSignalState(state);
        }
    }
}
