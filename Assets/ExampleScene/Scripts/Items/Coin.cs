using UnityEngine;
using CC.DialogueSystem;

public class Coin : MonoBehaviour
{
    private static CoinCounter _counter;

    #region MonoBehaviour

    // Initialize
    private void Start() => _counter = GameObject.Find("GameCanvas/Coins").GetComponent<CoinCounter>();

    #endregion

    public void Collected()
    {
        // Don't collect if we're not on the quest for the wizzard
        if (!VariableRepo.Instance.Retrieve<bool>("onQuest"))
            return;

        _counter.AddCoin();
        Destroy(gameObject);
    }
}
