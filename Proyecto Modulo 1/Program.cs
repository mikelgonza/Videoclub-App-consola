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

            // Splash
            Menu.Splash();

            // Menu login
            Menu.MenuLogin();
        }

        public static void ClearConsoleLines(int lines)
        {
            for (int i = 0; i < lines; i++)
            {
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                int currentLineCursor = Console.CursorTop;
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, currentLineCursor);
            }
        }

        public static void MessageError(string message, int linesToClear)
        {
            Console.WriteLine(message);
            Console.ReadLine();
            Program.ClearConsoleLines(linesToClear);
        }

    }
}
