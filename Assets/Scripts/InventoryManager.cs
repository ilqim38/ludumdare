using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [Header("Envanter Slotları (UI Image / RectTransform)")]
    public Image[] slotKutulari = new Image[6];

    [Header("Item Prefabları (üstünde ItemBilgi olmalı)")]
    public List<GameObject> itemPrefabs = new List<GameObject>();

    // Her slotun tuttuğu ItemID (-1 = boş)
    private int[] slotItemIDs;

    public static InventoryManager AnaYonetici;

    private void Awake()
    {
        AnaYonetici = this;

        if (slotKutulari == null || slotKutulari.Length == 0)
            Debug.LogError("[InventoryManager] slotKutulari boş!", this);

        slotItemIDs = new int[slotKutulari.Length];
        for (int i = 0; i < slotItemIDs.Length; i++) slotItemIDs[i] = -1;
    }

    /// <summary>
    /// Pickup'tan gelen ItemBilgi’ye göre, ID’si eşleşen prefabı
    /// ilk boş slota instantiate eder ve slotun currentItemID’sini set eder.
    /// </summary>
    public void EsyaEkle(ItemBilgi picked)
    {
        if (picked == null)
        {
            Debug.LogWarning("[InventoryManager] EsyaEkle: picked NULL");
            return;
        }

        // 1) İlk boş slotu bul (ID -1 olan)
        int targetIndex = -1;
        for (int i = 0; i < slotItemIDs.Length; i++)
        {
            if (slotItemIDs[i] == -1)
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

        // 2) ID’ye göre prefab bul
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

        // 3) Instantiate et → slotu parent yap → merkezle
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

        // 4) Slot scriptine yaz: currentItem + currentItemID
        var slotScript = slotTransform.GetComponent<EnvanterSlots>();
        var spawnedInfo = spawned.GetComponent<ItemBilgi>();

        if (slotScript != null)
        {
            slotScript.currentItem   = spawnedInfo;
            slotScript.currentItemID = spawnedInfo != null ? spawnedInfo.ItemID : picked.ItemID;
        }

        // 5) Manager tarafında da ID’yi sakla
        slotItemIDs[targetIndex] = picked.ItemID;

        // (İsteğe bağlı) slot görselini aç
        if (slotKutulari[targetIndex] != null)
            slotKutulari[targetIndex].enabled = true;

        Debug.Log($"[InventoryManager] Eklendi: {(spawnedInfo ? spawnedInfo.ItemName : $"ID:{picked.ItemID}")} → Slot {targetIndex}");
    }

    // İstersen dışarıdan bir slotu boşaltmak için:
    public void ClearSlot(int index)
    {
        if (index < 0 || index >= slotKutulari.Length) return;

        var t = slotKutulari[index].transform;
        for (int i = t.childCount - 1; i >= 0; i--)
            Destroy(t.GetChild(i).gameObject);

        var slotScript = t.GetComponent<EnvanterSlots>();
        if (slotScript) slotScript.ClearSlot();

        slotItemIDs[index] = -1;
    }
}
