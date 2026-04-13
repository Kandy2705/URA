using UnityEngine;
using TMPro;
using System;
using System.Linq;

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
    public string randomMistake(double originalPayment)
    {
        const double MISTAKE_RATE = 0.1;
        const int PAYMENT_TOLERANT = 50;

        bool isMistaken = (UnityEngine.Random.value < MISTAKE_RATE) ? true : false;
        if (isMistaken)
        {
            int randomTolerant = UnityEngine.Random.Range(0, PAYMENT_TOLERANT);
            int newPayment = (int) (originalPayment * (1 + randomTolerant / 100.0));
            Debug.Log("Số tiền bằng số  là " + newPayment);
            Debug.Log("Số tiền bằng chữ là " + NumberToVietnamese(newPayment));
            return "Số tiền thanh toán là " + NumberToVietnamese(newPayment) + "đồng";
        }
        else
        {
            return "Số tiền thanh toán là " + NumberToVietnamese((int) originalPayment) + "đồng";
        }
    }
    
   

    private enum SpecialCase { MƯƠI, MƯỜI, LẺ }

    private string HundredNumberToVietnamese(int number)
    {
        if (number == 0) return "Không";

        string[] units = { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
        string result = "";

        int tram = (number % 1000) / 100;
        int muoi = (number % 100) / 10;
        int donVi = number % 10;
        bool hasNumber = false;
        // Hàng Trăm
        if (tram > 0) {
            result += units[tram] + " trăm ";
            hasNumber = true;
        } 

        // Hàng Chục
        SpecialCase sCase = SpecialCase.MƯƠI;
        if (muoi == 0)
        {
            if (donVi > 0 && hasNumber)
            {
                result += " lẻ ";
                sCase = SpecialCase.LẺ;
            }
        }
        else if (muoi == 1)
        {
            result += " mười ";
            sCase = SpecialCase.MƯỜI;
            hasNumber = true;
        }
        else
        {
            result += units[muoi] + " mươi ";
            sCase = SpecialCase.MƯƠI;
            hasNumber = true;
        }

        // Hàng Đơn vị
        if (donVi > 0)
        {
            if (hasNumber && donVi == 1 && sCase == SpecialCase.MƯƠI) result += "mốt";
            else if (hasNumber && donVi == 5 && sCase != SpecialCase.LẺ) result += "lăm";
            else result += units[donVi];
        }

        return result.Trim().Replace("  ", " ");
    }

    private string NumberToVietnamese(int number)
    {
        if (number == 0) return "Không";
        
        string[] placeTxt = { "", " nghìn ", " triệu ", " tỷ " };
        int[] placeNum = { 1, 1000, 1000000, 1000000000 };
        string res = "";

        int currentPlaceNum = 0;
        for (int i = placeNum.Length - 1; i >= 0; i--)
        {
            if (number >= placeNum[i])
            {
                currentPlaceNum = i;
                break;
            }
        }

        while (currentPlaceNum >= 0)
        {
            int chunk = number / placeNum[currentPlaceNum];
            
            if (chunk > 0) 
            {
                res += HundredNumberToVietnamese(chunk) + placeTxt[currentPlaceNum];
            }
            else if (res != "" && currentPlaceNum > 0)
            {
                res += "không" + placeTxt[currentPlaceNum];
            }

            number %= placeNum[currentPlaceNum];
            currentPlaceNum--;
        }

        return res.Trim();
    }
}