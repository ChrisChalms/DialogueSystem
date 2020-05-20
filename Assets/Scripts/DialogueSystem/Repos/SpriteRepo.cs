using System.Collections.Generic;
using UnityEngine;

namespace CC.DialogueSystem
{
    // This is created by the conversation repo, there's no need for it to inherit from MonoBehaviour until there's a custom inspector or something
    public class SpriteRepo
    {
        private Dictionary<string, DialogueSprites_SO> _characterSprites;

        public static SpriteRepo Instance { get; private set; }

        // Apply singleton
        public SpriteRepo()
        {
            if (Instance == null)
                Instance = this;

            _characterSprites = new Dictionary<string, DialogueSprites_SO>();
        }

        // Add the sprites to the repo ready for retrieval
        public void RegisterCharacterSprites(DialogueSprites_SO sprites)
        {
            if (_characterSprites.ContainsKey(sprites.CharactersName))
                DialogueLogger.LogWarning($"Character sprites for {sprites.CharactersName} already exist, overwritting");

            if (validateSprites(sprites))
                _characterSprites[sprites.CharactersName] = sprites;
        }

        // Check all values are valid
        private bool validateSprites(DialogueSprites_SO sprites)
        {
            var names = new List<string>();

            if (sprites.CharacterSprites.Count == 0)
                DialogueLogger.LogWarning($"Trying to register character sprites for {sprites.CharactersName} but the sprite list is empty");

            foreach (var spritePair in sprites.CharacterSprites)
            {
                // Check name
                if (string.IsNullOrEmpty(spritePair.Name) || string.IsNullOrWhiteSpace(spritePair.Name))
                {
                    DialogueLogger.LogError($"Error registering character sprites for {sprites.CharactersName}, one or more of the names is empty");
                    return false;
                }

                // Check for duplicate names
                if (names.Contains(spritePair.Name))
                {
                    DialogueLogger.LogError($"Error registering character sprites for {sprites.CharactersName}, there is already a sprite with the name {spritePair.Name}. Each sprite must have a unique name");
                    return false;
                }

                names.Add(spritePair.Name);

                // Check sprites
                if (spritePair.Sprite == null)
                {
                    DialogueLogger.LogError($"Error registering character sprites for {sprites.CharactersName} with the sprite name {spritePair.Name}, but the sprite is empty");
                    return false;
                }
            }

            return true;
        }

        // Return the sprite for the character if found
        public Sprite RetrieveSprite(string character, string name = "Default")
        {
            if (string.IsNullOrEmpty(character))
            {
                DialogueLogger.Log("Trying to retrieve sprite from repo, but the character parameter is empty");
                return null;
            }

            if (!_characterSprites.ContainsKey(character))
            {
                DialogueLogger.LogError($"trying to retrieve sprite {name} for the character {character}, but it is not registered in the repo.");
                return null;
            }

            // Seems to be consistently faster than Linq
            return _characterSprites[character].CharacterSprites.Find(s => s.Name == name)?.Sprite;
        }
    }
}