using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HCT_Client
{
    public class QuizManager
    {
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
            for (int i = 0; i < instance.quizArray.Length; i++)
            {
                SingleQuizObject obj = new SingleQuizObject();
                obj.quizText = "QuizText " + (i + 1);
                obj.correctChoice = 0;
                for (int k = 0; k < 4; k++)
                {
                    obj.SetChoiceText(k, "Choice " + i + " : " + k);
                }


                instance.quizArray[i] = obj;
            }
        }

        public static SingleQuizObject[] GetQuizArray()
        {
            return instance.quizArray;
        }
    }
}
