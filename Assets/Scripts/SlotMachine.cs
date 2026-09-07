using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;


// Top-level controller for the slot machine. Triggers all reels to spin, waits for them to finish, evaluates the win condition, and reports the result.
public class SlotMachine : MonoBehaviour
{
    [ContextMenu("Spin (for testing)")]
    public void TestSpin() => StartSpin();


    [Header("Reels")]
    [SerializeField] private List<Reel> reels;

    [Header("Events")]
    public UnityEvent onSpinStarted;
    public UnityEvent<bool, SlotSymbol, float> onSpinResolved; // won, matchedSymbol (or null), payoutMultiplier

    private bool spinInProgress;
    public bool SpinInProgress => spinInProgress;


   //Call this from our Spin button, Ignored if a spin is already running.

    public void StartSpin()
    {
        if (spinInProgress) return;

        spinInProgress = true;
        onSpinStarted?.Invoke();

        int remaining = reels.Count;

        foreach (var reel in reels)
        {
            reel.Spin(_ =>
            {
                remaining--;
                if (remaining == 0)
                {
                    EvaluateResult();
                }
            });
        }
    }

    private void EvaluateResult()
    {
        List<SlotSymbol> results = reels.Select(r => r.CurrentSymbol).ToList();

        // Win condition: all reels show the same symbol.
        bool won = results.All(s => s == results[0]);
        SlotSymbol matched = won ? results[0] : null;
        float multiplier = won ? matched.payoutMultiplier : 0f;

        spinInProgress = false;
        onSpinResolved?.Invoke(won, matched, multiplier);
    }
}
