using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace ETMmachine.Services
{
    public class GUIDcomparator
    {
        public readonly Guid ConstantGuid = new Guid("00000000-0000-0000-0000-000000000000");

        public void AreGuidsEqual(Guid checkGUID)
        {
            if (ConstantGuid.Equals(checkGUID)) 
            {
                Console.WriteLine("Welcom to Danske");
            }
            else
            {
                Console.WriteLine("This ATM is Danske, your card is not valid");
                Console.ReadKey();
                Environment.Exit(0);
            }
        }
    }
}
