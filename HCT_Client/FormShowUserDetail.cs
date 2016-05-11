﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HCT_Client
{
    public class SingleUserAttributePanel : Panel
    {
        public Label attributeHeaderLabel;
        public Label attributeContentLabel;

        public SingleUserAttributePanel(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            int margin = 15;
            int gapX = 20;

            attributeHeaderLabel = new Label();
            attributeHeaderLabel.Width = (int)(this.Width * 0.25);
            attributeHeaderLabel.Height = this.Height;
            attributeHeaderLabel.Location = new Point(margin, 0);
            attributeHeaderLabel.TextAlign = ContentAlignment.MiddleLeft;
            attributeHeaderLabel.BackColor = Color.Orange;
            attributeHeaderLabel.Font = new Font(this.Font.FontFamily, 14);

            attributeContentLabel = new Label();
            attributeContentLabel.Width = this.Width - attributeHeaderLabel.Width - gapX - margin * 2;
            attributeContentLabel.Height = attributeHeaderLabel.Height;
            attributeContentLabel.Location = new Point(attributeHeaderLabel.Width + margin + gapX, attributeHeaderLabel.Location.Y);
            attributeContentLabel.TextAlign = ContentAlignment.MiddleLeft;
            attributeContentLabel.Font = new Font(this.Font.FontFamily, 12);

            this.BackColor = Color.Transparent;
            this.Controls.Add(attributeHeaderLabel);
            this.Controls.Add(attributeContentLabel);
        }
    }

    public partial class FormShowUserDetail : FixedSizeForm
    {
        PictureBox userPhotoPictureBox;
        MediumButton takePhotoButton;
        MediumButton goToExamButton;
        MediumButton backButton;
        SingleUserAttributePanel fullnamePanel;
        SingleUserAttributePanel citizenIDPanel;
        SingleUserAttributePanel addressPanel;
        SingleUserAttributePanel courseNamePanel;
        SingleUserAttributePanel examDatePanel;

        private BlinkButtonSignalClock blinkButtonSignalClock;

        public FormShowUserDetail()
        {
            InitializeComponent();

            RenderUI();

            blinkButtonSignalClock = new BlinkButtonSignalClock();
            blinkButtonSignalClock.TheTimeChanged += new BlinkButtonSignalClock.BlinkButtonSignalClockTickHandler(BlinkButtonSignalClockHasChanged);
        }

        private void RenderUI()
        {
            userPhotoPictureBox = new PictureBox();
            userPhotoPictureBox.Width = 150;
            userPhotoPictureBox.Height = 150;
            userPhotoPictureBox.BackColor = Color.Green;
            userPhotoPictureBox.Location = new Point(130, 130);

            takePhotoButton = new MediumButton();
            takePhotoButton.Width = userPhotoPictureBox.Width;
            takePhotoButton.Height = 70;
            takePhotoButton.Location = new Point(userPhotoPictureBox.Location.X,
                                                 userPhotoPictureBox.Location.Y + userPhotoPictureBox.Height + 40);

            goToExamButton = new MediumButton();
            goToExamButton.Location = new Point(SCREEN_WIDTH - goToExamButton.Width - 50,
                                                SCREEN_HEIGHT - goToExamButton.Height - 50);
            goToExamButton.Click += new EventHandler(GoToExamButtonClicked);

            backButton = new MediumButton();
            backButton.Location = new Point(goToExamButton.Location.X - backButton.Width - 50,
                                            goToExamButton.Location.Y);
            backButton.Click += new EventHandler(BackButtonClicked);

            int attributePanelWidth = SCREEN_WIDTH - userPhotoPictureBox.Width - userPhotoPictureBox.Location.X - 100;
            int attributePanelHeight = 70;
            fullnamePanel = new SingleUserAttributePanel(attributePanelWidth, attributePanelHeight);
            citizenIDPanel = new SingleUserAttributePanel(attributePanelWidth, attributePanelHeight);
            addressPanel = new SingleUserAttributePanel(attributePanelWidth, attributePanelHeight);
            courseNamePanel = new SingleUserAttributePanel(attributePanelWidth, attributePanelHeight);
            examDatePanel = new SingleUserAttributePanel(attributePanelWidth, attributePanelHeight);

            SingleUserAttributePanel[] attributePanelList = new SingleUserAttributePanel[5];
            attributePanelList[0] = fullnamePanel;
            attributePanelList[1] = citizenIDPanel;
            attributePanelList[2] = addressPanel;
            attributePanelList[3] = courseNamePanel;
            attributePanelList[4] = examDatePanel;

            int attributePanelStartX = userPhotoPictureBox.Location.X + userPhotoPictureBox.Width + 50;
            int attributePanelStartY = userPhotoPictureBox.Location.Y;
            int attributePanelGapY = 17;
            for (int i = 0; i < attributePanelList.Length; i++)
            {
                int locationY = attributePanelStartY + (attributePanelHeight + attributePanelGapY) * i;
                attributePanelList[i].Location = new Point(attributePanelStartX, locationY);

                this.Controls.Add(attributePanelList[i]);
            }

            this.Controls.Add(userPhotoPictureBox);
            this.Controls.Add(takePhotoButton);
            this.Controls.Add(goToExamButton);
            this.Controls.Add(backButton);
        }

        public void RefreshUI()
        {
            takePhotoButton.Text = LocalizedTextManager.GetLocalizedTextForKey("FormShowUserDetail.Button.TakePhoto");
            backButton.Text = LocalizedTextManager.GetLocalizedTextForKey("FormShowUserDetail.Button.GoBack");
            goToExamButton.Text = LocalizedTextManager.GetLocalizedTextForKey("FormShowUserDetail.Button.TakeExam");
            
            fullnamePanel.attributeHeaderLabel.Text = LocalizedTextManager.GetLocalizedTextForKey("FormShowUserDetail.Label.Fullname");
            citizenIDPanel.attributeHeaderLabel.Text = LocalizedTextManager.GetLocalizedTextForKey("FormShowUserDetail.Label.CitizenID");
            addressPanel.attributeHeaderLabel.Text = LocalizedTextManager.GetLocalizedTextForKey("FormShowUserDetail.Label.Address");
            courseNamePanel.attributeHeaderLabel.Text = LocalizedTextManager.GetLocalizedTextForKey("FormShowUserDetail.Label.CourseName");
            examDatePanel.attributeHeaderLabel.Text = LocalizedTextManager.GetLocalizedTextForKey("FormShowUserDetail.Label.ExamDate");

            fullnamePanel.attributeContentLabel.Text = "  " + (UserProfileManager.GetFullnameTH().Trim().Length > 0 ? UserProfileManager.GetFullnameTH() : UserProfileManager.GetFullnameEN());
            citizenIDPanel.attributeContentLabel.Text = "  " + UserProfileManager.GetCitizenID();
            addressPanel.attributeContentLabel.Text = "  " + UserProfileManager.GetAddress();
            int courseType = QuizManager.GetExamCourseType() + 1;
            courseNamePanel.attributeContentLabel.Text = "  " + LocalizedTextManager.GetLocalizedTextForKey("FormChooseExamCourse.Button." + courseType);
            examDatePanel.attributeContentLabel.Text = "  " + DateTime.Now.ToString("d/MM/yyyy");
        }

        void GoToExamButtonClicked(object sender, EventArgs e)
        {
            GoToNextForm();
        }

        void BackButtonClicked(object sender, EventArgs e)
        {
            GoToPreviousForm();
        }

        void GoToNextForm()
        {
            FormExecuteExam instanceFormExecuteExam = FormsManager.GetFormExecuteExam();
            instanceFormExecuteExam.LoadExamData();
            instanceFormExecuteExam.Visible = true;
            instanceFormExecuteExam.Enabled = true;
            instanceFormExecuteExam.RefreshUI();
            instanceFormExecuteExam.BringToFront();
            this.Visible = false;
        }

        void GoToPreviousForm()
        {
            FormChooseExamCourse instanceFormChooseExamCourse = FormsManager.GetFormChooseExamCourse();
            instanceFormChooseExamCourse.Visible = true;
            instanceFormChooseExamCourse.Enabled = true;
            instanceFormChooseExamCourse.RefreshUI();
            instanceFormChooseExamCourse.BringToFront();
            this.Visible = false;
        }

        protected void BlinkButtonSignalClockHasChanged(int state)
        {
            goToExamButton.BackColor = Util.GetButtonBlinkColorAtSignalState(state);
            takePhotoButton.BackColor = Util.GetButtonBlinkColorAtSignalState(state);
        }
    }
}