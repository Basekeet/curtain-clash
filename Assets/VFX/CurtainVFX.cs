using UnityEngine;
using DG.Tweening; // Требуется DOTween для плавной анимации

public class CurtainVFX : MonoBehaviour
{
    public GameObject curtainLeft;
    public GameObject curtainRight;
    public GameObject light;
    
    [Header("Настройки анимации")]
    public float animationDuration = 1f;
    public Vector3 openPosition = new Vector3(2f, 0, 0);
    public Ease easingType = Ease.InOutQuad;
    
    private Vector3 leftCurtainClosedPosition;
    private Vector3 rightCurtainClosedPosition;

    private void Start()
    {
        // Сохраняем начальные позиции штор
        if (curtainLeft != null) leftCurtainClosedPosition = curtainLeft.transform.localPosition;
        if (curtainRight != null) rightCurtainClosedPosition = curtainRight.transform.localPosition;
    }

    public void CloseCurtains()
    {
        if (curtainLeft != null)
        {
            curtainLeft.transform.DOLocalMove(leftCurtainClosedPosition, animationDuration)
                .SetEase(easingType);
        }
        
        if (curtainRight != null)
        {
            curtainRight.transform.DOLocalMove(rightCurtainClosedPosition, animationDuration)
                .SetEase(easingType);
        }
        light.GetComponent<SpriteRenderer>().DOFade(1f, animationDuration);
    }

    public void OpenCurtains()
    {
        if (curtainLeft != null)
        {
            curtainLeft.transform.DOLocalMove(leftCurtainClosedPosition - openPosition, animationDuration)
                .SetEase(easingType);
        }
        
        if (curtainRight != null)
        {
            curtainRight.transform.DOLocalMove(rightCurtainClosedPosition + openPosition, animationDuration)
                .SetEase(easingType);
        }

        light.GetComponent<SpriteRenderer>().DOFade(0f, animationDuration);
    }
}