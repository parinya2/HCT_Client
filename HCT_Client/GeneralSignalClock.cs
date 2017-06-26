using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HCT_Client
{
    public class GeneralSignalClock
    {
        private Timer timer;
        private int signalState = 1;
        private int totalStateCount = 20;
        private int interval = 1;

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

        public GeneralSignalClock()
        {
            timer = new Timer();
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Interval = interval;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (SignalState < totalStateCount)
            {
                SignalState++;
            }
            else if (SignalState == totalStateCount)
            {
                SignalState = 1;
            }
        }

        public delegate void GeneralSignalClockTickHandler(int newTime);
        public event GeneralSignalClockTickHandler TheTimeChanged;

        protected void OnTheTimeChanged(int newTime)
        {
            if (TheTimeChanged != null)
            {
                TheTimeChanged(newTime);
            }
        }
    }
}
