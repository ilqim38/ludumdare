using UnityEngine;
using UnityEngine.EventSystems;

[DisallowMultipleComponent]
public class EnvanterSlots : MonoBehaviour, IDropHandler
{
    public ItemBilgi currentItem;
    private RectTransform slotRT;

    private void Awake() { slotRT = GetComponent<RectTransform>(); }

    private void Start()
    {
        if (currentItem == null && transform.childCount > 0)
            currentItem = transform.GetChild(0).GetComponent<ItemBilgi>();

        if (currentItem != null) SnapIntoThisSlot(currentItem);
    }

    public void OnDrop(PointerEventData eventData)
    {
        var go = eventData.pointerDrag;
        if (!go) return;

        var item = go.GetComponent<ItemBilgi>();
        if (!item) return;

        SnapIntoThisSlot(item);
        currentItem = item;
    }

    private void SnapIntoThisSlot(ItemBilgi item)
    {
        var rtItem = item.GetComponent<RectTransform>();

        // 1) child yap
        item.transform.SetParent(transform, worldPositionStays: false);

        if (rtItem != null) // UI
        {
            // 2) merkeze koy
            rtItem.anchoredPosition = Vector2.zero;

            // 3) Z'yi sabitle (UI'de genelde 0 tutulur)
            var lp = rtItem.localPosition;
            rtItem.localPosition = new Vector3(lp.x, lp.y, 0f);
        }
        else // 3D / world space
        {
            // Eski Z'yi koru (istersen slotZ kullan)
            float keepZ = item.transform.position.z;               // koru
            // float keepZ = transform.position.z;                 // slot'un Z'sine ata (tercih edersen)

            item.transform.position = new Vector3(transform.position.x,
                                                  transform.position.y,
                                                  keepZ);
        }
    }
}
