using UnityEngine;
using UnityEngine.SceneManagement; 
public class MenuManager : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene(1); 
    }

    public void QuitGame()
    {
        Debug.Log("Oyundan Çıkış Yapıldı!");
        
        // Uygulamayı KAPIYOZ İŞTE
        Application.Quit();
    }
}