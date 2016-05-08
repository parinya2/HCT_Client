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
    public partial class FormExamResult : FixedSizeForm
    {
        public LargeButton finishExamButton;
        public LargeButton viewAnswerButton;

        public FormExamResult()
        {
            InitializeComponent();
            
            RenderUI();
        }

        private void RenderUI()
        {
            int buttonOffsetX = 50;
            int buttonOffsetY = 30;

            finishExamButton = new LargeButton();
            finishExamButton.Click += new EventHandler(FinishExamButtonClicked);
            finishExamButton.Location = new Point(SCREEN_WIDTH - buttonOffsetX - finishExamButton.Width,
                                                  SCREEN_HEIGHT - buttonOffsetY - finishExamButton.Height);
            finishExamButton.Text = LocalizedTextManager.GetLocalizedTextForKey("FormExamResult.FinishButton");

            viewAnswerButton = new LargeButton();
            viewAnswerButton.Click += new EventHandler(ViewAnswerButtonClicked);
            viewAnswerButton.Location = new Point(finishExamButton.Location.X - buttonOffsetX - viewAnswerButton.Width,
                                                  finishExamButton.Location.Y);
            viewAnswerButton.Text = LocalizedTextManager.GetLocalizedTextForKey("FormExamResult.ViewAnswer");

            this.Controls.Add(finishExamButton);
            this.Controls.Add(viewAnswerButton);
        }

        void FinishExamButtonClicked(object sender, EventArgs e)
        {
            FormChooseLanguage instanceFormChooseLanguage = FormsManager.GetFormChooseLanguage();
            this.Visible = false;
            instanceFormChooseLanguage.Visible = true;
            instanceFormChooseLanguage.BringToFront();
        }

        void ViewAnswerButtonClicked(object sender, EventArgs e)
        {
      
        }
    }
}
