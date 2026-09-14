using System.Collections;
using UnityEngine;
using TMPro;

public class GrabTutorialManager : MonoBehaviour
{
    private enum TutorialStep { GrabItem, PutToCart, Completed }
    [SerializeField] private TutorialStep currentStep = TutorialStep.GrabItem;

    [Header("References")]
    [SerializeField] private Pointer pointer;
    [SerializeField] private Transform sampleItemTransform;
    [SerializeField] private Transform cartTransform;

    [Header("UI References")]
    [SerializeField] private GameObject questCanvas;
    [SerializeField] private TextMeshProUGUI notificationText;
    [SerializeField] private float uiDisplayDuration = 4.0f;

    private Coroutine hideUiCoroutine;

    private void OnEnable()
    {
        SampleItem.OnSampleItemAddedToCart += HandleItemPlacedInCart;
        SampleItem.OnWrongItemTouched += HandleWrongItem;
    }

    private void OnDisable()
    {
        SampleItem.OnSampleItemAddedToCart -= HandleItemPlacedInCart;
        SampleItem.OnWrongItemTouched -= HandleWrongItem;
    }

    void Start()
    {
        // Có thể gọi StartTutorial() từ Script trước (QuestManager) sau khi hoàn tất bước Checkpoint
        StartTutorial();
    }

    public void StartTutorial()
    {
        currentStep = TutorialStep.GrabItem;

        // 1. Trỏ mũi tên vào Item mẫu
        if (pointer != null && sampleItemTransform != null)
        {
            pointer.gameObject.SetActive(true);
            pointer.SetTarget(sampleItemTransform);
        }

        // 2. Hiện hướng dẫn cầm nắm
        ShowMessage("Hướng tay cầm vào item và bấm nút bên hông (Grip) để lấy!");
    }

    // Gọi hàm này từ XR Grab Interactable event: 'Select Entered' của item mẫu
    public void OnSampleItemGrabbed()
    {
        if (currentStep != TutorialStep.GrabItem) return;

        currentStep = TutorialStep.PutToCart;
        ShowMessage("Hãy mang item và đặt vào giỏ hàng!");

        // Chuyển mũi tên chỉ sang vị trí giỏ hàng
        if (pointer != null && cartTransform != null)
        {
            pointer.SetTarget(cartTransform);
        }
    }

    // Khi người chơi chạm/nhặt nhầm vật khác
    private void HandleWrongItem()
    {
        if (currentStep == TutorialStep.GrabItem)
        {
            ShowMessage("Sai vật thể! Vui lòng làm theo mũi tên và bấm nút bên hông để lấy đúng item.");
        }
    }

    // Khi item đã nằm trong giỏ hàng
    private void HandleItemPlacedInCart()
    {
        if (currentStep == TutorialStep.Completed) return;

        currentStep = TutorialStep.Completed;
        StartCoroutine(CompleteRoutine());
    }

    private IEnumerator CompleteRoutine()
    {
        ShowMessage("Đã hoàn thành thêm item vào giỏ hàng!");

        if (pointer != null)
        {
            pointer.gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(uiDisplayDuration);

        if (questCanvas != null)
            questCanvas.SetActive(false);

        Debug.Log("[GrabTutorial] Hoàn tất toàn bộ tutorial!");
    }

    public void ShowMessage(string message)
    {
        if (notificationText != null)
            notificationText.text = message;

        if (questCanvas != null)
            questCanvas.SetActive(true);

        if (hideUiCoroutine != null)
            StopCoroutine(hideUiCoroutine);

        hideUiCoroutine = StartCoroutine(HideNotification(uiDisplayDuration));
    }

    private IEnumerator HideNotification(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (questCanvas != null)
            questCanvas.SetActive(false);
    }
}