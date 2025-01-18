using System.Net.Http;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class AuthResponse
{
    public bool auth;
    public string auth_token;
    public string refresh_token;
}

public class AuthController : MonoBehaviour
{
    public TMP_Text email;
    public TMP_Text password;

    void Start()
    {
        if (PlayerPrefs.HasKey("auth_token"))
        {
            WebController.client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", PlayerPrefs.GetString("auth_token"));
            Check();
        }
    }

    public static void GetInfoAndLogin() {
        string email = GameObject.Find("Email").GetComponent<TMP_InputField>().text;
        string password = GameObject.Find("Password").GetComponent<TMP_InputField>().text;
        Login(email, password);
    }

    public static async Task Login(string username, string password)
    {
        Debug.Log("Logging in with username: " + username + " and password: " + password);
        string res = await WebController.Post("/auth/login", "{\"email\": \"" + username + "\", \"password\": \"" + password + "\"}");
        AuthResponse authResponse = JsonUtility.FromJson<AuthResponse>(res);
        if(authResponse.auth) {
            Debug.Log("Logged in successfully!");
            PlayerPrefs.SetString("auth_token", authResponse.auth_token);
            PlayerPrefs.SetString("refresh_token", authResponse.refresh_token);
            PlayerPrefs.SetString("email", username);
            PlayerPrefs.SetString("password", password);
            PlayerPrefs.Save();
            Debug.Log("Saved auth token: " + authResponse.auth_token);
            Debug.Log("Saved refresh token: " + authResponse.refresh_token);
            Debug.Log("Saved email: " + username);
            Debug.Log("Saved password: " + password);
            WebController.client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authResponse.auth_token);
            Check();
        } else {
            Debug.Log("Failed to log in!");
        }
    }

    public static void CheckAuth() {
        Check();
    }

    public static async Task Check()
    {
        string res = await WebController.Get("/auth/check");
        AuthResponse authResponse = JsonUtility.FromJson<AuthResponse>(res);
        if (authResponse.auth)
        {
            Debug.Log("User is still authenticated.");
        }
        else
        {
            Debug.Log("User is not authenticated anymore.");
        }
    }
}