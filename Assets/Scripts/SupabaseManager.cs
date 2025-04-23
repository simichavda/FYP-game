using Supabase;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public static class SupabaseManager
    {
        private static Supabase.Client _client;
        private static bool _isInitialised;

        private static readonly string Url = "https://jnjnlzttyzcjnbonvqra.supabase.co";
        private static readonly string Key = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Impuam5senR0eXpjam5ib252cXJhIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NDUxODMyMTUsImV4cCI6MjA2MDc1OTIxNX0.RC6Y2aVUt6QIRU8TeShAO9q1feS6Cwr6tIjQl-e8Gvs";

        public static Supabase.Client Client
        {
            get
            {
                if (!_isInitialised)
                    throw new InvalidOperationException("Supabase client is not initialized. Call InitializeAsync() first.");
                return _client;
            }
        }

        public static async Task InitialiseAsync()
        {
            if (_isInitialised)
            {
                Debug.Log("Supabase client is already initialized.");
                return;
            }

            var options = new SupabaseOptions
            {
                AutoConnectRealtime = true
            };

            _client = new Supabase.Client(Url, Key, options);
            await _client.InitializeAsync().ConfigureAwait(false);
            _isInitialised = true;
            Debug.Log("Supabase client initialized.");
        }

        public static async Task SignInAsync(string email, string password)
        {
            try
            {
                await Client.Auth.SignInWithPassword(email, password).ConfigureAwait(false);
                Debug.Log($"User {email} signed in successfully.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error signing in: {ex.Message}");
                throw;
            }
        }

        public static async Task SignOutAsync()
        {
            try
            {
                await Client.Auth.SignOut().ConfigureAwait(false);
                Debug.Log("User signed out successfully.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error signing out: {ex.Message}");
                throw;
            }
        }

        public static async Task SignUpAsync(string email, string password)
        {
            try
            {
                await Client.Auth.SignUp(email, password).ConfigureAwait(false);
                Debug.Log($"User {email} registered successfully.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error registering user: {ex.Message}");
                throw;
            }
        }
    }
}
