using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;

namespace Proyecto_Modulo_1
{
    class Customer
    {
        // Declare sql connection
        static SqlConnection connection = new SqlConnection("Data Source=TOSHIBA\\SQLEXPRESS; Initial Catalog=VIDEOCLUB; Integrated Security=True");


        // Attibutes
        public int Id { get; set; }

        public string UserName { get; set; }

        public string UserSurname { get; set; }

        public DateTime BirthDate { get; set; }

        public string Email { get; set; }

        public string UserPass { get; set; }


        // Methods
        public static List<Customer> CreateList()
        {
            List<Customer> customerList = new List<Customer>();

            string query = "SELECT * FROM Customer";
            SqlCommand command = new SqlCommand(query, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Customer customer = new Customer();
                customer.Id = Convert.ToInt32(reader[0]);
                customer.UserName = reader[1].ToString();
                customer.UserSurname = reader[2].ToString();
                customer.BirthDate = Convert.ToDateTime(reader[3]);
                customer.Email = reader[4].ToString();
                customer.UserPass = reader[5].ToString();

                customerList.Add(customer);
            }
            connection.Close();

            return customerList;
        }

        public static void LoginUser()
        {
            bool exit = false;
            int totalErrors = 5;
            int currentError = totalErrors;
            Customer customer = new Customer();
            string password;
            ConsoleKeyInfo key;

            Console.Clear();

            // Loop to check login
            do
            {
                Menu.LogoHeader();
                Console.WriteLine("* USER LOGIN *");
                Console.WriteLine();

                Console.Write("Enter Email: ");
                string email = Console.ReadLine();

                Console.Write("Enter password: ");
                password = string.Empty;

                // Hide password
                do
                {
                    key = Console.ReadKey(true);

                    // Ignore enter and backspace keys
                    if (key.Key != ConsoleKey.Enter && key.Key != ConsoleKey.Backspace)
                    {
                        // Append the character to the password
                        password += key.KeyChar;
                        Console.Write("*");
                    }
                    // if key is backspace remove latest char
                    else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                    {
                        password = password.Remove(password.Length - 1);
                        Console.Write("\b \b");
                    }

                    // exit if enter key is pressed
                } while (key.Key != ConsoleKey.Enter);

                Console.WriteLine();


                // Check if login is ok
                customer = CheckIfPasswordIsOk(email, password);
                if (customer != null)
                {
                    // If object received is not null, login is ok
                    exit = true;

                    Console.WriteLine();
                    Console.WriteLine("Successfully logged in");
                    Thread.Sleep(1500);

                    // Call to movies menu and send customer object
                    Menu.MenuFilms(customer);
                }
                else
                {
                    // If login is not ok
                    currentError--;

                    if (currentError == 0)
                    {
                        exit = true;

                        Console.Beep(200, 400);
                        Console.Beep(200, 800);

                        Console.WriteLine();
                        Console.WriteLine($"You have exceeded the number of attempts allowed ({totalErrors}), press a key to exit");
                        Console.ReadLine();
                    }
                    else
                    {
                        Console.Beep(200, 400);

                        Console.WriteLine();
                        Console.WriteLine($"Email or password are not valid, you have {currentError} attempts left");
                        Console.WriteLine("Press any key to continue, 0 exit to menu");
                        if (Console.ReadLine() == "0")
                            exit = true;
                    }
                }

                Console.Clear();

            } while (!exit);
        }

        public static void RegisterUser()
        {
            bool error;
            DateTime birthdate = DateTime.Now;
            string email, pass, name, surname;
            Customer customer = new Customer();

            Console.Clear();

            Menu.LogoHeader();
            Console.WriteLine("* REGISTER NEW USER *");
            Console.WriteLine();

            // Check name format
            do
            {
                error = false;

                Console.Write("Name: ");
                name = Console.ReadLine();

                if (name != string.Empty && name.Length <= 25)
                {
                    // Name is correct
                    customer.UserName = name;
                }
                else
                {
                    Console.WriteLine("Error: invalid name, try again please");
                    error = true;
                }

            } while (error);


            // Check surname format
            do
            {
                error = false;

                Console.Write("Surname: ");
                surname = Console.ReadLine();

                if (surname != string.Empty && surname.Length <= 25)
                {
                    // Name is correct
                    customer.UserSurname = surname;
                }
                else
                {
                    Console.WriteLine("Error: invalid surname, try again please");
                    error = true;
                }

            } while (error);

            // Check the date format
            do
            {
                error = false;
                try
                {
                    Console.Write("Birthdate: ");
                    birthdate = Convert.ToDateTime(Console.ReadLine());
                }
                catch (FormatException)
                {
                    Console.WriteLine("Error: date format error, try again please");
                    error = true;
                }

            } while (error);
            customer.BirthDate = birthdate;

            // Check the Email format
            do
            {
                error = false;

                Console.Write("Email: ");
                email = Console.ReadLine();

                //if (new EmailAddressAttribute().IsValid(email))
                if (EmailValidator.EmailIsValid(email) && email.Length <= 50)
                {
                    // email is correct
                    customer.Email = email;
                }
                else
                {
                    Console.WriteLine("Error: email format error, try again please");
                    error = true;
                }

            } while (error);

            // Check the password format
            // 8 characters minimal
            do
            {
                error = false;

                Console.Write("Password (min 8, max 16 characters): ");
                pass = Console.ReadLine();

                if (pass.Length >= 8 && pass.Length <= 16)
                {
                    // pass is correct
                    customer.UserPass = pass;
                }
                else
                {
                    Console.WriteLine("Error: password format error, try again please");
                    error = true;
                }
            } while (error);

            Console.WriteLine();

            // Check if the data has been written successfully
            if (InsertUserIntoDatabase(customer))
            {
                Console.WriteLine("User created successfully");
                Thread.Sleep(1500);
            }
            else
            {
                Console.WriteLine("Error writing to database");
                Console.WriteLine("Please try again");
                Thread.Sleep(1500);
            }

        }
        
        private static bool InsertUserIntoDatabase(Customer customer)
        {
            try
            {
                string query = $"INSERT INTO Customer (UserName, UserSurname, BirthDate, Email, UserPass) values ('{customer.UserName}','{customer.UserSurname}','{customer.BirthDate}','{customer.Email}','{customer.UserPass}')";

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

        private static Customer CheckIfPasswordIsOk(string email, string password)
        {
            Customer customer = null;

            string query = $"SELECT * FROM Customer WHERE Email = '{email}' AND UserPass = '{password}'";
            SqlCommand command = new SqlCommand(query, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                customer = new Customer
                {
                    Id = Convert.ToInt32(reader[0]),
                    UserName = reader[1].ToString(),
                    UserSurname = reader[2].ToString(),
                    BirthDate = Convert.ToDateTime(reader[3]),
                    Email = reader[4].ToString(),
                    UserPass = reader[5].ToString()
                };
            }

            connection.Close();

            return customer;
        }
    }
}
