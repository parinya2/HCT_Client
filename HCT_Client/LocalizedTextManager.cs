using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HCT_Client
{
    public class LocalizedTextManager
    {
        public enum LanguageMode { TH, EN};
        private static LocalizedTextManager instance;
        private Dictionary<string, string> dataDictTH;
        private Dictionary<string, string> dataDictEN;
        LanguageMode language;

        private LocalizedTextManager()
        {
            dataDictTH = new Dictionary<string, string>();
            dataDictEN = new Dictionary<string, string>();

            SetDictValueForKey("FormChooseExamCourse.Button.GoBack", "<< ย้อนกลับ", "<< Go Back");
            SetDictValueForKey("FormChooseExamCourse.Topic", "กรุณาเลือกหลักสูตรที่ต้องการสอบ", "Please select the exam course");
            SetDictValueForKey("FormChooseExamCourse.Button.1", "หลักสูตรการสอนขับรถยนต์", "Car Course");
            SetDictValueForKey("FormChooseExamCourse.Button.2", "หลักสูตรการสอนขับรถจักรยานยนต์", "Motorcycle Course");

            SetDictValueForKey("FormInsertSmartCard.Login.Label", "กรุณาสอดบัตรประชาชนของท่าน แล้วกดปุ่ม 'เข้าสู่ระบบ'", "Please insert your ID card and click 'Login' button");
            SetDictValueForKey("FormInsertSmartCard.Login.Button", "เข้าสู่ระบบ", "Login");
            SetDictValueForKey("FormInsertSmartCard.GoBack", "<< ย้อนกลับ", "<< Go Back");
            SetDictValueForKey("FormInsertSmartCard.Error.ReaderNotFound", "ไม่พบเครื่องอ่านบัตรประชาชน กรุณาตรวจสอบเครื่องอ่านบัตรประชาชนอีกครั้ง", "ID Card Reader not found. Please check the ID Card Reader and try again.");
            SetDictValueForKey("FormInsertSmartCard.Error.CardNotFound", "ไม่พบบัตรประชาชน กรุณาสอดบัตรประชาชนของท่าน และกดปุ่ม 'เข้าสู่ระบบ' อีกครั้ง", "ID Card not found. Please insert your ID Card and press 'Login' button again");
            SetDictValueForKey("FormInsertSmartCard.Error.Unknown", "เกิดข้อผิดพลาดบางอย่างขึ้น กรุณาติดต่อผู้ดูแลระบบ", "Some errors occurred. Please contact system administrators.");
            SetDictValueForKey("SmartCardErrorMessageBox.RightButton", "ตกลง", "OK");

            SetDictValueForKey("FormShowUserDetail.Button.TakePhoto", "ถ่ายภาพ", "Take a photo");
            SetDictValueForKey("FormShowUserDetail.Button.DeletePhoto", "ลบรูปภาพ", "Delete photo");
            SetDictValueForKey("FormShowUserDetail.Button.GoBack", "<< ย้อนกลับ", "<< Go Back");
            SetDictValueForKey("FormShowUserDetail.Button.TakeExam", "เริ่มทำข้อสอบ", "Start an exam");
            SetDictValueForKey("FormShowUserDetail.Label.Fullname", "  ชื่อ-นามสกุล", "  Name");
            SetDictValueForKey("FormShowUserDetail.Label.CitizenID", "  เลขประจำตัวประชาชน", "  Citizen ID");
            SetDictValueForKey("FormShowUserDetail.Label.Address", "  ที่อยู่", "  Address");
            SetDictValueForKey("FormShowUserDetail.Label.CourseName", "  หลักสูตรที่เรียน", "  Course Name");
            SetDictValueForKey("FormShowUserDetail.Label.ExamDate", "  วันที่สอบ", "  Exam Date");

            SetDictValueForKey("FormExecuteExam.TimerLabel", "เหลือเวลา", "Time Remain");
            SetDictValueForKey("FormExecuteExam.QuizTextLabel.Header", "ข้อที่", "Question No.");
            SetDictValueForKey("FormExecuteExam.SubmitExamButton", "ส่งข้อสอบ", "Submit Answer");
            SetDictValueForKey("FormExecuteExam.GoBack", "<< ย้อนกลับ", "<< Go Back");

            SetDictValueForKey("QuizChoicePanel.ChoiceHeader.1", "ก", "A");
            SetDictValueForKey("QuizChoicePanel.ChoiceHeader.2", "ข", "B");
            SetDictValueForKey("QuizChoicePanel.ChoiceHeader.3", "ค", "C");
            SetDictValueForKey("QuizChoicePanel.ChoiceHeader.4", "ง", "D");

            SetDictValueForKey("TimeoutMessageBox.Message", "หมดเวลาทำข้อสอบ กรุณากดปุ่ม 'ส่งคำตอบ'", "Time is over, please click 'Submit' button");
            SetDictValueForKey("TimeoutMessageBox.RightButton", "ส่งคำตอบ", "Submit");

            SetDictValueForKey("QuizNotCompletedMessageBox.Message", "คุณยังตอบคำถามไม่ครบทุกข้อ คุณยังต้องการส่งข้อสอบหรือไม่", "You have not answered all questions yet. Do you still want to submit the answers ?");
            SetDictValueForKey("QuizNotCompletedMessageBox.RightButton", "ส่งข้อสอบ", "Submit Answer");
            SetDictValueForKey("QuizNotCompletedMessageBox.LeftButton", "ย้อนกลับ", "Go Back");

            SetDictValueForKey("ConfirmExitMessageBox.Message", "คุณต้องการออกจากโปรแกรมใช่หรือไม่", "Do you want to exit the program ?");
            SetDictValueForKey("ConfirmExitMessageBox.RightButton", "ตกลง", "OK");
            SetDictValueForKey("ConfirmExitMessageBox.LeftButton", "ย้อนกลับ", "Go Back");

            SetDictValueForKey("FinishExamMessageBox.Message", "การสอบได้เสร็จสิ้นแล้ว กรุณาติดต่อเจ้าหน้าที่ เพื่อรับเอกสารแสดงผลการสอบของท่าน", "Examination is now completed. Please contact an officer to receive your examination document ?");
            SetDictValueForKey("FinishExamMessageBox.RightButton", "ตกลง", "OK");

            SetDictValueForKey("GoToExamMessageBox.Message", "คุณพร้อมจะเริ่มต้นการสอบแล้วใช่หรือไม่", "Are you ready to start the exam ?");
            SetDictValueForKey("GoToExamMessageBox.RightButton", "ตกลง", "OK");
            SetDictValueForKey("GoToExamMessageBox.LeftButton", "ย้อนกลับ", "Go Back");

            SetDictValueForKey("NoUserPhotoMessageBox.Message", "กรุณากดปุ่ม 'ถ่ายภาพ' เพื่อถ่ายภาพของท่านก่อน จึงจะสามารถไปทำข้อสอบได้", "Please press 'Take a photo' button before you proceed.");
            SetDictValueForKey("NoUserPhotoMessageBox.RightButton", "ตกลง", "OK");

            SetDictValueForKey("FormExamResult.FinishButton", "เสร็จสิ้น", "Finish");
            SetDictValueForKey("FormExamResult.ViewAnswer", "ดูเฉลย", "View Answer");

            SetDictValueForKey("FormExamResult.PassExam", "คุณสอบผ่าน", "You passed");
            SetDictValueForKey("FormExamResult.FailExam", "คุณสอบไม่ผ่าน", "You failed");
            SetDictValueForKey("FormExamResult.ScoreText", "คุณได้คะแนน ", "You scored ");

            SetDictValueForKey("MockQuiz.QuizText", "สัญลักษณ์นี้หมายความว่าอย่างไร ", "What does this sign mean ?");
            SetDictValueForKey("MockQuiz.1.Choice.1", "ห้ามเลี้ยวซ้าย", "Do not turn left");
            SetDictValueForKey("MockQuiz.1.Choice.2", "ห้ามเลี้ยวขวา", "Do not turn right");
            SetDictValueForKey("MockQuiz.1.Choice.3", "ห้ามกลับรถ", "Do not U-turn");
            SetDictValueForKey("MockQuiz.1.Choice.4", "ห้ามตรงไป", "Do not go straight");

            SetDictValueForKey("MockQuiz.2.Choice.1", "ถนนลื่น", "Slippery road");
            SetDictValueForKey("MockQuiz.2.Choice.2", "ถนนขรุขระ", "Bumpy road");
            SetDictValueForKey("MockQuiz.2.Choice.3", "ห้ามเลี้ยวซ้าย", "Do not turn left");
            SetDictValueForKey("MockQuiz.2.Choice.4", "ห้ามเลี้ยวขวา", "Do not turn right");

            SetDictValueForKey("MockQuiz.3.Choice.1", "จำกัดความเร็ว 40 กม.ต่อชั่วโมง", "Speed limit at 40 km/h");
            SetDictValueForKey("MockQuiz.3.Choice.2", "ห้ามเลี้ยวซ้าย", "Do not turn left");
            SetDictValueForKey("MockQuiz.3.Choice.3", "ถนนลื่น", "Slippery road");
            SetDictValueForKey("MockQuiz.3.Choice.4", "ถนนขรุขระ", "Bumpy road");
        }

        public static LocalizedTextManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new LocalizedTextManager();
                }
                return instance;
            }
        }

        public static void InitInstance()
        {
            if (instance == null)
            {
                instance = new LocalizedTextManager();
            }
        }

        private void SetDictValueForKey(string key, string valueTH, string valueEN)
        {
            dataDictTH.Add(key, valueTH);
            dataDictEN.Add(key, valueEN);
        }

        public static void SetLanguage(int languageMode)
        {
            if (languageMode == 0)
            {
                instance.language = LanguageMode.TH;
            }
            if (languageMode == 1)
            {
                instance.language = LanguageMode.EN;
            }
        }

        public static string GetLocalizedTextForKeyTH(string key)
        {
            string returnValue;
            Dictionary<string, string> targetDict = Instance.dataDictTH;

            if (targetDict.ContainsKey(key))
            {
                returnValue = targetDict[key];
            }
            else
            {
                returnValue = null;
            }

            return returnValue;
        }

        public static string GetLocalizedTextForKey(string key)
        {
            string returnValue;
            Dictionary<string, string> targetDict = Instance.dataDictTH;
           
            if (Instance.language == LanguageMode.EN)
            {
                targetDict = Instance.dataDictEN;
            }

            if (targetDict.ContainsKey(key)) {
                returnValue = targetDict[key];
            } 
            else 
            {
                returnValue = null;
            }
            
            return returnValue;
        }
    }
}
