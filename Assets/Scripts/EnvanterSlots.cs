using UnityEngine;
using UnityEngine.EventSystems;

[DisallowMultipleComponent]
public class EnvanterSlots : MonoBehaviour, IDropHandler
{
    [Header("Bu slotun tuttuğu Item (component)")]
    public ItemBilgi currentItem;

    [Header("Bu slotun tuttuğu Item ID (yoksa -1)")]
    public int currentItemID = -1;

    private RectTransform slotRT;

    private void Awake() { slotRT = GetComponent<RectTransform>(); }

    private void Start()
    {
        // Başlangıçta child varsa oradan doldur (opsiyonel)
        if (currentItem == null && transform.childCount > 0)
            currentItem = transform.GetChild(0).GetComponent<ItemBilgi>();

        if (currentItem != null)
        {
            SnapIntoThisSlot(currentItem);
            currentItemID = currentItem.ItemID;
        }
        else
        {
            currentItemID = -1;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        var go = eventData.pointerDrag;
        if (!go) return;

        var item = go.GetComponent<ItemBilgi>();
        if (!item) return;

        SnapIntoThisSlot(item);
        currentItem   = item;
        currentItemID = item.ItemID;
    }

    private void SnapIntoThisSlot(ItemBilgi item)
    {
        var rtItem = item.GetComponent<RectTransform>();

        // child yap
        item.transform.SetParent(transform, worldPositionStays: false);

        if (rtItem != null) // UI
        {
            rtItem.anchoredPosition = Vector2.zero;
            var lp = rtItem.localPosition;
            rtItem.localPosition = new Vector3(lp.x, lp.y, 0f);
        }
        else // 3D
        {
            float keepZ = item.transform.position.z;
            item.transform.position = new Vector3(transform.position.x, transform.position.y, keepZ);
        }
    }

    // Slotu boşaltmak için yardımcı
    public void ClearSlot()
    {
        if (transform.childCount > 0)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
                Destroy(transform.GetChild(i).gameObject);
        }
        currentItem   = null;
        currentItemID = -1;
    }
}
