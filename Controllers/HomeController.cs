using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{
    private readonly IShipmentService _shipmentService;

    public HomeController(IShipmentService shipmentService)
    {
        _shipmentService = shipmentService;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> CustomerDashboard(string trackingNumber)
    {
        var shipments = await _shipmentService.GetAllShipments();
        var shipment = shipments.FirstOrDefault(s => s.TrackingNumber == trackingNumber);
        return View(shipment);
    }
}