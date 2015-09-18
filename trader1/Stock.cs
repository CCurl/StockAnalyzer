using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace trader1
{
	public class Stock
	{
		#region [Initialization]

		public Stock()
		{
            this.Quotes = new List<StockQuote>();
		}

		#endregion // [Initialization]

		#region [Properties]

        /// <summary>
        /// Gets or sets the Company property.
        /// </summary>
        // [DataMember]
        public string Company
        {
            get { return this.company; }
            set
            {
                company = value;
                IsChanged = true;
            }
        }
        private string company;

        /// <summary>
		/// Gets or sets the Symbol property.
		/// </summary>
		// [DataMember]
		public string Symbol
		{ 
			get { return this.symbol; }
			set
			{
				symbol = value;
                IsChanged = true;
            }
		}
		private string symbol;

		/// <summary>
		/// Gets or sets the IsChanged property.
		/// </summary>
		//[DataMember]
		public bool IsChanged 
		{ 
			get { return this.isChanged; }
			set { this.isChanged = value; }
		}
		private bool isChanged;

		/// <summary>
		/// Gets or sets the IsChangedFromUI property.
		/// </summary>
		//[DataMember]
        public bool IsChangedFromUI
        {
            get { return this.isChangedFromUi; }
            set { this.isChangedFromUi = value; }
        }
        private bool isChangedFromUi;

        /// <summary>
        /// Status
        /// </summary>
        public string Status
        {
            get { return this.status; }
            set
            {
                status = value;
                IsChanged = true;
            }
        }
        private string status;

        public string Exchange { get; set; }

        public bool IsValid { get; set; }

        public List<StockQuote> Quotes { get; set; }

        public string ForSQL
        {
            get
            {
                return string.Format("'{0}','{1}', '{2}', '{3}'", this.Symbol, this.Company.Replace("'","''"), this.status, this.Exchange);
            }
        }

		#endregion // [Properties]

		#region [Methods]

        /// <summary>
        /// SaveQuotes.
        /// </summary>
        public void SaveQuotes()
        {
            if (this.Quotes.Count > 0)
            {
                foreach (var sp in this.Quotes)
                {
                    if (sp.IsChanged)
                    {
                        StockDB.SaveQuote(this, sp);
                        sp.IsChanged = false;
                    }
                }
            }
        }

        /// <summary>
        /// SaveQuotes.
        /// </summary>
        public void Save()
        {
            if (this.IsChanged)
            {
                StockDB.SaveStock(this);
                this.IsChanged = false;
            }
        }
        #endregion // [Methods]

		#region [Helpers]

		/// <summary>
		/// Returns an XML representation of the object.
		/// </summary>
		/// <returns>An XML representation of the object.</returns>
		public string ToXML()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("<Stock>");
            //Helpers.XMLHelper("Name", this.Name, sb);
            //Helpers.XMLHelper("Symbol", this.Symbol, sb);
            //Helpers.XMLHelper("LastPrice", this.LastPrice, sb);
            //Helpers.XMLHelper("LastPrice", this.LastPrice, sb);
            //Helpers.XMLHelper("Change", this.Change, sb);
            //Helpers.XMLHelper("ChangePercent", this.ChangePercent, sb);
            //Helpers.XMLHelper("Timestamp", this.Timestamp, sb);
            //Helpers.XMLHelper("MSDate", this.MSDate, sb);
            //Helpers.XMLHelper("MarketCap", this.MarketCap, sb);
            //Helpers.XMLHelper("Volume", this.Volume, sb);
            //Helpers.XMLHelper("ChangeYTD", this.ChangeYTD, sb);
            //Helpers.XMLHelper("ChangePercentYTD", this.ChangePercentYTD, sb);
            //Helpers.XMLHelper("High", this.High, sb);
            //Helpers.XMLHelper("Low", this.Low, sb);
            //Helpers.XMLHelper("Open", this.Open, sb);
            //Helpers.XMLHelper("IsChanged", this.IsChanged, sb);
            //Helpers.XMLHelper("IsChangedFromUI", this.IsChangedFromUI, sb);
			sb.Append("</Stock>");
			return sb.ToString();
		}

		#endregion // [Helpers]

		#region [Source]
		/*********************************************************
		Stocks Stock ccc
		string Name
		string Symbol
		float LastPrice
		float LastPrice
		float Change
		float ChangePercent
		datetime Timestamp
		float MSDate
		double MarketCap
		double Volume
		float ChangeYTD
		float ChangePercentYTD
		float High
		float Low
		float Open
		*********************************************************/
		#endregion // [Source]

        public static string FileName = "Stocks.txt";
        public static string FileName_BAD = "Stocks-BAD.txt";
    }
}
