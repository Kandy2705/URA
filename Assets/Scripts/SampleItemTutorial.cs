using System.Collections;
using UnityEngine;
using TMPro;

public class SampleItemTutorial : MonoBehaviour
{
    [Header("Core References")]
    [SerializeField] private Pointer pointer;             // Mũi tên chỉ hướng
    [SerializeField] private Transform sampleItem;         // Item mẫu cần lấy

    [Header("UI References")]
    [SerializeField] private GameObject questCanvas;       // UI thông báo
    [SerializeField] private TextMeshProUGUI notificationText;
    [SerializeField] private float uiDuration = 4f;

    private bool isTaskCompleted = false;
    private Coroutine hideCoroutine;

    void Start()
    {
        StartTutorial();
    }

    public void StartTutorial()
    {
        // 1. Chỉ mũi tên vào đúng item mẫu
        if (pointer != null && sampleItem != null)
        {
            pointer.gameObject.SetActive(true);
            pointer.SetTarget(sampleItem);
        }

        // 2. Hiện câu nhắc
        ShowNotice("Hướng tay cầm vào item.\nBấm nút bên hông để lấy item.");
    }

    // GỌI HÀM NÀY KHI BẤM CHỌN ĐÚNG ITEM MẪU (BỎ VÀO GIỎ)
    public void OnSampleItemAddedToCart()
    {
        if (isTaskCompleted) return;
        isTaskCompleted = true;

        StartCoroutine(CompleteTaskRoutine());
    }

    // GỌI HÀM NÀY NẾU BẤM / CHẠM NHẦM VẬT THỂ KHÁC TRÊN KỆ
    public void OnWrongItemClicked()
    {
        if (isTaskCompleted) return;

        ShowNotice("Bạn chọn nhầm vật thể khác!\nHãy hướng tay vào item được chỉ mũi tên và bấm nút bên hông.");
    }

    private IEnumerator CompleteTaskRoutine()
    {
        ShowNotice("Đã hoàn thành! Item đã được thêm vào giỏ hàng.");

        // Ẩn mũi tên
        if (pointer != null)
            pointer.gameObject.SetActive(false);

        // Chờ vài giây rồi ẩn bảng thông báo
        yield return new WaitForSeconds(3.0f);

        if (questCanvas != null)
            questCanvas.SetActive(false);

        Debug.Log("[Tutorial] Hoàn thành task thành công!");
    }

    public void ShowNotice(string msg)
    {
        if (notificationText != null)
            notificationText.text = msg;

        if (questCanvas != null)
            questCanvas.SetActive(true);

        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);

        hideCoroutine = StartCoroutine(HideNoticeRoutine());
    }

    private IEnumerator HideNoticeRoutine()
    {
        yield return new WaitForSeconds(uiDuration);
        if (questCanvas != null)
            questCanvas.SetActive(false);
    }
}