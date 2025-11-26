using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;

public class FirebaseManager : MonoBehaviour
{
    #region Firebase References
    [Header("Firebase")]
    private FirebaseAuth auth;
    private FirebaseUser user;
    private DatabaseReference dbReference;
    #endregion

    #region UI References - Login
    [Header("Login")]
    public TMP_InputField loginEmailField;
    public TMP_InputField loginPasswordField;
    public TMP_Text warningLoginText;
    public TMP_Text confirmLoginText;
    public Button btnLogin;
    #endregion

    #region UI References - Signup
    [Header("Signup")]
    public TMP_InputField signupEmailField;
    public TMP_InputField signupPasswordField;
    public TMP_InputField signupUsernameField;
    public TMP_Text warningSignupText;
    public TMP_Text confirmSignupText;
    public Button btnSignup;
    #endregion

    #region Unity Lifecycle
    private async void Awake()
    {
        await InitializeFirebase();
    }

    private void Start()
    {
        if (btnLogin != null)
            btnLogin.onClick.AddListener(LoginButton);
        
        if (btnSignup != null)
            btnSignup.onClick.AddListener(SignupButton);
    }
    #endregion

    #region Firebase Initialization
    private async Task<bool> InitializeFirebase()
    {
        var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();

        if (dependencyStatus == DependencyStatus.Available)
        {
            Debug.Log("Firebase dependencies resolved. Initializing...");
            auth = FirebaseAuth.DefaultInstance;
            dbReference = FirebaseDatabase.DefaultInstance.RootReference;
            Debug.Log("Firebase initialized successfully!");
            return true;
        }
        else
        {
            Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
            return false;
        }
    }
    #endregion

    #region Authentication - Login
    public void LoginButton()
    {
        StartCoroutine(LoginCoroutine(loginEmailField.text, loginPasswordField.text));
    }

    private IEnumerator LoginCoroutine(string email, string password)
    {
        warningLoginText.text = "";
        confirmLoginText.text = "";

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            warningLoginText.text = "Please fill in all fields!";
            yield break;
        }

        var loginTask = auth.SignInWithEmailAndPasswordAsync(email, password);
        yield return new WaitUntil(() => loginTask.IsCompleted);

        if (loginTask.Exception != null)
        {
            Debug.LogWarning($"Login failed: {loginTask.Exception}");
            FirebaseException firebaseEx = loginTask.Exception.GetBaseException() as FirebaseException;
            
            if (firebaseEx != null)
            {
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                warningLoginText.text = GetAuthErrorMessage(errorCode);
            }
            else
            {
                warningLoginText.text = "Login Failed!";
            }
        }
        else
        {
            user = loginTask.Result.User;
            Debug.Log($"User logged in successfully: {user.DisplayName} ({user.UserId})");
            confirmLoginText.text = "Login Successful!";
            
            yield return StartCoroutine(LoadUserDataCoroutine(user.UserId));
        }
    }
    #endregion

    #region Authentication - Signup
    public void SignupButton()
    {
        if (btnSignup != null)
            btnSignup.interactable = false;
            
        StartCoroutine(SignupCoroutine(signupEmailField.text, signupPasswordField.text, signupUsernameField.text));
    }

    private IEnumerator SignupCoroutine(string email, string password, string username)
    {
        warningSignupText.text = "";
        confirmSignupText.text = "";

        // Validation
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            warningSignupText.text = "Please fill in all fields!";
            if (btnSignup != null) btnSignup.interactable = true;
            yield break;
        }

        // Create user account
        var createUserTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);
        yield return new WaitUntil(() => createUserTask.IsCompleted);

        if (createUserTask.Exception != null)
        {
            Debug.LogWarning($"Signup failed: {createUserTask.Exception}");
            FirebaseException firebaseEx = createUserTask.Exception.GetBaseException() as FirebaseException;
            
            if (firebaseEx != null)
            {
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                warningSignupText.text = GetSignupErrorMessage(errorCode);
            }
            else
            {
                warningSignupText.text = "Signup Failed!";
            }
            
            if (btnSignup != null) btnSignup.interactable = true;
            yield break;
        }

        // User created successfully
        user = createUserTask.Result.User;
        Debug.Log($"Auth account created: {user.UserId}");

        // Update user profile with username
        UserProfile profile = new UserProfile { DisplayName = username };
        var updateProfileTask = user.UpdateUserProfileAsync(profile);
        yield return new WaitUntil(() => updateProfileTask.IsCompleted);

        if (updateProfileTask.Exception != null)
        {
            Debug.LogWarning($"Failed to update profile: {updateProfileTask.Exception}");
        }
        else
        {
            Debug.Log($"Display name set to: {username}");
        }

        // Create user in database
        yield return StartCoroutine(CreateUserInDatabaseCoroutine(user.UserId, username, email));

        // Clear input fields
        signupEmailField.text = "";
        signupPasswordField.text = "";
        signupUsernameField.text = "";

        // Show success message
        confirmSignupText.text = "Signup Successful!";
        Debug.Log("User signup completed successfully!");

        // Re-enable button
        if (btnSignup != null) btnSignup.interactable = true;

        // Navigate to login screen if UIManager exists
        if (UIManager.instance != null)
            UIManager.instance.LoginScreen();
    }
    #endregion

    #region Authentication - Utilities
    public void SignOut()
    {
        if (auth != null && user != null)
        {
            auth.SignOut();
            user = null;
            Debug.Log("User signed out");
        }
    }

    public bool IsUserLoggedIn()
    {
        return user != null;
    }

    public FirebaseUser GetCurrentUser()
    {
        return user;
    }
    #endregion

    #region Database - CRUD Operations
    private IEnumerator CreateUserInDatabaseCoroutine(string userId, string username, string email)
    {
        var playerData = new Player
        {
            playeruid = userId,
            playerName = username,
            email = email,
        };

        string json = JsonUtility.ToJson(playerData);
        DatabaseReference newUserRef = dbReference.Child("users").Child(userId);
        
        var setTask = newUserRef.SetRawJsonValueAsync(json);
        yield return new WaitUntil(() => setTask.IsCompleted);

        if (setTask.Exception != null)
        {
            Debug.LogError($"Failed to write user data: {setTask.Exception}");
        }
        else
        {
            Debug.Log($"User data written to database for userId: {userId}");
        }
    }

    private IEnumerator LoadUserDataCoroutine(string userId)
    {
        var dataTask = dbReference.Child("users").Child(userId).GetValueAsync();
        yield return new WaitUntil(() => dataTask.IsCompleted);

        if (dataTask.Exception != null)
        {
            Debug.LogError($"Failed to load user data: {dataTask.Exception}");
            yield break;
        }

        DataSnapshot snapshot = dataTask.Result;
        if (snapshot.Exists)
        {
            string json = snapshot.GetRawJsonValue();
            Player playerData = JsonUtility.FromJson<Player>(json);
            Debug.Log($"Player data loaded: {playerData.playerName}");
        }
        else
        {
            Debug.LogWarning("User data does not exist in database");
        }
    }

    public async Task<Player> GetPlayerData(string playerId)
    {
        try
        {
            var snapshot = await dbReference.Child("users").Child(playerId).GetValueAsync();
            
            if (!snapshot.Exists)
            {
                Debug.LogWarning($"Player data does not exist for playerId: {playerId}");
                return null;
            }

            string json = snapshot.GetRawJsonValue();
            Player playerData = JsonUtility.FromJson<Player>(json);
            return playerData;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error reading user data: {ex.Message}");
            return null;
        }
    }

    public async Task UpdateUserData(string userId, Player playerData)
    {
        try
        {
            string json = JsonUtility.ToJson(playerData);
            await dbReference.Child("users").Child(userId).SetRawJsonValueAsync(json);
            Debug.Log($"User data updated for userId: {userId}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error updating user data: {ex.Message}");
        }
    }

    public async Task DeleteUserData(string userId)
    {
        try
        {
            await dbReference.Child("users").Child(userId).RemoveValueAsync();
            Debug.Log($"User data deleted for userId: {userId}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error deleting user data: {ex.Message}");
        }
    }
    #endregion

    #region Player Management
    public async Task OnAddPlayerClick()
    {
        var playerName = signupUsernameField.text.Trim();
        
        var newPlayerKey = dbReference.Child("players").Push();
        var newPlayer = new Player(playerName);
        
        await newPlayerKey.SetRawJsonValueAsync(JsonUtility.ToJson(newPlayer));
        Debug.Log($"Player created. Key={newPlayerKey.Key}, Name={playerName}");
    }
    #endregion

    #region Error Messages
    private string GetAuthErrorMessage(AuthError errorCode)
    {
        return errorCode switch
        {
            AuthError.MissingEmail => "Missing Email!",
            AuthError.MissingPassword => "Missing Password!",
            AuthError.InvalidEmail => "Invalid Email!",
            AuthError.WrongPassword => "Wrong Password!",
            AuthError.UserNotFound => "Account does not exist!",
            _ => "Login Failed!"
        };
    }

    private string GetSignupErrorMessage(AuthError errorCode)
    {
        return errorCode switch
        {
            AuthError.MissingEmail => "Missing Email!",
            AuthError.MissingPassword => "Missing Password!",
            AuthError.WeakPassword => "Weak Password! (Minimum 6 characters)",
            AuthError.EmailAlreadyInUse => "Email Already In Use!",
            AuthError.InvalidEmail => "Invalid Email Format!",
            _ => "Signup Failed!"
        };
    }
    #endregion

}