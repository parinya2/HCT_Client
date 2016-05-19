﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HCT_Client
{
    public partial class FormExamResult : FixedSizeFormWithHeader
    {
        public LargeButton finishExamButton;
        public LargeButton viewAnswerButton;
        public BaseTextLabel passOrFailLabel;
        public BaseTextLabel scoreLabel;
        BlinkButtonSignalClock blinkButtonSignalClock;
        FormLargeMessageBox finishExamMessageBox;
        FormFadeView fadeForm;
        int SCORE_TO_PASS = 40;
        int score;

        public FormExamResult()
        {
            InitializeComponent();
            
            RenderUI();

            finishExamMessageBox = new FormLargeMessageBox(0);
            finishExamMessageBox.Visible = false;
            finishExamMessageBox.rightButton.Click += new EventHandler(FinishExamMessageBoxRightButtonClicked);

            fadeForm = FormsManager.GetFormFadeView();

            blinkButtonSignalClock = new BlinkButtonSignalClock();
            blinkButtonSignalClock.TheTimeChanged += new BlinkButtonSignalClock.BlinkButtonSignalClockTickHandler(BlinkButtonSignalClockHasChanged);  
        }

        private void RenderUI()
        {
            int buttonOffsetX = 50;
            int buttonOffsetY = 30;

            finishExamButton = new LargeButton();
            finishExamButton.Click += new EventHandler(FinishExamButtonClicked);
            finishExamButton.Location = new Point(SCREEN_WIDTH - buttonOffsetX - finishExamButton.Width,
                                                  SCREEN_HEIGHT - buttonOffsetY - finishExamButton.Height);
            finishExamButton.Text = LocalizedTextManager.GetLocalizedTextForKey("FormExamResult.FinishButton");

            viewAnswerButton = new LargeButton();
            viewAnswerButton.Click += new EventHandler(ViewAnswerButtonClicked);
            viewAnswerButton.Location = new Point(finishExamButton.Location.X - buttonOffsetX - viewAnswerButton.Width,
                                                  finishExamButton.Location.Y);
            viewAnswerButton.Text = LocalizedTextManager.GetLocalizedTextForKey("FormExamResult.ViewAnswer");

            passOrFailLabel = new BaseTextLabel();
            passOrFailLabel.Width = SCREEN_WIDTH;
            passOrFailLabel.Height = 100;
            passOrFailLabel.TextAlign = ContentAlignment.MiddleCenter;
            passOrFailLabel.Location = new Point(0, headerLineLabel.Location.Y + 60);
            passOrFailLabel.Font = new Font(this.Font.FontFamily, 50);

            scoreLabel = new BaseTextLabel();
            scoreLabel.Width = SCREEN_WIDTH;
            scoreLabel.Height = 100;
            scoreLabel.TextAlign = ContentAlignment.MiddleCenter;
            scoreLabel.Location = new Point(0, passOrFailLabel.Location.Y + passOrFailLabel.Height + 50);
            scoreLabel.ForeColor = Color.Black;
            scoreLabel.Font = new Font(this.Font.FontFamily, 30);

            this.Controls.Add(passOrFailLabel);
            this.Controls.Add(scoreLabel);
            this.Controls.Add(finishExamButton);
            this.Controls.Add(viewAnswerButton);
        }

        public void RefreshUI()
        {
            finishExamButton.Text = LocalizedTextManager.GetLocalizedTextForKey("FormExamResult.FinishButton");
            viewAnswerButton.Text = LocalizedTextManager.GetLocalizedTextForKey("FormExamResult.ViewAnswer");
            finishExamMessageBox.messageLabel.Text = LocalizedTextManager.GetLocalizedTextForKey("FinishExamMessageBox.Message");
            finishExamMessageBox.rightButton.Text = LocalizedTextManager.GetLocalizedTextForKey("FinishExamMessageBox.RightButton");
        }

        public void calculateScore()
        {
            score = 0;
            SingleQuizObject[] quizArray = QuizManager.GetQuizArray();
            for (int i = 0; i < quizArray.Length; i++)
            {
                SingleQuizObject obj = quizArray[i];
                if (obj.selectedChoice == obj.correctChoice)
                {
                    score++;
                }
            }

            if (score >= SCORE_TO_PASS)
            {
                passOrFailLabel.Text = LocalizedTextManager.GetLocalizedTextForKey("FormExamResult.PassExam");
                passOrFailLabel.ForeColor = Color.LightSeaGreen;
            }
            else
            {
                passOrFailLabel.Text = LocalizedTextManager.GetLocalizedTextForKey("FormExamResult.FailExam");
                passOrFailLabel.ForeColor = Color.Red;
            }

            scoreLabel.Text = LocalizedTextManager.GetLocalizedTextForKey("FormExamResult.ScoreText") + " " + score + " / " + quizArray.Length;
        }

        void GenerateExamResultDocument()
        {
            string fullname = UserProfileManager.GetFullnameTH().Length > 0 ? UserProfileManager.GetFullnameTH() : UserProfileManager.GetFullnameEN();
            string citizenID = UserProfileManager.GetCitizenID();
            
            int courseType = QuizManager.GetExamCourseType() + 1;
            string courseName = LocalizedTextManager.GetLocalizedTextForKeyTH("FormChooseExamCourse.Button." + courseType);
            
            string dateString = DateTime.Now.ToString("d/MM/yyyy");
            string passOrFail = "";

            if (score >= SCORE_TO_PASS)
            {
                passOrFail = "สอบผ่าน ( ได้คะแนน " + score + " / " + "50 )";
            }
            else
            {
                passOrFail = "สอบไม่ผ่าน ( ได้คะแนน " + score + " / " + "50 )";
            }

            string emailBody = "นี่คือผลการสอบของ " + fullname + " ณ วันที่ " + dateString + 
                               "\n" + "ผลการสอบ คือ " + passOrFail +
                               "\n\n" + "ไฟล์ pdf ที่แนบมาด้วย คือ หลักฐานแสดงผลการสอบของผู้เข้าสอบ" +
                               "\n\n" + "อีเมล์ฉบับนี้ถูกส่งมาจากระบบอัตโนมัติ กรุณาอย่าตอบกลับอีเมล์ฉบับนี้" +
                               "\n\n\n" + "ขอแสดงความนับถือ" +
                               "\n" + "หาดใหญ่คาร์เทรนเนอร์";

            string pdfPath = Util.CreateExamResultPDF(fullname, citizenID, courseName, passOrFail, dateString);
            Util.SendEmailWithAttachment(pdfPath, emailBody);
        }

        void GoToFirstForm()
        {
            UserProfileManager.ClearUserProfile();
            FormChooseLanguage instanceFormChooseLanguage = FormsManager.GetFormChooseLanguage();
            instanceFormChooseLanguage.Visible = true;
            instanceFormChooseLanguage.BringToFront();
            
            this.Visible = false;
            fadeForm.Visible = false;
            finishExamMessageBox.Visible = false;
        }

        void FinishExamButtonClicked(object sender, EventArgs e)
        {
            Point centerPoint = new Point((SCREEN_WIDTH - finishExamMessageBox.Width) / 2,
                                          (SCREEN_HEIGHT - finishExamMessageBox.Height) / 2);
    
            fadeForm.Visible = true;
            fadeForm.BringToFront();

            FormLargeMessageBox systemProcessingMessageBox = FormsManager.GetFormSystemProcessingMessageBox();
            systemProcessingMessageBox.ShowMessageBoxAtLocation(centerPoint);

            GenerateExamResultDocument();
            systemProcessingMessageBox.Visible = false;

            finishExamMessageBox.ShowMessageBoxAtLocation(centerPoint);
        }

        void ViewAnswerButtonClicked(object sender, EventArgs e)
        {
            FormExecuteExam instanceFormExecuteExam = FormsManager.GetFormExecuteExam();
            instanceFormExecuteExam.ShowAnswer();
            instanceFormExecuteExam.Visible = true;
            instanceFormExecuteExam.BringToFront();         
            this.Visible = false;
        }

        void FinishExamMessageBoxRightButtonClicked(object sender, EventArgs e)
        {
            GoToFirstForm();
        }

        protected void BlinkButtonSignalClockHasChanged(int state)
        {
            finishExamButton.BackColor = Util.GetButtonBlinkColorAtSignalState(state);
            finishExamMessageBox.rightButton.BackColor = Util.GetButtonBlinkColorAtSignalState(state);
        }
    }
}
