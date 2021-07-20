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
            int userAge = DateTime.Now.Year - customer.BirthDate.Year;

            Console.Clear();

            // Show Header
            Menu.LogoHeader();
            Console.WriteLine("* LIST OF MOVIES *");
            Console.WriteLine($"User: {customer.Email}  Age: {userAge}");

            // Show Film list with age restriction
            Film.ShowFilmList(filmList, customer, true);

            do
            {
                Console.WriteLine("Press 0 to exit.");
                string option = Console.ReadLine();
                if (option == "0")
                {
                    exit = true;
                }
                else
                {
                    Program.ClearConsoleLines(2);
                }

            } while (!exit);
        }

        public static void RentMovie(List<Film> filmList, Customer customer)
        {
            bool exit = false;
            string message;
            int userAge = DateTime.Now.Year - customer.BirthDate.Year;

            Console.Clear();

            // Show Header
            Menu.LogoHeader();
            Console.WriteLine("* LIST OF MOVIES AVAILABLES FOR RENT*");
            Console.WriteLine($"User: {customer.Email}  Age: {userAge}");

            // Show Film list with age restriction and available for rent
            Film.ShowFilmList(filmList, customer, false);

            do
            {
                Console.Write("Enter the movie number to rent, 0 to Exit: ");
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
                                message = "Error writing to database, press any key to retry";
                                Program.MessageError(message, 4);
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
                                    message = "Error writing to database, press any key to retry";
                                    Program.MessageError(message, 4);
                                }
                            }
                        }
                        else
                        {
                            message = "Error: number is not valid, press any key to retry";
                            Program.MessageError(message, 4);
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
                    message = "Error: wrong option, press any key to retry";
                    Program.MessageError(message, 3);
                }

            } while (!exit);
        }

        public static void MyRentals(List<Film> filmList, Customer customer)
        {
            bool areFilmsPending = false;
            string message;
            DateTime dateToday = DateTime.Today;
            Rental rentalSelected = new Rental();
            Film filmSelected = new Film();
            List<Rental> rentalList = new List<Rental>();

            // Search rentals of customer
            string query = $"SELECT Rental.Id, Rental.FilmId, Film.Title, Rental.RentalDate, Rental.ReturnDate, Rental.ReturnedDate FROM Rental LEFT JOIN Film ON Film.Id = Rental.FilmId WHERE Rental.CustomerEmail = '{customer.Email}'";

            SqlCommand command = new SqlCommand(query, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            // Header
            Console.Clear();
            Menu.LogoHeader();
            Console.WriteLine($"* MY FILMS RENTED * user: {customer.Email} *");

            // If there are results, create object list and show it
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
                ShowUserRentalList(customer, rentalList, filmList);

                // If there are films pending, show message
                if (areFilmsPending)
                {
                    bool exit = false;

                    // Ask if want to return film
                    do
                    {
                        Console.WriteLine();
                        Console.WriteLine("You have movies pending to return, do you want to return any now?");
                        Console.Write("Enter the number to return the movie, or 0 to exit: ");
                        string option = Console.ReadLine();

                        Console.WriteLine();

                        // Check if option is a number
                        if (int.TryParse(option, out int rentalNum))
                        {
                            if (rentalNum != 0)
                            {
                                // get rental object from rentalList
                                foreach (var rental in rentalList)
                                {
                                    if (rental.ListNum == rentalNum)
                                    {
                                        rentalSelected = rental;
                                        break;
                                    }
                                }

                                // get film object from filmList
                                foreach (var film in filmList)
                                {
                                    if (film.Id == rentalSelected.FilmId)
                                    {
                                        filmSelected = film;
                                        break;
                                    }
                                }

                                // Return film, and check if the data has not been written successfully
                                if (!ReturnFilm(rentalSelected, customer))
                                {
                                    message = "Error with rental number, press any key to retry";
                                    Program.MessageError(message, 6);
                                }
                                else
                                {
                                    exit = true;
                                    Console.WriteLine("Movie returned successfully.");

                                    // Check update availability to database
                                    if (UpdateFilmAvailability(filmSelected, false))
                                    {
                                        Console.WriteLine("Updated film availability successfully");
                                        Console.WriteLine("Press any key to continue");
                                        Console.ReadLine();
                                    }
                                    else
                                    {
                                        message = "Sorry, error writing to database, press any key to retry";
                                        Program.MessageError(message, 6);
                                    }
                                    MyRentals(filmList, customer);
                                }
                            }
                            else
                            {
                                exit = true;
                            }
                        }
                        // if option is not a number
                        else
                        {
                            message = "Wrong answer, any key to retry";
                            Program.MessageError(message, 6);
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
            // If there are no movie rented, show message
            else
            {
                connection.Close();
                
                Console.WriteLine("─────────────────────────────────────────────────────");
                Console.WriteLine();
                Console.WriteLine("There is no movie rented, press any key to continue");
                Console.ReadLine();
            }
        }

        private static void ShowUserRentalList(Customer customer, List<Rental> rentalList, List<Film> filmList)
        {
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
                    Console.WriteLine($"Today is: {DateTime.Today.ToShortDateString()} - RETURN DATE PASSED");

                Console.ResetColor();
            }
            Console.WriteLine("─────────────────────────────────────────────────────");
            Console.WriteLine();
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

        private static bool ReturnFilm(Rental rentalSelected, Customer customer)
        {
            DateTime returnedDate = DateTime.Today;

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

