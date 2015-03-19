using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

namespace OPCServerProject
{
    class DBUtil
    {
        private SqlConnection getConnection()
        {
            Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            string dbhost = cfa.AppSettings.Settings["dbhost"].Value;
            string dbname = cfa.AppSettings.Settings["dbname"].Value;
            string dbuser = cfa.AppSettings.Settings["dbuser"].Value;
            string dbpass = cfa.AppSettings.Settings["dbpass"].Value;
            string connStr = "Data Source=" + dbhost + ";Initial Catalog=" + dbname + ";User ID=" + dbuser + ";Password=" + dbpass;
            SqlConnection conn = new SqlConnection(connStr);
            return conn;
        }

        public bool testConnection()
        {
            bool result = true;
            SqlConnection conn = getConnection();
            try
            {
                conn.Open();
                conn.Close();
            }
            catch (Exception ex)
            {
                result = false;
            }

            return result;
        }

        public void executeNonQuerySQL(string sql)
        {
            SqlConnection conn = getConnection();
            lock (conn)
            {
                conn.Open();
                SqlCommand command = conn.CreateCommand();
                command.CommandText = sql;
                command.ExecuteNonQuery();
                conn.Close();
            }
        }

        public DataSet executeQuery(string queryStr)
        {
            SqlConnection conn = getConnection();
            DataSet result = new DataSet();
            lock (conn)
            {
                conn.Open();
                SqlCommand command = conn.CreateCommand();
                command.CommandText = queryStr;
                SqlDataAdapter ada = new SqlDataAdapter(command);
                ada.Fill(result);
                conn.Close();
            }
            return result;
        }
    }
}
