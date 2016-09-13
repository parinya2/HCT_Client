﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using Microsoft.Office.Interop.Excel;
using System.Reflection;
using System.Net.Mail;

namespace HCT_Client
{
    public class Util
    {
        private static Dictionary<int, Color> buttonBlinkColorDict;

        public static void SendEmailWithAttachment(string pdfName, string emailBody)
        {
            string hctEmail = "hct.agent@gmail.com";
            
            SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;
            client.Timeout = 10000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(hctEmail, ExtractEmailPassword());

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(hctEmail);
            mail.Subject = pdfName.Replace(".pdf","");
            mail.SubjectEncoding = UTF8Encoding.UTF8;
            mail.Body = emailBody;
            mail.BodyEncoding = UTF8Encoding.UTF8;
            string[] emailAdmins = readEmailAdminFromTextFile();
            for (int i = 0; i < emailAdmins.Length; i++)
            {
                string email = emailAdmins[i] + "";
                try
                {
                    mail.To.Add(email.Trim());
                }
                catch (Exception ex)
                {
                    string createText = ex.ToString() + Environment.NewLine;
                    File.WriteAllText(GetSendMailErrorLogPath1(), createText);
                }                
            }
            mail.To.Add(hctEmail);           
            mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

            string EXAM_RESULT_PDF_PATH = GetExecutingPath() + "/" + pdfName;
            Attachment attachment;
            attachment = new System.Net.Mail.Attachment(EXAM_RESULT_PDF_PATH);
            attachment.Name = pdfName;
            mail.Attachments.Add(attachment);

            try
            {
                client.Send(mail);
                attachment.Dispose();
                DeleteFileIfExists(EXAM_RESULT_PDF_PATH);
            }
            catch (Exception ex)
            {
                attachment.Dispose();
                string createText = ex.ToString() + Environment.NewLine;
                File.WriteAllText(GetSendMailErrorLogPath2(), createText);
            }
                     
            //https://www.google.com/settings/security/lesssecureapps
        }

        static string GetExecutingPath()
        {
            return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        }

        static string GetSendMailErrorLogPath1()
        {
            return GetExecutingPath() + "/Private/ErorLog1.txt";
        }

        static string GetSendMailErrorLogPath2()
        {
            return GetExecutingPath() + "/Private/ErorLog2.txt";
        }

        static string GetTokenPath()
        {
            return GetExecutingPath() + "/Private/token";
        }

        static string ExtractEmailPassword()
        {
            string text = "" + File.ReadAllText(GetTokenPath(), Encoding.UTF8);
            text = text.Trim();
            StringBuilder result = new StringBuilder("");
            for (int i = 0; i < text.Length; i++)
            {
                if (i % 2 == 0)
                {
                    result.Append(text[i]);
                }
            }
            return result.ToString();
        }

        public static string[] readEmailAdminFromTextFile()
        {
            string[] result;
            string text = "" + File.ReadAllText(GetExecutingPath() + "/Config/Email_admin.txt", Encoding.UTF8);
            text = text.Trim();
            if (text.Contains(","))
            {
                result = text.Split(',');
            }
            else
            {
                result = new string[1];
                result[0] = text;
            }
            return result;
        }

        public static string CreateExamResultPDF(string userFullname, string citizenID, string courseName, string passOrFail, string dateString)
        {
            string dateStringForFileName = dateString.Replace("/", "-");

            string executingPath = GetExecutingPath();
            string EXAM_RESULT_TEMPLATE_PATH = executingPath + "/ExamResultTemplate.xls";
            string EXAM_RESULT_TEMPLATE_TEMP_PATH = executingPath + "/ExamResultTemplate_Temp.xls";

            string userFullNameForFile = userFullname.Replace(" ", "_");
            string PDF_NAME = "ผลการสอบ_" + userFullNameForFile + "_" + dateStringForFileName + ".pdf";
            string EXAM_RESULT_PDF_PATH = executingPath + "/" + PDF_NAME;

            Application app = new Application();
            Workbook workBook = app.Workbooks.Open(EXAM_RESULT_TEMPLATE_PATH);
            Sheets workSheets = workBook.Worksheets;
            Worksheet workSheet1 = workSheets.get_Item("Sheet1");
            workSheet1.Cells[10, 3] = userFullname;
            workSheet1.Cells[11, 3] = citizenID;
            workSheet1.Cells[12, 3] = courseName;
            workSheet1.Cells[13, 3] = passOrFail;
            workSheet1.Cells[14, 3] = dateString;

            workBook.SaveAs(EXAM_RESULT_TEMPLATE_TEMP_PATH);
            workBook.ExportAsFixedFormat(XlFixedFormatType.xlTypePDF, EXAM_RESULT_PDF_PATH);
            workBook.Close();

            DeleteFileIfExists(EXAM_RESULT_TEMPLATE_TEMP_PATH);

            return PDF_NAME;
        }

        static void DeleteFileIfExists(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        public static void printLine(string msg)
        {
            System.Diagnostics.Debug.WriteLine(msg + "\n");
        }

        public static string GetSoapParamStr(int paramNo)
        {
            return "HCT_param" + paramNo;
        }

        public static string GetSoapXmlTemplate(int operationNo)
        {
            System.Reflection.Assembly myAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            string path = "HCT_Client.Private.SOAP.op" + operationNo + ".xml";
            Stream myStream = myAssembly.GetManifestResourceStream(path);
            StreamReader reader = new StreamReader(myStream);
            string result = reader.ReadToEnd();

            return result;
        }
        public static Bitmap GetImageFromImageResources(string imageName)
        {
            System.Reflection.Assembly myAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            string path = "HCT_Client.Images." + imageName;
            Stream myStream = myAssembly.GetManifestResourceStream(path);
            return new Bitmap(myStream);
        }

        public static void GenerateButtonBlinkColorDict()
        {
            buttonBlinkColorDict = new Dictionary<int, Color>();

            int stateCount = 40;
            int H = 30;
            int S = 240;
            int L = 240;
            for (int i = 1; i <= stateCount; i++)
            {
                L = 240 - i * 3;
                Color targetColor = ColorFromHSL(H, S, L);
                buttonBlinkColorDict[i] = targetColor;
            }

            for (int i = 1; i <= 3; i++)
            {
                buttonBlinkColorDict[stateCount + i] = buttonBlinkColorDict[stateCount];
            }
        }

        public static Color GetButtonBlinkColorAtSignalState(int state)
        {
            return buttonBlinkColorDict[state];
        }

        public static Color ColorFromHSL(int H, int S, int L)
        {
            double hue = (H / 240.0) * 360.0;
            double sat = S / 240.0;
            double lightness = L / 240.0;

            double C = (1 - Math.Abs(2 * lightness - 1)) * sat;
            double hueTmp = hue / 60.0;
            double X = C * (1 - Math.Abs((hueTmp % 2) - 1));
            double m = lightness - C / 2;
           
            double tmpR = 0, tmpG = 0, tmpB = 0;
            if (hue >= 0 && hue < 60)
            {
                tmpR = C;
                tmpG = X;
                tmpB = 0;
            }
            else if (hue >= 60 && hue < 120)
            {
                tmpR = X;
                tmpG = C;
                tmpB = 0;
            }
            else if (hue >= 120 && hue < 180)
            {
                tmpR = 0;
                tmpG = C;
                tmpB = X;
            }
            else if (hue >= 180 && hue < 240)
            {
                tmpR = 0;
                tmpG = X;
                tmpB = C;
            }
            else if (hue >= 240 && hue < 300)
            {
                tmpR = X;
                tmpG = 0;
                tmpB = C;
            }
            else if (hue >= 300 && hue < 360)
            {
                tmpR = C;
                tmpG = 0;
                tmpB = X;
            }

            int newR = (int)((tmpR + m) * 255);
            int newG = (int)((tmpG + m) * 255);
            int newB = (int)((tmpB + m) * 255);
            
            return Color.FromArgb(255, newR, newG, newB);
        }
    }
}
