using UnityEngine;

public class AnimationEventActivator : MonoBehaviour
{
    [SerializeField] GameObject targetToToggle;   // Inspector
    [SerializeField] string fallbackTagOrName;    // Yedek arama

    void OnEnable() {
        if (targetToToggle == null && !string.IsNullOrEmpty(fallbackTagOrName)) {
            targetToToggle = GameObject.FindWithTag(fallbackTagOrName);
            if (!targetToToggle) targetToToggle = GameObject.Find(fallbackTagOrName);
        }
    }

    public void ActivateObject()   { if (targetToToggle) targetToToggle.SetActive(true);  }
    public void DeactivateObject() { if (targetToToggle) targetToToggle.SetActive(false); }
}
