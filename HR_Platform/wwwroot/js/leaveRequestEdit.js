document.addEventListener("DOMContentLoaded", function () {
    const saveButton = document.getElementById("saveButton");
    const form = document.getElementById("editForm");

    if (saveButton && form) {
        saveButton.addEventListener("click", async function (event) {
            event.preventDefault(); // Previne trimiterea implicită a formularului

            const startDate = document.getElementById("StartDate").value;
            const endDate = document.getElementById("EndDate").value;
            const reason = document.getElementById("Reason").value;
            const requestId = document.querySelector('input[name="LeaveRequestID"]').value;
            const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

            try {
                const response = await fetch(`/LeaveRequests/Edit/${requestId}`, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'X-CSRF-TOKEN': token
                    },
                    body: JSON.stringify({
                        LeaveRequestID: requestId,
                        StartDate: startDate,
                        EndDate: endDate,
                        Reason: reason
                    })
                });

                if (response.ok) {
                    alert("Saved successfully!");
                    window.location.href = "/LeaveRequests/Index"; // Redirect după succes
                } else {
                    const error = await response.text();
                    alert("Error saving data: " + error);
                }
            } catch (err) {
                console.error("Error:", err);
                alert("An unexpected error occurred.");
            }
        });
    }
});
