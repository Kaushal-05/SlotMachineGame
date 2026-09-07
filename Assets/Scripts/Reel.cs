using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


// Controls a single slot reel -> scrolling animation, landing on a randomly chosen symbol, and exposing that result to the SlotMachine controller.

public class Reel : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image symbolDisplay;
    [SerializeField] private List<SlotSymbol> symbolPool;

    [Header("Spin Feel")]
    [Tooltip("How many symbols flash past before landing, for visual effect.")]
    [SerializeField] private int spinCycles = 12;
    [Tooltip("Total spin duration in seconds for THIS reel.")]
    [SerializeField] private float spinDuration = 1.2f;
    [Tooltip("Delay in seconds before this reel starts spinning (used to stagger reels).")]
    [SerializeField] private float startDelay = 0f;

    private SlotSymbol currentSymbol;
    public SlotSymbol CurrentSymbol => currentSymbol;

    private bool isSpinning;
    public bool IsSpinning => isSpinning;

    
    // Starts the spin animation and lands on a weighted-random symbol. Calls onComplete when the reel has fully stopped.
    
    
    public void Spin(System.Action<Reel> onComplete)
    {
        StartCoroutine(SpinRoutine(onComplete));
    }

    private IEnumerator SpinRoutine(System.Action<Reel> onComplete)
    {
        isSpinning = true;

        if (startDelay > 0f)
            yield return new WaitForSeconds(startDelay);

        // Decide the result up front — the animation is purely visual,
        // the outcome is already fair and fixed before we show anything.
        SlotSymbol result = SymbolRandomizer.GetWeightedRandomSymbol(symbolPool);

        // Flash through random symbols with easing: fast at first, slowing near the end.
        float elapsed = 0f;
        int cyclesShown = 0;
        float cycleInterval = spinDuration / spinCycles;

        while (elapsed < spinDuration)
        {
            // Ease-out: intervals get longer as we approach the end (slows down).
            float progress = elapsed / spinDuration;
            float easedInterval = cycleInterval * (1f + progress * 2f);

            SlotSymbol flashSymbol = SymbolRandomizer.GetWeightedRandomSymbol(symbolPool);
            symbolDisplay.sprite = flashSymbol.icon;

            yield return new WaitForSeconds(easedInterval);
            elapsed += easedInterval;
            cyclesShown++;
        }

        // Land on the actual result.
        currentSymbol = result;
        symbolDisplay.sprite = currentSymbol.icon;

        isSpinning = false;
        onComplete?.Invoke(this);
    }

}
    