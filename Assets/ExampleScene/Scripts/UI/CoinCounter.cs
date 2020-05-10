using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CoinCounter : MonoBehaviour
{
    private readonly float FADE_TWEEN_TIME = 0.3f;

    private Image _coinIcon;
    private Text _amountText;

    #region MonoBehaviour

    // Initialize
    void Start()
    {
        _coinIcon = GetComponent<Image>();
        _amountText = GetComponentInChildren<Text>();
    }

    #endregion

    // We've started the quest so show coins counter
    public void ShowCoins()
    {
        // We've got DOTween so might aswell use it
        _coinIcon.DOFade(1f, FADE_TWEEN_TIME);
        _amountText.DOFade(1f, FADE_TWEEN_TIME);
    }

    // Adds a single coin to the amount
    public void AddCoin() => addCoins(1);

    // Add 10 coins to the total
    public void Add17Coins() => addCoins(17);

    // Retrieve, increment, then put back
    private void addCoins(int amount)
    {
        var coinAmount = DialogueVariableRepo.Instance.Retrieve<int>("playerGold");
        coinAmount += amount;
        _amountText.text = $"x {coinAmount}";
        DialogueVariableRepo.Instance.Register("playerGold", coinAmount);
    }

    // Reset the coins
    public void ResetCoins()
    {
        _amountText.text = "x 0";
        DialogueVariableRepo.Instance.Register("playerGold", 0);
    }
}
