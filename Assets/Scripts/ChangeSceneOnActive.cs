using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneOnActive : MonoBehaviour
{
    [Tooltip("Geçilecek sahnenin adı (Build Settings'te ekli olmalı)")]
    public string sceneName = "NextScene";

    private void OnEnable()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning("ChangeSceneOnActive: sceneName boş!");
        }
    }
}
