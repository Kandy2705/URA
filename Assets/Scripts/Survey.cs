using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
public class Survey : MonoBehaviour
{
    private int currentQuestion = 0;
    private List<string> questions = new List<string>
    {
        "Món nào đang được giảm giá ?",
        "Món đầu tiên bạn mua là gì ?",
        "Món nào trong danh sách đã bị đổi sang món khác ?",
        "Bạn mua món {X} sau món nào và đó là món thứ mấy ?",
        "Bạn có hoàn thành nhiệm vụ phụ không ?"
    };

    public class Answers
    {
        public List<string> answers = new List<string>();
    }
    private List<Answers> answersList = new List<Answers>();
    private List<int> userChoices = new List<int>();

    [SerializeField] private TextMeshProUGUI QuestObject;
    [SerializeField] private List<TextMeshProUGUI> Options = new List<TextMeshProUGUI>();
    [SerializeField] private string confirmButtonName = "ConfirmButton";

    // Gọi khi người chơi trả lời xong câu cuối (hoặc không có câu hỏi nào để hỏi).
    public event Action OnCompleted;

    private List<string> allItems = new List<string>();
    private List<string> discountedItems = new List<string>();
    private bool isShowing;

    public bool HasQuestions => answersList.Count > 0;



    private void getAnswer1() {
        allItems = ItemsManager.getItemList()
            .Where(item => item != null && !string.IsNullOrWhiteSpace(item.itemName))
            .Select(item => item.itemName)
            .Distinct()
            .ToList();
        if(allItems.Count <= 0) return;

        discountedItems = ItemsManager.getDiscountedList()
            .Where(item => item != null && !string.IsNullOrWhiteSpace(item.itemName))
            .Select(item => item.itemName)
            .Distinct()
            .ToList();
        if(discountedItems.Count <= 0) return;

        // Logging("All items: " + allItems.Count.ToString());
        // Logging("Discounted Items: " + discountedItems.Count.ToString());

        List<string> notDiscountedList = allItems.Except(discountedItems).ToList();
        int randomCorrectAnswer = UnityEngine.Random.Range(0, discountedItems.Count);

        answersList.Add(new Answers());
        answersList[currentQuestion].answers.Add(discountedItems[randomCorrectAnswer]);
        for (int i = 0; i < Options.Count - 1 && notDiscountedList.Count > 0; i++)
        {
            int randomDistractAnswer = UnityEngine.Random.Range(0, notDiscountedList.Count);
            Logging($"Random distract ans: {randomDistractAnswer}");
            answersList[currentQuestion].answers.Add(notDiscountedList[randomDistractAnswer]);
            notDiscountedList.RemoveAt(randomDistractAnswer);
        }
    }

    // Hiện màn hình khảo sát (canvas gốc đang tắt từ đầu) và nạp câu hỏi đầu tiên.
    public void Show()
    {
        if (isShowing)
            return;
        isShowing = true;

        GameObject root = transform.root.gameObject;
        root.SetActive(true);

        Canvas canvas = root.GetComponent<Canvas>();
        if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceCamera && canvas.worldCamera == null)
            canvas.worldCamera = Camera.main;

        CenterPanel();
        WireConfirmButton();
        prepareQuestions();

        if (answersList.Count <= currentQuestion)
            Complete();
    }

    public void prepareQuestions()
    {
        getAnswer1();
        if (answersList.Count <= currentQuestion)
        {
            Logging("Chưa có dữ liệu giảm giá để tạo câu hỏi.");
            return;
        }
        QuestObject.text = questions[currentQuestion];
        List<string> shuffled = answersList[currentQuestion].answers
            .OrderBy(_ => UnityEngine.Random.value)
            .ToList();
        Logging($"{shuffled.Count}");

        for(int i = 0; i < Options.Count; i++){
            bool hasAnswer = i < shuffled.Count;
            Options[i].text = hasAnswer ? shuffled[i] : string.Empty;
            Button optionButton = Options[i].GetComponentInParent<Button>(true);
            if (optionButton != null)
                optionButton.gameObject.SetActive(hasAnswer);
        }

    }


    public void getAnswerInput(int index)
    {
        if(index < 0 || index >= Options.Count || answersList.Count <= currentQuestion) return;
        if(userChoices.Count <= currentQuestion){
            userChoices.Add(index);
        } else {
            userChoices[currentQuestion] = index;
        }
        Logging($"Câu {currentQuestion + 1} trả lời {((Options[index].text == answersList[currentQuestion].answers[0]) ? "đúng" : "sai")}");
    }

    public void confirm(){
        if (userChoices.Count <= currentQuestion)
        {
            Logging("Chưa chọn đáp án.");
            return;
        }

        currentQuestion++;
        // TODO: nạp câu tiếp theo khi đã có dữ liệu cho câu 2-5.
        if (currentQuestion >= answersList.Count)
            Complete();
    }

    private void Complete()
    {
        Logging("Hoàn thành khảo sát.");
        transform.root.gameObject.SetActive(false);
        OnCompleted?.Invoke();
    }

    // Panel trong prefab đang bị đặt lệch sang phải; luôn đưa về giữa canvas khi hiện.
    private void CenterPanel()
    {
        Transform root = transform.root;
        Transform panel = transform;
        while (panel.parent != null && panel.parent != root)
            panel = panel.parent;

        if (panel == root || !(panel is RectTransform rect))
            return;

        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition3D = Vector3.zero;
    }

    // Nút xác nhận trong prefab đang mang sẵn persistent call cũ (CartManager.ProcessCheckout),
    // tắt nó đi và gắn confirm() bằng code.
    private void WireConfirmButton()
    {
        foreach (Button button in transform.root.GetComponentsInChildren<Button>(true))
        {
            if (button.gameObject.name != confirmButtonName)
                continue;

            for (int i = 0; i < button.onClick.GetPersistentEventCount(); i++)
                button.onClick.SetPersistentListenerState(i, UnityEventCallState.Off);

            button.onClick.RemoveListener(confirm);
            button.onClick.AddListener(confirm);
            return;
        }
    }

    public void Logging(string msg) {
        Debug.Log($"[Survey] {msg}");
    }
}
