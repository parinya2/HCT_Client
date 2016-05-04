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
        int QUIZ_COUNT = 50;
        int currentQuizNumber = 0;
        Panel monitorPanel;
        Panel quizListPanel;
        SingleQuizStatusPanel[] singleQuizStatusPanelArray;

        Panel quizPanel;
        BaseTextLabel usernameLabel;
        BaseImageLabel photoLabel;
        BaseTextLabel timerLabel;
        Stopwatch stopwatch;

        BaseTextLabel quizTextLabel;
        BaseImageLabel quizImageLabel;
        QuizChoicePanel[] choicePanelArray;

        QuizManager quizManager;
        SingleQuizObject[] quizArray;

        public FormExecuteExam()
        {
            InitializeComponent();
            quizManager = new QuizManager();
            RenderUI();

            quizManager.LoadQuiz();
            quizArray = quizManager.GetQuizArray();
            SetContentForQuizPanel(0);
        }

        void RenderUI()
        {
            RenderMonitorPanelUI();
            RenderQuizPanelUI();

            stopwatch = new Stopwatch(100);
            stopwatch.TheTimeChanged += new Stopwatch.TimerTickHandler(TimeHasChanged);
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

            quizImageLabel = new BaseImageLabel();
            quizImageLabel.Width = quizTextLabel.Width;
            quizImageLabel.Height = 300;
            quizImageLabel.Location = new Point(quizTextLabel.Location.X, 
                quizTextLabel.Location.Y + quizTextLabel.Height + quizTextLabelOffsetY);
            quizImageLabel.BackColor = Color.Transparent;

            int choiceLabelOffsetX = 20;
            int choiceLabelOffsetY = 20;
            choicePanelArray = new QuizChoicePanel[4];
            for (int i = 0; i < choicePanelArray.Length; i++)
            {
                int objWidth = (quizPanel.Width - (choiceLabelOffsetX * 3)) / 2;
                int objHeight = (quizPanel.Height - quizImageLabel.Location.Y - quizImageLabel.Height - (choiceLabelOffsetY * 3)) / 2;
               
                QuizChoicePanel obj = new QuizChoicePanel(objWidth, objHeight, i);
                switch (i) 
                {
                    case 0:
                        obj.Location = new Point(choiceLabelOffsetX,
                                            quizImageLabel.Location.Y + quizImageLabel.Height + choiceLabelOffsetY);
                        break;
                    case 1:
                        obj.Location = new Point(obj.Width + (choiceLabelOffsetX * 2),
                                            quizImageLabel.Location.Y + quizImageLabel.Height + choiceLabelOffsetY);
                        break;
                    case 2:
                        obj.Location = new Point(choiceLabelOffsetX,
                                            quizImageLabel.Location.Y + quizImageLabel.Height + obj.Height + (choiceLabelOffsetY * 2));
                        break;
                    case 3:
                        obj.Location = new Point(obj.Width + (choiceLabelOffsetX * 2),
                                            quizImageLabel.Location.Y + quizImageLabel.Height + obj.Height + (choiceLabelOffsetY * 2));
                        break;
                }

                obj.choiceHeaderLabel.Click += new EventHandler(ChoicePanelClicked);
                obj.choiceTextLabel.Click += new EventHandler(ChoicePanelClicked);
                obj.choiceCorrectStatusImage.Click += new EventHandler(ChoicePanelClicked);

                choicePanelArray[i] = obj;
                quizPanel.Controls.Add(obj);
            }

            quizPanel.Controls.Add(quizImageLabel);
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

            prepareQuizListPanelUI();

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
            for (int i = 0; i < quizObj.choiceTextArray.Length; i++)
            {
                string choiceText = quizObj.choiceTextArray[i];
                choicePanelArray[i].SetChoiceText(choiceText);

                bool isSelectedChoiceFlag = (i == quizObj.selectedChoice);
                choicePanelArray[i].SetSelectedChoicePanel(isSelectedChoiceFlag);
            }
        }

        private void prepareQuizListPanelUI()
        {
            quizListPanel = new Panel();
            quizListPanel.Location = new Point(0, timerLabel.Location.Y + timerLabel.Height);
            quizListPanel.Width = monitorPanel.Width;
            quizListPanel.Height = SCREEN_HEIGHT - quizListPanel.Location.Y - 50;

            singleQuizStatusPanelArray = new SingleQuizStatusPanel[QUIZ_COUNT];
            int quizPerColumn = 10;
            int columnCount = QUIZ_COUNT / quizPerColumn;
            int quizGapY = 10;
            int quizGapX = 10;

            for (int i = 0; i < singleQuizStatusPanelArray.Length; i++)
            {
                SingleQuizStatusPanel obj = new SingleQuizStatusPanel(i);

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
        }

        void ChoicePanelClicked(object sender, EventArgs e)
        {
            Label obj = (Label)sender;
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
            if (newQuizNumber != currentQuizNumber)
            {
                SingleQuizStatusPanel oldObj = (SingleQuizStatusPanel)singleQuizStatusPanelArray[currentQuizNumber];
                SingleQuizStatusPanel newObj = (SingleQuizStatusPanel)singleQuizStatusPanelArray[newQuizNumber];
                oldObj.SetActiveQuiz(false);
                newObj.SetActiveQuiz(true);

                currentQuizNumber = newQuizNumber;

                SetContentForQuizPanel(currentQuizNumber);
            }
        }

        protected void TimeHasChanged(string newTime)
        {
            timerLabel.Text = newTime;
        }
    }
}
