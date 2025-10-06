using UnityEngine;

public class ClickToPlayAndDisableWalls : MonoBehaviour
{
    [Header("Ses Ayarı")]
    [SerializeField] private AudioClip sesClip;  // Inspector’dan seçilecek ses klibi

    private AudioSource audioSource;

    private void OnMouseDown()
    {
        // 1️⃣ AudioSource yoksa ekle
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        // 2️⃣ Ses çal
        if (sesClip != null)
        {
            audioSource.clip = sesClip;
            audioSource.Play();
            Debug.Log("[ClickToPlayAndDisableWalls] Ses çalındı!");
        }
        else
        {
            Debug.LogWarning("[ClickToPlayAndDisableWalls] Ses clip'i atanmadı!");
        }

        // 3️⃣ Sahnedeki "duvar" tag’li nesneleri bul
        GameObject[] duvarObjeleri = GameObject.FindGameObjectsWithTag("duvar");

        int kapatilanSayisi = 0;

        foreach (GameObject obj in duvarObjeleri)
        {
            // Collider var mı kontrol et
            Collider col3D = obj.GetComponent<Collider>();
            Collider2D col2D = obj.GetComponent<Collider2D>();

            if (col3D != null && col3D.enabled)
            {
                col3D.enabled = false;
                kapatilanSayisi++;
            }
            else if (col2D != null && col2D.enabled)
            {
                col2D.enabled = false;
                kapatilanSayisi++;
            }
        }

        Debug.Log($"[ClickToPlayAndDisableWalls] {kapatilanSayisi} adet 'duvar' objesinin collider'ı kapatıldı.");
    }
}
