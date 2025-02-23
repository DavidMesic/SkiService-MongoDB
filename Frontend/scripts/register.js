document.getElementById('registerForm').addEventListener('submit', function (event) {
    console.log("Form submit detected");
    event.preventDefault();

    // Werte aus dem Formular abrufen
    const username = document.getElementById('username').value;
    const password = document.getElementById('password').value;
    const email = document.getElementById('email').value;
    var phone = document.getElementById('phone').value;

    if (!phone)
    {
        phone = null;
    }



    // Erstellung des Objekts für die Anmeldung
    const registerAccount = {
        username,
        password,
        email,
        phone
    };



    // Anfrage an den Server senden
    fetch('https://localhost:7290/api/Account/create', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(registerAccount)
    })
        .then(response => response.json())
        .then(data => {
            console.log('Serverantwort:', data);
        })
        .catch(error => {
            console.error('Error:', error);
            alert('Es gab ein Problem mit der Registration.');
        });
});