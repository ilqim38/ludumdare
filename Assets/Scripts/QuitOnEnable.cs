using UnityEngine;

public class QuitOnEnable : MonoBehaviour
{
    [Tooltip("Bu obje aktif olunca oyunu kapat.")]
    public bool quitWhenEnabled = true;

    [Tooltip("Kapanmadan önce bekleme süresi (saniye). 0 ise hemen kapanır.")]
    public float delaySeconds = 0f;

    private void OnEnable()
    {
        if (!quitWhenEnabled) return;
        if (delaySeconds <= 0f)
        {
            QuitNow();
        }
        else
        {
            Invoke(nameof(QuitNow), delaySeconds);
        }
    }

    public void QuitNow()
    {
#if UNITY_EDITOR
        // Editor’da Play modunu durdurur (build’de Application.Quit çalışır).
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
