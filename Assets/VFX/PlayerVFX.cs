using UnityEngine;

public class PlayerVFX : MonoBehaviour
{
    public DescriptionManager descriptionManager;
    public GameManager gameManager;
    void Start()
    {
        GetComponent<WoodObjectVFX>().OnHover += ShowCardDescription;
        GetComponent<WoodObjectVFX>().OnHoverEnd += HideCardDescription;
    }

    void ShowCardDescription()
    {
        descriptionManager.HideDescription();
        descriptionManager.ChangeDescription("Player health: " + gameManager.PlayerHealth.ToString());
        descriptionManager.ShowDescription();
    }
    
    void HideCardDescription()
    {
        descriptionManager.HideDescription();
    }


}