$(document).ready(function () {
    loadShipments();

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/shipmentHub")
        .build();

    connection.on("ReceiveShipmentUpdate", function (shipment) {
        updateCustomerDashboard(shipment);
    });

    connection.start().catch(err => console.error(err));
});

function loadShipments() {
    $.get("/api/shipment", function (data) {
        let html = '';
        data.forEach(s => {
            const latestStatus = s.statusHistory[s.statusHistory.length - 1];
            html += `
                <tr>
                    <td>${s.trackingNumber}</td>
                    <td>${latestStatus.status} - ${latestStatus.location}</td>
                    <td>${s.customerEmail}</td>
                    <td>
                        <button onclick="showStatusModal('${s.id}')" class="btn btn-sm btn-primary">Update</button>
                    </td>
                </tr>`;
        });
        $("#shipmentGrid").html(html);
    });
}

function showStatusModal(id) {
    $("#shipmentId").val(id);
    $("#statusModal").modal("show");
}

function updateStatus() {
    const id = $("#shipmentId").val();
    const status = {
        status: $("#statusSelect").val(),
        location: $("#locationInput").val(),
        timestamp: new Date().toISOString()
    };

    $.ajax({
        url: `/api/shipment/${id}/status`,
        method: "POST",
        contentType: "application/json",
        data: JSON.stringify(status),
        success: function () {
            $("#statusModal").modal("hide");
            loadShipments();
        }
    });
}

function updateCustomerDashboard(shipment) {
    const latestStatus = shipment.statusHistory[shipment.statusHistory.length - 1];
    $("#statusUpdates").append(`
        <div class="alert alert-info">
            ${latestStatus.status} - ${latestStatus.location} (${new Date(latestStatus.timestamp).toLocaleString()})
        </div>
    `);
}