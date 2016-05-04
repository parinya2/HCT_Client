using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HCT_Client
{
    public class QuizManager
    {
        SingleQuizObject[] quizArray;

        public QuizManager()
        { 
        
        }

        public void LoadQuiz()
        {
            MockQuiz();
        }

        private void MockQuiz()
        { 
            quizArray = new SingleQuizObject[50];
            for (int i = 0; i < quizArray.Length; i++)
            {
                SingleQuizObject obj = new SingleQuizObject();
                obj.quizText = "QuizText " + (i + 1);
                for (int k = 0; k < 4; k++)
                {
                    obj.SetChoiceText(k, "Choice " + i + " : " + k);
                }
                    

                quizArray[i] = obj;
            }
        }

        public SingleQuizObject[] GetQuizArray()
        {
            return quizArray;
        }
    }
}
