using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;

namespace Proyecto_Modulo_1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = " V I D E O   C L U B ";

            Console.Clear();
            //Console.WriteLine();
            //Console.WriteLine("  ██╗░░░██╗██╗██████╗░███████╗░█████╗░ ");
            //Console.WriteLine("  ██║░░░██║██║██╔══██╗██╔════╝██╔══██╗ ");
            //Console.WriteLine("  ╚██╗░██╔╝██║██║░░██║█████╗░░██║░░██║ ");
            //Console.WriteLine("  ░╚████╔╝░██║██║░░██║██╔══╝░░██║░░██║ ");
            //Console.WriteLine("  ░░╚██╔╝░░██║██████╔╝███████╗╚█████╔╝ ");
            //Console.WriteLine("  ░░░╚═╝░░░╚═╝╚═════╝░╚══════╝░╚════╝░ ");
            //Console.WriteLine();

            //Thread.Sleep(1000);

            //Console.WriteLine();
            //Console.WriteLine("   ░█████╗░██╗░░░░░██╗░░░██╗██████╗░");
            //Console.WriteLine("   ██╔══██╗██║░░░░░██║░░░██║██╔══██╗");
            //Console.WriteLine("   ██║░░╚═╝██║░░░░░██║░░░██║██████╦╝");
            //Console.WriteLine("   ██║░░██╗██║░░░░░██║░░░██║██╔══██╗");
            //Console.WriteLine("   ╚█████╔╝███████╗╚██████╔╝██████╦╝");
            //Console.WriteLine("   ░╚════╝░╚══════╝░╚═════╝░╚═════╝░");

            //Thread.Sleep(2000);

            // Menu login
            Menu.MenuLogin();

        }
    }
}
