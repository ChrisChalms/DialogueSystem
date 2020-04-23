using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Handles speaker's name box
public class NameBox : MonoBehaviour
{
    private Image _textBackground;
    private TextMeshProUGUI _nameText;

    private bool _isShowing;

    #region MonoBehaviour

    // Initialize
    private void Start()
    {
        _textBackground = GetComponent<Image>();
        _nameText = transform.Find("Text").GetComponent<TextMeshProUGUI>();
    }

    #endregion

    // Changes the name, hides if none
    public void SetName(string newName)
    {
        if (string.IsNullOrEmpty(newName))
        {
            hide();
            return;
        }

        _nameText.text = newName;

        if (!_isShowing)
            show();
    }

    // Hide box
    private void hide()
    {
        _isShowing = false;
        _textBackground.enabled = false;
        _nameText.text = string.Empty;
    }

    // Show box
    private void show()
    {
        _isShowing = true;
        _textBackground.enabled = true;
    }
}
