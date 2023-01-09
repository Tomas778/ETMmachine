using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ETMmachine.SQLite_Services
{
    public class DatabaseManager
    {
        //Setup for DB

        const string dbfile = "URI=file:MySqliteDB.db"; //Debug folder
        const string dbfilestring = "MySqliteDB.db"; //Predifined DB name
        const string AccountsTblString = "CREATE TABLE Danske(Id integer primary key autoincrement, " +
                                         "IBAN TEXT, " +
                                         "Name TEXT, " +
                                         "Surname TEXT, " +
                                         "Pin INTEGER, " +
                                         "Balance REAL)";

        const string RecordsTblString = "CREATE TABLE Records(Id integer primary key autoincrement, " +
                                        "Date NUMERIC, " +
                                        "IBAN TEXT, " +
                                        "Change REAL, " +
                                        "Balance REAL)";
        public static void CreateDatabase()
        {
            // Check if the database file already exists in the debug folder
            if (!System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), dbfilestring)))
            {
                // Create a new database
                SQLiteConnection.CreateFile(dbfilestring);

                // Connect to the database
                using (SQLiteConnection conn = new SQLiteConnection(dbfile))
                {

                    try
                    {
                        conn.Open();
                    }
                    catch (SQLiteException ex)
                    {
                        Console.WriteLine("Error opening connection to the database: " + ex.Message);
                        return;
                    }

                    // Create the BankAccounts table
                    string sql = AccountsTblString;
                    using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    // Create the Records table
                    sql = RecordsTblString;
                    using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        // Method to insert a new record into the BankAccounts table
        public static void Insert(string iban, string name, string surname, int pin, double balance)
        {
            using (SQLiteConnection conn = new SQLiteConnection(dbfile))
            {
                try
                {
                    conn.Open();
                }
                catch (SQLiteException ex)
                {
                    Console.WriteLine("Error opening connection to the database: " + ex.Message);
                    return;
                }

                string sql = "INSERT INTO Danske (IBAN, Name, Surname, Pin, Balance) VALUES (@IBAN, @Name, @Surname, @Pin, @Balance)";
                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@IBAN", iban);
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Surname", surname);
                    cmd.Parameters.AddWithValue("@Pin", pin);
                    cmd.Parameters.AddWithValue("@Balance", balance);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public static void InsertRecord(DateTime date, string iban, double change)
        {
            using (SQLiteConnection conn = new SQLiteConnection(dbfile))
            {
                try
                {
                    conn.Open();
                }
                catch (SQLiteException ex)
                {
                    Console.WriteLine("Error opening connection to the database: " + ex.Message);
                    return;
                }

                string sql = "INSERT INTO Records (Date, iban, Change, Balance) VALUES (@Date, @IBAN, @Change, @Balance)";
                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Date", date);
                    cmd.Parameters.AddWithValue("@IBAN", iban);
                    cmd.Parameters.AddWithValue("@Change", -change);
                    cmd.Parameters.AddWithValue("@Balance", ReadBalance(iban));
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Method to read all records from the BankAccounts table
        public static void Read()
        {
            using (SQLiteConnection conn = new SQLiteConnection(dbfile))
            {
                try
                {
                    conn.Open();
                }
                catch (SQLiteException ex)
                {
                    Console.WriteLine("Error opening connection to the database: " + ex.Message);
                    return;
                }

                SQLiteDataReader sqliteReader;
                SQLiteCommand sqliteCommand;
                sqliteCommand = conn.CreateCommand();
                sqliteCommand.CommandText = "SELECT * FROM Danske";
                sqliteReader = sqliteCommand.ExecuteReader();
                Console.WriteLine($"{sqliteReader.GetName(0)} {sqliteReader.GetName(1), -16}  {sqliteReader.GetName(2), -10}{sqliteReader.GetName(3), -12}{sqliteReader.GetName(4), -5}");
                while (sqliteReader.Read())
                {
                    int space1 = 10 - sqliteReader["Name"].ToString().Length;
                    int space2 = 10 - sqliteReader["Surname"].ToString().Length;
                    Console.WriteLine($"{sqliteReader.GetInt32(0)}  {sqliteReader["IBAN"]}  {sqliteReader["Name"], -10}{sqliteReader["Surname"], -12}{sqliteReader["Balance"]}");
                }
                Console.WriteLine();
            }
        }

        // Method to read PIN from the BankAccounts table
        public static int ReadPin(string iban)
        {
            int pin = -1;
            using (SQLiteConnection conn = new SQLiteConnection(dbfile))
            {
                try
                {
                    conn.Open();
                }
                catch (SQLiteException ex)
                {
                    Console.WriteLine("Error opening connection to the database: " + ex.Message);
                    return -1;
                }

                using (SQLiteCommand cmd = new SQLiteCommand("SELECT Pin FROM Danske WHERE IBAN = @IBAN", conn))
                {
                    cmd.Parameters.AddWithValue("@IBAN", iban);

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            pin = reader.GetInt32(0);
                        }
                    }
                }
                return pin;
            }
        }
        // Method to read Balance from the BankAccounts table
        public static decimal ReadBalance(string iban)
        {
            decimal balance = -1;
            using (SQLiteConnection conn = new SQLiteConnection(dbfile))
            {
                try
                {
                    conn.Open();
                }
                catch (SQLiteException ex)
                {
                    Console.WriteLine("Error opening connection to the database: " + ex.Message);
                    return -1;
                }

                using (SQLiteCommand cmd = new SQLiteCommand("SELECT Balance FROM Danske WHERE IBAN = @IBAN", conn))
                {
                    cmd.Parameters.AddWithValue("@IBAN", iban);

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            balance = reader.GetDecimal(0);
                        }
                    }
                }
                return balance;
            }
        }

        // Method to read History from the Records table last 5 records
        public static void PrintHistory(string iban)
        {

            using (SQLiteConnection conn = new SQLiteConnection(dbfile))
            {
                try
                {
                    conn.Open();
                }
                catch (SQLiteException ex)
                {
                    Console.WriteLine("Error opening connection to the database: " + ex.Message);
                }

                using (SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM Records WHERE IBAN = @IBAN ORDER BY Date DESC LIMIT 5", conn))
                {
                    cmd.Parameters.AddWithValue("@IBAN", iban);

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        Console.Clear();
                        Console.WriteLine($"{reader.GetName(1), -21}{reader.GetName(2), -18}{reader.GetName(3), -10}{reader.GetName(4)}");
                        while (reader.Read())
                        {
                             Console.WriteLine($"{reader.GetDateTime(1).ToString("yyyy-MM-dd HH:mm:ss")}  {reader["IBAN"]}  {reader["Change"], -10}{reader["Balance"]}");
                        }
                    }
                }
            }
        }

        // Method to count History from the Records table for current day and specific IBAN
        public static int ReadHistoryCurrentDay(string iban)
        {
            using (SQLiteConnection conn = new SQLiteConnection(dbfile))
            {
                try
                {
                    conn.Open();
                }
                catch (SQLiteException ex)
                {
                    Console.WriteLine("Error opening connection to the database: " + ex.Message);
                }

                using (SQLiteCommand cmd = new SQLiteCommand($"SELECT COUNT(*) FROM Records WHERE IBAN = {iban} AND Date >= DATE('now') AND Date < DATE('now', '+1 day')", conn))
                {
                    //cmd.Parameters.AddWithValue("@IBAN", iban);

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {

                        int i = 0;
                        while (reader.Read())
                        {
                             i = reader.GetInt32(0);
                        }
                        return i;
                    }
                }
            }
        }

        // Method to update the Balance column of a specific record in the BankAccounts table based on the IBAN column
        public static void Update(string iban, decimal change)
        {
            decimal newAmount = ReadBalance(iban) - change; 

            using (SQLiteConnection conn = new SQLiteConnection(dbfile))
            {
                try
                {
                    conn.Open();
                }
                catch (SQLiteException ex)
                {
                    Console.WriteLine("Error opening connection to the database: " + ex.Message);
                    return;
                }

                string sql = "UPDATE Danske SET Balance = @Balance WHERE IBAN = @IBAN";
                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Balance", newAmount);
                    cmd.Parameters.AddWithValue("@IBAN", iban);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Method to delete a specific record from the BankAccounts table based on the IBAN column
        public static void Delete(string iban)
        {
            using (SQLiteConnection conn = new SQLiteConnection(dbfile))
            {
                try
                {
                    conn.Open();
                }
                catch (SQLiteException ex)
                {
                    Console.WriteLine("Error opening connection to the database: " + ex.Message);
                    return;
                }

                string sql = "DELETE FROM Danske WHERE IBAN = @IBAN";
                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@IBAN", iban);

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
