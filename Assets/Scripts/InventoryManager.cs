using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [Header("Envanter Slotları (UI Image veya RectTransform içeren objeler)")]
    public Image[] slotKutulari = new Image[6];

    [Header("Envantere Eklenecek Prefablar (üstünde ItemBilgi olmalı)")]
    public List<GameObject> itemPrefabs = new List<GameObject>();

    private bool[] slotDolumu;

    public static InventoryManager AnaYonetici;

    private void Awake()
    {
        AnaYonetici = this;

        if (slotKutulari == null || slotKutulari.Length == 0)
            Debug.LogError("[InventoryManager] slotKutulari boş!", this);

        slotDolumu = new bool[slotKutulari.Length];
    }

    /// <summary>
    /// Pickup'tan gelen ItemBilgi’ye göre, ID’si eşleşen prefabı
    /// ilk boş slota instantiate eder.
    /// </summary>
    public void EsyaEkle(ItemBilgi picked)
    {
        if (picked == null)
        {
            Debug.LogWarning("[InventoryManager] EsyaEkle: picked NULL");
            return;
        }

        // 1) İlk boş slotu bul
        int targetIndex = -1;
        for (int i = 0; i < slotDolumu.Length; i++)
        {
            if (!slotDolumu[i])
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

            // 🔹 SCALE BURADA AYARLANIYOR
            rt.localScale = new Vector3(75f, 75f, 1f);

            // UI'de Z'yi sıfırla
            var lp = rt.localPosition;
            rt.localPosition = new Vector3(lp.x, lp.y, 0f);
        }
        else
        {
            // UI değilse world position eşitle
            spawned.transform.position = slotTransform.position;

            // 🔹 WORLD SPACE NESNELER İÇİN DE SCALE 75x75
            spawned.transform.localScale = new Vector3(75f, 75f, 1f);
        }

        // 4) Slotu dolu işaretle
        slotDolumu[targetIndex] = true;

        if (slotKutulari[targetIndex] != null)
        {
            slotKutulari[targetIndex].enabled = true;
        }

        // Log
        var info = spawned.GetComponent<ItemBilgi>();
        Debug.Log($"[InventoryManager] Eklendi: {(info ? info.ItemName : $"ID:{picked.ItemID}")} → Slot {targetIndex}");
    }
}
