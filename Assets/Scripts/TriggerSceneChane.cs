using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerSceneChane : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            SceneManager.LoadScene("tezgah");
        }
    }
}
