using UnityEngine;
using UnityEngine.UI; 

public class InventoryManager : MonoBehaviour
{
    // 3 adet envanter kutusu görseli.
    public Image[] slotKutulari = new Image[3]; 
    
    // Hangi kutunun dolu olduğunu kaydeden basit bir liste (3 tane False ile başlar).
    private bool[] slotDolumu = new bool[3]; 

    // Bütün oyunun bu yöneticiye kolayca ulaşması için bir "anahtar" oluşturuyoruz.
    public static InventoryManager AnaYonetici;

    private void Awake()
    {
        AnaYonetici = this;
    }

    // Elma alındığında çağrılan fonksiyon
    public void EsyaEkle(Sprite elmaGorseli)
    {
        // 3 kutuyu da sırayla kontrol et
        for (int i = 0; i < slotKutulari.Length; i++)
        {
            // Eğer kutu boşsa (yani False ise)...
            if (slotDolumu[i] == false)
            {
                // Kutuyu DOLDUR (True yap)
                slotDolumu[i] = true; 

                // Kutunun içine Elma görselini koy ve göster
                slotKutulari[i].sprite = elmaGorseli;
                slotKutulari[i].enabled = true; 

                return; // İlk boş kutuyu buldu, işimiz bitti, fonksiyondan çık.
            }
        }
        // Buraya gelindiyse, 3 kutu da doludur.
        Debug.Log("ENVANTER DOLU!");
    }
}