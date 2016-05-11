using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HCT_Client
{
    public class FormsManager
    {
        private Form1 instanceForm1;
        private FormExecuteExam instanceFormExecuteExam;
        private FormFadeView instanceFormFadeView;
        private FormExamResult instanceFormExamResult;
        private FormChooseLanguage instanceFormChooseLanguage;
        private FormChooseExamCourse instanceFormChooseExamCourse;
        private FormInsertSmartCard instanceFormInsertSmartCard;

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

        public static FormInsertSmartCard GetFormInsertSmartCard()
        {
            if (Instance.instanceFormInsertSmartCard == null)
            {
                Instance.instanceFormInsertSmartCard = new FormInsertSmartCard();
            }
            return Instance.instanceFormInsertSmartCard;
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
