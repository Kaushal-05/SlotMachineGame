using UnityEngine;
using UnityEngine.UI;


// Small Script that ties PayoutManager (economy) and SlotMachine (game logic) together for UI input. Exists so Button.onClick / lever taps can trigger a single method that does "check bet, then spin" —
// a UnityEvent alone can't do that conditional chaining from the Inspector.
public class GameFlowController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SlotMachine slotMachine;
    [SerializeField] private PayoutManager payoutManager;

    [Header("Optional UI to disable while spinning")]
    [SerializeField] private Button spinButton;
    [SerializeField] private Button betUpButton;
    [SerializeField] private Button betDownButton;

    private void OnEnable()
    {
        slotMachine.onSpinStarted.AddListener(HandleSpinStarted);
        slotMachine.onSpinResolved.AddListener(HandleSpinResolved);
    }

    private void OnDisable()
    {
        slotMachine.onSpinStarted.RemoveListener(HandleSpinStarted);
        slotMachine.onSpinResolved.RemoveListener(HandleSpinResolved);
    }

    //Calling this from the Spin button's OnClick() or from LeverController.
    public void RequestSpin()
    {
        if (slotMachine.SpinInProgress) return;

        if (!payoutManager.TryPlaceBet())
        {
            Debug.Log("Not enough credits to spin.");
            // Hook a "not enough credits" popup here if you want one.
            return;
        }

        slotMachine.StartSpin();
    }

    private void HandleSpinStarted()
    {
        SetInteractable(false);
    }

    private void HandleSpinResolved(bool won, SlotSymbol matched, float multiplier)
    {
        payoutManager.HandleSpinResult(won, matched, multiplier);
        SetInteractable(true);
    }

    private void SetInteractable(bool value)
    {
        if (spinButton) spinButton.interactable = value;
        if (betUpButton) betUpButton.interactable = value;
        if (betDownButton) betDownButton.interactable = value;
    }
}
