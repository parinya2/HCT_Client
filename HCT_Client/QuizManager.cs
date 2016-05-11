using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace HCT_Client
{
    public class QuizManager
    {
        public int examCourseType; // 0 = Car , 1 = Motorcycle
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
            MockQuiz();
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
                    obj.SetChoiceText(k, LocalizedTextManager.GetLocalizedTextForKey("MockQuiz." + (i % 3 + 1) + ".Choice." + (k + 1)));
                }

                obj.correctChoice = 0;



                instance.quizArray[i] = obj;
            }
        }

        public static SingleQuizObject[] GetQuizArray()
        {
            return instance.quizArray;
        }

        public static void SetExamCourseType(int type)
        {
            instance.examCourseType = type;
        }

        public static int GetExamCourseType()
        {
            return instance.examCourseType;
        }
    }
}
