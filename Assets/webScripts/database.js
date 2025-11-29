
    // Import the functions you need from the SDKs you need
    import { initializeApp } from "https://www.gstatic.com/firebasejs/12.6.0/firebase-app.js";
    // TODO: Add SDKs for Firebase products that you want to use
    // https://firebase.google.com/docs/web/setup#available-libraries
    import { getDatabase, ref, child, get, set } from "https://www.gstatic.com/firebasejs/12.6.0/firebase-database.js";
    import { getAuth, createUserWithEmailAndPassword, signInWithEmailAndPassword, onAuthStateChanged, signOut } from "https://www.gstatic.com/firebasejs/12.6.0/firebase-auth.js";

    // Your web app's Firebase configuration
    const firebaseConfig = {
        apiKey: "AIzaSyCeUj6qc8vuiyPNuHInyNYtwHGJLFNsnVk",
        authDomain: "itd-asg1-yakuntoast.firebaseapp.com",
        databaseURL: "https://itd-asg1-yakuntoast-default-rtdb.asia-southeast1.firebasedatabase.app",
        projectId: "itd-asg1-yakuntoast",
        storageBucket: "itd-asg1-yakuntoast.firebasestorage.app",
        messagingSenderId: "645369306165",
        appId: "1:645369306165:web:836a5413da348b0f4ee57c"
    };
    // Initialize Firebase
    const app = initializeApp(firebaseConfig);
    const db = getDatabase(app);
    const players = ref(db, "users");
    
    function getPlayerData() {
        get(players).then((snapshot) => {
            if (snapshot.exists()) {
                try {
                    snapshot.forEach((child) => {
                    console.log(child.key);
                    });
                } catch(error) {
                console.log(error);
                }
                }
            });
        }
    getPlayerData();

    //Working with Authentication
    const auth = getAuth();
    var createUserForm = document.getElementById("createUserForm");
    createUserForm.addEventListener("submit", function(e) {
        e.preventDefault();
        var email = document.getElementById("email").value;
        var password = document.getElementById("password").value;
        createUser(email, password);
        console.log("email: " + email + " password: " + password);
    });

    function createUser(email, password) {
        createUserWithEmailAndPassword(auth, email, password)
        .then((userCredential) => {
            // Signed in 
            const user = userCredential.user;
            console.log("User created: " + JSON.stringify(userCredential));
            console.log("User is signed in")
        })
        .catch((error) => {
            const errorCode = error.code;
            const errorMessage = error.message;
            console.log("Error " + errorCode + ": " + errorMessage);
        });
    }
