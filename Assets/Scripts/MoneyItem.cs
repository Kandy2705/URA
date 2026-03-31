using UnityEngine;
using UnityEngine.UI;

public class MoneyItem : MonoBehaviour
{
    private int moneyValue;

    public int MoneyValue { 
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
    }
    void Start()
    {
        Button btn = GetComponent<Button>();
        
        if (btn != null)
        {
            btn.onClick.AddListener(OnMoneyClicked);
        }
    }

    void OnMoneyClicked()
    {
        int moneyValue = 0;

        if (int.TryParse(gameObject.name, out moneyValue))
        {
            Debug.Log("Đã chọn tờ tiền có mệnh giá: " + moneyValue + " VND");
            PaymentManager.Instance.AddMoney(moneyValue);
        }
        else
        {
            Debug.LogWarning("Tên object không hợp lệ để tính tiền: " + gameObject.name);
        }
    }
}