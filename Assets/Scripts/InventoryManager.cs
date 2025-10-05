using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class InventoryManager : MonoBehaviour
{
    [Header("Envanter Slotları (UI Image / RectTransform)")]
    public Image[] slotKutulari = new Image[6];

    [Header("Item Prefabları (üstünde ItemBilgi olmalı)")]
    public List<GameObject> itemPrefabs = new List<GameObject>();

    private int[] slotItemIDs;
    public static InventoryManager AnaYonetici;

    private void Awake()
    {
        AnaYonetici = this;

        if (slotKutulari == null || slotKutulari.Length == 0)
            Debug.LogError("[InventoryManager] slotKutulari boş!", this);

        slotItemIDs = new int[slotKutulari.Length];
        for (int i = 0; i < slotItemIDs.Length; i++) slotItemIDs[i] = 0;

        if (InventoryPersist.Instance != null)
            InventoryPersist.Instance.ResizeIfNeeded(slotKutulari.Length);
    }

    private void Update()
    {
        // Her frame slotları kontrol et → Persist'e yansıt
        if (InventoryPersist.Instance == null) return;

        for (int i = 0; i < slotKutulari.Length; i++)
        {
            var slotT = slotKutulari[i].transform;

            if (slotT.childCount > 0)
            {
                var child = slotT.GetChild(0).GetComponent<ItemBilgi>();
                int id = (child != null) ? child.ItemID : 0;

                slotItemIDs[i] = id;
                InventoryPersist.Instance.SetSlot(i, id);
            }
            else
            {
                slotItemIDs[i] = 0;
                InventoryPersist.Instance.SetSlot(i, 0);
            }
        }
    }

    public void EsyaEkle(ItemBilgi picked)
    {
        if (picked == null)
        {
            Debug.LogWarning("[InventoryManager] EsyaEkle: picked NULL");
            return;
        }

        // 1️⃣ İlk boş slot (ID 0 olan)
        int targetIndex = -1;
        for (int i = 0; i < slotItemIDs.Length; i++)
        {
            if (slotItemIDs[i] == 0)
            {
                targetIndex = i;
                break;
            }
        }

        if (targetIndex == -1)
        {
            Debug.Log("ENVANTER DOLU!");
            return;
        }

        // 2️⃣ Prefab bul
        GameObject prefab = itemPrefabs.Find(p =>
        {
            var ib = p ? p.GetComponent<ItemBilgi>() : null;
            return ib != null && ib.ItemID == picked.ItemID;
        });

        if (prefab == null)
        {
            Debug.LogError($"[InventoryManager] ID {picked.ItemID} için prefab bulunamadı!");
            return;
        }

        // 3️⃣ Instantiate → parent → merkez
        var slotTransform = slotKutulari[targetIndex].transform;
        GameObject spawned = Instantiate(prefab, slotTransform);

        var rt = spawned.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = Vector2.zero;
            rt.localScale = new Vector3(75f, 75f, 1f);
            var lp = rt.localPosition;
            rt.localPosition = new Vector3(lp.x, lp.y, 0f);
        }
        else
        {
            spawned.transform.position = slotTransform.position;
            spawned.transform.localScale = new Vector3(75f, 75f, 1f);
        }

        // 4️⃣ Slot script
        var slotScript = slotTransform.GetComponent<EnvanterSlots>();
        var spawnedInfo = spawned.GetComponent<ItemBilgi>();
        if (slotScript != null)
        {
            slotScript.currentItem = spawnedInfo;
            slotScript.currentItemID = spawnedInfo != null ? spawnedInfo.ItemID : picked.ItemID;
        }

        // 5️⃣ Local ve Persist’e yaz
        slotItemIDs[targetIndex] = picked.ItemID;
        InventoryPersist.Instance?.SetSlot(targetIndex, picked.ItemID);

        // (Opsiyonel) Görünürlük
        slotKutulari[targetIndex].enabled = true;

        Debug.Log($"[InventoryManager] Eklendi: {picked.ItemName} → Slot {targetIndex}");
    }

    public void ClearSlot(int index)
    {
        if (index < 0 || index >= slotKutulari.Length) return;

        var t = slotKutulari[index].transform;
        for (int i = t.childCount - 1; i >= 0; i--)
            Destroy(t.GetChild(i).gameObject);

        var slotScript = t.GetComponent<EnvanterSlots>();
        if (slotScript)
        {
            slotScript.ClearSlot();
        }

        slotItemIDs[index] = 0;
        InventoryPersist.Instance?.ClearSlot(index);
    }
}
