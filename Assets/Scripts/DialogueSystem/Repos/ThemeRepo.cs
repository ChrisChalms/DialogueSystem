using System.Collections.Generic;
using UnityEngine;

namespace CC.DialogueSystem
{
    // Repo to store, validate, and retrieve all of the dialogue themes used
    public class ThemeRepo : MonoBehaviour
    {
        public List<Theme_SO> _themesToLoad;

        private Dictionary<string, Theme_SO> _themeSprites;

        public static ThemeRepo Instance { get; private set; }

        #region MonoBehaviour

        // Apply singleton
        public void Awake()
        {
            if (Instance == null)
                Instance = this;

            _themeSprites = new Dictionary<string, Theme_SO>();

            loadThemes();
        }

        #endregion

        // Loads any themes that have been set in the inspector
        private void loadThemes()
        {
            foreach (var theme in _themesToLoad)
                RegisterThemeSprites(theme);
        }

        // Check all the values are valid
        private bool validateSprites(Theme_SO sprites)
        {
            var names = new List<string>();

            if (sprites.ThemeSprites.Count == 0)
                DialogueLogger.LogWarning($"Trying to register theme sprites for {sprites.Name} but the sprite list is empty");

            foreach (var spritePair in sprites.ThemeSprites)
            {
                // Check name
                if (string.IsNullOrEmpty(spritePair.Name) || string.IsNullOrWhiteSpace(spritePair.Name))
                {
                    DialogueLogger.LogError($"Error registering theme sprites for {sprites.Name}, one or more of the names is empty");
                    return false;
                }

                // Check for duplicate names
                if (names.Contains(spritePair.Name))
                {
                    DialogueLogger.LogError($"Error registering theme sprites for {sprites.Name}, there is already a sprite with the name {spritePair.Name}. Each sprite must have a unique name");
                    return false;
                }

                names.Add(spritePair.Name);

                // Check sprites
                if (spritePair.Sprite == null)
                {
                    DialogueLogger.LogError($"Error registering theme sprites for {sprites.Name} with the sprite name {spritePair.Name}, but the sprite is empty");
                    return false;
                }
            }

            return true;
        }

        // Add a theme to the repo if it's valid
        public void RegisterThemeSprites(Theme_SO sprites)
        {
            if (_themeSprites.ContainsKey(sprites.Name))
                DialogueLogger.LogWarning($"Theme sprites for {sprites.Name} already exist, overwritting");

            if (validateSprites(sprites))
                _themeSprites[sprites.Name] = sprites;
        }

        // Retrieve a sprite from a registered theme
        public Sprite RetrieveSprite(string themeName, string spriteName)
        {
            if (string.IsNullOrEmpty(themeName))
            {
                DialogueLogger.Log("Trying to retrieve sprite from repo, but the themeName parameter is empty");
                return null;
            }

            if (!_themeSprites.ContainsKey(themeName))
            {
                DialogueLogger.LogError($"Trying to retrieve sprite {themeName} for the theme {themeName}, but it is not registered in the repo.");
                return null;
            }

            // Seems to be consistently faster than Linq
            return _themeSprites[themeName].ThemeSprites.Find(s => s.Name == spriteName)?.Sprite;
        }
    }
}
