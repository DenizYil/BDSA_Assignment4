using System;
using System.Data;
using System.IO;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Assignment4
{
    class Program
    {
        static void Main(string[] args)
        {
            //get user secrets and connection string
            var configuration = LoadConfiguration();
            var connectionString = configuration.GetConnectionString("KanbanBoard");

            connectionString = "Server=localhost;Port=5432;Database=BDSA/Assignment4;User ID=postgres;Password=deyi;";

            //connect to database
            using(var connection = new NpgsqlConnection(connectionString)) 
            {
                connection.Open();

                using (var cmd = new NpgsqlCommand(String.Join("\n", File.ReadAllLines("createSchemas.sql")), connection)) 
                {
                    cmd.ExecuteNonQuery();
                }

                if(connection.State == ConnectionState.Open) {
                    Console.WriteLine("connected to database");
                }

            }
        }

        public static IConfiguration LoadConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddUserSecrets<Program>();

            return builder.Build();
        }
    }
}
