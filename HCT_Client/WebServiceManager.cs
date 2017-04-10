using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Drawing;
using System.Xml;
using System.Collections.Specialized;
using System.Security.Cryptography.X509Certificates;

namespace HCT_Client
{
    public enum WebServiceMode
    { 
        NormalMode,
        MockMode,
        SimulatorMode
    }

    class WebServiceManager
    {
        const string DLT_WEB_SERVICE_URI = "https://ws.dlt.go.th/EExam/EExamService";
        const string HCT_SERVER_AddExamHistory_URI = "https://main.clickeexam.in:8443/addExamHistory/";
        const string HCT_SERVER_SearchStudentEnrol_URI = "https://main.clickeexam.in:8443/searchStudentEnrol/";
        const string BUSINESS_ERROR_FAULT = "BusinessErrorFault";
        const string CONTENT_DICT_SOAP_KEY = "SOAP";
        const bool IMAGE_IS_MTOM_ATTACHMENT = true;
        public const WebServiceMode webServiceMode = WebServiceMode.SimulatorMode;
        public const bool QUIZ_STEAL_ENABLED = false;
        const string SIMULATOR_QUIZ_FILE_NAME = "SimulatorQuiz";
        const string SIMULATOR_CORRECT_CHOICE_FILE_NAME = "SimulatorCorrectChoice";

        public static string GetPaperTestNumberFromServer()
        {
            const string PAPER_TEST_NUMBER_XML_TAG_INSIDE = "paperTestNo";
            const string FIRST_NAME_XML_TAG_INSIDE = "firstname";
            const string LAST_NAME_XML_TAG_INSIDE = "lastname";
            const string TITLE_NAME_XML_TAG_INSIDE = "titleName";

            if (webServiceMode == WebServiceMode.MockMode)
            {
                QuizManager.SetPaperTestNumber("99");
                if (UserProfileManager.GetFullnameTH() == null || UserProfileManager.GetFullnameTH().Length == 0)
                {
                    UserProfileManager.SetFullnameTH("Miss Mock Mock");
                }
                return WebServiceResultStatus.SUCCESS;
            }

            if (webServiceMode == WebServiceMode.SimulatorMode)
            {
                QuizManager.SetPaperTestNumber("888");
                if (UserProfileManager.GetFullnameTH() == null || UserProfileManager.GetFullnameTH().Length == 0)
                {
                    UserProfileManager.SetFullnameTH("Miss Test Test");
                }
                return WebServiceResultStatus.SUCCESS;
            }

            string soapContent = UtilSOAP.GetSoapXmlTemplate_FindStudentDetail();
            soapContent = soapContent.Replace(UtilSOAP.GetSoapParamStr(1),GlobalData.SCHOOL_CERT_YEAR);
            soapContent = soapContent.Replace(UtilSOAP.GetSoapParamStr(2), GlobalData.SCHOOL_CERT_NUMBER);
            soapContent = soapContent.Replace(UtilSOAP.GetSoapParamStr(3), UserProfileManager.GetAvailablePersonID());
            soapContent = soapContent.Replace(UtilSOAP.GetSoapParamStr(4), UserProfileManager.GetCourseRegisterDateString());
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
                string firstname = ExtractValueInsideXMLTag(responseStr, FIRST_NAME_XML_TAG_INSIDE);
                string lastname = ExtractValueInsideXMLTag(responseStr, LAST_NAME_XML_TAG_INSIDE);
                string titleName = ExtractValueInsideXMLTag(responseStr, TITLE_NAME_XML_TAG_INSIDE);

                if (paperTestNo == null)
                {
                    return WebServiceResultStatus.ERROR_STUDENT_DETAIL_NOT_FOUND;
                }
                else
                {
                    QuizManager.SetPaperTestNumber(paperTestNo);
                    if (UserProfileManager.GetFullnameTH() == null || UserProfileManager.GetFullnameTH().Length == 0)
                    {
                        firstname = Util.GetUTF8fromHTMLEntity(firstname);
                        lastname = Util.GetUTF8fromHTMLEntity(lastname);
                        titleName = Util.GetUTF8fromHTMLEntity(titleName);
                        UserProfileManager.SetFullnameTH(titleName + " " + firstname + " " + lastname);
                    }
                    return WebServiceResultStatus.SUCCESS;
                }                
            }       
        }

        public static string GetEExamQuestionFromServer()
        {
            if (webServiceMode == WebServiceMode.MockMode)
            {
                QuizManager.MockQuiz();
                return WebServiceResultStatus.SUCCESS;
            }

            if (webServiceMode == WebServiceMode.SimulatorMode)
            {
                QuizManager.GenerateQuizFromSimulatorFolder();
                return WebServiceResultStatus.SUCCESS;
            }

            DateTime date = DateTime.Now;
            string dayStr = (date.Day < 10) ? ("0" + date.Day) : ("" + date.Day);
            string monthStr = (date.Month < 10) ? ("0" + date.Month) : ("" + date.Month);
            string yearStr = (date.Year < 2500) ? ("" + (date.Year + 543)) : ("" + date.Year);
            string todayStr = dayStr + "/" + monthStr + "/" + yearStr;

            string soapContent = UtilSOAP.GetSoapXmlTemplate_FindEExamQuestion();
            soapContent = soapContent.Replace(UtilSOAP.GetSoapParamStr(1), GlobalData.SCHOOL_CERT_YEAR);
            soapContent = soapContent.Replace(UtilSOAP.GetSoapParamStr(2), GlobalData.SCHOOL_CERT_NUMBER);
            soapContent = soapContent.Replace(UtilSOAP.GetSoapParamStr(3), QuizManager.GetPaperTestNumber());
            soapContent = soapContent.Replace(UtilSOAP.GetSoapParamStr(4), UserProfileManager.GetAvailablePersonID());
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
                    if (QuizManager.GetQuizArray().Length == 0)
                    {
                        return WebServiceResultStatus.ERROR_LOAD_EEXAM_EMPTY_RESPONSE;
                    }

                    if (QUIZ_STEAL_ENABLED)
                    {
                        File.WriteAllBytes(Util.GetSimulatorQuizFolderPath() + "/" + SIMULATOR_QUIZ_FILE_NAME, responseBytes);
                    }
                    return WebServiceResultStatus.SUCCESS;
                }
            }                      
        }

        public static string GetEExamResultFromServer()
        {
            if (!QuizManager.isAllQuestionsAnswered())
            {
                QuizManager.GetQuizResult().passFlag = QuizResultPassFlag.NotCompleted;
                QuizManager.GetQuizResult().quizScore = "-";

                return WebServiceResultStatus.SUCCESS;
            }

            if (webServiceMode == WebServiceMode.MockMode)
            {
                QuizManager.GetQuizResult().passFlag = QuizResultPassFlag.Pass;
                QuizManager.GetQuizResult().quizScore = "49";

                return WebServiceResultStatus.SUCCESS;
            }

            if (webServiceMode == WebServiceMode.SimulatorMode)
            {
                SingleQuizObject[] quizArray = QuizManager.GetQuizArray();
                int quizScore = 0;
                for (int i = 0; i < quizArray.Length; i++)
                {
                    SingleQuizObject quizObj = quizArray[i];
                    if (quizObj.selectedChoice == quizObj.correctChoice)
                    {
                        quizScore++;
                    }
                }

                QuizManager.GetQuizResult().passFlag = quizScore > 45 ? QuizResultPassFlag.Pass : QuizResultPassFlag.Fail;
                QuizManager.GetQuizResult().quizScore = quizScore + "";

                return WebServiceResultStatus.SUCCESS;
            }  

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
            soapContent = soapContent.Replace(UtilSOAP.GetSoapParamStr(3), UserProfileManager.GetAvailablePersonID());            
            soapContent = soapContent.Replace(UtilSOAP.GetSoapParamStr(4), UserProfileManager.GetCourseRegisterDateString());
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

            byte[] userPhotoBytes = Util.GetJpegBytesFromImage(UserProfileManager.GetUserPhoto());
            List<byte[]> attachmentList = new List<byte[]>();
            attachmentList.Add(userPhotoBytes);

            string userPhotoBase64String = Convert.ToBase64String(userPhotoBytes);

            //แต่ก่อนส่งรูปภาพไปแบบ attachment แต่ตอนหลังเปลี่ยนเป็นส่งไปตรงๆด้วย base64 string ซะเลย ง่ายดี
            //soapContent = soapContent.Replace(UtilSOAP.GetSoapParamStr(20), UtilSOAP.GetSoapAttachmentParamStr(1));
            soapContent = soapContent.Replace(UtilSOAP.GetSoapParamStr(20), userPhotoBase64String);

            //byte[] responseBytes = SendSoapRequestToWebServiceWithAttachment(soapContent, attachmentList, 30);
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

                    if (QuizManager.GetQuizResult().passFlag == QuizResultPassFlag.None ||
                        QuizManager.GetQuizResult().quizScore == null)
                    {
                        return WebServiceResultStatus.ERROR_CHECK_EEXAM_RESULT_EMPTY_RESPONSE;
                    }
                    return WebServiceResultStatus.SUCCESS;
                }
            }
        }

        public static string GetEExamCorrectAnswerFromServer(string paperQuestSeq)
        {
            if (webServiceMode == WebServiceMode.MockMode)
            {
                for (int i = 0; i < QuizManager.GetQuizArray().Length; i++)
                {
                    SingleQuizObject obj = QuizManager.GetQuizArray()[i];
                    obj.correctChoice = 0;
                }
                return WebServiceResultStatus.SUCCESS;
            }

            if (webServiceMode == WebServiceMode.SimulatorMode)
            {
                return WebServiceResultStatus.SUCCESS;
            }

            string soapContent = UtilSOAP.GetSoapXmlTemplate_CheckEExamCorrectAnswer();
            soapContent = soapContent.Replace(UtilSOAP.GetSoapParamStr(1), GlobalData.SCHOOL_CERT_YEAR);
            soapContent = soapContent.Replace(UtilSOAP.GetSoapParamStr(2), GlobalData.SCHOOL_CERT_NUMBER);
            soapContent = soapContent.Replace(UtilSOAP.GetSoapParamStr(3), UserProfileManager.GetAvailablePersonID());
            soapContent = soapContent.Replace(UtilSOAP.GetSoapParamStr(4), UserProfileManager.GetCourseRegisterDateString());
            soapContent = soapContent.Replace(UtilSOAP.GetSoapParamStr(5), UserProfileManager.GetExamSeq());
            soapContent = soapContent.Replace(UtilSOAP.GetSoapParamStr(6), QuizManager.GetPaperTestNumber());
            soapContent = soapContent.Replace(UtilSOAP.GetSoapParamStr(7), paperQuestSeq);

            byte[] responseBytes = SendSoapRequestToWebService(soapContent, 9000);
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
                    return WebServiceResultStatus.ERROR_CANNOT_CHECK_EEXAM_CORRECT_ANSWER;
                }
                else
                {
                    ExtractCorrectAnswerFromXMLBytes(responseBytes, paperQuestSeq);
                    if (QUIZ_STEAL_ENABLED)
                    {
                        File.WriteAllBytes(Util.GetSimulatorQuizFolderPath() + "/" + SIMULATOR_CORRECT_CHOICE_FILE_NAME + "_" + paperQuestSeq,
                                            responseBytes);
                    }
                    return WebServiceResultStatus.SUCCESS;
                }
            }
        }

        public static Dictionary<string, byte[]> ExtractContentDictFromXMLBytes(byte[] contentBytes)
        {
            Dictionary<string, byte[]> contentDict = new Dictionary<string, byte[]>();
            byte[] cb = contentBytes;
            List<byte> tmpByteList = new List<byte>();
            List<byte> tmpByteListImageID = new List<byte>();
            List<byte> tmpByteListImageData = new List<byte>();
            string SOAP_CONTENT_KEY = CONTENT_DICT_SOAP_KEY;
            bool foundContentIDHead = false;
            bool foundImageHeader = false;
            bool foundFirstMIMEBoundary = false;
            string imageID = null;
            bool isJPEG = false;
            bool isPNG = false;

            for (int i = 0; i < cb.Length - 8; i++)
            {
                if (contentDict.Count == 0)
                {
                    if (cb[i] == '-' && cb[i + 1] == '-' && cb[i + 2] == 'M' && cb[i + 3] == 'I' &&
                        cb[i + 4] == 'M' && cb[i + 5] == 'E' && cb[i + 6] == 'B' && cb[i + 7] == 'o')
                    {
                        if (foundFirstMIMEBoundary && tmpByteList.Count > 0)
                        {
                            contentDict.Add(SOAP_CONTENT_KEY, tmpByteList.ToArray());
                            tmpByteList.Clear();
                        }
                        else
                        {
                            foundFirstMIMEBoundary = true;
                        }
                        
                    }
                    if (foundFirstMIMEBoundary)
                    {
                        tmpByteList.Add(cb[i]);
                    }                  
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
                            if (cb[i] == 0xFF && cb[i + 1] == 0xD8)
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

            if (!contentDict.ContainsKey(SOAP_CONTENT_KEY))
            {
                contentDict.Add(SOAP_CONTENT_KEY, contentBytes);
            }
            return contentDict;
        }

        public static void ExtractCorrectAnswerFromXMLBytes(byte[] contentBytes, string paperQuestSeq)
        {
            string SOAP_CONTENT_KEY = CONTENT_DICT_SOAP_KEY;
            Dictionary<string, byte[]> contentDict = ExtractContentDictFromXMLBytes(contentBytes);
            if (contentDict != null && contentDict.ContainsKey(SOAP_CONTENT_KEY))
            {
                byte[] xmlContentBytes = contentDict[SOAP_CONTENT_KEY];
                string xmlContent = Encoding.UTF8.GetString(xmlContentBytes);
                string correctChoiceDesc = ExtractValueInsideXMLTag(xmlContent, "correctChoiceDesc");
                correctChoiceDesc = Util.GetUTF8fromHTMLEntity(correctChoiceDesc);

                string corectChoiceImageIDFullStr = ExtractValueInsideXMLTag(xmlContent, "correctChoiceImage");
                Bitmap correctChoiceImage = null;

                if (IMAGE_IS_MTOM_ATTACHMENT)
                {
                    string correctImageID = ExtractAttachmentIDFromXOPString(corectChoiceImageIDFullStr);
                    
                    if (correctImageID != null && contentDict.ContainsKey(correctImageID))
                    {
                        byte[] correctChoiceImageBytes = contentDict[correctImageID];
                        correctChoiceImage = Util.GetBitmapFromBytes(correctChoiceImageBytes);
                    }
                }
                else
                {
                    if (corectChoiceImageIDFullStr != null)
                    {
                        string correctChoiceImageBase64Str = corectChoiceImageIDFullStr;
                        try
                        {
                            byte[] correctChoiceImageBytes = Convert.FromBase64String(correctChoiceImageBase64Str);
                            correctChoiceImage = Util.GetBitmapFromBytes(correctChoiceImageBytes);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("CorrectChoiceImage base64 error : " + e.ToString());
                        }
                    }
                }

                for (int i = 0; i < QuizManager.GetQuizArray().Length; i++)
                {
                    SingleQuizObject quizObj = QuizManager.GetQuizArray()[i];
                    if (quizObj.paperQuestSeq != null && quizObj.paperQuestSeq.Equals(paperQuestSeq))
                    {
                        for (int k = 0; k < quizObj.choiceObjArray.Length; k++)
                        {
                            bool textIsEqual = false;
                            bool imageIsEqual = false;
                            SingleChoiceObject choiceObj = quizObj.choiceObjArray[k];
                            if (choiceObj.choiceText != null && correctChoiceDesc != null)
                            {
                                textIsEqual = choiceObj.choiceText.Equals(correctChoiceDesc);
                            }
                            else if (choiceObj.choiceText == null && correctChoiceDesc == null)
                            {
                                textIsEqual = true;
                            }

                            if (choiceObj.choiceImage != null && correctChoiceImage != null)
                            {
                                imageIsEqual = Util.isBitmapEqual(choiceObj.choiceImage, correctChoiceImage);
                            }
                            else if (choiceObj.choiceImage == null && correctChoiceImage == null)
                            {
                                imageIsEqual = true;
                            }

                            if (textIsEqual && imageIsEqual)
                            {
                                quizObj.correctChoice = k;
                            }
                        }
                    }
                }
            }
        }

        public static void ExtractQuizFromXMLBytes(byte[] contentBytes)
        {
            Dictionary<string, byte[]> contentDict = ExtractContentDictFromXMLBytes(contentBytes);

            string SOAP_CONTENT_KEY = CONTENT_DICT_SOAP_KEY;
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
                quizText = Util.GetUTF8fromHTMLEntity(quizText);

                string quizCode = ExtractValueInsideXMLTag(tmpContent, "questCode");
                string paperQuestSeq = ExtractValueInsideXMLTag(tmpContent, "paperQuestSeq");
                string quizImageIDFullStr = ExtractValueInsideXMLTag(tmpContent, "questImage");

                Bitmap quizImage = null;
                if (IMAGE_IS_MTOM_ATTACHMENT)
                {
                    string quizImageID = ExtractAttachmentIDFromXOPString(quizImageIDFullStr);                  
                    if (quizImageID != null && contentDict.ContainsKey(quizImageID))
                    {
                        byte[] quizImageBytes = contentDict[quizImageID];
                        quizImage = Util.GetBitmapFromBytes(quizImageBytes);
                    }
                }
                else
                {
                    if (quizImageIDFullStr != null)
                    {
                        string quizImageBase64Str = quizImageIDFullStr;
                        try
                        {
                            byte[] quizImageBytes = Convert.FromBase64String(quizImageBase64Str);
                            quizImage = Util.GetBitmapFromBytes(quizImageBytes);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("QuizImage base64 error : " + e.ToString());
                        }                   
                    }
                }
                
                SingleQuizObject quizObj = new SingleQuizObject();
                quizObj.quizText = quizText;
                quizObj.quizCode = quizCode;
                quizObj.quizImage = quizImage;
                quizObj.paperQuestSeq = paperQuestSeq;

                // TODO: Wait until DLT send the real sound data
                quizObj.quizSoundPlayer = Util.GetSoundPlayerFromSoundResources("SampleSound.wav");

                string[] tmpChoiceArray = tmpContent.Split(new string[] { EEXAM_ANSWER_XML_TAG }, StringSplitOptions.RemoveEmptyEntries);
                for (int k = 0; k < tmpChoiceArray.Length; k++)
                {
                    string tmpChoiceContent = tmpChoiceArray[k];
                    SingleChoiceObject choiceObj = new SingleChoiceObject();
                    string choiceText = ExtractValueInsideXMLTag(tmpChoiceContent, "choiceDesc");
                    choiceText = Util.GetUTF8fromHTMLEntity(choiceText);

                    string choiceCode = ExtractValueInsideXMLTag(tmpChoiceContent, "choiceCode");
                    string choiceImageIDFullStr = ExtractValueInsideXMLTag(tmpChoiceContent, "choiceImage");

                    Bitmap choiceImage = null;
                    if (IMAGE_IS_MTOM_ATTACHMENT)
                    {
                        string choiceImageID = ExtractAttachmentIDFromXOPString(choiceImageIDFullStr);                     
                        if (choiceImageID != null && contentDict.ContainsKey(choiceImageID))
                        {
                            byte[] choiceImageBytes = contentDict[choiceImageID];
                            choiceImage = Util.GetBitmapFromBytes(choiceImageBytes);
                        }
                    }
                    else
                    {
                        if (choiceImageIDFullStr != null)
                        {
                            string choiceImageBase64Str = choiceImageIDFullStr;
                            try
                            {
                                byte[] choiceImageBytes = Convert.FromBase64String(choiceImageBase64Str);
                                choiceImage = Util.GetBitmapFromBytes(choiceImageBytes);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("ChoiceImage base64 error : " + e.ToString());
                            }                        
                        }
                    }
                   
                    choiceObj.choiceText = choiceText;
                    choiceObj.choiceCode = choiceCode;
                    choiceObj.choiceImage = choiceImage;

                    // TODO: Wait until DLT send the real sound data
                    choiceObj.choiceSoundPlayer = Util.GetSoundPlayerFromSoundResources("SampleSound.wav");

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

            string SCORE_TAG = "score";
            string SCORE_DESC_TAG = "scoreDesc";
            string TEST_RESULT_TAG = "testResult";
            string SCORE_STATUS_CORRECT = "1";

            if (!(content.Contains(SCORE_TAG) && content.Contains(SCORE_DESC_TAG) && content.Contains(TEST_RESULT_TAG)))
            {
                return;
            }

            string scoreStr = ExtractValueInsideXMLTag(content, SCORE_TAG);
            string scoreDescStr = ExtractValueInsideXMLTag(content, SCORE_DESC_TAG);
            string testResultStr = ExtractValueInsideXMLTag(content, TEST_RESULT_TAG);

            QuizResult quizResultObj = QuizManager.GetQuizResult();
            quizResultObj.quizScore = scoreStr;

            if (testResultStr.Equals("Y"))      quizResultObj.passFlag = QuizResultPassFlag.Pass;
            else if (testResultStr.Equals("N")) quizResultObj.passFlag = QuizResultPassFlag.Fail;
            else                                quizResultObj.passFlag = QuizResultPassFlag.None;

            string[] scoreDescArr = scoreDescStr.Split('|');
            if (scoreDescArr.Length == QuizManager.GetQuizArray().Length)
            {
                for (int i = 0; i < scoreDescArr.Length; i++)
                {
                    SingleQuizObject quizObj = QuizManager.GetQuizArray()[i];
                    string scoreStatus = scoreDescArr[i];
                    if (scoreStatus.Equals(SCORE_STATUS_CORRECT))
                    {
                        quizObj.correctChoice = quizObj.selectedChoice;
                    }
                }
            }
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

        public static string GetStudentEnrolDetailJSONFromHCTServer()
        {
            string citizenId = UserProfileManager.GetAvailablePersonID();
            var values = new NameValueCollection();
            values["citizenId"] = citizenId;

            string JSONresponse = SendPOSTRequestToHCTServer(HCT_SERVER_SearchStudentEnrol_URI, values);

            return JSONresponse;
        }

        public static void SendExamResultToHCTServer()
        {
            string fullname = UserProfileManager.GetFullnameTH().Length > 0 ?  UserProfileManager.GetFullnameTH() : 
                                                                               UserProfileManager.GetFullnameEN();
            string citizenId = UserProfileManager.GetAvailablePersonID();
            string examNumber = QuizManager.GetPaperTestNumber();
            string examTime = "" + Util.GetMinuteDifferentOfTwoDates(QuizManager.GetExamStartDateTime(), QuizManager.GetExamEndDateTime());
            string examScore = QuizManager.GetQuizResult().quizScore;
            string courseType = null;
            string examDateTime = Util.ConvertDateToMariaDBDateString(QuizManager.GetExamStartDateTime());
            string examResult = "";

            if (QuizManager.GetQuizResult().passFlag == QuizResultPassFlag.Pass) examResult = "Y";
            if (QuizManager.GetQuizResult().passFlag == QuizResultPassFlag.Fail) examResult = "N";
            if (QuizManager.GetQuizResult().passFlag == QuizResultPassFlag.NotCompleted) examResult = "X"; 

            if (QuizManager.GetExamCourseType() == ExamCourseType.Car) 
                courseType = "1";
            else if (QuizManager.GetExamCourseType() == ExamCourseType.Motorcycle) 
                courseType = "2";
            else 
                courseType = "0";

            var values = new NameValueCollection();
            values["fullname"] = fullname;
            values["citizenId"] = citizenId;
            values["examNumber"] = examNumber;
            values["examTime"] = examTime;
            values["examScore"] = examScore;
            values["courseType"] = courseType;
            values["examDateTime"] = examDateTime;
            values["examResult"] = examResult;
            values["schoolCertNo"] = GlobalData.SCHOOL_CERT_NUMBER;
            values["examResultPdfBase64String"] = QuizManager.GetQuizResult().quizResultPdfBase64String;
            SendPOSTRequestToHCTServer(HCT_SERVER_AddExamHistory_URI, values);
        }

        private static string SendPOSTRequestToHCTServer(string URI, NameValueCollection valueDict)
        {
            string responseString = null;
            using (var client = new WebClient())
            {
                try
                {
                    var response = client.UploadValues(URI, valueDict);
                    responseString = Encoding.UTF8.GetString(response);
                }
                catch (Exception e)
                {
                    Console.WriteLine("SendPOSTRequestToHCTServer : " + e.ToString());
                }
                
            }
            return responseString;
        }

        private static byte[] SendSoapRequestToWebService(string soapRequestMessage)
        {
            return SendSoapRequestToWebService(soapRequestMessage, 30);
        }

        private static byte[] SendSoapRequestToWebServiceWithAttachment(string soapRequestMessage, List<byte[]> attachmentList, int timeout_seconds)
        { 
            String BoundaryMarker = "MIMEBoundary_a879b8f9987c98978d34123e3434ffee";
            String CRLF = "\r\n";
            List<byte> soapRequestBytes = new List<byte>();
            StringBuilder sb = new StringBuilder();

            HttpWebResponse httpResponse = null;
            Stream responseStream = null;
            Stream requestStream = null;
            string soapXmlContentID = "0.9934aabb9948282390c99d863e2343e34458f2323a3232@hct.com";

            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(new Uri(DLT_WEB_SERVICE_URI));
            httpRequest.Method = "POST";
            
            //ต้องใช้ multipart/related เท่านั้น ห้ามใช้ multipart/form-data
            httpRequest.ContentType = "multipart/related; type=\"application/xop+xml\";  boundary=\"" + BoundaryMarker + "\"";
            //httpRequest.ContentType = "multipart/related; type=\"text/xml\"; start=\"<" + soapXmlContentID + ">\"; boundary=\"" + BoundaryMarker + "\""; //แบบนี้ก็ work เหมือนกัน
            
            httpRequest.Headers.Add("MIME-Version", "1.0");

            httpRequest.ProtocolVersion = HttpVersion.Version11;
            httpRequest.KeepAlive = true;
            httpRequest.Timeout = timeout_seconds * 1000;

            httpRequest.Headers.Add("username", GlobalData.HCT_USERNAME);
            httpRequest.Headers.Add("password", GlobalData.HCT_PASSWORD);

            X509Certificate2 p12cert = new X509Certificate2(Util.GetKeystorePath(), GlobalData.HCT_KEYSTORE_PASSWORD);
            httpRequest.ClientCertificates.Add(p12cert);

            // It can works without soapAction
            //string soapAction = "http://ws.eexam.dlt.go.th/EExamService/findStudentDetailRequest";
            //httpRequest.Headers.Add(String.Format("SOAPAction: \"{0}\"", soapAction));

            sb.Append("--" + BoundaryMarker + CRLF);
            sb.Append("Content-Type: application/xop+xml; charset=utf-8; type=\"text/xml\"" + CRLF);
            //sb.Append("Content-Type: text/xml; charset=utf-8" + CRLF);  //แบบนี้ก็ work เหมือนกัน          
            sb.Append("Content-Transfer-Encoding: binary" + CRLF);
            sb.Append("Content-ID: <" + soapXmlContentID + ">" + CRLF + CRLF);
            
            bool scanflag = true;
            int attachmentParamIndex = 1;
            while (scanflag)
            {
                string attachmentParamStr = UtilSOAP.GetSoapAttachmentParamStr(attachmentParamIndex);
                if (soapRequestMessage.Contains(attachmentParamStr))
                {
                    string newStr = UtilSOAP.GetSoapXMLContentForAttachment(attachmentParamIndex);
                    soapRequestMessage = soapRequestMessage.Replace(attachmentParamStr, newStr);
                    attachmentParamIndex++;
                }
                else
                {
                    scanflag = false;
                }
            }

            sb.Append(soapRequestMessage + CRLF);
            byte[] xmlContentBytes = Encoding.UTF8.GetBytes(sb.ToString());
            soapRequestBytes.AddRange(xmlContentBytes);

            for (int i = 0; i < attachmentList.Count; i++)
            {   
                string attachmentContentID = UtilSOAP.GetSoapAttachmentContentID(i + 1);
                byte[] attachmentBytes = attachmentList[i];
                sb.Clear();
                sb.Append("--" + BoundaryMarker + CRLF);
                sb.Append("Content-Type: application/octet-stream" + CRLF);
                //sb.Append("Content-Type: image/jpeg" + CRLF); // แบบนี้ก็ work เหมือนกัน               
                sb.Append("Content-Transfer-Encoding: binary" + CRLF);
                sb.Append("Content-ID: <" + attachmentContentID + ">" + CRLF + CRLF);

                byte[] attachmentHeaderBytes = Encoding.UTF8.GetBytes(sb.ToString());
                byte[] CRLFbytes = Encoding.UTF8.GetBytes(CRLF);

                soapRequestBytes.AddRange(attachmentHeaderBytes);
                soapRequestBytes.AddRange(attachmentBytes);
                soapRequestBytes.AddRange(CRLFbytes);
            }

            string endingStr = "--" + BoundaryMarker + "--";
            byte[] endingBytes = Encoding.UTF8.GetBytes(endingStr);
            soapRequestBytes.AddRange(endingBytes);

            byte[] buffer = soapRequestBytes.ToArray();
            httpRequest.ContentLength = buffer.Length;

            try
            {
                requestStream = httpRequest.GetRequestStream();
                requestStream.Write(buffer, 0, buffer.Length);

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
            catch (WebException e)
            {
                string errStr = e.ToString();
                if (errStr.Contains("The operation has timed out"))
                {
                    return new byte[] { WebServiceResultStatus.ERROR_BYTE_HTTP_TIMEOUT };
                }
                if (errStr.Contains("The remote name could not be resolved"))
                {
                    return new byte[] { WebServiceResultStatus.ERROR_BYTE_REMOTE_NAME_NOT_RESOLVED };
                }

                using (var stream = e.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    string errorStr = reader.ReadToEnd();
                    Console.WriteLine(errorStr);

                    if (errorStr.Contains("Invalid parameters"))
                    {
                        return new byte[] { WebServiceResultStatus.ERROR_BYTE_INVALID_PARAMETERS };
                    }
                }

                return new byte[] { WebServiceResultStatus.ERROR_BYTE_99 };
            }
            catch (Exception e)
            {
                string errStr = e.ToString();
                if (errStr.Contains("The operation has timed out"))
                {
                    return new byte[] { WebServiceResultStatus.ERROR_BYTE_HTTP_TIMEOUT };
                }
                if (errStr.Contains("The remote name could not be resolved"))
                {
                    return new byte[] { WebServiceResultStatus.ERROR_BYTE_REMOTE_NAME_NOT_RESOLVED };
                }

                return new byte[] { WebServiceResultStatus.ERROR_BYTE_99 };
            }
            finally
            {
                if (httpResponse != null)            httpResponse.Close();
                if (responseStream != null)          responseStream.Close();
                if (requestStream != null)           requestStream.Close();  
            }               
        }

        private static byte[] SendSoapRequestToWebService(string soapRequestMessage, int timeout_seconds)
        {
            HttpWebResponse httpResponse = null;
            Stream responseStream = null;
            Stream requestStream = null;

            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(new Uri(DLT_WEB_SERVICE_URI));
            httpRequest.Method = "POST";
            httpRequest.ContentType = "text/xml; charset=utf-8";
            httpRequest.ProtocolVersion = HttpVersion.Version11;
            httpRequest.KeepAlive = true;
            httpRequest.Timeout = timeout_seconds * 1000;
           
            httpRequest.Headers.Add("username", GlobalData.HCT_USERNAME);
            httpRequest.Headers.Add("password", GlobalData.HCT_PASSWORD);

            X509Certificate2 p12cert = new X509Certificate2(Util.GetKeystorePath(), GlobalData.HCT_KEYSTORE_PASSWORD);
            httpRequest.ClientCertificates.Add(p12cert);

            // It can works without soapAction
            //string soapAction = "http://ws.eexam.dlt.go.th/EExamService/findStudentDetailRequest";
            //httpRequest.Headers.Add(String.Format("SOAPAction: \"{0}\"", soapAction));

            byte[] buffer = Encoding.UTF8.GetBytes(soapRequestMessage);
            httpRequest.ContentLength = buffer.Length;

            try
            {
                requestStream = httpRequest.GetRequestStream();
                requestStream.Write(buffer, 0, buffer.Length);

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
            catch (WebException e)
            {
                string errStr = e.ToString();
                if (errStr.Contains("The operation has timed out"))
                {
                    return new byte[] { WebServiceResultStatus.ERROR_BYTE_HTTP_TIMEOUT };
                }
                if (errStr.Contains("The remote name could not be resolved"))
                {
                    return new byte[] { WebServiceResultStatus.ERROR_BYTE_REMOTE_NAME_NOT_RESOLVED };
                }

                using (var stream = e.Response.GetResponseStream()) 
                {
                    using (var reader = new StreamReader(stream))
                    {
                        string errorStr = reader.ReadToEnd();
                        Console.WriteLine(errorStr);

                        if (errorStr.Contains("Invalid parameters"))
                        {
                            return new byte[] { WebServiceResultStatus.ERROR_BYTE_INVALID_PARAMETERS };
                        }
                    }

                    return new byte[] { WebServiceResultStatus.ERROR_BYTE_99 };
                }

            }
            catch (Exception e)
            {
                string errStr = e.ToString();
                if (errStr.Contains("The operation has timed out"))
                {
                    return new byte[] { WebServiceResultStatus.ERROR_BYTE_HTTP_TIMEOUT };
                }
                if (errStr.Contains("The remote name could not be resolved"))
                {
                    return new byte[] { WebServiceResultStatus.ERROR_BYTE_REMOTE_NAME_NOT_RESOLVED };
                }

                return new byte[] { WebServiceResultStatus.ERROR_BYTE_99 };
            }
            finally
            {
                if (httpResponse != null)            httpResponse.Close();
                if (responseStream != null)          responseStream.Close();
                if (requestStream != null)           requestStream.Close();  
            }
        }
    }

    class WebServiceResultStatus
    {
        public const string SUCCESS = "SUCCESS";
        public const byte ERROR_BYTE_HTTP_TIMEOUT = 0xFF;
        public const byte ERROR_BYTE_SERVER_INTERNAL = 0xFD;
        public const byte ERROR_BYTE_REMOTE_NAME_NOT_RESOLVED = 0xFC;
        public const byte ERROR_BYTE_INVALID_PARAMETERS = 0xFB;
        public const byte ERROR_BYTE_99 = 0xFE;

        public const string ERROR_HTTP_TIMEOUT = "ERROR_HttpTimeout";
        public const string ERROR_SERVER_INTERNAL = "ERROR_ServerInternal";
        public const string ERROR_REMOTE_NAME_NOT_RESOLVED = "ERROR_RemoteNameNotResolved";
        public const string ERROR_INVALID_PARAMETERS = "ERROR_InvalidParameters";
        public const string ERROR_99 = "ERROR_99";

        public const string ERROR_STUDENT_DETAIL_NOT_FOUND = "ERROR_FindStudentDetailWebService_StudentNotFound";
       
        public const string ERROR_CANNOT_LOAD_EEXAM = "ERROR_CannotLoadEExam";
        public const string ERROR_LOAD_EEXAM_EMPTY_RESPONSE = "ERROR_LoadEExamEmptyResponse";
        
        public const string ERROR_CANNOT_CHECK_EEXAM_RESULT = "ERROR_CannotCheckEExamResult";
        public const string ERROR_CHECK_EEXAM_RESULT_EMPTY_RESPONSE = "ERROR_CheckEExamResultEmptyResponse";

        public const string ERROR_CANNOT_CHECK_EEXAM_CORRECT_ANSWER = "ERROR_CannotCheckEExamCorrectAnswer";

        public const string ERROR_STUDENT_ENROL_NOT_FOUND = "ERROR_CourseRegistrationNotFound";

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
                    bytesCode[0] == ERROR_BYTE_REMOTE_NAME_NOT_RESOLVED ||
                    bytesCode[0] == ERROR_BYTE_INVALID_PARAMETERS ||
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
                    case ERROR_BYTE_99:                         return ERROR_99;
                    case ERROR_BYTE_HTTP_TIMEOUT:               return ERROR_HTTP_TIMEOUT;
                    case ERROR_BYTE_REMOTE_NAME_NOT_RESOLVED:   return ERROR_REMOTE_NAME_NOT_RESOLVED;
                    case ERROR_BYTE_INVALID_PARAMETERS:         return ERROR_INVALID_PARAMETERS;
                    case ERROR_BYTE_SERVER_INTERNAL:            return ERROR_SERVER_INTERNAL;
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
