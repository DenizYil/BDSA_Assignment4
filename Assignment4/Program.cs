using System;
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

            //connect to database
            using(var connection = new NpgsqlConnection(connectionString)) {

                connection.Open();

                if(connection.State == System.Data.ConnectionState.Open) {
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
