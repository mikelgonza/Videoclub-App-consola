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

        public static void ShowFilmList(List<Film> filmList, Customer customer)
        {
            foreach (var film in filmList)
            {
                string rented;

                if (film.Rented)
                    rented = "Yes";
                else
                    rented = "No";

                Console.WriteLine();
                Console.WriteLine($"Number: {film.Id}");
                Console.WriteLine($"Title: {film.Title}");
                Console.WriteLine($"Synopsis: {film.Synopsis}");
                Console.WriteLine($"Recommended age: {film.RecommendedAge}");
                Console.WriteLine($"Is rented?: {rented}");
                Console.WriteLine();
            }
        }

        public static void ShowFilmListAvailable(List<Film> filmList, Customer customer)
        {
            foreach (var film in filmList)
            {
                string rented;

                if (film.Rented)
                    rented = "Yes";
                else
                    rented = "No";

                if (film.Rented == false)
                {
                    Console.WriteLine();
                    Console.WriteLine($"Number: {film.Id}");
                    Console.WriteLine($"Title: {film.Title}");
                    Console.WriteLine($"Synopsis: {film.Synopsis}");
                    Console.WriteLine($"Recommended age: {film.RecommendedAge}");
                    Console.WriteLine($"Is rented?: {rented}");
                    Console.WriteLine();
                }
            }
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
