using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    public static CraftingManager Instance { get; private set; }

    [Header("Girdi Slotları (Inspector'dan ATAMAK ZORUNLU)")]
    public Slots slot1;
    public Slots slot2;

    [Header("Üretilebilecek Prefab'lar (ör.: c=ID3, f=ID6)")]
    public List<GameObject> craftableItems; // Prefabların üstünde ItemBilgi olacak

    [Header("Sonucun Spawn Noktası (yalnızca POZİSYON ve REFERANS için)")]
    public Transform craftSpawnPoint; // Hiyerarşide bunun HEMEN ALTINA (sibling) yerleştirilecek, child olmayacak

    // (minID,maxID) -> resultID
    private readonly Dictionary<(int, int), int> craftRecipes = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        if (!slot1) Debug.LogError("[CM] slot1 atanmamış!");
        if (!slot2) Debug.LogError("[CM] slot2 atanmamış!");
        if (!craftSpawnPoint) Debug.LogWarning("[CM] craftSpawnPoint atanmamış (world origin + scene root kullanılacak).");
    }

    private void Start()
    {
        // Tarifler
        craftRecipes[NormalizeKey(1, 2)] = 3; // a + b = c
        craftRecipes[NormalizeKey(4, 5)] = 6; // d + e = f

        if (craftableItems == null || craftableItems.Count == 0)
            Debug.LogWarning("[CM] craftableItems boş. ID=3(c) ve ID=6(f) prefablarını ekleyin.");
    }

    private (int, int) NormalizeKey(int a, int b) => a <= b ? (a, b) : (b, a);

    // Slot bir item alınca bunu çağırır
    public void NotifySlotChanged()
    {
        if (!slot1 || !slot2) return;

        var i1 = slot1.currentItem;
        var i2 = slot2.currentItem;

        // Biri boşsa bekle; hata verme
        if (i1 == null || i2 == null) return;

        TryCraft(i1, i2);
    }

    private void TryCraft(ItemBilgi item1, ItemBilgi item2)
    {
        var key = NormalizeKey(item1.ItemID, item2.ItemID);
        if (!craftRecipes.TryGetValue(key, out int resultID))
        {
            Debug.Log("Bu kombinasyonla craft yok.");
            return;
        }

        // Sonuç prefab'ını ID ile bul
        GameObject resultPrefab = craftableItems.Find(p =>
        {
            var info = p ? p.GetComponent<ItemBilgi>() : null;
            return info && info.ItemID == resultID;
        });

        if (resultPrefab == null)
        {
            Debug.LogWarning($"[CM] Result prefab bulunamadı! (ID:{resultID})");
            return;
        }

        // Girdileri kaldır
        if (item1) Destroy(item1.gameObject);
        if (item2) Destroy(item2.gameObject);
        slot1.currentItem = null;
        slot2.currentItem = null;

        // --- SONUÇ OLUŞTUR ---
        // Parent'ı, instantiate ANINDA craftSpawnPoint.parent yapıyoruz ki
        // DragDrop.Awake() içinde GetComponentInParent<Canvas>() çalışsın.
        Transform targetParent = craftSpawnPoint && craftSpawnPoint.parent
            ? craftSpawnPoint.parent
            : null; // parent yoksa scene root

        Vector3 spawnWorldPos = craftSpawnPoint ? craftSpawnPoint.position : Vector3.zero;

        // Overload: (prefab, position, rotation, parent)
        var newItem = Instantiate(resultPrefab, spawnWorldPos, Quaternion.identity, targetParent);

        // UI RectTransform ise world pozisyonu zaten eşitlendi; yine de garanti:
        var newRT = newItem.GetComponent<RectTransform>();
        var spRT  = craftSpawnPoint as RectTransform;
        if (newRT != null && spRT != null)
        {
            newRT.position = spRT.position; // UI
        }
        else
        {
            newItem.transform.position = spawnWorldPos; // 3D
        }

        // Hierarchy'de spawnPoint'in HEMEN ALTINA (sibling) gelecek şekilde index ayarla
        if (craftSpawnPoint && craftSpawnPoint.parent)
        {
            int targetIndex = craftSpawnPoint.GetSiblingIndex() + 1;
            newItem.transform.SetSiblingIndex(targetIndex);
        }
        else
        {
            // scene root'ta kalsın
            newItem.transform.SetParent(null, true);
        }

        var infoNew = newItem.GetComponent<ItemBilgi>();
        Debug.Log($"Craft başarılı! Üretilen: {(infoNew ? infoNew.ItemName : $"ID:{resultID}")}");
    }
}
