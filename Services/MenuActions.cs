using ETMmachine.SQLite_Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMmachine.Services
{
    public class MenuActions
    {
        public int Print()
        {
                Console.WriteLine("1: Withdraw money");
                Console.WriteLine("2: Last records");
                Console.WriteLine("Enter the number of the option you want to select:");

            do
            {
                string input = Console.ReadLine();

                if (int.TryParse(input, out int option) && option > 0 && option < 3)
                {
                    return option;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please try again.");
                }
            }
            while (true);
        }

        public void Withdraw(string iban)
        {
            Console.WriteLine("Enter amount (the smallest bill - 5 EUR):");
            do
            {
                string input = Console.ReadLine();

                if (int.TryParse(input, out int change) && (change % 5) == 0 && DatabaseManager.ReadBalance(iban) > change && change <1000)
                {
                    DatabaseManager.Update(iban, change);

                    DatabaseManager.InsertRecord(DateTime.Now, iban, change);

                    break;
                }
                else
                {
                    Console.WriteLine("Invalid Input or Not Enough money or Request is more than 1000. Please try again.");
                }
            }
            while (true);
        }
    }
}
