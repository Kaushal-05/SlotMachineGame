using UnityEngine;
using TMPro;


//Listens to PayoutManager's credit/bet changes and updates the on-screen TextMeshPro labels. Kept separate from PayoutManager itself so the economy logic doesn't need to know anything about UI Text components.

public class UIManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PayoutManager payoutManager;
    [SerializeField] private TMP_Text creditsText;
    [SerializeField] private TMP_Text betText;

    private void OnEnable()
    {
        payoutManager.onCreditsChanged.AddListener(UpdateCreditsText);
        payoutManager.onBetChanged.AddListener(UpdateBetText);
    }

    private void OnDisable()
    {
        payoutManager.onCreditsChanged.RemoveListener(UpdateCreditsText);
        payoutManager.onBetChanged.RemoveListener(UpdateBetText);
    }

    private void UpdateCreditsText(float value)
    {
        creditsText.text = $"Credits: {value}";
    }

    private void UpdateBetText(float value)
    {
        betText.text = $"Bet: {value}";
    }
}
