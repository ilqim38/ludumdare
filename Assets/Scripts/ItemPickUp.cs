using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    private ItemBilgi _bilgi;

    [Header("Ses Ayarları")]
    [SerializeField] private AudioClip pickupSound; // 🎧 Alma sesi
    [SerializeField, Range(0f, 1f)] private float pickupVolume = 0.7f; // Ses seviyesi

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

        // Envantere ekle
        InventoryManager.AnaYonetici.EsyaEkle(_bilgi);

        // 🎵 Alma sesini çal (tek seferlik, sahnede 3D pozisyondan)
        if (pickupSound != null)
            AudioSource.PlayClipAtPoint(pickupSound, transform.position, pickupVolume);

        // Nesneyi yok et
        Destroy(gameObject);
    }
}
