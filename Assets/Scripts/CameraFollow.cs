using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Takip Ayarları")]
    public float smoothSpeed = 5f;             // takip hızı
    public Vector3 defaultOffset;              // varsayılan offset
    public string specialTargetName = "SpecialPlayer"; // özel isim
    public Vector3 specialOffset;              // özel offset

    [Header("Kamera Sınırları")]
    public float minX = -10f;
    public float maxX = 10f;
    public float minY = -5f;
    public float maxY = 5f;

    private Transform target;

    private void LateUpdate()
    {
        // Eğer target yoksa ya da artık Player tag’li değilse yeni Player bul
        if (target == null || !target.CompareTag("Player"))
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                target = playerObj.transform;
        }

        // Takip et
        if (target != null)
        {
            // Hedefin adı özel isimse özel offset kullan
            Vector3 currentOffset = target.name == specialTargetName ? specialOffset : defaultOffset;
            Vector3 desiredPosition = target.position + currentOffset;

            // X ve Y'yi sınırla
            float clampedX = Mathf.Clamp(desiredPosition.x, minX, maxX);
            float clampedY = Mathf.Clamp(desiredPosition.y, minY, maxY);

            Vector3 clampedPosition = new Vector3(clampedX, clampedY, desiredPosition.z);

            Vector3 smoothedPosition = Vector3.Lerp(transform.position, clampedPosition, smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;
        }
    }
}
