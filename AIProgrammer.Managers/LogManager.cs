using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AIProgrammer.Managers
{
    public static class LogManager
    {
        /// <summary>
        /// Logs a message to the logging web service Loggly.
        /// </summary>
        /// <param name="message">string</param>
        /// <param name="tag">string</param>
        /// <param name="logglyKey">Loggly API key</param>
        public static string Log(string message, string tag, string logglyKey)
        {
            string result = "";

            try
            {
                string url = "https://logs-01.loggly.com/inputs/" + logglyKey + "/tag/" + tag.Replace(" ", "_") + "/";

                using (WebClient client = new WebClient())
                {
                    byte[] response = client.UploadValues(url, new NameValueCollection() { { "Message", message } });
                    result = Encoding.UTF8.GetString(response);
                }
            }
            catch (Exception ex)
            {
                result = ex.ToString();
            }

            return result;
        }
    }
}
