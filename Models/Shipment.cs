using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;  // For DateTime
using System.Collections.Generic;  // For List<T>

public class Shipment
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public string? TrackingNumber { get; set; }
    public string? CustomerEmail { get; set; }
    public List<ShipmentStatus> StatusHistory { get; set; } = new List<ShipmentStatus>();
    public DateTime CreatedDate { get; set; }
}

public class ShipmentStatus
{
    public string? Status { get; set; }
    public string? Location { get; set; }
    public DateTime Timestamp { get; set; }
}