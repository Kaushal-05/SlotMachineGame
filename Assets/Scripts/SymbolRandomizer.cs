using System.Collections.Generic;
using UnityEngine;

public static class SymbolRandomizer
{
    // Picks a random symbol from the pool, respecting each symbol's spinWeight using UnityEngine.Random.Range.

    public static SlotSymbol GetWeightedRandomSymbol(List<SlotSymbol> symbolPool)
    {
        if (symbolPool == null || symbolPool.Count == 0)
        {
            Debug.LogError("SymbolRandomizer: symbol pool is empty.");
            return null;
        }

        int totalWeight = 0;
        foreach (var symbol in symbolPool)
        {
            totalWeight += Mathf.Max(1, symbol.spinWeight);
        }

        int roll = Random.Range(0, totalWeight);
        int cumulative = 0;

        foreach (var symbol in symbolPool)
        {
            cumulative += Mathf.Max(1, symbol.spinWeight);
            if (roll < cumulative)
            {
                return symbol;
            }
        }

        // Fallback (should not happen given the loop above)
        return symbolPool[symbolPool.Count - 1];
    }
}
