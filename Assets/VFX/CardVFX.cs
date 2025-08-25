using UnityEngine;

public class CardVFX : MonoBehaviour
{
    public DescriptionManager descriptionManager;
    public CardData cardData;
    
    void Start()
    {
        GetComponent<WoodObjectVFX>().OnHover += ShowCardDescription;
        GetComponent<WoodObjectVFX>().OnHoverEnd += HideCardDescription;
    }

    void ShowCardDescription()
    {
        descriptionManager.HideDescription();
        descriptionManager.ChangeDescription(cardData.description);
        descriptionManager.ShowDescription();
    }
    
    void HideCardDescription()
    {
        descriptionManager.HideDescription();
    }


}
