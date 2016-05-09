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
        public PictureBox choiceCorrectStatusPictureBox;
        public int choiceNumber;
        int margin = 10;

        public QuizChoicePanel(int width, int height, int choiceNumber)
        {
            this.Width = width;
            this.Height = height;
            this.choiceNumber = choiceNumber;

            RenderUI();
        }

        private void RenderUI()
        {
            Color panelColor = Color.Black;

            choiceHeaderLabel = new BaseTextLabel();
            choiceHeaderLabel.Location = new Point(margin, margin);
            choiceHeaderLabel.Width = (int)(this.Width * 0.1);
            choiceHeaderLabel.Height = this.Height;
            choiceHeaderLabel.Text = LocalizedTextManager.GetLocalizedTextForKey("QuizChoicePanel.ChoiceHeader." + (this.choiceNumber + 1)) + ". ";
            choiceHeaderLabel.BackColor = panelColor;
            choiceHeaderLabel.Tag = this.choiceNumber;

            choiceTextLabel = new BaseTextLabel();
            choiceTextLabel.Location = new Point(choiceHeaderLabel.Width + choiceHeaderLabel.Location.X, choiceHeaderLabel.Location.Y);
            choiceTextLabel.Width = (int)(this.Width * 0.75);
            choiceTextLabel.Height = this.Height;
            choiceTextLabel.BackColor = panelColor;
            choiceTextLabel.Tag = this.choiceNumber;
            choiceTextLabel.TextAlign = ContentAlignment.TopLeft;

            choiceCorrectStatusPictureBox = new PictureBox();
            choiceCorrectStatusPictureBox.Location = new Point(choiceHeaderLabel.Location.X + choiceHeaderLabel.Width + choiceTextLabel.Width, choiceHeaderLabel.Location.Y);
            choiceCorrectStatusPictureBox.Width = this.Width - choiceHeaderLabel.Width - choiceTextLabel.Width - margin * 2;
            choiceCorrectStatusPictureBox.Height = this.Height;
            choiceCorrectStatusPictureBox.BackColor = panelColor;
            choiceCorrectStatusPictureBox.Tag = this.choiceNumber;
            choiceCorrectStatusPictureBox.SizeMode = PictureBoxSizeMode.Zoom;

            this.BackColor = panelColor;
            this.Controls.Add(choiceHeaderLabel);
            this.Controls.Add(choiceTextLabel);
            this.Controls.Add(choiceCorrectStatusPictureBox);
        }

        public void SetSelectedChoicePanel(bool flag)
        {
            Color BGcolor = flag ? Color.Orange : Color.Black;
            Color textColor = flag ? Color.Black : Color.White;

            choiceHeaderLabel.BackColor = BGcolor;
            choiceTextLabel.BackColor = BGcolor;
            choiceCorrectStatusPictureBox.BackColor = BGcolor;
            this.BackColor = BGcolor;

            choiceHeaderLabel.ForeColor = textColor;
            choiceTextLabel.ForeColor = textColor;
        }

        public void SetChoiceText(string text)
        {
            choiceTextLabel.Text = text;
        }
    }
}
