using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.IO;

namespace OPCServerProject
{
    class DBUtil
    {
        private SqlConnection getConnection()
        {
            string dbhost = "";
            string dbname = "";
            string dbuser = "";
            string dbpass = "";

            if (!File.Exists("dbconn.properties"))
            {
                File.Create("dbconn.properties").Close();
            }
            else
            {
                string[] values = File.ReadAllLines("dbconn.properties");
                if (values.Length > 0)
                {
                    dbhost = values[0];
                    dbname = values[1];
                    dbuser = values[2];
                    dbpass = values[3];
                }
            }

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
                try
                {
                    SqlCommand command = conn.CreateCommand();
                    command.CommandText = sql;
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                }
                finally
                {
                    conn.Close();
                }
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
