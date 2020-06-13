#pragma warning disable 649

using UnityEngine;
using UnityEngine.UI;

namespace CC.DialogueSystem
{
    // Get notified of a theme change and search the repo for the right sprite
    public class ButtonThemeChangeNotifier : ThemeChangeNotifier
    {
        private enum buttonStates
        {
            HIGHLIGHT,
            PRESSED,
            SELECTED,
            DISABLED
        }

        [SerializeField]
        private string _buttonElementName;
        [SerializeField]
        private buttonStates _buttonStateToChange;
        
        private Button _thisButton;

        #region MonoBehaviour

        // Initialize
        private void Start()
        {
            _thisButton = GetComponent<Button>();
            _thisImage = _thisButton?.image;

            // Check we've got a name before registering for the action
            if (string.IsNullOrEmpty(_elementsName) || string.IsNullOrEmpty(_buttonElementName))
            {
                DialogueLogger.LogError($"GameObject with the name {gameObject.name} needs an Elements Name and Button Element Name value to be able to change theme");
                return;
            }

            // Check we've got the components
            if (_thisImage == null || _thisButton == null)
            {
                DialogueLogger.LogError($"GameObject with the name {gameObject.name} needs an Image and Button component to be able to change button theme");
                return;
            }

            // Check the button is using sprites
            if(_thisButton.transition != Selectable.Transition.SpriteSwap)
            {
                DialogueLogger.LogError($"GameObject with the name {gameObject.name}'s Button component needs to be in SpriteSwap transition mode to be able to change button theme");
                return;
            }

            DialogueController.Instance.ThemeChanged += findAndChangeSprite;
        }

        #endregion

        // Retrieve the sprite from the repo and apply it if it exists
        private void findAndChangeSprite(string name)
        {
            var imageSprite = getSprite(name, _elementsName);
            var buttonSprite = getSprite(name, _buttonElementName);

            if (imageSprite == null)
                DialogueLogger.LogError($"GameObject with the name {gameObject.name} can't change sprite with theme, {name} doesn't contain a sprite for {_elementsName}. Skipping");
            else
                _thisImage.sprite = imageSprite;

            if (buttonSprite == null)
                DialogueLogger.LogError($"GameObject with the name {gameObject.name} can't change sprite with theme, {name} doesn't contain a sprite for {_buttonElementName}. Skipping");
            else
            {
                var tempState = new SpriteState();
                switch (_buttonStateToChange)
                {
                    case buttonStates.HIGHLIGHT: tempState.highlightedSprite = buttonSprite; break;
                    case buttonStates.PRESSED: tempState.pressedSprite = buttonSprite; break;
                    case buttonStates.SELECTED: tempState.selectedSprite = buttonSprite; break;
                    case buttonStates.DISABLED: tempState.disabledSprite = buttonSprite; break;
                }

                _thisButton.spriteState = tempState;
            }

        }
    }
}
