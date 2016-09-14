﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HCT_Client
{
    class UtilSOAP
    {
        public static string GetSoapParamStr(int paramNo)
        {
            if (paramNo < 10)
            {
                return "HCT_param" + paramNo;
            }
            else
            {
                return "HCT_paramA" + (paramNo - 10);
            }            
        }

        public static string GetSoapXmlTemplate_FindStudentDetail()
        {
            return GetSoapXmlTemplate("FindStudentDetailSOAP");
        }

        public static string GetSoapXmlTemplate_FindEExamQuestion()
        {
            return GetSoapXmlTemplate("FindEExamQuestionSOAP");
        }

        public static string GetSoapXmlTemplate_CheckEExamResult()
        {
            return GetSoapXmlTemplate("CheckEExamResultSOAP");
        }

        private static string GetSoapXmlTemplate(string soapTemplateName)
        {
            System.Reflection.Assembly myAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            string path = "HCT_Client.Private.SOAP." + soapTemplateName + ".xml";
            Stream myStream = myAssembly.GetManifestResourceStream(path);
            StreamReader reader = new StreamReader(myStream);
            string result = reader.ReadToEnd();

            return result;
        }
    }
}
