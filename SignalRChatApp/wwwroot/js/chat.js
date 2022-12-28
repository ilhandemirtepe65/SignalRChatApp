

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
connection.on("ReceiveMessage", function (user, message)
{
    var lidata = document.createElement("li");
    document.getElementById("messagesList").appendChild(lidata);
    lidata.textContent = `${user} ---> ${message}`;
});
connection.start();
function sendMessage() {
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
    connection.invoke("SendMessage", user, message).catch(function (err) {
        return console.error(err.toString());
    });
}
