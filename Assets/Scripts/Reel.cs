using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// Controls a single slot reel using a small ring of recycled symbol images.. visually similar to scrolling a tall strip through a masked viewport (like a ScrollRect), but driven by one continuous eased motion instead of...
// physics/drag, so it can land on an EXACT symbol every time.



// Phase A: all symbols scroll continuously at a steady pace; any symbol that scrolls past the bottom of the window is recycled to the top with a new random sprite 
// Phase B: a short corrective ease brings the closest symbol to exactly rest position and assigns it the real result guarantees a clean landing regardless of where Phase A happened 

public class Reel : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image symbolDisplay;
    [SerializeField] private List<SlotSymbol> symbolPool;

    [Header("Spin Feel")]
    [Tooltip("How long the free-scrolling phase runs before the landing correction.")]
    [SerializeField] private float spinDuration = 3.1f;
    [Tooltip("How long the final corrective snap-to-landing takes.")]
    [SerializeField] private float alignDuration = 0.3f;
    [Tooltip("Scroll speed during the free-spinning phase, in UI units/second.")]
    [SerializeField] private float scrollSpeed =  1000f;
    [Tooltip("Vertical spacing between symbols - should roughly match your reel window's height. Leave 0 to auto-read from symbolDisplay's own height.")]
    [SerializeField] private float cellHeight = 0f;
    [Tooltip("Delay in seconds before this reel starts spinning (used to stagger reels).")]
    [SerializeField] private float startDelay = 0f;
    [Tooltip("How far past rest the landed symbol dips before settling back, for a small 'thunk' feel.")]
    [SerializeField] private float landingOvershoot = 10f;
    [Tooltip("How many symbol images to keep in the recycled ring. 3-4 is plenty for a single-symbol window.")]
    [SerializeField] private int poolSize = 4;

    private Image[] cells;
    private float[] baseY;
    private RectTransform reelRect;
    private Vector2 restPosition;
    private float sharedOffset;

    private SlotSymbol currentSymbol;
    public SlotSymbol CurrentSymbol => currentSymbol;

    private bool isSpinning;
    public bool IsSpinning => isSpinning;

    private void Awake()
    {
        reelRect = (RectTransform)transform;
        restPosition = symbolDisplay.rectTransform.anchoredPosition;

        if (cellHeight <= 0f)
        {
            cellHeight = symbolDisplay.rectTransform.rect.height;
        }

        BuildCellPool();
    }

    private void BuildCellPool()
    {
        cells = new Image[poolSize];
        baseY = new float[poolSize];

        cells[0] = symbolDisplay;
        for (int i = 1; i < poolSize; i++)
        {
            GameObject clone = Instantiate(symbolDisplay.gameObject, symbolDisplay.transform.parent);
            clone.name = symbolDisplay.name + "_" + i;
            cells[i] = clone.GetComponent<Image>();
        }

        for (int i = 0; i < poolSize; i++)
        {
            baseY[i] = restPosition.y + i * cellHeight;
            SetCellY(i, baseY[i]);
        }
    }

    private void SetCellY(int index, float y)
    {
        Vector2 pos = cells[index].rectTransform.anchoredPosition;
        pos.y = y;
        cells[index].rectTransform.anchoredPosition = pos;
    }

    
    // Starts the spin animation and lands on a weighted0random symbol
    // Calls onComplete when the reel has fully stopped
   
    public void Spin(System.Action<Reel> onComplete)
    {
        StopAllCoroutines();
        StartCoroutine(SpinRoutine(onComplete));
    }

    private IEnumerator SpinRoutine(System.Action<Reel> onComplete)
    {
        isSpinning = true;

        if (startDelay > 0f)
            yield return new WaitForSeconds(startDelay);

        // Decide the result up front - the animation is purely visual,
        // the outcome is already fair and fixed before we show anything.
        SlotSymbol result = SymbolRandomizer.GetWeightedRandomSymbol(symbolPool);

        // --- Phase A: continuous free scroll ---
        float elapsed = 0f;
        while (elapsed < spinDuration)
        {
            elapsed += Time.deltaTime;
            sharedOffset -= scrollSpeed * Time.deltaTime;
            ApplyOffsetAndRecycle();
            yield return null;
        }

        // --- Phase B: snap the nearest symbol exactly onto rest position ---
        int landingIndex = FindClosestCellToRest();
        symbolDisplay = cells[landingIndex]; // keep reference pointing at whichever cell is now "the" display
        cells[landingIndex].sprite = result.icon;

        float targetOffset = restPosition.y - baseY[landingIndex];
        yield return EaseSharedOffset(targetOffset, alignDuration);

        // Small settle bounce instead of stopping dead.
        yield return EaseSharedOffset(targetOffset - landingOvershoot, 0.06f);
        yield return EaseSharedOffset(targetOffset, 0.1f);

        currentSymbol = result;
        isSpinning = false;
        onComplete?.Invoke(this);
    }

    private void ApplyOffsetAndRecycle()
    {
        float exitThreshold = restPosition.y - cellHeight;

        for (int i = 0; i < poolSize; i++)
        {
            float y = baseY[i] + sharedOffset;

            if (y < exitThreshold)
            {
                baseY[i] += poolSize * cellHeight;
                cells[i].sprite = SymbolRandomizer.GetWeightedRandomSymbol(symbolPool).icon;
                y = baseY[i] + sharedOffset;
            }

            SetCellY(i, y);
        }
    }

    private int FindClosestCellToRest()
    {
        int closest = 0;
        float bestDist = Mathf.Infinity;

        for (int i = 0; i < poolSize; i++)
        {
            float y = baseY[i] + sharedOffset;
            float dist = Mathf.Abs(y - restPosition.y);
            if (dist < bestDist)
            {
                bestDist = dist;
                closest = i;
            }
        }

        return closest;
    }

    private IEnumerator EaseSharedOffset(float target, float duration)
    {
        float start = sharedOffset;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float eased = 1f - Mathf.Pow(1f - t, 2f); // ease-out
            sharedOffset = Mathf.LerpUnclamped(start, target, eased);
            ApplyOffsetAndRecycle();
            yield return null;
        }

        sharedOffset = target;
        ApplyOffsetAndRecycle();
    }
}
