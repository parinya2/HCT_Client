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
    public partial class FormInsertSmartCard : FixedSizeForm
    {
        BlinkButtonSignalClock blinkButtonSignalClock;
        LargeButton loginButton;
        MediumButton backButton;
        BaseTextLabel loginTextLabel;

        public FormInsertSmartCard()
        {
            InitializeComponent();

            RenderUI();
            blinkButtonSignalClock = new BlinkButtonSignalClock();
            blinkButtonSignalClock.TheTimeChanged += new BlinkButtonSignalClock.BlinkButtonSignalClockTickHandler(BlinkButtonSignalClockHasChanged);
        }

        private void RenderUI()
        {
            loginTextLabel = new BaseTextLabel();
            loginTextLabel.Width = SCREEN_WIDTH;
            loginTextLabel.Location = new Point(0, 150);
            loginTextLabel.TextAlign = ContentAlignment.MiddleCenter;
            loginTextLabel.ForeColor = Color.Black;

            loginButton = new LargeButton();
            loginButton.Location = new Point((SCREEN_WIDTH - loginButton.Width) / 2,
                                            loginTextLabel.Location.Y + loginTextLabel.Height + 100);
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
        }

        void LoginButtonClicked(object sender, EventArgs e)
        {
            GoToNextForm();
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
            FormChooseLanguage instanceFormChooseLanguage = FormsManager.GetFormChooseLanguage();
            instanceFormChooseLanguage.Visible = true;
            instanceFormChooseLanguage.Enabled = true;
            instanceFormChooseLanguage.BringToFront();
            this.Visible = false;
        }

        protected void BlinkButtonSignalClockHasChanged(int state)
        {
            loginButton.BackColor = Util.GetButtonBlinkColorAtSignalState(state);
        }
    }
}
