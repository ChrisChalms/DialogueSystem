#pragma warning disable 649

using UnityEngine;

namespace CC.DialogueSystem
{
    public class CharacterSpriteLoader : MonoBehaviour
    {
        [SerializeField]
        private DialogueSprites_SO _spritesObject;
        [SerializeField]
        private bool _registerOnStart;

        #region MonoBehaviour

        // Initialize
        private void Start()
        {
            if (_registerOnStart)
                LoadSprites();
        }

        #endregion

        // Send the character sprites to the repo
        public void LoadSprites()
        {
            // Can't load if there's no name
            if (string.IsNullOrEmpty(_spritesObject.CharactersName))
            {
                DialogueLogger.LogError($"Object {gameObject.name} cannot register character sprites without a name");
                return;
            }

            // Can't load if there's no sprites object
            if (_spritesObject == null)
            {
                DialogueLogger.LogError($"Object {gameObject.name} has an empty sprites object variable");
                return;
            }

            SpriteRepo.Instance.RegisterCharacterSprites(_spritesObject);
        }
    }
}