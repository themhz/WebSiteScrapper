using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace WebSiteScrapper
{
    public class SqlHandler
    {
        private string connectionString;

        public SqlHandler(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public DataSet ExecuteQuery(string query)
        {
            // Create a connection
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Create a command
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Open the connection
                    connection.Open();

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataSet dataSet = new DataSet();
                    adapter.Fill(dataSet);

                    // Close the connection
                    connection.Close();

                    return dataSet;
                }
            }
        }
    }
}
