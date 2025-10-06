using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MamudoSon : MonoBehaviour
{
    [Header("Ses Ayarları")]
    [SerializeField] private AudioClip sesClip;

    [Header("Görsel Ayarları")]
    [SerializeField] private string fadeImageTag = "FadeImage";
    [SerializeField] private float fadeSuresi = 2f;
    [SerializeField] private Color fadeRenk = Color.black;

    [Header("Sahne Ayarları")]
    [SerializeField] private string hedefSahneAdi;
    [SerializeField] private int hedefSahneIndex = -1; // -1 ise isim kullanılır

    private AudioSource audioSource;
    private Image fadeImage;

    void Start()
    {
        // FadeImage tag'ine sahip Image'ı bul
        GameObject fadeObj = GameObject.FindGameObjectWithTag(fadeImageTag);
        if (fadeObj != null)
        {
            fadeImage = fadeObj.GetComponent<Image>();
            if (fadeImage != null)
            {
                // Fade görselini en üste taşı
                fadeImage.transform.SetAsLastSibling();

                // Başlangıç alfa = 0, rengi fadeRenk olsun
                fadeImage.color = new Color(fadeRenk.r, fadeRenk.g, fadeRenk.b, 0f);
                fadeImage.raycastTarget = false; // butonlar tıklanabilir
                Debug.Log("FadeImage bulundu ve hazırlandı!");
            }
            else
            {
                Debug.LogError("FadeImage tag'ine sahip objede Image component'i yok!");
            }
        }
        else
        {
            Debug.LogError("FadeImage tag'ine sahip obje bulunamadı!");
        }

        // AudioSource component'ini ekle
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = sesClip;
        audioSource.playOnAwake = false;

        // Sesi çal
        if (sesClip != null)
        {
            audioSource.Play();
            StartCoroutine(SesiBekleVeFade());
        }
        else
        {
            Debug.LogWarning("Ses clip'i atanmamış!");
        }
    }

    IEnumerator SesiBekleVeFade()
    {
        // Ses bitene kadar bekle
        yield return new WaitWhile(() => audioSource.isPlaying);

        // Fade başlat
        if (fadeImage != null)
        {
            float gecenZaman = 0f;
            Color baslangicRenk = fadeImage.color;

            while (gecenZaman < fadeSuresi)
            {
                gecenZaman += Time.unscaledDeltaTime;
                float normalizeZaman = Mathf.Clamp01(gecenZaman / fadeSuresi);

                Color yeniRenk = baslangicRenk;
                yeniRenk.a = Mathf.Lerp(0f, 1f, normalizeZaman);
                fadeImage.color = yeniRenk;

                // Yavaş yavaş kararıyor ama tıklama %90'dan sonra engelleniyor
                fadeImage.raycastTarget = normalizeZaman >= 0.9f;

                yield return null;
            }

            // Tamamen karardı → tıklamayı engelle
            Color sonRenk = fadeImage.color;
            sonRenk.a = 1f;
            fadeImage.color = sonRenk;
            fadeImage.raycastTarget = true;
        }
        else
        {
            Debug.LogError("Fade image bulunamadı, sahne geçişi yapılıyor!");
        }

        // Sahne geçişi
        SahneyiDegistir();
    }

    void SahneyiDegistir()
    {
        if (hedefSahneIndex >= 0)
        {
            SceneManager.LoadScene(hedefSahneIndex);
        }
        else if (!string.IsNullOrEmpty(hedefSahneAdi))
        {
            SceneManager.LoadScene(hedefSahneAdi);
        }
        else
        {
            Debug.LogError("Hedef sahne belirlenmemiş!");
        }
    }
}
