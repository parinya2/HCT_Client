using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HCT_Client
{
    public class Stopwatch
    {
        private Timer timer;
        private int totalSeconds;
        private string theTime;
        private bool runningFlag;

        public string TheTime
        {
            get
            {
                return theTime;
            }
            set
            {
                theTime = value;
                OnTheTimeChanged(this.theTime);
            }
        }

        public Stopwatch(int totalSeconds)
        {
            this.totalSeconds = totalSeconds;
            this.runningFlag = true;
            timer = new Timer();
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Interval = 1000;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (runningFlag)
            {
                if (totalSeconds >= 0)
                {
                    TheTime = GetOfficialTimeText(totalSeconds);
                    totalSeconds--;
                }
                else
                {
                    TheTime = null;
                } 
            }
        }

        public void stopRunning()
        {
            runningFlag = false;
        }

        private string GetOfficialTimeText(int seconds)
        {
            int SECS_PER_HOUR = 3600;
            int SECS_PER_MINUTE = 60;
            int remainingSeconds = seconds;
            int hours = remainingSeconds / SECS_PER_HOUR;
            remainingSeconds -= hours * SECS_PER_HOUR;

            int minutes = remainingSeconds / SECS_PER_MINUTE;
            remainingSeconds -= minutes * SECS_PER_MINUTE;

            string hourStr = hours < 10 ? ("0" + hours) : ("" + hours);
            string minuteStr = minutes < 10 ? ("0" + minutes): ("" + minutes);
            string secStr = remainingSeconds < 10 ? ("0" + remainingSeconds) : ("" + remainingSeconds);
            return LocalizedTextManager.GetLocalizedTextForKey("FormExecuteExam.TimerLabel") + "  "
                    + hourStr + " : " + minuteStr + " : " + secStr;
        }

        public delegate void TimerTickHandler(string newTime);
        public event TimerTickHandler TheTimeChanged;

        protected void OnTheTimeChanged(string newTime)
        {
            if (TheTimeChanged != null)
            {
                TheTimeChanged(newTime);
            }
        }
    }
}
