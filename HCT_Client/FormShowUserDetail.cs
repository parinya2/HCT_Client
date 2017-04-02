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
            attributeHeaderLabel.Width = (int)(this.Width * 0.28);
            attributeHeaderLabel.Height = this.Height;
            attributeHeaderLabel.Location = new Point(margin, 0);
            attributeHeaderLabel.TextAlign = ContentAlignment.MiddleLeft;
            attributeHeaderLabel.BackColor = Color.Transparent;
            attributeHeaderLabel.Font = UtilFonts.GetTHSarabunFont(28);

            attributeContentLabel = new Label();
            attributeContentLabel.Width = this.Width - attributeHeaderLabel.Width - gapX - margin * 4;
            attributeContentLabel.Height = attributeHeaderLabel.Height;
            attributeContentLabel.Location = new Point(attributeHeaderLabel.Width + margin + gapX, attributeHeaderLabel.Location.Y);
            attributeContentLabel.TextAlign = ContentAlignment.MiddleLeft;
            attributeContentLabel.Font = UtilFonts.GetTHSarabunFont(26);
            attributeContentLabel.BackColor = GlobalColor.paleRoseColor;


            this.BackColor = Color.Transparent;
            this.Controls.Add(attributeHeaderLabel);
            this.Controls.Add(attributeContentLabel);
        }
    }

    public partial class FormShowUserDetail : FixedSizeFormWithHeader
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

        FormLargeMessageBox goToExamMessageBox;
        FormLargeMessageBox noUserPhotoMessageBox;
        FormFadeView fadeForm;
        private BlinkButtonSignalClock blinkButtonSignalClock;

        private WebCamCapture webcamCapture;

        public FormShowUserDetail()
        {
            InitializeComponent();

            RenderUI();
            PrepareWebcamCapture();

            goToExamMessageBox = new FormLargeMessageBox(1, MessageBoxIcon.Null);
            goToExamMessageBox.Visible = false;
            goToExamMessageBox.rightButton.Click += new EventHandler(GoToExamMessageBoxRightButtonClicked);
            goToExamMessageBox.leftButton.Click += new EventHandler(GoToExamMessageBoxLeftButtonClicked);

            noUserPhotoMessageBox = new FormLargeMessageBox(0, MessageBoxIcon.CameraIcon);
            noUserPhotoMessageBox.Visible = false;
            noUserPhotoMessageBox.rightButton.Click += new EventHandler(NoUserPhotoMessageBoxRightButtonClicked);          

            fadeForm = FormsManager.GetFormFadeView();

            blinkButtonSignalClock = new BlinkButtonSignalClock();
            blinkButtonSignalClock.TheTimeChanged += new BlinkButtonSignalClock.BlinkButtonSignalClockTickHandler(BlinkButtonSignalClockHasChanged);
        }

        private void PrepareWebcamCapture()
        {
            webcamCapture = new WebCamCapture();
            webcamCapture.CaptureHeight = 240;
            webcamCapture.CaptureWidth = 320;
            // TODO: Code generation for 'this.WebCamCapture.FrameNumber' failed because of Exception 'Invalid Primitive Type: System.UInt64. Only CLS compliant primitive types can be used. Consider using CodeObjectCreateExpression.'.
            webcamCapture.Location = new System.Drawing.Point(17, 17);
            webcamCapture.Name = "WebCamCapture";
            webcamCapture.Size = new System.Drawing.Size(342, 252);
            webcamCapture.TabIndex = 0;
            webcamCapture.TimeToCapture_milliseconds = 100;
            webcamCapture.ImageCaptured += new WebCamCapture.WebCamEventHandler(this.WebCamCapture_ImageCaptured);

            webcamCapture.CaptureHeight = userPhotoPictureBox.Height;
            webcamCapture.CaptureWidth = userPhotoPictureBox.Width;
        }

        private void RenderUI()
        {
            userPhotoPictureBox = new PictureBox();
            userPhotoPictureBox.Width = 250;
            userPhotoPictureBox.Height = 250;
            userPhotoPictureBox.BackColor = Color.Green;
            userPhotoPictureBox.Location = new Point(130, headerLineLabel.Location.Y + 60);

            takePhotoButton = new MediumButton();
            takePhotoButton.Width = userPhotoPictureBox.Width;
            takePhotoButton.Height = 70;
            takePhotoButton.Location = new Point(userPhotoPictureBox.Location.X,
                                                 userPhotoPictureBox.Location.Y + userPhotoPictureBox.Height + 40);
            takePhotoButton.Click += new EventHandler(TakePhotoButtonClicked);
            takePhotoButton.BackColor = GlobalColor.yellowColor;

            backButton = new MediumButton();
            backButton.SetLocationForBackButton();
            backButton.Click += new EventHandler(BackButtonClicked);
            backButton.ForeColor = Color.White;
            backButton.BackColor = GlobalColor.redColor;

            goToExamButton = new MediumButton();
            goToExamButton.Location = new Point(SCREEN_WIDTH - goToExamButton.Width - backButton.Location.X,
                                                backButton.Location.Y);
            goToExamButton.Click += new EventHandler(GoToExamButtonClicked);
            goToExamButton.ForeColor = Color.White;
            goToExamButton.BackColor = GlobalColor.redColor;

            int attributePanelWidth = SCREEN_WIDTH - userPhotoPictureBox.Width - userPhotoPictureBox.Location.X - 100;
            int attributePanelHeight = 90;
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

            int attributePanelStartX = userPhotoPictureBox.Location.X + userPhotoPictureBox.Width + 90;
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
            addressPanel.attributeHeaderLabel.Text = LocalizedTextManager.GetLocalizedTextForKey("FormShowUserDetail.Label.Address");
            courseNamePanel.attributeHeaderLabel.Text = LocalizedTextManager.GetLocalizedTextForKey("FormShowUserDetail.Label.CourseName");
            examDatePanel.attributeHeaderLabel.Text = LocalizedTextManager.GetLocalizedTextForKey("FormShowUserDetail.Label.ExamDate");

            fullnamePanel.attributeContentLabel.Text = "  " + (UserProfileManager.GetFullnameTH().Trim().Length > 0 ? UserProfileManager.GetFullnameTH() : UserProfileManager.GetFullnameEN());
            addressPanel.attributeContentLabel.Text = "  " + UserProfileManager.GetAddress();
            courseNamePanel.attributeContentLabel.Text = "  " + LocalizedTextManager.GetLocalizedTextForKey("FormChooseExamCourse.Button." + QuizManager.GetExamCourseType());
            examDatePanel.attributeContentLabel.Text = "  " + DateTime.Now.ToString("d/MM/yyyy");

            if (UserProfileManager.GetCitizenID() != null && UserProfileManager.GetCitizenID().Length > 0)
            {
                citizenIDPanel.attributeHeaderLabel.Text = LocalizedTextManager.GetLocalizedTextForKey("FormShowUserDetail.Label.CitizenID");
                citizenIDPanel.attributeContentLabel.Text = "  " + UserProfileManager.GetCitizenID();
            }
            else if (UserProfileManager.GetPassportID() != null && UserProfileManager.GetPassportID().Length > 0)
            {
                citizenIDPanel.attributeHeaderLabel.Text = LocalizedTextManager.GetLocalizedTextForKey("FormShowUserDetail.Label.PassportID");
                citizenIDPanel.attributeContentLabel.Text = "  " + UserProfileManager.GetPassportID();
            }

            goToExamMessageBox.messageLabel.Text = LocalizedTextManager.GetLocalizedTextForKey("GoToExamMessageBox.Message");
            goToExamMessageBox.rightButton.Text = LocalizedTextManager.GetLocalizedTextForKey("GoToExamMessageBox.RightButton");
            goToExamMessageBox.leftButton.Text = LocalizedTextManager.GetLocalizedTextForKey("GoToExamMessageBox.LeftButton");

            noUserPhotoMessageBox.messageLabel.Text = LocalizedTextManager.GetLocalizedTextForKey("NoUserPhotoMessageBox.Message");
            noUserPhotoMessageBox.rightButton.Text = LocalizedTextManager.GetLocalizedTextForKey("NoUserPhotoMessageBox.RightButton");

            userPhotoPictureBox.Image = null;
            UserProfileManager.ClearUserPhoto();
            StartWebcam();
        }

        void GoToExamButtonClicked(object sender, EventArgs e)
        {
            fadeForm.ShowFadeView(true);

            bool hasUserPhoto = (UserProfileManager.GetUserPhoto() != null);
            if (hasUserPhoto)
            {
                goToExamMessageBox.Visible = true;
                goToExamMessageBox.BringToFront();
                goToExamMessageBox.Location = new Point((SCREEN_WIDTH - goToExamMessageBox.Width) / 2,
                                                        (SCREEN_HEIGHT - goToExamMessageBox.Height) / 2);
            }
            else
            {
                noUserPhotoMessageBox.Visible = true;
                noUserPhotoMessageBox.BringToFront();
                noUserPhotoMessageBox.Location = new Point((SCREEN_WIDTH - noUserPhotoMessageBox.Width) / 2,
                                                        (SCREEN_HEIGHT - noUserPhotoMessageBox.Height) / 2);
            }
       }

        void BackButtonClicked(object sender, EventArgs e)
        {
            GoToPreviousForm();
        }

        void GoToExamMessageBoxRightButtonClicked(object sender, EventArgs e)
        {
            GoToNextForm();
        }

        void NoUserPhotoMessageBoxRightButtonClicked(object sender, EventArgs e)
        {
            noUserPhotoMessageBox.Visible = false;
            fadeForm.ShowFadeView(false);
            this.Visible = true;
            this.Enabled = true;
            this.BringToFront();
        }

        void GoToExamMessageBoxLeftButtonClicked(object sender, EventArgs e)
        {
            goToExamMessageBox.Visible = false;
            fadeForm.ShowFadeView(false);
            this.Visible = true;
            this.Enabled = true;
            this.BringToFront();
        }

        void TakePhotoButtonClicked(object sender, EventArgs e)
        {
            bool hasUserPhoto = (UserProfileManager.GetUserPhoto() != null);
            if (hasUserPhoto)
            {
                UserProfileManager.ClearUserPhoto();
                StartWebcam();
                takePhotoButton.Text = LocalizedTextManager.GetLocalizedTextForKey("FormShowUserDetail.Button.TakePhoto");
            }
            else
            {
                if (userPhotoPictureBox.Image == null)
                    return;

                UserProfileManager.SetUserPhoto(userPhotoPictureBox.Image);
                StopWebcam();
                takePhotoButton.Text = LocalizedTextManager.GetLocalizedTextForKey("FormShowUserDetail.Button.DeletePhoto");   
            }
        }

        void StartWebcam()
        {
            // change the capture time frame
            webcamCapture.TimeToCapture_milliseconds = 50;

            // start the video capture. let the control handle the
            // frame numbers.
            webcamCapture.Start(0);
        }

        void StopWebcam()
        {
            webcamCapture.Stop();
        }

        public void GoToNextForm()
        {
            StopWebcam();

            FormsManager.GetFormLoadingView().ShowLoadingView(true);
            BackgroundWorker bw = new BackgroundWorker();
            string status = "";
            bw.DoWork += new DoWorkEventHandler(
                delegate(object o, DoWorkEventArgs args)
                {
                    Thread.Sleep(10);
                    status = WebServiceManager.GetEExamQuestionFromServer();                    
                }
             );
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(
                delegate(object o, RunWorkerCompletedEventArgs args)
                {                    
                    if (status.Equals(WebServiceResultStatus.SUCCESS))
                    {
                        FormExecuteExam instanceFormExecuteExam = FormsManager.GetFormExecuteExam();
                        instanceFormExecuteExam.examState = ExamState.TakingExamState;
                        instanceFormExecuteExam.LoadExamData();
                        instanceFormExecuteExam.Visible = true;
                        instanceFormExecuteExam.Enabled = true;
                        instanceFormExecuteExam.RefreshUI();
                        instanceFormExecuteExam.BringToFront();

                        this.Visible = false;
                        fadeForm.Visible = false;
                        goToExamMessageBox.Visible = false;
                        FormsManager.GetFormLoadingView().ShowLoadingView(false);
                    }
                    else
                    {
                        FormsManager.GetFormLoadingView().ShowLoadingView(false);
                        FormLargeMessageBox errorFormMessageBox = FormsManager.GetFormErrorMessageBox(status, this);
                        Point centerPoint = new Point((SCREEN_WIDTH - errorFormMessageBox.Width) / 2,
                                                      (SCREEN_HEIGHT - errorFormMessageBox.Height) / 2);
                        errorFormMessageBox.ShowMessageBoxAtLocation(centerPoint);
                    }
                }
            );
            bw.RunWorkerAsync();
        }

        void GoToPreviousForm()
        {
            StopWebcam();
            FormCourseRegisterSetting instanceFormCourseRegisterSetting = FormsManager.GetFormCourseRegisterSetting();
            instanceFormCourseRegisterSetting.Visible = true;
            instanceFormCourseRegisterSetting.Enabled = true;
            instanceFormCourseRegisterSetting.RefreshUI();
            instanceFormCourseRegisterSetting.BringToFront();
            this.Visible = false;
        }

        protected void BlinkButtonSignalClockHasChanged(int state)
        {
            bool hasUserPhoto = (UserProfileManager.GetUserPhoto() != null);
            if (hasUserPhoto)
            {
                goToExamButton.BackColor = Util.GetButtonBlinkColorAtSignalState_Green(state);
                takePhotoButton.BackColor = GlobalColor.yellowColor;
            }
            else
            {
                goToExamButton.BackColor = GlobalColor.grayColor;
                takePhotoButton.BackColor = Util.GetButtonBlinkColorAtSignalState_Yellow(state);
            }
            goToExamMessageBox.rightButton.BackColor = Util.GetButtonBlinkColorAtSignalState_Green(state);        
        }

        private void WebCamCapture_ImageCaptured(object source, WebcamEventArgs e)
        {
            userPhotoPictureBox.Image = e.WebCamImage;
        }
    }
}
