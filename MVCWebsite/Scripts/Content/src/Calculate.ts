$(document).ready(function () {
    $("#EquationButton").click(() => {
        const equation = $("#EquationInput").val();

        console.log(equation);
        const socket = new WebSocket('ws://localhost:650');

        socket.addEventListener('open', function (event) {
            socket.send('Hello Server!');
        });
        socket.addEventListener('message', function (event) {
            console.log('Message from server ', event.data);
        });
    })
});