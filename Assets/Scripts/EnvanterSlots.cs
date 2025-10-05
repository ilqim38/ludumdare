using UnityEngine;
using UnityEngine.EventSystems;

[DisallowMultipleComponent]
public class EnvanterSlots : MonoBehaviour, IDropHandler
{
    [Header("Bu slotun tuttuğu item (Inspector'dan atayabilirsin)")]
    public ItemBilgi currentItem;

    private RectTransform slotRT;

    private void Awake()
    {
        slotRT = GetComponent<RectTransform>();
    }

    private void Start()
    {
        // 1) Eğer Inspector'dan currentItem atandıysa onu kullan
        if (currentItem == null && transform.childCount > 0)
        {
            // 2) Yoksa child varsa ondan doldur
            currentItem = transform.GetChild(0).GetComponent<ItemBilgi>();
        }

        // 3) Başlangıçta item varsa: parent = bu slot, pozisyon = slot merkezi
        if (currentItem != null)
        {
            SnapIntoThisSlot(currentItem);
        }
    }

    // Bu slota bırakıldığında
    public void OnDrop(PointerEventData eventData)
    {
        var go = eventData.pointerDrag;
        if (go == null) return;

        var item = go.GetComponent<ItemBilgi>();
        if (item == null) return;

        SnapIntoThisSlot(item);
        currentItem = item;
    }

    // ---------- Yardımcı ----------
    private void SnapIntoThisSlot(ItemBilgi item)
    {
        if (!item) return;

        var rtItem = item.GetComponent<RectTransform>();

        // Bu slotun child'ı yap
        item.transform.SetParent(transform, worldPositionStays: false);

        // UI ise anchoredPosition=0; değilse world position eşitle
        if (rtItem != null)
            rtItem.anchoredPosition = Vector2.zero;
        else
            item.transform.position = transform.position;
    }
}
