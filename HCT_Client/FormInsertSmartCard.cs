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
        MediumButton backButton;
        BaseTextLabel loginTextLabel;
        FormLargeMessageBox smartCardErrorMessageBox;
        FormFadeView fadeForm;

        public FormInsertSmartCard()
        {
            InitializeComponent();

            UserProfileManager.InitInstance();

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
                                            loginTextLabel.Location.Y + loginTextLabel.Height + 50);
            loginButton.Click += new EventHandler(LoginButtonClicked);

            backButton = new MediumButton();
            backButton.Location = new Point(SCREEN_WIDTH - backButton.Width - 50,
                                            SCREEN_HEIGHT - backButton.Height - 50);
            backButton.Click += new EventHandler(BackButtonClicked);

            this.Controls.Add(loginTextLabel);
            this.Controls.Add(loginButton);
            this.Controls.Add(backButton);
        }

        public void RefreshUI()
        {
            loginTextLabel.Text = LocalizedTextManager.GetLocalizedTextForKey("FormInsertSmartCard.Login.Label");
            loginButton.Text = LocalizedTextManager.GetLocalizedTextForKey("FormInsertSmartCard.Login.Button");
            backButton.Text = LocalizedTextManager.GetLocalizedTextForKey("FormInsertSmartCard.GoBack");
            smartCardErrorMessageBox.rightButton.Text = LocalizedTextManager.GetLocalizedTextForKey("SmartCardErrorMessageBox.RightButton");
        }

        void LoginButtonClicked(object sender, EventArgs e)
        {
            bool verifyResult = ReadSmartCard();
            if (verifyResult)
            {
                GoToNextForm();
            }   
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
