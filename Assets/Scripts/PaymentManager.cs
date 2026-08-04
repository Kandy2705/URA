using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using TMPro;
using System;
using System.Linq;
using UnityEngine.Networking;

public class PaymentManager : MonoBehaviour
{
    public static PaymentManager Instance;
    private static readonly string[] NumberWords =
    {
        "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín"
    };

    [Header("Dữ liệu thanh toán")]
    public int requiredAmount = 0;
    private int currentAmount = 0;

    [Header("Liên kết UI")]
    public TMP_Text requiredAmountText;
    public TMP_Text currentAmountText;

    [Header("Cashier intro")]
    [SerializeField] private bool playCashierIntroOnStart = false;
    [SerializeField] private Animator cashierAnimator;
    [SerializeField] private RuntimeAnimatorController cashierChargeController;
    [SerializeField] private RuntimeAnimatorController cashierIdleController;
    [SerializeField] private AudioSource cashierAudioSource;
    [SerializeField] private AudioClip cashierBeepClip;
    [SerializeField] private int cashierBeepLoopCount = 2;
    [SerializeField] private float pauseBetweenBeeps = 0.05f;
    [SerializeField] private string cashierPayingParameter = "isPaying";
    [SerializeField] private bool announceRequiredAmountWithTts = true;
    [SerializeField] private string amountAnnouncementTemplate = "Tổng tiền cần thanh toán là {0} đồng";

    [Header("Payment result")]
    [SerializeField] private bool announcePaymentResultWithTts = true;
    [SerializeField] private string notEnoughMoneyMessage = "Số tiền cô đưa chưa đủ, cần thêm {0} đồng. Cảm ơn cô đã tham gia làm nhiệm vụ, chào và tạm biệt cô";
    [SerializeField] private string enoughMoneyMessage = "Cảm ơn quý khách, tôi đã nhận được tiền, hẹn gặp lại quý khách";

    private bool isPlayingCashierIntro;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (requiredAmountText != null)
            requiredAmountText.text = requiredAmount.ToString("N0");
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
        Debug.Log("Nút reset đã được ấn");
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

    public void UpdatePaymentUI()
    {
        Debug.Log("PaymentManager: Cập nhật UI thanh toán...");
        if (CartManager.Instance != null)
        {
            requiredAmount = CartManager.Instance.TotalPaid;
            Debug.Log($"Update Payment UI: requiredAmount = {requiredAmount}");
            UpdateUI();
        }
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
            int newPayment = (int)(originalPayment * (1 + randomTolerant / 100.0));
            Debug.Log("Số tiền bằng số  là " + newPayment);
            Debug.Log("Số tiền bằng chữ là " + NumberToVietnamese(newPayment));
            return "Số tiền thanh toán là " + NumberToVietnamese(newPayment) + "đồng";
        }
        else
        {
            return "Số tiền thanh toán là " + NumberToVietnamese((int)originalPayment) + "đồng";
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
        if (tram > 0)
        {
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


    public void SetRequiredAmount(int amount)
    {
        requiredAmount = Mathf.Max(0, amount);
        currentAmount = 0;
        UpdateUI();
    }

    public void PlayCashierIntro()
    {
        if (isPlayingCashierIntro)
            return;

        StartCoroutine(PlayCashierIntroRoutine());
    }

    private IEnumerator PlayCashierIntroRoutine()
    {
        isPlayingCashierIntro = true;

        Animator targetAnimator = ResolveCashierAnimator();
        AudioSource targetAudioSource = ResolveCashierAudioSource(targetAnimator);
        bool canAnimateCashier = targetAnimator != null && HasAnimatorParameter(targetAnimator, cashierPayingParameter);

        if (targetAnimator == null)
        {
            Debug.LogWarning("PaymentManager: Không tìm thấy Animator của NPC cashier đang active trong Scene-level-2.");
        }
        else if (!canAnimateCashier)
        {
            Debug.LogWarning($"PaymentManager: Animator '{targetAnimator.runtimeAnimatorController?.name}' không có bool parameter '{cashierPayingParameter}'.");
        }

        if (canAnimateCashier)
        {
            targetAnimator.SetBool(cashierPayingParameter, true);
        }

        if (targetAudioSource != null && cashierBeepClip != null)
        {
            yield return PlayRepeatedClip(targetAudioSource, cashierBeepClip, Mathf.Max(1, cashierBeepLoopCount));
        }

        if (canAnimateCashier)
        {
            targetAnimator.SetBool(cashierPayingParameter, false);
        }

        if (announceRequiredAmountWithTts && targetAudioSource != null && requiredAmount > 0)
        {
            yield return PlayAmountAnnouncement(targetAudioSource, requiredAmount);
        }

        if (announcePaymentResultWithTts && targetAudioSource != null)
        {
            yield return PlayPaymentResultAnnouncement(targetAudioSource);
        }

        isPlayingCashierIntro = false;
    }

    private Animator ResolveCashierAnimator()
    {
        if (cashierAnimator != null &&
            cashierAnimator.gameObject.activeInHierarchy &&
            HasExpectedCashierController(cashierAnimator))
        {
            return cashierAnimator;
        }

        Animator[] animators = FindObjectsOfType<Animator>(true);
        foreach (Animator animator in animators)
        {
            if (animator == null || !animator.gameObject.activeInHierarchy)
                continue;

            if (animator.gameObject.name != "NPCs charge money")
                continue;

            if (!HasExpectedCashierController(animator))
                continue;

            cashierAnimator = animator;
            return cashierAnimator;
        }

        foreach (Animator animator in animators)
        {
            if (animator == null || !animator.gameObject.activeInHierarchy)
                continue;

            if (animator.gameObject.name != "NPCs charge money")
                continue;

            cashierAnimator = animator;
            return cashierAnimator;
        }

        NPCAnimator npcAnimator = FindObjectOfType<NPCAnimator>();
        if (npcAnimator != null && npcAnimator.gameObject.activeInHierarchy)
        {
            cashierAnimator = npcAnimator.GetComponent<Animator>();
            return cashierAnimator;
        }

        return null;
    }

    private bool HasExpectedCashierController(Animator animator)
    {
        if (animator == null || animator.runtimeAnimatorController == null)
            return false;

        return animator.runtimeAnimatorController.name == "NPC_charge_money";
    }

    private bool HasAnimatorParameter(Animator animator, string parameterName)
    {
        if (animator == null || string.IsNullOrWhiteSpace(parameterName))
            return false;

        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.type == AnimatorControllerParameterType.Bool && parameter.name == parameterName)
                return true;
        }

        return false;
    }

    private AudioSource ResolveCashierAudioSource(Animator targetAnimator)
    {
        if (cashierAudioSource == null)
        {
            GameObject hostObject = targetAnimator != null ? targetAnimator.gameObject : gameObject;
            cashierAudioSource = hostObject.GetComponent<AudioSource>();

            if (cashierAudioSource == null)
                cashierAudioSource = hostObject.AddComponent<AudioSource>();
        }

        cashierAudioSource.playOnAwake = false;
        cashierAudioSource.loop = false;
        cashierAudioSource.spatialBlend = 0f;
        cashierAudioSource.dopplerLevel = 0f;

        return cashierAudioSource;
    }

    private IEnumerator PlayRepeatedClip(AudioSource audioSource, AudioClip clip, int loopCount)
    {
        for (int i = 0; i < loopCount; i++)
        {
            audioSource.Stop();
            audioSource.clip = clip;
            audioSource.Play();

            yield return new WaitForSeconds(clip.length);

            if (pauseBetweenBeeps > 0f && i < loopCount - 1)
                yield return new WaitForSeconds(pauseBetweenBeeps);
        }

        audioSource.Stop();
        audioSource.clip = null;
    }

    private IEnumerator PlayAmountAnnouncement(AudioSource audioSource, int amount)
    {
        string amountWords = ConvertNumberToVietnameseWords(amount);
        string announcement = string.Format(
            CultureInfo.InvariantCulture,
            amountAnnouncementTemplate,
            amountWords);
        string requestUrl =
            $"https://translate.google.com/translate_tts?ie=UTF-8&client=tw-ob&tl=vi&q={UnityWebRequest.EscapeURL(announcement)}";

        using (UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(requestUrl, AudioType.MPEG))
        {
            request.timeout = 10;
            request.SetRequestHeader("User-Agent", "Mozilla/5.0");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning($"Không thể tải audio đọc số tiền: {request.error}");
                yield break;
            }

            AudioClip announcementClip = DownloadHandlerAudioClip.GetContent(request);
            if (announcementClip == null)
            {
                Debug.LogWarning("Không tạo được AudioClip đọc số tiền.");
                yield break;
            }

            audioSource.Stop();
            audioSource.PlayOneShot(announcementClip);
            yield return new WaitForSeconds(announcementClip.length);
        }
    }

    private IEnumerator PlayPaymentResultAnnouncement(AudioSource audioSource)
    {
        int missing = requiredAmount - currentAmount;
        string announcement;
        string amountWords = ConvertNumberToVietnameseWords(missing);

        if (missing > 0)
        {
            announcement = string.Format(CultureInfo.InvariantCulture, notEnoughMoneyMessage, amountWords);
        }
        else
        {
            announcement = enoughMoneyMessage;
        }

        Debug.Log($"[PaymentManager] Kết quả thanh toán: {announcement}");

        string requestUrl =
            $"https://translate.google.com/translate_tts?ie=UTF-8&client=tw-ob&tl=vi&q={UnityWebRequest.EscapeURL(announcement)}";

        using (UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(requestUrl, AudioType.MPEG))
        {
            request.timeout = 10;
            request.SetRequestHeader("User-Agent", "Mozilla/5.0");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning($"Không thể tải audio thông báo thanh toán: {request.error}");
                yield break;
            }

            AudioClip resultClip = DownloadHandlerAudioClip.GetContent(request);
            if (resultClip == null)
            {
                Debug.LogWarning("Không tạo được AudioClip thông báo thanh toán.");
                yield break;
            }

            audioSource.Stop();
            audioSource.PlayOneShot(resultClip);
            yield return new WaitForSeconds(resultClip.length);
        }
    }

    private static string ConvertNumberToVietnameseWords(int amount)
    {
        if (amount == 0)
            return "không";

        List<int> groups = new List<int>();
        long value = Mathf.Abs(amount);

        while (value > 0)
        {
            groups.Add((int)(value % 1000));
            value /= 1000;
        }

        string[] scales = { "", "nghìn", "triệu", "tỷ", "nghìn tỷ" };
        List<string> parts = new List<string>();

        for (int i = groups.Count - 1; i >= 0; i--)
        {
            int groupValue = groups[i];
            if (groupValue == 0)
                continue;

            bool hasHigherGroup = parts.Count > 0;
            string groupWords = ReadThreeDigits(groupValue, hasHigherGroup);

            if (string.IsNullOrEmpty(groupWords))
                continue;

            if (!string.IsNullOrEmpty(scales[i]))
                groupWords = $"{groupWords} {scales[i]}";

            parts.Add(groupWords);
        }

        return string.Join(" ", parts).Trim();
    }

    private static string ReadThreeDigits(int value, bool forceFull)
    {
        int hundreds = value / 100;
        int tens = (value / 10) % 10;
        int ones = value % 10;
        List<string> parts = new List<string>();

        if (hundreds > 0 || forceFull)
        {
            if (hundreds > 0)
                parts.Add($"{NumberWords[hundreds]} trăm");
            else
                parts.Add("không trăm");
        }

        if (tens > 1)
        {
            parts.Add($"{NumberWords[tens]} mươi");

            if (ones == 1)
                parts.Add("mốt");
            else if (ones == 4)
                parts.Add("tư");
            else if (ones == 5)
                parts.Add("lăm");
            else if (ones > 0)
                parts.Add(NumberWords[ones]);
        }
        else if (tens == 1)
        {
            parts.Add("mười");

            if (ones == 5)
                parts.Add("lăm");
            else if (ones > 0)
                parts.Add(NumberWords[ones]);
        }
        else
        {
            if (ones > 0 && (hundreds > 0 || forceFull))
                parts.Add("lẻ");

            if (ones > 0)
                parts.Add(NumberWords[ones]);
        }

        return string.Join(" ", parts).Trim();
    }
}
