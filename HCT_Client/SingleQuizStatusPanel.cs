using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace HCT_Client
{
    public class SingleQuizStatusPanel : Panel
    {
        public Label numberLabel;
        public Label selectedAnswerLabel;
        public int quizNumber;

        public SingleQuizStatusPanel(int quizNumber, int width, int height)
        {
            this.quizNumber = quizNumber;
            this.Width = width;
            this.Height = height;
            
            numberLabel = new Label();
            numberLabel.Width = (int)(this.Width * 0.55);
            numberLabel.Height = this.Height;
            numberLabel.Text = (quizNumber + 1) + "";
            numberLabel.TextAlign = ContentAlignment.MiddleCenter;
            numberLabel.Font = new Font(this.Font.FontFamily, 11);
            numberLabel.ForeColor = Color.Black;
            numberLabel.Location = new Point(0, 0);
            numberLabel.BackColor = Color.Orange;
            numberLabel.Tag = quizNumber;

            selectedAnswerLabel = new Label();
            selectedAnswerLabel.Width = this.Width - numberLabel.Width;
            selectedAnswerLabel.Height = this.Height;
            selectedAnswerLabel.Text = "-";
            selectedAnswerLabel.TextAlign = ContentAlignment.MiddleCenter;            
            selectedAnswerLabel.Font = new Font(this.Font.FontFamily, 13, FontStyle.Bold);
            selectedAnswerLabel.ForeColor = Color.White;
            selectedAnswerLabel.Location = new Point(numberLabel.Width, 0);
            selectedAnswerLabel.Tag = quizNumber;

            this.Controls.Add(numberLabel);
            this.Controls.Add(selectedAnswerLabel);
        }

        public void SetActiveQuiz(bool flag)
        {
            if (flag)
            {
                numberLabel.BackColor = Color.White;
            }
            else
            {
                numberLabel.BackColor = Color.Orange;
            }
        }
    }
}
