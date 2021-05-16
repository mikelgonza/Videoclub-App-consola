using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;

namespace Proyecto_Modulo_1
{
    class Rental
    {
        // Declare sql connection
        static SqlConnection connection = new SqlConnection("Data Source=TOSHIBA\\SQLEXPRESS; Initial Catalog=VIDEOCLUB; Integrated Security=True");


        // Attributes
        public int Id { get; set; }

        public int ListNum { get; set; }

        public int CustomerId { get; set; }

        public int FilmId { get; set; }

        public string FilmTitle { get; set; }

        public DateTime RentalDate { get; set; }

        public DateTime ReturnDate { get; set; }

        public DateTime ReturnedDate { get; set; }


        // Methods
        public static void ShowAvailableMovies(List<Film> filmList, Customer customer)
        {
            bool exit = false;

            Console.Clear();

            // Show Header
            Menu.LogoHeader();
            Console.WriteLine("* LIST OF MOVIES *");

            // Show Film list with age restriction
            Film.ShowFilmList(filmList, customer);

            do
            {
                Console.WriteLine("Press 0 to exit.");
                string option = Console.ReadLine();
                if (option == "0")
                    exit = true;

            } while (!exit);
        }

        public static void RentMovie(List<Film> filmList, Customer customer)
        {
            bool exit = false;

            Console.Clear();

            // Show Header
            Menu.LogoHeader();
            Console.WriteLine("* LIST OF MOVIES AVAILABLES FOR RENT*");

            // Show Film list with age restriction and available for rent
            Film.ShowFilmListAvailable(filmList, customer);

            do
            {
                Console.Write("Enter the movie number to rent, 0 to Cancel: ");
                string option = Console.ReadLine();

                // Check if option is a number
                if (int.TryParse(option, out int filmId))
                {
                    if (filmId != 0)
                    {
                        Console.WriteLine();

                        // Get film selected
                        Film filmSelected = Film.SearchMovieInList(filmList, filmId);

                        // Check if filmSelected is not null
                        if (filmSelected != null)
                        {
                            exit = true;

                            // Check if insert rental to database is NOT ok
                            if (!InsertRentalToDatabase(customer, filmSelected, filmList))
                            {
                                Console.WriteLine("Error writing to database");
                                Console.WriteLine("Please try again, press any key to continue");
                                Console.ReadLine();
                            }
                            else // if insert to database is ok
                            {
                                Console.WriteLine("Rental created successfully");

                                // Check update availability to database
                                if (UpdateFilmAvailability(filmSelected, true))
                                {
                                    Console.WriteLine("Updated film availability successfully");
                                    Console.WriteLine();
                                    Console.WriteLine("Thank you for using our video club service. Enjoy your movie.");
                                    Console.WriteLine("Press any key to continue");
                                    Console.ReadLine();
                                }
                                else
                                {
                                    Console.WriteLine("Sorry, error writing to database");
                                    Console.WriteLine("Please try again, press any key to continue");
                                    Console.ReadLine();
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Error: number is not valid");
                        }
                    }
                    else
                    {
                        // If option is 0 exit
                        exit = true;
                    }
                }
                else
                {
                    Console.WriteLine("Error: wrong option");
                }

            } while (!exit);
        }

        public static void MyRentals(Customer customer)
        {
            bool areFilmsPending = false;
            DateTime dateToday = DateTime.Today;
            List<Rental> rentalList = new List<Rental>();

            // Show Header
            Menu.LogoHeader();

            // Search rentals of customer
            string query = $"SELECT Rental.Id, Rental.FilmId, Film.Title, Rental.RentalDate, Rental.ReturnDate, Rental.ReturnedDate FROM Rental LEFT JOIN Film ON Film.Id = Rental.FilmId WHERE Rental.CustomerEmail = '{customer.Email}'";

            SqlCommand command = new SqlCommand(query, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();


            // If there are results, create list and show it
            if (reader.HasRows)
            {
                int listNum = 1;

                while (reader.Read())
                {
                    Rental rental = new Rental();
                    rental.ListNum = listNum;
                    rental.Id = Convert.ToInt32(reader[0]);
                    rental.FilmId = Convert.ToInt32(reader[1]);
                    rental.FilmTitle = reader[2].ToString();
                    rental.RentalDate = Convert.ToDateTime(reader[3]);
                    rental.ReturnDate = Convert.ToDateTime(reader[4]);
                    try
                    {
                        rental.ReturnedDate = Convert.ToDateTime(reader[5]);
                    }
                    catch (InvalidCastException)
                    {
                        rental.ReturnedDate = new DateTime();
                        areFilmsPending = true;
                    }

                    listNum++;
                    rentalList.Add(rental);
                }

                connection.Close();

                // Show the rental list of user
                ShowUserRentalList(customer, rentalList, areFilmsPending);

            }
            // If there are no movie rented, show message
            else
            {
                Console.WriteLine("─────────────────────────────────────────────────────");
                Console.WriteLine();
                Console.WriteLine("There is no movie rented");
            }

            connection.Close();
        }

        private static void ShowUserRentalList(Customer customer, List<Rental> rentalList, bool areFilmsPending)
        {

            Console.Clear();
            Console.WriteLine($"User: {customer.Email}");
            Console.WriteLine("MY FILMS RENTED");

            foreach (var rental in rentalList)
            {
                bool returnDatePassed = DateTime.Today > rental.ReturnDate && rental.ReturnedDate == new DateTime();

                Console.WriteLine("─────────────────────────────────────────────────────");

                // If return date passed, console to RED
                if (returnDatePassed)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                Console.Write($"No. {rental.ListNum} ");
                Console.WriteLine($"{rental.FilmTitle}");
                Console.Write($"Rental date: {rental.RentalDate.ToShortDateString()} ");
                Console.WriteLine($"Return date: {rental.ReturnDate.ToShortDateString()}");

                if (rental.ReturnedDate == new DateTime())
                    Console.WriteLine($"Returned date: Not returned");
                else
                    Console.WriteLine($"Returned date: {rental.ReturnedDate.ToShortDateString()}");

                if (returnDatePassed)
                    Console.WriteLine("DELIVERY DATE PASSED");

                Console.ResetColor();
            }
            Console.WriteLine("─────────────────────────────────────────────────────");
            Console.WriteLine();

            // If there are films pending, show message
            if (areFilmsPending)
            {
                bool exit = false;

                // Ask if want to return film
                do
                {
                    Console.WriteLine();
                    Console.WriteLine("You have movies pending to return, do you want to return any now?");
                    Console.Write("Enter rental number, or enter 0 to exit: ");
                    string option = Console.ReadLine();

                    Console.WriteLine();

                    // Check if option is a number
                    if (int.TryParse(option, out int rentalNum))
                    {
                        if (rentalNum != 0)
                        {
                            // Return film, and check if the data has been written successfully
                            if (ReturnFilm(rentalNum, customer, rentalList))
                            {
                                exit = true;
                                Console.WriteLine("Movie returned successfully.");
                                Console.WriteLine("Press any key to continue");
                                Console.ReadLine();
                                MyRentals(customer);
                            }
                            else
                            {
                                Console.WriteLine("Error with rental number");
                                Console.WriteLine("Please try again, press any key to continue");
                                Console.ReadLine();
                            }
                        }
                        else
                        {
                            exit = true;
                        }
                    }
                    else
                    {
                        // If is not number
                        Console.WriteLine("Wrong answer, enter the number or film, or 0 to exit");
                    }

                } while (!exit);
            }
            // If there are no films pending, show message
            else
            {
                Console.WriteLine("You have no films pending return.");
                Console.WriteLine("Press any key to exit");
                Console.ReadLine();
            }


        }

        private static bool InsertRentalToDatabase(Customer customer, Film film, List<Film> filmList)
        {
            DateTime rentalDate = DateTime.Today;
            DateTime returnDate = rentalDate.AddDays(1);

            try
            {
                string query = $"INSERT INTO Rental (CustomerEmail, FilmId, RentalDate, ReturnDate) values ('{customer.Email}','{film.Id}','{rentalDate}','{returnDate}')";

                SqlCommand commandInsertarCliente = new SqlCommand(query, connection);
                connection.Open();
                commandInsertarCliente.ExecuteNonQuery();
                connection.Close();

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                return false;
            }
        }

        private static bool UpdateFilmAvailability(Film film, bool rented)
        {
            try
            {
                string query = $"UPDATE Film SET Rented = '{rented}' WHERE Id = '{film.Id}'";

                SqlCommand commandInsertarCliente = new SqlCommand(query, connection);
                connection.Open();
                commandInsertarCliente.ExecuteNonQuery();
                connection.Close();

                // Update Film list
                film.Rented = true;

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                return false;
            }
        }

        private static bool ReturnFilm(int rentalNum, Customer customer, List<Rental> rentalList)
        {
            DateTime returnedDate = DateTime.Today;
            Rental rentalSelected = new Rental();

            foreach (var rental in rentalList)
            {
                if (rental.ListNum == rentalNum)
                    rentalSelected = rental;
            }

            try
            {
                string query = $"UPDATE Rental SET ReturnedDate = '{returnedDate}' WHERE Id = '{rentalSelected.Id}' AND CustomerEmail = '{customer.Email}' AND ReturnedDate IS NULL";

                SqlCommand commandInsertarCliente = new SqlCommand(query, connection);
                connection.Open();
                int rowsAffected = commandInsertarCliente.ExecuteNonQuery();
                connection.Close();

                if (rowsAffected == 1)
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }
    }
}

