using UnityEngine;
using UnityEngine.EventSystems;

public class Slots : MonoBehaviour, IDropHandler, IPointerExitHandler
{
    public ItemBilgi currentItem; // slotun tuttuğu item

    // --- ITEM SLOT'A BIRAKILDIĞINDA ---
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;

        var itemObj = eventData.pointerDrag;
        var item = itemObj.GetComponent<ItemBilgi>();
        if (item == null)
        {
            Debug.LogWarning($"[{name}] Bırakılan objede ItemBilgi yok!");
            return;
        }

        // Item slotun child'ı olsun
        itemObj.transform.SetParent(transform, worldPositionStays: false);

        // Pozisyonu slot merkezine hizala
        var draggedRT = itemObj.GetComponent<RectTransform>();
        var myRT = GetComponent<RectTransform>();
        if (draggedRT != null && myRT != null)
            draggedRT.anchoredPosition = Vector2.zero;
        else
            itemObj.transform.position = transform.position;

        // Slot artık bu item’i tutuyor
        currentItem = item;
        Debug.Log($"[{name}] Slot doldu: {item.ItemName} (ID:{item.ItemID})");

        // CraftingManager’a haber ver
        if (CraftingManager.Instance != null)
            CraftingManager.Instance.NotifySlotChanged();
    }

    // --- ITEM SLOTTAN ÇIKTIĞINDA ---
    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null || currentItem == null) return;

        var draggedItem = eventData.pointerDrag.GetComponent<ItemBilgi>();
        if (draggedItem == currentItem)
        {
            currentItem = null;
            Debug.Log($"[{name}] Slot boşaldı (item dışarı çıktı).");

            if (CraftingManager.Instance != null)
                CraftingManager.Instance.NotifySlotChanged();
        }
    }
}
