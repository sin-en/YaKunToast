using System;
using System.Collections.Generic; //collections
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI; //for UI interfaces

public class FirebaseDatabaseManager : MonoBehaviour
{
    //where our Firebase is referencing (takes reference frm google-services.json)
    DatabaseReference dbReference;
    public FirebaseAuth Auth { get; private set; }

    [Header("UI Elements")]
    public TMP_InputField txtPlayerName;
    public TMP_InputField txtEmail;
    public TMP_InputField txtPassword;
    public Button btnCreate;
    public Button btnLogin;

   // Use this for initialization
    async void Start()
    {
        var ready = await InitializeFirebase();
        if (!ready)
        {
            Debug.LogError("Firebase not ready. Stopping script.");
            return;
        }

        //initialize auth when db is ready
        Auth = FirebaseAuth.DefaultInstance;

        //button listeners
        btnCreate.onClick.AddListener(async () => await OnAddPlayerClick());
    }

    //upgrade to async task
    private async Task<bool> InitializeFirebase()
    {
        Debug.Log("Checking Firebase dependencies...");
        var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();

        if (dependencyStatus == DependencyStatus.Available)
        {
            FirebaseApp app = FirebaseApp.DefaultInstance;
            dbReference = FirebaseDatabase.DefaultInstance.RootReference;
            Debug.Log("Firebase is ready!");
            return true;
        }
        else
        {
            Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
            return false;
        }
    }

    //triggered when create button is clicked
    public async Task OnAddPlayerClick()
    {
        var playerName = txtPlayerName.text.Trim();
        var email = txtEmail.text.Trim();
        var password = txtPassword.text.Trim();

        if (string.IsNullOrEmpty(playerName) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            Debug.LogWarning("Please fill in all fields!");
            return;
        }

        //Step 1: Register user using Firebase Auth
        var user = await RegisterEmail(email, password);
        if (user == null)
        {
            Debug.LogError("Registration failed.");
            return;
        }

       //do playername check
        var newPlayerKey = dbReference.Child("players").Push();
        var newPlayer = new Player(newPlayerKey.Key, playerName, 0, true);
        newPlayer.uuid = user.UserId;

        await newPlayerKey.SetRawJsonValueAsync(JsonUtility.ToJson(newPlayer));
        Debug.Log($"Player created successfully! Key={newPlayerKey.Key}, Name={playerName}");
    }

    //register new user with email & password
    public async Task<FirebaseUser> RegisterEmail(string email, string password)
    {
        try
        {
            var result = await Auth.CreateUserWithEmailAndPasswordAsync(email, password);
            Debug.Log("Registered new user: " + result.User.UserId);
            return result.User;
        }
        catch (Exception e)
        {
            Debug.LogError("Registration Error: " + e.Message);
            return null;
        }
    }

    //CRUD: CREATE 
    // add new player manually
    public async Task AddNewPlayer(string playerId, string playerName, int score, bool isOnline)
    {
        Player player = new Player(playerId, playerName, score, isOnline);
        string json = JsonUtility.ToJson(player);

        var playerReference = dbReference.Child("players").Push();
        await playerReference.SetRawJsonValueAsync(json);

        Debug.Log("New player added to database!");
    }

    //CRUD: READ 
    // get player by ID
    public async Task<Player> GetPlayer(string playerId)
    {
        var snapshot = await dbReference.Child("players").Child(playerId).GetValueAsync();

        if (!snapshot.Exists)
        {
            Debug.LogWarning("Player not found in database!");
            return null;
        }

        string json = snapshot.GetRawJsonValue();
        Player player = JsonUtility.FromJson<Player>(json);
        Debug.Log("Player retrieved: " + player.playerName);
        return player;
    }

    //CRUD: UPDATE 
    // update player info
    public async Task UpdatePlayer(string playerId, string playerName = "", int score = 0, bool isOnline = false)
    {
        var updates = new Dictionary<string, object>();

        if (!string.IsNullOrEmpty(playerName)) updates["playerName"] = playerName;
        if (score > 0) updates["score"] = score;
        updates["isOnline"] = isOnline;

        if (updates.Count == 0)
        {
            Debug.LogWarning("No updates provided.");
            return;
        }

        await dbReference.Child("players").Child(playerId).UpdateChildrenAsync(updates);
        Debug.Log("Player data updated successfully!");
    }

    //CRUD: DELETE 
    // remove player by ID
    public async Task DeletePlayer(string playerId)
    {
        await dbReference.Child("players").Child(playerId).RemoveValueAsync();
        Debug.Log("Player deleted from database.");
    }
}
