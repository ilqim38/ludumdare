using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class DragDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private Canvas myCanvas; // atanmazsa Awake'te otomatik bulunur
    private RectTransform rectTransform;
    private RectTransform parentRect;
    private Vector2 pointerOffset;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup   = GetComponent<CanvasGroup>();

        // 1) Önce parent zincirinden rootCanvas dene
        if (myCanvas == null)
            myCanvas = GetComponentInParent<Canvas>()?.rootCanvas;

        // 2) Hâlâ yoksa sahnede herhangi bir Canvas bul (özellikle yeni instantiate edilen objeler için)
#if UNITY_2023_1_OR_NEWER
        if (myCanvas == null)
            myCanvas = FindAnyObjectByType<Canvas>(FindObjectsInactive.Exclude)?.rootCanvas;
#else
        if (myCanvas == null)
            myCanvas = FindObjectOfType<Canvas>()?.rootCanvas;
#endif

        parentRect = myCanvas ? myCanvas.transform as RectTransform : rectTransform.parent as RectTransform;

        if (parentRect == null)
            Debug.LogError("[DragDrop] Canvas/parentRect bulunamadı. Lütfen objeyi bir Canvas altında kullanın.", this);
    }

    public void OnPointerDown(PointerEventData eventData) { }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (canvasGroup) canvasGroup.blocksRaycasts = false;

        if (parentRect &&
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRect, eventData.position, eventData.pressEventCamera, out var localPoint))
        {
            pointerOffset = rectTransform.anchoredPosition - localPoint;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (parentRect &&
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRect, eventData.position, eventData.pressEventCamera, out var localPoint))
        {
            rectTransform.anchoredPosition = localPoint + pointerOffset;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (canvasGroup) canvasGroup.blocksRaycasts = true;
    }
}
