
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Web;


namespace ConsoleApplication1
{
    class Program
    {

        public const string SMS_TYPE_CLASSIC = "classic";
        public const string SMS_TYPE_CLASSIC_PLUS = "classic_plus";
        public const string SMS_TYPE_BASIC = "basic";
        public const string SMS_TYPE_TEST_CLASSIC = "test_classic";
        public const string SMS_TYPE_TEST_CLASSIC_PLUS = "test_classic_plus";
        public const string SMS_TYPE_TEST_BASIC = "test_basic";


        static void Main(string[] args)
        {
            // Single dispatch
            String[] recipients = new String[] { "393471234567" };

            // Multiple dispatch
            // String[] recipients = new String[] { "393471234567", "393477654321" };

            Hashtable result = new Hashtable();
            Hashtable credit_result = new Hashtable();
            String line;

            // ------------ SMS Classic dispatch --------------

            // SMS CLASSIC dispatch with custom alphanumeric sender
               result = skebbyGatewaySendSMS("username", "password", recipients, "Hi Mike, how are you?", SMS_TYPE_CLASSIC, "", "John","","");

            // SMS CLASSIC dispatch with custom numeric sender
            // result = skebbyGatewaySendSMS("username","password",recipients,"Hi Mike, how are you?", SMS_TYPE_CLASSIC,"393471234567","","","");

            // ------------ SMS Basic dispatch --------------
            // result = skebbyGatewaySendSMS("username","password",recipients,"Hi Mike, how are you? By John", SMS_TYPE_BASIC, "", "","","");

            // ------------ SMS Classic Plus dispatch --------------

            // SMS CLASSIC PLUS dispatch (with delivery report) with custom alphanumeric sender
            // result = skebbyGatewaySendSMS("username","password",recipients,"Hi Mike, how are you?", SMS_TYPE_CLASSIC_PLUS,"","John","","");

            // SMS CLASSIC PLUS dispatch (with delivery report) with custom numeric sender
            // result = skebbyGatewaySendSMS("username","password",recipients,"Hi Mike, how are you?", SMS_TYPE_CLASSIC_PLUS,"393471234567","","","");

            // SMS CLASSIC PLUS dispatch (with delivery report) with custom alphanumeric sender and custom reference string
            // result = skebbyGatewaySendSMS("username", "password", recipients, "Hi Mike, how are you?", SMS_TYPE_CLASSIC_PLUS, "393471234567", "", "riferimento", "");


            //  ------------------------------------------------------------------
			//     WARNING! THE SMS_TYPE_TEST* SMS TYPES DOESN'T DISPATCH ANY SMS 
			//     USE THEM ONLY TO CHECK IF YOU CAN REACH THE SKEBBY SERVER 
			//  ------------------------------------------------------------------

            // ------------- SMS Classic test dispatch ---------

            // SMS CLASSIC test dispatch with custom alphanumeric sender
            // result = skebbyGatewaySendSMS("username","password",recipients,"Hi Mike, how are you?", SMS_TYPE_TEST_CLASSIC,"","John","","");

            // SMS CLASSIC test dispatch with custom numeric sender
            // result = skebbyGatewaySendSMS("username","password",recipients,"Hi Mike, how are you?", SMS_TYPE_TEST_CLASSIC,"393471234567","","","");


            // ------------ SMS Classic Plus test dispatch -----------

            // SMS CLASSIC PLUS test dispatch (with delivery report) with custom alphanumeric sender
            // result = skebbyGatewaySendSMS("username","password",recipients,"Hi Mike, how are you?", SMS_TYPE_TEST_CLASSIC_PLUS,"","John","","");

            // SMS CLASSIC PLUS test dispatch (with delivery report) with custom numeric sender
            // result = skebbyGatewaySendSMS("username","password",recipients,"Hi Mike, how are you?", SMS_TYPE_TEST_CLASSIC_PLUS,"393471234567","","","");

            // ------------- SMS Basic test dispatch ----------------
            // result = skebbyGatewaySendSMS("username","password",recipients,"Hi Mike, how are you? By John", SMS_TYPE_TEST_BASIC,"","","","");

            if ((string)result["status"] == "success")
            {
                Console.WriteLine("Message Sent!");
                Console.WriteLine("Remaining SMS: " + result["remaining_sms"]);

                if (result.ContainsKey("id"))
                {
                    Console.WriteLine("ID: " + result["id"]);
                }

            }
			
			// ------------------------------------------------------------------
			// Controlla la documentazione completa all'indirizzo http://www.skebby.it/business/index/send-docs/ 
			// ------------------------------------------------------------------
			// Per i possibili errori si veda http://www.skebby.it/business/index/send-docs/#errorCodesSection
			// ATTENZIONE: in caso di errore Non si deve riprovare l'invio, trattandosi di errori bloccanti
			// ------------------------------------------------------------------			
            if ((string)result["status"] == "failed")
            {
                Console.WriteLine("Sending failed");
                Console.WriteLine("Code: " + result["code"]);
                Console.WriteLine("Message: " + result["message"]);
            }
            line = Console.ReadLine();

            //// ------------ Controllo del CREDITO RESIDUO -------------
            //credit_result = skebbyGatewayGetCredit("username", "password", "");

            //if ((string)credit_result["status"] == "success")
            //{
            //    Console.WriteLine("Current Balance: " + credit_result["credit_left"]);
            //    Console.WriteLine("Remaining SMS Classic: " + credit_result["classic_sms"]);
            //    Console.WriteLine("Remaining SMS Basic: " + credit_result["basic_sms"]);
            //}

            //if ((string)credit_result["status"] == "failed")
            //{
            //    Console.WriteLine("Sending request failed");
            //}
            //line = Console.ReadLine();
        }

        private static Hashtable skebbyGatewaySendSMS(
            String username, String password,
            String[] recipients, String text,
            String sms_type, String sender_number,
            String sender_string,
            String user_reference,
            String charset)
        {
            String url = "http://gateway.skebby.it/api/send/smseasy/advanced/http.php";
            String result = "";
            String[] results, temp;
            String parameters = "";
            String method = "send_sms_classic";
            int i = 0;
            StreamWriter myWriter = null;
            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(url);
            objRequest.ServicePoint.Expect100Continue = false;
            
            Hashtable r = new Hashtable();

            switch (sms_type)
            {
                case SMS_TYPE_CLASSIC:
                    method = "send_sms_classic";
                    break;
                case SMS_TYPE_CLASSIC_PLUS:
                    method = "send_sms_classic_report";
                    break;
                case SMS_TYPE_BASIC:
                    method = "send_sms_basic";
                    break;
                case SMS_TYPE_TEST_CLASSIC:
                    method = "test_send_sms_classic";
                    break;
                case SMS_TYPE_TEST_CLASSIC_PLUS:
                    method = "test_send_sms_classic_report";
                    break;
                case SMS_TYPE_TEST_BASIC:
                    method = "test_send_sms_basic";
                    break;
                default:
                    method = "send_sms_classic";
                    break;
            }

            parameters = "method=" + HttpUtility.UrlEncode(method) + "&"
                         + "username=" + HttpUtility.UrlEncode(username) + "&password=" + HttpUtility.UrlEncode(password) + "&"
                         + "text=" + HttpUtility.UrlEncode(text) + "&"
                         + "recipients[]=" + string.Join("&recipients[]=", recipients);

            if (sender_number != "" && sender_string != "")
            {
                r.Add("status", "failed");
                r.Add("code", "0");
                r.Add("message", "You can specify only one type of sender, numeric or alphanumeric");
                return r;
            }

            parameters += sender_number != "" ? "&sender_number=" + HttpUtility.UrlEncode(sender_number) : "";
            parameters += sender_string != "" ? "&sender_string=" + HttpUtility.UrlEncode(sender_string) : "";

            parameters += user_reference != "" ? "&user_reference=" + HttpUtility.UrlEncode(user_reference) : "";

            switch (charset)
            {
                case "UTF-8":
                    parameters += "&charset=" + HttpUtility.UrlEncode("UTF-8");
                    break;
                default:
                    break;
            }

            objRequest.Method = "POST";
            objRequest.ContentLength = Encoding.UTF8.GetByteCount(parameters);
            objRequest.ContentType = "application/x-www-form-urlencoded";
            HttpWebResponse objResponse;
            try
            {
                myWriter = new StreamWriter(objRequest.GetRequestStream());
                myWriter.Write(parameters);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }
            finally
            {
                myWriter.Close();
            }
            try
            {
                objResponse = (HttpWebResponse)objRequest.GetResponse();
            }
            catch (WebException e)
            {
                r.Add("status", "failed");
                r.Add("code", "0");
                r.Add("message", "Network error, unable to send the message");
                return r;
            }
            using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
            {
                result = sr.ReadToEnd();
                // Close and clean up the StreamReader
                sr.Close();
            }
            results = result.Split('&');
            for (i = 0; i < results.Length; i++)
            {
                temp = results[i].Split('=');
                r.Add(temp[0], temp[1]);
            }
            return r;
        }

        private static Hashtable skebbyGatewayGetCredit(
           String username, String password,
           String charset)
        {
            String url = "http://gateway.skebby.it/api/send/smseasy/advanced/http.php";
            String method = "get_credit";
            String result = "";
            String[] results, temp;
            String parameters = "";
            int i = 0;
            StreamWriter myWriter = null;
            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(url);
            Hashtable r = new Hashtable();

            parameters = "method=" + HttpUtility.UrlEncode(method) + "&"
                       + "username=" + HttpUtility.UrlEncode(username) + "&"
                       + "password=" + HttpUtility.UrlEncode(password);

            switch (charset)
            {
                case "UTF-8":
                    parameters += "&charset=" + HttpUtility.UrlEncode("UTF-8");
                    break;
                default:
                    break;
            }

            objRequest.Method = "POST";
            objRequest.ContentLength = Encoding.UTF8.GetByteCount(parameters);
            objRequest.ContentType = "application/x-www-form-urlencoded";
            HttpWebResponse objResponse;
            try
            {
                myWriter = new StreamWriter(objRequest.GetRequestStream());
                myWriter.Write(parameters);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }
            finally
            {
                myWriter.Close();
            }
            try
            {
                objResponse = (HttpWebResponse)objRequest.GetResponse();
            }
            catch (WebException e)
            {
                r.Add("status", "failed");
                r.Add("code", "0");
                r.Add("message", "Network error, unable to send the message");
                return r;
            }
            using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
            {
                result = sr.ReadToEnd();
                // Close and clean up the StreamReader
                sr.Close();
            }
            results = result.Split('&');
            for (i = 0; i < results.Length; i++)
            {
                temp = results[i].Split('=');
                r.Add(temp[0], temp[1]);
            }
            return r;

        }
    }
}