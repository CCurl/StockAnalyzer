using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Xml;

namespace trader1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindowViewModel dc;
        private System.Windows.Threading.DispatcherTimer validateTimer;
        private System.Windows.Threading.DispatcherTimer quoteTimer;

        public MainWindow()
        {
            dc = new MainWindowViewModel();
            this.DataContext = dc;
            InitializeComponent();
            fiveSeconds = new TimeSpan(0, 0, 0, 5, 0);
            oneSecond = new TimeSpan(0, 0, 0, 1, 0);
            fastTime = new TimeSpan(0, 0, 0, 0, 99);
            validateTimer = new System.Windows.Threading.DispatcherTimer();
            validateTimer.Tick += new EventHandler(ValidateOne);
            validateTimer.Interval = fastTime;
            quoteTimer = new System.Windows.Threading.DispatcherTimer();
            quoteTimer.Tick += new EventHandler(GetOneQuote);
            quoteTimer.Interval = fastTime;
            this.QuoteList = new List<Stock>();
        }

        private void SaveQuotes(object sender, RoutedEventArgs e)
        {
            foreach (var stock in dc.Stocks)
            {
                stock.SaveQuotes();
            }
        }

        private void SaveStocks(object sender, RoutedEventArgs e)
        {
            this.dc.SaveStocks();

            dc.Status = "Saved";
        }

        private bool IsKnownBad(string symbol)
        {
            foreach (var stock in dc.Stocks_BAD)
            {
                if (stock.Symbol == symbol)
                {
                    return true;
                }
            }

            return false;
        }

        private Stock FindStock(string symbol)
        {
            foreach (var stock in dc.Stocks)
            {
                if (stock.Symbol == symbol)
                {
                    return stock;
                }
            }

            return null;
        }

        private void LoadStocks(object sender, RoutedEventArgs e)
        {
            dc.LoadStocks();
            this.dc.Status = string.Format("{0} stocks, {1} bad, {2} new.", dc.Stocks.Count, dc.Stocks_BAD.Count, dc.Stocks_NEW.Count);
        }

        private void GetSymbols_BATS(object sender, RoutedEventArgs e)
        {
            string urlString = "http://www.batstrading.com/market_data/symbol_listing/xml/";
            using (XmlTextReader rdr = new XmlTextReader(urlString))
            {
                while (rdr.Read())
                {
                    if (rdr.NodeType == XmlNodeType.Element)
                    {
                        while (rdr.MoveToNextAttribute())
                        {
                            if (rdr.Name == "name")
                            {
                                string sym = rdr.Value;
                                Stock s = FindStock(sym);
                                if ((s == null) && (!IsKnownBad(sym)))
                                {
                                    s = new Stock() { Symbol = sym, IsChanged = true };
                                    dc.Stocks_NEW.Add(s);
                                }
                            }
                        }
                    }
                }
                rdr.Close();
            }
            this.dc.Status = string.Format("{0} stocks, {1} bad, {2} new.", dc.Stocks.Count, dc.Stocks_BAD.Count, dc.Stocks_NEW.Count);
        }

        private void ValidateStock(ref Stock stock)
        {
            stock.IsValid = false;
            string urlString = string.Format("http://query.yahooapis.com/v1/public/yql?q=select%20%2a%20from%20yahoo.finance.quotes%20where%20symbol%20in%20%28%22{0}%22%29&env=store://datatables.org/alltableswithkeys", stock.Symbol);
            //string urlString = string.Format("http://dev.markitondemand.com/Api/v2/Quote?symbol={0}", stock.Symbol);
            using (XmlTextReader rdr = new XmlTextReader(urlString))
            {
                while (rdr.Read())
                {
                    if ((rdr.NodeType == XmlNodeType.Element) && (rdr.Name == "Name") && (!rdr.IsEmptyElement))
                    {
                        rdr.Read(); // Move to value
                        stock.Company = rdr.Value;
                        rdr.Read(); // Move to end of element
                        stock.IsValid = (string.IsNullOrEmpty(stock.Company) == false);
                    }
                    if ((rdr.NodeType == XmlNodeType.Element) && (rdr.Name == "StockExchange") && (!rdr.IsEmptyElement))
                    {
                        rdr.Read(); // Move to value
                        stock.Exchange = rdr.Value;
                        rdr.Read(); // Move to end of element
                        break;
                    }
                }
                rdr.Close();
            }
        }

        private void ValidateStocks(object sender, RoutedEventArgs e)
        {
            dc.Validating = false;
            validateTimer.Start();
        }

        private TimeSpan fiveSeconds;
        private TimeSpan oneSecond;

        private void ValidateOne(object sender, EventArgs e)
        {
            validateTimer.Stop();
            if (dc.Stocks_NEW.Count > 0)
            {
                Stock stock = dc.Stocks_NEW.First();
                dc.Current = stock;
                try
                {
                    stock.IsChanged = true;
                    ValidateStock(ref stock);
                    if (stock.IsValid)
                    {
                        stock.Status = "G";
                        dc.Stocks.Add(stock);
                    }
                    else
                    {
                        stock.Status = "B";
                        dc.Stocks_BAD.Add(stock);
                    }
                    stock.Save();
                    dc.Stocks_NEW.Remove(stock);
                    validateTimer.Interval = fastTime;
                    this.dc.Status = string.Format("{0} stocks, {1} bad, {2} new.", dc.Stocks.Count, dc.Stocks_BAD.Count, dc.Stocks_NEW.Count);
                }
                catch (Exception ex)
                {
                    validateTimer.Interval = fiveSeconds;
                    this.dc.Status = ex.Message;
                }
                if (!dc.Stop)
                {
                    validateTimer.Start();
                }
                else
                {
                    dc.Validating = true;
                    dc.Status = "Validation stopped.";
                }
            }
            else
            {
                dc.Status = "Validation done.";
                dc.Validating = true;
            }
        }

        private void GetQuotes(object sender, RoutedEventArgs e)
        {
            if (this.dc.Stocks.Count == 0)
            {
                LoadStocks(sender, e);
            }

            dc.Validating = false;
            quoteTimer.Interval = oneSecond;

            if (this.QuoteList.Count == 0)
            {
                foreach (var stock in dc.Stocks)
                {
                    this.QuoteList.Add(stock);
                }
            }
            quoteTimer.Start();
        }

        private List<Stock> QuoteList { get; set; }

        private bool GetQuote(ref StockQuote stockPrice, ref Stock stock)
        {
            //dc.Status = string.Format("Unable to get quote (try #{0}).", currentQuote.NumTries);
            string urlString = string.Format("http://query.yahooapis.com/v1/public/yql?q=select%20%2a%20from%20yahoo.finance.quotes%20where%20symbol%20in%20%28%22{0}%22%29&env=store://datatables.org/alltableswithkeys", stock.Symbol);
            //string urlString = string.Format("http://dev.markitondemand.com/Api/v2/Quote?symbol={0}", stock.Symbol);
            XmlTextReader rdr = new XmlTextReader(urlString);
            while (rdr.Read())
            {
                if ((rdr.NodeType == XmlNodeType.Element) && (rdr.Name == "Status"))
                {
                    rdr.Read(); // Move to value
                    string status = rdr.Value;
                    rdr.Read(); // Move to end of element
                    if (status != "SUCCESS")
                    {
                        break;
                    }
                }

                if (rdr.IsEmptyElement)
                {
                    continue;
                }

                if ((rdr.NodeType == XmlNodeType.Element) && (rdr.Name == "Name"))
                {
                    rdr.Read(); // Move to value
                    stock.Company = rdr.Value.Replace(",", "");
                    rdr.Read(); // Move to end of element
                    continue;
                }

                if ((rdr.NodeType == XmlNodeType.Element) && (rdr.Name == "LastPrice"))
                {
                    rdr.Read(); // Move to value
                    stockPrice.LastPrice = float.Parse(rdr.Value);
                    rdr.Read(); // Move to end of element
                    continue;
                }

                if ((rdr.NodeType == XmlNodeType.Element) && (rdr.Name == "LastTradePriceOnly"))
                {
                    rdr.Read(); // Move to value
                    stockPrice.LastPrice = float.Parse(rdr.Value);
                    rdr.Read(); // Move to end of element
                    continue;
                }

                if ((rdr.NodeType == XmlNodeType.Element) && (rdr.Name == "Change"))
                {
                    rdr.Read(); // Move to value
                    stockPrice.Change = float.Parse(rdr.Value);
                    rdr.Read(); // Move to end of element
                    continue;
                }

                if ((rdr.NodeType == XmlNodeType.Element) && (rdr.Name == "ChangePercent"))
                {
                    rdr.Read(); // Move to value
                    stockPrice.ChangePercent = float.Parse(rdr.Value);
                    rdr.Read(); // Move to end of element
                    continue;
                }

                if ((rdr.NodeType == XmlNodeType.Element) && (rdr.Name == "LastTradeDate"))
                {
                    rdr.Read(); // Move to value
                    stockPrice.Timestamp = rdr.Value;
                    rdr.Read(); // Move to end of element
                    continue;
                }

                if ((rdr.NodeType == XmlNodeType.Element) && (rdr.Name == "MSDate"))
                {
                    rdr.Read(); // Move to value
                    stockPrice.MSDate = float.Parse(rdr.Value);
                    rdr.Read(); // Move to end of element
                    continue;
                }

                if ((rdr.NodeType == XmlNodeType.Element) && (rdr.Name == "MarketCapitalization") && (rdr.IsEmptyElement == false))
                {
                    rdr.Read(); // Move to value
                    string val = rdr.Value;
                    rdr.Read(); // Move to end of element
                    if (val.EndsWith("B"))
                    {
                        // Billion
                        val = val.Substring(0, val.Length - 1);
                        stockPrice.MarketCap = double.Parse(val);
                        stockPrice.MarketCap *= (1024 * 1024 * 1024);
                    }
                    else if (val.EndsWith("M"))
                    {
                        // Million
                        val = val.Substring(0, val.Length - 1);
                        stockPrice.MarketCap = double.Parse(val);
                        stockPrice.MarketCap *= (1024 * 1024);
                    }
                    else
                    {
                        double mc = 0;
                        double.TryParse(val, out mc);
                        stockPrice.MarketCap = mc;
                    }
                    continue;
                }

                if ((rdr.NodeType == XmlNodeType.Element) && (rdr.Name == "Volume"))
                {
                    rdr.Read(); // Move to value
                    stockPrice.Volume = double.Parse(rdr.Value);
                    rdr.Read(); // Move to end of element
                    continue;
                }

                if ((rdr.NodeType == XmlNodeType.Element) && (rdr.Name == "ChangeYTD"))
                {
                    rdr.Read(); // Move to value
                    stockPrice.ChangeYTD = float.Parse(rdr.Value);
                    rdr.Read(); // Move to end of element
                    continue;
                }

                if ((rdr.NodeType == XmlNodeType.Element) && (rdr.Name == "ChangePercentYTD"))
                {
                    rdr.Read(); // Move to value
                    stockPrice.ChangePercentYTD = float.Parse(rdr.Value);
                    rdr.Read(); // Move to end of element
                    continue;
                }

                if ((rdr.NodeType == XmlNodeType.Element) && (rdr.Name == "DaysHigh"))
                {
                    rdr.Read(); // Move to value
                    stockPrice.High = float.Parse(rdr.Value);
                    rdr.Read(); // Move to end of element
                    continue;
                }

                if ((rdr.NodeType == XmlNodeType.Element) && (rdr.Name == "DaysLow"))
                {
                    rdr.Read(); // Move to value
                    stockPrice.Low = float.Parse(rdr.Value);
                    rdr.Read(); // Move to end of element
                    continue;
                }

                if ((rdr.NodeType == XmlNodeType.Element) && (rdr.Name == "PreviousClose"))
                {
                    rdr.Read(); // Move to value
                    stockPrice.Open = float.Parse(rdr.Value);
                    rdr.Read(); // Move to end of element
                    continue;
                }
            }

            return ((stockPrice.LastPrice != 0) && (stockPrice.Timestamp != null));
        }

        private StockQuote currentQuote;
        private TimeSpan fastTime;

        private void GetOneQuote(object sender, EventArgs e)
        {
            this.quoteTimer.Stop();
            if (this.QuoteList.Count > 0)
            {
                Stock s = this.QuoteList.First();
                dc.Current = s;
                if (currentQuote == null)
                {
                    currentQuote = new StockQuote() { IsChanged = true, NumTries = 0 };
                }
                bool isOK = false;
                try
                {
                    isOK = GetQuote(ref currentQuote, ref s);
                }
                catch (Exception ex)
                {
                    dc.Status = ex.Message;
                }

                if (isOK)
                {
                    s.Quotes.Add(currentQuote);
                    s.SaveQuotes();
                    this.QuoteList.Remove(s);
                    dc.Status = currentQuote.ForSQL;
                    currentQuote = null;
                    this.quoteTimer.Interval = fastTime;
                }
                else
                {
                    currentQuote.NumTries++;
                    if (currentQuote.NumTries <= 3)
                    {
                        this.quoteTimer.Interval = oneSecond;
                        dc.Status = string.Format("Unable to get quote (try #{0}).", currentQuote.NumTries);
                    }
                    else
                    {
                        this.QuoteList.Remove(s);
                        this.quoteTimer.Interval = fastTime;
                        dc.Status = string.Format("Unable to get quote (skipping).");
                        dc.Stocks.Remove(s);
                        s.Status = "U";
                        s.Company = "";
                        dc.Stocks_NEW.Add(s);
                        s.Save();
                    }
                }
                if (!dc.Stop)
                {
                    quoteTimer.Start();
                }
                else
                {
                    dc.Validating = true;
                    this.dc.Status = "Stopped.";
                }
            }
            else
            {
                dc.Validating = true;
                dc.Status = "Done.";
            }
        }
    }
}
