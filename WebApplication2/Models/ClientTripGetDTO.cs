namespace WebApp.Models;

public class ClientTripGetDTO
{
    public string TripName { get; set; }
    public string TripDescription { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public int MaxPeople { get; set; }
    public int RegisteredAt { get; set; }
    public string PaymentDate { get; set; }
}