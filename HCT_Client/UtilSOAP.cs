using System;
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
                return "HCT_param0" + paramNo;
            }
            else
            {
                return "HCT_param" + paramNo;
            }            
        }

        public static string GetSoapAttachmentParamStr(int attachmentNo)
        {
            if (attachmentNo < 10)
            {
                return "HCT_param_attachment0" + attachmentNo;
            }
            else
            {
                return "HCT_param_attachment" + attachmentNo;
            }
        }

        public static string GetSoapAttachmentContentID(int attachmentNo)
        {
            switch (attachmentNo)
            {
                case 1:  return "a993888b8232c45d2323e39988f12323a288f938da98c@hct.com";
                case 2:  return "da317b7276c347a3292e939f221a233b31d87a9b9c8ab@hct.com";
                default: return "987d8e8237fb2299ac77388d62849bc912a77de88bc8a@hct.com";
            }
        }

        public static string GetSoapXMLContentForAttachment(int attachmentNo)
        {
            string contentID = GetSoapAttachmentContentID(attachmentNo);
            string result = "<xop:Include xmlns:xop=\"http://www.w3.org/2004/08/xop/include\"" +
                            " href=\"cid:" + contentID + "\"/>";
            return result;
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

        public static string GetSoapXmlTemplate_CheckEExamCorrectAnswer()
        {
            return GetSoapXmlTemplate("CheckEExamCorrectAnswerSOAP");
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
