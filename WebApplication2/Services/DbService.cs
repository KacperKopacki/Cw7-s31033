using Microsoft.Data.SqlClient;
using WebApp.Models;

namespace WebApp.Services;

public interface IDbService
{
    public Task<IEnumerable<TripGetDTO>> GetAllTrips(); //pobiera wszystkie wycieczki i kraje
    public Task<IEnumerable<ClientTripGetDTO>> GetClientTrips(int id); // pobiera wycieczki do klienta
    
    public Task<int> AddClient(ClientPostDTO dto); // dodaje nowe klienta i daje id
}

public class DbService(IConfiguration config) : IDbService //edpoint get, pobiera wszystkie wycieczki z przypisanymi krajami
{
    public async Task<IEnumerable<TripGetDTO>> GetAllTrips()
    {
        var result = new Dictionary<int, TripGetDTO>();
        
        var connectionString = config.GetConnectionString("Default");
        await using var connection = new SqlConnection(connectionString);
        // zapytanie - pobranie wszystkich wycieczek z przypisanymi krajami
        var sql = "SELECT t.IdTrip, t.Name, t.Description, t.DateFrom, t.DateTo, t.MaxPeople, c.Name AS CountryName FROM Trip t JOIN Country_Trip ct ON ct.IdTrip = t.IdTrip JOIN Country c ON c.IdCountry = ct.IdCountry";
        
        await using var command = new SqlCommand(sql, connection);
        await connection.OpenAsync();
        
        await using var reader = await command.ExecuteReaderAsync();
        
        // mapowanie wynikow

        while (await reader.ReadAsync())
        {
            int idTrip = reader.GetInt32(0);
            if (!result.ContainsKey(idTrip))
            {
                result.Add(idTrip, new TripGetDTO
                {
                    IdTrip = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Description = reader.GetString(2),
                    DateFrom = reader.GetDateTime(3),
                    DateTo = reader.GetDateTime(4),
                    MaxPeople = reader.GetInt32(5),
                    Countries = new List<string>()
                });
            }

            result[idTrip].Countries.Add(reader.GetString(6));
        }
        return result.Values;
    }
    

    public async Task<IEnumerable<ClientTripGetDTO>> GetClientTrips(int id) // edpoint get  pobiera wszystkie wycieczki danego klienta
    {
        var result = new List<ClientTripGetDTO>();
        
        var connectionString = config.GetConnectionString("Default");
        await using var connection = new SqlConnection(connectionString);
        // zapytanie - pobranie wycieczek w ktorych klient bierze udzial
        var sql = "SELECT t.Name, t.Description, t.DateFrom, t.DateTo, t.MaxPeople, ct.RegisteredAt, ct.PaymentDate FROM Client c JOIN Client_Trip ct ON ct.IdClient = c.IdClient JOIN Trip t ON t.IdTrip = ct.IdTrip WHERE c.IdClient = @IdClient";
        
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@IdClient", id);
        
        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var registeredAt = reader.GetInt32(5);
            string paymentDate = reader.IsDBNull(6) ? "brak" : reader.GetInt32(6).ToString();
            result.Add(new ClientTripGetDTO
            {
                TripName = reader.GetString(0),
                TripDescription = reader.GetString(1),
                DateFrom = reader.GetDateTime(2),
                DateTo = reader.GetDateTime(3),
                MaxPeople = reader.GetInt32(4),
                RegisteredAt = registeredAt,
                PaymentDate = paymentDate
            });
        }

        return result;
    }

    public async Task<int> AddClient(ClientPostDTO dto) //edpoint post, dodanie nowego klienta oraz zworcenie jego id
    {
        var connectionString = config.GetConnectionString("Default");
        await using var connection = new SqlConnection(connectionString);
        // wstawienie nowego pracownika i zwrocenie jego id
        var sql = "INSERT INTO Client (FirstName, LastName, Email, Telephone, Pesel) OUTPUT INSERTED.IdClient VALUES (@FirstName, @LastName, @Email, @Telephone, @Pesel)";
        
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@FirstName", dto.FirstName);
        command.Parameters.AddWithValue("@LastName", dto.LastName);
        command.Parameters.AddWithValue("@Email", dto.Email);
        command.Parameters.AddWithValue("@Telephone", dto.Telephone);
        command.Parameters.AddWithValue("@Pesel", dto.Pesel);
        await connection.OpenAsync();
        
        var inserted = (int)await command.ExecuteScalarAsync();
        return inserted;
    }
}