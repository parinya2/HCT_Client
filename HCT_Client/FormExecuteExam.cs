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
        Panel[] singleQuizStatusPanelArray;

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

            prepareQuizListPanelUI();

            monitorPanel.Controls.Add(photoLabel);
            monitorPanel.Controls.Add(usernameLabel);
            monitorPanel.Controls.Add(timerLabel);
            monitorPanel.Controls.Add(quizListPanel);

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
                SingleQuizStatusPanel obj = new SingleQuizStatusPanel(i + 1);

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

        void SingleQuizStatusPanelClicked(object sender, EventArgs e)
        {
            Label obj = (Label)sender;
            int newQuizNumber = (int)obj.Tag - 1;
            if (newQuizNumber != currentQuizNumber)
            {
                SingleQuizStatusPanel oldObj = (SingleQuizStatusPanel)singleQuizStatusPanelArray[currentQuizNumber];
                SingleQuizStatusPanel newObj = (SingleQuizStatusPanel)singleQuizStatusPanelArray[newQuizNumber];
                oldObj.SetActiveQuiz(false);
                newObj.SetActiveQuiz(true);

                currentQuizNumber = newQuizNumber;
           
            }
         
        }

        protected void TimeHasChanged(string newTime)
        {
            timerLabel.Text = newTime;
        }
    }
}
