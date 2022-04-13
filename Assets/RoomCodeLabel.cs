using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class RoomCodeLabel : MonoBehaviour, 
    IPointerClickHandler, 
    IPointerEnterHandler, 
    IPointerExitHandler
{
    private TextMeshProUGUI codeLabel;
    private string pattern;
    private Color baseColor;
    [SerializeField] private Color hoverColor = Color.green;
    [SerializeField] private Color clickColor = Color.red;
    [SerializeField] private float hoverColorChangeDuration = 0.25f;
    [SerializeField] private float clickColorChangeDuration = 0.5f;

    void Start()
    {
        codeLabel = GetComponent<TextMeshProUGUI>();
        pattern = codeLabel.text;
        baseColor = codeLabel.color;
        RoomCode.OnChanged += RefreshCode;
        RefreshCode();
    }

    private void OnEnable() => RefreshCode();

    private void RefreshCode()
    {
        if (pattern != null)
            codeLabel.text = string.Format(pattern, RoomCode.Value);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        GUIUtility.systemCopyBuffer = RoomCode.Value;
        var sequence = DOTween.Sequence();
        sequence.Append(codeLabel.DOColor(clickColor, clickColorChangeDuration).SetEase(Ease.InQuart));
        sequence.Append(codeLabel.DOColor(baseColor, clickColorChangeDuration).SetEase(Ease.InQuart));
    }

    public void OnPointerEnter(PointerEventData eventData) 
        => codeLabel.DOColor(hoverColor, hoverColorChangeDuration);

    public void OnPointerExit(PointerEventData eventData) 
        => codeLabel.DOColor(baseColor, hoverColorChangeDuration);
}