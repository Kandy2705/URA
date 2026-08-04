using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

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

    [Header("Ví tiền ban đầu của player")]
    [SerializeField] private List<WalletBillDefinition> startingWallet = new List<WalletBillDefinition>
    {
        new WalletBillDefinition(100000, 1),
        new WalletBillDefinition(50000, 2),
        new WalletBillDefinition(10000, 2)
    };
    [SerializeField] private bool randomizeStartingWallet = true;
    [SerializeField] private int minimumStartingMoney = 150000;
    [SerializeField] private int maximumStartingMoney = 300000;
    [SerializeField] private int startingMoneyStep = 10000;

    [Header("UI runtime")]
    [SerializeField] private string confirmButtonName = "ConfirmButton";
    [SerializeField] private string paymentUiRootName = "PaymentUI Variant";
    [SerializeField] private GameObject paymentUiRoot;

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

    [Header("Cashier payment feedback")]
    [SerializeField] private string paymentSuccessTemplate =
        "Cháu đã nhận {0} đồng và thối lại {1} đồng. Cảm ơn bác.";
    [SerializeField] private string paymentShortageTemplate =
        "Bác đã đưa thiếu {0} đồng rồi, bác vui lòng chọn thêm tiền nhé.";
    [SerializeField] private string insufficientWalletMessage =
        "Bác không đủ tiền rồi nên cháu sẽ bỏ bớt đồ ra nhé. Cảm ơn bác đã tham gia.";

    private readonly Dictionary<int, int> availableBillCounts = new Dictionary<int, int>();
    private readonly Dictionary<int, int> submittedBillCounts = new Dictionary<int, int>();
    private readonly Dictionary<int, List<MoneyItem>> moneyItemsByDenomination = new Dictionary<int, List<MoneyItem>>();

    private bool isPlayingCashierIntro;
    private bool isResolvingPayment;
    private bool checkoutSessionActive;
    private bool paymentConfirmed;
    private PaymentSummary lastPaymentSummary;
    private static int lastGeneratedWalletTotal = -1;
    private static string lastGeneratedWalletSignature;

    public PaymentSummary LastPaymentSummary => lastPaymentSummary;
    public bool HasConfirmedPayment => paymentConfirmed;
    public int CurrentAmount => currentAmount;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        PrepareStartingWallet();
        ResolveRuntimeReferences();
        RebuildWalletState();
        UpdateUI();
        WireConfirmButton();
    }

    private void Start()
    {
        RegisterExistingMoneyItems();
        UpdateUI();

        if (playCashierIntroOnStart)
            PlayCashierIntro();
    }

    private void EnsureDefaultWallet()
    {
        if (startingWallet != null && startingWallet.Count > 0)
            return;

        startingWallet = new List<WalletBillDefinition>
        {
            new WalletBillDefinition(100000, 1),
            new WalletBillDefinition(50000, 2),
            new WalletBillDefinition(10000, 2)
        };
    }

    private void PrepareStartingWallet()
    {
        if (!randomizeStartingWallet)
        {
            EnsureDefaultWallet();
            return;
        }

        int step = Mathf.Max(10000, startingMoneyStep);
        int minimum = Mathf.Max(step, Mathf.CeilToInt(minimumStartingMoney / (float)step) * step);
        int maximum = Mathf.Max(minimum, Mathf.FloorToInt(maximumStartingMoney / (float)step) * step);

        List<WalletBillDefinition> generatedWallet = null;
        string generatedSignature = string.Empty;

        for (int attempt = 0; attempt < 20; attempt++)
        {
            int targetUnits = Random.Range(minimum / step, maximum / step + 1);
            int targetAmount = targetUnits * step;
            generatedWallet = BuildRandomWallet(targetAmount);
            generatedSignature = BuildWalletSignature(generatedWallet);

            if (targetAmount != lastGeneratedWalletTotal &&
                generatedSignature != lastGeneratedWalletSignature)
                break;
        }

        startingWallet = generatedWallet ?? new List<WalletBillDefinition>
        {
            new WalletBillDefinition(100000, 1),
            new WalletBillDefinition(50000, 1)
        };
        int total = startingWallet.Sum(bill => bill.denomination * bill.count);
        lastGeneratedWalletTotal = total;
        lastGeneratedWalletSignature = BuildWalletSignature(startingWallet);
        Debug.Log($"PaymentManager: Ví random của lượt này = {total:N0} VND ({lastGeneratedWalletSignature}).");
    }

    private static List<WalletBillDefinition> BuildRandomWallet(int targetAmount)
    {
        int[] denominations = { 100000, 50000, 20000, 10000 };
        List<WalletBillDefinition> wallet = new List<WalletBillDefinition>();
        int remaining = targetAmount;

        for (int i = 0; i < denominations.Length; i++)
        {
            int denomination = denominations[i];
            int maximumCount = remaining / denomination;
            int count = i == denominations.Length - 1
                ? maximumCount
                : Random.Range(0, maximumCount + 1);

            if (count > 0)
                wallet.Add(new WalletBillDefinition(denomination, count));

            remaining -= denomination * count;
        }

        return wallet;
    }

    private static string BuildWalletSignature(IEnumerable<WalletBillDefinition> wallet)
    {
        return string.Join(
            ", ",
            wallet
                .Where(bill => bill != null && bill.count > 0)
                .OrderByDescending(bill => bill.denomination)
                .Select(bill => $"{bill.denomination:N0}x{bill.count}"));
    }

    private void ResolveRuntimeReferences()
    {
        if (paymentUiRoot != null)
            return;

        Transform[] sceneTransforms = FindObjectsOfType<Transform>(true);
        foreach (Transform sceneTransform in sceneTransforms)
        {
            if (sceneTransform == null || sceneTransform.name != paymentUiRootName)
                continue;

            paymentUiRoot = sceneTransform.gameObject;
            return;
        }

        Debug.LogWarning($"PaymentManager: Không tìm thấy UI '{paymentUiRootName}' trong scene.");
    }

    private void WireConfirmButton()
    {
        Button[] buttons = FindObjectsOfType<Button>(true);
        foreach (Button button in buttons)
        {
            if (button == null || button.gameObject.name != confirmButtonName)
                continue;

            button.onClick.RemoveListener(ConfirmPayment);
            button.onClick.AddListener(ConfirmPayment);
            return;
        }
    }

    private void RegisterExistingMoneyItems()
    {
        MoneyItem[] moneyItems = FindObjectsOfType<MoneyItem>(true);
        foreach (MoneyItem moneyItem in moneyItems)
            RegisterMoneyItem(moneyItem);
    }

    public void RegisterMoneyItem(MoneyItem moneyItem)
    {
        if (moneyItem == null)
            return;

        int denomination = moneyItem.MoneyValue;
        if (denomination <= 0)
            return;

        if (!moneyItemsByDenomination.TryGetValue(denomination, out List<MoneyItem> items))
        {
            items = new List<MoneyItem>();
            moneyItemsByDenomination[denomination] = items;
        }

        if (!items.Contains(moneyItem))
            items.Add(moneyItem);

        moneyItem.RefreshAvailability(GetRemainingBillCount(denomination), CanUseBills());
    }

    public void AddMoney(int amount)
    {
        if (!checkoutSessionActive || paymentConfirmed)
        {
            Debug.Log("PaymentManager: Phiên thanh toán chưa mở hoặc đã chốt.");
            return;
        }

        if (amount <= 0)
            return;

        if (!availableBillCounts.TryGetValue(amount, out int remaining) || remaining <= 0)
        {
            Debug.Log($"PaymentManager: Không còn tờ {amount:N0} để đưa.");
            RefreshMoneyItemState(amount);
            return;
        }

        availableBillCounts[amount] = remaining - 1;
        submittedBillCounts[amount] = GetSubmittedBillCount(amount) + 1;
        currentAmount += amount;

        UpdateUI();
        RefreshMoneyItemState(amount);
    }

    public void ResetMoney()
    {
        if (!checkoutSessionActive && !paymentConfirmed)
        {
            RebuildWalletState();
            UpdateUI();
            return;
        }

        currentAmount = 0;
        submittedBillCounts.Clear();
        RebuildWalletState();
        paymentConfirmed = false;
        lastPaymentSummary = null;

        UpdateUI();
    }

    private void RebuildWalletState()
    {
        availableBillCounts.Clear();
        EnsureDefaultWallet();

        foreach (WalletBillDefinition bill in startingWallet)
        {
            if (bill == null || bill.denomination <= 0 || bill.count < 0)
                continue;

            availableBillCounts[bill.denomination] = bill.count;
        }

        RefreshAllMoneyItems();
    }

    private void UpdateUI()
    {
        if (requiredAmountText != null)
            requiredAmountText.text = requiredAmount.ToString("N0");

        if (currentAmountText != null)
            currentAmountText.text = currentAmount.ToString("N0");

        RefreshAllMoneyItems();
    }

    public void UpdatePaymentUI()
    {
        Debug.Log("PaymentManager: Cập nhật UI thanh toán...");
        if (CartManager.Instance != null)
        {
            requiredAmount = CartManager.Instance.TotalPaid;
            Debug.Log($"Update Payment UI: requiredAmount = {requiredAmount}");
        }

        BeginCheckoutSession();
    }

    public void SetRequiredAmount(int amount)
    {
        requiredAmount = Mathf.Max(0, amount);
        BeginCheckoutSession();
    }

    private void BeginCheckoutSession()
    {
        ResolveRuntimeReferences();
        checkoutSessionActive = true;
        isResolvingPayment = false;
        paymentConfirmed = false;
        lastPaymentSummary = null;
        currentAmount = 0;
        submittedBillCounts.Clear();
        RebuildWalletState();

        if (paymentUiRoot != null)
            paymentUiRoot.SetActive(true);

        UpdateUI();
    }

    public void ConfirmPayment()
    {
        if (!checkoutSessionActive || isResolvingPayment)
        {
            Debug.Log("PaymentManager: Không có phiên thanh toán đang mở.");
            return;
        }

        if (paymentConfirmed)
        {
            Debug.Log("PaymentManager: Phiên thanh toán này đã được xác nhận.");
            return;
        }

        isResolvingPayment = true;
        checkoutSessionActive = false;
        RefreshAllMoneyItems();

        if (paymentUiRoot != null)
            paymentUiRoot.SetActive(false);

        StartCoroutine(ResolvePaymentAttemptRoutine());
    }

    private IEnumerator ResolvePaymentAttemptRoutine()
    {
        while (isPlayingCashierIntro)
            yield return null;

        int difference = currentAmount - requiredAmount;
        AudioSource targetAudioSource = ResolveCashierAudioSource(ResolveCashierAnimator());

        if (difference >= 0)
        {
            string successMessage = string.Format(
                CultureInfo.InvariantCulture,
                paymentSuccessTemplate,
                ConvertNumberToVietnameseWords(currentAmount),
                ConvertNumberToVietnameseWords(difference));

            yield return PlayCashierSpeech(targetAudioSource, successMessage);
            FinalizePayment();
            yield break;
        }

        int shortage = Mathf.Abs(difference);
        if (HasRemainingBills())
        {
            string shortageMessage = string.Format(
                CultureInfo.InvariantCulture,
                paymentShortageTemplate,
                ConvertNumberToVietnameseWords(shortage));

            yield return PlayCashierSpeech(targetAudioSource, shortageMessage);

            checkoutSessionActive = true;
            isResolvingPayment = false;

            if (paymentUiRoot != null)
                paymentUiRoot.SetActive(true);

            UpdateUI();
            yield break;
        }

        yield return PlayCashierSpeech(targetAudioSource, insufficientWalletMessage);
        FinalizePayment();
    }

    private bool HasRemainingBills()
    {
        return availableBillCounts.Values.Any(count => count > 0);
    }

    private void FinalizePayment()
    {
        paymentConfirmed = true;
        isResolvingPayment = false;
        lastPaymentSummary = BuildPaymentSummary();

        if (DataManager.Instance != null)
            DataManager.Instance.SetPaymentSummary(lastPaymentSummary);

        ListResultCompare[] listResultCompares = FindObjectsOfType<ListResultCompare>(true);
        ListResultCompare listResultCompare = listResultCompares.Length > 0 ? listResultCompares[0] : null;
        if (listResultCompare != null)
        {
            listResultCompare.FinalizeResultsForReport();
        }
        else if (DataManager.Instance != null)
        {
            DataManager.Instance.Report();
        }

        MonoBehaviour[] behaviours = FindObjectsOfType<MonoBehaviour>(true);
        foreach (MonoBehaviour behaviour in behaviours)
        {
            if (behaviour == null || behaviour.GetType().Name != "Level2MllmDialogueBridge")
                continue;

            behaviour.SendMessage("FinalizeCheckoutAfterPayment", lastPaymentSummary, SendMessageOptions.DontRequireReceiver);
            break;
        }

        RefreshAllMoneyItems();

        Debug.Log(
            $"PaymentManager: Đã chốt thanh toán | required={requiredAmount:N0} | paid={currentAmount:N0} | " +
            $"result={lastPaymentSummary.resultCode} | note={lastPaymentSummary.note}");
    }

    private PaymentSummary BuildPaymentSummary()
    {
        return PaymentSummary.Create(
            requiredAmount,
            currentAmount,
            BuildStartingWalletSnapshots(),
            BuildSubmittedWalletSnapshots());
    }

    private List<WalletBillSnapshot> BuildStartingWalletSnapshots()
    {
        List<WalletBillSnapshot> snapshots = new List<WalletBillSnapshot>();
        foreach (WalletBillDefinition bill in startingWallet.OrderByDescending(item => item.denomination))
        {
            if (bill == null || bill.denomination <= 0 || bill.count <= 0)
                continue;

            snapshots.Add(new WalletBillSnapshot(bill.denomination, bill.count));
        }

        return snapshots;
    }

    private List<WalletBillSnapshot> BuildSubmittedWalletSnapshots()
    {
        List<WalletBillSnapshot> snapshots = new List<WalletBillSnapshot>();
        foreach ((int denomination, int count) in submittedBillCounts.OrderByDescending(pair => pair.Key))
        {
            if (count <= 0)
                continue;

            snapshots.Add(new WalletBillSnapshot(denomination, count));
        }

        return snapshots;
    }

    private bool CanUseBills()
    {
        return checkoutSessionActive && !paymentConfirmed;
    }

    private int GetRemainingBillCount(int denomination)
    {
        return availableBillCounts.TryGetValue(denomination, out int count) ? count : 0;
    }

    private int GetSubmittedBillCount(int denomination)
    {
        return submittedBillCounts.TryGetValue(denomination, out int count) ? count : 0;
    }

    private void RefreshAllMoneyItems()
    {
        foreach (int denomination in moneyItemsByDenomination.Keys.ToList())
            RefreshMoneyItemState(denomination);
    }

    private void RefreshMoneyItemState(int denomination)
    {
        if (!moneyItemsByDenomination.TryGetValue(denomination, out List<MoneyItem> items))
            return;

        int remainingCount = GetRemainingBillCount(denomination);
        bool canInteract = CanUseBills();

        for (int i = items.Count - 1; i >= 0; i--)
        {
            if (items[i] == null)
            {
                items.RemoveAt(i);
                continue;
            }

            items[i].RefreshAvailability(remainingCount, canInteract);
        }
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
            targetAnimator.SetBool(cashierPayingParameter, true);

        if (targetAudioSource != null && cashierBeepClip != null)
            yield return PlayRepeatedClip(targetAudioSource, cashierBeepClip, Mathf.Max(1, cashierBeepLoopCount));

        if (canAnimateCashier)
            targetAnimator.SetBool(cashierPayingParameter, false);

        if (announceRequiredAmountWithTts && targetAudioSource != null && requiredAmount > 0)
            yield return PlayAmountAnnouncement(targetAudioSource, requiredAmount);

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

        if (cashierChargeController != null)
            return animator.runtimeAnimatorController == cashierChargeController;

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

        yield return PlayCashierSpeech(audioSource, announcement);
    }

    private IEnumerator PlayCashierSpeech(AudioSource audioSource, string announcement)
    {
        if (audioSource == null || string.IsNullOrWhiteSpace(announcement))
            yield break;

        Debug.Log($"PaymentManager cashier: {announcement}");

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
