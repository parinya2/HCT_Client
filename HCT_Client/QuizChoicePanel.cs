using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace HCT_Client
{
    public class QuizChoicePanel : Panel
    {
        public BaseTextLabel choiceHeaderLabel;
        public BaseTextLabel choiceTextLabel;
        public PictureBox choiceCorrectStatusPictureBox;
        public PictureBox choiceImagePictureBox;
        public PictureBox choiceSoundPictureBox;
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

            this.Tag = this.choiceNumber;

            choiceHeaderLabel = new BaseTextLabel();
            choiceHeaderLabel.Location = new Point(0, 0);
            choiceHeaderLabel.Width = (int)(this.Width * 0.1);
            choiceHeaderLabel.Height = choiceHeaderLabel.Width;;
            choiceHeaderLabel.Text = LocalizedTextManager.GetLocalizedTextForKey("QuizChoicePanel.ChoiceHeader." + (this.choiceNumber + 1)) + ". ";
            choiceHeaderLabel.TextAlign = ContentAlignment.MiddleRight;
            choiceHeaderLabel.BackColor = GlobalColor.orangeColor;
            choiceHeaderLabel.Tag = this.choiceNumber;

            choiceTextLabel = new BaseTextLabel();
            choiceTextLabel.Location = new Point(choiceHeaderLabel.Width + choiceHeaderLabel.Location.X, choiceHeaderLabel.Location.Y);
            choiceTextLabel.Width = (int)(this.Width * 0.75);
            choiceTextLabel.Height = this.Height;
            choiceTextLabel.BackColor = Color.Transparent;
            choiceTextLabel.Tag = this.choiceNumber;
            choiceTextLabel.TextAlign = ContentAlignment.TopLeft;

            choiceCorrectStatusPictureBox = new PictureBox();
            choiceCorrectStatusPictureBox.Location = new Point(choiceHeaderLabel.Location.X + choiceHeaderLabel.Width + choiceTextLabel.Width, choiceHeaderLabel.Location.Y);
            choiceCorrectStatusPictureBox.Width = this.Width - choiceHeaderLabel.Width - choiceTextLabel.Width;
            choiceCorrectStatusPictureBox.Height = this.Height;
            choiceCorrectStatusPictureBox.BackColor = Color.Transparent;
            choiceCorrectStatusPictureBox.Tag = this.choiceNumber;
            choiceCorrectStatusPictureBox.SizeMode = PictureBoxSizeMode.Zoom;

            choiceImagePictureBox = new PictureBox();
            choiceImagePictureBox.BackColor = Color.Transparent;
            choiceImagePictureBox.Tag = this.choiceNumber;
            choiceImagePictureBox.SizeMode = PictureBoxSizeMode.Zoom;

            Bitmap soundIconBitmap = Util.GetImageFromImageResources("SoundIcon.png");

            choiceSoundPictureBox = new PictureBox();
            choiceSoundPictureBox.Width = 50;
            choiceSoundPictureBox.Height = choiceSoundPictureBox.Width;
            choiceSoundPictureBox.Location = new Point(margin / 2, this.Height - choiceSoundPictureBox.Height - margin / 2);
            choiceSoundPictureBox.BackColor = Color.Transparent;
            choiceSoundPictureBox.Tag = this.choiceNumber;
            choiceSoundPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            choiceSoundPictureBox.Image = soundIconBitmap;

            this.BackColor = panelColor;
            this.Controls.Add(choiceSoundPictureBox);
            this.Controls.Add(choiceHeaderLabel);
            this.Controls.Add(choiceTextLabel);
            this.Controls.Add(choiceCorrectStatusPictureBox);
            this.Controls.Add(choiceImagePictureBox);
        }

        private void RefreshUI()
        {
            if (choiceImagePictureBox.Image == null)
            {
                choiceHeaderLabel.Height = choiceHeaderLabel.Width;
                choiceTextLabel.Height = this.Height;
                choiceTextLabel.TextAlign = ContentAlignment.MiddleCenter;
                choiceImagePictureBox.Height = 0;
                choiceImagePictureBox.Width = choiceImagePictureBox.Height;
            }
            else
            {
                choiceHeaderLabel.Height = choiceHeaderLabel.Width;
                choiceTextLabel.Height = (int)(this.Height * 0.2);

                choiceImagePictureBox.Height = this.Height - choiceHeaderLabel.Height - margin;
                choiceImagePictureBox.Width = choiceImagePictureBox.Height;
                choiceImagePictureBox.Location = new Point(this.Width - choiceCorrectStatusPictureBox.Width - margin - choiceImagePictureBox.Width,
                                                            choiceHeaderLabel.Height + margin / 2);
            }
        }

        public void SetSelectedChoicePanel(bool flag)
        {
            Color BGcolor = flag ? GlobalColor.orangeColor : Color.Black;
            Color textColor = flag ? Color.Black : Color.White;

            this.BackColor = BGcolor;

            choiceHeaderLabel.ForeColor = Color.Black;
            choiceTextLabel.ForeColor = textColor;
        }

        public void SetChoiceTextAndImage(string text, Bitmap image)
        {
            choiceTextLabel.Text = text;
            choiceImagePictureBox.Image = image;
            RefreshUI();
        }
    }
}
