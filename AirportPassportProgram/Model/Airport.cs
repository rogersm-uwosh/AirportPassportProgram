using System;
using System.ComponentModel;

namespace AirportPassportProgram.Model;

[Serializable()]
public class Airport : INotifyPropertyChanged
{
    String id;
    String city;
    DateTime dateVisited;
    int rating;

    public String Id
    {
        get { return id; }
        set { id = value;
            OnPropertyChanged(nameof(Id));
        }
    }

    public String City
    {
        get { return city; }
        set { city = value;
            OnPropertyChanged(nameof(City));
        }
    }

    public DateTime DateVisited
    {
        get { return dateVisited; }
        set { dateVisited = value;
            OnPropertyChanged(nameof(DateVisited));
        }
    }

    public int Rating
    {
        get { return rating; }
        set { rating = value;
            OnPropertyChanged(nameof(Rating));
        }
    }

    public Airport(String id, String city, DateTime dateVisited, int rating)
    {
        Id = id;
        City = city;
        DateVisited = dateVisited;
        Rating = rating;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public override bool Equals(object obj)
    {
        var otherAirport = obj as Airport;
        return Id == otherAirport.Id;
    }

}
