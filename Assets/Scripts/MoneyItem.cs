using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MoneyItem : MonoBehaviour
{
    private int moneyValue;
    private Button button;
    private TMP_Text countLabel;

    public int MoneyValue
    {
        get { return moneyValue; }
        set { moneyValue = value; }
    }

    void Awake()
    {
        if (int.TryParse(gameObject.name, out moneyValue))
        {
            moneyValue = int.Parse(gameObject.name);
        }
        else
        {
            Debug.LogWarning("Tên object không hợp lệ để tính tiền: " + gameObject.name);
        }

        button = GetComponent<Button>();
        EnsureCountLabel();
    }

    void Start()
    {
        if (button != null)
        {
            button.onClick.AddListener(OnMoneyClicked);
        }

        if (PaymentManager.Instance != null)
            PaymentManager.Instance.RegisterMoneyItem(this);
    }

    private void OnEnable()
    {
        if (PaymentManager.Instance != null)
            PaymentManager.Instance.RegisterMoneyItem(this);
    }

    void OnMoneyClicked()
    {
        if (PaymentManager.Instance == null)
        {
            Debug.LogWarning("MoneyItem: Không tìm thấy PaymentManager.");
            return;
        }

        PaymentManager.Instance.AddMoney(moneyValue);
    }

    public void RefreshAvailability(int remainingCount, bool canInteract)
    {
        if (button != null)
            button.interactable = canInteract && remainingCount > 0;

        if (countLabel != null)
            countLabel.text = $"x{remainingCount}";
    }

    private void EnsureCountLabel()
    {
        Transform existingBadge = transform.Find("BillCountBadge");
        GameObject badgeObject;

        if (existingBadge != null)
        {
            badgeObject = existingBadge.gameObject;
        }
        else
        {
            badgeObject = new GameObject("BillCountBadge", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            badgeObject.transform.SetParent(transform, false);
        }

        RectTransform badgeRect = badgeObject.GetComponent<RectTransform>();
        badgeRect.anchorMin = new Vector2(1f, 0f);
        badgeRect.anchorMax = new Vector2(1f, 0f);
        badgeRect.pivot = new Vector2(1f, 0f);
        badgeRect.anchoredPosition = new Vector2(-8f, 8f);
        badgeRect.sizeDelta = new Vector2(72f, 42f);

        Image badgeImage = badgeObject.GetComponent<Image>();
        badgeImage.color = new Color(0.03f, 0.05f, 0.07f, 0.88f);
        badgeImage.raycastTarget = false;

        countLabel = badgeObject.GetComponentInChildren<TMP_Text>(true);
        if (countLabel == null)
        {
            GameObject textObject = new GameObject("CountText", typeof(RectTransform));
            textObject.transform.SetParent(badgeObject.transform, false);

            RectTransform textRect = textObject.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            countLabel = textObject.AddComponent<TextMeshProUGUI>();
        }

        countLabel.alignment = TextAlignmentOptions.Center;
        countLabel.fontSize = 27f;
        countLabel.fontStyle = FontStyles.Bold;
        countLabel.color = Color.white;
        countLabel.raycastTarget = false;
    }
}
