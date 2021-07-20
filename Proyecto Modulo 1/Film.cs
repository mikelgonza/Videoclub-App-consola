using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Proyecto_Modulo_1
{
    class Film
    {
        // Declare sql connection
        static SqlConnection connection = new SqlConnection("Data Source=TOSHIBA\\SQLEXPRESS; Initial Catalog=VIDEOCLUB; Integrated Security=True");


        // Attributes
        public int Id { get; set; }

        public string Title { get; set; }

        public string Synopsis { get; set; }

        public int RecommendedAge { get; set; }

        public bool Rented { get; set; }


        // Methods
        public static List<Film> CreateList(Customer customer)
        {
            int currentYear = DateTime.Now.Year;
            int userYear = currentYear - customer.BirthDate.Year;
            List<Film> filmList = new List<Film>();

            string query = $"SELECT * FROM Film WHERE RecommendedAge <= '{userYear}'";
            SqlCommand command = new SqlCommand(query, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Film film = new Film();
                film.Id = Convert.ToInt32(reader[0]);
                film.Title = reader[1].ToString();
                film.Synopsis = reader[2].ToString();
                film.RecommendedAge = Convert.ToInt32(reader[3]);
                film.Rented = Convert.ToBoolean(reader[4]);

                filmList.Add(film);
            }
            connection.Close();

            return filmList;
        }

        public static void ShowFilmList(List<Film> filmList, Customer customer, bool showRented)
        {
            Console.WriteLine();

            foreach (var film in filmList)
            {
                string availability;
                string age = film.RecommendedAge.ToString();

                if (film.Rented)
                    availability = "No";
                else
                    availability = "Yes";

                if (film.RecommendedAge == 0)
                    age = "All";

                if (!film.Rented || showRented)
                {
                    Console.WriteLine("─────────────────────────────────────────────────────");
                    Console.WriteLine($"Number: {film.Id}");
                    Console.WriteLine($"Title: {film.Title}");
                    Console.WriteLine($"Synopsis: {film.Synopsis}");
                    Console.WriteLine($"Recommended age: {age}");
                    Console.WriteLine($"Available: {availability}");
                }
            }
            Console.WriteLine("─────────────────────────────────────────────────────");
            Console.WriteLine();
        }

        public static Film SearchMovieInList(List<Film> filmList, int filmNumber)
        {
            foreach (var film in filmList)
            {
                if (film.Id == filmNumber && film.Rented == false)
                    return film;
            }

            return null;
        }
    }
}
