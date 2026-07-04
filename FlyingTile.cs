using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FlyingTile : MonoBehaviour
{
    private RectTransform rectTransform;
    private Image image;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
    }

    public void Fly(Sprite sprite, Color color, Vector2 from, Vector2 to, System.Action onComplete)
    {
        image.sprite = sprite;
        image.color = color;
        rectTransform.anchoredPosition = from;

        Sequence seq = DOTween.Sequence();

        // Bay theo arc: đi lên trước rồi xuống slot
        Vector2 midPoint = Vector2.Lerp(from, to, 0.5f) + Vector2.up * 100f;

        seq.Append(
            rectTransform.DOAnchorPos(midPoint, 0.2f)
                .SetEase(Ease.OutQuad)
        );
        seq.Append(
            rectTransform.DOAnchorPos(to, 0.2f)
                .SetEase(Ease.InQuad)
        );

        // Scale nhỏ lại khi vào slot
        seq.Join(
            rectTransform.DOScale(Vector3.one * 0.8f, 0.4f)
                .SetEase(Ease.InBack)
        );

        seq.OnComplete(() =>
        {
            onComplete?.Invoke();
            Destroy(gameObject);
        });
    }
}