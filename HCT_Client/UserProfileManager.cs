using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HCT_Client
{
    public class UserProfileManager
    {
        private static UserProfileManager instance;
        string citizenID;

        public UserProfileManager()
        { 
        
        }

        public static UserProfileManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new UserProfileManager();
                }
                return instance;
            }
        }

        public static void InitInstance()
        {
            if (instance == null)
            {
                instance = new UserProfileManager();
            }
        }

        public static string GetCitizenID()
        {
            return instance.citizenID;
        }

        public static void SetCitizenID(string citizenID)
        {
            instance.citizenID = citizenID;
        }

    }
}
