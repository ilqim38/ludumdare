using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    
    public float hareketHizi = 5f; 

        private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Fizik hesaplamaları burada yapılır.
    void FixedUpdate()
    {
        // 1. Kullanıcıdan Girdiyi Al
        float yatayGirdi = Input.GetAxis("Horizontal"); // A/D (Sol/Sağ)
        float dikeyGirdi = Input.GetAxis("Vertical");   // W/S (Yukarı/Aşağı)

        // 2. Hareket Vektörünü Hesapla
        Vector2 hareketVektoru = new Vector2(yatayGirdi, dikeyGirdi);
        
        // Çapraz hareket hızını sabit tutmak için normalleştirme 
        hareketVektoru.Normalize();

        rb.linearVelocity = hareketVektoru * hareketHizi;
    }
}