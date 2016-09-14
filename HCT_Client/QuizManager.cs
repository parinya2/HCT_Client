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
        Car,
        Motorcycle
    };

    public class QuizResult
    {
        string passFlag;
        int quizScore;

        public QuizResult()
        {
            passFlag = null;
            quizScore = -1;
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

        public static void LoadQuiz()
        {
            WebServiceManager.GetEExamQuestionFromServer();
        }

        private static void MockQuiz()
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

                if (i % 3 == 0) obj.quizImage = Q1image;
                if (i % 3 == 1) obj.quizImage = Q2image;
                if (i % 3 == 2) obj.quizImage = Q3image;

                for (int k = 0; k < 4; k++)
                {
                   obj.choiceObjArray[k].choiceText = LocalizedTextManager.GetLocalizedTextForKey("MockQuiz." + (i % 3 + 1) + ".Choice." + (k + 1));
                }

                obj.correctChoice = 0;
                instance.quizArray[i] = obj;
            }
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
    }
}
