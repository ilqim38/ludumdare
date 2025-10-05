using UnityEngine;

[DisallowMultipleComponent]
public class InventoryPersist : MonoBehaviour
{
    public static InventoryPersist Instance { get; private set; }

    [Header("Toplam slot sayısı (UI ile aynı olmalı)")]
    public int slotCount = 6;

    // Her slotun tuttuğu item ID'si (0 = boş)
    [Tooltip("Index = slot indeksi, Değer = ItemID (0 = boş)")]
    public int[] slotIDs;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (slotIDs == null || slotIDs.Length != slotCount)
        {
            slotIDs = new int[slotCount]; // default 0 = boş
        }
    }

    // Dışarıdan güvenli set
    public void SetSlot(int index, int itemID)
    {
        if (!IsValidIndex(index)) return;
        slotIDs[index] = Mathf.Max(0, itemID); // 0'dan küçükse 0'a zorla
        // Debug.Log($"[InventoryPersist] slot[{index}] = {slotIDs[index]}");
    }

    public int GetSlot(int index)
    {
        if (!IsValidIndex(index)) return 0;
        return slotIDs[index];
    }

    public void ClearSlot(int index) => SetSlot(index, 0);

    public void ResizeIfNeeded(int newCount)
    {
        if (newCount <= 0) return;
        if (slotIDs == null) { slotIDs = new int[newCount]; return; }
        if (slotIDs.Length == newCount) return;

        var old = slotIDs;
        slotIDs = new int[newCount];
        int copy = Mathf.Min(old.Length, newCount);
        for (int i = 0; i < copy; i++) slotIDs[i] = old[i]; // kalanlar 0 (boş)
        slotCount = newCount;
    }

    private bool IsValidIndex(int i) => i >= 0 && slotIDs != null && i < slotIDs.Length;
}
