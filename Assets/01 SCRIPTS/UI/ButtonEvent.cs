using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

public class ButtonEvent : Button
{
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        transform.DOScale(0.93f, 0.2f).SetEase(Ease.InOutSine).SetLoops(1, LoopType.Yoyo);
        SoundManager.Instance.PlaySound(SoundManager.SoundType.Click);
    }

    // Button is released
    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        transform.DOScale(1f, 0.2f).SetEase(Ease.InOutSine).SetLoops(1, LoopType.Yoyo);    
    }
}
