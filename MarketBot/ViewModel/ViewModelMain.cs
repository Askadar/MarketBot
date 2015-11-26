
//#define new DEBUG_NEW

using MarketBot.Models;
using MarketBot.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
namespace MarketBot.ViewModel
{
    class Settings
    {
        string Apikey { get; set; }

        public Settings()
        {
            try { Apikey = JsonConvert.DeserializeObject<string>(File.ReadAllText("settings.json")); }
            catch { }
        }
        public static string LoadApi() {
            string Apikey = "";
            try { Apikey = JsonConvert.DeserializeObject<string>(File.ReadAllText("settings.json")); }
            catch { }
            return Apikey;
        }
        public static void SaveApi(string api)
        {
            File.WriteAllText("settings.json",JsonConvert.SerializeObject(api));
        }
    }
    class ViewModelMain : ViewModelBase
    {
        //private string filePath = "db.json";
        private double balance = 0;
        private static object _itemsLock = new object();
        private static string _apikey = Settings.LoadApi(); //blah, fix dat somehow...
        private double updateInterval = 30;
        private static bool executing;


        public static string Apikey { get { return _apikey; } set { _apikey = value; } }
        static public string update_req = "https://market.csgo.com/api/Trades/?key={0}"; //remove dat shiet
        public string FilePath { get; set; }
        public Inventory Items { get; set; }
        public double Balance
        {
            get { return balance; }
            set
            {
                if (value != balance)
                {
                    balance = value;
                    NotifyPropertyChanged("Balance");
                }
            }
        }
        public double UpdateInterval
        {
            get { return updateInterval; }
            set { updateInterval = value; }
        }
        public static bool Executing
        {
            get { return executing; }
            set { executing = value;
                //set token to stop all
            }
        }



        //public Inventory items { get; set; }
        //public RelayCommand Update { get; set; }
        CancellationTokenSource updateTokenSource;
        public ViewModelMain()
        {
            Items = new Inventory();
            Load();
            //UpdateInventory();
            updateTokenSource = new CancellationTokenSource();
            executing = true;
            Task Pinging = new Task(PingPong.Ping);
            Task InvUpdate = new Task(UpdateInventory, updateTokenSource.Token);
            Pinging.Start();
            InvUpdate.Start();
            BindingOperations.EnableCollectionSynchronization(Items, _itemsLock); //testing-etc
        }
        ~ViewModelMain()
        {
            executing = false;
            //add update and ping stoppers
            Save();
            Settings.SaveApi(Apikey);
        }
        void UpdateBalance()
        {
            string money = (Request.GetResponseTo("https://market.csgo.com/api/GetMoney/?key={0}", Apikey));
            JObject Jmoney = JObject.Parse(money);
            double x = (Double.Parse((Jmoney["money"].ToString()))) / 100;
            if (Balance != x)
            {
                Balance = x;
            }
        }

        async void UpdateInventory()
        {
            do
            {

                UpdateBalance();
                List<Item> items = JsonConvert.DeserializeObject<List<Item>>(Request.GetResponseTo("https://market.csgo.com/api/Trades/?key={0}", Apikey));

                //IEnumerable<Item> listedEnum = items.Where( i=> i); unnecessary 
                IEnumerable<Item> decide = items.Where(i => i.ui_status == 2 || i.ui_status == 4);
                foreach (var i in items)
                {
                    if (Items.Any(j => j.i_classid == i.i_classid))
                    {
                        Item updater = Items.Where(j => j.i_classid == i.i_classid).SingleOrDefault();
                        if (updater != null)
                        {
                            Items.Remove(i);
                            Items.Add(updater);
                        }
                    }
                    else
                        Items.Add(i);
                }
                for (int i = 0; i < Items.Count; i++)
                {
                    if (!items.Contains(Items[i]))
                    {
                        Items.Remove(Items[i]);
                    }
                }
                foreach (var item in decide)
                {
                    item.Decide();
                }
                await Task.Delay(TimeSpan.FromSeconds(UpdateInterval));
                //Items.Add(new Item()); delay testing purposes
                //UpdateInventory(); possibru memory leak
            } while (executing);
        }
        private void Load()
        {
            //try
            //{
            //    Items = JsonConvert.DeserializeObject<Inventory>(File.ReadAllText("db.json"));
            //}
            //catch (Exception)
            //{
            //    throw;
            //}
        }
        private void Save()
        {
            //Need history inventory to make use of save-load features
            //string output = JsonConvert.SerializeObject(items);
            //File.WriteAllText(@"db.json", output);
        }
    }
}
