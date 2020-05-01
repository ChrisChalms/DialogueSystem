using UnityEngine;
using DG.Tweening;

public class ExampleCharacterMessageReciever : MonoBehaviour
{
    private Vector2 GREEN_SHOW_POSITION = new Vector3(16.06f, 2.98f, 0);
    private float GREEN_TWEEN_TIME = 0.5f;

    private Vector3 _startingPos;

    #region MonoBehaviour

    // Initialize
    private void Start()
    {
        _startingPos = transform.position;
    }

    #endregion

    public void MoveIntoView() => transform.DOMove(GREEN_SHOW_POSITION, GREEN_TWEEN_TIME);
    public void ResetPosition() => transform.DOMove(_startingPos, GREEN_TWEEN_TIME);
}
