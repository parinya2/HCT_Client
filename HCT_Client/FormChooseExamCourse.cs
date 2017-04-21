using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Drawing2D;
using System.Threading;

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
            int margin = 10;           

            examCourseIconPictureBox = new PictureBox();
            examCourseIconPictureBox.Location = new Point(margin, margin);
            examCourseIconPictureBox.Width = this.Height - margin * 2;
            examCourseIconPictureBox.Height = examCourseIconPictureBox.Width;
            examCourseIconPictureBox.BackColor = Color.White;

            GraphicsPath gp = new GraphicsPath();
            gp.AddEllipse(0, 0, examCourseIconPictureBox.Width - 3, examCourseIconPictureBox.Height - 3);
            Region rg = new Region(gp);
            examCourseIconPictureBox.Region = rg;


            examCourseTextLabel = new BaseTextLabel();
            examCourseTextLabel.Location = new Point(examCourseIconPictureBox.Location.X + examCourseIconPictureBox.Width,
                                                     examCourseIconPictureBox.Location.Y);
            examCourseTextLabel.Width = this.Width - examCourseIconPictureBox.Width - margin * 2;
            examCourseTextLabel.Height = examCourseIconPictureBox.Height;
            examCourseTextLabel.BackColor = Color.Transparent;
            examCourseTextLabel.TextAlign = ContentAlignment.MiddleCenter;
            examCourseTextLabel.ForeColor = Color.Black;
            examCourseTextLabel.Font = UtilFonts.GetTHSarabunFont(24);

            this.Controls.Add(examCourseIconPictureBox);
            this.Controls.Add(examCourseTextLabel);
        }
    }

    public partial class FormChooseExamCourse : FixedSizeFormWithHeader
    {
        public SingleExamCoursePanel examCourseType1panel;
        public SingleExamCoursePanel examCourseType2panel;
        public BaseTextLabel examCourseTopicLabel;
        private BlinkButtonSignalClock blinkButtonSignalClock;
        public Button backButton;

        public FormChooseExamCourse()
        {
            InitializeComponent();
            RenderUI();

            blinkButtonSignalClock = new BlinkButtonSignalClock();
            blinkButtonSignalClock.TheTimeChanged += new BlinkButtonSignalClock.BlinkButtonSignalClockTickHandler(BlinkButtonSignalClockHasChanged);
        }

        private void RenderUI()
        {
            backButton = new MediumButton();
            backButton.Location = new Point(SCREEN_WIDTH - backButton.Width - 50,
                                            SCREEN_HEIGHT - backButton.Height - 50);
            backButton.Click += new EventHandler(BackButtonClicked);
            backButton.ForeColor = Color.White;
            backButton.BackColor = GlobalColor.redColor;

            examCourseTopicLabel = new BaseTextLabel();
            examCourseTopicLabel.Width = SCREEN_WIDTH;
            examCourseTopicLabel.TextAlign = ContentAlignment.MiddleCenter;
            examCourseTopicLabel.ForeColor = Color.Black;
            examCourseTopicLabel.Location = new Point(0, headerLineLabel.Location.Y + 60);

            int buttonGapX = 240;
            int buttonGapY = 30;

            int width = SCREEN_WIDTH - buttonGapX * 2;
            int height = 160;
            examCourseType1panel = new SingleExamCoursePanel(width, height);
            examCourseType1panel.Location = new Point(buttonGapX, 
                                                        examCourseTopicLabel.Location.Y + examCourseTopicLabel.Height + buttonGapY);
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

            Bitmap carBitmap = Util.GetImageFromImageResources("CarIcon.png");
            examCourseType1panel.examCourseIconPictureBox.Image = carBitmap;
            examCourseType1panel.examCourseIconPictureBox.SizeMode = PictureBoxSizeMode.Zoom;

            Bitmap motocycleBitmap = Util.GetImageFromImageResources("MotorcycleIcon.png");
            examCourseType2panel.examCourseIconPictureBox.Image = motocycleBitmap;
            examCourseType2panel.examCourseIconPictureBox.SizeMode = PictureBoxSizeMode.Zoom;

            this.Controls.Add(backButton);
            this.Controls.Add(examCourseTopicLabel);
            this.Controls.Add(examCourseType1panel);
            this.Controls.Add(examCourseType2panel);

            RefreshUI();
        }

        public void RefreshUI()
        {
            backButton.Text = LocalizedTextManager.GetLocalizedTextForKey("FormChooseExamCourse.Button.GoBack");
            examCourseTopicLabel.Text = LocalizedTextManager.GetLocalizedTextForKey("FormChooseExamCourse.Topic");
            examCourseType1panel.examCourseTextLabel.Text = LocalizedTextManager.GetLocalizedTextForKey("FormChooseExamCourse.Button." + ExamCourseType.Car);
            examCourseType2panel.examCourseTextLabel.Text = LocalizedTextManager.GetLocalizedTextForKey("FormChooseExamCourse.Button." + ExamCourseType.Motorcycle);
        }

        void ExamCourseType1ButtonClicked(object sender, EventArgs e)
        {
            QuizManager.SetExamCourseType(ExamCourseType.Car);
            GoToNextForm();
        }

        void ExamCourseType2ButtonClicked(object sender, EventArgs e)
        {
            QuizManager.SetExamCourseType(ExamCourseType.Motorcycle);            
            GoToNextForm();
        }

        public void GoToNextForm()
        {
            string JSONstr = "" + UserProfileManager.GetStudentEnrolDetailJSON();
            bool foundStudentEnrol= false;
            if (JSONstr.Length > 4)
            {
                string[] tmpArr = new string[2];
                JSONstr = JSONstr.Replace("\"","");
                int a1 = JSONstr.IndexOf("{");
                int a2 = JSONstr.IndexOf("}");
                tmpArr[0] = JSONstr.Substring(a1 + 1, a2 - a1 - 1);

                JSONstr = JSONstr.Substring(a2 + 1);
                int b1 = JSONstr.IndexOf("{");
                int b2 = JSONstr.IndexOf("}");
                tmpArr[1] = (b1 > -1 && b2 > -1) ? JSONstr.Substring(b1 + 1, b2 - b1 - 1) : "";

                ExamCourseType examCourseType = QuizManager.GetExamCourseType();
                string targetJSONStr = "";
                for (int i = 0; i < tmpArr.Length; i++)
                {
                    if ((tmpArr[i].Contains("CourseType:1") && examCourseType == ExamCourseType.Car) ||
                        (tmpArr[i].Contains("CourseType:2") && examCourseType == ExamCourseType.Motorcycle))
                    {
                        targetJSONStr = tmpArr[i];
                        foundStudentEnrol = true;
                    }
                }

                string[] tmpArr2 = targetJSONStr.Split(',');
                for (int i = 0; i < tmpArr2.Length; i++)
                {
                    string tmpStr = tmpArr2[i];
                    int idx = tmpStr.IndexOf(":");
                    if (idx == -1) continue;
                    string value = tmpStr.Substring(idx + 1);
                    if (tmpStr.Contains("EnrolDate"))
                    {
                        try
                        {
                            string[] dateArr = value.Split('/');
                            int day = Int32.Parse(dateArr[0]);
                            int month = Int32.Parse(dateArr[1]);
                            int year = Int32.Parse(dateArr[2]);
                            year = (year < 2500) ? year + 543 : year;

                            string dayStr = (day < 10) ? "0" + day : "" + day;
                            string monthStr = (month < 10) ? "0" + month : "" + month;
                            string yearStr = year + "";
                            string dateStr = dayStr + "/" + monthStr + "/" + yearStr;

                            UserProfileManager.SetCourseRegisterDate(dateStr);
                        }
                        catch (Exception e)
                        {
                            foundStudentEnrol = false;
                        }
                    }
                    else if (tmpStr.Contains("ExamCount"))
                    {
                        try
                        {
                            int examCount = Int32.Parse(value);
                            UserProfileManager.SetExamSeq((examCount + 1) + "");
                        }
                        catch (Exception e)
                        {
                            foundStudentEnrol = false;
                        }
                    }
                }
            }

            if (!foundStudentEnrol && WebServiceManager.webServiceMode == WebServiceMode.NormalMode)
            {
                FormLargeMessageBox errorFormMessageBox = FormsManager.GetFormErrorMessageBox(WebServiceResultStatus.ERROR_STUDENT_ENROL_NOT_FOUND, this);
                Point centerPoint = new Point((SCREEN_WIDTH - errorFormMessageBox.Width) / 2,
                                              (SCREEN_HEIGHT - errorFormMessageBox.Height) / 2);
                errorFormMessageBox.ShowMessageBoxAtLocation(centerPoint);
                return;
            }

            FormsManager.GetFormLoadingView().ShowLoadingView(true);
            BackgroundWorker bw = new BackgroundWorker();
            string status = "";
            bw.DoWork += new DoWorkEventHandler(
                delegate(object o, DoWorkEventArgs args)
                {
                    Thread.Sleep(10);
                    status = WebServiceManager.GetPaperTestNumberFromServer();
                }
             );
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(
                delegate(object o, RunWorkerCompletedEventArgs args)
                {
                    FormsManager.GetFormLoadingView().ShowLoadingView(false);
                    if (status.Equals(WebServiceResultStatus.SUCCESS))
                    {
                        FormShowUserDetail instanceFormShowUserDetail = FormsManager.GetFormShowUserDetail();
                        instanceFormShowUserDetail.Visible = true;
                        instanceFormShowUserDetail.Enabled = true;
                        instanceFormShowUserDetail.RefreshUI();
                        instanceFormShowUserDetail.BringToFront();
                        this.Visible = false;
                    }
                    else
                    {
                        FormLargeMessageBox errorFormMessageBox = FormsManager.GetFormErrorMessageBox(status, this);
                        Point centerPoint = new Point((SCREEN_WIDTH - errorFormMessageBox.Width) / 2,
                                                      (SCREEN_HEIGHT - errorFormMessageBox.Height) / 2);
                        errorFormMessageBox.ShowMessageBoxAtLocation(centerPoint);
                    }
                }
            );
            bw.RunWorkerAsync();
        }

        void BackButtonClicked(object sender, EventArgs e)
        {
            GoToPreviousForm();
        }

        void GoToPreviousForm()
        {
            FormInsertSmartCard instanceFormInsertSmartCard = FormsManager.GetFormInsertSmartCard();
            instanceFormInsertSmartCard.Visible = true;
            instanceFormInsertSmartCard.Enabled = true;
            instanceFormInsertSmartCard.RefreshUI();
            instanceFormInsertSmartCard.BringToFront();
            this.Visible = false;
        }

        protected void BlinkButtonSignalClockHasChanged(int state)
        {
            examCourseType1panel.BackColor = Util.GetButtonBlinkColorAtSignalState_Yellow(state);
            examCourseType2panel.BackColor = Util.GetButtonBlinkColorAtSignalState_Yellow(state);
        }
    }
}
