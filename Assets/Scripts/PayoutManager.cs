using UnityEngine;
using UnityEngine.Events;


//Manages player credits and bet amount Subscribes to SlotMachine's onSpinResolved event rather than SlotMachine knowing about credits directly

public class PayoutManager : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private float startingCredits = 100f;
    [SerializeField] private float betAmount = 5f;

    [Header("Events")]
    public UnityEvent<float> onCreditsChanged;
    public UnityEvent<float> onBetChanged;

    public float Credits { get; private set; }
    public float BetAmount => betAmount;

    private void Awake()
    {
        Credits = startingCredits;
    }

    private void Start()
    {
        onCreditsChanged?.Invoke(Credits);
        onBetChanged?.Invoke(betAmount);
    }

    //Call before spinning to deduct the bet. Returns false if insufficient credits
    public bool TryPlaceBet()
    {
        if (Credits < betAmount) return false;

        Credits -= betAmount;
        onCreditsChanged?.Invoke(Credits);
        return true;
    }

    // SlotMachine's onSpinResolved event
    public void HandleSpinResult(bool won, SlotSymbol matchedSymbol, float payoutMultiplier)
    {
        if (!won) return;

        float payout = betAmount * payoutMultiplier;
        Credits += payout;
        onCreditsChanged?.Invoke(Credits);
    }

    public void SetBetAmount(float amount)
    {
        betAmount = Mathf.Max(1f, amount);
        onBetChanged?.Invoke(betAmount);
    }

    // Connected this directly to the BetUpButton's OnClick().
    public void IncreaseBet() => SetBetAmount(betAmount + 5f);

    // Connected this directly to the BetDownButton's OnClick().
    public void DecreaseBet() => SetBetAmount(betAmount - 5f);
}
