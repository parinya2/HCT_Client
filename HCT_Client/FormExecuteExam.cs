using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace HCT_Client
{
    public enum ExamState
    { 
        TakingExamState,
        ShowAnswerState
    }

    public partial class FormExecuteExam : FixedSizeForm
    {
        int QUIZ_COUNT = 50;
        int TOTAL_EXAM_TIME_SECONDS = 3600;
        int currentQuizNumber = 0;
        int totalAnsweredCount = 0;
        Panel monitorPanel;
        Panel quizListPanel;
        SingleQuizStatusPanel[] singleQuizStatusPanelArray;

        Panel quizPanel;
        BaseTextLabel usernameLabel;
        BaseTextLabel examCourseLabel;
        BaseTextLabel questionCountLabel;
        PictureBox userPhotoPictureBox;
        BaseTextLabel timerLabel;
        Stopwatch stopwatch;
        SignalClock signalClock;
        BlinkButtonSignalClock blinkButtonSignalClock;

        BaseTextLabel quizTextLabel;
        PictureBox quizImagePictureBox;
        PictureBox quizSoundPictureBox;
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

        public ExamState examState;

        bool userHasTappedChoice = false;
        int signalClockState = -1;

        Bitmap correctSignBitmap;
        Bitmap wrongSignBitmap;
        Bitmap soundIconBitmap;

        public FormExecuteExam()
        {
            InitializeComponent();

            RenderUI();

            timeoutMessageBox = new FormLargeMessageBox(0);
            timeoutMessageBox.Visible = false;
            timeoutMessageBox.rightButton.Click += new EventHandler(TimeoutMessageBoxRightButtonClicked);

            quizNotCompletedMessageBox = new FormLargeMessageBox(0);
            quizNotCompletedMessageBox.Visible = false;
            quizNotCompletedMessageBox.rightButton.Click += new EventHandler(QuizNotCompletedMessageBoxRightButtonClicked);

            submitExamButton.Click += new EventHandler(SubmitExamButtonClicked);

            fadeForm = FormsManager.GetFormFadeView();

            signalClock = new SignalClock(30);
            signalClock.TheTimeChanged += new SignalClock.SignalClockTickHandler(SignalClockHasChanged);

            blinkButtonSignalClock = new BlinkButtonSignalClock();
            blinkButtonSignalClock.TheTimeChanged += new BlinkButtonSignalClock.BlinkButtonSignalClockTickHandler(BlinkButtonSignalClockHasChanged); 
        }

        void RenderUI()
        {
            correctSignBitmap = Util.GetImageFromImageResources("CorrectSign.png");
            wrongSignBitmap = Util.GetImageFromImageResources("WrongSign.png");
            soundIconBitmap = Util.GetImageFromImageResources("SoundIcon.png");

            RenderMonitorPanelUI();
            RenderQuizPanelUI();
       }

        public void RefreshUI()
        {
            userPhotoPictureBox.Image = UserProfileManager.GetUserPhoto();

            usernameLabel.Text = UserProfileManager.GetFullnameTH();

            timerLabel.Text = "";
            timeoutMessageBox.messageLabel.Text = LocalizedTextManager.GetLocalizedTextForKey("TimeoutMessageBox.Message");
            timeoutMessageBox.rightButton.Text = LocalizedTextManager.GetLocalizedTextForKey("TimeoutMessageBox.RightButton");

            examCourseLabel.Text = LocalizedTextManager.GetLocalizedTextForKey("FormChooseExamCourse.Button." + QuizManager.GetExamCourseType());

            quizNotCompletedMessageBox.messageLabel.Text = LocalizedTextManager.GetLocalizedTextForKey("QuizNotCompletedMessageBox.Message");
            quizNotCompletedMessageBox.rightButton.Text = LocalizedTextManager.GetLocalizedTextForKey("QuizNotCompletedMessageBox.RightButton");

            submitExamButton.Text = LocalizedTextManager.GetLocalizedTextForKey("FormExecuteExam.SubmitExamButton");
            submitExamButton.BackColor = Color.White;

            for (int i = 0; i < singleQuizStatusPanelArray.Length; i++)
            {
                SingleQuizStatusPanel obj = singleQuizStatusPanelArray[i];
                obj.selectedAnswerLabel.Text = "-";
                obj.selectedAnswerLabel.BackColor = Color.Transparent;
                if (i == 0)
                {
                    obj.SetQuizState(SingleQuizStatusPanel_State.Active);
                }                   
                else
                {
                    obj.SetQuizState(SingleQuizStatusPanel_State.Unanswered);  
                }
            }

            for (int i = 0; i < choicePanelArray.Length; i++)
            {
                QuizChoicePanel obj = choicePanelArray[i];
                obj.choiceHeaderLabel.Text = LocalizedTextManager.GetLocalizedTextForKey("QuizChoicePanel.ChoiceHeader." + (i + 1)) + ". ";
                obj.choiceCorrectStatusPictureBox.Image = null;
            }

            currentQuizNumber = 0;
            signalClockState = -1;
            totalAnsweredCount = 0;
        }

        public void LoadExamData()
        {            
            quizArray = QuizManager.GetQuizArray();
            SetContentForQuizPanel(0);
            QuizManager.SetExamStartDateTime(DateTime.Now);

            stopwatch = new Stopwatch(TOTAL_EXAM_TIME_SECONDS);
            stopwatch.TheTimeChanged += new Stopwatch.TimerTickHandler(StopwatchHasChanged);
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
            quizTextLabel.Height = 70;
            quizTextLabel.Location = new Point(quizTextLabelOffsetX, quizTextLabelOffsetY);
            quizTextLabel.ForeColor = Color.Black;

            quizImagePictureBox = new PictureBox();
            quizImagePictureBox.Width = quizTextLabel.Width;
            quizImagePictureBox.Height = 300;
            quizImagePictureBox.Location = new Point(quizTextLabel.Location.X, 
                quizTextLabel.Location.Y + quizTextLabel.Height + quizTextLabelOffsetY);
            quizImagePictureBox.BackColor = Color.Transparent;

            quizSoundPictureBox = new PictureBox();
            quizSoundPictureBox.Width = 50;
            quizSoundPictureBox.Height = quizSoundPictureBox.Width;
            quizSoundPictureBox.Location = new Point(quizTextLabel.Location.X,
                quizTextLabel.Location.Y + quizTextLabel.Height + quizTextLabelOffsetY);
            quizSoundPictureBox.BackColor = Color.Transparent;
            quizSoundPictureBox.Image = soundIconBitmap;
            quizSoundPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            quizSoundPictureBox.Click += new EventHandler(QuizSoundPictureBoxClicked);

            int choiceLabelOffsetX = 5;
            int choiceLabelOffsetY = 5;
            choicePanelArray = new QuizChoicePanel[4];
            for (int i = 0; i < choicePanelArray.Length; i++)
            {
                int objWidth = (quizPanel.Width - (choiceLabelOffsetX * 3)) / 2;
                int objHeight = (quizPanel.Height - quizImagePictureBox.Location.Y - quizImagePictureBox.Height - (choiceLabelOffsetY * 8)) / 2;
               
                QuizChoicePanel obj = new QuizChoicePanel(objWidth, objHeight, i);
                switch (i) 
                {
                    case 0:
                        obj.Location = new Point(choiceLabelOffsetX,
                                            quizImagePictureBox.Location.Y + quizImagePictureBox.Height + choiceLabelOffsetY * 6);
                        break;
                    case 1:
                        obj.Location = new Point(obj.Width + (choiceLabelOffsetX * 2),
                                            quizImagePictureBox.Location.Y + quizImagePictureBox.Height + choiceLabelOffsetY * 6);
                        break;
                    case 2:
                        obj.Location = new Point(choiceLabelOffsetX,
                                            quizImagePictureBox.Location.Y + quizImagePictureBox.Height + obj.Height + (choiceLabelOffsetY * 7));
                        break;
                    case 3:
                        obj.Location = new Point(obj.Width + (choiceLabelOffsetX * 2),
                                            quizImagePictureBox.Location.Y + quizImagePictureBox.Height + obj.Height + (choiceLabelOffsetY * 7));
                        break;
                }

                obj.choiceHeaderLabel.Click += new EventHandler(ChoicePanelClicked);
                obj.choiceTextLabel.Click += new EventHandler(ChoicePanelClicked);
                obj.choiceCorrectStatusPictureBox.Click += new EventHandler(ChoicePanelClicked);
                obj.choiceImagePictureBox.Click += new EventHandler(ChoicePanelClicked);
                obj.choiceSoundPictureBox.Click += new EventHandler(ChoiceSoundPictureBoxClicked);
                obj.Click += new EventHandler(ChoicePanelClicked);

                choicePanelArray[i] = obj;
                quizPanel.Controls.Add(obj);
            }

            quizPanel.Controls.Add(quizSoundPictureBox);
            quizPanel.Controls.Add(quizImagePictureBox);
            quizPanel.Controls.Add(quizTextLabel);
            
            this.Controls.Add(quizPanel);
        }

        private void RenderMonitorPanelUI()
        {
            monitorPanel = new Panel();
            monitorPanel.Width = (int)(SCREEN_WIDTH * 0.35);
            monitorPanel.Height = SCREEN_HEIGHT;
            monitorPanel.BackColor = Color.Black;
            monitorPanel.Location = new Point(0, 0);

            userPhotoPictureBox = new PictureBox();
            userPhotoPictureBox.Width = 120;
            userPhotoPictureBox.Height = 120;
            userPhotoPictureBox.Location = new Point(40, 40);
            userPhotoPictureBox.SizeMode = PictureBoxSizeMode.Zoom;

            usernameLabel = new BaseTextLabel();
            usernameLabel.Width = monitorPanel.Width - userPhotoPictureBox.Width - userPhotoPictureBox.Location.X - 10;
            usernameLabel.Location = new Point(userPhotoPictureBox.Location.X + userPhotoPictureBox.Width + 10,
                                               userPhotoPictureBox.Location.Y);

            examCourseLabel = new BaseTextLabel();
            examCourseLabel.Width = usernameLabel.Width;
            examCourseLabel.Text = LocalizedTextManager.GetLocalizedTextForKey("FormChooseExamCourse.Button." + QuizManager.GetExamCourseType());
            examCourseLabel.Location = new Point(usernameLabel.Location.X,
                                                 usernameLabel.Location.Y + usernameLabel.Height + 20);
            examCourseLabel.Font = new Font(this.Font.FontFamily, 13);

            questionCountLabel = new BaseTextLabel();
            questionCountLabel.Width = usernameLabel.Width;
            questionCountLabel.Location = new Point(usernameLabel.Location.X,
                                                 examCourseLabel.Location.Y + examCourseLabel.Height + 20);
            questionCountLabel.Font = new Font(this.Font.FontFamily, 13);
            
            string answeredCountText = LocalizedTextManager.GetLocalizedTextForKey("FormExecuteExam.AnsweredCount");
            answeredCountText = answeredCountText.Replace(LocalizedTextManager.PARAM_1, totalAnsweredCount + "");
            answeredCountText = answeredCountText.Replace(LocalizedTextManager.PARAM_2, (QuizManager.GetQuizArray().Length - totalAnsweredCount) + "");
            questionCountLabel.Text = answeredCountText;


            timerLabel = new BaseTextLabel();
            timerLabel.Text = "";
            timerLabel.Location = new Point(userPhotoPictureBox.Location.X,
                                            userPhotoPictureBox.Location.Y + userPhotoPictureBox.Height + 20);
            timerLabel.Width = monitorPanel.Width - (userPhotoPictureBox.Location.X * 2);

            submitExamButton = new MediumButton();
            submitExamButton.Location = new Point(monitorPanel.Width - 20 - submitExamButton.Width, 
                                                  monitorPanel.Height - 20 - submitExamButton.Height);
            submitExamButton.Text = LocalizedTextManager.GetLocalizedTextForKey("FormExecuteExam.SubmitExamButton");

            prepareQuizListPanelUI();

            prevQuizButton = new MediumButton();
            prevQuizButton.Width = 120;
            prevQuizButton.Height = submitExamButton.Height;
            prevQuizButton.Location = new Point(timerLabel.Location.X, submitExamButton.Location.Y);
            prevQuizButton.Text = LocalizedTextManager.GetLocalizedTextForKey("FormExecuteExam.PrevQuiz");
            prevQuizButton.Click += new EventHandler(PrevQuizButtonClicked);

            nextQuizButton = new MediumButton();
            nextQuizButton.Width = 120;
            nextQuizButton.Height = submitExamButton.Height;
            nextQuizButton.Location = new Point(prevQuizButton.Location.X + prevQuizButton.Width + 5, submitExamButton.Location.Y);
            nextQuizButton.Text = LocalizedTextManager.GetLocalizedTextForKey("FormExecuteExam.NextQuiz");
            nextQuizButton.Click += new EventHandler(NextQuizButtonClicked);

            monitorPanel.Controls.Add(prevQuizButton);
            monitorPanel.Controls.Add(nextQuizButton);
            monitorPanel.Controls.Add(submitExamButton);
            monitorPanel.Controls.Add(userPhotoPictureBox);
            monitorPanel.Controls.Add(usernameLabel);
            monitorPanel.Controls.Add(examCourseLabel);
            monitorPanel.Controls.Add(questionCountLabel);
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

            for (int i = 0; i < quizObj.choiceObjArray.Length; i++)
            {
                SingleChoiceObject choiceObj = quizObj.choiceObjArray[i];
                choicePanelArray[i].SetChoiceTextAndImage(choiceObj.choiceText, choiceObj.choiceImage);

                bool isSelectedChoiceFlag = (i == quizObj.selectedChoice);
                choicePanelArray[i].SetSelectedChoicePanel(isSelectedChoiceFlag);
            }

            if (examState == ExamState.ShowAnswerState)
            {
                for (int i = 0; i < quizObj.choiceObjArray.Length; i++)
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
            int quizGapX = 10;
            int quizGapY = 10;

            int singleQuizStatusPanelWidth = 60;
            int singleQuizStatusPanelHeight = (quizListPanel.Height / quizPerColumn) - quizGapY - 5;

            int panelOffsetX = (quizListPanel.Width - (columnCount * singleQuizStatusPanelWidth) - ((columnCount - 1) * quizGapX)) / 2;

            for (int i = 0; i < singleQuizStatusPanelArray.Length; i++)
            {
                SingleQuizStatusPanel obj = new SingleQuizStatusPanel(i, singleQuizStatusPanelWidth, singleQuizStatusPanelHeight);
                
                int columnNo = i / quizPerColumn;
                int rowNo = i % quizPerColumn;

                int locationX = (obj.Width + quizGapX) * columnNo + panelOffsetX;
                int locationY = (obj.Height + quizGapY) * rowNo + 30;
                obj.Location = new Point(locationX, locationY);
                obj.numberLabel.Click += new EventHandler(SingleQuizStatusPanelClicked);
                obj.selectedAnswerLabel.Click += new EventHandler(SingleQuizStatusPanelClicked);

                bool isActiveQuiz = (i == currentQuizNumber);
                if (isActiveQuiz)
                {
                    obj.SetQuizState(SingleQuizStatusPanel_State.Active);
                }
                else
                {
                    obj.SetQuizState(SingleQuizStatusPanel_State.Unanswered);
                }

                quizListPanel.Controls.Add(obj);
                singleQuizStatusPanelArray[i] = obj;
            }
            
        }

        void PerformUserSelectChoiceAction(int quizNumber, int choiceNumber)
        {            
            SingleQuizObject quizObject = quizArray[quizNumber];
            if (quizObject.selectedChoice == -1)
            {
                totalAnsweredCount++;
            }
            quizObject.selectedChoice = choiceNumber;

            SingleQuizStatusPanel singleQuizPanel = singleQuizStatusPanelArray[quizNumber];
            singleQuizPanel.selectedAnswerLabel.Text = LocalizedTextManager.GetLocalizedTextForKey("QuizChoicePanel.ChoiceHeader." + (choiceNumber + 1));

            userHasTappedChoice = true;
            
            string answeredCountText = LocalizedTextManager.GetLocalizedTextForKey("FormExecuteExam.AnsweredCount");
            answeredCountText = answeredCountText.Replace(LocalizedTextManager.PARAM_1, totalAnsweredCount + "");
            answeredCountText = answeredCountText.Replace(LocalizedTextManager.PARAM_2, (QuizManager.GetQuizArray().Length - totalAnsweredCount) + "");
            questionCountLabel.Text = answeredCountText;
        }

        void GoToQuiz(int newQuizNumber)
        {
            if (newQuizNumber != currentQuizNumber)
            {
                SingleQuizStatusPanel oldObj = (SingleQuizStatusPanel)singleQuizStatusPanelArray[currentQuizNumber];
                SingleQuizStatusPanel newObj = (SingleQuizStatusPanel)singleQuizStatusPanelArray[newQuizNumber];

                SingleQuizObject newQuizObj = quizArray[newQuizNumber];

                if (WebServiceManager.QUIZ_STEAL_ENABLED)
                {
                    if (examState == ExamState.ShowAnswerState)
                    {
                        WebServiceManager.GetEExamCorrectAnswerFromServer(newQuizObj.paperQuestSeq);
                    }
                }

                if (examState == ExamState.ShowAnswerState && newQuizObj.correctChoice == -1)
                {
                    FormsManager.GetFormLoadingView().ShowLoadingView(true);
                    BackgroundWorker bw = new BackgroundWorker();
                    bw.DoWork += new DoWorkEventHandler(
                        delegate(object o, DoWorkEventArgs args)
                        {
                            Thread.Sleep(10);
                            string status = WebServiceManager.GetEExamCorrectAnswerFromServer(newQuizObj.paperQuestSeq);

                        }
                     );
                    bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(
                        delegate(object o, RunWorkerCompletedEventArgs args)
                        {
                            FormsManager.GetFormLoadingView().ShowLoadingView(false);
                            currentQuizNumber = newQuizNumber;
                            SetContentForQuizPanel(currentQuizNumber);
                        }
                    );
                    bw.RunWorkerAsync();
                }
                else
                {
                    if (examState == ExamState.TakingExamState)
                    {
                        SingleQuizObject tmpQuizObj = quizArray[currentQuizNumber];
                        if (tmpQuizObj.selectedChoice == -1)
                        {
                            oldObj.SetQuizState(SingleQuizStatusPanel_State.Unanswered);
                        }
                        else
                        {
                            oldObj.SetQuizState(SingleQuizStatusPanel_State.Answered);
                        }
                        newObj.SetQuizState(SingleQuizStatusPanel_State.Active);
                    }
                    currentQuizNumber = newQuizNumber;
                    SetContentForQuizPanel(currentQuizNumber);
                }  
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

        public void GoToFormExamResult()
        {            
            if (examState == ExamState.TakingExamState)
            {
                stopwatch.stopRunning();
                QuizManager.SetExamEndDateTime(DateTime.Now);

                FormsManager.GetFormLoadingView().ShowLoadingView(true);
                BackgroundWorker bw = new BackgroundWorker();
                string status = "";
                bw.DoWork += new DoWorkEventHandler(
                    delegate(object o, DoWorkEventArgs args)
                    {
                        Thread.Sleep(10);
                        status = WebServiceManager.GetEExamResultFromServer();
                    }
                 );
                bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(
                    delegate(object o, RunWorkerCompletedEventArgs args)
                    {
                        FormsManager.GetFormLoadingView().ShowLoadingView(false);
                        if (status.Equals(WebServiceResultStatus.SUCCESS))
                        {
                            FormExamResult instanceFormExamResult = FormsManager.GetFormExamResult();
                            instanceFormExamResult.RefreshUI();
                            instanceFormExamResult.Visible = true;
                            instanceFormExamResult.BringToFront();
                            instanceFormExamResult.displayScore();

                            this.Visible = false;
                            fadeForm.Visible = false;
                            timeoutMessageBox.Visible = false;
                            quizNotCompletedMessageBox.Visible = false;
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
            else
            {
                FormExamResult instanceFormExamResult = FormsManager.GetFormExamResult();
                instanceFormExamResult.RefreshUI();
                instanceFormExamResult.Visible = true;
                instanceFormExamResult.BringToFront();
                instanceFormExamResult.displayScore();

                this.Visible = false;
                fadeForm.Visible = false;
                timeoutMessageBox.Visible = false;
                quizNotCompletedMessageBox.Visible = false;
            }

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
            examState = ExamState.ShowAnswerState;
            this.Enabled = true;
            this.BringToFront();
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

            submitExamButton.Text = LocalizedTextManager.GetLocalizedTextForKey("FormExecuteExam.GoBack");
            GoToQuiz(0);
        }

        void TimeoutMessageBoxRightButtonClicked(object sender, EventArgs e)
        {
            GoToFormExamResult();
        }

        void QuizNotCompletedMessageBoxRightButtonClicked(object sender, EventArgs e)
        {
            quizNotCompletedMessageBox.Visible = false;
            fadeForm.Visible = false;
            this.Visible = true;
            this.Enabled = true;
            this.BringToFront();
        }

        void ChoiceSoundPictureBoxClicked(object sender, EventArgs e)
        {
            Control obj = (Control)sender;
            int choiceNumber = (int)obj.Tag;
            SingleQuizObject quizObj = QuizManager.GetQuizArray()[currentQuizNumber];
            quizObj.choiceObjArray[choiceNumber].choiceSoundPlayer.Play();
        }

        void QuizSoundPictureBoxClicked(object sender, EventArgs e)
        {
            QuizManager.GetQuizArray()[currentQuizNumber].quizSoundPlayer.Play();
        }

        void ChoicePanelClicked(object sender, EventArgs e)
        {
            if (examState == ExamState.ShowAnswerState)
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
            if (examState == ExamState.ShowAnswerState)
            {
               // modeShowAnswer = false;
                GoToFormExamResult();
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
                    quizNotCompletedMessageBox.messageLabel.Text = LocalizedTextManager.GetLocalizedTextForKey("QuizNotCompletedMessageBox.Message") + 
                                                                    GetUnansweredQuestionsString();
                }  
            }
        }

        private string GetUnansweredQuestionsString()
        {
            StringBuilder sb = new StringBuilder("");
            for (int i = 0; i < QuizManager.GetQuizArray().Length; i++)
            {
                SingleQuizObject quizObj = QuizManager.GetQuizArray()[i];
                if (quizObj.selectedChoice == -1)
                {
                    sb.Append(i + 1);
                    sb.Append(",");
                }
            }
            string result = sb.ToString();
            if (result.Contains(","))
            {
                int idx = result.LastIndexOf(',');
                result = result.Substring(0, idx);           
            }

            return result;
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

        protected void BlinkButtonSignalClockHasChanged(int state)
        {
            timeoutMessageBox.rightButton.BackColor = Util.GetButtonBlinkColorAtSignalState(state);
            if (examState == ExamState.ShowAnswerState)
            {
                submitExamButton.BackColor = Util.GetButtonBlinkColorAtSignalState(state);
            }
            else if (examState == ExamState.TakingExamState)
            {
                bool allQuizAnswered = true;
                for (int i = 0; i < quizArray.Length; i++)
                {
                    SingleQuizObject obj = quizArray[i];
                    if (obj.selectedChoice == -1)
                    {
                        allQuizAnswered = false;
                        break;
                    }
                }

                if (allQuizAnswered)
                {
                    submitExamButton.BackColor = Util.GetButtonBlinkColorAtSignalState(state);
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
