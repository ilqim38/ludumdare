using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerSceneChange : MonoBehaviour
{
    [SerializeField] private AudioClip audioClip;
    private AudioSource persistentAudio;

    private void Start()
    {
        // Ses çalacak kalıcı objeyi oluştur
        GameObject audioObj = new GameObject("PersistentAudio");
        persistentAudio = audioObj.AddComponent<AudioSource>();

        persistentAudio.clip = audioClip;
        persistentAudio.loop = false; // 🔁 sahne geçse bile çalmaya devam etsin
        persistentAudio.playOnAwake = false;
        persistentAudio.volume = 0.8f;

        DontDestroyOnLoad(audioObj); // 🔒 sahne değişse de silinmesin
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Sesi başlat
            if (!persistentAudio.isPlaying)
                persistentAudio.Play();

            // Sahneyi değiştir
            SceneManager.LoadScene("tezgah");
        }
    }
}
