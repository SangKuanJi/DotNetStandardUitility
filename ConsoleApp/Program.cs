using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotPot.Utility;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var urlEncode = "黑泽明".UrlEncode();
            Console.WriteLine(urlEncode);
            Console.ReadLine();
        }
    }
}
