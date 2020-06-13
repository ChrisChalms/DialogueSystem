using System.Collections;
using System.Collections.Generic;
using CC.DialogueSystem;
using UnityEngine;
using UnityEngine.UI;

public class OptionsButtonController : MonoBehaviour
{
    private readonly WaitForSeconds DELAY_BETWEEN_BUTTONS = new WaitForSeconds(0.1f);

    private HorizontalLayoutGroup _hzGroup;
    private OptionButton[] _buttons;

    private bool _isShowingOptions;

    public bool IsShowingOptions => _isShowingOptions;

    #region MonoBehaviour

    // Start is called before the first frame update
    private void Start()
    {
        _hzGroup = GetComponent<HorizontalLayoutGroup>();
        _buttons = GetComponentsInChildren<OptionButton>();

        _hzGroup.enabled = false;
    }

    #endregion

    // Update the buttons' texts and animate on if we have to
    public IEnumerator ShowOptions(List<Option> options)
    {
        _hzGroup.enabled = false;
        _isShowingOptions = true;

        // Turn buttons on/off if they're needed or not
        for (var i = 0; i < _buttons.Length; i++)
        {
            if (i < options.Count)
            {
                StartCoroutine(_buttons[i].SetText(options[i].Text));
                _buttons[i].Enable();
            }
            else
                _buttons[i].Disable();
        }

        // Wait a frame for the buttons' content fitters to change the size, enable the _hzGroup to get the buttons' x positions, wait a frame for the changes to be made, disable the _hzGroup
        // There's got to be a better way of doing this
        yield return null;
        _hzGroup.enabled = true;
        yield return null;
        _hzGroup.enabled = false;
        yield return null;

        // Animate the buttons to their guide objects with a short delay between
        foreach (var button in _buttons)
        {
            if (button.gameObject.activeSelf)
                button.Show();

            yield return DELAY_BETWEEN_BUTTONS;
        }
    }

    // Animate the buttons off screen
    public void HideOptions()
    {
        _isShowingOptions = false;
        foreach (var button in _buttons)
            button.Hide();
    }
}