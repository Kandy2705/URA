using UnityEngine;
using TMPro; 

public class PaymentManager : MonoBehaviour
{
    public static PaymentManager Instance;

    [Header("Dữ liệu thanh toán")]
    public int requiredAmount = 200000;
    private int currentAmount = 0;     

    [Header("Liên kết UI")]
    public TMP_Text requiredAmountText; 
    public TMP_Text currentAmountText; 

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        UpdateUI();
    }

    public void AddMoney(int amount)
    {
        currentAmount += amount;
        UpdateUI();
    }

    public void ResetMoney()
    {
        currentAmount = 0;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (requiredAmountText != null)
            requiredAmountText.text = requiredAmount.ToString("N0");
            
        if (currentAmountText != null)
            currentAmountText.text = currentAmount.ToString("N0");
    }

    public void ConfirmPayment()
    {
        if (currentAmount >= requiredAmount)
        {
            int change = currentAmount - requiredAmount;
        }
        else
        {
            int missing = requiredAmount - currentAmount;
        }
    }
}