using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class InventorySyncManager : MonoBehaviour
{
    [Header("Sahnedeki envanter slotları (UI Image veya RectTransform)")]
    public Image[] slotImages;

    private InventoryPersist persist;

    private void Start()
    {
        persist = InventoryPersist.Instance;

        if (persist == null)
        {
            //Debug.LogError("[InventorySyncManager] InventoryPersist bulunamadı!");
            enabled = false;
            return;
        }

        if (slotImages == null || slotImages.Length == 0)
        {
            Debug.LogError("[InventorySyncManager] slotImages boş!");
            enabled = false;
            return;
        }

        // Persist slot sayısını eşitle
        persist.ResizeIfNeeded(slotImages.Length);
    }

    private void Update()
    {
        // Her frame slotları kontrol et
        for (int i = 0; i < slotImages.Length; i++)
        {
            var slotTransform = slotImages[i]?.transform;
            if (slotTransform == null) continue;

            // Eğer child varsa (envanterde bir item var)
            if (slotTransform.childCount > 0)
            {
                var item = slotTransform.GetChild(0).GetComponent<ItemBilgi>();
                int id = (item != null) ? item.ItemID : 0;
                persist.SetSlot(i, id); // Persist’e bildir
            }
            else
            {
                persist.ClearSlot(i); // Slot boşsa persist’te 0 yap
            }
        }
    }
}
