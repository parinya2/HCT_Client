using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace HCT_Client
{
    public enum ExamCourseType
    { 
        None,
        Car,
        Motorcycle
    };

    public enum QuizResultPassFlag
    {
        None,
        Pass,
        Fail,
        NotCompleted
    };

    public class QuizResult
    {
        public QuizResultPassFlag passFlag;
        public string quizScore;
        public string quizResultPdfBase64String;

        public QuizResult()
        {
            ClearAll();
        }

        public void ClearAll()
        {
            passFlag = QuizResultPassFlag.None;
            quizScore = null;
            quizResultPdfBase64String = null;
        }
    }

    public class QuizManager
    {
        static string COURSE_CODE_CAR = "201";
        static string COURSE_CODE_MOTORCYCLE = "301";

        Random randomGenerator;
        ExamCourseType examCourseType;
        QuizResult quizResult;
        string examCourseCode; // 201 = Car , 301 = Motorcycle
        string paperTestNumber; // หมายเลขชุดข้อสอบ
        DateTime examStartDateTime;
        DateTime examEndDateTime;
        SingleQuizObject[] quizArray;

        private static QuizManager instance;
        
        public QuizManager()
        { 
        
        }

        public static QuizManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new QuizManager();
                }
                return instance;
            }
        }

        public static void InitInstance()
        {
            if (instance == null)
            {
                instance = new QuizManager();
            }
        }

        private static string RandomSimulatorQuizNumber()
        {
            string simulatorConfigFilePath = Util.GetSimulatorQuizFolderPath() + "/SimulatorSetting.txt";
            string[] contentsArr = File.ReadAllLines(simulatorConfigFilePath);
            bool shouldRandom = false;
            string targetQuizNumber = "1";
            if (contentsArr.Length >= 2)
            {
                string[] line1Arr = contentsArr[0].Split('=');
                shouldRandom = line1Arr[1].ToLower().Contains("y");

                string[] line2Arr = contentsArr[1].Split('=');
                targetQuizNumber = line2Arr[1];
            }
            else
            {
                return "1";
            }

            if (shouldRandom)
            {
                int totolFolderCount = Directory.GetDirectories(Util.GetSimulatorQuizFolderPath()).Length;
                int oldNumber = Int32.Parse(targetQuizNumber);
                int newNumber = (oldNumber == totolFolderCount) ? 1 : oldNumber + 1;

                contentsArr[1] = contentsArr[1].Replace("=" + oldNumber, "=" + newNumber);
                File.WriteAllLines(simulatorConfigFilePath, contentsArr);
            }
            return targetQuizNumber;
        }

        public static void GenerateQuizFromSimulatorFolder()
        {
            /* ข้อมูล Simulator หน้าตาแบบนี้
             7|ผู้ขับรถกระทำผิดตามกฎหมายจราจรทางบกและได้รับใบสั่งจากเจ้าพนักงานจราจรต้องไปติดต่อชำระค่าปรับภายในกี่วัน$x|
             10 วัน$x|7 วัน$x|15 วัน$x|30 วัน$x|1#
             8|เมื่อใบอนุญาตขับรถสูญหายหรือชำรุดต้องยื่นขอรับใบแทนต่อนายทะเบียนภายในกี่วัน$x|
             20 วัน$x|30 วัน$x|15 วัน$x|45 วัน$x|2# 
             */
            string quizSetNumber = RandomSimulatorQuizNumber();
            string simulatorFolderPath = Util.GetSimulatorQuizFolderPath() + "/QuizSet" + quizSetNumber;
            string simulatorImageFolderPath = simulatorFolderPath + "/SimulatorImages";
            string quizDataString = File.ReadAllText(simulatorFolderPath + "/SimulatorQuiz.txt");
            quizDataString = quizDataString.Replace(Environment.NewLine, "");

            List<SingleQuizObject> quizList = new List<SingleQuizObject>();
            string[] quizStrArray = quizDataString.Split('#');
            for (int i = 0; i < quizStrArray.Length; i++)
            {         
                SingleQuizObject quizObj = new SingleQuizObject();
                string tmpQuizStr = quizStrArray[i].Trim();
                string[] tmpArr = tmpQuizStr.Split('|');

                if (tmpArr.Length == 7)
                {
                    quizObj.paperQuestSeq = tmpArr[0].Trim(); //หมายเลขโจทย์
                    quizObj.correctChoice = Int32.Parse(tmpArr[6].Trim()) - 1;

                    string[] tmpQuest_Arr = tmpArr[1].Split('$');
                    quizObj.quizText = tmpQuest_Arr[0].Trim();
                    quizObj.quizImage = tmpQuest_Arr[1].Trim().ToLower().Equals("x") ? null : (Bitmap)Image.FromFile(simulatorImageFolderPath + "/" + tmpQuest_Arr[1].Trim());
                    quizObj.quizSoundPlayer = Util.GetSoundPlayerFromSoundResources("SampleSound.wav");
                    
                    List<SingleChoiceObject> choiceList = new List<SingleChoiceObject>();
                    for (int m = 2; m <= 5; m++)
                    {
                        SingleChoiceObject choiceObj = new SingleChoiceObject();
                        string[] tmpChoice_Arr = tmpArr[m].Split('$');
                        choiceObj.choiceText = tmpChoice_Arr[0].Trim();
                        choiceObj.choiceImage = tmpChoice_Arr[1].Trim().ToLower().Equals("x") ? null : (Bitmap)Image.FromFile(simulatorImageFolderPath + "/" + tmpChoice_Arr[1].Trim());
                        choiceObj.choiceSoundPlayer = Util.GetSoundPlayerFromSoundResources("SampleSound.wav");

                        choiceList.Add(choiceObj);
                    }
                    quizObj.choiceObjArray = choiceList.ToArray();
                    quizList.Add(quizObj);
                }                
            }
            instance.quizArray = quizList.ToArray();
            instance.paperTestNumber = quizSetNumber;
        }

        public static void MockQuiz()
        { 
            instance.quizArray = new SingleQuizObject[50];

            System.Reflection.Assembly myAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            Stream myStream = myAssembly.GetManifestResourceStream("HCT_Client.Images.Q1image.jpg");
            Bitmap Q1image = new Bitmap(myStream);

            myStream = myAssembly.GetManifestResourceStream("HCT_Client.Images.Q2image.jpg");
            Bitmap Q2image = new Bitmap(myStream);

            myStream = myAssembly.GetManifestResourceStream("HCT_Client.Images.Q3image.jpg");
            Bitmap Q3image = new Bitmap(myStream);

            for (int i = 0; i < instance.quizArray.Length; i++)
            {
                SingleQuizObject obj = new SingleQuizObject();
                obj.quizText = LocalizedTextManager.GetLocalizedTextForKey("MockQuiz.QuizText");
                obj.quizSoundPlayer = Util.GetSoundPlayerFromSoundResources("SampleSound.wav");

                if (i % 3 == 0) obj.quizImage = Q1image;
                if (i % 3 == 1) obj.quizImage = Q2image;
                if (i % 3 == 2) obj.quizImage = Q3image;

                for (int k = 0; k < 4; k++)
                {
                    obj.choiceObjArray[k] = new SingleChoiceObject();
                    obj.choiceObjArray[k].choiceText = LocalizedTextManager.GetLocalizedTextForKey("MockQuiz." + (i % 3 + 1) + ".Choice." + (k + 1));
                    obj.choiceObjArray[k].choiceSoundPlayer = Util.GetSoundPlayerFromSoundResources("SampleSound.wav");
                }

                obj.correctChoice = 0;
                instance.quizArray[i] = obj;
            }
        }

        public static bool isAllQuestionsAnswered()
        {
            for (int i = 0; i < instance.quizArray.Length; i++)
            {
                SingleQuizObject obj = instance.quizArray[i];
                if (obj.selectedChoice == -1)
                {
                    return false;
                }
            }
            return true;
        }

        public static void SetQuizArray(SingleQuizObject[] arr)
        {
            instance.quizArray = arr;
        }

        public static SingleQuizObject[] GetQuizArray()
        {
            return instance.quizArray;
        }

        public static void SetExamCourseType(ExamCourseType type)
        {
            instance.examCourseType = type;
            if (type == ExamCourseType.Car)
            {
                instance.examCourseCode = COURSE_CODE_CAR;
            }
            else if (type == ExamCourseType.Motorcycle)
            {
                instance.examCourseCode = COURSE_CODE_MOTORCYCLE;
            }
        }

        public static ExamCourseType GetExamCourseType()
        {
            return instance.examCourseType;
        }

        public static void SetPaperTestNumber(string testNo)
        {
            instance.paperTestNumber = testNo;
        }

        public static string GetPaperTestNumber()
        {
            return instance.paperTestNumber;
        }

        public static string GetExamCourseCode()
        {
            return instance.examCourseCode;
        }

        public static Random GetRandomGenerator()
        {
            if (instance.randomGenerator == null)
            {
                instance.randomGenerator = new Random();
            }
            return instance.randomGenerator;
        }

        public static QuizResult GetQuizResult()
        {
            if (instance.quizResult == null)
            {
                instance.quizResult = new QuizResult();
            }
            return instance.quizResult;
        }

        public static void SetExamStartDateTime(DateTime dt)
        {
            instance.examStartDateTime = dt;
        }

        public static DateTime GetExamStartDateTime()
        {
            return instance.examStartDateTime;
        }

        public static void SetExamEndDateTime(DateTime dt)
        {
            instance.examEndDateTime = dt;
        }

        public static DateTime GetExamEndDateTime()
        {
            return instance.examEndDateTime;
        }

        public static void ClearAllData()
        {
            instance.examCourseCode = null;
            instance.examCourseType = ExamCourseType.None;
            instance.examStartDateTime = DateTime.Now;
            instance.examEndDateTime = DateTime.Now;
            instance.paperTestNumber = null;
            instance.quizArray = null;
            instance.quizResult = null;            
        }
    }
}
