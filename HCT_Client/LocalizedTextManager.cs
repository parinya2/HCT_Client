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

            SetDictValueForKey("FormExecuteExam.TimerLabel", "เหลือเวลา", "Time Remain");
            SetDictValueForKey("FormExecuteExam.QuizTextLabel.Header", "ข้อที่", "Question No.");
            SetDictValueForKey("FormExecuteExam.SubmitExamButton", "ส่งข้อสอบ", "Submit Answer");
            SetDictValueForKey("FormExecuteExam.GoBack", "ย้อนกลับ", "Go Back");

            SetDictValueForKey("QuizChoicePanel.ChoiceHeader.1", "ก", "A");
            SetDictValueForKey("QuizChoicePanel.ChoiceHeader.2", "ข", "B");
            SetDictValueForKey("QuizChoicePanel.ChoiceHeader.3", "ค", "C");
            SetDictValueForKey("QuizChoicePanel.ChoiceHeader.4", "ง", "D");

            SetDictValueForKey("TimeoutMessageBox.Message", "หมดเวลาทำข้อสอบ กรุณากดปุ่ม 'ส่งคำตอบ'", "Time is over, please click 'Submit' button");
            SetDictValueForKey("TimeoutMessageBox.RightButton", "ส่งคำตอบ", "Submit");

            SetDictValueForKey("QuizNotCompletedMessageBox.Message", "คุณยังตอบคำถามไม่ครบทุกข้อ คุณยังต้องการส่งข้อสอบหรือไม่", "You have not answered all questions yet. Do you still want to submit the answers ?");
            SetDictValueForKey("QuizNotCompletedMessageBox.RightButton", "ส่งข้อสอบ", "Submit Answer");
            SetDictValueForKey("QuizNotCompletedMessageBox.LeftButton", "ยกเลิก", "Cancel");

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
