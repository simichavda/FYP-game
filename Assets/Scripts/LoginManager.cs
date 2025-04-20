using TMPro;
using UnityEngine;

public class LoginManager : MonoBehaviour
{
    public static LoginManager Instance { get; private set; }

    public TMP_InputField UsernameInputField;
    public TMP_InputField PasswordInputField;

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

    public void Login()
    {
        // Implement login logic here
        string username = UsernameInputField.text;
        string password = PasswordInputField.text;

        Debug.Log("Test");
        Debug.Log($"Logging in with Username: {username} and Password: {password}");
    }
}
