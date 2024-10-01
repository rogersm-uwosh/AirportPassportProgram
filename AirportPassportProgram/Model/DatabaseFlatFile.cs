using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using System.Collections.ObjectModel;

// https://www.dotnetperls.com/serialize-list
// https://www.daveoncsharp.com/2009/07/xml-serialization-of-collections/


namespace AirportPassportProgram.Model;

public class DatabaseFlatFile : IDatabase
{
    const String filename = "airports.db";
    String airportsFile;
    ObservableCollection<Airport> airports;
    JsonSerializerOptions options;

    public DatabaseFlatFile ()
    {
        SelectAllAirports();
        options = new JsonSerializerOptions { WriteIndented = true };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public ObservableCollection<Airport>? SelectAllAirports()
    {
        string jsonString;

        String mainDir = FileSystem.Current.AppDataDirectory;
        airportsFile = String.Format("{0}/{1}", mainDir, filename);
        if (!File.Exists(airportsFile))
        {
            File.CreateText(airportsFile);
            airports = new ObservableCollection<Airport>();
            jsonString = JsonSerializer.Serialize(airports, options);
            File.WriteAllText(airportsFile, jsonString);
            return airports;
        }

        jsonString = File.ReadAllText(airportsFile);
        if (jsonString.Length > 0)
        {
            if (airports == null) // just starting the app, so let Deserialize() instantiate the ObservableCollection
            {
                airports = JsonSerializer.Deserialize<ObservableCollection<Airport>>(jsonString);
            } else { // airports already exists, and we have bound to it, so we cannot recreate it
                airports.Clear();
                ObservableCollection<Airport> localAirports = JsonSerializer.Deserialize<ObservableCollection<Airport>>(jsonString);
                foreach (Airport airport in localAirports)
                {
                    airports.Add(airport);
                }
            }
        }
        return airports;
    }

    public Airport? SelectAirport(String id)
    {
        foreach (Airport airport in airports)
        {
            if (airport.Id == id)
            {
                return airport;
            }
        }
        return null;
    }

    public AirportAdditionError InsertAirport(Airport airport)
    {
        try
        {
            airports.Add(airport);

            string jsonString = JsonSerializer.Serialize(airports, options);
            File.WriteAllText(airportsFile, jsonString);
            return AirportAdditionError.NoError;

        }
        catch (IOException ioe)
        {
            Console.WriteLine("Error while adding airport: {0}", ioe);
            return AirportAdditionError.DBAdditionError;

        }
    }



    /// <summary>
    /// Deletes an airport 
    /// </summary>
    /// 
    /// <param name="airport">An airport, which is presumed to exist</param>
    public AirportDeletionError DeleteAirport(Airport airport)
    {
        try
        {
            var result = airports.Remove(airport);
            string jsonString = JsonSerializer.Serialize(airports, options);
            File.WriteAllText(airportsFile, jsonString);
            return AirportDeletionError.NoError;
        }
        catch (IOException ioe)
        {
            Console.WriteLine("Error while deleting airport: {0}", ioe);
            return AirportDeletionError.DBDeletionError;
        }
    }

    public AirportEditError UpdateAirport(Airport replacementAirport)
    {
        foreach (Airport airport in airports) // iterate through entries until we find the Entry in question
        {
            if (airport.Id == replacementAirport.Id) // found it
            {
                airport.City = replacementAirport.City;
                airport.DateVisited = replacementAirport.DateVisited;
                airport.Rating = replacementAirport.Rating;         // change it then write it out

                try
                {
                    string jsonString = JsonSerializer.Serialize(airports, options);
                    File.WriteAllText(airportsFile, jsonString);
                    return AirportEditError.NoError;
                }
                catch (IOException ioe)
                {
                    Console.WriteLine("Error while replacing airport: {0}", ioe);
                    return AirportEditError.DBEditError;
                }
            }
        }
        return AirportEditError.NoError;
    }


}
