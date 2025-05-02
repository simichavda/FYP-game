using Assets.Scripts;
using UnityEngine;

public class LogoutHandler : MonoBehaviour
{
    public async void Logout()
    {
        // Call the Supabase logout function
        await SupabaseManager.SignOutAsync();

        // Optionally, you can load the login scene or main menu
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}