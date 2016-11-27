using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Core;
using System.Reflection;
using System.Net.Mail;
using System.Media;
using System.Drawing.Imaging;
using System.Net.Mime;
using System.Net;

namespace HCT_Client
{
    public class Util
    {
        private static Dictionary<int, Color> buttonBlinkColorDict;

        public static void SendEmailWithAttachment(string pdfName, string emailBody)
        {
            string hctEmail = GlobalData.HCT_EMAIL; ;
            
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
            attachment = new Attachment(EXAM_RESULT_PDF_PATH);
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

        public static string GetKeystorePath()
        {
            return GetExecutingPath() + "/Private/Keystore/hct_keystore.p12";
        }

        static string GetTokenPath()
        {
            return GetExecutingPath() + "/Private/token";
        }

        public static string GetSimulatorQuizFolderPath()
        {
            string folderPath = GetExecutingPath() + "/SimulatorQuiz";
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            return folderPath;
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

        public static Dictionary<string, string> CreateExamResultPDF(string userFullname, 
                                                 string citizenID, 
                                                 string courseName, 
                                                 string passOrFail, 
                                                 string examStartDateString,
                                                 string examEndDateString,
                                                 string courseRegisterDateString,
                                                 string paperTestNumber,
                                                 string examSeq)
        {
            string dateStringForFileName = examStartDateString.Replace("/", "-");
            dateStringForFileName = dateStringForFileName.Replace(":", "-");
            dateStringForFileName = dateStringForFileName.Replace(" ", "_เวลา_");

            string executingPath = GetExecutingPath();
            string EXAM_RESULT_TEMPLATE_PATH = executingPath + "/ExamResultTemplate.xls";
            string EXAM_RESULT_TEMPLATE_TEMP_PATH = executingPath + "/ExamResultTemplate_Temp.xls";
            string USER_PHOTO_TEMP_PATH = executingPath + "/UserPhoto.jpeg";

            string userFullNameForFile = userFullname.Replace(" ", "_");
            string PDF_NAME = "ผลการสอบ_" + userFullNameForFile + "_วันที่_" + dateStringForFileName + ".pdf";
            string EXAM_RESULT_PDF_PATH = executingPath + "/" + PDF_NAME;

            Application app = new Application();
            Workbook workBook = app.Workbooks.Open(EXAM_RESULT_TEMPLATE_PATH);
            Sheets workSheets = workBook.Worksheets;
            Worksheet workSheet1 = workSheets.get_Item("Sheet1");
            workSheet1.Cells[10, 3] = userFullname;
            workSheet1.Cells[11, 3] = citizenID;
            workSheet1.Cells[12, 3] = courseName;
            workSheet1.Cells[13, 3] = courseRegisterDateString;
            workSheet1.Cells[14, 3] = examSeq;
            workSheet1.Cells[15, 3] = paperTestNumber;
            workSheet1.Cells[16, 3] = passOrFail;
            workSheet1.Cells[17, 3] = examStartDateString;
            workSheet1.Cells[18, 3] = examEndDateString;
            if (!useCitizenID)
            {
                workSheet1.Cells[11, 1] = "หมายเลข Passport";
            }

            Image userPhoto = UserProfileManager.GetUserPhoto();
            if (userPhoto != null)
            {
                userPhoto.Save(USER_PHOTO_TEMP_PATH, ImageFormat.Jpeg);
                workSheet1.Shapes.AddPicture(USER_PHOTO_TEMP_PATH, MsoTriState.msoFalse, MsoTriState.msoCTrue, 360, 160, 100, 100);
            }

            workBook.SaveAs(EXAM_RESULT_TEMPLATE_TEMP_PATH);
            workBook.ExportAsFixedFormat(XlFixedFormatType.xlTypePDF, EXAM_RESULT_PDF_PATH);
            workBook.Close();

            DeleteFileIfExists(EXAM_RESULT_TEMPLATE_TEMP_PATH);
            DeleteFileIfExists(USER_PHOTO_TEMP_PATH);

            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict["pdfName"] = PDF_NAME;

            byte[] pdfBytes = File.ReadAllBytes(EXAM_RESULT_PDF_PATH);
            string pdfBase64String = Convert.ToBase64String(pdfBytes);

            File.WriteAllText(@"C:\Users\PRINYA\Desktop\pdfBase64.txt", pdfBase64String);
            dict["pdfBase64String"] = pdfBase64String;
            return dict;
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

        public static Bitmap GetImageFromImageResources(string imageName)
        {
            System.Reflection.Assembly myAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            string path = "HCT_Client.Images." + imageName;
            Stream myStream = myAssembly.GetManifestResourceStream(path);
            return new Bitmap(myStream);
        }

        public static SoundPlayer GetSoundPlayerFromSoundResources(string soundName)
        {
            System.Reflection.Assembly myAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            string path = "HCT_Client.Sounds." + soundName;
            Stream myStream = myAssembly.GetManifestResourceStream(path);
            return new SoundPlayer(myStream);
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

        public static int GetMinuteDifferentOfTwoDates(DateTime dt1, DateTime dt2)
        {
            TimeSpan ts = dt2.Subtract(dt1);
            int totalMinutes = (int)ts.TotalMinutes + 1;
            return totalMinutes;
        }

        public static string ConvertDateToMariaDBDateString(DateTime dt)
        {
            string year = dt.Year < 2500 ? "" + dt.Year : "" + (dt.Year - 543);
            string month = dt.Month < 10 ? "0" + dt.Month : "" + dt.Month;
            string day = dt.Day < 10 ? "0" + dt.Day : "" + dt.Day;
            string hour = dt.Hour < 10 ? "0" + dt.Hour : "" + dt.Hour;
            string minute = dt.Minute < 10 ? "0" + dt.Minute : "" + dt.Minute;
            string second = dt.Second < 10 ? "0" + dt.Second : "" + dt.Second;

            string result = year + "-" + month + "-" + day + "T" + hour + ":" + minute + ":" + second;
            return result;
        }

        public static string ConvertDateToNormalDateString(DateTime dt, bool showTimeString)
        {
            string year = dt.Year < 2500 ? "" + (dt.Year + 543) : "" + dt.Year;
            string month = dt.Month < 10 ? "0" + dt.Month : "" + dt.Month;
            string day = dt.Day < 10 ? "0" + dt.Day : "" + dt.Day;
            string hour = dt.Hour < 10 ? "0" + dt.Hour : "" + dt.Hour;
            string minute = dt.Minute < 10 ? "0" + dt.Minute : "" + dt.Minute;
            string second = dt.Second < 10 ? "0" + dt.Second : "" + dt.Second;

            string result = day + "/" + month + "/" + year;
            if (showTimeString)
            {
                result += " " + hour + ":" + minute + ":" + second;
            }
            return result;
        }

        public static bool isBitmapEqual(Bitmap bmp1, Bitmap bmp2)
        {
            if (!bmp1.Size.Equals(bmp2.Size))
            {
                return false;
            }
            
            BitmapData bmp1Data = bmp1.LockBits(new System.Drawing.Rectangle(0, 0, bmp1.Width, bmp1.Height), ImageLockMode.ReadOnly, bmp1.PixelFormat);
            BitmapData bmp2Data = bmp2.LockBits(new System.Drawing.Rectangle(0, 0, bmp2.Width, bmp2.Height), ImageLockMode.ReadOnly, bmp2.PixelFormat);

            IntPtr ptr1 = bmp1Data.Scan0;
            IntPtr ptr2 = bmp2Data.Scan0;

            int numBytes = bmp1Data.Stride * bmp1.Height;
            byte[] bytes1 = new byte[numBytes];
            byte[] bytes2 = new byte[numBytes];

            System.Runtime.InteropServices.Marshal.Copy(ptr1, bytes1, 0, numBytes);
            System.Runtime.InteropServices.Marshal.Copy(ptr2, bytes2, 0, numBytes);

            bool isEqual = true;
            for (int i = 0; i < numBytes; i++)
            {
                if (bytes1[i] != bytes2[i])
                {
                    isEqual = false;
                    break;
                }
            }

            bmp1.UnlockBits(bmp1Data);
            bmp2.UnlockBits(bmp2Data);

            return isEqual;         
        }

        public static Bitmap GetBitmapFromBytes(byte[] imageData)
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

        public static byte[] GetJpegBytesFromImage(Image image)
        {
            var stream = new MemoryStream();
            image.Save(stream, ImageFormat.Jpeg);
            List<byte> result = new List<byte>();
            byte[] tmpBuffer = stream.GetBuffer();
            for (int i = 0; i < tmpBuffer.Length - 1; i++)
            {
                bool reachEndOfImage = false;
                if (i >= 1)
                {
                    if (tmpBuffer[i - 1] == 0xff && tmpBuffer[i] == 0xd9)
                    {
                        reachEndOfImage = true;
                    }               
                }
                result.Add(tmpBuffer[i]);
                if (reachEndOfImage)
                {
                    break;
                }
            }
            stream.Close();
            return result.ToArray();
        }

        public static string GetUTF8fromHTMLEntity(string str)
        {
            if (str.Contains("&#") && str.Contains(";"))
            {
                string newStr = WebUtility.HtmlDecode(str);
                return newStr;
            }
            return str;
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
