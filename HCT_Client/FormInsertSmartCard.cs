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
    public partial class FormInsertSmartCard : FixedSizeFormWithHeader
    {
        BlinkButtonSignalClock blinkButtonSignalClock;
        LargeButton loginButton;
        MediumButton passportButton;
        MediumButton backButton;
        BaseTextLabel loginTextLabel;
        BaseTextLabel passportTextLabel;
        FormLargeMessageBox smartCardErrorMessageBox;
        FormFadeView fadeForm;

        public FormInsertSmartCard()
        {
            InitializeComponent();            

            RenderUI();
            fadeForm = FormsManager.GetFormFadeView();
            smartCardErrorMessageBox = new FormLargeMessageBox(0);
            smartCardErrorMessageBox.messageLabel.Text = LocalizedTextManager.GetLocalizedTextForKey("TimeoutMessageBox.Message");
            smartCardErrorMessageBox.rightButton.Text = LocalizedTextManager.GetLocalizedTextForKey("TimeoutMessageBox.RightButton");
            smartCardErrorMessageBox.Visible = false;
            smartCardErrorMessageBox.rightButton.Click += new EventHandler(SmartCardErorMessageBoxRightButtonClicked);

            blinkButtonSignalClock = new BlinkButtonSignalClock();
            blinkButtonSignalClock.TheTimeChanged += new BlinkButtonSignalClock.BlinkButtonSignalClockTickHandler(BlinkButtonSignalClockHasChanged);
        }

        private void RenderUI()
        {
            loginTextLabel = new BaseTextLabel();
            loginTextLabel.Width = SCREEN_WIDTH;
            loginTextLabel.Location = new Point(0, headerLineLabel.Location.Y + 60);
            loginTextLabel.TextAlign = ContentAlignment.MiddleCenter;
            loginTextLabel.ForeColor = Color.Black;

            loginButton = new LargeButton();
            loginButton.Location = new Point((SCREEN_WIDTH - loginButton.Width) / 2,
                                            loginTextLabel.Location.Y + loginTextLabel.Height + 40);
            loginButton.Click += new EventHandler(LoginButtonClicked);

            passportTextLabel = new BaseTextLabel();
            passportTextLabel.Width = SCREEN_WIDTH;
            passportTextLabel.Location = new Point(0, loginButton.Location.Y + loginButton.Height + 60);
            passportTextLabel.TextAlign = ContentAlignment.MiddleCenter;
            passportTextLabel.ForeColor = Color.Black;

            passportButton = new MediumButton();
            passportButton.Width = loginButton.Width;
            passportButton.Height = 80;
            passportButton.Font = new Font(this.Font.FontFamily, 15);
            passportButton.Location = new Point((SCREEN_WIDTH - passportButton.Width) / 2,
                                            passportTextLabel.Location.Y + passportTextLabel.Height + 20);
            passportButton.Click += new EventHandler(PassportButtonClicked);

            backButton = new MediumButton();
            backButton.Location = new Point(SCREEN_WIDTH - backButton.Width - 50,
                                            SCREEN_HEIGHT - backButton.Height - 50);
            backButton.Click += new EventHandler(BackButtonClicked);

            this.Controls.Add(loginTextLabel);
            this.Controls.Add(loginButton);
            this.Controls.Add(backButton);
            this.Controls.Add(passportTextLabel);
            this.Controls.Add(passportButton);
        }

        public void RefreshUI()
        {
            loginTextLabel.Text = LocalizedTextManager.GetLocalizedTextForKey("FormInsertSmartCard.Login.Label");
            loginButton.Text = LocalizedTextManager.GetLocalizedTextForKey("FormInsertSmartCard.Login.Button");
            backButton.Text = LocalizedTextManager.GetLocalizedTextForKey("FormInsertSmartCard.GoBack");
            smartCardErrorMessageBox.rightButton.Text = LocalizedTextManager.GetLocalizedTextForKey("SmartCardErrorMessageBox.RightButton");

            passportTextLabel.Text = LocalizedTextManager.GetLocalizedTextForKey("FormInsertSmartCard.Passport.Label");
            passportButton.Text = LocalizedTextManager.GetLocalizedTextForKey("FormInsertSmartCard.Passport.Button");

            UserProfileManager.ClearUserProfile();
            UserProfileManager.ClearUserPhoto();
        }

        void LoginButtonClicked(object sender, EventArgs e)
        {
            bool verifyResult = ReadSmartCard();
            if (verifyResult)
            {
                GoToNextForm();
            }   
        }

        void PassportButtonClicked(object sender, EventArgs e)
        {
            GoToFormPassport();
        }

        void BackButtonClicked(object sender, EventArgs e)
        {
            GoToPreviousForm();
        }

        private bool ReadSmartCard()
        {            
            if (CardReaderManager.cardReaderMode == CardReaderMode.FULL_BYPASS)
            {
                UserProfileManager.FillUserProfileWithMockData();
                return true;
            }

            string cardData = CardReaderManager.ReadCardAndGetData();
            if (cardData.Equals(CardReaderManager.NO_CARD_ERROR) ||
                cardData.Equals(CardReaderManager.NO_READER_ERROR) ||
                cardData.Equals(CardReaderManager.UNKNOWN_ERROR))
            {
                if (!smartCardErrorMessageBox.Visible)
                {
                    fadeForm.Visible = true;
                    fadeForm.BringToFront();

                    smartCardErrorMessageBox.Visible = true;
                    smartCardErrorMessageBox.BringToFront();
                    smartCardErrorMessageBox.Location = new Point((SCREEN_WIDTH - smartCardErrorMessageBox.Width) / 2,
                          (SCREEN_HEIGHT - smartCardErrorMessageBox.Height) / 2);

                    string msg = "";
                    if (cardData.Equals(CardReaderManager.NO_CARD_ERROR))
                        msg = LocalizedTextManager.GetLocalizedTextForKey("FormInsertSmartCard.Error.CardNotFound");
                    else if (cardData.Equals(CardReaderManager.NO_READER_ERROR))
                        msg = LocalizedTextManager.GetLocalizedTextForKey("FormInsertSmartCard.Error.ReaderNotFound");
                    else if (cardData.Equals(CardReaderManager.UNKNOWN_ERROR))
                        msg = LocalizedTextManager.GetLocalizedTextForKey("FormInsertSmartCard.Error.Unknown");
                    smartCardErrorMessageBox.messageLabel.Text = msg;
                    this.Enabled = false;
                }
                return false;
            }
            else
            {
                UserProfileManager.FillUserProfileFromSmartCardData(cardData);                              
                return true;
            }
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

        private void GoToFormPassport()
        {
            FormPassport instanceFormPassport = FormsManager.GetFormPassport();
            instanceFormPassport.Visible = true;
            instanceFormPassport.Enabled = true;
            instanceFormPassport.RefreshUI();
            instanceFormPassport.BringToFront();
            this.Visible = false;
        }

        private void GoToPreviousForm()
        {
            FormChooseLanguage instanceFormChooseLanguage = FormsManager.GetFormChooseLanguage();
            instanceFormChooseLanguage.Visible = true;
            instanceFormChooseLanguage.Enabled = true;
            instanceFormChooseLanguage.BringToFront();
            this.Visible = false;
        }

        protected void BlinkButtonSignalClockHasChanged(int state)
        {
            loginButton.BackColor = Util.GetButtonBlinkColorAtSignalState(state);
            passportButton.BackColor = Util.GetButtonBlinkColorAtSignalState(state);
            smartCardErrorMessageBox.rightButton.BackColor = Util.GetButtonBlinkColorAtSignalState(state);
        }

        void SmartCardErorMessageBoxRightButtonClicked(object sender, EventArgs e)
        {
            smartCardErrorMessageBox.Visible = false;
            fadeForm.Visible = false;
            this.Visible = true;
            this.Enabled = true;
            this.BringToFront();
        }
    }
}
