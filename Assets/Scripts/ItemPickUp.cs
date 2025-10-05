using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    private ItemBilgi _bilgi;

    private void Awake()
    {
        _bilgi = GetComponent<ItemBilgi>();
        if (_bilgi == null)
            Debug.LogError("[ItemPickUp] Üstünde ItemBilgi yok!", this);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (InventoryManager.AnaYonetici == null)
        {
            Debug.LogError("[ItemPickUp] InventoryManager.AnaYonetici yok!");
            return;
        }

        // Artık sprite değil, doğrudan ItemBilgi gönderiyoruz
        InventoryManager.AnaYonetici.EsyaEkle(_bilgi);

        Destroy(gameObject);
    }
}
