using System.Collections.ObjectModel;
using Microsoft.Extensions.Configuration;
using Npgsql; // To install this, use dotnet add package Npgsql 


namespace AirportPassportProgram.Model;

public class Database : IDatabase
{
    private static System.Random rng = new();
    private String connString;

    ObservableCollection<Airport> airports = new();

    public Database()
    {
        connString = GetConnectionString();
    }


    // Fills our local Airports ObservableCollection with all the airports in the database
    // We don't cache the airports in the database, so we have to go to the database to get them
    // This is one of those "tradeoffs" we have to make when we use a database
    public ObservableCollection<Airport> SelectAllAirports()
    {
        airports.Clear();
        using var conn = new NpgsqlConnection(connString);
        try
        {
            conn.Open();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        // using() ==> disposable types are properly disposed of, even if there is an exception thrown 
        using var cmd = new NpgsqlCommand("SELECT id, city, date_visited, rating FROM airports", conn);
        using var reader = cmd.ExecuteReader(); // used for SELECT statement, returns a forward-only traversable object

        while (reader.Read()) // each time through we get another row in the table (i.e., another Airport)
        {
            String id = reader.GetString(0);
            String city = reader.GetString(1);
            DateTime dateVisited = reader.GetDateTime(2);
            Int32 rating = reader.GetInt32(3);
            Airport airportToAdd = new(id, city, dateVisited, rating);
            airports.Add(airportToAdd);
            Console.WriteLine(airportToAdd);
        }

        return airports;
    }

    // Finds the airport with the given id, null if not found
    public Airport SelectAirport(String id)
    {
        Airport airportToAdd = null;
        var conn = new NpgsqlConnection(connString);
        conn.Open();

        using var cmd = new NpgsqlCommand("SELECT id, city, date_visited, rating FROM airports WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("id", id);

        using var reader = cmd.ExecuteReader(); // used for SELECT statement, returns a forward-only traversable object
        if (reader.Read())
        { // there should be only one row, so we don't need a while loop TODO: Sanity check

            id = reader.GetString(0);
            String city = reader.GetString(1);
            DateTime dateVisited = reader.GetDateTime(2);
            Int32 rating = reader.GetInt32(3);
            airportToAdd = new(id, city, dateVisited, rating);
        }
        return airportToAdd;
    }

    // This inserts a airport into the database, or prints out an error message and returns false if the airport already exists
    // Notice the try-catch block, how could INSERT possibly fail?
    public AirportAdditionError InsertAirport(Airport airport)
    {
        try
        {
            using var conn = new NpgsqlConnection(connString); // conn, short for connection, is a connection to the database

            conn.Open(); // open the connection ... now we are connected!
            var cmd = new NpgsqlCommand(); // create the sql commaned
            cmd.Connection = conn; // commands need a connection, an actual command to execute
            cmd.CommandText = "INSERT INTO airports (id, city, date_visited, rating) VALUES (@id, @city, @date_visited, @rating)";
            cmd.Parameters.AddWithValue("id", airport.Id);
            cmd.Parameters.AddWithValue("city", airport.City);
            cmd.Parameters.AddWithValue("date_visited", airport.DateVisited);
            cmd.Parameters.AddWithValue("rating", airport.Rating);
            cmd.ExecuteNonQuery(); // used for INSERT, UPDATE & DELETE statements - returns # of affected rows 

            SelectAllAirports();
        }
        catch (Npgsql.PostgresException pe)
        {
            Console.WriteLine("Insert failed, {0}", pe);
            return AirportAdditionError.DBAdditionError;
        }
        return AirportAdditionError.NoError;
    }



    // The UI >> BusinessLogic >> Database the currently selected airport (selected in the CollectionView) for upating 
    public AirportEditError UpdateAirport(Airport airportToUpdate)
    {
        try
        {
            using var conn = new NpgsqlConnection(connString); // conn, short for connection, is a connection to the database

            conn.Open(); // open the connection ... now we are connected!
            var cmd = new NpgsqlCommand(); // create the sql commaned
            cmd.Connection = conn; // commands need a connection, an actual command to execute
            cmd.CommandText = "UPDATE airports SET city = @city, date_visited = @date_visited, rating = @rating WHERE id = @id;";

            cmd.Parameters.AddWithValue("id", airportToUpdate.Id);
            cmd.Parameters.AddWithValue("city", airportToUpdate.City);
            cmd.Parameters.AddWithValue("date_visited", airportToUpdate.DateVisited);
            cmd.Parameters.AddWithValue("rating", airportToUpdate.Rating);
            var numAffected = cmd.ExecuteNonQuery();

            SelectAllAirports();
        }
        catch (Npgsql.PostgresException pe)
        {
            Console.WriteLine("Update failed, {0}", pe);
            return AirportEditError.DBEditError;
        }
        return AirportEditError.NoError;
    }


    public AirportDeletionError DeleteAirport(Airport airportToDelete)
    {
        var conn = new NpgsqlConnection(connString);
        conn.Open();

        using var cmd = new NpgsqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "DELETE FROM airports WHERE id = @id";
        cmd.Parameters.AddWithValue("id", airportToDelete.Id);
        int numDeleted = cmd.ExecuteNonQuery();

        if (numDeleted > 0)
        {
            SelectAllAirports(); // go and fetch the airports again, otherwise Airports will be out of sync with the database
            return AirportDeletionError.NoError;
        }
        else
        {
            return AirportDeletionError.AirportNotFound;
        }
    }


    // Builds a ConnectionString, which is used to connect to the database
    static String GetConnectionString()
    {
        var connStringBuilder = new NpgsqlConnectionStringBuilder();
        connStringBuilder.Host = "";
        connStringBuilder.Port = 26257;
        connStringBuilder.SslMode = SslMode.Require;
        connStringBuilder.Username = ""; // won't hardcode this in your app
        connStringBuilder.Password = FetchPassword();
        connStringBuilder.Database = "lab4";
        connStringBuilder.ApplicationName = "whatever";
        connStringBuilder.KeepAlive = 10;
        connStringBuilder.Pooling = false;
        connStringBuilder.Timeout = 60;

        return connStringBuilder.ConnectionString;
    }

    // Fetches the password from the user secrets store (um, this works in VS, but not in the beta of VSC's C# extension)
    // This assumes the NuGet package is installed -- dotnet add package Microsoft.Extensions.Configuration.UserSecrets
    static String FetchPassword()
    {
        return ""; // A very bad idea indeed ...
    }


}

