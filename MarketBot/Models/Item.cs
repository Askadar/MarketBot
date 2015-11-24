using MarketBot.Services;
using MarketBot.ViewModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MarketBot.Models
{
    public class Item
    {

        static private TaskFactory Decider = new TaskFactory();

        private static Dictionary<int, string> status = new Dictionary<int, string>()
        {
            { 1, "Listed" }, { 2, "Sending" }, { 3, "Bought" }, { 4, "Recieving" }
        };
        public string Status
        {
            get
            {
                return status[ui_status];
            }
            set { }
        }


        public string ui_bid { get; set; }  // Bot id, 1 for send
        public int i_classid { get; set; }  // Identification stuff
        public int i_instanceid { get; set; }
        public int ui_id { get; set; }  //future key for dictionary
        public int ui_status { get; set; }  // item status, 1 - listed, 2 - sold, 3 - bought, 4 - ready to recive
        public double ui_price { get; set; }  // Price in info
        public double i_market_price { get; set; }  //Sell cost
        public string i_name { get; set; } // Human-readable name
        public int position { get; set; }  // How long to arrive to market first lines

        public double s_price { get; set; }  // Sell price
        public double b_price { get; set; }  // Buy price
        public double revenue { get; set; }  // Profit
                                             //not implemented
                                             //private double b_st_profit; // Possible profit of buying on steam and selling on tm
                                             //private double s_st_profit; // Possible profit of selling on steam and buiyng on tm
                                             //will be implemented - steambuy profit, steamsell profit, swap(margin), else 

        public Item()
        {
            i_name = "xxx";
            ui_status = 1;
        }
        public Item(int id, double s, double b)
        {
            this.ui_id = id;
            this.s_price = s;
            this.b_price = b;
        }
        Item(Item inItem)
        {
            ui_status = inItem.ui_status;
            ui_bid = inItem.ui_bid;
            ui_price = inItem.ui_price;
            i_market_price = inItem.i_market_price;
        }
        public override string ToString()
        {
            return "Name: "+i_name+" ID: "+ui_id;
        }
        public override bool Equals(object obj)
        {
            var item = obj as Item;
            return Equals(item);
        }
        public bool Equals(Item i)
        {
            if ((object)i == null)
            {
                return false;
            }

            return (ui_id == i.ui_id);
        }
        public void TrySend()
        {
            bool success = false;
            do
            {
                success = Send();
                Task.Delay(10000);
            } while (!success);
        }

        public void TryRecieve()
        {
            bool success = false;
            do
            {
                success = Recieve();
                Task.Delay(10000);
            } while (!success);
        }
        private bool Send()
        {

            //Console.WriteLine("Да начнется отправка!");
            string result = Request.GetResponseTo("https://market.csgo.com/api/ItemRequest/{0}/{1}/?key={2}", "in", "1", ViewModelMain.Apikey);
            JToken resp = JObject.Parse(result);
            //bool success = (bool)resp["success"];
            //Console.WriteLine("Response: {0}", resp.error);
            if ((bool)resp["success"] == true)
            {
                s_price = ui_price;
                return true;
                //Console.WriteLine("Sending {0} to bot with secret: {1}. \nPrice: {2} | Revenue: not implemented", i_name, resp.secret, ui_price); //b_price - s_price
                //moving = false;
                //if (isListed == true)
                //{
                //    Console.WriteLine("We sent wold item!");
                //    DrawL -= this.DrawElement;
                //    isListed = false;
                //}
            }
            //Re-try in 25 seconds
            else
                return false;
            //Console.WriteLine("Aw, sh*t! We caught error: " + resp.error);
            //Thread.Sleep(TimeSpan.FromSeconds(25));


        }
        private bool Recieve()
        {
            string result = Request.GetResponseTo("https://market.csgo.com/api/ItemRequest/{0}/{1}/?key={2}", "out", ui_bid, ViewModelMain.Apikey);
            JToken resp = JObject.Parse(result);
            if ((bool)resp["success"] == true)
            {
                b_price = ui_price;
                return true;
            }
            else
                return false;
        }

        internal void Decide()
        {
            switch (ui_status)
            {
                case 2:
                    Decider.StartNew(TrySend);  //Task send = 
                    break;
                case 4:
                    Decider.StartNew(TryRecieve);  //Task recieve = 
                    break;
                default:
                    break;
            }
        }
    }
}