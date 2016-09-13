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
 
        public static string SendSoapRequestToWebService(string soapContent)
        {
            try
            {             
                HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(new Uri(DLT_WEB_SERVICE_URI));
                httpRequest.Method = "POST";
                httpRequest.ContentType = "text/xml; charset=utf-8";
                httpRequest.ProtocolVersion = HttpVersion.Version11;
                httpRequest.Timeout = 6000;

                // It can works without soapAction
                //string soapAction = "http://ws.eexam.dlt.go.th/EExamService/findStudentDetailRequest";
                //httpRequest.Headers.Add(String.Format("SOAPAction: \"{0}\"", soapAction));

                byte[] buffer = Encoding.UTF8.GetBytes(soapContent);
                httpRequest.ContentLength = buffer.Length;

                Stream requestStream = httpRequest.GetRequestStream();
                requestStream.Write(buffer, 0, buffer.Length);

                HttpWebResponse response = (HttpWebResponse)httpRequest.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader responseStreamReader = new StreamReader(responseStream);

                string result = responseStreamReader.ReadToEnd();
                return result;
            }
            catch (Exception e)
            {
                return WebServiceError.ERROR_99;
            }
        }
    }

    class WebServiceError
    {
        public static string ERROR_99 = "ERROR_99_SendSoapRequestToWebService_Throw";
    }
}
