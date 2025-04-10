using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace QA_APPROVAL_REQUEST.Models.DBConn
{
    public class _Excute
    {
        //string user_z = "z_Develop";
        //string pwd_z = "z_Develop";

        //string user_supatta = "s_supatta";
        //string pwd_supatta = "2830";
        string user_z = "z_MoldTracking";
        string pwd_z = "z_MoldTracking";

        public DataTable GetDataTable(string queryString, string databaseName, string serverName)
        {
            //string user_supatta = "s_supatta";
            //string pwd_supatta = "2830";
            string connectionString = "Data Source=" + serverName + "; Initial Catalog=" + databaseName + ";UID=" + user_z + ";PWD=" + pwd_z + ";";
            //string connectionString = "Data Source=" + serverName + "; Initial Catalog=" + databaseName + ";UID=s_supatta;PWD=2830;";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                DataTable dt = new DataTable();
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                dt.Load(reader);

                reader.Close();
                connection.Close();
                return dt;
            }
        }
        public DataTable GetDataTableDataAll(string queryString, string databaseName, string serverName)
        {

            string connectionString = "Data Source=" + serverName + "; Initial Catalog=" + databaseName + ";UID=" + user_z + ";PWD=" + pwd_z + ";";
            //string connectionString = "Data Source=" + serverName + "; Initial Catalog=" + databaseName + ";UID=s_supatta;PWD=2830;";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                DataTable dt = new DataTable();
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                dt.Load(reader);

                reader.Close();
                connection.Close();
                return dt;
            }
        }
        public int InsertData(string queryString, string databaseName, string serverName)
        {
            int eInt = 0;
            string connectionString = "Data Source=" + serverName + "; Initial Catalog=" + databaseName + ";UID=" + user_z + ";PWD=" + pwd_z + ";";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(queryString, conn))
                    {
                        conn.Open();
                        eInt = cmd.ExecuteNonQuery();
                        conn.Close();

                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                eInt = 0;
            }
           
            return eInt;
        }
        public int DeleteData(string queryString, string databaseName, string serverName)
        {
            int eInt = 0;
            string connectionString = "Data Source=" + serverName + "; Initial Catalog=" + databaseName + ";UID=" + user_z + ";PWD=" + pwd_z + ";";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(queryString, conn))
                    {
                        conn.Open();
                        eInt = cmd.ExecuteNonQuery();
                        conn.Close();

                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                eInt = 0;
            }

            return eInt;
        }



    }
}
