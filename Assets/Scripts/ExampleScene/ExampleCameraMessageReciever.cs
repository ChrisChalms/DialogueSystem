using UnityEngine;

public class ExampleCameraMessageReciever : MonoBehaviour
{
    [SerializeField]
    private Color _nicePink;

    private Color _startingColour;

    #region MonoBehaviour

    // Initialize
    private void Start()
    {
        _startingColour = Camera.main.backgroundColor;
    }

    #endregion

    public void ChangeToPink() => Camera.main.backgroundColor = _nicePink;
    public void ResetColour() => Camera.main.backgroundColor = _startingColour;

}
