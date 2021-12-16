using System;
using System.Drawing;
using System.IO;
using System.Data.SQLite;

namespace DbController
{
    class Program
    {
        static string dbName = "URI=file:Images.db";
        static void Main(string[] args)
        {
            CreateTable();
            InsertData();
            ReadData();
        }

        static void CreateTable()
        {
            using (var connection = new SQLiteConnection(dbName))
            {
                connection.Open();
                using (var command = connection.CreateCommand()) //command allow db control
                {
                    command.CommandText = "CREATE TABLE IF NOT EXISTS images (filename VARCHAR (25), width INT, height INT, image BLOB);";
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

        static SQLiteConnection Connect()
        {
            SQLiteConnection connection = new SQLiteConnection(dbName);
            try {
                connection.Open();
            }
            catch (Exception ex) {
                Console.WriteLine(ex);
            }
            return connection;
        }

        static void InsertData()
        {
            using (var connection = new SQLiteConnection(dbName))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    try
                    {
                        Image image = Image.FromFile("C:\\Users\\Marcel\\Downloads\\SwordByLex\\Weapons_0010_Capa-8.png");
                        Byte[] picture = ImageToByteArray(image);
                        //string query = "insert into Table (Photo) values (@pic);"; Deutlich sauberer!
                        command.CommandText = "INSERT INTO items (filename, width, heiht, image) VALUES(@0, @1, @2, @3);";

                        SQLiteParameter param = new SQLiteParameter("@0", System.Data.DbType.String);
                        param.Value = "Sword";
                        command.Parameters.Add(param);

                        param = new SQLiteParameter("@1", System.Data.DbType.Int32);
                        param.Value = image.Width;
                        command.Parameters.Add(param);

                        param = new SQLiteParameter("@2", System.Data.DbType.Int32);
                        param.Value = image.Height;
                        command.Parameters.Add(param);

                        param = new SQLiteParameter("@3", System.Data.DbType.Binary);
                        param.Value = picture;
                        command.Parameters.Add(param);

                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        connection.Close();
                    }
                }
            }
        }
        static byte[] ImageToByteArray(Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, imageIn.RawFormat);
                return ms.ToArray();
            }
        }
        static void ReadData()
        {
            using (var dbConnection = new SQLiteConnection(dbName))
            {
                dbConnection.Open();
                using (var transaction = dbConnection.BeginTransaction())
                {
                    using (var command = new SQLiteCommand(dbConnection))
                    {
                        command.CommandText = "SELECT image FROM items;";
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read()) {
                                using (var ms = new MemoryStream((byte[]) reader["image"]))
                                {
                                    Image image = Image.FromStream(ms);
                                    image.Save("sword.png");
                                }
                            }
                        }
                    }
                    transaction.Commit();
                }
            }
        }

    }
}
