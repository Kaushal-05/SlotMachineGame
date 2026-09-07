using UnityEngine;


// Represents a single slot symbol (e.g. Cherry, Bar, Seven).
// Created as a ScriptableObject asset so you can configure symbols
[CreateAssetMenu(fileName = "NewSlotSymbol", menuName = "Slot Game/Symbol")]
public class SlotSymbol : ScriptableObject
{
    [Header("Visuals")]
    public Sprite icon;
    public string symbolName;

    [Header("Payout")]
    [Tooltip("Multiplier applied to the current bet when 3 of this symbol line up.")]
    public float payoutMultiplier = 1f;

    [Header("Weighting")]
    [Tooltip("Higher weight = appears more often. Keep rare/high-value symbols low (e.g. 1-2), common ones high (e.g. 10-20).")]
    public int spinWeight = 10;
}
