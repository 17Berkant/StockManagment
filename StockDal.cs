using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StockManagment
{
    public class StockDal
    {
        SqlConnection _connection = new SqlConnection("Server=localhost\\SQLEXPRESS; Database=Stock;Integrated Security=True;Encrypt=true;TrustServerCertificate=True");

        public void CheckAndCreateDatabaseAndTable()
        {
            try
            {
                _connection.Open();

                // Veritabanını kontrol et
                string checkDbQuery = "SELECT COUNT(*) FROM sys.databases WHERE name = 'Stock'";
                using (SqlCommand checkDbCommand = new SqlCommand(checkDbQuery, _connection))
                {
                    int dbCount = (int)checkDbCommand.ExecuteScalar();

                    // Eğer veritabanı yoksa oluştur
                    if (dbCount == 0)
                    {
                        string createDbQuery = "CREATE DATABASE Stock";
                        using (SqlCommand createDbCommand = new SqlCommand(createDbQuery, _connection))
                        {
                            createDbCommand.ExecuteNonQuery();
                        }
                    }
                }

                // Veritabanını kullan
                string useDbQuery = "USE Stock";
                using (SqlCommand useDbCommand = new SqlCommand(useDbQuery, _connection))
                {
                    useDbCommand.ExecuteNonQuery();
                }

                // Tabloyu kontrol et
                string checkTableQuery = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'StockTable'";
                using (SqlCommand checkTableCommand = new SqlCommand(checkTableQuery, _connection))
                {
                    int tableCount = (int)checkTableCommand.ExecuteScalar();

                    // Eğer tablo yoksa oluştur
                    if (tableCount == 0)
                    {
                        string createTableQuery = "CREATE TABLE StockTable (Id INT PRIMARY KEY, ProductName NVARCHAR(255), StockQuantity INT, Categorie NVARCHAR(255))";
                        using (SqlCommand createTableCommand = new SqlCommand(createTableQuery, _connection))
                        {
                            createTableCommand.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
            finally
            {
                if (_connection.State == ConnectionState.Open)
                {
                    _connection.Close();
                }
            }
        }
        public List<Stock> GetAll()
        {
            ConnectionControl();
            SqlCommand cmd = new SqlCommand("Select * from StockTable", _connection);
            SqlDataReader reader = cmd.ExecuteReader();
            List<Stock> stocks = new List<Stock>();

            while (reader.Read())
            {
                Stock stock = new Stock
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    ProductName = reader["ProductName"].ToString(),
                    Categorie = reader["Categorie"].ToString(),
                    StockQuantity = Convert.ToInt32(reader["StockQuantity"])
                };
                stocks.Add(stock);

            }

            reader.Close();
            _connection.Close();
            return stocks;
        }


        private void ConnectionControl()
        {
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
        }

        public void Add(Stock stock)
        {
            ConnectionControl();
            SqlCommand sqlCommand = new SqlCommand("Insert into StockTable values(@productName,@stockQuantity,@categorie)", _connection);
            sqlCommand.Parameters.AddWithValue("@productName", stock.ProductName);
            sqlCommand.Parameters.AddWithValue("@stockQuantity", stock.StockQuantity);
            sqlCommand.Parameters.AddWithValue("@categorie", stock.Categorie);

            sqlCommand.ExecuteNonQuery();

            _connection.Close();
        }
        public void Update(Stock stock)
        {
            ConnectionControl();
            SqlCommand sqlCommand = new SqlCommand("Update StockTable set ProductName=@productName,StockQuantity=@stockQuantity,Categorie=@categorie where Id=@id", _connection);
            sqlCommand.Parameters.AddWithValue("@productName", stock.ProductName);
            sqlCommand.Parameters.AddWithValue("@stockQuantity", stock.StockQuantity);
            sqlCommand.Parameters.AddWithValue("@categorie", stock.Categorie);
            sqlCommand.Parameters.AddWithValue("@id", stock.Id);

            sqlCommand.ExecuteNonQuery();

            _connection.Close();
        }


        public void Delete(int id)
        {
            ConnectionControl();
            SqlCommand sqlCommand = new SqlCommand("Delete from StockTable where Id=@id", _connection);

            sqlCommand.Parameters.AddWithValue("@id", id);

            sqlCommand.ExecuteNonQuery();

            _connection.Close();
        }
    }

}
