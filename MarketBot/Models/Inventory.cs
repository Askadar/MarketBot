using MarketBot.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MarketBot.Models
{
    // WARNING [Using some random async sclass found on internet] WARNING
    // check it later
    // http://www.thomaslevesque.com/2009/04/17/wpf-binding-to-an-asynchronous-collection/

    public class Inventory : BindingList<Item>
    {
        public Inventory()
        {
        }
    }
}
