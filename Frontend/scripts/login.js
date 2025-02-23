document.getElementById('loginForm').addEventListener('submit', function (event) {
    console.log("Form submit detected");
    event.preventDefault();

    // Werte aus dem Formular abrufen
    const email = document.getElementById('email').value;
    const password = document.getElementById('password').value;

    // Erstellung des Objekts für die Anmeldung
    const loginAccount = {
        email,
        password
    };

    console.log('Zu sendende Daten:', JSON.stringify(loginAccount));

    // Anfrage an den Server senden
    fetch('https://localhost:7290/api/Login', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(loginAccount)
    })
    .then(response => response.json())
    .then(data => {
        console.log('Serverantwort (Text):', data);

        const jwt = data.jwt;
        const accountId = data.accountId;

        // Speichern der AccountID in localStorage oder einer globalen Variablen
        window.sessionStorage.setItem('accountId', accountId);  // AccountID speichern
        console.log('AcoountID: ', accountId);

        showJwtPopup(jwt);
    })
    .catch(error => {
        console.error('Fehler:', error);
        alert('Es gab ein Problem bei der Anmeldung: ' + error.message);
    });
});

// Funktion zum Öffnen des Modals und Anzeigen des JWT
function showJwtPopup(jwt) {
    const jwtText = document.getElementById('jwtText');
    jwtText.textContent = jwt;  // JWT in das Textfeld einfügen

    const jwtModal = new bootstrap.Modal(document.getElementById('jwtModal'));
    jwtModal.show();  // Modal anzeigen
}