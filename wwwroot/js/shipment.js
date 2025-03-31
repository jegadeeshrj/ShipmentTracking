$(document).ready(function () {
    loadShipments();

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("http://localhost:5175/shipmentHub")  // Ensure port matches your app
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
            document.getElementById("statusModal").classList.remove("show");
            document.body.classList.remove("modal-open");
            $(".modal-backdrop").remove();
            loadShipments();
        },
        error: function (xhr, status, error) {
            console.error("Error updating status:", status, error);
            console.error("Response:", xhr.responseText);
            alert("Status update " + (xhr.status === 500 ? "succeeded but email failed: " : "failed: ") + xhr.responseText);
            $("#statusModal").modal("hide");
            document.getElementById("statusModal").classList.remove("show");
            document.body.classList.remove("modal-open");
            $(".modal-backdrop").remove();
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