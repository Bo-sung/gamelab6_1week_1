using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class AugmentButton : MonoBehaviour
{
    string augmentName;
    string description;
    int index;

    public System.Action<int> Onclicked;
    [SerializeField]
    Button button;
    [SerializeField]
    TextMeshProUGUI nameText;
    [SerializeField]
    TextMeshProUGUI descriptionText;
    private void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();
        if (nameText == null)
            nameText = GetComponentInChildren<TextMeshProUGUI>();
        if (descriptionText == null)
            descriptionText = GetComponentInChildren<TextMeshProUGUI>();
        button.onClick.AddListener(() => Onclicked?.Invoke(index));
    }

    public void SetAugmentData(string name, string desc, int idx)
    {
        augmentName = name;
        description = desc;
        index = idx;

        if (nameText != null)
            nameText.text = augmentName;
        if (descriptionText != null)
            descriptionText.text = description;
    }
}
