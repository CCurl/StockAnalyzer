using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace trader1
{
    class StockDB
    {
        private static SqlConnection sqlConnection;

        private static void EnsureConnection()
        {
            if (sqlConnection == null)
            {
                string connStr = ConfigurationManager.ConnectionStrings["StocksDB"].ConnectionString;
                sqlConnection = new SqlConnection(connStr);
                try
                {
                    sqlConnection.Open();
                }
                catch (Exception ex)
                {
                    string msg = ex.Message;
                }
            }
        }

        internal static void LoadStocks(ref List<Stock> stocks, string status)
        {
            EnsureConnection();
            string sql = string.Format("SELECT Symbol, Company FROM Stocks WHERE Status = '{0}'", status);
            SqlDataAdapter adapter = new SqlDataAdapter();
            using (SqlCommand command = new SqlCommand(sql, sqlConnection))
            {
                using (var dr = command.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        Stock s = new Stock();
                        s.Symbol = dr.GetString(dr.GetOrdinal("Symbol"));
                        s.Company = dr.GetString(dr.GetOrdinal("Company"));
                        s.Status = status;
                        s.IsChanged = false;
                        stocks.Add(s);
                    }
                    dr.Close();
                }
            }
        }

        internal static void SaveStock(Stock stock)
        {
            EnsureConnection();
            string sql = string.Format("EXECUTE UpdateStock {0}", stock.ForSQL);
            using (SqlCommand command = new SqlCommand(sql, sqlConnection))
            {
                command.CommandType = System.Data.CommandType.Text;
                command.ExecuteNonQuery();
            }
        }

        internal static void SaveQuote(Stock stock, StockQuote quote)
        {
            EnsureConnection();
            string sql = string.Format("EXECUTE UpdateQuote '{0}', {1}", stock.Symbol, quote.ForSQL);
            using (SqlCommand command = new SqlCommand(sql, sqlConnection))
            {
                command.CommandType = System.Data.CommandType.Text;
                command.ExecuteNonQuery();
            }
        }
    }
}
