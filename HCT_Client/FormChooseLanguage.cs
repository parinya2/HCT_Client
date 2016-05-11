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
    public partial class FormChooseLanguage : FixedSizeForm
    {
        private BlinkButtonSignalClock blinkButtonSignalClock;
        LargeButton thaiButton;
        LargeButton engButton;

        public FormChooseLanguage() 
        {
            InitializeComponent();
            LocalizedTextManager.InitInstance();
            FormsManager.InitInstance();
            FormsManager.SetFormChooseLanguage(this);
            Util.GenerateButtonBlinkColorDict();
            RenderUI();

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

            Button exitButton = new Button();
            exitButton.Width = 200;
            exitButton.Height = 90;
            exitButton.Font = new Font(this.Font.FontFamily, 18);
            exitButton.Location = new Point(SCREEN_WIDTH - exitButton.Width - 50,
                              SCREEN_HEIGHT - exitButton.Height - 50);
            exitButton.Text = "ออกจากโปรแกรม";
            exitButton.Click += new EventHandler(ExitButtonClicked);

            this.Controls.Add(exitButton);
            this.Controls.Add(thaiButton);
            this.Controls.Add(engButton);
        }

        void ExitButtonClicked(object sender, EventArgs e)
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

        protected void BlinkButtonSignalClockHasChanged(int state)
        {
            thaiButton.BackColor = Util.GetButtonBlinkColorAtSignalState(state);
            engButton.BackColor = Util.GetButtonBlinkColorAtSignalState(state);
        }
    }
}
