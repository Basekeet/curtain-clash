using TMPro;
using UnityEngine;

public class DescriptionManager : MonoBehaviour
{
    public GameObject description;
    public TextMeshProUGUI descriptionText;
    private WoodObjectVFX woodObjectVFX;
    
    
    void Awake()
    {
        woodObjectVFX = description.GetComponent<WoodObjectVFX>();
    }

    public void ChangeDescription(string description)
    {
        descriptionText.text = description;
    }

    public void ShowDescription()
    {
        woodObjectVFX.Appear();
    }

    public void HideDescription()
    {
        woodObjectVFX.Disappear();
    }

}
