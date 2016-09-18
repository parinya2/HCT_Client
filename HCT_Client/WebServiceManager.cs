using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Drawing;
using System.Xml;

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

            byte[] responseBytes = SendSoapRequestToWebService(soapContent);
            if (WebServiceResultStatus.IsErrorBytesCode(responseBytes))
            {
                string errorCode = WebServiceResultStatus.GetErrorStringFromBytesCode(responseBytes);
                return errorCode;
            }
            else
            {
                string responseStr = Encoding.UTF8.GetString(responseBytes);
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

            byte[] responseBytes = SendSoapRequestToWebService(soapContent);
           
            if (WebServiceResultStatus.IsErrorBytesCode(responseBytes))
            {
                string errorCode = WebServiceResultStatus.GetErrorStringFromBytesCode(responseBytes);
                return errorCode;
            }
            else
            {
                string responseStr = Encoding.UTF8.GetString(responseBytes);
                bool isBusinessError = responseStr.Contains(BUSINESS_ERROR_FAULT);
                if (isBusinessError)
                {
                    return WebServiceResultStatus.ERROR_CANNOT_LOAD_EEXAM;
                }
                else
                {
                    ExtractQuizFromXMLBytes(responseBytes);
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

            DateTime st = QuizManager.GetExamStartDateTime();
            DateTime et = QuizManager.GetExamEndDateTime();
            string examStartStr = (st.Day < 10 ? "0" + st.Day : "" + st.Day) + "/" + (st.Month < 10 ? "0" + st.Month : "" + st.Month) + "/" +
                                  (st.Year < 2500 ? "" + (st.Year + 543) : "" + st.Year) + " " +
                                  (st.Hour < 10 ? "0" + st.Hour : "" + st.Hour) + ":" +
                                  (st.Minute < 10 ? "0" + st.Minute : "" + st.Minute) + ":" +
                                  (st.Second < 10 ? "0" + st.Second : "" + st.Second);

            string examEndStr = (et.Day < 10 ? "0" + et.Day : "" + et.Day) + "/" + (et.Month < 10 ? "0" + et.Month : "" + et.Month) + "/" +
                                  (et.Year < 2500 ? "" + (et.Year + 543) : "" + et.Year) + " " +
                                  (et.Hour < 10 ? "0" + et.Hour : "" + et.Hour) + ":" +
                                  (et.Minute < 10 ? "0" + et.Minute : "" + et.Minute) + ":" +
                                  (et.Second < 10 ? "0" + et.Second : "" + et.Second);

            string soapContent = UtilSOAP.GetSoapXmlTemplate_CheckEExamResult();
            soapContent = soapContent.Replace(UtilSOAP.GetSoapParamStr(1), GlobalData.SCHOOL_CERT_YEAR);
            soapContent = soapContent.Replace(UtilSOAP.GetSoapParamStr(2), GlobalData.SCHOOL_CERT_NUMBER);
            soapContent = soapContent.Replace(UtilSOAP.GetSoapParamStr(3), UserProfileManager.GetCitizenID());            
            soapContent = soapContent.Replace(UtilSOAP.GetSoapParamStr(4), UserProfileManager.GetCourseRegisterDate());
            soapContent = soapContent.Replace(UtilSOAP.GetSoapParamStr(5), UserProfileManager.GetExamSeq());
            soapContent = soapContent.Replace(UtilSOAP.GetSoapParamStr(6), examStartStr);
            soapContent = soapContent.Replace(UtilSOAP.GetSoapParamStr(7), examEndStr);
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

            byte[] responseBytes = SendSoapRequestToWebService(soapContent);

            if (WebServiceResultStatus.IsErrorBytesCode(responseBytes))
            {
                string errorCode = WebServiceResultStatus.GetErrorStringFromBytesCode(responseBytes);
                return errorCode;
            }
            else
            {
                string responseStr = Encoding.UTF8.GetString(responseBytes);
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

        private static Bitmap GetBitmapFromBytes(byte[] imageData)
        {
            Bitmap bmp = null;
            try
            {
                using (MemoryStream ms = new MemoryStream(imageData))
                {
                    bmp = new Bitmap(ms);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("SHIT byteArrayToImage " + e.ToString());
            }
            return bmp;
        }

        public static void ExtractQuizFromXMLBytes(byte[] contentBytes)
        {
            byte[] cb = contentBytes;
            List<byte> tmpByteList = new List<byte>();
            List<byte> tmpByteListImageID = new List<byte>();
            List<byte> tmpByteListImageData = new List<byte>();
            Dictionary<string, byte[]> contentDict = new Dictionary<string, byte[]>();
            string SOAP_CONTENT_KEY = "SOAP";
            bool foundContentIDHead = false;
            bool foundImageHeader = false;
            string imageID = null;
            bool isJPEG = false;
            bool isPNG = false;

            for (int i = 0; i < cb.Length - 8; i++)
            {
                if (contentDict.Count == 0)
                {
                    if (cb[i] == '-' && cb[i + 1] == '-' && cb[i + 2] == 'M' && cb[i + 3] == 'I' &&
                        cb[i + 4] == 'M' && cb[i + 5] == 'E' && cb[i + 6] == 'B' && cb[i + 7] == 'o' &&
                        tmpByteList.Count > 0)
                    {
                        contentDict.Add(SOAP_CONTENT_KEY, tmpByteList.ToArray());
                        tmpByteList.Clear();
                    }
                    tmpByteList.Add(cb[i]);
                }
                else
                {
                    if (imageID == null)
                    {
                        if (!foundContentIDHead)
                        {
                            if (cb[i - 4] == 'I' && cb[i - 3] == 'D' && cb[i - 2] == ':' && cb[i - 1] == ' ' && cb[i] == '<')
                            {
                                foundContentIDHead = true;
                            }
                        }
                        else
                        {
                            if (cb[i] == '>')
                            {
                                imageID = Encoding.UTF8.GetString(tmpByteListImageID.ToArray());
                            }
                            else
                            {
                                tmpByteListImageID.Add(cb[i]);
                            }
                        }
                    }
                    else
                    {
                        if (!foundImageHeader)
                        {
                            if (cb[i] == 0xFF && cb[i+1] == 0xD8)
                            {
                                isJPEG = true;
                                tmpByteListImageData.Add(cb[i]);
                                foundImageHeader = true;
                            }
                            else if (cb[i] == 0x89 && cb[i + 1] == 0x50 && cb[i + 2] == 0x4E && cb[i + 3] == 0x47)
                            {
                                isPNG = true;
                                tmpByteListImageData.Add(cb[i]);
                                foundImageHeader = true;
                            }
                        }
                        else
                        {
                            if ((cb[i - 2] == 0xFF && cb[i - 1] == 0xD9 && isJPEG) ||
                                (cb[i - 8] == 0x49 && cb[i - 7] == 0x45 && cb[i - 6] == 0x4E && cb[i - 5] == 0x44 &&
                                 cb[i - 4] == 0xAE && cb[i - 3] == 0x42 && cb[i - 2] == 0x60 && cb[i - 1] == 0x82 &&
                                 isPNG))
                            {
                                contentDict.Add(imageID, tmpByteListImageData.ToArray());
                                tmpByteListImageData.Clear();
                                tmpByteListImageID.Clear();
                                foundImageHeader = false;
                                foundContentIDHead = false;
                                imageID = null;
                            }
                            else
                            {
                                tmpByteListImageData.Add(cb[i]);
                            }                            
                        }
                    }                   
                }
            }

            string RESULT_XML_TAG = "<return>";
            string EEXAM_ANSWER_XML_TAG = "<eexamAnswer>";
            string SOAP_BODY_XML_TAG_INSIDE = "soapenv:Body";
            byte[] xmlContentBytes = contentDict[SOAP_CONTENT_KEY];
            string xmlContent = Encoding.UTF8.GetString(xmlContentBytes);
            xmlContent = ExtractValueInsideXMLTag(xmlContent, SOAP_BODY_XML_TAG_INSIDE);
            string[] tmpStrArray = xmlContent.Split(new string[] { RESULT_XML_TAG }, StringSplitOptions.None);

            // The First element is not used, so we remove it.
            List<string> tmpStrList = new List<string>(tmpStrArray);
            tmpStrList.RemoveAt(0);
            tmpStrArray = tmpStrList.ToArray();

            // Create array of SingleQuizObject
            List<SingleQuizObject> quizList = new List<SingleQuizObject>();
            for (int i = 0; i < tmpStrArray.Length; i++)
            {
                string tmpContent = tmpStrArray[i];
                string quizText = ExtractValueInsideXMLTag(tmpContent, "questDesc");
                string quizCode = ExtractValueInsideXMLTag(tmpContent, "questCode");
                string quizImageIDFullStr = ExtractValueInsideXMLTag(tmpContent, "questImage");
                string quizImageID = ExtractAttachmentIDFromXOPString(quizImageIDFullStr);

                Bitmap quizImage = null;
                if (quizImageID != null && contentDict.ContainsKey(quizImageID))
                {
                    byte[] quizImageBytes = contentDict[quizImageID];
                    quizImage = GetBitmapFromBytes(quizImageBytes);
                }
                
                SingleQuizObject quizObj = new SingleQuizObject();
                quizObj.quizText = quizText;
                quizObj.quizCode = quizCode;
                quizObj.quizImage = quizImage;

                string[] tmpChoiceArray = tmpContent.Split(new string[] { EEXAM_ANSWER_XML_TAG }, StringSplitOptions.RemoveEmptyEntries);
                for (int k = 0; k < tmpChoiceArray.Length; k++)
                {
                    string tmpChoiceContent = tmpChoiceArray[k];
                    SingleChoiceObject choiceObj = new SingleChoiceObject();
                    string choiceText = ExtractValueInsideXMLTag(tmpChoiceContent, "choiceDesc");
                    string choiceCode = ExtractValueInsideXMLTag(tmpChoiceContent, "choiceCode");
                    string choiceImageIDFullStr = ExtractValueInsideXMLTag(tmpContent, "choiceImage");
                    string choiceImageID = ExtractAttachmentIDFromXOPString(choiceImageIDFullStr);

                    Bitmap choiceImage = null;
                    if (choiceImageID != null && contentDict.ContainsKey(choiceImageID))
                    {
                        byte[] choiceImageBytes = contentDict[choiceImageID];
                        choiceImage = GetBitmapFromBytes(choiceImageBytes);
                    }

                    choiceObj.choiceText = choiceText;
                    choiceObj.choiceCode = choiceCode;
                    choiceObj.choiceImage = choiceImage;

                    quizObj.choiceObjArray[k] = choiceObj;
                }

                quizObj.shuffleChoice();
                quizList.Add(quizObj);
            }

            QuizManager.SetQuizArray(quizList.ToArray());     
        }

        private static string ExtractAttachmentIDFromXOPString(string str)
        {
            if (str == null || str.Length == 0)
                return null;

            int idx1 = str.IndexOf("cid:");
            if (idx1 != -1)
            {
                str = str.Substring(idx1 + 4);
                int idx2 = str.IndexOf("\"");
                if (idx2 != -1)
                {
                    str = str.Substring(0, idx2);
                    return str;
                }                
            }
            return null;
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

        public static byte[] SendSoapRequestToWebService(string soapRequestMessage)
        {
            HttpWebResponse httpResponse = null;
            Stream responseStream = null;

            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(new Uri(DLT_WEB_SERVICE_URI));
            httpRequest.Method = "POST";
            httpRequest.ContentType = "text/xml; charset=utf-8";
            httpRequest.ProtocolVersion = HttpVersion.Version11;
            httpRequest.KeepAlive = true;
            httpRequest.Timeout = 100000; ;

            // It can works without soapAction
            //string soapAction = "http://ws.eexam.dlt.go.th/EExamService/findStudentDetailRequest";
            //httpRequest.Headers.Add(String.Format("SOAPAction: \"{0}\"", soapAction));

            byte[] buffer = Encoding.UTF8.GetBytes(soapRequestMessage);
            httpRequest.ContentLength = buffer.Length;

            Stream requestStream = httpRequest.GetRequestStream();
            requestStream.Write(buffer, 0, buffer.Length);
            requestStream.Close();

            try
            {
                using (httpResponse = (HttpWebResponse)httpRequest.GetResponse())
                {                    
                    responseStream = httpResponse.GetResponseStream();                          

                    MemoryStream ms = new MemoryStream();
                    responseStream.CopyTo(ms);
                    byte[] resultBytes = ms.ToArray();

                    string result = Encoding.UTF8.GetString(resultBytes);
                    if (result.Contains("Internal Error"))
                    {
                        return new byte[] { WebServiceResultStatus.ERROR_BYTE_SERVER_INTERNAL };
                    }

                    return resultBytes;                                               
                }
            }
            catch (Exception e)
            {
                string errStr = e.ToString();
                if (errStr.Contains("The operation has timed out"))
                {
                    return new byte[] { WebServiceResultStatus.ERROR_BYTE_HTTP_TIMEOUT };
                }

                return new byte[] { WebServiceResultStatus.ERROR_BYTE_99 };
            }
            finally
            {
                if(httpResponse != null)            httpResponse.Close();
                if(responseStream != null)          responseStream.Close();              
            }
        }
    }

    class WebServiceResultStatus
    {
        public const string SUCCESS = "SUCCESS";
        public const byte ERROR_BYTE_HTTP_TIMEOUT = 0xFF;
        public const byte ERROR_BYTE_SERVER_INTERNAL = 0xFD;
        public const byte ERROR_BYTE_99 = 0xFE;

        public const string ERROR_HTTP_TIMEOUT = "ERROR_HttpTimeout";
        public const string ERROR_SERVER_INTERNAL = "ERROR_ServerInternal";
        public const string ERROR_99 = "ERROR_99";

        public const string ERROR_STUDENT_DETAIL_NOT_FOUND = "ERROR_FindStudentDetailWebService_StudentNotFound";
        public const string ERROR_CANNOT_LOAD_EEXAM = "ERROR_CannotLoadEExam";
        public const string ERROR_CANNOT_CHECK_EEXAM_RESULT = "ERROR_CannotCheckEExamResult";
        

        public static bool IsErrorCode(string code)
        {
            return code.Contains("ERROR_");
        }

        public static bool IsErrorBytesCode(byte[] bytesCode)
        {
            if (bytesCode.Length == 1)
            {
                if (bytesCode[0] == ERROR_BYTE_99 ||
                    bytesCode[0] == ERROR_BYTE_HTTP_TIMEOUT ||
                    bytesCode[0] == ERROR_BYTE_SERVER_INTERNAL)
                {
                    return true;
                }
            }
            return false;
        }

        public static string GetErrorStringFromBytesCode(byte[] bytesCode)
        {
            if (bytesCode.Length == 1)
            {
                switch (bytesCode[0])
                {
                    case ERROR_BYTE_99:              return ERROR_99;
                    case ERROR_BYTE_HTTP_TIMEOUT:    return ERROR_HTTP_TIMEOUT;
                    case ERROR_BYTE_SERVER_INTERNAL: return ERROR_SERVER_INTERNAL;
                    default: return ERROR_99;
                }
            }
            else
            {
                return ERROR_99;
            }
        }
    }
}
