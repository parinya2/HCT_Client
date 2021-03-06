﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace HCT_Client
{
    public class UserProfileManager
    {
        private static UserProfileManager instance;
        string citizenID;
        string passportID;
        string fullnameTH;
        string fullnameEN;
        string address;
        Image userPhoto;
        string courseRegisterDate;
        string examSeq; //ครั้งที่สอบ
        string studentEnrolDetailJSON;

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
                ClearUserProfile();
            }
        }

        public static string GetCourseRegisterDateString()
        {
            return instance.courseRegisterDate;
        }

        public static void SetCourseRegisterDate(string date)
        {
            instance.courseRegisterDate = date;
        }

        public static string GetAvailablePersonID()
        {
            if (instance.citizenID != null && instance.citizenID.Length > 0)
                return instance.citizenID;
            else if (instance.passportID != null && instance.passportID.Length > 0)
                return instance.passportID;
            else
                return "";
        }

        public static bool IsCitizenIDAvailable()
        {
            bool result = instance.citizenID != null && instance.citizenID.Length > 0;
            return result;
        }

        public static string GetCitizenID()
        {
            return instance.citizenID;
        }

        public static string GetPassportID()
        {
            return instance.passportID;
        }

        public static void SetPassportID(string text)
        {
            instance.passportID = text;
        }

        public static string GetFullnameTH()
        {
            if (instance.fullnameTH == null) return null;
            return instance.fullnameTH.Trim();
        }

        public static void SetFullnameTH(string text)
        {
            instance.fullnameTH = text; ;
        }

        public static string GetFullnameEN()
        {
            if (instance.fullnameEN == null) return null;
            return instance.fullnameEN.Trim();
        }

        public static string GetAddress()
        {
            if (instance.address == null) return null;
            return instance.address.Trim();
        }

        public static Image GetUserPhoto()
        {
            return instance.userPhoto;
        }

        public static void SetUserPhoto(Image photo)
        {
            instance.userPhoto = photo;
        }

        public static void ClearUserPhoto()
        {
            instance.userPhoto = null;
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

        public static void FillUserProfileWithMockData()
        {
            if (GlobalData.tmpRount == 0) instance.citizenID = "5851801522689";
            else if (GlobalData.tmpRount == 1) instance.citizenID = "5006812126765";
            else instance.citizenID = "5852730731141";

            instance.fullnameTH = "นายสมปอง ทองดี";
            instance.address = "123 ซ.สุขุมวิท 33 กทม. 10120";
            instance.courseRegisterDate = "14/09/2559";
            instance.examSeq = "1";
        }

        public static void FillUserProfileFromSmartCardData(string NIDData)
        {
            string[] fields = NIDData.Split('#');

            instance.citizenID = fields[(int)NID_FIELD.NID_Number];

            if (CardReaderManager.cardReaderMode == CardReaderMode.HALF_BYPASS)
            {
                if (GlobalData.tmpRount == 0) instance.citizenID = "4234411056241";
                else if (GlobalData.tmpRount == 1) instance.citizenID = "5006812126765";
                else instance.citizenID = "5852730731141";
            }

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

        public static void SetExamSeq(string seq)
        {
            instance.examSeq = seq;
        }

        public static string GetExamSeq()
        {
            return instance.examSeq;
        }

        public static string GetStudentEnrolDetailJSON()
        {
            return instance.studentEnrolDetailJSON;
        }

        public static void SetStudentEnrolDetailJSON(string text)
        {
            instance.studentEnrolDetailJSON = text;
        }

        public static void ClearUserProfile()
        {
            instance.citizenID = null;
            instance.passportID = null;
            instance.fullnameTH = null;
            instance.fullnameEN = null;
            instance.address = null;
            instance.userPhoto = null;
            instance.courseRegisterDate = null;
            instance.examSeq = null;
            instance.studentEnrolDetailJSON = null;
        }
    }
}
