

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
connection.on("ReceiveMessage", data => {
   
    var lidata = document.createElement("li");
    document.getElementById("messagesList").appendChild(lidata);
    lidata.textContent = `${userNickName} ---> ${message}`;
});
connection.start();

