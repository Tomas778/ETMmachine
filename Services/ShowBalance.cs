using ETMmachine.SQLite_Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMmachine.Services
{
    public class ShowBalance
    {
        public void Print(string iban)
        {
            Console.WriteLine($"Your Balance: {DatabaseManager.ReadBalance(iban).ToString("0.00")} EUR");
        }

    }
}
