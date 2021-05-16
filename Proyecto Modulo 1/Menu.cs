using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Proyecto_Modulo_1
{
    class Menu
    {
        public static void MenuLogin()
        {
            bool exit = false;

            Console.Clear();
            

            do
            {
                LogoHeader();
                Console.WriteLine(" ╔═════════════════════════════════════════════╗ ");
                Console.WriteLine(" ║                                             ║ ");
                Console.WriteLine(" ║ 1. User Login                               ║ ");
                Console.WriteLine(" ║                                             ║ ");
                Console.WriteLine(" ║ 2. Register new user                        ║ ");
                Console.WriteLine(" ║                                             ║ ");
                Console.WriteLine(" ╠═════════════════════════════════════════════╣ ");
                Console.WriteLine(" ║ 0. Exit                                     ║ ");
                Console.WriteLine(" ╚═════════════════════════════════════════════╝ ");
                Console.WriteLine();
                Console.Write("Select an option: ");
                string option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        Customer.LoginUser();
                        break;
                    case "2":
                        Customer.RegisterUser();
                        break;
                    case "0":
                        Console.WriteLine("Thanks for using our VideoClub app");
                        Console.ReadLine();
                        exit = true;
                        break;
                    default:
                        break;
                }

                Console.Clear();

            } while (!exit);
        }

        public static void MenuFilms(Customer customer)
        {
            bool exit = false;
            List<Film> filmList = Film.CreateList(customer);

            Console.Clear();

            do
            {
                LogoHeader();
                Console.WriteLine(" ╔═════════════════════════════════════════════╗ ");
                Console.WriteLine(" ║                                             ║ ");
                Console.WriteLine(" ║ 1. List available movies                    ║ ");
                Console.WriteLine(" ║                                             ║ ");
                Console.WriteLine(" ║ 2. Rent                                     ║ ");
                Console.WriteLine(" ║                                             ║ ");
                Console.WriteLine(" ║ 3. My rentals                               ║ ");
                Console.WriteLine(" ║                                             ║ ");
                Console.WriteLine(" ║ 4. Logout                                   ║ ");
                Console.WriteLine(" ║                                             ║ ");
                Console.WriteLine(" ╚═════════════════════════════════════════════╝ ");
                Console.WriteLine($"  User: {customer.Email}");

                Console.WriteLine();
                Console.Write("Select an option: ");
                string option = Console.ReadLine();
                Console.WriteLine();

                switch (option)
                {
                    case "1":
                        Rental.ShowAvailableMovies(filmList, customer);
                        break;
                    case "2":
                        Rental.RentMovie(filmList, customer);
                        break;
                    case "3":
                        Rental.MyRentals(customer);
                        break;
                    case "4":
                        Console.WriteLine("Are you sure you want to logout? (\"yes\" to confirm, any key to Cancel)");
                        string logout = Console.ReadLine();
                        if (logout == "yes" || logout == "Yes" || logout == "YES")
                        {
                            exit = true;
                            Console.WriteLine("OK, logging out...");
                            Thread.Sleep(1500);
                        }
                        break;
                    default:
                        break;
                }

                Console.Clear();

            } while (!exit);
        }

        public static void LogoHeader()
        {
            Console.WriteLine();
            Console.WriteLine(" ░█  ░█  ▀  █▀▀▄ █▀▀ █▀▀█ 　 ░█▀▀█ █   █  █ █▀▀▄ ");
            Console.WriteLine("  ░█░█  ▀█▀ █  █ █▀▀ █  █ 　 ░█    █   █  █ █▀▀▄ ");
            Console.WriteLine("   ▀▄▀  ▀▀▀ ▀▀▀  ▀▀▀ ▀▀▀▀ 　 ░█▄▄█ ▀▀▀  ▀▀▀ ▀▀▀  ");
            Console.WriteLine();
        }
    }
}
