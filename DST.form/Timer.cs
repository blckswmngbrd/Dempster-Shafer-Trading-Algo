using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IBApi;
using System.Timers;

namespace WindowsFormsApplication3
{
    class Timer
    {
        public System.Timers.Timer DecisionTimer;
        public System.Threading.AutoResetEvent ReEval;
        private int d_Counter;

        public Timer()//Constructor
        {
        ReEval = new System.Threading.AutoResetEvent(true);
        DecisionTimer = new System.Timers.Timer( 15000 );
        DecisionTimer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimedEvent );
        DecisionTimer.Enabled = true;
        DecisionTimer.Start();
        }

        public System.Threading.AutoResetEvent get_ReEval()
        {
            return ReEval;
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            if (d_Counter == 15)
            {
                DecisionTimer.Stop();
                ReEval.Set();
            }
        }
    }
}
