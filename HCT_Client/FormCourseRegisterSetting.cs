using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace HCT_Client
{
    public partial class FormCourseRegisterSetting : FixedSizeFormWithHeader
    {
        MediumButton goToUserDetailButton;
        MediumButton backButton;

        Button[] dayButtonArray;
        Button[] monthButtonArray;
        Button[] yearButtonArray;
        Button[] examSeqButtonArray;

        Label courseRegisterDateTopicLabel;
        Label dayLabel;
        Label monthLabel;
        Label yearLabel;
        Label examSeqTopicLabel;

        public FormCourseRegisterSetting()
        {
            InitializeComponent();

            RenderUI();
        }

        private void RenderUI()
        {
            goToUserDetailButton = new MediumButton();
            goToUserDetailButton.Location = new Point(SCREEN_WIDTH - goToUserDetailButton.Width - 50,
                                                      SCREEN_HEIGHT - goToUserDetailButton.Height - 50);
            goToUserDetailButton.Click += new EventHandler(GoToUserDetailButtonClicked);


            backButton = new MediumButton();
            backButton.Location = new Point(goToUserDetailButton.Location.X - backButton.Width - 50,
                                            goToUserDetailButton.Location.Y);
            backButton.Click += new EventHandler(BackButtonClicked);

            int gapX = 30;
            int gapY = 5;
            courseRegisterDateTopicLabel = new Label();
            courseRegisterDateTopicLabel.Width = SCREEN_WIDTH - gapX * 2;
            courseRegisterDateTopicLabel.Height = 80;
            courseRegisterDateTopicLabel.BackColor = Color.Orange;
            courseRegisterDateTopicLabel.Location = new Point(gapX, headerLineLabel.Location.Y + 20);
            courseRegisterDateTopicLabel.TextAlign = ContentAlignment.MiddleCenter;
            courseRegisterDateTopicLabel.Font = new Font(this.Font.FontFamily, 22);

            examSeqTopicLabel = new Label();
            examSeqTopicLabel.Width = courseRegisterDateTopicLabel.Width;
            examSeqTopicLabel.Height = courseRegisterDateTopicLabel.Height;
            examSeqTopicLabel.BackColor = courseRegisterDateTopicLabel.BackColor;
            examSeqTopicLabel.Location = new Point(gapX, backButton.Location.Y - examSeqTopicLabel.Height - 100);
            examSeqTopicLabel.TextAlign = ContentAlignment.MiddleCenter;
            examSeqTopicLabel.Font = courseRegisterDateTopicLabel.Font;

            dayLabel = new Label();
            dayLabel.Width = 120;
            dayLabel.Height = (examSeqTopicLabel.Location.Y - (courseRegisterDateTopicLabel.Location.Y + courseRegisterDateTopicLabel.Height) - gapY * 2) / 3;
            dayLabel.Location = new Point(gapX * 3, courseRegisterDateTopicLabel.Location.Y + courseRegisterDateTopicLabel.Height + gapY);
            dayLabel.TextAlign = ContentAlignment.MiddleLeft;
            dayLabel.Font = new Font(this.Font.FontFamily, 18);

            monthLabel = new Label();
            monthLabel.Size = new Size(dayLabel.Width, dayLabel.Height);
            monthLabel.Location = new Point(dayLabel.Location.X, dayLabel.Location.Y + dayLabel.Height);
            monthLabel.TextAlign = dayLabel.TextAlign;
            monthLabel.Font = dayLabel.Font;

            yearLabel = new Label();
            yearLabel.Size = new Size(dayLabel.Width, dayLabel.Height);
            yearLabel.Location = new Point(dayLabel.Location.X, monthLabel.Location.Y + monthLabel.Height);
            yearLabel.TextAlign = dayLabel.TextAlign;
            yearLabel.Font = dayLabel.Font;

            this.Controls.Add(courseRegisterDateTopicLabel);
            this.Controls.Add(examSeqTopicLabel);
            this.Controls.Add(dayLabel);
            this.Controls.Add(monthLabel);
            this.Controls.Add(yearLabel);
            this.Controls.Add(goToUserDetailButton);
            this.Controls.Add(backButton);
        }

        public void RefreshUI()
        {
            backButton.Text = LocalizedTextManager.GetLocalizedTextForKey("FormCourseRegisterSetting.Button.GoBack");
            goToUserDetailButton.Text = LocalizedTextManager.GetLocalizedTextForKey("FormCourseRegisterSetting.Button.Next");
            courseRegisterDateTopicLabel.Text = LocalizedTextManager.GetLocalizedTextForKey("FormCourseRegisterSetting.Topic.CourseRegisterDate");
            examSeqTopicLabel.Text = LocalizedTextManager.GetLocalizedTextForKey("FormCourseRegisterSetting.Topic.ExamSeq");
            dayLabel.Text = LocalizedTextManager.GetLocalizedTextForKey("FormCourseRegisterSetting.Label.Day");
            monthLabel.Text = LocalizedTextManager.GetLocalizedTextForKey("FormCourseRegisterSetting.Label.Month");
            yearLabel.Text = LocalizedTextManager.GetLocalizedTextForKey("FormCourseRegisterSetting.Label.Year");
           
        }

        void GoToUserDetailButtonClicked(object sender, EventArgs e)
        {
            GoToNextForm();
        }

        void BackButtonClicked(object sender, EventArgs e)
        {
            GoToPreviousForm();
        }

        void GoToNextForm()
        {
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

        void GoToPreviousForm()
        {
            FormChooseExamCourse instanceFormChooseExamCourse = FormsManager.GetFormChooseExamCourse();
            instanceFormChooseExamCourse.Visible = true;
            instanceFormChooseExamCourse.Enabled = true;
            instanceFormChooseExamCourse.RefreshUI();
            instanceFormChooseExamCourse.BringToFront();
            this.Visible = false;
        }
    }
}
