using Spectre.Console;
using PragueParking.Core;
using PragueParking.Data;

internal class Program
{
    private static ParkingGarage? garage;
    private static GarageConfiguration? config;
    private static void Main(string[] args)
    {
        AnsiConsole.Write(new FigletText("Prague Parking V2").Centered().Color(Color.Blue));
        
        // Läser in konfigurationen för garaget från en JSON-fil.
        config = ConfigurationService.ReadConfiguration();

        if (config == null)
        {
            AnsiConsole.MarkupLine("[red]Kunde inte läsa in konfigurationen. Avslutar programmet.[/]");
            return;
        }
        // Skapar ett parkeringsgarage baserat på den inlästa konfigurationen.
        garage = new ParkingGarage(config.NumberOfSpots);

        // Laddar tidigare sparad parkeringsdata om sådan finns.
        GarageStateService.LoadGarageState(garage);

        AnsiConsole.MarkupLine("[bold green]Garaget är klart för att användas. [/]");

        var uiService = new UiService(garage, config);

        bool running = true;
        while (running)
        {
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold]Välj ett alternativ:[/]")
                    .AddChoices(new[] {
                        "Parkera fordon",
                        "Hämta fordon",
                        "Flytta fordon",
                        "Sök fordon",
                        "Visa garage",
                        "Avsluta"
                    }));

            switch (choice)
            {
                case "Parkera fordon":
                    uiService.ParkVehicle();
                    break;
                case "Hämta fordon":
                    uiService.UnparkVehicle();
                    break;
                case "Flytta fordon":
                    uiService.MoveVehicle();
                    break;
                case "Sök fordon":
                    uiService.FindVehicle();
                    break;
                case "Visa garage":
                    uiService.ShowGarage();
                    break;
                case "Avsluta":
                    running = false;
                    AnsiConsole.MarkupLine("[bold]Avslutar programmet...[/]");
                    Thread.Sleep(1000);
                    break;
            }
        }
    }
}