namespace pl_mongosh_script_gen
{
    public interface IDatabaseService
    {
        Client RegisterClient(string firstName, string lastName, string email, string login, string password);

        Employee RegisterEmployee(string firstName, string lastName, string pesel, Contract contract);

        BusStop RegisterBusStop(string name, Address address);

        Route RegisterRoute(IEnumerable<BusStop> busStops, float length);

        BusRide AddRide(Route route, IEnumerable<BusStop> busStops, Employee employee, DateTime startDate, DateTime endDate, Bus bus);

        Transaction MakeTransaction(string paymentMethod, DateTime purchaseDate, IEnumerable<Ticket> tickets, string status, Client client);

        Ticket CreateTicket(BusRide busRide, float basePrice, float discountPercent, string discountName);

        Bus RegisterBus(int productionYear, string model, int tankCapacity, int seatsCount, string condition);

        IEnumerable<Bus> GetBuses();

        IEnumerable<Employee> GetEmployees();

        IEnumerable<Route> GetRoutes();

        IEnumerable<Client> GetClients();

        IEnumerable<Transaction> GetTransactions();

        IEnumerable<BusStop> GetBusStops();

        IEnumerable<Ticket> GetTickets();
    }
}
