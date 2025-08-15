using TMPro;
using UnityEngine;

public class VersionSetter : MonoBehaviour
{
    void Start()
    {
        var versionText = GetComponent<TextMeshProUGUI>();
        versionText.text = "v" + Application.version;
    }
}
