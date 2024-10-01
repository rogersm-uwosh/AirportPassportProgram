using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AirportPassportProgram.Model
{
    public interface IDatabase
    {
        ObservableCollection<Airport> SelectAllAirports();
        Airport SelectAirport(String id);
        AirportAdditionError InsertAirport(Airport airport);
        AirportDeletionError DeleteAirport(Airport airport);
        AirportEditError UpdateAirport(Airport replacementAirport);
    }
}