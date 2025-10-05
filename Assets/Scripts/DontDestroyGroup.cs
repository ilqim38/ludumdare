using UnityEngine;

[DisallowMultipleComponent]
public class DontDestroyGroup : MonoBehaviour
{
    [Header("Kimi kalıcı yapalım? (boşsa bu objenin kendisi)")]
    public Transform targetRoot;

    [Tooltip("targetRoot'u da işin içine kat (önerilir).")]
    public bool includeSelf = true;

    [Tooltip("Deaktif çocukları da tarayalım mı?")]
    public bool includeInactive = true;

    private void Awake()
    {
        if (targetRoot == null) targetRoot = transform;
        MarkHierarchy(targetRoot);
    }

    // Bu obje altına sonradan çocuk eklenirse otomatik işaretle
    private void OnTransformChildrenChanged()
    {
        if (targetRoot == null) return;
        // Sadece yeni gelen/eklenenleri de kapsasın diye tekrar işaretlemek güvenli.
        MarkHierarchy(targetRoot);
    }

    /// <summary>
    /// Verilen kökün kendisini (opsiyonel) ve tüm çocuklarını DontDestroyOnLoad yapar.
    /// </summary>
    public void MarkHierarchy(Transform root)
    {
        if (root == null) return;

        if (includeSelf)
            DontDestroyOnLoad(root.gameObject);

        // tüm çocukları tara
        var allChildren = root.GetComponentsInChildren<Transform>(includeInactive);
        foreach (var t in allChildren)
        {
            if (!includeSelf && t == root) continue;
            DontDestroyOnLoad(t.gameObject);
        }
    }

    /// <summary>
    /// İstersen dışarıdan da dinamik kayıt:
    /// DontDestroyGroup.Register(someGameObject, true, true);
    /// </summary>
    public static void Register(GameObject go, bool includeChildren = true, bool includeInactive = true)
    {
        if (go == null) return;

        DontDestroyOnLoad(go);

        if (includeChildren)
        {
            var trs = go.GetComponentsInChildren<Transform>(includeInactive);
            foreach (var t in trs)
                DontDestroyOnLoad(t.gameObject);
        }
    }
}
