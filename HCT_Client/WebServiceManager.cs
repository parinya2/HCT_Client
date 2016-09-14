using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace HCT_Client
{
    class WebServiceManager
    {
        static string DLT_WEB_SERVICE_URI = "http://ws.dlt.go.th:80/EExam/EExamService";
        static string PAPER_TEST_NUMBER_XML_TAG_INSIDE = "paperTestNo";
        static string BUSINESS_ERROR_FAULT = "BusinessErrorFault";

        public static string GetPaperTestNumberFromServer()
        {
            string soapContent = UtilSOAP.GetSoapXmlTemplate_FindStudentDetail();
            soapContent = soapContent.Replace(UtilSOAP.GetSoapParamStr(1),GlobalData.SCHOOL_CERT_YEAR);
            soapContent = soapContent.Replace(UtilSOAP.GetSoapParamStr(2), GlobalData.SCHOOL_CERT_NUMBER);
            soapContent = soapContent.Replace(UtilSOAP.GetSoapParamStr(3), UserProfileManager.GetCitizenID());
            soapContent = soapContent.Replace(UtilSOAP.GetSoapParamStr(4), UserProfileManager.GetCourseRegisterDate());
            soapContent = soapContent.Replace(UtilSOAP.GetSoapParamStr(5), UserProfileManager.GetExamSeq());
            soapContent = soapContent.Replace(UtilSOAP.GetSoapParamStr(6), QuizManager.GetExamCourseCode());

            string responseStr = SendSoapRequestToWebService(soapContent);
            if (WebServiceResultStatus.isErrorCode(responseStr))
            {
                string errorCode = responseStr;
                return errorCode;
            }
            else
            {                
                string paperTestNo = ExtractValueInsideXMLTag(responseStr, PAPER_TEST_NUMBER_XML_TAG_INSIDE);
                if (paperTestNo == null)
                {
                    return WebServiceResultStatus.ERROR_STUDENT_DETAIL_NOT_FOUND;
                }
                else
                {
                    QuizManager.SetPaperTestNumber(paperTestNo);
                    return WebServiceResultStatus.SUCCESS;
                }                
            }
        }

        public static string GetEExamQuestionFromServer()
        {
            DateTime date = DateTime.Now;
            string dayStr = (date.Day < 10) ? ("0" + date.Day) : ("" + date.Day);
            string monthStr = (date.Month < 10) ? ("0" + date.Month) : ("" + date.Month);
            string yearStr = (date.Year < 2500) ? ("" + (date.Year + 543)) : ("" + date.Year);
            string todayStr = dayStr + "/" + monthStr + "/" + yearStr;

            string soapContent = UtilSOAP.GetSoapXmlTemplate_FindEExamQuestion();
            soapContent = soapContent.Replace(UtilSOAP.GetSoapParamStr(1), GlobalData.SCHOOL_CERT_YEAR);
            soapContent = soapContent.Replace(UtilSOAP.GetSoapParamStr(2), GlobalData.SCHOOL_CERT_NUMBER);
            soapContent = soapContent.Replace(UtilSOAP.GetSoapParamStr(3), QuizManager.GetPaperTestNumber());
            soapContent = soapContent.Replace(UtilSOAP.GetSoapParamStr(4), UserProfileManager.GetCitizenID());
            soapContent = soapContent.Replace(UtilSOAP.GetSoapParamStr(5), UserProfileManager.GetExamSeq());
            soapContent = soapContent.Replace(UtilSOAP.GetSoapParamStr(6), todayStr);

            string responseStr = SendSoapRequestToWebService(soapContent);
           
            if (WebServiceResultStatus.isErrorCode(responseStr))
            {
                string errorCode = responseStr;
                return errorCode;
            }
            else
            {
                bool isBusinessError = responseStr.Contains(BUSINESS_ERROR_FAULT);
                if (isBusinessError)
                {
                    return WebServiceResultStatus.ERROR_CANNOT_LOAD_EEXAM;
                }
                else
                {
                    ExtractQuizFromXMLString(responseStr);
                    return WebServiceResultStatus.SUCCESS;
                }
            }
        }

        public static string GetEExamResultFromServer()
        {
            SingleQuizObject[] quizObjectArray = QuizManager.GetQuizArray();
            StringBuilder sbQuizCode = new StringBuilder();
            StringBuilder sbChoiceCode = new StringBuilder();
            string[] quizCodeParamArray = new string[5];
            string[] choiceCodeParamArray = new string[5];
            int objCountPerParam = 10;

            for (int i = 0; i < quizObjectArray.Length; i++)
            {
                SingleQuizObject quizObj = quizObjectArray[i];
                string quizCode = quizObj.quizCode;
                string choiceCode = (quizObj.selectedChoice == -1) ? "" : quizObj.choiceObjArray[quizObj.selectedChoice].choiceCode;
                
                sbQuizCode.Append(quizCode);
                sbChoiceCode.Append(choiceCode);

                if (i % objCountPerParam != objCountPerParam - 1)
                {
                    sbQuizCode.Append("|");
                    sbChoiceCode.Append("|");
                }
                else
                {
                    int idx = i / objCountPerParam;
                    quizCodeParamArray[idx] = sbQuizCode.ToString();
                    choiceCodeParamArray[idx] = sbChoiceCode.ToString();

                    sbQuizCode.Clear();
                    sbChoiceCode.Clear();
                }                
            }

            string soapContent = UtilSOAP.GetSoapXmlTemplate_CheckEExamResult();
            soapContent = soapContent.Replace(UtilSOAP.GetSoapParamStr(1), GlobalData.SCHOOL_CERT_YEAR);
            soapContent = soapContent.Replace(UtilSOAP.GetSoapParamStr(2), GlobalData.SCHOOL_CERT_NUMBER);
            soapContent = soapContent.Replace(UtilSOAP.GetSoapParamStr(3), UserProfileManager.GetCitizenID());            
            soapContent = soapContent.Replace(UtilSOAP.GetSoapParamStr(4), UserProfileManager.GetCourseRegisterDate());
            soapContent = soapContent.Replace(UtilSOAP.GetSoapParamStr(5), UserProfileManager.GetExamSeq());
            soapContent = soapContent.Replace(UtilSOAP.GetSoapParamStr(6), "14/09/2559 22:10:10");
            soapContent = soapContent.Replace(UtilSOAP.GetSoapParamStr(7), "14/09/2559 22:40:10");
            soapContent = soapContent.Replace(UtilSOAP.GetSoapParamStr(8), QuizManager.GetPaperTestNumber());
            soapContent = soapContent.Replace(UtilSOAP.GetSoapParamStr(9), QuizManager.GetExamCourseCode());
            soapContent = soapContent.Replace(UtilSOAP.GetSoapParamStr(10), quizCodeParamArray[0]);
            soapContent = soapContent.Replace(UtilSOAP.GetSoapParamStr(11), quizCodeParamArray[1]);
            soapContent = soapContent.Replace(UtilSOAP.GetSoapParamStr(12), quizCodeParamArray[2]);
            soapContent = soapContent.Replace(UtilSOAP.GetSoapParamStr(13), quizCodeParamArray[3]);
            soapContent = soapContent.Replace(UtilSOAP.GetSoapParamStr(14), quizCodeParamArray[4]);
            soapContent = soapContent.Replace(UtilSOAP.GetSoapParamStr(15), choiceCodeParamArray[0]);
            soapContent = soapContent.Replace(UtilSOAP.GetSoapParamStr(16), choiceCodeParamArray[1]);
            soapContent = soapContent.Replace(UtilSOAP.GetSoapParamStr(17), choiceCodeParamArray[2]);
            soapContent = soapContent.Replace(UtilSOAP.GetSoapParamStr(18), choiceCodeParamArray[3]);
            soapContent = soapContent.Replace(UtilSOAP.GetSoapParamStr(19), choiceCodeParamArray[4]);

            Console.WriteLine("XX = "+soapContent);

            string responseStr = SendSoapRequestToWebService(soapContent);

            if (WebServiceResultStatus.isErrorCode(responseStr))
            {
                string errorCode = responseStr;
                return errorCode;
            }
            else
            {
                bool isBusinessError = responseStr.Contains(BUSINESS_ERROR_FAULT);
                if (isBusinessError)
                {
                    return WebServiceResultStatus.ERROR_CANNOT_CHECK_EEXAM_RESULT;
                }
                else
                {
                    ExtractExamResultFromXMLString(responseStr);
                    return WebServiceResultStatus.SUCCESS;
                }
            }
        }

        private static void ExtractQuizFromXMLString(string content)
        {
            string RESULT_XML_TAG = "<return>";
            string EEXAM_ANSWER_XML_TAG = "<eexamAnswer>";
            string SOAP_BODY_XML_TAG_INSIDE = "soapenv:Body";

            content = ExtractValueInsideXMLTag(content, SOAP_BODY_XML_TAG_INSIDE);
            
            string[] tmpStrArray = content.Split(new string[] {RESULT_XML_TAG}, StringSplitOptions.None);

            // The First element is not used, so we remove it.
            List<string> tmpList = new List<string>(tmpStrArray);
            tmpList.RemoveAt(0);
            tmpStrArray = tmpList.ToArray();

            // Create array of SingleQuizObject
            List<SingleQuizObject> quizList = new List<SingleQuizObject>();
            for (int i = 0; i < tmpStrArray.Length; i++)
            {
                string tmpContent = tmpStrArray[i];
                string quizText = ExtractValueInsideXMLTag(tmpContent, "questDesc");
                string quizCode = ExtractValueInsideXMLTag(tmpContent, "questCode");

                SingleQuizObject quizObj = new SingleQuizObject();
                quizObj.quizText = quizText;
                quizObj.quizCode = quizCode;

                string[] tmpChoiceArray = tmpContent.Split(new string[] { EEXAM_ANSWER_XML_TAG }, StringSplitOptions.RemoveEmptyEntries);
                for (int k = 0; k < tmpChoiceArray.Length; k++)
                {
                    string tmpChoiceContent = tmpChoiceArray[k];
                    SingleChoiceObject choiceObj = new SingleChoiceObject();
                    string choiceText = ExtractValueInsideXMLTag(tmpChoiceContent, "choiceDesc");
                    string choiceCode = ExtractValueInsideXMLTag(tmpChoiceContent, "choiceCode");

                    choiceObj.choiceText = choiceText;
                    choiceObj.choiceCode = choiceCode;

                    quizObj.choiceObjArray[k] = choiceObj;
                }

                quizObj.shuffleChoice();
                quizList.Add(quizObj);
            }

            QuizManager.SetQuizArray(quizList.ToArray());
        }

        private static void ExtractExamResultFromXMLString(string content)
        {
            Console.WriteLine("Exam Result = " + content);
        }

        private static string ExtractValueInsideXMLTag(string content, string xmlTag)
        {
            string startTag = "<" + xmlTag + ">";
            string endTag = "</" + xmlTag + ">";
            string result = null;
            if (content.Contains(startTag) && content.Contains(endTag))
            {
                int startIdx = content.IndexOf(startTag);
                int endIdx = content.IndexOf(endTag);
                int length = endIdx - startIdx;
                string subStr = content.Substring(startIdx, length);
                if (subStr.Length > startTag.Length)
                {
                    result = subStr.Replace(startTag, "");
                }
            }

            return result;            
        }

        public static string SendSoapRequestToWebService(string soapContent)
        {
            HttpWebResponse httpResponse = null;
            Stream responseStream = null;
            StreamReader responseStreamReader = null;

            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(new Uri(DLT_WEB_SERVICE_URI));
            httpRequest.Method = "POST";
            httpRequest.ContentType = "text/xml; charset=utf-8";
            httpRequest.ProtocolVersion = HttpVersion.Version11;
            httpRequest.KeepAlive = true;
            httpRequest.Timeout = 100000; ;

            // It can works without soapAction
            //string soapAction = "http://ws.eexam.dlt.go.th/EExamService/findStudentDetailRequest";
            //httpRequest.Headers.Add(String.Format("SOAPAction: \"{0}\"", soapAction));

            byte[] buffer = Encoding.UTF8.GetBytes(soapContent);
            httpRequest.ContentLength = buffer.Length;

            Stream requestStream = httpRequest.GetRequestStream();
            requestStream.Write(buffer, 0, buffer.Length);
            requestStream.Close();

            try
            {
                using (httpResponse = (HttpWebResponse)httpRequest.GetResponse())
                {
                    responseStream = httpResponse.GetResponseStream();
                    responseStreamReader = new StreamReader(responseStream);

                    string result = responseStreamReader.ReadToEnd();

                    if (result.Contains("Internal Error"))
                    {
                        return WebServiceResultStatus.ERROR_SERVER_INTERNAL;
                    }

                    return result;        
                }
            }
            catch (Exception e)
            {
                string errStr = e.ToString();
                if (errStr.Contains("The operation has timed out"))
                {
                    return WebServiceResultStatus.ERROR_HTTP_TIMEOUT;
                }

                return WebServiceResultStatus.ERROR_99;
            }
            finally
            {
                if(httpResponse != null)            httpResponse.Close();
                if(responseStreamReader != null)    responseStreamReader.Close();
                if(responseStream != null)          responseStream.Close();              
            }
        }
    }

    class WebServiceResultStatus
    {
        public static string SUCCESS = "SUCCESS";
        public static string ERROR_HTTP_TIMEOUT = "ERROR_SendSoapRequestToWebService_HttpTimeout";
        public static string ERROR_STUDENT_DETAIL_NOT_FOUND = "ERROR_FindStudentDetailWebService_StudentNotFound";
        public static string ERROR_SERVER_INTERNAL = "ERROR_ServerInternal";
        public static string ERROR_CANNOT_LOAD_EEXAM = "ERROR_CannotLoadEExam";
        public static string ERROR_CANNOT_CHECK_EEXAM_RESULT = "ERROR_CannotCheckEExamResult";
        public static string ERROR_99 = "ERROR_99_SendSoapRequestToWebService_Throw";

        public static bool isErrorCode(string code)
        {
            return code.Contains("ERROR_");
        }
    }
}
