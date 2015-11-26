using MarketBot.ViewModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MarketBot.Services
{
    class PingPong
    {
        PingPong()
        {
            Thread ping = new Thread(Ping); ping.Name = "Pinging";
            ping.Start();
        }
        async public static void Ping()
        {
            do
            {
                string res = (Request.GetResponseTo("https://market.csgo.com/api/PingPong/?key={0}", ViewModelMain.Apikey));

                try
                {
                    JObject ping = JObject.Parse(res);
                    if ((ping["success"].ToString()) == "True")
                        Console.WriteLine("[Ping]-[{0}] Successful ping", DateTime.Now);
                }
                catch (Exception)
                {
                    ///say some stuff about wrong ping
                }
                finally
                {
                    await Task.Delay(TimeSpan.FromSeconds(185));
                }

            } while (ViewModelMain.Executing);
        }
    }
}
