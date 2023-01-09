using ETMmachine.Services;
using ETMmachine.SQLite_Services;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace ETMmachine
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //It checks if it doesn't exist and insert accounts in the table
            DatabaseManager.CreateDatabase();

            //Create Initial accounts
            //DatabaseManager.Insert("1111111111111111", "Jonas", "Jonaitis", 1234, 1000.0);
            //DatabaseManager.Insert("2222222222222222", "Saulius", "Prusaitis", 2345, 200.0);
            //DatabaseManager.Insert("3333333333333333", "Gezas", "Gezaitis", 3456, 2000.0);
            //DatabaseManager.Insert("4444444444444444", "Kentas", "Kentaitis", 4567, 5000.0);

            //Shows all accounts in DB
            DatabaseManager.Read();

            //Delete method for Danske Table
            //DatabaseManager.Delete("1111111111111111");

            //Init classes
            GUIDcomparator checkGUID = new GUIDcomparator();
            PINchecker checkPIN = new PINchecker();
            ShowBalance printBalance = new ShowBalance();
            MenuActions printMenu = new MenuActions();

            var testCard = new
            {
                GUID = new Guid("00000000-0000-0000-0000-000000000000"),
                IBAN = "1111111111111111",
                Name = "Jonas",
                Surname = "Jonaitis"
            };

            //var testCard = new
            //{
            //    GUID = new Guid("00000000-0000-0000-0000-000000000000"),
            //    IBAN = "2222222222222222",
            //    Name = "Saulius",
            //    Surname = "Prusaitis"
            //};

            //Invalid card GUID
            //var testCard = new
            //{
            //    GUID = new Guid("11111111-1111-1111-1111-111111111111"),
            //    IBAN = "5555555555555555",
            //    Name = "Belekas",
            //    Surname = "Belekavicius"
            //};

            //User placed the Card
            checkGUID.AreGuidsEqual(testCard.GUID);
            checkPIN.PromptForPin(testCard.IBAN);
            printBalance.Print(testCard.IBAN);
            if (printMenu.Print() == 1)
            {
                if(DatabaseManager.ReadHistoryCurrentDay(testCard.IBAN) < 10)
                {
                    printMenu.Withdraw(testCard.IBAN);
                }
                else
                {
                    Console.WriteLine("Max(10) day transactions reached");
                }
            }
            else
            {
                DatabaseManager.PrintHistory(testCard.IBAN);
            }
            Console.WriteLine("Thank you! Exiting...");
            Console.ReadKey();
        }
    }
}
