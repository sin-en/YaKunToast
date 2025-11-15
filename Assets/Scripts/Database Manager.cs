using System;
using Firebase;
using Firebase.Database;
using UnityEngine;
using System.Collections.Generic;//collections
using System.Threading.Tasks;
using Firebase.Extensions;
using UnityEngine.UI; //for UI interfaces
using TMPro;
using Firebase.Auth;

public class DatabaseManager : MonoBehaviour
{
    DatabaseReference dbReference;
    public FirebaseAuth Auth { get; private set; }
    [Header("UI")]
    public TMP_InputField txtPlayerName;
    public TMP_InputField txtPassword;
    public TMP_InputField txtEmail;
    public Button btnSignUp;
    public Button btnLogin;
    async void Start()
    {
        var ready = await InitializeFirebase();
        if (!ready)
        {
            Debug.LogError("Firebase not ready. Let's stop.");
            return;
        }
        Auth = FirebaseAuth.DefaultInstance;
        btnSignUp.onClick.AddListener(async () => await OnAddPlayerClick());
    }
    private async Task<bool> InitializeFirebase()
    {
        Debug.Log("Checking Firebase dependencies...");
        var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();

        if (dependencyStatus == DependencyStatus.Available)
        {
            FirebaseApp app = FirebaseApp.DefaultInstance;
            dbReference = FirebaseDatabase.DefaultInstance.RootReference;
            Debug.Log("Firebase is ready.");
            return true;
        }
        Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
        return false;
    }
    public async Task OnAddPlayerClick()
    {
        var playerName = txtPlayerName.text.Trim();

        var newPlayerKey = dbReference.Child("players").Push();
        var newPlayer = new Player(playerName);

        await newPlayerKey.SetRawJsonValueAsync(JsonUtility.ToJson(newPlayer));
        Debug.Log($"Player created. Key={newPlayerKey.Key}, Name={playerName}");
    }
    public void WriteNewPlayer()
    {
        var playerName = txtPlayerName.text.Trim();
        Player player = new Player();
        string json = JsonUtility.ToJson(player);
        dbReference.Child("users").Child("playerId").SetRawJsonValueAsync(json)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError($"Error writing user data: {task.Exception}");
                }
                else if (task.IsCompleted)
                {
                    Debug.Log("User data written successfully!");
                }
            });
    }
    public async void CreateNewPlayer(string playeruid, string playerName, string email)
    {
        try
        {
            var player = new Player(playeruid, playerName, email);
            var playerSecond = new Player(playeruid, playerName, email);
            string json = JsonUtility.ToJson(playerSecond);
            DatabaseReference newUserRef = dbReference.Child("users").Push();
            await newUserRef.SetRawJsonValueAsync(json);
            Debug.Log($"User written with ID: {newUserRef.Key}");
        }
        catch (Exception ex)
        {
            Debug.LogError("Add player failed: " + ex.Message);
        }
    }
    public async Task AddNewPlayer(string playerId, string playerName, string email)
    {
        var player = new Player(playerId, playerName, email);
        string json = JsonUtility.ToJson(player);
        
        //create unique key and write data to the database with the reference root/players
        var playerReference = dbReference.Child("players").Push();
        await playerReference.SetRawJsonValueAsync(json);
    }
    public async Task<Player> GetPlayer(string playerId)
    {
        var playerSnapshot = await dbReference.Child("players").Child(playerId).GetValueAsync();
        if (!playerSnapshot.Exists)
        {
            Debug.LogWarning("Player does not exist!");
            return null;
        }
        var json = playerSnapshot.GetRawJsonValue();
        var player = JsonUtility.FromJson<Player>(json);
        return player;
    }
     public async Task DeletePlayer(string playerId)
    {
        await dbReference.Child("players").Child(playerId).RemoveValueAsync();
    }
    public async Task<FirebaseUser> RegisterEmail(string email, string password)
    {
        var result = await Auth.CreateUserWithEmailAndPasswordAsync(email, password);
        Debug.Log("Registered: " + result.User.UserId);
        return result.User;
    }
}

