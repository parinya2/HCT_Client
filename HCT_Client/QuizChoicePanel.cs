using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace HCT_Client
{
    public class QuizChoicePanel : Panel
    {
        public BaseTextLabel choiceHeaderLabel;
        public BaseTextLabel choiceTextLabel;
        public BaseImageLabel choiceCorrectStatusImage;
        public int choiceNumber;

        public QuizChoicePanel(int width, int height, int choiceNumber)
        {
            this.Width = width;
            this.Height = height;
            this.choiceNumber = choiceNumber;

            RenderUI();
        }

        private void RenderUI()
        {
            choiceHeaderLabel = new BaseTextLabel();
            choiceHeaderLabel.Location = new Point(0, 0);
            choiceHeaderLabel.Width = (int)(this.Width * 0.2);
            choiceHeaderLabel.Height = this.Height;
            choiceHeaderLabel.Text = LocalizedTextManager.GetLocalizedTextForKey("QuizChoicePanel.ChoiceHeader." + (this.choiceNumber + 1)) + ". ";
            choiceHeaderLabel.BackColor = Color.Black;
            choiceHeaderLabel.Tag = this.choiceNumber;

            choiceTextLabel = new BaseTextLabel();
            choiceTextLabel.Location = new Point(choiceHeaderLabel.Width, 0);
            choiceTextLabel.Width = (int)(this.Width * 0.6);
            choiceTextLabel.Height = this.Height;
            choiceTextLabel.BackColor = Color.Black;
            choiceTextLabel.Tag = this.choiceNumber;

            choiceCorrectStatusImage = new BaseImageLabel();
            choiceCorrectStatusImage.Location = new Point(choiceHeaderLabel.Width + choiceTextLabel.Width, 0);
            choiceCorrectStatusImage.Width = this.Width - choiceHeaderLabel.Width - choiceTextLabel.Width;
            choiceCorrectStatusImage.Height = this.Height;
            choiceCorrectStatusImage.BackColor = Color.Black;
            choiceCorrectStatusImage.Tag = this.choiceNumber;

            this.Controls.Add(choiceHeaderLabel);
            this.Controls.Add(choiceTextLabel);
            this.Controls.Add(choiceCorrectStatusImage);
        }

        public void SetSelectedChoicePanel(bool flag)
        {
            Color BGcolor = flag ? Color.Orange : Color.Black;
            Color textColor = flag ? Color.Black : Color.White;

            choiceHeaderLabel.BackColor = BGcolor;
            choiceTextLabel.BackColor = BGcolor;
            choiceCorrectStatusImage.BackColor = BGcolor;

            choiceHeaderLabel.ForeColor = textColor;
            choiceTextLabel.ForeColor = textColor;
        }

        public void SetChoiceText(string text)
        {
            choiceTextLabel.Text = text;
        }
    }
}
