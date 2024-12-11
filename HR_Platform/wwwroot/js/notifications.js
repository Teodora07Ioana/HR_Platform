connection = new signalR.HubConnectionBuilder()
    .withUrl("/notifications")
    .build();


connection.on("ReceiveNotification", function (message) {
    console.log("Notificare primită:", message); // Log pentru verificare
    alert(message); // Sau afișează notificarea într-un container UI
})

/*connection.on("ReceiveNotification", function (message) {
    // Afișează mesajul într-un element HTML existent
    const notificationElement = document.getElementById("notificationText");
    if (notificationElement) {
        notificationElement.innerText = message;
    } else {
        console.error("Elementul HTML pentru notificări nu există.");
    }
});*/

/*connection.on("ReceiveNotification", function (message) {
    // Creează un container pentru notificări dacă nu există
    let notificationContainer = document.getElementById("notificationContainer");
    if (!notificationContainer) {
        notificationContainer = document.createElement("div");
        notificationContainer.id = "notificationContainer";
        notificationContainer.style.position = "fixed";
        notificationContainer.style.top = "10px";
        notificationContainer.style.right = "10px";
        notificationContainer.style.zIndex = "9999";
        notificationContainer.style.maxWidth = "300px";
        document.body.appendChild(notificationContainer);
    }

    // Creează notificarea
    const notification = document.createElement("div");
    notification.className = "notification success"; // Poți schimba în "error" pentru erori
    notification.innerText = message;

    // Adaugă un buton pentru a închide notificarea
    const closeButton = document.createElement("button");
    closeButton.innerText = "×";
    closeButton.style.float = "right";
    closeButton.style.border = "none";
    closeButton.style.background = "transparent";
    closeButton.style.cursor = "pointer";
    closeButton.style.fontSize = "16px";
    closeButton.style.marginLeft = "10px";
    closeButton.addEventListener("click", function () {
        notification.remove();
    });

    notification.appendChild(closeButton);

    // Adaugă notificarea în container
    notificationContainer.appendChild(notification);
});
*/
// Inițializează conexiunea SignalR
connection.start()
    .then(() => console.log("Connected to SignalR hub"))
    .catch(err => console.error("Error connecting to SignalR hub:", err));



connection.start()
    .then(() => console.log("Connected to SignalR hub"))
    .catch(err => console.error("Error connecting to SignalR hub:", err));