using Microsoft.AspNetCore.Mvc;  // For ControllerBase and routing attributes
using System.Threading.Tasks;  // For Task return types
using Microsoft.Extensions.Logging;  // For ILogger

[Route("api/[controller]")]
[ApiController]
public class ShipmentController : ControllerBase
{
    private readonly IShipmentService _shipmentService;
    private readonly ILogger<ShipmentController> _logger;

    public ShipmentController(IShipmentService shipmentService, ILogger<ShipmentController> logger)
    {
        _shipmentService = shipmentService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        _logger.LogInformation("GET all shipments requested at {Time}", DateTime.Now);
        var shipments = await _shipmentService.GetAllShipments();
        _logger.LogInformation("Returning {Count} shipments", shipments.Count);
        return Ok(shipments);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        _logger.LogInformation("GET shipment {Id} requested at {Time}", id, DateTime.Now);
        var shipment = await _shipmentService.GetShipmentById(id);
        return Ok(shipment);
    }

    [HttpPost("{id}/status")]
    public async Task<IActionResult> UpdateStatus(string id, [FromBody] ShipmentStatus status)
    {
        _logger.LogInformation("POST status update for shipment {Id} at {Time}", id, DateTime.Now);
        await _shipmentService.UpdateShipmentStatus(id, status);
        _logger.LogInformation("Status updated successfully for shipment {Id}", id);
        return Ok();
    }
}