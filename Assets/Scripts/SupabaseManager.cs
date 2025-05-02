using Supabase;
using Supabase.Postgrest.Models;
using System;
using System.Threading.Tasks;
using UnityEngine;
using Supabase.Postgrest.Attributes;
using System.Collections.Generic;

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

        public static async Task<List<SkinData>> GetSkinsAsync()
        {
            // Goes through each skin in skins table (as above) but also check if player owns it (playerStats.activeSkin) or just has it purchased (playerSkins):
            try
            {
                Debug.Log("Fetching skins from Supabase...");
                var skinsTable = await Client.From<SkinRecord>().Get();
                var playerSkins = await Client.From<PlayerSkinRecord>().Get();
                var playerStats = await Client.From<playerStatsRecord>().Get();

                List<SkinData> allSkins = new List<SkinData>();
                foreach (var skin in skinsTable.Models)
                {
                    var skinData = new SkinData
                    {
                        DisplayName = skin.DisplayName,
                        PreviewSprite = Resources.Load<Sprite>($"skins/{skin.PreviewImage}"),
                        MeshTexture = Resources.Load<Texture2D>($"textures/{skin.MaterialName}"),
                        Price = skin.Price,
                        Status = SkinStatus.Locked // Default to locked
                    };

                    // Check if the player owns this skin
                    foreach (var playerSkin in playerSkins.Models)
                    {
                        if (playerSkin.Skin == skin.DisplayName && playerSkin.PlayerId == GetPlayerId())
                        {
                            skinData.Status = SkinStatus.Purchased;
                            break;
                        }
                    }

                    // Check if the player has this skin equipped
                    foreach (var playerStat in playerStats.Models)
                    {
                        if (playerStat.ActiveSkin == skin.DisplayName && playerStat.PlayerId == GetPlayerId())
                        {
                            skinData.Status = SkinStatus.Equipped;
                            break;
                        }
                    }

                    Debug.Log($"Loaded skin: {skinData.DisplayName} with image: {skinData.PreviewSprite.name}");
                    allSkins.Add(skinData);
                }

                allSkins.Sort((x, y) => x.Price.CompareTo(y.Price));

                return allSkins;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error fetching skins: {ex.Message}");
                throw;
            }
        }

        //get player id from supabase
        public static string GetPlayerId()
        {
            try
            {
                var user = Client.Auth.CurrentUser;
                if (user != null)
                {
                    return user.Id;
                }
                else
                {
                    Debug.LogError("User not found.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error fetching player ID: {ex.Message}");
                throw;
            }
        }

        public static async Task<bool> DoesPlayerOwnSkin(string skinName)
        {
            try
            {
                var playerSkins = await Client.From<PlayerSkinRecord>().Get();
                foreach (var skin in playerSkins.Models)
                {
                    if (skin.Skin == skinName && skin.PlayerId == GetPlayerId())
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error checking if player owns skin: {ex.Message}");
                throw;
            }
        }

        //get active skin from supabase
        public static async Task<SkinData> GetActiveSkin()
        {
            try
            {
                var playerStats = await Client.From<playerStatsRecord>().Get();
                foreach (var playerStat in playerStats.Models)
                {
                    if (playerStat.PlayerId == GetPlayerId())
                    {
                        var skinsTable = await Client.From<SkinRecord>().Get();
                        foreach (var skinRecord in skinsTable.Models)
                        {
                            if (skinRecord.DisplayName == playerStat.ActiveSkin)
                            {
                                return new SkinData
                                {
                                    DisplayName = skinRecord.DisplayName,
                                    PreviewSprite = Resources.Load<Sprite>($"skins/{skinRecord.PreviewImage}"),
                                    MeshTexture = Resources.Load<Texture2D>($"textures/{skinRecord.MaterialName}"),
                                    Price = skinRecord.Price,
                                    Status = SkinStatus.Equipped
                                };
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error fetching active skin: {ex.Message}");
                throw;
            }
        }

        // add skin to playerSkins (just purchased)
        public static async Task<bool> AddSkin(SkinData skin)
        {
            try
            {
                var playerId = GetPlayerId();
                var skinName = skin.DisplayName;
                var playerSkins = await Client.From<PlayerSkinRecord>().Get();
                foreach (var playerSkin in playerSkins.Models)
                {
                    if (playerSkin.PlayerId == playerId && playerSkin.Skin == skinName)
                    {
                        Debug.Log($"Player already owns skin: {skinName}");
                        return false; // Skin already owned
                    }
                }

                var newPlayerSkin = new PlayerSkinRecord
                {
                    PlayerId = playerId,
                    Skin = skinName
                };

                await Client.From<PlayerSkinRecord>().Insert(newPlayerSkin);
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error adding skin: {ex.Message}");
                throw;
            }
        }

        // mark skin as equipped in playerStats
        public static async Task<bool> EquipSkin(string skinName)
        {
            try
            {
                var playerStats = await Client.From<playerStatsRecord>().Get();
                foreach (var playerStat in playerStats.Models)
                {
                    if (playerStat.PlayerId == GetPlayerId())
                    {
                        playerStat.ActiveSkin = skinName;
                        await Client.From<playerStatsRecord>().Update(playerStat);
                        Debug.Log($"Supabase: Skin {skinName} equipped for player {playerStat.PlayerId}");
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error equipping skin: {ex.Message}");
                throw;
            }
        }


        public static async Task<int> getPoints()
        {
            try
            {
                var playerStats = await Client.From<playerStatsRecord>().Get();
                foreach (var playerStat in playerStats.Models)
                {
                    if (playerStat.PlayerId == GetPlayerId())
                    {
                        return playerStat.Points;
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error fetching player points: {ex.Message}");
                throw;
            }
        }

        // Add points
        public static async Task<bool> AddPoints(string playerId, int points)
        {
            try
            {
                var playerStats = await Client.From<playerStatsRecord>().Get();
                foreach (var playerStat in playerStats.Models)
                {
                    if (playerStat.PlayerId == playerId)
                    {
                        playerStat.Points += points;
                        await Client.From<playerStatsRecord>().Update(playerStat);
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error adding points: {ex.Message}");
                throw;
            }
        }

        // Remove points
        public static async Task<bool> RemovePoints(string playerId, int points)
        {
            try
            {
                var playerStats = await Client.From<playerStatsRecord>().Get();
                foreach (var playerStat in playerStats.Models)
                {
                    if (playerStat.PlayerId == playerId)
                    {
                        playerStat.Points -= points;
                        await Client.From<playerStatsRecord>().Update(playerStat);
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error removing points: {ex.Message}");
                throw;
            }
        }

        // Set points
        public static async Task<bool> setPoints(int points)
        {
            try
            {
                var playerStats = await Client.From<playerStatsRecord>().Get();
                foreach (var playerStat in playerStats.Models)
                {
                    if (playerStat.PlayerId == GetPlayerId())
                    {
                        playerStat.Points = points;
                        await Client.From<playerStatsRecord>().Update(playerStat);
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error setting points: {ex.Message}");
                throw;
            }
        }
    }


    



    [Table("skins")]
    public class SkinRecord : BaseModel
    {
        // If you don’t have an integer “id” column,
        // you can treat displayName as your PK:
        [PrimaryKey("displayName")]
        public string DisplayName { get; set; }

        [Column("previewImage")]
        public string PreviewImage { get; set; }

        [Column("materialName")]
        public string MaterialName { get; set; }

        [Column("price")]
        public int Price { get; set; }
    }

    [Table("playerSkins")]
    public class PlayerSkinRecord : BaseModel
    {
        [PrimaryKey("player_id", true)]
        public string PlayerId { get; set; } = null!;


        [PrimaryKey("skin", true)]
        public string Skin { get; set; } = null!;
    }

    [Table("playerStats")]
    public class playerStatsRecord : BaseModel
    {
        [PrimaryKey("player_id", true)]
        public string PlayerId { get; set; } = null!;

        [Column("points")]
        public int Points { get; set; }

        [Column("activeSkin")]
        public string ActiveSkin { get; set; } = null!;
    }
}
