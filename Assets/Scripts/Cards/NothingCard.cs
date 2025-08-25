using System.Collections;
using UnityEngine;

public class NothingCard : BaseCard
{
    
    public override IEnumerator Activate()
    {
        yield break;
    }

    public override bool CanActivate()
    {
        return true;
    }

    public override IEnumerator UpdateCard()
    {
        yield break;
    }

}