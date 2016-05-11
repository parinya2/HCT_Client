﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HCT_Client
{
    public class UserProfileManager
    {
        private static UserProfileManager instance;
        string citizenID;
        string fullnameTH;
        string fullnameEN;
        string address;

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

        public static string GetFullnameTH()
        {
            return instance.fullnameTH.Trim(); ;
        }

        public static string GetFullnameEN()
        {
            return instance.fullnameEN.Trim(); ;
        }

        public static string GetAddress()
        {
            return instance.address.Trim();
        }

        enum NID_FIELD
        {
            NID_Number,   //1234567890123#

            TITLE_T,    //Thai title#
            NAME_T,     //Thai name#
            MIDNAME_T,  //Thai mid name#
            SURNAME_T,  //Thai surname#

            TITLE_E,    //Eng title#
            NAME_E,     //Eng name#
            MIDNAME_E,  //Eng mid name#
            SURNAME_E,  //Eng surname#

            HOME_NO,    //12/34#
            MOO,        //10#
            TROK,       //ตรอกxxx#
            SOI,        //ซอยxxx#
            ROAD,       //ถนนxxx#
            TUMBON,     //ตำบลxxx#
            AMPHOE,     //อำเภอxxx#
            PROVINCE,   //จังหวัดxxx#

            GENDER,     //1#			//1=male,2=female

            BIRTH_DATE, //25200131#	    //YYYYMMDD 
            ISSUE_PLACE,//xxxxxxx#      //
            ISSUE_DATE, //25580131#     //YYYYMMDD 
            EXPIRY_DATE,//25680130      //YYYYMMDD 

            END
        }; 

        public static void FillUserProfileFromSmartCardData(string NIDData)
        {
            string[] fields = NIDData.Split('#');

            instance.citizenID = fields[(int)NID_FIELD.NID_Number];

            instance.fullnameTH = fields[(int)NID_FIELD.TITLE_T] + " " +
                                  fields[(int)NID_FIELD.NAME_T] + " " +
                                  fields[(int)NID_FIELD.MIDNAME_T] + " " +
                                  fields[(int)NID_FIELD.SURNAME_T];

            instance.fullnameEN = fields[(int)NID_FIELD.TITLE_E] + " " +
                                  fields[(int)NID_FIELD.NAME_E] + " " +
                                  fields[(int)NID_FIELD.MIDNAME_E] + " " +
                                  fields[(int)NID_FIELD.SURNAME_E];

            instance.address = fields[(int)NID_FIELD.HOME_NO] + " " +
                               fields[(int)NID_FIELD.MOO] + " " +
                               fields[(int)NID_FIELD.TROK] + " " +
                               fields[(int)NID_FIELD.SOI] + " " +
                               fields[(int)NID_FIELD.ROAD] + " " +
                               fields[(int)NID_FIELD.TUMBON] + " " +
                               fields[(int)NID_FIELD.AMPHOE] + " " +
                               fields[(int)NID_FIELD.PROVINCE] + " ";
        }

        public static void ClearUserProfile()
        {
            instance.citizenID = null;
            instance.fullnameTH = null;
            instance.fullnameEN = null;
            instance.address = null;
        }
    }
}