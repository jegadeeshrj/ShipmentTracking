using MongoDB.Driver;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;  // For Task return types
using System.Collections.Generic;  // For List<T>

public class ShipmentService : IShipmentService
{
    private readonly IMongoCollection<Shipment> _shipments;
    private readonly IEmailService _emailService;
    private readonly IHubContext<ShipmentHub> _hubContext;

    public ShipmentService(IMongoClient mongoClient, IEmailService emailService,
        IHubContext<ShipmentHub> hubContext)
    {
        var database = mongoClient.GetDatabase("ShipmentDB");
        _shipments = database.GetCollection<Shipment>("Shipments");
        _emailService = emailService;
        _hubContext = hubContext;
    }

    public async Task<List<Shipment>> GetAllShipments()
    {
        return await _shipments.Find(_ => true).ToListAsync();
    }

    public async Task<Shipment> GetShipmentById(string id)
    {
        return await _shipments.Find(s => s.Id == id).FirstOrDefaultAsync();
    }

    public async Task UpdateShipmentStatus(string id, ShipmentStatus status)
    {
        var shipment = await GetShipmentById(id);
        if (shipment != null)
        {
            shipment.StatusHistory.Add(status);
            await _shipments.ReplaceOneAsync(s => s.Id == id, shipment);

            // Send email notification
            if (!string.IsNullOrEmpty(shipment.CustomerEmail))
            {
                await _emailService.SendEmailAsync(shipment.CustomerEmail,
                    "Shipment Status Update",
                    $"Your shipment {shipment.TrackingNumber} is now {status.Status} at {status.Location}");
            }

            // Notify clients via SignalR
            await _hubContext.Clients.All.SendAsync("ReceiveShipmentUpdate", shipment);
        }
    }
}