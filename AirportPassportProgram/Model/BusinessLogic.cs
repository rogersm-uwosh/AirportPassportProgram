using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AirportPassportProgram;

namespace AirportPassportProgram.Model;


public class BusinessLogic : IBusinessLogic
{
    const int BRONZE_LEVEL = 42;
    const int SILVER_LEVEL = 84;
    const int GOLD_LEVEL = 128;


    IDatabase db;
    private readonly int MAX_RATING = 5;

    public ObservableCollection<Airport> Airports
    {
        get { return GetAirports(); }

    }
    public BusinessLogic(IDatabase? db)
    {
        this.db = db;
    }


    public Airport FindAirport(String id)
    {
        return db.SelectAirport(id);
    }

    private AirportAdditionError CheckAirportFields(String id, String city, DateTime dateVisited, int rating)
    {
        if (id.Length < 3 || id.Length > 4)
        {
            return AirportAdditionError.InvalidIdLength;
        }
        if (city.Length < 3)
        {
            return AirportAdditionError.InvalidCityLength;
        }
        if (rating < 1 || rating > MAX_RATING)
        {
            return AirportAdditionError.InvalidRating;
        }

        return AirportAdditionError.NoError;
    }


    public AirportAdditionError AddAirport(String id, String city, DateTime dateVisited, int rating)
    {

        var result = CheckAirportFields(id, city, dateVisited, rating);
        if (result != AirportAdditionError.NoError)
        {
            return result;
        }

        if (db.SelectAirport(id) != null)
        {
            return AirportAdditionError.DuplicateAirportId;
        }
        Airport airport = new Airport(id, city, dateVisited, rating);
        db.InsertAirport(airport);

        return AirportAdditionError.NoError;
    }

    public AirportDeletionError DeleteAirport(String id)
    {

        var entry = db.SelectAirport(id);

        if (entry != null)
        {
            AirportDeletionError success = db.DeleteAirport(entry);
            if (success == AirportDeletionError.NoError)
            {
                return AirportDeletionError.NoError;

            }
            else
            {
                return AirportDeletionError.DBDeletionError;
            }
        }
        else
        {
            return AirportDeletionError.AirportNotFound;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="clue"></param>
    /// <param name="answer"></param>
    /// <param name="difficulty"></param>
    /// <param name="date"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public AirportEditError EditAirport(String id, String city, DateTime dateVisited, int rating)
    {

        var fieldCheck = CheckAirportFields(id, city, dateVisited, rating);
        if (fieldCheck != AirportAdditionError.NoError)
        {
            return AirportEditError.InvalidFieldError;
        }

        var airport = db.SelectAirport(id);
        airport.Id = id;
        airport.City = city;
        airport.DateVisited = dateVisited;
        airport.Rating = rating;

        AirportEditError success = db.UpdateAirport(airport);
        if (success != AirportEditError.NoError)
        {
            return AirportEditError.DBEditError;
        }

        return AirportEditError.NoError;
    }


    public String CalculateStatistics()
    {
        FlyWisconsinLevel nextLevel;
        int numAirportsUntilNextLevel;

        int numAirportsVisited = db.SelectAllAirports().Count;
        if(numAirportsVisited < BRONZE_LEVEL)
        {
            nextLevel = FlyWisconsinLevel.Bronze;
            numAirportsUntilNextLevel = BRONZE_LEVEL - numAirportsVisited;
        } else if(numAirportsVisited < SILVER_LEVEL)
        {
            nextLevel = FlyWisconsinLevel.Silver;
            numAirportsUntilNextLevel = SILVER_LEVEL - numAirportsVisited;
        } else if(numAirportsVisited < GOLD_LEVEL)
        {
            nextLevel = FlyWisconsinLevel.Gold;
            numAirportsUntilNextLevel = GOLD_LEVEL - numAirportsVisited;
        } else
        {
            nextLevel = FlyWisconsinLevel.None;
            numAirportsUntilNextLevel = 0;
        }

        return String.Format("{0} airport{1} visited; {2} airports remaining until achieving {3}",
              numAirportsVisited, numAirportsVisited != 1 ? "s" : "", numAirportsUntilNextLevel, nextLevel);
    }

    public ObservableCollection<Airport> GetAirports()
    {
        return db.SelectAllAirports();
    }


}

