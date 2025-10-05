using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private AudioSource footstepAudio; // 🎧 Adım sesi (loop açık olmalı)
    
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator animator;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Eğer AudioSource atanmadıysa karakterin üzerinden bulmayı dene
        if (footstepAudio == null)
            footstepAudio = GetComponent<AudioSource>();
    }

    private void Update()
    {
        // W-A-S-D tuşlarını oku
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput.Normalize(); // çapraz basınca fazla hız olmasın

        // Hareket uygula
        rb.linearVelocity = moveInput * moveSpeed;

        // Yürüme durumu
        bool isWalking = moveInput != Vector2.zero;
        animator.SetBool("isWalking", isWalking);

        // Yön değerleri
        if (isWalking)
        {
            animator.SetFloat("LastInputX", moveInput.x);
            animator.SetFloat("LastInputY", moveInput.y);
        }

        animator.SetFloat("InputX", moveInput.x);
        animator.SetFloat("InputY", moveInput.y);

        // 🎧 Ses kontrolü
        HandleFootstepSound(isWalking);
    }

    private void HandleFootstepSound(bool isWalking)
    {
        if (footstepAudio == null) return;

        if (isWalking)
        {
            if (!footstepAudio.isPlaying)
                footstepAudio.Play(); // başlamamışsa çal
        }
        else
        {
            if (footstepAudio.isPlaying)
                footstepAudio.Stop(); // durduysa kapat
        }
    }
}
