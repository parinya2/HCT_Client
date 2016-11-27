using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Media;

namespace HCT_Client
{
    public class SingleChoiceObject
    {
        public string choiceText;
        public string choiceCode;
        public Bitmap choiceImage;
        public SoundPlayer choiceSoundPlayer;
    }

    public class SingleQuizObject
    {
        public string paperQuestSeq;
        public string quizText;
        public string quizCode;
        public Bitmap quizImage;
        public SingleChoiceObject[] choiceObjArray;
        public int correctChoice;
        public int selectedChoice;
        public SoundPlayer quizSoundPlayer;

        public SingleQuizObject()
        {
            choiceObjArray = new SingleChoiceObject[4];
            selectedChoice = -1;
            correctChoice = -1;
        }

        public void shuffleChoice()
        {
            Random rnd = QuizManager.GetRandomGenerator();
            int shuffleRoundCount = 20;
            for (int i = 0; i < shuffleRoundCount; i++)
            {
                int idx1 = rnd.Next(0, 4);
                int idx2 = rnd.Next(0, 4);
                if (idx1 != idx2)
                {
                    SingleChoiceObject tmp = choiceObjArray[idx1];
                    choiceObjArray[idx1] = choiceObjArray[idx2];
                    choiceObjArray[idx2] = tmp;
                }
            }
        }
    }
}
