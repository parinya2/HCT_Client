using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HCT_Client
{
    public class FormsManager
    {
        private Form1 instanceForm1;
        private FormExecuteExam instanceFormExecuteExam;
        private static FormsManager instance;

        private FormsManager()
        {
            
        }

        public static FormsManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new FormsManager();
                }
                return instance;
            }
        }

        public static void InitInstance()
        {
            if (instance == null)
            {
                instance = new FormsManager();
            }
        }

        public static Form1 GetForm1()
        {
            if (Instance.instanceForm1 == null)
            {
                Instance.instanceForm1 = new Form1();
            }
            return Instance.instanceForm1;
        }

        public static FormExecuteExam GetFormExecuteExam()
        {
            if (Instance.instanceFormExecuteExam == null)
            {
                Instance.instanceFormExecuteExam = new FormExecuteExam();
            }
            return Instance.instanceFormExecuteExam;
        }
    }
}
