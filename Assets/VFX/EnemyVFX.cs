using UnityEngine;

public class EnemyVFX : MonoBehaviour
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
        descriptionManager.ChangeDescription("Enemy health: " + gameManager.EnemyHealth.ToString());
        descriptionManager.ShowDescription();
    }
    
    void HideCardDescription()
    {
        descriptionManager.HideDescription();
    }


}