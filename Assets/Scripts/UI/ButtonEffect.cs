using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] float scale;
    [SerializeField] float duration;
    
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        rectTransform.DOScale(scale, duration);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        rectTransform.DOScale(1, duration);
    }
}
