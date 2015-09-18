using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace trader1
{
    public class StockQuote
    {
        /// <summary>
        /// Gets or sets the LastPrice property.
        /// </summary>
        // [DataMember]
        public float LastPrice
        {
            get { return this.lastPrice; }
            set
            {
                lastPrice = value;
            }
        }
        private float lastPrice;

        /// <summary>
        /// Gets or sets the Change property.
        /// </summary>
        // [DataMember]
        public float Change
        {
            get { return this.change; }
            set
            {
                change = value;
            }
        }
        private float change;

        /// <summary>
        /// Gets or sets the ChangePercent property.
        /// </summary>
        // [DataMember]
        public float ChangePercent
        {
            get { return this.changePercent; }
            set
            {
                changePercent = value;
            }
        }
        private float changePercent;

        /// <summary>
        /// Gets or sets the Timestamp property.
        /// </summary>
        // [DataMember]
        public string Timestamp
        {
            get { return this.timestamp; }
            set
            {
                timestamp = value;
            }
        }
        private string timestamp;

        /// <summary>
        /// Gets or sets the MSDate property.
        /// </summary>
        // [DataMember]
        public float MSDate
        {
            get { return this.msdate; }
            set
            {
                msdate = value;
            }
        }
        private float msdate;

        /// <summary>
        /// Gets or sets the MarketCap property.
        /// </summary>
        // [DataMember]
        public double MarketCap
        {
            get { return this.marketCap; }
            set
            {
                marketCap = value;
            }
        }
        private double marketCap;

        /// <summary>
        /// Gets or sets the Volume property.
        /// </summary>
        // [DataMember]
        public double Volume
        {
            get { return this.volume; }
            set
            {
                volume = value;
            }
        }
        private double volume;

        /// <summary>
        /// Gets or sets the ChangeYTD property.
        /// </summary>
        // [DataMember]
        public float ChangeYTD
        {
            get { return this.changeYtd; }
            set
            {
                changeYtd = value;
            }
        }
        private float changeYtd;

        /// <summary>
        /// Gets or sets the ChangePercentYTD property.
        /// </summary>
        // [DataMember]
        public float ChangePercentYTD
        {
            get { return this.changePercentYtd; }
            set
            {
                changePercentYtd = value;
            }
        }
        private float changePercentYtd;

        /// <summary>
        /// Gets or sets the High property.
        /// </summary>
        // [DataMember]
        public float High
        {
            get { return this.high; }
            set
            {
                high = value;
            }
        }
        private float high;

        /// <summary>
        /// Gets or sets the Low property.
        /// </summary>
        // [DataMember]
        public float Low
        {
            get { return this.low; }
            set
            {
                low = value;
            }
        }
        private float low;

        /// <summary>
        /// Gets or sets the Open property.
        /// </summary>
        // [DataMember]
        public float Open
        {
            get { return this.open; }
            set
            {
                open = value;
            }
        }
        private float open;

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

        public string ForSQL
        {
            get
            {
                DateTime dt = DateTime.Now;
                //DateTime.TryParse(this.Timestamp, out dt);
                string dtStr = dt.ToString("yyyy-MM-dd");
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("'{0}'", dtStr);
                sb.AppendFormat(",{0}", this.Open);
                sb.AppendFormat(",{0}", this.LastPrice);
                sb.AppendFormat(",{0}", this.High);
                sb.AppendFormat(",{0}", this.Low);
                sb.AppendFormat(",{0}", this.Volume);
                sb.AppendFormat(",{0}", this.MarketCap);
                //sb.AppendFormat(",{0}", this.Change);
                //sb.AppendFormat(",{0}", this.ChangeYTD);
                //sb.AppendFormat(",{0}", this.ChangePercent);
                //sb.AppendFormat(",{0}", this.ChangePercentYTD);
                //sb.AppendFormat(",{0}", this.MSDate);
                return sb.ToString();
            }
        }



        public int NumTries { get; set; }
    }
}
