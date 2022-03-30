using System.Text.Encodings.Web;
using System.Text.Json;

namespace pl_mongosh_script_gen
{
    public record Employee(int Id, string FirstName, string LastName, string Pesel, Contract Contract);

    public record Contract(string PositionName, DateTime SigningDate, DateTime ExpiryDate, float SalaryAmount);

    public record Route(int Id, BusStop[] BusStops, float Length, List<BusRide> BusRides);

    public record BusStop(int Id, string Name, Address Address);

    public record Address(string StreetName, string City, string ZipCode);

    public record BusRide(int Id, BusStop[] BusStops, BusRideEmployee Employee, DateTime StartTime, DateTime EndTime, float Length, int BusId);

    public record BusRideEmployee(int Id, string FirstName, string LastName)
    {
        public BusRideEmployee(Employee employee) : this(employee.Id, employee.FirstName, employee.LastName)
        { }
    }

    public record Client(int Id, string FirstName, string LastName, string Email, string Login, string Password, List<int> TransactionIds);

    public record Transaction(int Id, string PaymentMethod, DateTime PurchaseDate, Ticket[] Tickets, string Status, int ClientId, string ClientEmail)
    {
        public decimal TotalPrice => decimal.Round(new decimal(Tickets.Select(t => t.PriceAfterDiscount).Sum()), 2);
    }

    public record Ticket(int Id, BusRide BusRide, float BasePrice, float PriceAfterDiscount, string DiscountName);

    public record Bus(int Id, int ProductionYear, string Model, int TankCapacity, int SeatsCount, string Condition);

    public class Program
    {
        public static void Main()
        {
            //GenerateJsonsNaively();
            SimulateDatabaseAndGenerateJsons();
        }

        private static void SimulateDatabaseAndGenerateJsons()
        {
            IDatabaseService db = new FakeDatabaseService();
            db.RegisterEmployee("Patryk", "Maj", "97237784563", new Contract("Kierowca", new(2021, 3, 13), new(2025, 3, 13), 4200));
            db.RegisterEmployee("Filip", "Lipiec", "95252452785", new Contract("Kierowca", new(2020, 5, 24), new(2023, 5, 24), 4300));
            db.RegisterEmployee("Dawid", "Dudek", "93134574891", new Contract("Administrator", new(2018, 1, 1), new(2028, 1, 1), 5500));
            db.RegisterEmployee("Kamil", "Goch", "91114790263", new Contract("Mechanik", new(2020, 5, 12), new(2024, 5, 12), 3800));
            db.RegisterEmployee("Kinga", "Kowalska", "00278934594", new Contract("Księgowa", new(2018, 3, 18), new(2023, 3, 18), 4000));

            db.RegisterBus(1998, "Mercedes", 240, 55, "Niesprawny");
            db.RegisterBus(2004, "MAN", 300, 65, "Sprawny");
            db.RegisterBus(2002, "Iveco", 280, 60, "Niesprawny");
            db.RegisterBus(2010, "Mercedes", 350, 70, "Sprawny");
            db.RegisterBus(2018, "Scania", 180, 50, "Sprawny");

            db.RegisterClient("Andrzej", "Kowalski", "andrzejoK123@gmail.com", "AK123", "haslo321");
            db.RegisterClient("Marta", "Nowak", "mnowak@gmail.com", "martaaa", "qwerty.1");
            db.RegisterClient("Zbigniew", "Wodecki", "zbigoAmigo@wp.pl", "zibi3", "pass00");
            db.RegisterClient("Katarzyna", "Babicka", "babik123@wp.pl", "kaba2", "zaqzaq123");
            db.RegisterClient("Andrzej", "Król", "andrzejkrol@gmail.com", "andrzejK", "andrzejo320");

            db.RegisterBusStop("Lubelska 1", new Address("Lubelska", "Puławy", "24-100"));
            db.RegisterBusStop("Ruska 2", new Address("Ruska", "Lublin", "20-126"));
            db.RegisterBusStop("Słoneczna 2", new Address("Słoneczna", "Warszawa", "00-789"));
            db.RegisterBusStop("Piłsudskiego 1", new Address("Piłsudskiego", "Bychawa", "23-100"));
            db.RegisterBusStop("Sienkiewicza 1", new Address("Sienkiewicza", "Kielce", "25-501"));
            var busStops = (IReadOnlyList<BusStop>)db.GetBusStops().ToList();

            db.RegisterRoute(new BusStop[] { busStops[0], busStops[1], busStops[2] }, 56);
            db.RegisterRoute(new BusStop[] { busStops[1], busStops[2], busStops[3] }, 77);
            db.RegisterRoute(new BusStop[] { busStops[4], busStops[3], busStops[2], busStops[0] }, 241);
            db.RegisterRoute(new BusStop[] { busStops[2], busStops[1] }, 43);
            db.RegisterRoute(new BusStop[] { busStops[0], busStops[4], busStops[3], busStops[2], busStops[1] }, 556);

            var routes = (IReadOnlyList<Route>)db.GetRoutes().ToList();
            var employees = (IReadOnlyList<Employee>)db.GetEmployees().ToList();
            var buses = (IReadOnlyList<Bus>)db.GetBuses().ToList();
            var ride1 = db.AddRide(routes[0], new BusStop[] { busStops[0], busStops[1], busStops[2] }, employees[0], new(2021, 02, 02, 15, 5, 0), new(2021, 02, 02, 16, 0, 0), buses[0]);
            var ride2 = db.AddRide(routes[0], new BusStop[] { busStops[0], busStops[1], busStops[2] }, employees[1], new(2021, 02, 03, 16, 5, 0), new(2021, 02, 03, 17, 0, 0), buses[1]);
            var ride3 = db.AddRide(routes[1], new BusStop[] { busStops[1], busStops[2], busStops[3] }, employees[2], new(2021, 02, 04, 17, 5, 0), new(2021, 02, 04, 18, 0, 0), buses[2]);
            var ride4 = db.AddRide(routes[1], new BusStop[] { busStops[1], busStops[2], busStops[3] }, employees[3], new(2021, 02, 05, 18, 5, 0), new(2021, 02, 05, 19, 0, 0), buses[3]);
            var ride5 = db.AddRide(routes[2], new BusStop[] { busStops[4], busStops[3], busStops[2], busStops[0] }, employees[4], new(2021, 02, 06, 19, 5, 0), new(2021, 02, 06, 20, 0, 0), buses[4]);
            db.AddRide(routes[2], new BusStop[] { busStops[4], busStops[3], busStops[2], busStops[0] }, employees[0], new(2021, 02, 07, 20, 5, 0), new(2021, 02, 07, 21, 0, 0), buses[0]);
            db.AddRide(routes[3], new BusStop[] { busStops[2], busStops[1] }, employees[1], new(2021, 02, 08, 21, 5, 0), new(2021, 02, 08, 22, 0, 0), buses[1]);
            db.AddRide(routes[3], new BusStop[] { busStops[2], busStops[1] }, employees[2], new(2021, 02, 09, 22, 5, 0), new(2021, 02, 09, 23, 0, 0), buses[2]);
            db.AddRide(routes[4], new BusStop[] { busStops[0], busStops[4], busStops[3], busStops[2], busStops[1] }, employees[3], new(2021, 02, 10, 23, 5, 0), new(2021, 02, 11, 00, 0, 0), buses[3]);
            db.AddRide(routes[4], new BusStop[] { busStops[0], busStops[4], busStops[3], busStops[2], busStops[1] }, employees[4], new(2021, 02, 12, 0, 5, 0), new(2021, 02, 12, 01, 0, 0), buses[4]);

            db.CreateTicket(ride1, 300, .51f, "Ulga studencka (51%)");
            db.CreateTicket(ride1, 300, .49f, "Ulga uczniowska (49%)");
            db.CreateTicket(ride2, 150, .78f, "Ulga dziecko niepełnosprawne (78%)");
            db.CreateTicket(ride3, 90, .37f, "Ulga kombatanta (37%)");
            db.CreateTicket(ride4, 200, .33f, "Ulga nauczyciela (33%)");
            db.CreateTicket(ride5, 200, .51f, "Ulga studencka (51%)");

            var clients = (IReadOnlyList<Client>)db.GetClients().ToList();
            var tickets = (IReadOnlyList<Ticket>)db.GetTickets().ToList();
            db.MakeTransaction("Karta kredytowa", new(2021, 2, 3), new[] { tickets[0] }, "Zrealizowane", clients[0]);
            db.MakeTransaction("Gotówka", new(2022, 3, 14), new[] { tickets[3] }, "Zrealizowane", clients[1]);
            db.MakeTransaction("Blik", new(2022, 2, 14), new[] { tickets[1] }, "Odmowa", clients[2]);
            db.MakeTransaction("Karta kredytowa", new(2022, 1, 12), new[] { tickets[2] }, "Zwrócono", clients[3]);
            db.MakeTransaction("Blik", new(2022, 2, 16), new[] { tickets[4], tickets[5] }, "Zrealizowane", clients[4]);

            WriteJsons(employees, buses, clients, busStops, routes, tickets, db.GetTransactions());
        }

        private static void GenerateJsonsNaively()
        {
            var employees = new Employee[]
                        {
                new(1, "Patryk", "Maj", "97237784563", new Contract("Kierowca", new(2021, 3, 13), new(2025, 3, 13), 4200)),
                new(2, "Filip", "Lipiec", "95252452785", new Contract("Kierowca", new(2020, 5, 24), new(2023, 5, 24), 4300)),
                new(3, "Dawid", "Dudek", "93134574891", new Contract("Administrator", new(2018, 1, 1), new(2028, 1, 1), 5500)),
                new(4, "Kamil", "Goch", "91114790263", new Contract("Mechanik", new(2020, 5, 12), new(2024, 5, 12), 3800)),
                new(5, "Kinga", "Kowalska", "00278934594", new Contract("Księgowa", new(2018, 3, 18), new(2023, 3, 18), 4000))
                        };

            var buses = new Bus[]
            {
                new(1, 1998, "Mercedes", 240, 55, "Niesprawny"),
                new(2, 2004, "MAN", 300, 65, "Sprawny"),
                new(3, 2002, "Iveco", 280, 60, "Niesprawny"),
                new(4, 2010, "Mercedes", 350, 70, "Sprawny"),
                new(5, 2018, "Scania", 180, 50, "Sprawny"),
            };

            var clients = new Client[]
            {
                new(1, "Andrzej", "Kowalski", "andrzejoK123@gmail.com", "AK123", "haslo321", new()),
                new(2, "Marta", "Nowak", "mnowak@gmail.com", "martaaa", "qwerty.1", new()),
                new(3, "Zbigniew", "Wodecki", "zbigoAmigo@wp.pl", "zibi3", "pass00", new()),
                new(4, "Katarzyna", "Babicka", "babik123@wp.pl", "kaba2", "zaqzaq123", new()),
                new(5, "Andrzej", "Król", "andrzejkrol@gmail.com", "andrzejK", "andrzejo320", new()),
            };

            var busStops = new BusStop[]
            {
                new(1, "Lubelska 1", new Address("Lubelska", "Puławy", "24-100")),
                new(2, "Ruska 2", new Address("Ruska", "Lublin", "20-126")),
                new(3, "Słoneczna 2", new Address("Słoneczna", "Warszawa", "00-789")),
                new(4, "Piłsudskiego 1", new Address("Piłsudskiego", "Bychawa", "23-100")),
                new(5, "Sienkiewicza 1", new Address("Sienkiewicza", "Kielce", "25-501")),
            };

            var rides = new BusRide[]
            {
                new(1, new BusStop[] {busStops[0], busStops[1], busStops[2]}, new(employees[0]), new(2021, 02, 02, 15, 5, 0), new(2021, 02, 02, 16, 0, 0), 56, 1),
                new(2, new BusStop[] {busStops[0], busStops[1], busStops[2]}, new(employees[1]), new(2021, 02, 03, 16, 5, 0), new(2021, 02, 03, 17, 0, 0), 56, 2),
                new(3, new BusStop[] {busStops[1], busStops[2], busStops[3]}, new(employees[2]), new(2021, 02, 04, 17, 5, 0), new(2021, 02, 04, 18, 0, 0), 77, 3),
                new(4, new BusStop[] {busStops[1], busStops[2], busStops[3]}, new(employees[3]), new(2021, 02, 05, 18, 5, 0), new(2021, 02, 05, 19, 0, 0), 77, 4),
                new(5, new BusStop[] {busStops[4], busStops[3], busStops[2], busStops[0]}, new(employees[4]), new(2021, 02, 06, 19, 5, 0), new(2021, 02, 06, 20, 0, 0), 241, 5),
                new(6, new BusStop[] {busStops[4], busStops[3], busStops[2], busStops[0]}, new(employees[0]), new(2021, 02, 07, 20, 5, 0), new(2021, 02, 07, 21, 0, 0), 241, 1),
                new(7, new BusStop[] {busStops[2], busStops[1]}, new(employees[1]), new(2021, 02, 08, 21, 5, 0), new(2021, 02, 08, 22, 0, 0), 43, 2),
                new(8, new BusStop[] {busStops[2], busStops[1]}, new(employees[2]), new(2021, 02, 09, 22, 5, 0), new(2021, 02, 09, 23, 0, 0), 43, 3),
                new(9, new BusStop[] {busStops[0], busStops[4], busStops[3], busStops[2], busStops[1]}, new(employees[3]), new(2021, 02, 10, 23, 5, 0), new(2021, 02, 11, 00, 0, 0), 556, 4),
                new(10, new BusStop[] {busStops[0], busStops[4], busStops[3], busStops[2], busStops[1]}, new(employees[4]), new(2021, 02, 12, 0, 5, 0), new(2021, 02, 12, 01, 0, 0), 556, 5),
            };

            var routes = new Route[]
            {
                new(1, new BusStop[] {busStops[0], busStops[1], busStops[2]}, 56, new() {rides[0], rides[1]}),
                new(2, new BusStop[] {busStops[1], busStops[2], busStops[3]}, 77, new() {rides[2], rides[3]}),
                new(3, new BusStop[] {busStops[4], busStops[3], busStops[2], busStops[0]}, 241, new() {rides[4], rides[5]}),
                new(4, new BusStop[] {busStops[2], busStops[1]}, 43, new() {rides[6], rides[7]}),
                new(5, new BusStop[] {busStops[0], busStops[4], busStops[3], busStops[2], busStops[1]}, 556, new() {rides[8], rides[9]}),
            };

            var tickets = new Ticket[]
            {
                new(1, rides[0], 300, 300 * .49f, "Ulga studencka (51%)"),
                new(2, rides[0], 300, 300 * .51f, "Ulga uczniowska (49%)"),
                new(3, rides[1], 150, 150 * .22f, "Ulga dziecko niepełnosprawne (78%)"),
                new(4, rides[2], 90, 90 * .63f, "Ulga kombatanta (37%)"),
                new(5, rides[3], 200, 200 * .67f, "Ulga nauczyciela (33%)"),
                new(6, rides[4], 200, 200 * .49f, "Ulga studencka (51%)")
            };

            var transactions = new Transaction[]
            {
                new(1, "Karta kredytowa", new(2021, 2, 3), new[] { tickets[0] }, "Zrealizowane", 1, clients[0].Email),
                new(2, "Gotówka", new(2022, 3, 14), new[] { tickets[3] }, "Zrealizowane", 2, clients[1].Email),
                new(3, "Blik", new(2022, 2, 14), new[] { tickets[1] }, "Odmowa", 3, clients[2].Email),
                new(4, "Karta kredytowa", new(2022, 1, 12), new[] { tickets[2] }, "Zwrócono", 4, clients[3].Email),
                new(5, "Blik", new(2022, 2, 16), new[] { tickets[4], tickets[5] }, "Zrealizowane", 5, clients[4].Email),

            };

            WriteJsons(employees, buses, clients, busStops, routes, tickets, transactions);
        }

        private static void WriteJsons(IEnumerable<Employee> employees, IEnumerable<Bus> buses, IEnumerable<Client> clients, IEnumerable<BusStop> busStops, IEnumerable<Route> routes, IEnumerable<Ticket> tickets, IEnumerable<Transaction> transactions)
        {
            WriteJson("Employees", employees);
            WriteJson("Routes", routes);
            WriteJson("Clients", clients);
            WriteJson("Tickets", tickets);
            WriteJson("BusStops", busStops);
            WriteJson("Transactions", transactions);
            WriteJson("Buses", buses);
        }

        private static void WriteJson<T>(string name, T obj)
        {
            Directory.CreateDirectory("jsons");
            var serializerOptions = new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
            File.WriteAllText($"jsons/{name}.json", JsonSerializer.Serialize(obj, serializerOptions));
        }
    }
}
