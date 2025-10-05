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

    // Geri döndürme için tutulacak bilgiler
    private Transform originalParent;
    private int originalSiblingIndex;
    private Vector2 originalAnchoredPos;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup   = GetComponent<CanvasGroup>();

        // 1) Parent zincirinden rootCanvas dene
        if (myCanvas == null)
            myCanvas = GetComponentInParent<Canvas>()?.rootCanvas;

        // 2) Hâlâ yoksa sahnede herhangi bir Canvas bul (instantiate edilen objeler için faydalı)
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
        // Orijinal konumu kaydet (envanter grid düzeni için kritik)
        originalParent       = transform.parent;
        originalSiblingIndex = transform.GetSiblingIndex();
        originalAnchoredPos  = rectTransform.anchoredPosition;

        // Eğer bir EnvanterSlots içindeysek, o slot kendini boş saysın
        var parentInventorySlot = GetComponentInParent<EnvanterSlots>();
        if (parentInventorySlot && parentInventorySlot.currentItem == GetComponent<ItemBilgi>())
            parentInventorySlot.currentItem = null;

        // Alttaki slotların raycast alabilmesi için kapat
        canvasGroup.blocksRaycasts = false;

        // Offset hesapla
        if (parentRect &&
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRect, eventData.position, eventData.pressEventCamera, out var localPoint))
        {
            pointerOffset = rectTransform.anchoredPosition - localPoint;
        }

        // Çizim sırasını öne al (opsiyonel ama faydalı)
        transform.SetAsLastSibling();
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
        // Raycast'leri geri aç
        canvasGroup.blocksRaycasts = true;

        // Bırakılan hedef GEÇERLİ slot mu? (EnvanterSlots veya Slots)
        bool droppedOnValid =
            HasComponentInParents<EnvanterSlots>(eventData.pointerEnter) ||
            HasComponentInParents<Slots>(eventData.pointerEnter);

        // Geçerli slot yoksa → eski yerine geri dön
        if (!droppedOnValid)
        {
            transform.SetParent(originalParent, worldPositionStays: false);
            transform.SetSiblingIndex(originalSiblingIndex);
            rectTransform.anchoredPosition = originalAnchoredPos;
            // Debug.Log("[DragDrop] Geçerli slot yok → eski yerine döndü.");
        }

        // Not: Geçerli slota bırakıldıysa ilgili slot scripti (OnDrop)
        // zaten pozisyonu/parent'ı doğru set edecektir.
    }

    // ----------------- Helpers -----------------
    private bool HasComponentInParents<T>(GameObject obj) where T : Component
    {
        if (obj == null) return false;
        return obj.GetComponentInParent<T>() != null;
    }
}
