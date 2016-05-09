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
    public partial class FormExecuteExam : FixedSizeForm
    {
        int QUIZ_COUNT = 50;
        int TOTAL_EXAM_TIME_SECONDS = 1000;
        int currentQuizNumber = 0;
        Panel monitorPanel;
        Panel quizListPanel;
        SingleQuizStatusPanel[] singleQuizStatusPanelArray;

        Panel quizPanel;
        BaseTextLabel usernameLabel;
        BaseImageLabel photoLabel;
        BaseTextLabel timerLabel;
        Stopwatch stopwatch;
        SignalClock signalClock;

        BaseTextLabel quizTextLabel;
        PictureBox quizImagePictureBox;
        QuizChoicePanel[] choicePanelArray;

        MediumButton submitExamButton;
        MediumButton prevQuizButton;
        MediumButton nextQuizButton;

        SingleQuizObject[] quizArray;

        FormLargeMessageBox timeoutMessageBox;
        FormLargeMessageBox quizNotCompletedMessageBox;
        FormFadeView fadeForm;

        Color correctAnswerColor = Color.ForestGreen;
        Color wrongAnswerColor = Color.Red;

        bool modeShowAnswer = false;

        bool userHasTappedChoice = false;
        int signalClockState = -1;

        Bitmap correctSignBitmap;
        Bitmap wrongSignBitmap;

        public FormExecuteExam()
        {
            InitializeComponent();
            QuizManager.InitInstance();
            RenderUI();

            timeoutMessageBox = new FormLargeMessageBox(0);
            timeoutMessageBox.messageLabel.Text = LocalizedTextManager.GetLocalizedTextForKey("TimeoutMessageBox.Message");
            timeoutMessageBox.rightButton.Text = LocalizedTextManager.GetLocalizedTextForKey("TimeoutMessageBox.RightButton");
            timeoutMessageBox.Visible = false;
            timeoutMessageBox.rightButton.Click += new EventHandler(TimeoutMessageBoxRightButtonClicked);

            quizNotCompletedMessageBox = new FormLargeMessageBox(1);
            quizNotCompletedMessageBox.messageLabel.Text = LocalizedTextManager.GetLocalizedTextForKey("QuizNotCompletedMessageBox.Message");
            quizNotCompletedMessageBox.rightButton.Text = LocalizedTextManager.GetLocalizedTextForKey("QuizNotCompletedMessageBox.RightButton");
            quizNotCompletedMessageBox.leftButton.Text = LocalizedTextManager.GetLocalizedTextForKey("QuizNotCompletedMessageBox.LeftButton");
            quizNotCompletedMessageBox.Visible = false;
            quizNotCompletedMessageBox.rightButton.Click += new EventHandler(QuizNotCompletedMessageBoxRightButtonClicked);
            quizNotCompletedMessageBox.leftButton.Click += new EventHandler(QuizNotCompletedMessageBoxLeftButtonClicked);

            submitExamButton.Click += new EventHandler(SubmitExamButtonClicked);

            fadeForm = FormsManager.GetFormFadeView();

            System.Reflection.Assembly myAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            Stream myStream = myAssembly.GetManifestResourceStream("HCT_Client.CorrectSign.png");
            correctSignBitmap = new Bitmap(myStream);

            myStream = myAssembly.GetManifestResourceStream("HCT_Client.WrongSign.png");
            wrongSignBitmap = new Bitmap(myStream);
        }

        void RenderUI()
        {
            RenderMonitorPanelUI();
            RenderQuizPanelUI();
       }

        public void RefreshUI()
        {
            timeoutMessageBox.messageLabel.Text = LocalizedTextManager.GetLocalizedTextForKey("TimeoutMessageBox.Message");
            timeoutMessageBox.rightButton.Text = LocalizedTextManager.GetLocalizedTextForKey("TimeoutMessageBox.RightButton");

            quizNotCompletedMessageBox.messageLabel.Text = LocalizedTextManager.GetLocalizedTextForKey("QuizNotCompletedMessageBox.Message");
            quizNotCompletedMessageBox.rightButton.Text = LocalizedTextManager.GetLocalizedTextForKey("QuizNotCompletedMessageBox.RightButton");
            quizNotCompletedMessageBox.leftButton.Text = LocalizedTextManager.GetLocalizedTextForKey("QuizNotCompletedMessageBox.LeftButton");

            submitExamButton.Text = LocalizedTextManager.GetLocalizedTextForKey("FormExecuteExam.SubmitExamButton");

            for (int i = 0; i < singleQuizStatusPanelArray.Length; i++)
            {
                SingleQuizStatusPanel obj = singleQuizStatusPanelArray[i];
                obj.selectedAnswerLabel.Text = "-";
                obj.selectedAnswerLabel.BackColor = Color.Transparent;
                obj.SetActiveQuiz(i == 0);
            }

            for (int i = 0; i < choicePanelArray.Length; i++)
            {
                QuizChoicePanel obj = choicePanelArray[i];
                obj.choiceHeaderLabel.Text = LocalizedTextManager.GetLocalizedTextForKey("QuizChoicePanel.ChoiceHeader." + (i + 1)) + ". ";
                obj.choiceCorrectStatusPictureBox.Image = null;
            }

            currentQuizNumber = 0;
            signalClockState = -1;
        }

        public void LoadExamData()
        {
            QuizManager.LoadQuiz();
            quizArray = QuizManager.GetQuizArray();
            SetContentForQuizPanel(0);

            stopwatch = new Stopwatch(TOTAL_EXAM_TIME_SECONDS);
            stopwatch.TheTimeChanged += new Stopwatch.TimerTickHandler(StopwatchHasChanged);

            signalClock = new SignalClock(30);
            signalClock.TheTimeChanged += new SignalClock.SignalClockTickHandler(SignalClockHasChanged);
 
        }

        private void RenderQuizPanelUI()
        {
            quizPanel = new Panel();
            quizPanel.Width = SCREEN_WIDTH - monitorPanel.Width;
            quizPanel.Height = SCREEN_HEIGHT;
            quizPanel.BackColor = Color.LightGray;
            quizPanel.Location = new Point(monitorPanel.Width, 0);

            quizTextLabel = new BaseTextLabel();
            int quizTextLabelOffsetX = 40;
            int quizTextLabelOffsetY = 40;
            quizTextLabel.Width = quizPanel.Width - (quizTextLabelOffsetX * 2);
            quizTextLabel.Height = 100;
            quizTextLabel.Location = new Point(quizTextLabelOffsetX, quizTextLabelOffsetY);
            quizTextLabel.ForeColor = Color.Black;

            quizImagePictureBox = new PictureBox();
            quizImagePictureBox.Width = quizTextLabel.Width;
            quizImagePictureBox.Height = 300;
            quizImagePictureBox.Location = new Point(quizTextLabel.Location.X, 
                quizTextLabel.Location.Y + quizTextLabel.Height + quizTextLabelOffsetY);
            quizImagePictureBox.BackColor = Color.Transparent;

            int choiceLabelOffsetX = 20;
            int choiceLabelOffsetY = 20;
            choicePanelArray = new QuizChoicePanel[4];
            for (int i = 0; i < choicePanelArray.Length; i++)
            {
                int objWidth = (quizPanel.Width - (choiceLabelOffsetX * 3)) / 2;
                int objHeight = (quizPanel.Height - quizImagePictureBox.Location.Y - quizImagePictureBox.Height - (choiceLabelOffsetY * 3)) / 2;
               
                QuizChoicePanel obj = new QuizChoicePanel(objWidth, objHeight, i);
                switch (i) 
                {
                    case 0:
                        obj.Location = new Point(choiceLabelOffsetX,
                                            quizImagePictureBox.Location.Y + quizImagePictureBox.Height + choiceLabelOffsetY);
                        break;
                    case 1:
                        obj.Location = new Point(obj.Width + (choiceLabelOffsetX * 2),
                                            quizImagePictureBox.Location.Y + quizImagePictureBox.Height + choiceLabelOffsetY);
                        break;
                    case 2:
                        obj.Location = new Point(choiceLabelOffsetX,
                                            quizImagePictureBox.Location.Y + quizImagePictureBox.Height + obj.Height + (choiceLabelOffsetY * 2));
                        break;
                    case 3:
                        obj.Location = new Point(obj.Width + (choiceLabelOffsetX * 2),
                                            quizImagePictureBox.Location.Y + quizImagePictureBox.Height + obj.Height + (choiceLabelOffsetY * 2));
                        break;
                }

                obj.choiceHeaderLabel.Click += new EventHandler(ChoicePanelClicked);
                obj.choiceTextLabel.Click += new EventHandler(ChoicePanelClicked);
                obj.choiceCorrectStatusPictureBox.Click += new EventHandler(ChoicePanelClicked);

                choicePanelArray[i] = obj;
                quizPanel.Controls.Add(obj);
            }

            quizPanel.Controls.Add(quizImagePictureBox);
            quizPanel.Controls.Add(quizTextLabel);

            this.Controls.Add(quizPanel);
        }

        private void RenderMonitorPanelUI()
        {
            monitorPanel = new Panel();
            monitorPanel.Width = (int)(SCREEN_WIDTH * 0.3);
            monitorPanel.Height = SCREEN_HEIGHT;
            monitorPanel.BackColor = Color.Black;
            monitorPanel.Location = new Point(0, 0);

            photoLabel = new BaseImageLabel();
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

            submitExamButton = new MediumButton();
            submitExamButton.Location = new Point(monitorPanel.Width - 20 - submitExamButton.Width, 
                                                  monitorPanel.Height - 20 - submitExamButton.Height);
            submitExamButton.Text = LocalizedTextManager.GetLocalizedTextForKey("FormExecuteExam.SubmitExamButton");

            prepareQuizListPanelUI();

            prevQuizButton = new MediumButton();
            prevQuizButton.Width = 50;
            prevQuizButton.Height = submitExamButton.Height;
            prevQuizButton.Location = new Point(timerLabel.Location.X, submitExamButton.Location.Y);
            prevQuizButton.Text = "<<";
            prevQuizButton.Click += new EventHandler(PrevQuizButtonClicked);

            nextQuizButton = new MediumButton();
            nextQuizButton.Width = 50;
            nextQuizButton.Height = submitExamButton.Height;
            nextQuizButton.Location = new Point(prevQuizButton.Location.X + prevQuizButton.Width + 5, submitExamButton.Location.Y);
            nextQuizButton.Text = ">>";
            nextQuizButton.Click += new EventHandler(NextQuizButtonClicked);

            monitorPanel.Controls.Add(prevQuizButton);
            monitorPanel.Controls.Add(nextQuizButton);
            monitorPanel.Controls.Add(submitExamButton);
            monitorPanel.Controls.Add(photoLabel);
            monitorPanel.Controls.Add(usernameLabel);
            monitorPanel.Controls.Add(timerLabel);
            monitorPanel.Controls.Add(quizListPanel);

            this.Controls.Add(monitorPanel);
        }

        private void SetContentForQuizPanel(int quizNumber)
        { 
            string quizTextHeader = LocalizedTextManager.GetLocalizedTextForKey("FormExecuteExam.QuizTextLabel.Header");
            SingleQuizObject quizObj = (SingleQuizObject)(quizArray[quizNumber]);
            
            quizTextLabel.Text = quizTextHeader + " " + (quizNumber + 1) + "   " +quizObj.quizText;
            quizImagePictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            quizImagePictureBox.Image = quizObj.quizImage;

            for (int i = 0; i < quizObj.choiceTextArray.Length; i++)
            {
                string choiceText = quizObj.choiceTextArray[i];
                choicePanelArray[i].SetChoiceText(choiceText);

                bool isSelectedChoiceFlag = (i == quizObj.selectedChoice);
                choicePanelArray[i].SetSelectedChoicePanel(isSelectedChoiceFlag);
            }

            if (modeShowAnswer)
            {
                for (int i = 0; i < quizObj.choiceTextArray.Length; i++)
                {
                    choicePanelArray[i].choiceCorrectStatusPictureBox.Image = null;
                    if (quizObj.correctChoice == i)
                    {
                        choicePanelArray[i].choiceCorrectStatusPictureBox.Image = correctSignBitmap;
                    }
                    else if (quizObj.selectedChoice == i && quizObj.correctChoice != i)
                    {
                        choicePanelArray[i].choiceCorrectStatusPictureBox.Image = wrongSignBitmap;
                    }
                }
            }
        }

        private void prepareQuizListPanelUI()
        {
            quizListPanel = new Panel();
            quizListPanel.Location = new Point(0, timerLabel.Location.Y + timerLabel.Height);
            quizListPanel.Width = monitorPanel.Width;
            quizListPanel.Height = SCREEN_HEIGHT - quizListPanel.Location.Y - submitExamButton.Height - 20;

            singleQuizStatusPanelArray = new SingleQuizStatusPanel[QUIZ_COUNT];
            int quizPerColumn = 10;
            int columnCount = QUIZ_COUNT / quizPerColumn;
            int quizGapY = 10;
            int quizGapX = 10;

            int singleQuizStatusPanelHeight = (quizListPanel.Height / quizPerColumn) - quizGapY - 5;

            for (int i = 0; i < singleQuizStatusPanelArray.Length; i++)
            {
                SingleQuizStatusPanel obj = new SingleQuizStatusPanel(i, singleQuizStatusPanelHeight);
                
                int columnNo = i / quizPerColumn;
                int rowNo = i % quizPerColumn;

                int locationX = (obj.Width + quizGapX) * columnNo+ 30;
                int locationY = (obj.Height + quizGapY) * rowNo + 30;
                obj.Location = new Point(locationX, locationY);
                obj.numberLabel.Click += new EventHandler(SingleQuizStatusPanelClicked);
                obj.selectedAnswerLabel.Click += new EventHandler(SingleQuizStatusPanelClicked);

                bool isActiveQuiz = (i == currentQuizNumber);
                obj.SetActiveQuiz(isActiveQuiz);

                quizListPanel.Controls.Add(obj);
                singleQuizStatusPanelArray[i] = obj;
            }
            
        }

        void PerformUserSelectChoiceAction(int quizNumber, int choiceNumber)
        {
            SingleQuizObject quizObject = quizArray[quizNumber];
            quizObject.selectedChoice = choiceNumber;

            SingleQuizStatusPanel singleQuizPanel = singleQuizStatusPanelArray[quizNumber];
            singleQuizPanel.selectedAnswerLabel.Text = LocalizedTextManager.GetLocalizedTextForKey("QuizChoicePanel.ChoiceHeader." + (choiceNumber + 1));

            userHasTappedChoice = true;
        }

        void GoToQuiz(int newQuizNumber)
        {
            if (newQuizNumber != currentQuizNumber)
            {
                SingleQuizStatusPanel oldObj = (SingleQuizStatusPanel)singleQuizStatusPanelArray[currentQuizNumber];
                SingleQuizStatusPanel newObj = (SingleQuizStatusPanel)singleQuizStatusPanelArray[newQuizNumber];

                if (modeShowAnswer)
                {
                    
                }
                else
                {
                    oldObj.SetActiveQuiz(false);
                    newObj.SetActiveQuiz(true);
                }


                currentQuizNumber = newQuizNumber;

                SetContentForQuizPanel(currentQuizNumber);
            }
        }

        void GoToNextUnansweredQuiz()
        {
            bool shouldStop = false;
            int counter = 0;
            int index = currentQuizNumber;
            int nextUnansweredQuizNumber = -1;
            while (!shouldStop)
            {
                index = (index == quizArray.Length - 1) ? 0 : (index + 1);
                SingleQuizObject quizObj = quizArray[index];
                if (quizObj.selectedChoice == -1)
                {
                    nextUnansweredQuizNumber = index;
                    shouldStop = true;
                }

                counter++;
                if (counter > quizArray.Length)
                {
                    shouldStop = true;
                }
            }

            if (nextUnansweredQuizNumber != -1)
            {
                GoToQuiz(nextUnansweredQuizNumber);
            }                   
        }

        void GoToFormExamResult()
        {
            stopwatch.stopRunning();
            FormExamResult instanceFormExamResult = FormsManager.GetFormExamResult();
            instanceFormExamResult.Visible = true;
            instanceFormExamResult.BringToFront();
            instanceFormExamResult.calculateScore();

            this.Visible = false;
            fadeForm.Visible = false;
            timeoutMessageBox.Visible = false;
            quizNotCompletedMessageBox.Visible = false;
        }

        void GoToFormChooseLanguage()
        {
            stopwatch.stopRunning();
            FormChooseLanguage instanceFormChooseLanguage = FormsManager.GetFormChooseLanguage();
            instanceFormChooseLanguage.Visible = true;
            instanceFormChooseLanguage.BringToFront();

            this.Visible = false;
            fadeForm.Visible = false;
            timeoutMessageBox.Visible = false;
            quizNotCompletedMessageBox.Visible = false;
        }

        public void ShowAnswer()
        {
            modeShowAnswer = true;

            for (int i = 0; i < singleQuizStatusPanelArray.Length; i++)
            {
                SingleQuizStatusPanel panelObj = singleQuizStatusPanelArray[i];
                SingleQuizObject quizObj = quizArray[i];

                if (quizObj.selectedChoice == quizObj.correctChoice)
                {
                    panelObj.selectedAnswerLabel.BackColor = correctAnswerColor;
                    panelObj.numberLabel.BackColor = correctAnswerColor;
                }
                else
                {
                    panelObj.selectedAnswerLabel.BackColor = wrongAnswerColor;
                    panelObj.numberLabel.BackColor = wrongAnswerColor;
                }
            }

            submitExamButton.Text = LocalizedTextManager.GetLocalizedTextForKey("FormExamResult.FinishButton");
            GoToQuiz(0);
        }

        void TimeoutMessageBoxRightButtonClicked(object sender, EventArgs e)
        {
            GoToFormExamResult();
        }

        void QuizNotCompletedMessageBoxRightButtonClicked(object sender, EventArgs e)
        {
            GoToFormExamResult();
        }

        void QuizNotCompletedMessageBoxLeftButtonClicked(object sender, EventArgs e)
        {
            quizNotCompletedMessageBox.Visible = false;
            fadeForm.Visible = false;
            this.Visible = true;
            this.Enabled = true;
            this.BringToFront();
        }

        void ChoicePanelClicked(object sender, EventArgs e)
        {
            if (modeShowAnswer)
            {
                return;
            }
            
            Control obj = (Control)sender;
            int newChoiceNumber = (int)obj.Tag;
            SingleQuizObject currentQuizObject = quizArray[currentQuizNumber];
            int currentChoiceNumber = currentQuizObject.selectedChoice;
            if (currentChoiceNumber == -1)
            {
                QuizChoicePanel newObj = (QuizChoicePanel)choicePanelArray[newChoiceNumber];
                newObj.SetSelectedChoicePanel(true);
            }
            else if (newChoiceNumber != currentChoiceNumber)
            {
                QuizChoicePanel oldObj = (QuizChoicePanel)choicePanelArray[currentChoiceNumber];
                QuizChoicePanel newObj = (QuizChoicePanel)choicePanelArray[newChoiceNumber];
                oldObj.SetSelectedChoicePanel(false);
                newObj.SetSelectedChoicePanel(true);
            }

            PerformUserSelectChoiceAction(currentQuizNumber, newChoiceNumber);        
        }

        void SingleQuizStatusPanelClicked(object sender, EventArgs e)
        {
            Label obj = (Label)sender;
            int newQuizNumber = (int)obj.Tag;
            GoToQuiz(newQuizNumber);
        }

        void PrevQuizButtonClicked(object sender, EventArgs e)
        {
            if (currentQuizNumber > 0)
            {
                GoToQuiz(currentQuizNumber - 1);
            }
        }

        void NextQuizButtonClicked(object sender, EventArgs e)
        {
            if (currentQuizNumber < quizArray.Length - 1)
            {
                GoToQuiz(currentQuizNumber + 1);
            }
        }

        void SubmitExamButtonClicked(object sender, EventArgs e)
        {
            if (modeShowAnswer)
            {
                modeShowAnswer = false;
                GoToFormChooseLanguage();
            }
            else
            {
                bool canProceed = true;
                for (int i = 0; i < quizArray.Length; i++)
                {
                    SingleQuizObject obj = quizArray[i];
                    if (obj.selectedChoice == -1)
                    {
                        canProceed = false;
                        break;
                    }
                }
                if (canProceed)
                {
                    GoToFormExamResult();
                }
                else
                {
                    fadeForm.Visible = true;
                    fadeForm.BringToFront();

                    quizNotCompletedMessageBox.Visible = true;
                    quizNotCompletedMessageBox.BringToFront();
                    quizNotCompletedMessageBox.Location = new Point((SCREEN_WIDTH - quizNotCompletedMessageBox.Width) / 2,
                                                                        (SCREEN_HEIGHT - quizNotCompletedMessageBox.Height) / 2);
                }  
            }
        }

        protected void StopwatchHasChanged(string newTime)
        {
            if (newTime != null)
            {
                timerLabel.Text = newTime;
            }
            else
            {
                if (!timeoutMessageBox.Visible)
                {                    
                    fadeForm.Visible = true;
                    fadeForm.BringToFront();

                    timeoutMessageBox.Visible = true;
                    timeoutMessageBox.BringToFront();
                    timeoutMessageBox.Location = new Point((SCREEN_WIDTH - timeoutMessageBox.Width) / 2,
                          (SCREEN_HEIGHT - timeoutMessageBox.Height) / 2);
                    this.Enabled = false;                  
                }
            }
        }

        protected void SignalClockHasChanged(int state)
        {
            if (userHasTappedChoice)
            {
                if (signalClockState == -1)
                {
                    signalClockState = state;
                }
                else
                {
                    if (signalClockState == state)
                    {
                        
                        signalClockState = -1;
                        userHasTappedChoice = false;
                        GoToNextUnansweredQuiz();
                    }
                }
            }
        }
    }
}
