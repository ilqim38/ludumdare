using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    // Elmanın kendi görseli
    private Sprite kendiGorselim; 

    void Start()
    {
        // Elmanın görselini al ve değişkene kaydet.
        kendiGorselim = GetComponent<SpriteRenderer>().sprite;
    }

    // Başka bir nesne bana dokunduğunda çalışır
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Eğer bana dokunan nesnenin etiketi "Player" ise...
        if (other.CompareTag("Player"))
        {
            // 1. Yöneticiye haber ver ve Elma görselini gönder.
            // AnaYonetici sayesinde InventoryManager'a ulaşıyoruz.
            InventoryManager.AnaYonetici.EsyaEkle(kendiGorselim);

            // 2. Elmayı sahneden sil (kaybolsun).
            Destroy(gameObject); 
        }
    }
}