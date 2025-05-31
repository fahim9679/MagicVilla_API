using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicVilla_VillaAPI.Logging
{
    public class Logging : ILogging
    {
        public void Log(string message, string type)
        {
            if(type=="error")
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: {message}");
            }
            else if(type=="info")
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Info: {message}");
            }
            else if(type=="warning")
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Warning: {message}");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"Log: {message}");
            }            
        }
    }
}
