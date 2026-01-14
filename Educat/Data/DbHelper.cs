using Educat.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;


namespace Educat.Data
{
    public static class DbHelper
    {
        public static List<Good> GetGoods()
        {
            try
            {
                List<Good> list = new List<Good>();

                using (SqlConnection conn = Db.GetConnection())
                {
                    string sql = @"SELECT g.IdGood, g.Article, g.IdLabel, 
                    l.Label, g.UnitOfMeasure, g.Price, g.IdSupplier, s.Supplier, 
                    g.IdFabric, f.Fabric, g.IdCategory, c.Category, g.Discount, 
                    g.Count, g.Description, g.Photo 
                    FROM Goods g 
                    JOIN Labels l ON l.IdLabel = g.IdLabel
                    JOIN Suppliers s ON s.IdSupplier = g.IdSupplier 
                    JOIN Fabrics f ON f.IdFabric = g.IdFabric 
                    JOIN Categories c ON c.IdCategory = g.IdCategory;";

                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Good
                            {
                                Id = reader.GetInt32(0),
                                Article = reader.GetString(1),
                                IdLabel = reader.GetInt32(2),
                                Label = reader.GetString(3),
                                UnitOfMeasure = reader.GetString(4),
                                Price = reader.GetDouble(5),
                                IdSupplier = reader.GetInt32(6),
                                Supplier = reader.GetString(7),
                                IdFabric = reader.GetInt32(8),
                                Fabric = reader.GetString(9),
                                IdCategory = reader.GetInt32(10),
                                Category = reader.GetString(11),
                                Discount = reader.GetInt32(12),
                                Count = reader.GetInt32(13),
                                Description = reader.GetString(14),
                                Photo = reader.IsDBNull(15) ? null : reader.GetString(15),
                            });
                        }
                        return list;
                    }
                }
            }
            catch
            {
                MessageBox.Show($"Ошибка загрузки списка товаров!", "Ошибка работы с БД",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            return null;
        }

        public static User Authorize(string username, string password)
        {
            try
            {
                using (SqlConnection conn = Db.GetConnection())
                {
                    string sql = @"SELECT u.IdRole, r.Role, u.FullName FROM Users u 
                    JOIN Roles r ON r.IdRole = u.IdRole
                    WHERE u.Login = @username AND u.Password = @password;";
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            { 
                                IdRole = reader.GetInt32(0),
                                Role = reader.GetString(1),
                                FullName = reader.GetString(2)
                            };
                        }
                    }
                }
            } catch {
                MessageBox.Show("Введен неверный логин или пароль!", "Ошибка авторизации", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            return null;
        }

        public static List<ComboBoxItems> GetComboBoxItems(string tableName)
        {
            try
            {
                List<ComboBoxItems> list = new List<ComboBoxItems>();

                using (SqlConnection conn = Db.GetConnection())
                {
                    string sql = "";

                    if (tableName == "Categories")
                    {
                        sql = @"SELECT IdCategory, Category FROM Categories;";
                    }
                    else if (tableName == "Fabrics")
                    {
                        sql = @"SELECT IdFabric, Fabric FROM Fabrics;";
                    }
                    else if (tableName == "Suppliers")
                    {
                        sql = @"SELECT IdSupplier, Supplier FROM Suppliers;";
                    }
                    else if (tableName == "Labels")
                    {
                        sql = @"SELECT IdLabel, Label FROM Labels;";
                    }
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new ComboBoxItems
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                            });
                        }
                        return list;
                    }
                }
            }
            catch
            {
                MessageBox.Show($"Ошибка загрузки списка из {tableName}!", "Ошибка работы с БД",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            return null;
        }
    }
}
