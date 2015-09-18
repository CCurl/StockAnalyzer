using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace trader1
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public List<Stock> Stocks { get { return this.stocks; } }
        private List<Stock> stocks;

        public List<Stock> Stocks_BAD { get { return this.stocks_bad; } }
        private List<Stock> stocks_bad;

        public List<Stock> Stocks_NEW { get { return this.stocks_new; } }
        private List<Stock> stocks_new;

        public Stock Current
        {
            get { return currentStock; }
            set { currentStock = value; NotifyPropertyChanged(); }
        }
        private Stock currentStock;

        public string Status 
        {
            get { return status; }
            set { status = value; NotifyPropertyChanged(); }
        }
        private string status;

        public bool Stop { get; set; }
        
        public bool Validating
        {
            get { return validating; }
            set { validating = value; NotifyPropertyChanged(); }
        }
        private bool validating;

        public MainWindowViewModel()
        {
            stocks = new List<Stock>();
            stocks_bad = new List<Stock>();
            stocks_new = new List<Stock>();
            Status = "init";
            validating = true;
        }

        public void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        internal void LoadStocks()
        {
            this.stocks.Clear();
            this.stocks_bad.Clear();
            this.stocks_new.Clear();

            StockDB.LoadStocks(ref this.stocks, "G");
            StockDB.LoadStocks(ref this.stocks_bad, "B");
            StockDB.LoadStocks(ref this.stocks_new, "U");
        }

        internal void SaveStocks()
        {
            foreach (var s in this.Stocks)
            {
                if (s.IsChanged)
                {
                    s.Save();
                }
            }

            foreach (var s in this.Stocks_BAD)
            {
                if (s.IsChanged)
                {
                    if (string.IsNullOrEmpty(s.Company))
                    {
                        s.Company = "";
                    }
                    s.Status = "B";
                    s.Save();
                }
            }
            
            foreach (var s in this.Stocks_NEW)
            {
                if (s.IsChanged)
                {
                    if (string.IsNullOrEmpty(s.Company))
                    {
                        s.Company = "";
                    }
                    s.Status = "U";
                    s.Save();
                }
            }
        }
    }
}
