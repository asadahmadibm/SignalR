// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
const connection = new signalR.HubConnectionBuilder()
    .withUrl("http://signalrmvc.tnlink.ir/chat")
    .configureLogging(signalR.LogLevel.Information)
    .build();


async function start() {
    try {
        await connection.start();
        console.log("SignalR Connected.");
    } catch (err) {
        console.log(err);
        setTimeout(start, 5000);
    }
};

connection.onclose(async () => {
    await start();
});

start();

connection.on("ConcurrentJobs", function (message) {
    var li = document.createElement("li");
    document.getElementById("concurrentJobs").appendChild(li);
    li.textContent = `${message}`;
});

connection.on("NonConcurrentJobs", function (message) {
    var li = document.createElement("li");
    document.getElementById("nonConcurrentJobs").appendChild(li);
    li.textContent = `${message}`;
});


connection.on('newMessage', (sender, messageText) => {
    console.log(`${sender}:${messageText}`);

    const newMessage = document.createElement('li');
    newMessage.appendChild(document.createTextNode(`${sender}:${messageText}`));
    messages.appendChild(newMessage);
});


connection.on("spanupdate", function (spannumber ,amount) {
    //if (spannumber == 1) {
    //    const spannelement = document.getElementById('span1');
    //    $("#span1").html("testing <b>1 2 3</b>");
    //}
    switch (spannumber) {
        case 1:
            $("#span1").html("<b>" + amount + "</b>");
            break;
        case 2:
            $("#span2").html("<b>" + amount + "</b>");
            break;
        case 3:
            $("#span3").html("<b>" + amount + "</b>");
            break;
        case 4:
            $("#span4").html("<b>" + amount + "</b>");
            break;
        case 5:
            $("#span5").html("<b>" + amount + "</b>");
            break;
        case 6:
            $("#span6").html("<b>" + amount +"</b>");
            break;
    }
    console.log("aaaaaaaaaaa" + `${spannumber}:${amount}`);
});



messageForm.addEventListener('submit', ev => {
    ev.preventDefault();
    const message = messageBox.value;
    connection.invoke('SendMessage', message);
    messageBox.value = '';
});