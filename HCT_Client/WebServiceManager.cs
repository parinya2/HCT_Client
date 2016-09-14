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
        static string PAPER_TEST_NO_XML_TAG = "paperTestNo";


        public static string GetPaperTestNumberFromServer()
        {
            string soapContent = UtilSOAP.GetSoapXmlTemplate_FindStudentDetail();
            soapContent = soapContent.Replace(UtilSOAP.GetSoapParamStr(1),GlobalData.SCHOOL_CERT_YEAR);
            soapContent = soapContent.Replace(UtilSOAP.GetSoapParamStr(2), GlobalData.SCHOOL_CERT_NUMBER);
            soapContent = soapContent.Replace(UtilSOAP.GetSoapParamStr(3), UserProfileManager.GetCitizenID());
            soapContent = soapContent.Replace(UtilSOAP.GetSoapParamStr(4), UserProfileManager.GetCourseRegisterDate());
            soapContent = soapContent.Replace(UtilSOAP.GetSoapParamStr(5), UserProfileManager.GetExamSeq());
            soapContent = soapContent.Replace(UtilSOAP.GetSoapParamStr(6), QuizManager.GetExamCourseCode());

            string x = ExtractValueInsideXMLTag("<aaa></aaa>","aaa");
            return "";


            string responseStr = SendSoapRequestToWebService(soapContent);
            if (WebServiceResultStatus.isErrorCode(responseStr))
            {
                string errorCode = responseStr;
                return errorCode;
            }
            else
            {                
                string paperTestNo = ExtractValueInsideXMLTag(responseStr, PAPER_TEST_NO_XML_TAG);
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

        public static string ExtractValueInsideXMLTag(string content, string xmlTag)
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
            try
            {
                HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(new Uri(DLT_WEB_SERVICE_URI));
                httpRequest.Method = "POST";
                httpRequest.ContentType = "text/xml; charset=utf-8";
                httpRequest.ProtocolVersion = HttpVersion.Version11;
                httpRequest.Timeout = 8000;

                // It can works without soapAction
                //string soapAction = "http://ws.eexam.dlt.go.th/EExamService/findStudentDetailRequest";
                //httpRequest.Headers.Add(String.Format("SOAPAction: \"{0}\"", soapAction));

                byte[] buffer = Encoding.UTF8.GetBytes(soapContent);
                httpRequest.ContentLength = buffer.Length;

                Stream requestStream = httpRequest.GetRequestStream();
                requestStream.Write(buffer, 0, buffer.Length);
                requestStream.Close();

                httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                responseStream = httpResponse.GetResponseStream();
                responseStreamReader = new StreamReader(responseStream);

                string result = responseStreamReader.ReadToEnd();

                return result;
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
        public static string ERROR_99 = "ERROR_99_SendSoapRequestToWebService_Throw";

        public static bool isErrorCode(string code)
        {
            return code.Contains("ERROR_");
        }
    }
}
