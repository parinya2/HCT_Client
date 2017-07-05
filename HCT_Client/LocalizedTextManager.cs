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

        public static string PARAM_1 = "<param1>";
        public static string PARAM_2 = "<param2>";

        private LocalizedTextManager()
        {
            dataDictTH = new Dictionary<string, string>();
            dataDictEN = new Dictionary<string, string>();

            SetDictValueForKey("FormChooseExamCourse.Button.GoBack", "<< ย้อนกลับ", "<< Go Back");
            SetDictValueForKey("FormChooseExamCourse.Topic", "กรุณาเลือกหลักสูตรที่ต้องการสอบ", "Please select the exam course");
            SetDictValueForKey("FormChooseExamCourse.Button." + ExamCourseType.Car, "หลักสูตรการสอนขับรถยนต์", "Car Course");
            SetDictValueForKey("FormChooseExamCourse.Button." + ExamCourseType.Motorcycle, "หลักสูตรการสอนขับรถจักรยานยนต์", "Motorcycle Course");

            SetDictValueForKey("FormInsertSmartCard.Login.Label", "กรุณาสอดบัตรประชาชนของท่าน แล้วกดปุ่ม 'เข้าสู่ระบบ'", "กรุณาสอดบัตรประชาชนของท่าน แล้วกดปุ่ม 'เข้าสู่ระบบ'");
            SetDictValueForKey("FormInsertSmartCard.Login.Button", "เข้าสู่ระบบ", "เข้าสู่ระบบ");
            SetDictValueForKey("FormInsertSmartCard.Passport.Label", "Please enter your passport number", "Please enter your passport no.");
            SetDictValueForKey("FormInsertSmartCard.Passport.Button", "Passport No.", "Passport No.");
            SetDictValueForKey("FormInsertSmartCard.GoBack", "<< ย้อนกลับ", "<< Go Back");
            SetDictValueForKey("FormInsertSmartCard.Error.ReaderNotFound", "ไม่พบเครื่องอ่านบัตรประชาชน \n กรุณาตรวจสอบเครื่องอ่านบัตรประชาชนอีกครั้ง", "ID Card Reader not found. Please check the ID Card Reader and try again.");
            SetDictValueForKey("FormInsertSmartCard.Error.CardNotFound", "ไม่พบบัตรประชาชน \n กรุณาสอดบัตรประชาชนของท่าน และกดปุ่ม 'เข้าสู่ระบบ' อีกครั้ง", "ID Card not found. Please insert your ID Card and press 'Login' button again");
            SetDictValueForKey("FormInsertSmartCard.Error.Unknown", "เกิดข้อผิดพลาดบางอย่างขึ้น กรุณาติดต่อผู้ดูแลระบบ", "Some errors occurred. Please contact system administrators.");
            SetDictValueForKey("SmartCardErrorMessageBox.RightButton", "ตกลง", "OK");

            SetDictValueForKey("FormPassport.Passport.Label", "Passport Number", "Passport Number");
            SetDictValueForKey("FormPassport.Login.Button", "เข้าสู่ระบบ", "Login");
            SetDictValueForKey("FormPassport.Delete.Button", "ลบ", "Delete");

            SetDictValueForKey("FormCourseRegisterSetting.Button.GoBack", "<< ย้อนกลับ", "<< Go Back");
            SetDictValueForKey("FormCourseRegisterSetting.Button.Next", "ถัดไป >>", "Next >>");
            SetDictValueForKey("FormCourseRegisterSetting.Topic.CourseRegisterDate", "กรุณาระบุวันที่ท่านสมัครเรียน", "Please specify your course registration date.");
            SetDictValueForKey("FormCourseRegisterSetting.Topic.ExamSeq", "สอบครั้งที่", "Exam Seq.");
            SetDictValueForKey("FormCourseRegisterSetting.Label.Day", "วันที่", "Day");
            SetDictValueForKey("FormCourseRegisterSetting.Label.Month", "เดือน", "Month");
            SetDictValueForKey("FormCourseRegisterSetting.Label.Year", "ปี", "Year");
            SetDictValueForKey("FormCourseRegisterSetting.Label.ExamSeq", "สอบครั้งที่", "Exam Seq");

            SetDictValueForKey("FormCourseRegisterSetting.Month.1", "มกราคม", "January");
            SetDictValueForKey("FormCourseRegisterSetting.Month.2", "กุมภาพันธ์", "February");
            SetDictValueForKey("FormCourseRegisterSetting.Month.3", "มีนาคม", "March");
            SetDictValueForKey("FormCourseRegisterSetting.Month.4", "เมษายน", "April");
            SetDictValueForKey("FormCourseRegisterSetting.Month.5", "พฤษภาคม", "May");
            SetDictValueForKey("FormCourseRegisterSetting.Month.6", "มิถุนายน", "June");
            SetDictValueForKey("FormCourseRegisterSetting.Month.7", "กรกฏาคม", "July");
            SetDictValueForKey("FormCourseRegisterSetting.Month.8", "สิงหาคม", "August");
            SetDictValueForKey("FormCourseRegisterSetting.Month.9", "กันยายน", "September");
            SetDictValueForKey("FormCourseRegisterSetting.Month.10", "ตุลาคม", "October");
            SetDictValueForKey("FormCourseRegisterSetting.Month.11", "พฤศจิกายน", "November");
            SetDictValueForKey("FormCourseRegisterSetting.Month.12", "ธันวาคม", "December");

            SetDictValueForKey("FormShowUserDetail.Button.TakePhoto", "ถ่ายภาพ", "Take a photo");
            SetDictValueForKey("FormShowUserDetail.Button.DeletePhoto", "ลบรูปภาพ", "Delete photo");
            SetDictValueForKey("FormShowUserDetail.Button.GoBack", "<< ย้อนกลับ", "<< Go Back");
            SetDictValueForKey("FormShowUserDetail.Button.TakeExam", "เริ่มทำข้อสอบ >>", "Start an exam >>");
            SetDictValueForKey("FormShowUserDetail.Label.Fullname", "  ชื่อ-นามสกุล", "  Name");
            SetDictValueForKey("FormShowUserDetail.Label.CitizenID", "  เลขประจำตัวประชาชน", "  Citizen ID");
            SetDictValueForKey("FormShowUserDetail.Label.PassportID", "  หมายเลข Passport", "  Passport ID");
            SetDictValueForKey("FormShowUserDetail.Label.Address", "  ที่อยู่", "  Address");
            SetDictValueForKey("FormShowUserDetail.Label.CourseName", "  หลักสูตรที่เรียน", "  Course Name");
            SetDictValueForKey("FormShowUserDetail.Label.ExamDate", "  วันที่สอบ", "  Exam Date");

            SetDictValueForKey("FormExecuteExam.TimerLabel", "เหลือเวลา", "Time Remain");
            SetDictValueForKey("FormExecuteExam.QuizTextLabel.Header", "ข้อที่", "Question No.");
            SetDictValueForKey("FormExecuteExam.SubmitExamButton", "ส่งข้อสอบ", "Submit");
            SetDictValueForKey("FormExecuteExam.GoBack", "<< ย้อนกลับ", "<< Go Back");
            SetDictValueForKey("FormExecuteExam.AnsweredCount", "ตอบแล้ว " + PARAM_1 + " ข้อ ยังไม่ตอบ " + PARAM_2 + " ข้อ", "Answered " + PARAM_1 + " Unanswered " + PARAM_2);
            SetDictValueForKey("FormExecuteExam.PrevQuiz", "ข้อก่อนหน้า", "Previous");
            SetDictValueForKey("FormExecuteExam.NextQuiz", "ข้อถัดไป", "Next");

            SetDictValueForKey("QuizChoicePanel.ChoiceHeader.1", "ก", "A");
            SetDictValueForKey("QuizChoicePanel.ChoiceHeader.2", "ข", "B");
            SetDictValueForKey("QuizChoicePanel.ChoiceHeader.3", "ค", "C");
            SetDictValueForKey("QuizChoicePanel.ChoiceHeader.4", "ง", "D");

            SetDictValueForKey("TimeoutMessageBox.Message", "หมดเวลาทำข้อสอบ กรุณากดปุ่ม 'ส่งคำตอบ'", "Time is over, please click 'Submit' button");
            SetDictValueForKey("TimeoutMessageBox.RightButton", "ส่งคำตอบ", "Submit");

            SetDictValueForKey("QuizNotCompletedMessageBox.Message", "คุณต้องตอบคำถามให้ครบทุกข้อ มิฉะนั้นจะสอบไม่ผ่าน" + Environment.NewLine + " ข้อที่ยังไม่ตอบ ได้แก่ ", 
                                                                     "You must answer all questions, otherwise you will fail." + Environment.NewLine + "Unanswered questions are ");
            SetDictValueForKey("QuizNotCompletedMessageBox.RightButton", "ย้อนกลับ", "Go Back");

            SetDictValueForKey("ConfirmExitMessageBox.Message", "คุณต้องการออกจากโปรแกรมใช่หรือไม่", "Do you want to exit the program ?");
            SetDictValueForKey("ConfirmExitMessageBox.RightButton", "ตกลง", "OK");
            SetDictValueForKey("ConfirmExitMessageBox.LeftButton", "ย้อนกลับ", "Go Back");

            SetDictValueForKey("FinishExamMessageBox.Message", "การสอบได้เสร็จสิ้นแล้ว กรุณาติดต่อเจ้าหน้าที่ เพื่อรับเอกสารแสดงผลการสอบของท่าน", "Examination is now completed. Please contact an officer to receive your examination document");
            SetDictValueForKey("FinishExamMessageBox.RightButton", "ตกลง", "OK");

            SetDictValueForKey("GoToExamMessageBox.Message", "คุณพร้อมจะเริ่มต้นการสอบแล้วใช่หรือไม่", "Are you ready to start the exam ?");
            SetDictValueForKey("GoToExamMessageBox.RightButton", "ตกลง", "OK");
            SetDictValueForKey("GoToExamMessageBox.LeftButton", "ย้อนกลับ", "Go Back");

            SetDictValueForKey("SystemProcessingMessageBox.Message", "ระบบกำลังดำเนินการ กรุณารอสักครู่ ...", "Please wait while the system is processing ...");

            SetDictValueForKey("ErrorMessageBox." + WebServiceResultStatus.ERROR_HTTP_TIMEOUT + ".Message", "ไม่มีการตอบกลับจาก Server ของกรมการขนส่งทางบก กรุณารอ 5 วินาที แล้วลองอีกครั้ง", "Server did not response, please wait for 5 seconds and try again");
            SetDictValueForKey("ErrorMessageBox." + WebServiceResultStatus.ERROR_REMOTE_NAME_NOT_RESOLVED + ".Message", "ไม่สามารถติดต่อ Server ได้ กรุณาตรวจสอบ Internet ของท่าน", "Unable to connect to the server, please check your internet connection.");
            SetDictValueForKey("ErrorMessageBox." + WebServiceResultStatus.ERROR_INVALID_PARAMETERS + ".Message", "ข้อมูลของท่านในฐานข้อมูลของโรงเรียนมีบางอย่างไม่ถูกต้อง เช่น วันที่สมัครเรียน ครั้งที่สอบ หลักสูตรที่สมัคร ฯลฯ  กรุณาติดต่อเจ้าหน้าที่", "Some information in School Database is incorrect, please contact an officer.");           
            SetDictValueForKey("ErrorMessageBox." + WebServiceResultStatus.ERROR_99 + ".Message", "เกิดข้อผิดพลาดบางอย่างที่ Server กรมการขนส่งทางบก  กรุณาติดต่อเจ้าหน้าที่", "Something went wrong at DLT server, please contact an officer.");
            SetDictValueForKey("ErrorMessageBox." + WebServiceResultStatus.ERROR_STUDENT_DETAIL_NOT_FOUND + ".Message", "ไม่พบชื่อของท่านในรายชื่อผู้มีสิทธิ์สอบจากกรมการขนส่งทางบก กรุณาติดต่อเจ้าหน้าที่", "Your name was not found in students list. Please contact an officer.");
            SetDictValueForKey("ErrorMessageBox." + WebServiceResultStatus.ERROR_SERVER_INTERNAL + ".Message", "Server มีปัญหาภายใน กรุณาติดต่อเจ้าหน้าที่", "Internal Server error");
            SetDictValueForKey("ErrorMessageBox." + WebServiceResultStatus.ERROR_LOAD_EEXAM_EMPTY_RESPONSE + ".Message", "Server ไม่ส่งข้อมูลชุดคำถามกลับมาให้ ท่านอาจกรอกข้อมูลครั้งที่สอบผิด กรุณาติดต่อเจ้าหน้าที่", "Server returned an empty questions data, you might choose wrong exam seq. Please contact an officer.");
            SetDictValueForKey("ErrorMessageBox." + WebServiceResultStatus.ERROR_CHECK_EEXAM_RESULT_EMPTY_RESPONSE + ".Message", "Server ไม่ส่งข้อมูลผลการสอบกลับมาให้ กรุณาติดต่อเจ้าหน้าที่", "Server returned an empty exam result, please contact an officer.");
            SetDictValueForKey("ErrorMessageBox." + WebServiceResultStatus.ERROR_STUDENT_ENROL_NOT_FOUND + ".Message", "ไม่พบข้อมูลการลงทะเบียนเรียนของท่านในฐานข้อมูลของโรงเรียน กรุณาติดต่อเจ้าหน้าที่", "Your course registration detail not found, please contact an officer.");
            SetDictValueForKey("ErrorMessageBox." + WebServiceResultStatus.ERROR_CANNOT_CREATE_SSL_TLS_CHANNEL + ".Message", "ไม่สามารถเชื่อมต่อแบบ Secure Channel กับ Server ได้ กรุณาติดต่อเจ้าหน้าที่", "Could not create SSL/TLS secure channel with Server, please contact an officer.");
            SetDictValueForKey("ErrorMessageBox.RightButton", "ตกลง", "OK");

            SetDictValueForKey("NoUserPhotoMessageBox.Message", "กรุณากดปุ่ม 'ถ่ายภาพ' เพื่อถ่ายภาพของท่านก่อน จึงจะสามารถไปทำข้อสอบได้", "Please press 'Take a photo' button before you proceed.");
            SetDictValueForKey("NoUserPhotoMessageBox.RightButton", "ตกลง", "OK");

            SetDictValueForKey("CourseRegisterDataMissingMessageBox.Message", "กรุณาระบุข้อมูลวันที่สมัครเรียนและครั้งทีสอบให้ครบถ้วนก่อน", "Please complete all necessary information.");
            SetDictValueForKey("CourseRegisterDataMissingMessageBox.RightButton", "ตกลง", "OK");

            SetDictValueForKey("FormExamResult.FinishButton", "พิมพ์ผลสอบ", "Print Exam Result");
            SetDictValueForKey("FormExamResult.ViewAnswer", "ดูเฉลย", "View Answer");

            SetDictValueForKey("FormExamResult.PassExam", "คุณสอบผ่าน", "You passed");
            SetDictValueForKey("FormExamResult.FailExam", "คุณสอบไม่ผ่าน", "You failed");
            SetDictValueForKey("FormExamResult.ScoreText", "คุณได้คะแนน ", "You scored ");

            SetDictValueForKey("SimulatorQuizName.NotFound", "ไม่พบชื่อข้อสอบ", "Simulator Quiz Name not found ");
            SetDictValueForKey("TakePhotoCountdown.Ready", "เตรียมตัว", "Ready");


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

        public static LanguageMode GetLanguage()
        {
            return instance.language;
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
