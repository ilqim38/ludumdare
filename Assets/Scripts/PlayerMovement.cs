using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator animator;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // W-A-S-D tuşlarını oku
        moveInput.x = Input.GetAxisRaw("Horizontal"); // A/D veya Sol/Sağ ok
        moveInput.y = Input.GetAxisRaw("Vertical");   // W/S veya Yukarı/Aşağı ok
        moveInput.Normalize(); // çapraz basınca fazla hız olmasın

        // Rigidbody2D hareketi
        rb.linearVelocity = moveInput * moveSpeed;

        // Yürüme animasyonu aktif mi?
        bool isWalking = moveInput != Vector2.zero;
        animator.SetBool("isWalking", isWalking);

        // Hareket varsa yönleri animatöre gönder
        if (isWalking)
        {
            animator.SetFloat("LastInputX", moveInput.x);
            animator.SetFloat("LastInputY", moveInput.y);
        }

        // Her frame yön değerlerini güncelle
        animator.SetFloat("InputX", moveInput.x);
        animator.SetFloat("InputY", moveInput.y);
    }
}
