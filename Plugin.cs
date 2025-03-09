using System;
using BepInEx;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilla;
using Utilla.Attributes;

namespace MonkeBlocksMod
{
    [ModdedGamemode]
    [BepInDependency("org.legoandmars.gorillatag.utilla", "1.5.0")]
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        bool inModdedLobby;
        private GameObject dropZonesObject;
        private GameObject buildZoneObject;
        private Vector3 originalBuildZoneScale;

        void Start()
        {
            Utilla.Events.GameInitialized += OnGameInitialized;
            SceneManager.sceneLoaded += OnSceneLoaded; // Subscribe to the scene loaded event
        }

        void OnGameInitialized(object sender, EventArgs e)
        {
            // Code here runs after the game initializes
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Check if the loaded scene is the MonkeBlocks scene
            if (scene.name == "MonkeBlocks")
            {
                // Find the DropZones GameObject
                dropZonesObject = GameObject.Find("DropZones");
                if (dropZonesObject == null)
                {
                    Logger.LogWarning("DropZones GameObject not found in the MonkeBlocks scene.");
                }

                // Find the BuildZone GameObject
                buildZoneObject = GameObject.Find("BuildZone");
                if (buildZoneObject != null)
                {
                    // Save the original scale of BuildZone
                    originalBuildZoneScale = buildZoneObject.transform.localScale;
                }
                else
                {
                    Logger.LogWarning("BuildZone GameObject not found in the MonkeBlocks scene.");
                }
            }
        }

        void Update()
        {
            if (inModdedLobby)
            {
                // Mod is active in a modded lobby
                if (dropZonesObject != null && dropZonesObject.activeSelf)
                {
                    dropZonesObject.SetActive(false); // Disable DropZones
                }

                if (buildZoneObject != null)
                {
                    // Scale up BuildZone
                    buildZoneObject.transform.localScale = new Vector3(1000f, 100f, 1000f);
                }
            }
            else
            {
                // Mod is not active (in a non-modded lobby or left a modded lobby)
                if (dropZonesObject != null && !dropZonesObject.activeSelf)
                {
                    dropZonesObject.SetActive(true); // Re-enable DropZones
                }

                if (buildZoneObject != null)
                {
                    // Restore BuildZone's original scale
                    buildZoneObject.transform.localScale = originalBuildZoneScale;
                }
            }
        }

        [ModdedGamemodeJoin]
        public void OnJoin(string gamemode)
        {
            // The player joined a modded lobby
            inModdedLobby = true;
            Logger.LogInfo("Joined modded lobby. Mod features activated.");
        }

        [ModdedGamemodeLeave]
        public void OnLeave(string gamemode)
        {
            // The player left a modded lobby
            inModdedLobby = false;
            Logger.LogInfo("Left modded lobby. Mod features deactivated.");
        }
    }
}