using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HCT_Client
{
    public class FormsManager
    {
        private Form1 instanceForm1;
        private FormExecuteExam instanceFormExecuteExam;
        private FormFadeView instanceFormFadeView;
        private FormFadeView instanceFormBaseBackgroundView;
        private FormLoadingView instanceFormLoadingView;
        private FormExamResult instanceFormExamResult;
        private FormChooseLanguage instanceFormChooseLanguage;
        private FormChooseExamCourse instanceFormChooseExamCourse;
        private FormInsertSmartCard instanceFormInsertSmartCard;
        private FormShowUserDetail instanceFormShowUserDetail;
        private FormLargeMessageBox instanceFormSystemProcessingMessageBox;
        private FormLargeMessageBox instanceFormErrorMessageBox;

        private static FormsManager instance;

        private FormsManager()
        {
            
        }

        public static FormsManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new FormsManager();
                }
                return instance;
            }
        }

        public static void InitInstance()
        {
            if (instance == null)
            {
                instance = new FormsManager();
            }
        }

        public static Form1 GetForm1()
        {
            if (Instance.instanceForm1 == null)
            {
                Instance.instanceForm1 = new Form1();
            }
            return Instance.instanceForm1;
        }

        public static FormExecuteExam GetFormExecuteExam()
        {
            if (Instance.instanceFormExecuteExam == null)
            {
                Instance.instanceFormExecuteExam = new FormExecuteExam();
            }
            return Instance.instanceFormExecuteExam;
        }

        public static FormFadeView GetFormFadeView()
        {
            if (Instance.instanceFormFadeView == null)
            {
                Instance.instanceFormFadeView = new FormFadeView();
            }
            return Instance.instanceFormFadeView;
        }

        public static FormFadeView GetFormBaseBackgroundView()
        {
            if (Instance.instanceFormBaseBackgroundView == null)
            {
                Instance.instanceFormBaseBackgroundView = new FormFadeView();
            }
            return Instance.instanceFormBaseBackgroundView;
        }

        public static FormLoadingView GetFormLoadingView()
        {
            if (Instance.instanceFormLoadingView == null)
            {
                Instance.instanceFormLoadingView = new FormLoadingView();
            }
            return Instance.instanceFormLoadingView;
        }

        public static FormExamResult GetFormExamResult()
        {
            if (Instance.instanceFormExamResult == null)
            {
                Instance.instanceFormExamResult = new FormExamResult();
            }
            return Instance.instanceFormExamResult;
        }

        public static FormChooseExamCourse GetFormChooseExamCourse()
        {
            if (Instance.instanceFormChooseExamCourse == null)
            {
                Instance.instanceFormChooseExamCourse = new FormChooseExamCourse();
            }
            return Instance.instanceFormChooseExamCourse;
        }

        public static FormShowUserDetail GetFormShowUserDetail()
        {
            if (Instance.instanceFormShowUserDetail == null)
            {
                Instance.instanceFormShowUserDetail = new FormShowUserDetail();
            }
            return Instance.instanceFormShowUserDetail;
        }

        public static FormInsertSmartCard GetFormInsertSmartCard()
        {
            if (Instance.instanceFormInsertSmartCard == null)
            {
                Instance.instanceFormInsertSmartCard = new FormInsertSmartCard();
            }
            return Instance.instanceFormInsertSmartCard;
        }

        public static FormLargeMessageBox GetFormSystemProcessingMessageBox()
        {
            if (Instance.instanceFormSystemProcessingMessageBox == null)
            {
                Instance.instanceFormSystemProcessingMessageBox = new FormLargeMessageBox(-1);
                Instance.instanceFormSystemProcessingMessageBox.messageLabel.Text = LocalizedTextManager.GetLocalizedTextForKey("SystemProcessingMessageBox.Message");
            }
            return Instance.instanceFormSystemProcessingMessageBox;
        }

        public static FormLargeMessageBox GetFormErrorMessageBox(string errorCode, Form callerForm)
        {
            if (Instance.instanceFormErrorMessageBox == null)
            {
                Instance.instanceFormErrorMessageBox = new FormLargeMessageBox(0);
                Instance.instanceFormErrorMessageBox.rightButton.Text = LocalizedTextManager.GetLocalizedTextForKey("ErrorMessageBox.RightButton");
                Instance.instanceFormErrorMessageBox.rightButton.Click += new EventHandler(ErrorMessageBoxRightButtonClick);
            }
            Instance.instanceFormErrorMessageBox.callerForm = callerForm;
            Instance.instanceFormErrorMessageBox.messageLabel.Text = LocalizedTextManager.GetLocalizedTextForKey("ErrorMessageBox." + errorCode + ".Message");
            return Instance.instanceFormErrorMessageBox;
        }

        private static void ErrorMessageBoxRightButtonClick(object sender, EventArgs e)
        {
            FormLargeMessageBox msgBox = (FormLargeMessageBox)(((Button)sender).FindForm());
            msgBox.Visible = false;
            instance.instanceFormFadeView.Visible = false;
            msgBox.callerForm.Visible = true;
            msgBox.callerForm.Enabled = true;
            msgBox.callerForm.BringToFront();
        }

        public static FormChooseLanguage GetFormChooseLanguage()
        {
            return Instance.instanceFormChooseLanguage;
        }

        public static void SetFormChooseLanguage(FormChooseLanguage formChooseLanguage)
        {
            Instance.instanceFormChooseLanguage = formChooseLanguage;
        }
    }
}
