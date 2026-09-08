using UnityEngine;
using TMPro;


// Shows Popup for wins and insufficient credits... Losses are silent, since it update the credits text and loss popup will be annoying to the player
public class PopupController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SlotMachine slotMachine;
    [SerializeField] private GameObject popupPanel;      // the WinPopup GameObject
    [SerializeField] private TMP_Text popupMessageText;    // Popup Panel MessageText inside it

    private void Awake()
    {
        // Makeing sure it starts hidden even if it was left active in the Editor 
        popupPanel.SetActive(false);
    }

    private void OnEnable()
    {
        slotMachine.onSpinResolved.AddListener(HandleSpinResolved);
    }

    private void OnDisable()
    {
        slotMachine.onSpinResolved.RemoveListener(HandleSpinResolved);
    }

    private void HandleSpinResolved(bool won, SlotSymbol matched, float multiplier)
    {
        if (!won) return; // losses update silently via the credits text - no popup needed

        string symbolName = matched != null ? matched.symbolName : "Symbol";
        ShowMessage($"You won {multiplier}x!\n(Three {symbolName}s)");
    }

    // Calling this from GameFlowController when a spin is requested without enough credits.
    public void ShowInsufficientCreditsMessage()
    {
        ShowMessage("Not enough credits!\nLower your bet or add credits.");
    }

    private void ShowMessage(string message)
    {
        popupMessageText.text = message;
        popupPanel.SetActive(true);
    }

    public void ClosePopup()
    {
        popupPanel.SetActive(false);
    }
}
