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
        Label examSeqLabel;

        Color buttonClickColor = GlobalColor.yellowColor;
        Color buttonDefaultColor = GlobalColor.paleRoseColor;

        FormLargeMessageBox dataMissingMessageBox;
        BlinkButtonSignalClock blinkButtonSignalClock;

        int selectedDayIndex = -1;
        int selectedMonthIndex = -1;
        int selectedYearIndex = -1;
        int selectedExamSeqIndex = -1;

        public FormCourseRegisterSetting()
        {
            InitializeComponent();

            RenderUI();

            dataMissingMessageBox = new FormLargeMessageBox(0, MessageBoxIcon.Null);
            dataMissingMessageBox.Visible = false;
            dataMissingMessageBox.rightButton.Click += new EventHandler(DataMissingMessageBoxRightButtonClicked);

            blinkButtonSignalClock = new BlinkButtonSignalClock();
            blinkButtonSignalClock.TheTimeChanged += new BlinkButtonSignalClock.BlinkButtonSignalClockTickHandler(BlinkButtonSignalClockHasChanged);
    
        }

        private void RenderUI()
        {
            goToUserDetailButton = new MediumButton();
            goToUserDetailButton.Location = new Point(SCREEN_WIDTH - goToUserDetailButton.Width - 50,
                                                      SCREEN_HEIGHT - goToUserDetailButton.Height - 50);
            goToUserDetailButton.Click += new EventHandler(GoToUserDetailButtonClicked);
            goToUserDetailButton.ForeColor = Color.White;
            goToUserDetailButton.BackColor = GlobalColor.redColor;

            backButton = new MediumButton();
            backButton.Location = new Point(goToUserDetailButton.Location.X - backButton.Width - 50,
                                            goToUserDetailButton.Location.Y);
            backButton.Click += new EventHandler(BackButtonClicked);
            backButton.ForeColor = Color.White;
            backButton.BackColor = GlobalColor.redColor;

            int gapX = 30;
            int gapY = 5;
            int buttonHeight = 50;
            int buttonFontSize = 16;

            courseRegisterDateTopicLabel = new Label();
            courseRegisterDateTopicLabel.Width = SCREEN_WIDTH - gapX * 2;
            courseRegisterDateTopicLabel.Height = 60;
            courseRegisterDateTopicLabel.BackColor = GlobalColor.yellowColor;
            courseRegisterDateTopicLabel.Location = new Point(gapX, headerLineLabel.Location.Y + 20);
            courseRegisterDateTopicLabel.TextAlign = ContentAlignment.MiddleCenter;
            courseRegisterDateTopicLabel.Font = new Font(this.Font.FontFamily, 18);

            examSeqTopicLabel = new Label();
            examSeqTopicLabel.Width = courseRegisterDateTopicLabel.Width;
            examSeqTopicLabel.Height = courseRegisterDateTopicLabel.Height;
            examSeqTopicLabel.BackColor = courseRegisterDateTopicLabel.BackColor;
            examSeqTopicLabel.Location = new Point(gapX, backButton.Location.Y - examSeqTopicLabel.Height - 130);
            examSeqTopicLabel.TextAlign = ContentAlignment.MiddleCenter;
            examSeqTopicLabel.Font = courseRegisterDateTopicLabel.Font;

            int heightPerRow = (examSeqTopicLabel.Location.Y - (courseRegisterDateTopicLabel.Location.Y + courseRegisterDateTopicLabel.Height) - gapY * 2) / 4;
            dayLabel = new Label();
            dayLabel.Width = 140;
            dayLabel.Height = heightPerRow * 2;
            dayLabel.Location = new Point(gapX * 3, courseRegisterDateTopicLabel.Location.Y + courseRegisterDateTopicLabel.Height + gapY);
            dayLabel.TextAlign = ContentAlignment.MiddleLeft;
            dayLabel.Font = new Font(this.Font.FontFamily, 18);

            monthLabel = new Label();
            monthLabel.Size = new Size(dayLabel.Width, heightPerRow);
            monthLabel.Location = new Point(dayLabel.Location.X, dayLabel.Location.Y + dayLabel.Height);
            monthLabel.TextAlign = dayLabel.TextAlign;
            monthLabel.Font = dayLabel.Font;

            yearLabel = new Label();
            yearLabel.Size = new Size(dayLabel.Width, heightPerRow);
            yearLabel.Location = new Point(dayLabel.Location.X, monthLabel.Location.Y + monthLabel.Height);
            yearLabel.TextAlign = dayLabel.TextAlign;
            yearLabel.Font = dayLabel.Font;

            dayButtonArray = new Button[31];
            int dayButtonGapX = 10;
            for (int i = 0; i < dayButtonArray.Length; i++)
            {
                Button b = new Button();
                b.Height = buttonHeight;
                b.Width = (int)(b.Height * 1.6);
                b.Text = "" + (i + 1);
                b.TextAlign = ContentAlignment.MiddleCenter;
                b.Font = new Font(this.Font.FontFamily, buttonFontSize);
                b.BackColor = buttonDefaultColor;
                b.Tag = i;
                if (i <= 15)
                {
                    b.Location = new Point(dayLabel.Location.X + dayLabel.Width + dayButtonGapX + (b.Width + dayButtonGapX) * i,
                                           dayLabel.Location.Y + dayLabel.Height / 2 - b.Height - gapY);
                }
                else
                {
                    b.Location = new Point(dayLabel.Location.X + dayLabel.Width + dayButtonGapX + (b.Width + dayButtonGapX) * (i - 16),
                                           dayLabel.Location.Y + dayLabel.Height / 2 + gapY);
               }
                b.Click += new EventHandler(DayButtonArrayClicked);
                dayButtonArray[i] = b;
                this.Controls.Add(b);
            }

            monthButtonArray = new Button[12];
            for (int i = 0; i < monthButtonArray.Length; i++ )
            {
                Button b = new Button();
                b.Height = buttonHeight;
                b.Width = (int)(b.Height * 3);
                b.Text = LocalizedTextManager.GetLocalizedTextForKey("FormCourseRegisterSetting.Month." + (i + 1));
                b.TextAlign = ContentAlignment.MiddleCenter;
                b.Font = new Font(this.Font.FontFamily, buttonFontSize);
                b.BackColor = buttonDefaultColor;
                b.Tag = i;
                if (i <= 5)
                {
                    b.Location = new Point(dayLabel.Location.X + dayLabel.Width + dayButtonGapX + (b.Width + dayButtonGapX) * i,
                                               monthLabel.Location.Y + monthLabel.Height / 2 - b.Height - gapY);             
                }
                else
                {
                    b.Location = new Point(dayLabel.Location.X + dayLabel.Width + dayButtonGapX + (b.Width + dayButtonGapX) * (i - 6),
                                               monthLabel.Location.Y + monthLabel.Height / 2 + gapY);  
                }
                b.Click += new EventHandler(MonthButtonArrayClicked);
                monthButtonArray[i] = b;
                this.Controls.Add(b);
            }

            yearButtonArray = new Button[3];
            int thisYear = DateTime.Now.Year;
            thisYear = thisYear < 2500 ? thisYear + 543 : thisYear;

            for (int i = 0; i < yearButtonArray.Length; i++)
            {
                Button b = new Button();
                b.Height = buttonHeight;
                b.Width = (int)(b.Height * 2);
                b.Text = (thisYear - (2 - i)) + "";
                b.TextAlign = ContentAlignment.MiddleCenter;
                b.Font = new Font(this.Font.FontFamily, buttonFontSize);
                b.BackColor = buttonDefaultColor;
                b.Tag = i;
                b.Location = new Point(dayLabel.Location.X + dayLabel.Width + dayButtonGapX + (b.Width + dayButtonGapX) * i,
                                           yearLabel.Location.Y + yearLabel.Height / 2 - b.Height / 2);
                b.Click += new EventHandler(YearButtonArrayClicked);
                yearButtonArray[i] = b;
                this.Controls.Add(b);
            }

            examSeqLabel = new Label();
            examSeqLabel.Size = new Size((int)(dayLabel.Width * 1.5), heightPerRow);
            examSeqLabel.Location = new Point(gapX * 3, examSeqTopicLabel.Location.Y + examSeqTopicLabel.Height + gapY);
            examSeqLabel.TextAlign = dayLabel.TextAlign;
            examSeqLabel.Font = dayLabel.Font;

            examSeqButtonArray = new Button[3];
            for (int i = 0; i < examSeqButtonArray.Length; i++)
            {
                Button b = new Button();
                b.Height = buttonHeight;
                b.Width = (int)(b.Height * 1.6);
                b.Text = "" + (i + 1);
                b.TextAlign = ContentAlignment.MiddleCenter;
                b.Font = new Font(this.Font.FontFamily, buttonFontSize);
                b.BackColor = buttonDefaultColor;
                b.Tag = i;
                b.Location = new Point(examSeqLabel.Location.X + examSeqLabel.Width + dayButtonGapX + (b.Width + dayButtonGapX) * i,
                                           examSeqLabel.Location.Y + examSeqLabel.Height / 2 - b.Height / 2);
                b.Click += new EventHandler(ExamSeqButtonArrayClicked);
                examSeqButtonArray[i] = b;
                this.Controls.Add(b);
            }

            this.Controls.Add(courseRegisterDateTopicLabel);
            this.Controls.Add(examSeqTopicLabel);
            this.Controls.Add(examSeqLabel);
            this.Controls.Add(dayLabel);
            this.Controls.Add(monthLabel);
            this.Controls.Add(yearLabel);
            this.Controls.Add(goToUserDetailButton);
            this.Controls.Add(backButton);
        }

        void DayButtonArrayClicked(object sender, EventArgs e)
        {
            for (int i = 0; i < dayButtonArray.Length; i++)
            {
                dayButtonArray[i].BackColor = buttonDefaultColor;
            }
            int targetIdx = (int)((Button)sender).Tag;
            dayButtonArray[targetIdx].BackColor = buttonClickColor;
            selectedDayIndex = targetIdx;
        }

        void MonthButtonArrayClicked(object sender, EventArgs e)
        {
            for (int i = 0; i < monthButtonArray.Length; i++)
            {
                monthButtonArray[i].BackColor = buttonDefaultColor;
            }
            int targetIdx = (int)((Button)sender).Tag;
            monthButtonArray[targetIdx].BackColor = buttonClickColor;
            selectedMonthIndex = targetIdx;
        }

        void YearButtonArrayClicked(object sender, EventArgs e)
        {
            for (int i = 0; i < yearButtonArray.Length; i++)
            {
                yearButtonArray[i].BackColor = buttonDefaultColor;
            }
            int targetIdx = (int)((Button)sender).Tag;
            yearButtonArray[targetIdx].BackColor = buttonClickColor;
            selectedYearIndex = targetIdx;
        }

        void ExamSeqButtonArrayClicked(object sender, EventArgs e)
        {
            for (int i = 0; i < examSeqButtonArray.Length; i++)
            {
                examSeqButtonArray[i].BackColor = buttonDefaultColor;
            }
            int targetIdx = (int)((Button)sender).Tag;
            examSeqButtonArray[targetIdx].BackColor = buttonClickColor;
            selectedExamSeqIndex = targetIdx;
        }

        void DataMissingMessageBoxRightButtonClicked(object sender, EventArgs e)
        {
            dataMissingMessageBox.Visible = false;
            FormsManager.GetFormFadeView().ShowFadeView(false);
            this.Visible = true;
            this.Enabled = true;
            this.BringToFront();
        }

        public void RefreshUI()
        {
            backButton.Text = LocalizedTextManager.GetLocalizedTextForKey("FormCourseRegisterSetting.Button.GoBack");
            goToUserDetailButton.Text = LocalizedTextManager.GetLocalizedTextForKey("FormCourseRegisterSetting.Button.Next");
            courseRegisterDateTopicLabel.Text = LocalizedTextManager.GetLocalizedTextForKey("FormCourseRegisterSetting.Topic.CourseRegisterDate");
            examSeqTopicLabel.Text = LocalizedTextManager.GetLocalizedTextForKey("FormCourseRegisterSetting.Topic.ExamSeq");
            examSeqLabel.Text = LocalizedTextManager.GetLocalizedTextForKey("FormCourseRegisterSetting.Label.ExamSeq");           
            dayLabel.Text = LocalizedTextManager.GetLocalizedTextForKey("FormCourseRegisterSetting.Label.Day");
            monthLabel.Text = LocalizedTextManager.GetLocalizedTextForKey("FormCourseRegisterSetting.Label.Month");
            yearLabel.Text = LocalizedTextManager.GetLocalizedTextForKey("FormCourseRegisterSetting.Label.Year");

            dataMissingMessageBox.messageLabel.Text = LocalizedTextManager.GetLocalizedTextForKey("CourseRegisterDataMissingMessageBox.Message");
            dataMissingMessageBox.rightButton.Text = LocalizedTextManager.GetLocalizedTextForKey("CourseRegisterDataMissingMessageBox.RightButton");

            for (int i = 0; i < monthButtonArray.Length; i++)
            {
                Button b = monthButtonArray[i];
                b.Text = LocalizedTextManager.GetLocalizedTextForKey("FormCourseRegisterSetting.Month." + (i + 1));
            }

            if (selectedDayIndex != -1)
            {
                dayButtonArray[selectedDayIndex].BackColor = buttonDefaultColor;
                selectedDayIndex = -1;
            }
            if (selectedMonthIndex != -1)
            {
                monthButtonArray[selectedMonthIndex].BackColor = buttonDefaultColor;
                selectedMonthIndex = -1;
            }
            if (selectedYearIndex != -1)
            {
                yearButtonArray[selectedYearIndex].BackColor = buttonDefaultColor;
                selectedYearIndex = -1;
            }
            if (selectedExamSeqIndex != -1)
            {
                examSeqButtonArray[selectedExamSeqIndex].BackColor = buttonDefaultColor;
                selectedExamSeqIndex = -1;
            }
        }

        void GoToUserDetailButtonClicked(object sender, EventArgs e)
        {
            if (selectedDayIndex >= 0 && selectedMonthIndex >= 0 &&
                selectedYearIndex >= 0 && selectedExamSeqIndex >= 0)
            {
                GoToNextForm();
            }
            else
            {
                Point centerPoint = new Point((SCREEN_WIDTH - dataMissingMessageBox.Width) / 2,
                                              (SCREEN_HEIGHT - dataMissingMessageBox.Height) / 2);
                dataMissingMessageBox.ShowMessageBoxAtLocation(centerPoint);
            }            
        }

        void BackButtonClicked(object sender, EventArgs e)
        {
            GoToPreviousForm();
        }

        public void GoToNextForm()
        {
            string dayStr = (selectedDayIndex + 1 < 10) ? "0" + (selectedDayIndex + 1) : "" + (selectedDayIndex + 1);
            string monthStr = (selectedMonthIndex + 1 < 10) ? "0" + (selectedMonthIndex + 1) : "" + (selectedMonthIndex + 1);
            string yearStr = yearButtonArray[selectedYearIndex].Text;
            string dateStr = dayStr + "/" + monthStr + "/" + yearStr;
            string examSeqStr = "" + (selectedExamSeqIndex + 1);

            UserProfileManager.SetCourseRegisterDate(dateStr);
            UserProfileManager.SetExamSeq(examSeqStr);

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

        protected void BlinkButtonSignalClockHasChanged(int state)
        {
            //Temporary disable this code
            /*bool isDataComplete = selectedDayIndex >= 0 && selectedMonthIndex >= 0 &&
                                  selectedYearIndex >= 0 && selectedExamSeqIndex >= 0;
            if (isDataComplete)
            {
                goToUserDetailButton.BackColor = Util.GetButtonBlinkColorAtSignalState(state);
            }
            else
            {
                goToUserDetailButton.BackColor = Color.White;
            }*/
        }
    }
}
