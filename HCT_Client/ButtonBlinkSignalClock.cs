using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HCT_Client
{
    public class BlinkButtonSignalClock
    {
        private Timer timer;
        private int signalState = 1;
        private int totalStateCount = 40;
        private int interval = 1;
        private bool goFarward = true;

        public int SignalState
        {
            get
            {
                return signalState;
            }
            set
            {
                signalState = value;
                OnTheTimeChanged(this.signalState);
            }
        }

        public BlinkButtonSignalClock()
        {
            timer = new Timer();
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Interval = interval;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (SignalState > 1 && SignalState < totalStateCount)
            {
                SignalState = goFarward ? SignalState + 1 : SignalState - 1;
            }
            else if (SignalState ==totalStateCount)
            {
                SignalState--;
                goFarward = false;
            }
            else if (SignalState == 1)
            {
                SignalState++;
                goFarward = true;
            }
        }

        public delegate void BlinkButtonSignalClockTickHandler(int newTime);
        public event BlinkButtonSignalClockTickHandler TheTimeChanged;

        protected void OnTheTimeChanged(int newTime)
        {
            if (TheTimeChanged != null)
            {
                TheTimeChanged(newTime);
            }
        }
    }
}
