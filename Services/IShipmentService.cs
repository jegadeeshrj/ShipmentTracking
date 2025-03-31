using System.Collections.Generic;  // For List<T>
using System.Threading.Tasks;  // For Task return types

public interface IShipmentService
{
    Task<List<Shipment>> GetAllShipments();
    Task<Shipment> GetShipmentById(string id);
    Task UpdateShipmentStatus(string id, ShipmentStatus status);
}

