using ETMmachine.SQLite_Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMmachine.Services
{
    public class PINchecker
    {

        public int CorrectPin;

        public bool IsPinCorrect(int pin, string iban)
        {

            return pin == DatabaseManager.ReadPin(iban);
        }

        public void PromptForPin(string iban)
        {
            int attempts = 0;
            while (attempts < 3)
            {
                Console.WriteLine("Please enter your PIN: ");
                string pinString = Console.ReadLine();
                int pin;
                if (int.TryParse(pinString, out pin) && IsPinCorrect(pin, iban))
                {
                    Console.Clear();
                    Console.WriteLine("The PIN is correct.");

                    return;
                }
                else
                {
                    Console.WriteLine("The PIN is incorrect.");
                    attempts++;
                }
            }
            Console.WriteLine("You have exceeded the maximum number of attempts");
            Console.ReadKey();
            Environment.Exit(0);
        }
    }
}
