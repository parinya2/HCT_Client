using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Printing;

namespace HCT_Client
{
    public partial class FormChooseLanguage : FixedSizeFormWithHeader
    {
        private BlinkButtonSignalClock blinkButtonSignalClock;
        LargeButton thaiButton;
        LargeButton engButton;
        Button exitButton;
        FormFadeView fadeForm;
        FormLargeMessageBox confirmExitMessageBox;

        public FormChooseLanguage() 
        {
            InitializeComponent();
            LocalizedTextManager.InitInstance();
            FormsManager.InitInstance();
            FormsManager.SetFormChooseLanguage(this);
            UserProfileManager.InitInstance();
            CardReaderManager.InitInstance();
            QuizManager.InitInstance();
            Util.GenerateButtonBlinkColorDict();
            
            RenderUI();

            confirmExitMessageBox = new FormLargeMessageBox(1);
            confirmExitMessageBox.messageLabel.Text = LocalizedTextManager.GetLocalizedTextForKey("ConfirmExitMessageBox.Message");
            confirmExitMessageBox.rightButton.Text = LocalizedTextManager.GetLocalizedTextForKey("ConfirmExitMessageBox.RightButton");
            confirmExitMessageBox.leftButton.Text = LocalizedTextManager.GetLocalizedTextForKey("ConfirmExitMessageBox.LeftButton");
            confirmExitMessageBox.Visible = false;
            confirmExitMessageBox.rightButton.Click += new EventHandler(ConfirmExitMessageBoxRightButtonClicked);
            confirmExitMessageBox.leftButton.Click += new EventHandler(ConfirmExitMessageBoxLeftButtonClicked);

            fadeForm = FormsManager.GetFormFadeView();

            FormFadeView baseBG = FormsManager.GetFormBaseBackgroundView();
            baseBG.Visible = true;
            this.BringToFront();

            blinkButtonSignalClock = new BlinkButtonSignalClock();
            blinkButtonSignalClock.TheTimeChanged += new BlinkButtonSignalClock.BlinkButtonSignalClockTickHandler(BlinkButtonSignalClockHasChanged);
        }

        public void RenderUI() 
        {
            int buttonGap = 100;

            thaiButton = new LargeButton();
            thaiButton.Location = new Point((SCREEN_WIDTH  / 2 - thaiButton.Width - buttonGap / 2),
                                             (SCREEN_HEIGHT - thaiButton.Height) / 2);
            thaiButton.Text = "ภาษาไทย";
            thaiButton.Click += new EventHandler(ButtonClickedTH);


            engButton = new LargeButton();
            engButton.Location = new Point((SCREEN_WIDTH / 2 + buttonGap / 2),
                                             (SCREEN_HEIGHT - engButton.Height) / 2);
            engButton.Text = "ENGLISH";
            engButton.Click += new EventHandler(ButtonClickedEN);

            exitButton = new Button();
            exitButton.Width = 200;
            exitButton.Height = 90;
            exitButton.Font = new Font(this.Font.FontFamily, 18);
            exitButton.Location = new Point(SCREEN_WIDTH - exitButton.Width - 50,
                              SCREEN_HEIGHT - exitButton.Height - 50);
            exitButton.Text = "ออกจากโปรแกรม";
            exitButton.Click += new EventHandler(ExitButtonClicked);
            exitButton.BackColor = Color.FromArgb(239,64,43);
            exitButton.ForeColor = Color.White;

            this.Controls.Add(exitButton);
            this.Controls.Add(thaiButton);
            this.Controls.Add(engButton);
        }

        void ExitButtonClicked(object sender, EventArgs e)
        {
            fadeForm.Visible = true;
            fadeForm.BringToFront();

            confirmExitMessageBox.Visible = true;
            confirmExitMessageBox.BringToFront();
            confirmExitMessageBox.Location = new Point((SCREEN_WIDTH - confirmExitMessageBox.Width) / 2,
                                                    (SCREEN_HEIGHT - confirmExitMessageBox.Height) / 2);
        }

        void ButtonClickedTH(object sender, EventArgs e)
        {
            LocalizedTextManager.SetLanguage(0);
            GoToNextForm();
        }

        void ButtonClickedEN(object sender, EventArgs e)
        {
            LocalizedTextManager.SetLanguage(1);
            GoToNextForm();
        }

        void GoToNextForm()
        {
            FormInsertSmartCard instanceFormInsertSmartCard = FormsManager.GetFormInsertSmartCard();
            instanceFormInsertSmartCard.Visible = true;
            instanceFormInsertSmartCard.Enabled = true;
            instanceFormInsertSmartCard.RefreshUI();
            instanceFormInsertSmartCard.BringToFront();
            this.Visible = false;
        }

        void ExitProgram()
        {
            if (System.Windows.Forms.Application.MessageLoop)
            {
                // WinForms app
                System.Windows.Forms.Application.Exit();
            }
            else
            {
                // Console app
                System.Environment.Exit(1);
            }
        }

        void ConfirmExitMessageBoxRightButtonClicked(object sender, EventArgs e)
        {
            ExitProgram();
        }

        void ConfirmExitMessageBoxLeftButtonClicked(object sender, EventArgs e)
        {
            confirmExitMessageBox.Visible = false;
            fadeForm.Visible = false;
            this.Visible = true;
            this.Enabled = true;
            this.BringToFront();
        }

        protected void BlinkButtonSignalClockHasChanged(int state)
        {
            thaiButton.BackColor = Util.GetButtonBlinkColorAtSignalState(state);
            engButton.BackColor = Util.GetButtonBlinkColorAtSignalState(state);
        }
    }
}
