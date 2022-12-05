using System;
using System.Data.SqlClient;

namespace CsharpStruct
{
    public class Something : IEquatable<Something>
    {
        private string a = "123";
        private string b = "456";

        public bool Equals(Something other)
        {
            return other != null && a == other.a;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            //Data Source=localhost\\SQLEXPRESS;Initial Catalog=Introduction_Database;User ID=sa;Password=1234

            //string connectionString = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=Introduction_Database;User ID=sa;Password=1234";

            try
            {

                string category = PrintCategories();

                PrintProductsByCategory(category);

                InsertProduct("iPhone 8", "Not needed at the moment", (decimal)699.99, "Electronic Devices");

                UpdateProduct(51, "iPhone 8 Changed", "Changed description...", (decimal)199.99, "Electronic Devices");

                DeleteProducts(52);

            } catch(Exception ex)
            {
                Console.WriteLine($"{ex}");
            }

            Console.ReadLine();
        }

        static void InsertProduct(string productName, string productDescription, decimal productPrice, string productCategory)
        {
            string connectionString = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=Introduction_Database;User ID=sa;Password=1234";

            try
            {

                SqlConnection sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();

                /*
                 * INSERT INTO Products(ProductName, ProductDescription, ProductPrice, ProductCategory) VALUES ('iPhone 13', 'A mobile phone manufacture by Apple', 999.99, 'Electronic Devices');
                 */

                string sqlQuery = $"INSERT INTO Products (ProductName, ProductDescription, ProductPrice, ProductCategory) VALUES ('{productName}', '{productDescription}', {productPrice}, '{productCategory}');";

                SqlCommand command = new SqlCommand(sqlQuery, sqlConnection);

                int result = command.ExecuteNonQuery();
                Console.WriteLine($"Insert completed, number of rows changed: {result}");

                sqlConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex}");
            }
        }


        static void UpdateProduct(int id, string productName, string productDescription, decimal productPrice, string productCategory)
        {
            string connectionString = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=Introduction_Database;User ID=sa;Password=1234";

            try
            {

                SqlConnection sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();

                /*
                 * INSERT INTO Products(ProductName, ProductDescription, ProductPrice, ProductCategory) VALUES ('iPhone 13', 'A mobile phone manufacture by Apple', 999.99, 'Electronic Devices');
                 */

                string sqlQuery = $"UPDATE Products SET ProductName = '{productName}', ProductDescription = '{productDescription}', ProductPrice = {productPrice}, ProductCategory = '{productCategory}' WHERE Id = {id};";

                SqlCommand command = new SqlCommand(sqlQuery, sqlConnection);

                int result = command.ExecuteNonQuery();
                Console.WriteLine($"Update completed, number of rows changed: {result}");

                sqlConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex}");
            }
        }


        static void DeleteProducts(int startingId)
        {
            string connectionString = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=Introduction_Database;User ID=sa;Password=1234";

            try
            {

                SqlConnection sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();

                /* 
                 * For example
                 * DELETE FROM Products WHERE Id >= 7;
                 */

                string sqlQuery = $"DELETE FROM Products WHERE Id >= {startingId};";

                SqlCommand command = new SqlCommand(sqlQuery, sqlConnection);

                int result = command.ExecuteNonQuery();
                Console.WriteLine($"Delete completed, number of rows changed: {result}");

                sqlConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex}");
            }
        }

        static string PrintCategories()
        {
            string connectionString = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=Introduction_Database;User ID=sa;Password=1234";

            try
            {

                Console.WriteLine("Available Categories: ");
                SqlConnection sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();

                string sqlQuery = "SELECT DISTINCT ProductCategory FROM Products;";

                SqlCommand command = new SqlCommand(sqlQuery, sqlConnection);

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    // Id
                    // Another example of reading the data by specifying the column name,
                    // and we need to make sure that we didn't receive any null values from the reader.
                    // In case we did, we need to skip reading of that particular entry, and use some default value (for example, empty string, or whatever we want depeding on the data type we work with).
                    //int id = Convert.ToInt32(reader["ID"]);

                    if(reader["ProductCategory"] != DBNull.Value)
                    {
                        string category = Convert.ToString(reader["ProductCategory"]);

                        Console.WriteLine($"{category}");
                    }
                }

                sqlConnection.Close();

                Console.WriteLine("Please enter the category you want to see, or just press enter to list everything:");

                return Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex}");
                return "";
            }
        }

        static void PrintProductsByCategory(string category)
        {
            string connectionString = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=Introduction_Database;User ID=sa;Password=1234";
            try
            {

                Console.WriteLine("Products: ");
                SqlConnection sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();

                string sqlQuery = "SELECT * FROM Products";

                if(!string.IsNullOrEmpty(category)) {
                    sqlQuery += $" WHERE ProductCategory = '{category}';";
                } else { 
                    sqlQuery += ";";
                }

                SqlCommand command = new SqlCommand(sqlQuery, sqlConnection);

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    // Id
                    //int id = Convert.ToInt32(reader["ID"]);
                    int id = reader.GetInt32(0);
                    string name = reader.GetString(1);
                    string description = reader.GetString(2);
                    decimal price = reader.GetDecimal(3);
                    string prodCategory = reader.GetString(4);

                    Console.WriteLine($"Id: {id}, ProductName: {name}, ProductDescription: {description}, ProductPrice: {price}, ProductCategory: {prodCategory}");
                }

                sqlConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex}");
            }
        }
    }
}