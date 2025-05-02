using TMPro;
using UnityEngine;
using Supabase.Gotrue.Exceptions;
using UnityEngine.SceneManagement;
using Assets.Scripts;

public class RegisterManager : MonoBehaviour
{
    public TMP_InputField EmailInputField;
    public TMP_InputField PasswordInputField;
    public int sceneToLoad = 3;

    public async void Register()
    {
        string email = EmailInputField.text;
        string password = PasswordInputField.text;

        await SupabaseManager.InitialiseAsync();

        try
        {
            await SupabaseManager.SignUpAsync(email, password);
            Debug.Log("Registration successful");

            SceneManager.LoadScene(sceneToLoad);
        }
        catch (GotrueException ex)
        {
            Debug.LogError($"Registration failed: {ex.Message}");
            if (ex.StatusCode == 400) Debug.LogError("Invalid Email");
            else if (ex.StatusCode == 422) Debug.LogError("Weak Password");
        }
    }
}
