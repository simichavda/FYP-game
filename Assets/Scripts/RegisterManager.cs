using TMPro;
using UnityEngine;
using Supabase.Gotrue.Exceptions;
using UnityEngine.SceneManagement;
using Assets.Scripts;

public class RegisterManager : MonoBehaviour
{
    public TMP_InputField EmailInputField;
    public TMP_InputField PasswordInputField;
    public TMP_InputField PasswordConfirmInputField;
    public int sceneToLoad = 3;

    public async void Register()
    {
        string email = EmailInputField.text;
        string password = PasswordInputField.text;
        string passwordConfirm = PasswordConfirmInputField.text;

        if(password != passwordConfirm)
        {
            Debug.LogError("Passwords do not match");
            ErrorPopup.Show("Passwords do not match");
            return;
        }

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

            ErrorPopup.Show(ex);
        }
    }
}
