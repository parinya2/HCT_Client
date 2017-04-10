using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

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
            passportTextLabel.Width = 360;
            passportTextLabel.Height = 60;
            passportTextLabel.Location = new Point(370, headerLineLabel.Location.Y + 160);
            passportTextLabel.TextAlign = ContentAlignment.BottomCenter;
            passportTextLabel.ForeColor = Color.Black;
            passportTextLabel.Font = UtilFonts.GetTHSarabunFont(38);

            passportTextbox = new TextBox();
            passportTextbox.Width = 430;
            passportTextbox.Height = 110;
            passportTextbox.Font = UtilFonts.GetTHSarabunFont(38);

            passportTextbox.Location = new Point(passportTextLabel.Location.X + passportTextLabel.Width + 20, 
                                                passportTextLabel.Location.Y);

            deleteCharacterButton = new Button();
            deleteCharacterButton.Width = 150;
            deleteCharacterButton.Height = passportTextbox.Height;
            deleteCharacterButton.TextAlign = ContentAlignment.MiddleCenter;
            deleteCharacterButton.Location = new Point(passportTextbox.Location.X + passportTextbox.Width + 20,
                                                        passportTextbox.Location.Y);
            deleteCharacterButton.Font = UtilFonts.GetTHSarabunFont(30);
            deleteCharacterButton.Click += new EventHandler(DeleteCharacterButtonClicked);
            deleteCharacterButton.ForeColor = Color.White;
            deleteCharacterButton.BackColor = GlobalColor.redColor;

            backButton = new MediumButton();
            backButton.SetLocationForBackButton();
            backButton.Click += new EventHandler(BackButtonClicked);
            backButton.ForeColor = Color.White;
            backButton.BackColor = GlobalColor.redColor;

            // Initialize Character buttons //
            int characterButtonHeight = 70;
            int characterButtonGapX = 7;
            int characterButtonGapY = 7;
            int characterAndNumberGapX = 50;
            Color characterButtonColor = GlobalColor.paleRoseColor;
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

                int buttonGapX = characterButtonGapX;
                int buttonGapY = characterButtonGapY;
                Button b = new Button();
                b.Height = characterButtonHeight;
                b.Width = (int)(b.Height * 1.3);
                b.Text = characterArray[i];
                b.TextAlign = ContentAlignment.MiddleCenter;
                b.Font = UtilFonts.GetTHSarabunFont(33);
                b.BackColor = characterButtonColor;
                int offsetX = (SCREEN_WIDTH - (b.Width * 13) - (buttonGapX * 11) - characterAndNumberGapX) / 2;

                if (yPos == 1)  offsetX += b.Width / 2 + buttonGapX;
                if (yPos == 2) offsetX += (int)(b.Width * 1.5 + buttonGapX * 2);
                
                int offsetY = passportTextbox.Location.Y + passportTextbox.Height + 70;
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

                int buttonGapX = characterButtonGapX;
                int buttonGapY = characterButtonGapY;
                Button b = new Button();
                b.Height = characterButtonHeight;
                b.Width = (int)(b.Height * 1.3);
                b.Text = numberArray[i];
                b.TextAlign = ContentAlignment.MiddleCenter;
                b.Font = UtilFonts.GetTHSarabunFont(33);
                b.BackColor = characterButtonColor;
                int offsetX = characterButtonArray[9].Location.X + characterButtonArray[9].Width + characterAndNumberGapX;//(SCREEN_WIDTH - (b.Width * 13) - (buttonGapX * 12)) / 2;
                int offsetY = passportTextbox.Location.Y + passportTextbox.Height + 70;
                b.Location = new Point(offsetX + (b.Width + buttonGapX) * xPos,
                                       offsetY + (b.Height + buttonGapY) * yPos);
                b.Click += new EventHandler(CharacterButtonArrayClicked);
                numberButtonArray[i] = b;
                this.Controls.Add(b);
            }

            loginButton = new LargeButton();
            loginButton.Font = UtilFonts.GetTHSarabunFont((int)loginButton.Font.Size + 5);
            loginButton.Height = 110;
            loginButton.Location = new Point(SCREEN_WIDTH - loginButton.Width - backButton.Location.X,
                                             backButton.Location.Y);
            loginButton.Click += new EventHandler(LoginButtonClicked);
            loginButton.ForeColor = Color.White;
            loginButton.BackColor = GlobalColor.greenColor;

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
            FormsManager.GetFormLoadingView().ShowLoadingView(true);
            BackgroundWorker bw = new BackgroundWorker();
            string JSONstring = "";
            bw.DoWork += new DoWorkEventHandler(
                delegate(object o, DoWorkEventArgs args)
                {
                    Thread.Sleep(10);
                    JSONstring = WebServiceManager.GetStudentEnrolDetailJSONFromHCTServer();
                }
             );
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(
                delegate(object o, RunWorkerCompletedEventArgs args)
                {
                    FormsManager.GetFormLoadingView().ShowLoadingView(false);
                    UserProfileManager.SetStudentEnrolDetailJSON(JSONstring);
                    Console.WriteLine("JSON = " + JSONstring);
                    FormChooseExamCourse instanceFormChooseExamCourse = FormsManager.GetFormChooseExamCourse();
                    instanceFormChooseExamCourse.Visible = true;
                    instanceFormChooseExamCourse.Enabled = true;
                    instanceFormChooseExamCourse.RefreshUI();
                    instanceFormChooseExamCourse.BringToFront();
                    this.Visible = false;
                }
            );
            bw.RunWorkerAsync();
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
                loginButton.BackColor = Util.GetButtonBlinkColorAtSignalState_Green(state);
            }
            else
            {
                loginButton.BackColor = GlobalColor.grayColor;
            }        
        }

    }
}
