using UnityEngine;
using UnityEngine.EventSystems;

public class Slots : MonoBehaviour, IDropHandler, IPointerExitHandler
{
    [HideInInspector] public ItemBilgi currentItem; // slotun tuttuğu item

    // --- ITEM SLOT'A BIRAKILDIĞINDA ---
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;

        // Pozisyon eşitle
        var draggedRT = eventData.pointerDrag.GetComponent<RectTransform>();
        var myRT = GetComponent<RectTransform>();
        if (draggedRT != null && myRT != null)
            draggedRT.anchoredPosition = myRT.anchoredPosition;

        // Item bilgisini al
        var item = eventData.pointerDrag.GetComponent<ItemBilgi>();
        if (item == null)
        {
            Debug.LogWarning($"[{name}] Bırakılan objede ItemBilgi yok!");
            return;
        }

        // Slot artık bu itemi tutuyor
        currentItem = item;
        Debug.Log($"[{name}] Slot doldu: {item.ItemName} (ID:{item.ItemID})");

        // CraftingManager’a haber ver
        if (CraftingManager.Instance != null)
            CraftingManager.Instance.NotifySlotChanged();
    }

    // --- ITEM SLOTTAN ÇIKTIĞINDA ---
    public void OnPointerExit(PointerEventData eventData)
    {
        // Eğer slotun item’i varsa ve slotun üstünde sürüklenen item bu ise
        if (eventData.pointerDrag != null && currentItem != null)
        {
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
}
