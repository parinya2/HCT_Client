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
    public partial class FormExamResult : FixedSizeFormWithHeader
    {
        public LargeButton finishExamButton;
        public LargeButton viewAnswerButton;
        public BaseTextLabel passOrFailLabel;
        public BaseTextLabel scoreLabel;
        BlinkButtonSignalClock blinkButtonSignalClock;
        FormLargeMessageBox finishExamMessageBox;
        FormFadeView fadeForm;

        public FormExamResult()
        {
            InitializeComponent();
            
            RenderUI();

            finishExamMessageBox = new FormLargeMessageBox(0, MessageBoxIcon.Null);
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

            passOrFailLabel = new BaseTextLabel();
            passOrFailLabel.Width = SCREEN_WIDTH;
            passOrFailLabel.Height = 140;
            passOrFailLabel.TextAlign = ContentAlignment.MiddleCenter;
            passOrFailLabel.Location = new Point(0, headerLineLabel.Location.Y + 60);
            passOrFailLabel.Font = UtilFonts.GetTHSarabunFont(85);

            scoreLabel = new BaseTextLabel();
            scoreLabel.Width = SCREEN_WIDTH;
            scoreLabel.Height = 100;
            scoreLabel.TextAlign = ContentAlignment.MiddleCenter;
            scoreLabel.Location = new Point(0, passOrFailLabel.Location.Y + passOrFailLabel.Height + 50);
            scoreLabel.ForeColor = Color.Black;
            scoreLabel.Font = UtilFonts.GetTHSarabunFont(60);

            viewAnswerButton = new LargeButton();
            viewAnswerButton.Click += new EventHandler(ViewAnswerButtonClicked);
            viewAnswerButton.Location = new Point(SCREEN_WIDTH / 2 - viewAnswerButton.Width- buttonOffsetX,
                                                  scoreLabel.Location.Y + scoreLabel.Height + buttonOffsetY * 2);
            viewAnswerButton.Text = LocalizedTextManager.GetLocalizedTextForKey("FormExamResult.ViewAnswer");

            finishExamButton = new LargeButton();
            finishExamButton.Click += new EventHandler(FinishExamButtonClicked);
            finishExamButton.Location = new Point(SCREEN_WIDTH / 2 + buttonOffsetX,
                                                  viewAnswerButton.Location.Y);
            finishExamButton.Text = LocalizedTextManager.GetLocalizedTextForKey("FormExamResult.FinishButton");

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

        public void displayScore()
        {
            QuizResult quizResult = QuizManager.GetQuizResult();

            if (quizResult.passFlag == QuizResultPassFlag.Pass)
            {
                passOrFailLabel.Text = LocalizedTextManager.GetLocalizedTextForKey("FormExamResult.PassExam");
                passOrFailLabel.ForeColor = GlobalColor.greenColor;
            }
            else if (quizResult.passFlag == QuizResultPassFlag.Fail)
            {
                passOrFailLabel.Text = LocalizedTextManager.GetLocalizedTextForKey("FormExamResult.FailExam");
                passOrFailLabel.ForeColor = GlobalColor.redColor;
            }
            else
            {
                passOrFailLabel.Text = "-";
                passOrFailLabel.ForeColor = Color.Black;           
            }

            scoreLabel.Text = LocalizedTextManager.GetLocalizedTextForKey("FormExamResult.ScoreText") + " " + quizResult.quizScore + " / " + QuizManager.GetQuizArray().Length;
        }

        void GenerateExamResultDocument()
        {
            string fullname = UserProfileManager.GetFullnameTH().Length > 0 ? UserProfileManager.GetFullnameTH() : UserProfileManager.GetFullnameEN();
            string citizenID = UserProfileManager.GetAvailablePersonID();
            
            string courseName = LocalizedTextManager.GetLocalizedTextForKeyTH("FormChooseExamCourse.Button." + QuizManager.GetExamCourseType());

            string examStartDateString = Util.ConvertDateToNormalDateString(QuizManager.GetExamStartDateTime(), true);
            string examEndDateString = Util.ConvertDateToNormalDateString(QuizManager.GetExamEndDateTime(), true);
            string paperTestNumber = QuizManager.GetPaperTestNumber();
            string examDuration = "" + Util.GetMinuteDifferentOfTwoDates(QuizManager.GetExamStartDateTime(), QuizManager.GetExamEndDateTime());
            string courseRegisterDateString = UserProfileManager.GetCourseRegisterDateString();
            string examSeqString = UserProfileManager.GetExamSeq();
            bool isCitizenIDAvailable = UserProfileManager.IsCitizenIDAvailable();

            string passOrFail = "";

            QuizResult quizResult = QuizManager.GetQuizResult();
            int quizCount = QuizManager.GetQuizArray().Length;

            if (quizResult.passFlag == QuizResultPassFlag.Pass)
            {
                passOrFail = "สอบผ่าน ( ได้คะแนน " + quizResult.quizScore + " / " + quizCount + " )";
            }
            else if (quizResult.passFlag == QuizResultPassFlag.Fail)
            {
                passOrFail = "สอบไม่ผ่าน ( ได้คะแนน " + quizResult.quizScore + " / " + quizCount + " )";
            }
            else if (quizResult.passFlag == QuizResultPassFlag.NotCompleted)
            {
                passOrFail = "สอบไม่ผ่าน เนื่องจากตอบคำถามไม่ครบทุกข้อ";
            }
            else
            {
                passOrFail = "เกิดความผิดพลาดบางอย่าง กรุณาติดต่อผู้ดูแลระบบ";
            }

            string emailBody = "นี่คือผลการสอบของ " + fullname + " ณ วันที่ " + examStartDateString + " น." +
                               "\n" + "ผลการสอบ คือ " + passOrFail +
                               "\n\n" + "ไฟล์ pdf ที่แนบมาด้วย คือ หลักฐานแสดงผลการสอบของผู้เข้าสอบ" +
                               "\n\n" + "อีเมล์ฉบับนี้ถูกส่งมาจากระบบอัตโนมัติ กรุณาอย่าตอบกลับอีเมล์ฉบับนี้" +
                               "\n\n\n" + "ขอแสดงความนับถือ" +
                               "\n" + "โรงเรียนสอนขับรถ หาดใหญ่ คาร์ เทรนเนอร์";

            Dictionary<string, string> pdfDict = Util.CreateExamResultPDF(fullname, 
                                                      citizenID, 
                                                      courseName, 
                                                      passOrFail, 
                                                      examStartDateString,
                                                      examEndDateString,
                                                      courseRegisterDateString,
                                                      paperTestNumber,
                                                      examSeqString,
                                                      isCitizenIDAvailable);

            string pdfName = pdfDict["pdfName"];
            string pdfBase64String = pdfDict["pdfBase64String"];
            QuizManager.GetQuizResult().quizResultPdfBase64String = pdfBase64String;
            Util.SendEmailWithAttachment(pdfName, emailBody);
        }

        void GoToFirstForm()
        {
            UserProfileManager.ClearUserProfile();
            QuizManager.ClearAllData();

            FormChooseLanguage instanceFormChooseLanguage = FormsManager.GetFormChooseLanguage();
            instanceFormChooseLanguage.Visible = true;
            instanceFormChooseLanguage.BringToFront();
            
            this.Visible = false;
            fadeForm.Visible = false;
            finishExamMessageBox.Visible = false;

            GlobalData.tmpRount++;
        }

        void FinishExamButtonClicked(object sender, EventArgs e)
        {
            if (WebServiceManager.webServiceMode == WebServiceMode.SimulatorMode)
            {
                GoToFirstForm();
                return;
            }

            Point centerPoint = new Point((SCREEN_WIDTH - finishExamMessageBox.Width) / 2,
                                          (SCREEN_HEIGHT - finishExamMessageBox.Height) / 2);

            FormsManager.GetFormLoadingView().ShowLoadingView(true);
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(
                delegate(object o, DoWorkEventArgs args)
                {
                    Thread.Sleep(10);
                    GenerateExamResultDocument();
                    WebServiceManager.SendExamResultToHCTLogServer();                    
                }
             );
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(
                delegate(object o, RunWorkerCompletedEventArgs args)
                {
                    FormsManager.GetFormLoadingView().ShowLoadingView(false);
                    finishExamMessageBox.ShowMessageBoxAtLocation(centerPoint);
                }
            );
            bw.RunWorkerAsync();

            /* This is the old code
            Point centerPoint = new Point((SCREEN_WIDTH - finishExamMessageBox.Width) / 2,
                                          (SCREEN_HEIGHT - finishExamMessageBox.Height) / 2);
    
            fadeForm.Visible = true;
            fadeForm.BringToFront();

            FormLargeMessageBox systemProcessingMessageBox = FormsManager.GetFormSystemProcessingMessageBox();
            systemProcessingMessageBox.ShowMessageBoxAtLocation(centerPoint);

            WebServiceManager.SendExamResultToHCTLogServer();
            GenerateExamResultDocument();
            systemProcessingMessageBox.Visible = false;

            finishExamMessageBox.ShowMessageBoxAtLocation(centerPoint);
             */
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
            //Temporary disable this code
            //finishExamMessageBox.rightButton.BackColor = Util.GetButtonBlinkColorAtSignalState(state);
        }
    }
}
