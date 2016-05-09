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
    public partial class FormExamResult : FixedSizeForm
    {
        public LargeButton finishExamButton;
        public LargeButton viewAnswerButton;
        public BaseTextLabel passOrFailLabel;
        public BaseTextLabel scoreLabel;
        BlinkButtonSignalClock blinkButtonSignalClock;
        int SCORE_TO_PASS = 40;

        public FormExamResult()
        {
            InitializeComponent();
            
            RenderUI();

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
            passOrFailLabel.Location = new Point(0, 150);
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
        }

        public void calculateScore()
        {
            int score = 0;
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

        void FinishExamButtonClicked(object sender, EventArgs e)
        {
            FormChooseLanguage instanceFormChooseLanguage = FormsManager.GetFormChooseLanguage();
            this.Visible = false;
            instanceFormChooseLanguage.Visible = true;
            instanceFormChooseLanguage.BringToFront();
        }

        void ViewAnswerButtonClicked(object sender, EventArgs e)
        {
            FormExecuteExam instanceFormExecuteExam = FormsManager.GetFormExecuteExam();
            this.Visible = false;
            instanceFormExecuteExam.ShowAnswer();
            instanceFormExecuteExam.Visible = true;
            instanceFormExecuteExam.BringToFront();
        }

        protected void BlinkButtonSignalClockHasChanged(int state)
        {
            finishExamButton.BackColor = Util.GetButtonBlinkColorAtSignalState(state);
        }
    }
}