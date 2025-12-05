
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
    // DOM Elements
    const output = document.getElementById("output");
    const btnLoad = document.getElementById("btnLoad");
    const frmCreateUser = document.getElementById("frmCreateUser");
    const frmDeleteUser = document.getElementById("frmDeleteUser");
       
    //Get player data from database
    btnLoad.addEventListener("click", getPlayerData);
    function getPlayerData() {
        output.innerHTML = "Loading player data...";
        get(players).then((snapshot) => {
            if (snapshot.exists()) {
                try {
                    let text = "<h3>Players Found:</h3>";
                    snapshot.forEach((childSnapshot) => {
                    const data = childSnapshot.val();
                    text += `
                        <div style="border: 1px solid #ccc; padding: 10px; margin: 5px 0;">
                        <strong>Player Key:</strong> ${childSnapshot.key}<br>
                        <strong>Email:</strong> ${data.email || 'N/A'}<br>
                        <strong>Created:</strong> ${data.createdAt || 'N/A'}
                        </div>
                    `;
                    console.log("Child Key: " + childSnapshot.key);
                        });
                    } catch(error) {
                    console.log(error);
                    }
                    }
                });
        }
    
    

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

    //Create User Function
    function createUser(email, password) {
        output.innerHTML = "Creating user...";
        createUserWithEmailAndPassword(auth, email, password)
        .then((userCredential) => {
            // Signed in 
            const user = userCredential.user;
            console.log("User created: " + JSON.stringify(userCredential));
            console.log("User is signed in")
            //Write player data to database
            return set(ref(db, 'users/' + user.uid), {
                email: user.email,
                createdAt: new Date().toISOString()
            });
        })
        .then(() => {
            output.innerHTML = "User created and data saved to database.";
            frmCreateUser.reset();
        })
        .catch((error) => {
            const errorCode = error.code;
            const errorMessage = error.message;
            console.log("Error " + errorCode + ": " + errorMessage);
        });
    }

    //Delete User Form Listener
    frmDeleteUser.addEventListener("submit", function (e) {
    e.preventDefault();

    const userKey = document.getElementById("userKey").value;

    deleteUser(userKey);
    });

    // Delete User
    function deleteUser(userKey) {
    output.innerHTML = "Deleting user...";

    // Delete from database
    remove(ref(db, `players/${userKey}`))
        .then(() => {
        output.innerHTML = `User with key ${userKey} deleted from database successfully!<br>
            <span style='color:orange;'>Note: Auth account cannot be deleted unless the user is currently signed in.</span>`;
        frmDeleteUser.reset();
        })
        .catch((error) => {
        console.log("Error:", error.code, error.message);
        output.innerHTML = `<span style='color:red;'>Error deleting user: ${error.message}</span>`;
        });
    }