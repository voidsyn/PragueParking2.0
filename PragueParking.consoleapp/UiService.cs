using Spectre.Console;
using PragueParking.Core;
using PragueParking.Data;
using System.ComponentModel.Design;
using Spectre.Console.Rendering;

public class UiService
{
    private readonly ParkingGarage _garage;
    private readonly GarageConfiguration _config;
    public UiService(ParkingGarage garage, GarageConfiguration config)
    {
        _garage = garage;
        _config = config;
    }
    public void ParkVehicle()
    {
        //Frågar anvädaren om fordonstyp.
        var userInput = AnsiConsole.Ask<string>("Ange fordonstyp ([blue]Bil[/] eller [blue]MC[/]):").ToUpper().Trim();

        string? internalVehicleTypeKey = userInput switch
        {
            "BIL" => "CAR",
            "MC" => "MC",
            _ => null
        };

        if (internalVehicleTypeKey == null || !_config.VehicleTypes.ContainsKey(internalVehicleTypeKey))
        {
            AnsiConsole.MarkupLine("[red]Ogiltig fordonstyp.[/]");
            return;
        }

        // Frågar användaren om registreringsnummer.
        var regNr = AnsiConsole.Ask<string>("Ange registreringsnummer: ").ToUpper().Trim();
        if (string.IsNullOrWhiteSpace(regNr))
        {
            AnsiConsole.MarkupLine("[red]Registreringsnummer kan inte vara tomt.[/]");
            return;
        }
        Vehicle vehicleToPark;
        switch (internalVehicleTypeKey)
        {
            case "CAR":
                vehicleToPark = new Car(regNr);
                break;
            case "MC":
                vehicleToPark = new MC(regNr);
                break;
            default:
                AnsiConsole.MarkupLine("[red]Okänd fordonstyp.[/]");
                return;
        }

        bool success = _garage.ParkVehicle(vehicleToPark);

        if (success)
        {
            var spot = _garage.Spots.FirstOrDefault(s => s.ParkedVehicles.Contains(vehicleToPark));
            if (spot != null)
            {
                AnsiConsole.MarkupLine($"[green] Fordonet {vehicleToPark.Type} {regNr} har parkerats på plats {spot.SpotNumber + 1}. [/]");
                GarageStateService.SaveGarageState(_garage);
            }
            else
            {
                AnsiConsole.MarkupLine($"[red]Fordonet Parkerades, men platsen kunde inte hittas.[/]");
            }
        }
        else
        {
            AnsiConsole.MarkupLine($"[red]Kunde inte parkera {vehicleToPark.Type} {regNr}. Inga lediga platser.[/]");
        }

    }
    public void UnparkVehicle()
    {
        // Frågar användaren om registreringsnummer.
        string regNumber = AnsiConsole.Ask<string>("Ange registreringsnummer för fordonet som ska hämtas: ").ToUpper().Trim();

        Vehicle? vehicleToUnpark = _garage.GetVehicle(regNumber);

        if (vehicleToUnpark == null)
        {
            AnsiConsole.MarkupLine($"[red]Fordonet med registeringsnummret {regNumber} hittades inte i garaget.[/]");
            return;
        }

        bool unparkSuccess = _garage.UnparkVehicle(regNumber);

        if (unparkSuccess)
        {
            double pricePerHour = _config.PricesPerHour.GetValueOrDefault(vehicleToUnpark.Type, 0.0);
            int freeMinutes = _config.FreeMinutes;
            string receipt = vehicleToUnpark.GenerateReceipt(pricePerHour, freeMinutes);



            AnsiConsole.MarkupLine($"[green]Fordonet med registeringsnummer {regNumber} har hämtats.[/]");
            AnsiConsole.Write(new Panel(receipt)
                .Header("Kvitto")
                .Border(BoxBorder.Double)
                .BorderColor(Color.Green)
                .Expand());

            GarageStateService.SaveGarageState(_garage);
        }
        else
        {
            AnsiConsole.MarkupLine($"[red]Kunde inte ta bort fordonet {regNumber} från garaget.[/]");
        }
    }

    public void MoveVehicle()
    {
        // Frågar användaren om registreringsnummer.
        string regNumber = AnsiConsole.Ask<string>("Ange registreringsnummer för fordonet som ska flyttas: ").ToUpper().Trim();

        string promptMassage = $"Ange målplatsnummer (1 till {_garage.Spots.Count}): ";
        int newSpotNumber = AnsiConsole.Ask<int>(promptMassage);

        int newSpotIndex = newSpotNumber - 1;

        if (newSpotIndex < 0 || newSpotIndex >= _garage.Spots.Count)
        {
            AnsiConsole.MarkupLine($"[red]Ogiltigt platsnummer: Måste vara mellan 1 och {_garage.Spots.Count}.[/]");
            return;
        }

        bool moveSuccess = _garage.MoveVehicle(regNumber, newSpotIndex);

        if (moveSuccess)
        {
            var panelContent = $"Fordonet [yellow]{regNumber}[/] har flyttats till plats [green]{newSpotNumber}[/]";
            AnsiConsole.Write(
                new Panel(panelContent)
                    .Border(BoxBorder.Double)
                    .BorderColor(Color.Green)
                    .Header("Flytt Lyckades ✅")
                    .Expand());

            GarageStateService.SaveGarageState(_garage);
        }
        else
        {
            AnsiConsole.MarkupLine($"[red]Flytta av fordonet {regNumber} till plats {newSpotNumber} misslyckades[/]");
            AnsiConsole.MarkupLine("[red]Kontrollera att fordonet finns och att målplatsen är ledig och lämplig för fordonstypen.[/]");
        }
    }

    public void FindVehicle()
    {
        // Frågar användare om registeringsnummer.
        string regNumber = AnsiConsole.Ask<string>("Ange registreringsnummer att söka efter: ").ToUpper().Trim();

        int? spotIndex = _garage.FindVehicleSpotIndex(regNumber);

        if (spotIndex.HasValue)
        {
            int spotNumberToShow = spotIndex.Value + 1;
            AnsiConsole.MarkupLine($"[green]Fordonet med registeringsnummer [bold]{regNumber}[/] hittades på plats [bold]{spotNumberToShow}[/].[/]");
        }
        else

        {
            AnsiConsole.MarkupLine($"[red]Fordonet med registreringsnummer [bold]{regNumber}[/] hittades inte i garaget.[/]");

        }
    }
    public void ShowGarage()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new FigletText("Garage Översikt").Centered().Color(Color.Blue));

        //Skapar tabell
        var table = new Table()
            .Border(TableBorder.Rounded)
            .Centered();

        //Lägger till kolumner
        for (int i = 0; i < 10; i++)
        {
            int start = i * 10 + 1;
            int end = (i + 1) * 10;
            table.AddColumn($"Plats {start}-{end}");
        }

        //Lägger till rader
        for (int row = 0; row < 10; row++)
        {
            var cells = new List<string>();

            for (int col = 0; col < 10; col++)
            {
                int spotIndex = col * 10 + row;
                int spotNumber = spotIndex + 1;

                if (spotIndex >= _garage.Spots.Count)
                {
                    cells.Add("[grey]---[/]");
                    continue;
                }

                var spot = _garage.Spots[spotIndex];

                if (spot.IsFree)
                {
                    cells.Add("[green]Ledig[/]");
                }
                else if (spot.ParkedVehicles.Count == 1 && spot.ParkedVehicles[0].Type == "MC")
                {
                    //En plats med en MC
                    var mc = spot.ParkedVehicles[0];
                    cells.Add($"[yellow]{mc.RegistrationNumber}[/]");
                }
                else if (spot.ParkedVehicles.Count == 2 && spot.ParkedVehicles.All(v => v.Type == "MC"))
                {
                    //En plats med två MC
                    string regs = string.Join(", ", spot.ParkedVehicles.Select(v => v.RegistrationNumber));
                    cells.Add($"[red]{regs}[/]");
                }
                else
                {
                    //En plats med en bil
                    var car = spot.ParkedVehicles[0];
                    cells.Add($"[red]{car.RegistrationNumber}[/]");

                }
            }

            table.AddRow(cells.ToArray());
        }

        //Skriver ut tabellen
        AnsiConsole.Write(table);

        var summary = new Panel(
            $"Totala platser: [bold]{_garage.TotalSpots}[/]\n" +
            $"Upptagna: [red]{_garage.OccupiedSpots}[/]\n" +
            $"Lediga: [green]{_garage.FreeSpots}[/]"
        )
            .Header("Sammanfattning")
            .Border(BoxBorder.Rounded)
            .BorderColor(Color.Blue)
            .Expand();

        AnsiConsole.Write(summary);
    }
}
