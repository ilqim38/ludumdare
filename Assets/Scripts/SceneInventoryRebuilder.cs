using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class SceneInventoryRebuilder : MonoBehaviour
{
    [Header("Sahnedeki slotlar (sıra önemli)")]
    public Image[] slotImages;

    [Header("Item prefabları (üstlerinde ItemBilgi olmalı)")]
    public List<GameObject> itemPrefabs = new();

    [Header("UI item scale (şu an kullanılmıyor, hepsi 1 olacak)")]
    public Vector2 uiItemScale = new Vector2(1f, 1f);

    private Dictionary<int, GameObject> prefabById;

    private void Awake()
    {
        // Prefab -> ID sözlüğü
        prefabById = new Dictionary<int, GameObject>();
        foreach (var p in itemPrefabs)
        {
            if (p == null) continue;
            var info = p.GetComponent<ItemBilgi>();
            if (info == null)
            {
                Debug.LogWarning($"[SceneInventoryRebuilder] Prefab '{p.name}' üzerinde ItemBilgi yok.");
                continue;
            }
            prefabById[info.ItemID] = p;
        }
    }

    private void Start()
    {
        RebuildFromPersist();
    }

    /// <summary>
    /// InventoryPersist.slotIDs'e göre UI slotlarını sıfırdan kurar.
    /// </summary>
    public void RebuildFromPersist()
    {
        var persist = InventoryPersist.Instance;
        if (persist == null)
        {
            Debug.LogError("[SceneInventoryRebuilder] InventoryPersist bulunamadı!");
            return;
        }
        if (slotImages == null || slotImages.Length == 0)
        {
            Debug.LogError("[SceneInventoryRebuilder] slotImages boş!");
            return;
        }

        int count = Mathf.Min(slotImages.Length, persist.slotIDs.Length);

        // Slotları temizle
        for (int i = 0; i < count; i++)
        {
            var slotT = slotImages[i]?.transform;
            if (slotT == null) continue;

            for (int c = slotT.childCount - 1; c >= 0; c--)
                Destroy(slotT.GetChild(c).gameObject);

            var slotScript = slotT.GetComponent<EnvanterSlots>();
            if (slotScript != null) slotScript.ClearSlot();
        }

        // Persist verisine göre yeniden oluştur
        for (int i = 0; i < count; i++)
        {
            int id = persist.slotIDs[i]; // 0 = boş
            if (id == 0) continue;

            var slotT = slotImages[i]?.transform;
            if (slotT == null) continue;

            if (!prefabById.TryGetValue(id, out var prefab) || prefab == null)
            {
                Debug.LogWarning($"[SceneInventoryRebuilder] ID {id} için prefab bulunamadı.");
                continue;
            }

            var spawned = Instantiate(prefab, slotT);

            // 🔹 Ölçeği 1 yap (UI ve 3D fark etmeksizin)
            spawned.transform.localScale = Vector3.one;

            // 🔹 UI pozisyon ayarı
            var rt = spawned.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.anchoredPosition = Vector2.zero;

                // Z'yi sıfırla
                var lp = rt.localPosition;
                rt.localPosition = new Vector3(lp.x, lp.y, 0f);
            }
            else
            {
                // World space ise
                spawned.transform.position = slotT.position;
            }

            // Slot scriptini doldur
            var info = spawned.GetComponent<ItemBilgi>();
            var slotScript = slotT.GetComponent<EnvanterSlots>();
            if (slotScript != null)
            {
                slotScript.currentItem = info;
                slotScript.currentItemID = info != null ? info.ItemID : id;
            }
        }
    }
}
