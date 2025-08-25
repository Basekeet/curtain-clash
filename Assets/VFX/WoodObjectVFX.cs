using System;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class WoodObjectVFX : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Z Swing (влево-вправо)")]
    public float swingAngleZ = 5f;
    public float swingDurationZ = 0.8f;

    [Header("Y Bob (вверх-вниз)")]
    public float swingYOffset = 10f;
    public float swingDurationY = 0.5f;

    [Header("Disappear Settings")]
    public float disappearYOffset = 1000f;
    public float disappearDuration = 1f;

    [Header("Hover Settings")]
    public float hoverScale = 1.2f;
    public float scaleDuration = 0.2f;

    public float shakeDuration = 0.2f;
    public float shakeStrength = 2.0f;
    public int shakeVibrato = 20;
    
    private Vector3 originalPos;
    private Vector3 originalScale;
    private Tween zTween;
    private Tween yTween;
    private Tween moveTween;
    private Tween scaleTween;

    public delegate void OnHoverHandler();
    public delegate void OnHoverEndHandler();
    public delegate void OnClickHandler();
    
    public event OnHoverHandler OnHover;
    public event OnHoverEndHandler OnHoverEnd;
    public event OnClickHandler OnClick;


    public bool IsAppearOnStart = true;
    
    private void Start()
    {
        originalPos = transform.localPosition;
        originalScale = transform.localScale;

        if (IsAppearOnStart)
        {
            StartSwinging();    
        }
        else
        {
            Vector3 targetPos = originalPos + new Vector3(0, 1 * disappearYOffset, 0);
            transform.localPosition = targetPos;
        }

        // Disappear();
        // StartSwinging();
    }

    public bool IsShaking = false;

    public void Shake(float strength=1.0f)
    {
        if (IsShaking) return;
        IsShaking = true;
        Vector3 originalPosition = transform.localPosition;
        transform.DOShakePosition(shakeDuration, strength * shakeStrength, shakeVibrato, 90f)
            .OnComplete((() =>
            {
                transform.localPosition = originalPosition;
                IsShaking = false;
            }
    ));
}
    
    private void StartSwinging()
    {
        StopSwinging();

        zTween = DOTween.Sequence()
            .Append(transform.DOLocalRotate(new Vector3(0, 0, swingAngleZ), swingDurationZ).SetEase(Ease.InOutSine))
            .Append(transform.DOLocalRotate(new Vector3(0, 0, -swingAngleZ), swingDurationZ).SetEase(Ease.InOutSine))
            .SetLoops(-1, LoopType.Yoyo);

        float randomStart = Random.Range(0f, swingDurationZ);
        zTween.Goto(randomStart, true);
        
        yTween = DOTween.Sequence()
            .Append(transform.DOLocalMoveY(originalPos.y + swingYOffset, swingDurationY).SetEase(Ease.InOutSine))
            .Append(transform.DOLocalMoveY(originalPos.y - swingYOffset, swingDurationY).SetEase(Ease.InOutSine))
            .SetLoops(-1, LoopType.Yoyo);
        
        randomStart = Random.Range(0f, swingDurationY);
        yTween.Goto(randomStart, true);
    }

    private void StopSwinging()
    {
        zTween?.Kill();
        yTween?.Kill();
    }

    private void StopMove()
    {
        moveTween?.Kill();
        IsDisappearing = false;
    }

    public bool IsDisappearing = false;

    public void Disappear(bool goUp = true)
    {
        if (IsDisappearing) return;
        StopMove();
        StopSwinging();
        IsDisappearing = true;

        float direction = goUp ? 1f : -1f;
        Vector3 targetPos = originalPos + new Vector3(0, direction * disappearYOffset, 0);

        moveTween = transform.DOLocalMoveY(targetPos.y, disappearDuration)
            .SetEase(Ease.InOutSine).OnComplete(() => IsDisappearing = false);
    }

    public void Appear()
    {
        IsDisappearing = false;
        StopMove();

        moveTween = transform.DOLocalMoveY(originalPos.y, disappearDuration)
            .SetEase(Ease.OutBack)
            .OnComplete(() => StartSwinging());
    }

    private bool startedHover = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (IsDisappearing)
        {
            return;
        }

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

       if (hit.collider == null || (hit.collider.gameObject != gameObject && !hit.collider.transform.IsChildOf(transform)))
       {
           return;
        }
        OnHover?.Invoke(); 
        startedHover = true;
        StopSwinging();
        scaleTween?.Kill();
        scaleTween = transform.DOScale(originalScale * hoverScale, scaleDuration).SetEase(Ease.OutBack);
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!startedHover) return;
        OnHoverEnd?.Invoke();
        if (IsDisappearing) return;
        StartSwinging();
        scaleTween?.Kill();
        scaleTween = transform.DOScale(originalScale, scaleDuration).SetEase(Ease.OutBack);
        startedHover = false;
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (IsDisappearing) return;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider == null || (hit.collider.gameObject != gameObject && !hit.collider.transform.IsChildOf(transform)))
        {
            return;
        }
        OnClick?.Invoke();
    }
}
