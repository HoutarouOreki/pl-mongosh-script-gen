namespace pl_mongosh_script_gen
{
    public class FakeDatabaseService : IDatabaseService
    {
        private readonly List<Client> clients = new();
        private readonly List<Employee> employees = new();
        private readonly List<Route> routes = new();
        private readonly List<Transaction> transactions = new();
        private readonly List<BusStop> busStops = new();
        private readonly List<Ticket> tickets = new();
        private readonly List<Bus> buses = new();

        private static int GenerateId<T>(IEnumerable<T> collection, Func<T, int> ids)
        {
            if (!collection.Any())
                return 1;
            return collection.Max(ids) + 1;
        }

        public BusRide AddRide(Route route, IEnumerable<BusStop> busStops, Employee employee, DateTime startDate, DateTime endDate, Bus bus)
        {
            // wszystkie przystanki powinny być zawarte w podstawowej trasie
            if (!busStops.All(bs => route.BusStops.Contains(bs)) || busStops.Count() > route.BusStops.Length)
                throw new Exception("Nie wszystkie przystanki kursu są zawarte w trasie.");

            if (endDate < startDate)
                throw new Exception("Data zakończenia kursu jest przed datą jego rozpoczęcia");

            var rideLength = route.Length * (busStops.Count() / (float)route.BusStops.Length);
            var ride = new BusRide(GenerateId(routes.SelectMany(r => r.BusRides), r => r.Id), busStops.ToArray(), new(employee), startDate, endDate, rideLength, bus.Id);
            route.BusRides.Add(ride);
            return ride;
        }

        public Ticket CreateTicket(BusRide busRide, float basePrice, float discountPercent, string discountName)
        {
            var id = GenerateId(tickets, t => t.Id);
            var ticket = new Ticket(id, busRide, basePrice, basePrice * (1 - discountPercent), discountName);
            tickets.Add(ticket);
            return ticket;
        }

        public IEnumerable<Bus> GetBuses() => buses;

        public IEnumerable<BusStop> GetBusStops() => busStops;

        public IEnumerable<Client> GetClients() => clients;

        public IEnumerable<Employee> GetEmployees() => employees;

        public IEnumerable<Route> GetRoutes() => routes;

        public IEnumerable<Ticket> GetTickets() => tickets;

        public IEnumerable<Transaction> GetTransactions() => transactions;

        public Transaction MakeTransaction(string paymentMethod, DateTime purchaseDate, IEnumerable<Ticket> tickets, string status, Client client)
        {
            var id = GenerateId(transactions, t => t.Id);
            var transaction = new Transaction(id, paymentMethod, purchaseDate, tickets.ToArray(), status, client.Id, client.Email);
            client.TransactionIds.Add(id);
            transactions.Add(transaction);
            return transaction;
        }

        public Bus RegisterBus(int productionYear, string model, int tankCapacity, int seatsCount, string condition)
        {
            var id = GenerateId(buses, b => b.Id);
            var bus = new Bus(id, productionYear, model, tankCapacity, seatsCount, condition);
            buses.Add(bus);
            return bus;
        }

        public BusStop RegisterBusStop(string name, Address address)
        {
            var id = GenerateId(busStops, bs => bs.Id);
            var busStop = new BusStop(id, name, address);
            busStops.Add(busStop);
            return busStop;
        }

        public Client RegisterClient(string firstName, string lastName, string email, string login, string password)
        {
            var id = GenerateId(clients, c => c.Id);
            var client = new Client(id, firstName, lastName, email, login, password, new());
            clients.Add(client);
            return client;
        }

        public Employee RegisterEmployee(string firstName, string lastName, string pesel, Contract contract)
        {
            var id = GenerateId(employees, e => e.Id);
            var employee = new Employee(id, firstName, lastName, pesel, contract);
            employees.Add(employee);
            return employee;
        }

        public Route RegisterRoute(IEnumerable<BusStop> busStops, float length)
        {
            var id = GenerateId(routes, r => r.Id);
            var route = new Route(id, busStops.ToArray(), length, new());
            routes.Add(route);
            return route;
        }
    }
}
