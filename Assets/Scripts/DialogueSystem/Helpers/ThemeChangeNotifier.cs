#pragma warning disable 649

using UnityEngine;
using UnityEngine.UI;

namespace CC.DialogueSystem
{
    // Get notified of a theme change and search the repo for the right sprite
    public class ThemeChangeNotifier : MonoBehaviour
    {
        [SerializeField]
        protected string _elementsName;

        // Could subclass, but then it's not as easy to just throw one script onto an object
        protected Image _thisImage;
        private SpriteRenderer _thisSprite;

        #region MonoBehaviour

        // Initialize
        private void Start()
        {
            _thisImage = GetComponent<Image>();
            _thisSprite = GetComponent<SpriteRenderer>();

            // Check we've got a name before registering for the action
            if(string.IsNullOrEmpty(_elementsName))
            {
                DialogueLogger.LogError($"GameObject with the name {gameObject.name} needs an Elements Name value to be able to change theme");
                return;
            }
                
            if(_thisImage == null && _thisSprite)
            {
                DialogueLogger.LogError($"GameObject with the name {gameObject.name} needs an Image or Sprite Renderer component to be able to change theme");
                return;
            }

            DialogueController.Instance.ThemeChanged += findAndChangeSprite;
        }

        #endregion

        // Retrieve the sprite from the repo and apply it if it exists
        private void findAndChangeSprite(string name)
        {
            var tempSprite = getSprite(name, _elementsName);

            if(tempSprite == null)
            {
                DialogueLogger.LogError($"GameObject with the name {gameObject.name} can't change sprite with theme, {name} doesn't contain a sprite for {_elementsName}. Skipping");
                return;
            }

            // If we're just an image
            if(_thisImage != null)
                _thisImage.sprite = tempSprite;

            // If we're a sprite renderer
            if (_thisSprite != null)
                _thisSprite.sprite = tempSprite;
        }

        // Get the sprite from the repo
        protected virtual Sprite getSprite(string name, string spritename) => ThemeRepo.Instance.RetrieveSprite(name, spritename);
    }
}