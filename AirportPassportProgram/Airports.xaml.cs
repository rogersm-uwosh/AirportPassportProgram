namespace AirportPassportProgram;
using AirportPassportProgram.Model;

public partial class AirportsPage : ContentPage
{
    public AirportsPage()
    {
        InitializeComponent();

        // We've set the BindingContext for the entire page to be the BusinessLogic layer
        // So any control on the page can bind to the BusinessLogic layer
        // There's really only one control that needs to talk to the BusinessLogic layer, and that's the CollectionView

        BindingContext = MauiProgram.BusinessLogic;
    }

    // Various event handlers for the buttons on the main page

    void AddAirport_Clicked(System.Object sender, System.EventArgs e)
    {
        // The UI layer talks to the BusinessLogic layer, telling it what to do
        DateTime dateVisited;

        if (DateTime.TryParse(DateVisitedENT.Text, out dateVisited) == false)
        {
            DisplayAlert("Ruhroh", "Illegal date format", "OK");
        }
        else
        {
            AirportAdditionError result = MauiProgram.BusinessLogic.AddAirport(IdENT.Text, CityENT.Text, DateTime.Parse(DateVisitedENT.Text), int.Parse(RatingENT.Text));
            if (result != AirportAdditionError.NoError)
            {
                DisplayAlert("Ruhroh", result.ToString(), "OK");
            }
        }
    }

    void DeleteAirport_Clicked(System.Object sender, System.EventArgs e)
    {
        Airport currentAirport = CV.SelectedItem as Airport;
        AirportDeletionError result = MauiProgram.BusinessLogic.DeleteAirport(currentAirport.Id);
        if (result != AirportDeletionError.NoError)
        {
            DisplayAlert("Ruhroh", result.ToString(), "OK");
        }
    }

    void EditAirport_Clicked(System.Object sender, System.EventArgs e)
    {
        Airport currentAirport = CV.SelectedItem as Airport;
        DateTime dateVisited;

        if (DateTime.TryParse(DateVisitedENT.Text, out dateVisited) == false)
        {
            DisplayAlert("Ruhroh", "Illegal date format", "OK");
        }
        else
        {
            AirportEditError result = MauiProgram.BusinessLogic.EditAirport(currentAirport.Id, CityENT.Text, DateTime.Parse(DateVisitedENT.Text), int.Parse(RatingENT.Text));
            if (result != AirportEditError.NoError)
            {
                DisplayAlert("Ruhroh", result.ToString(), "OK");
            }
        }
    }

    void CalculateStatistics_Clicked(System.Object sender, System.EventArgs e)
    {
        String result = MauiProgram.BusinessLogic.CalculateStatistics();
        DisplayAlert("Your Progress", result.ToString(), "Good to know");
    }
}


