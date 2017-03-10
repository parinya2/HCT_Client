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
    public partial class FormPassport : FixedSizeFormWithHeader
    {
        TextBox passportTextbox;
        Button deleteCharacterButton;

        BlinkButtonSignalClock blinkButtonSignalClock;
        LargeButton loginButton;
        MediumButton backButton;
        BaseTextLabel passportTextLabel;
        FormFadeView fadeForm;
        Button[] characterButtonArray;
        Button[] numberButtonArray;
        int passportTextBoxMaxLength = 16;

        public FormPassport()
        {
            InitializeComponent();            

            RenderUI();
            fadeForm = FormsManager.GetFormFadeView();

            blinkButtonSignalClock = new BlinkButtonSignalClock();
            blinkButtonSignalClock.TheTimeChanged += new BlinkButtonSignalClock.BlinkButtonSignalClockTickHandler(BlinkButtonSignalClockHasChanged);
        }

        private void RenderUI()
        {
            passportTextLabel = new BaseTextLabel();
            passportTextLabel.Width = 350;
            passportTextLabel.Location = new Point(100, headerLineLabel.Location.Y + 60);
            passportTextLabel.TextAlign = ContentAlignment.MiddleCenter;
            passportTextLabel.ForeColor = Color.Black;

            passportTextbox = new TextBox();
            passportTextbox.Width = 430;
            passportTextbox.Height = 100;
            passportTextbox.Font = new Font(this.Font.FontFamily, 22);
            passportTextbox.Location = new Point(passportTextLabel.Location.X + passportTextLabel.Width + 20, 
                                                passportTextLabel.Location.Y - 10);

            deleteCharacterButton = new Button();
            deleteCharacterButton.Width = 120;
            deleteCharacterButton.Height = passportTextbox.Height;
            deleteCharacterButton.TextAlign = ContentAlignment.MiddleCenter;
            deleteCharacterButton.Location = new Point(passportTextbox.Location.X + passportTextbox.Width + 20,
                                                        passportTextbox.Location.Y);
            deleteCharacterButton.Font = new Font(this.Font.FontFamily, 19);
            deleteCharacterButton.Click += new EventHandler(DeleteCharacterButtonClicked);

            backButton = new MediumButton();
            backButton.Location = new Point(SCREEN_WIDTH - backButton.Width - 50,
                                            SCREEN_HEIGHT - backButton.Height - 50);
            backButton.Click += new EventHandler(BackButtonClicked);

            // Initialize Character buttons //
            characterButtonArray = new Button[26];
            string[] characterArray = { "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P", 
                                      "A","S","D","F","G","H","J","K","L",
                                      "Z","X","C","V","B","N","M",};
            for (int i = 0; i < characterButtonArray.Length; i++)
            {
                int xPos = 0, yPos = 0;
                if (i < 10)
                {
                    xPos = i;
                    yPos = 0;
                }
                else if (i >= 10 && i < 19)
                {
                    xPos = i - 10;
                    yPos = 1;
                }
                else
                {
                    xPos = i - 19;
                    yPos = 2;
                }

                int buttonGapX = 7;
                int buttonGapY = 7;
                Button b = new Button();
                b.Height = 50;
                b.Width = (int)(b.Height * 1.3);
                b.Text = characterArray[i];
                b.TextAlign = ContentAlignment.MiddleCenter;
                b.Font = new Font(this.Font.FontFamily, 14);
                b.BackColor = Color.White;
                int offsetX = 120;//(SCREEN_WIDTH - (b.Width * 10) - (buttonGapX * 9)) / 2;
                if (yPos == 1)  offsetX += b.Width / 2 + buttonGapX;
                if (yPos == 2) offsetX += (int)(b.Width * 1.5 + buttonGapX * 2);
                
                int offsetY = passportTextbox.Location.Y + passportTextbox.Height + 30;
                b.Location = new Point(offsetX + (b.Width + buttonGapX) * xPos,
                                       offsetY + (b.Height + buttonGapY) * yPos);
                b.Click += new EventHandler(CharacterButtonArrayClicked);
                characterButtonArray[i] = b;
                this.Controls.Add(b);
            }

            // Initialize Number buttons //
            numberButtonArray = new Button[10];
            string[] numberArray = { "7", "8", "9", "4", "5", "6", "1", "2", "3", "0"};
            for (int i = 0; i < numberButtonArray.Length; i++)
            {
                int xPos = 0, yPos = 0;
                if (i < 3)
                {
                    xPos = i;
                    yPos = 0;
                }
                else if (i >= 3 && i < 6)
                {
                    xPos = i - 3;
                    yPos = 1;
                }
                else if (i >= 6 && i < 9)
                {
                    xPos = i - 6;
                    yPos = 2;
                }
                else
                {
                    xPos = 1;
                    yPos = 3;
                }

                int buttonGapX = 5;
                int buttonGapY = 5;
                Button b = new Button();
                b.Height = 50;
                b.Width = (int)(b.Height * 1.3);
                b.Text = numberArray[i];
                b.TextAlign = ContentAlignment.MiddleCenter;
                b.Font = new Font(this.Font.FontFamily, 14);
                b.BackColor = Color.White;
                int offsetX = characterButtonArray[9].Location.X + characterButtonArray[9].Width + 50;//(SCREEN_WIDTH - (b.Width * 13) - (buttonGapX * 12)) / 2;
                int offsetY = passportTextbox.Location.Y + passportTextbox.Height + 30;
                b.Location = new Point(offsetX + (b.Width + buttonGapX) * xPos,
                                       offsetY + (b.Height + buttonGapY) * yPos);
                b.Click += new EventHandler(CharacterButtonArrayClicked);
                numberButtonArray[i] = b;
                this.Controls.Add(b);
            }

            loginButton = new LargeButton();
            loginButton.Height = 100;
            loginButton.Location = new Point((SCREEN_WIDTH - loginButton.Width) / 2,
                                            characterButtonArray.Last().Location.Y + characterButtonArray.Last().Height + 60);
            loginButton.Click += new EventHandler(LoginButtonClicked);

            this.Controls.Add(passportTextLabel);
            this.Controls.Add(passportTextbox);
            this.Controls.Add(deleteCharacterButton);
            this.Controls.Add(loginButton);
            this.Controls.Add(backButton);
        }

        public void RefreshUI()
        {
            passportTextLabel.Text = LocalizedTextManager.GetLocalizedTextForKey("FormPassport.Passport.Label");
            loginButton.Text = LocalizedTextManager.GetLocalizedTextForKey("FormPassport.Login.Button");
            deleteCharacterButton.Text = LocalizedTextManager.GetLocalizedTextForKey("FormPassport.Delete.Button");
            backButton.Text = LocalizedTextManager.GetLocalizedTextForKey("FormInsertSmartCard.GoBack");
            passportTextbox.Text = "";
            loginButton.Enabled = false;
        }

        void CharacterButtonArrayClicked(object sender, EventArgs e)
        {
            string text = ((Button)sender).Text;
            if (passportTextbox.Text.Length < passportTextBoxMaxLength)
            {
                passportTextbox.Text = passportTextbox.Text + text;
                loginButton.Enabled = true;
            }        
        }

        void LoginButtonClicked(object sender, EventArgs e)
        {
            if (passportTextbox.Text != null && passportTextbox.Text.Length > 0)
            {
                UserProfileManager.SetPassportID(passportTextbox.Text);
                GoToNextForm();
            }            
        }

        void DeleteCharacterButtonClicked(object sender, EventArgs e)
        {            
            string text = passportTextbox.Text;
            if (text.Length == 0) 
                return;
            
            text = text.Substring(0, text.Length - 1);
            passportTextbox.Text = text;
            loginButton.Enabled = !(text == null || text.Length == 0);
        }

        void BackButtonClicked(object sender, EventArgs e)
        {
            GoToPreviousForm();
        }

        private void GoToNextForm()
        {     
            FormChooseExamCourse instanceFormChooseExamCourse = FormsManager.GetFormChooseExamCourse();
            instanceFormChooseExamCourse.Visible = true;
            instanceFormChooseExamCourse.Enabled = true;
            instanceFormChooseExamCourse.RefreshUI();
            instanceFormChooseExamCourse.BringToFront();
            this.Visible = false;
        }

        private void GoToPreviousForm()
        {
            FormInsertSmartCard instanceFormInsertSmartCard= FormsManager.GetFormInsertSmartCard();
            instanceFormInsertSmartCard.Visible = true;
            instanceFormInsertSmartCard.Enabled = true;
            instanceFormInsertSmartCard.BringToFront();
            this.Visible = false;
        }

        protected void BlinkButtonSignalClockHasChanged(int state)
        {
            if (loginButton.Enabled)
            {
                loginButton.BackColor = Util.GetButtonBlinkColorAtSignalState(state);
            }
            else
            {
                loginButton.BackColor = Color.White;
            }
            
        }

    }
}
