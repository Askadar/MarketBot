using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MarketBot.Services
{
    public class Request
    {
        string request;
        string[] keys;
        private string response;

        public Request(string r, params string[] k)
        {
            request = r; keys = k;           
            
        }
        public static string GetResponseTo(string r, params string[] k)
        {
            string request = String.Format(r, k);
            string response = null;
            WebRequest webRequest;
            Stream responseStream = null;
            int i = 0;
            do
            {
                try
                {
                    webRequest = WebRequest.Create(request);
                    responseStream = webRequest.GetResponse().GetResponseStream();
                }
                catch (Exception)
                {
                    Log.Addressed("Caught an exception. Trying to recconect...", typeof(Request).Name);
                    if (i < 5)
                    {
                        Log.Addressed("Failed to connect...", typeof(Request).Name);
                    }
                    i++;
                    Thread.Sleep(TimeSpan.FromSeconds(5));
                }
                if (responseStream != null)
                {

                    using (StreamReader objReader = new StreamReader(responseStream))
                    {
                        response = objReader.ReadLine();
                    }
                }
            } while (response == null);
            return response;
        }
    }
}
