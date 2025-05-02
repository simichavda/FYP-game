using Assets.Scripts;
using Supabase.Gotrue.Exceptions;
using System;
using System.Text.Json;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    public static LoginManager Instance { get; private set; }

    public TMP_InputField UsernameInputField;
    public TMP_InputField PasswordInputField;
    public int sceneToLoad = 3;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public async void Login()
    {
        string username = UsernameInputField.text;
        string password = PasswordInputField.text;

        Debug.Log($"Logging in with Username: {username} and Password: {password}");


        await SupabaseManager.InitialiseAsync();

        try
        {
            await SupabaseManager.SignInAsync(username, password);
            Debug.Log("Login successful");

            SceneManager.LoadScene(sceneToLoad);
        }
        catch (GotrueException ex)
        {
            Debug.LogError($"Login failed: {ex.Message}");
            ErrorPopup.Show(ex);
        }
    }
}
