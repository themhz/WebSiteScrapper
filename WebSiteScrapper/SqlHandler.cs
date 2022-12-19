using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace WebSiteScrapper
{
    public class SqlHandler
    {
        private string connectionString;

        public SqlHandler(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void ExecuteQuery(string query)
        {
            // Create a connection
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Create a command
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Open the connection
                    connection.Open();

                    // Execute the command
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // Process the result
                        while (reader.Read())
                        {
                            Console.WriteLine("{0} {1}", reader[0], reader[1]);
                        }
                    }

                    // Close the connection
                    connection.Close();
                }
            }
        }
    }
}
