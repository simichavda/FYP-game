using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SnapScroll : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    public ScrollRect scrollRect;
    public RectTransform contentPanel;
    public float snapSpeed = 10f;
    public float swipeThreshold = 50f;

    private bool isDragging = false;
    private int currentIndex = 0;
    private Vector2 startPos;

    void Start()
    {
        // Auto-get references if not set
        if (!scrollRect) scrollRect = GetComponent<ScrollRect>();
        if (!contentPanel && scrollRect) contentPanel = scrollRect.content;
    }

    void Update()
    {
        if (!isDragging)
        {
            SnapToElement();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        startPos = contentPanel.anchoredPosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        Vector2 endPos = contentPanel.anchoredPosition;
        float delta = startPos.x - endPos.x;

        // Determine swipe direction
        if (Mathf.Abs(delta) > swipeThreshold)
        {
            if (delta > 0)
            {
                currentIndex = Mathf.Clamp(currentIndex + 1, 0, contentPanel.childCount - 1);
            }
            else
            {
                currentIndex = Mathf.Clamp(currentIndex - 1, 0, contentPanel.childCount - 1);
            }
        }
    }

    void SnapToElement()
    {
        float elementWidth = contentPanel.rect.width / contentPanel.childCount;
        float targetPosition = -currentIndex * elementWidth;

        Vector2 newPos = new Vector2(
            Mathf.Lerp(contentPanel.anchoredPosition.x, targetPosition, snapSpeed * Time.deltaTime),
            contentPanel.anchoredPosition.y
        );

        contentPanel.anchoredPosition = newPos;
    }

    // Optional: Manual navigation buttons
    public void NextItem() => currentIndex = Mathf.Clamp(currentIndex + 1, 0, contentPanel.childCount - 1);
    public void PreviousItem() => currentIndex = Mathf.Clamp(currentIndex - 1, 0, contentPanel.childCount - 1);
}