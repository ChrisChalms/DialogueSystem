using UnityEngine;

public class ExampleMessageReciever : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;

    #region MonoBehaviour
    private void Start() =>_spriteRenderer = GetComponent<SpriteRenderer>();

    #endregion

    public void ChangeColour() => _spriteRenderer.color = Color.magenta;
    public void ResetColour() => _spriteRenderer.color = Color.white;
}
