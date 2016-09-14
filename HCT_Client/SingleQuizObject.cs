using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace HCT_Client
{
    public class SingleChoiceObject
    {
        public string choiceText;
        public string choiceCode;
        public int choiceSeq;
        public Bitmap choiceImage;
    }

    public class SingleQuizObject
    {
        public string quizText;
        public string quizCode;
        public Bitmap quizImage;
        public string[] choiceTextArray;
        public int correctChoice;
        public int selectedChoice;

        public SingleQuizObject()
        {
            choiceTextArray = new string[4];
            selectedChoice = -1;
        }

        public void SetChoiceText(int index, string text)
        {
            choiceTextArray[index] = text;
        }

        public string GetChoiceText(int index)
        { 
            return choiceTextArray[index];
        }
    }
}
