using UnityEngine;
using UnityEngine.EventSystems;

public class Slots : MonoBehaviour, IDropHandler, IPointerExitHandler
{
    public ItemBilgi currentItem;

    public void OnDrop(PointerEventData eventData)
    {
        if (!eventData.pointerDrag) return;

        var itemObj = eventData.pointerDrag;
        var item    = itemObj.GetComponent<ItemBilgi>();
        if (!item)
        {
            Debug.LogWarning($"[{name}] Bırakılan objede ItemBilgi yok!");
            return;
        }

        // child yap
        itemObj.transform.SetParent(transform, worldPositionStays: false);

        var draggedRT = itemObj.GetComponent<RectTransform>();
        var myRT      = GetComponent<RectTransform>();

        if (draggedRT != null && myRT != null) // UI
        {
            // merkeze hizala
            draggedRT.anchoredPosition = Vector2.zero;

            // Z'yi 0'a sabitle (UI)
            var lp = draggedRT.localPosition;
            draggedRT.localPosition = new Vector3(lp.x, lp.y, 0f);
        }
        else // 3D / world space
        {
            // Eski Z'yi koru (veya slot'un Z'si)
            float keepZ = itemObj.transform.position.z;             // koru
            // float keepZ = transform.position.z;                  // slot'un Z'si (istersen)

            itemObj.transform.position = new Vector3(transform.position.x,
                                                     transform.position.y,
                                                     keepZ);
        }

        currentItem = item;
        CraftingManager.Instance?.NotifySlotChanged();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!eventData.pointerDrag || currentItem == null) return;

        var draggedItem = eventData.pointerDrag.GetComponent<ItemBilgi>();
        if (draggedItem == currentItem)
        {
            currentItem = null;
            CraftingManager.Instance?.NotifySlotChanged();
        }
    }
}
