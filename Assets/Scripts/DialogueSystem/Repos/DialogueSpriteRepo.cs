#pragma warning disable 649

using System.Collections.Generic;
using UnityEngine;

namespace CC.DialogueSystem
{
    // TODO: Might be best to combine this in with the conversation loader so the dialogue and charater sprites are all together
    public class DialogueSpriteRepo : MonoBehaviour
    {
        private Dictionary<string, DialogueSprites_SO> _characterSprites;

        public static DialogueSpriteRepo Instance { get; private set; }

        #region MonoBehaviour

        // Apply singleton
        private void Awake()
        {
            if (Instance == null)
                Instance = this;

            _characterSprites = new Dictionary<string, DialogueSprites_SO>();
        }

        #endregion

        // Add the sprites to the repo ready for retrieval
        public void RegisterCharacterSprites(string name, DialogueSprites_SO sprites)
        {
            if (_characterSprites.ContainsKey(name))
                DialogueLogger.LogWarning($"Character sprites for {name} already exist, overwritting");

            if (validateSprites(name, sprites))
                _characterSprites[name] = sprites;
        }

        // Check all properties are valid
        private bool validateSprites(string name, DialogueSprites_SO sprites)
        {
            var names = new List<string>();

            if (sprites.CharacterSprites.Count == 0)
                DialogueLogger.LogWarning($"Trying to register character sprites for {name} but the sprite list is empty");

            foreach (var spritePair in sprites.CharacterSprites)
            {
                // Check name
                if (string.IsNullOrEmpty(spritePair.Name) || string.IsNullOrWhiteSpace(spritePair.Name))
                {
                    DialogueLogger.LogError($"Error registering character sprites for {name} but one or more of the names is empty");
                    return false;
                }

                // Check for duplicate names
                if (names.Contains(spritePair.Name))
                {
                    DialogueLogger.LogError($"Error registering character sprites for {name}, there is already a sprite with the name {spritePair.Name}. Each sprite must have a unique name");
                    return false;
                }

                names.Add(spritePair.Name);

                // Check sprites
                if (spritePair.Sprite == null)
                {
                    DialogueLogger.LogError($"Error registering character sprites for {name} with the sprite name {spritePair.Name}, but the sprite is empty");
                    return false;
                }
            }

            return true;
        }

        // Return the sprite for the character if found
        public Sprite RetrieveSprites(string character, string name = "Default")
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