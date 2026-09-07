using UnityEngine;
using UnityEngine.Events;


// Manages player credits and bet amount. Subscribes to SlotMachine's
// onSpinResolved event rather than SlotMachine knowing about credits directly.... keeps game logic and economy logic decoupled.
public class PayoutManager : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private float startingCredits = 100f;
    [SerializeField] private float betAmount = 5f;

    [Header("Events (hook these to your UI text)")]
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

    // Call before spinning to deduct the bet. Returns false if insufficient credits
    public bool TryPlaceBet()
    {
        if (Credits < betAmount) return false;

        Credits -= betAmount;
        onCreditsChanged?.Invoke(Credits);
        return true;
    }

    //Hook this to SlotMachine's onSpinResolved even
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
}
