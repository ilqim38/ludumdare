using UnityEngine;
using UnityEngine.EventSystems;

public class ClickToPlayAndDisableWallsUI : MonoBehaviour, IPointerClickHandler
{
    [Header("Ses Ayarı")]
    [SerializeField] private AudioClip sesClip;      // Inspector’dan ata
    [SerializeField, Range(0f,1f)] private float volume = 1f;

    [Header("Hedefler")]
    [SerializeField] private string duvarTag = "duvar";   // Tag adı
    [SerializeField] private bool includeChildren = true; // Çocuk colliderları da kapat

    // UI Image tıklaması burada gelir
    public void OnPointerClick(PointerEventData eventData)
    {
        // 1) Sesi ayrı bir host üzerinde çal (obje yok olsa da ses kesilmesin)
        if (sesClip != null)
        {
            var host = new GameObject("OneShotAudioHost_UI");
            var src  = host.AddComponent<AudioSource>();
            src.clip = sesClip;
            src.volume = Mathf.Clamp01(volume);
            src.playOnAwake = false;
            src.loop = false;
            src.spatialBlend = 0f;         // 2D ses (UI için ideal)
            DontDestroyOnLoad(host);       // Sahne değişse de klip bitsin
            src.Play();
            Destroy(host, sesClip.length + 0.1f); // Klip bitince host’u temizle
        }
        else
        {
            Debug.LogWarning("[ClickToPlayAndDisableWallsUI] Ses clip'i atanmadı!");
        }

        // 2) 'duvar' tag'li objeleri bul ve colliderlarını kapat
        GameObject[] duvarlar;
        try
        {
            duvarlar = GameObject.FindGameObjectsWithTag(duvarTag);
        }
        catch
        {
            Debug.LogError($"[ClickToPlayAndDisableWallsUI] Tag bulunamadı: '{duvarTag}'. Project Settings > Tags & Layers'tan ekleyin.");
            return;
        }

        int kapatilan = 0;

        foreach (var go in duvarlar)
        {
            // 3D colliderlar
            if (includeChildren)
            {
                foreach (var c in go.GetComponentsInChildren<Collider>(false))
                {
                    if (c.enabled) { c.enabled = false; kapatilan++; }
                }
            }
            else
            {
                var c = go.GetComponent<Collider>();
                if (c != null && c.enabled) { c.enabled = false; kapatilan++; }
            }

            // 2D colliderlar
            if (includeChildren)
            {
                foreach (var c in go.GetComponentsInChildren<Collider2D>(false))
                {
                    if (c.enabled) { c.enabled = false; kapatilan++; }
                }
            }
            else
            {
                var c = go.GetComponent<Collider2D>();
                if (c != null && c.enabled) { c.enabled = false; kapatilan++; }
            }
        }

        Debug.Log($"[ClickToPlayAndDisableWallsUI] '{duvarTag}' tag'li objelerde {kapatilan} collider kapatıldı.");

        // 3) Tıklanan UI objesini yok et (ses ayrı host’ta çalmaya devam eder)
        Destroy(gameObject);
    }
}
