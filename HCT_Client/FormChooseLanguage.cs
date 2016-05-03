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
        public FormChooseLanguage() 
        {
            InitializeComponent();
            LocalizedTextManager.InitInstance();
            FormsManager.InitInstance();
            RenderUI();
        }

        public void RenderUI() 
        {
            int buttonGap = 100;

            LargeButton thaiButton = new LargeButton();
            thaiButton.Location = new Point((SCREEN_WIDTH  / 2 - thaiButton.Width - buttonGap / 2),
                                             (SCREEN_HEIGHT - thaiButton.Height) / 2);
            thaiButton.Text = "ภาษาไทย";
            thaiButton.Click += new EventHandler(ButtonClickedTH);


            LargeButton engButton = new LargeButton();
            engButton.Location = new Point((SCREEN_WIDTH / 2 + buttonGap / 2),
                                             (SCREEN_HEIGHT - engButton.Height) / 2);
            engButton.Text = "ENGLISH";
            engButton.Click += new EventHandler(ButtonClickedEN);

            this.Controls.Add(thaiButton);
            this.Controls.Add(engButton);
        }

        void ButtonClickedTH(object sender, EventArgs e)
        {
            LocalizedTextManager.SetLanguage(0);
            Form1 instanceForm1 = FormsManager.GetForm1();
            instanceForm1.Visible = true;
            instanceForm1.BringToFront();
        }

        void ButtonClickedEN(object sender, EventArgs e)
        {
            LocalizedTextManager.SetLanguage(1);
            Form1 instanceForm1 = FormsManager.GetForm1();
            instanceForm1.Visible = true;
            instanceForm1.BringToFront();
        }
    }
}
