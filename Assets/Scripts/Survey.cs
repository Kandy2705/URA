using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Survey : MonoBehaviour
{
    private int currentQuestion = 0;

    public class Answers
    {
        public List<string> answers = new List<string>();
    }

    private List<Answers> answersList = new List<Answers>();
    private List<string> activeQuestions = new List<string>();
    private List<int> userChoices = new List<int>();

    [SerializeField] private TextMeshProUGUI QuestObject;
    [SerializeField] private List<TextMeshProUGUI> Options = new List<TextMeshProUGUI>();
    [SerializeField] private Button confirmButton;

    [Header("Question Pool (Kéo thả các Question SO vào đây)")]
    [SerializeField] private List<DynamicQuestionSO> questionPool = new List<DynamicQuestionSO>();

    private const int NUMBER_OF_OPTIONS = 4;

    public event Action OnCompleted;

    private List<string> allItems = new List<string>();
    private bool isShowing;

    public bool HasQuestions => answersList.Count > 0;

    private static List<string> shopping_history = new List<string>();
    private static string replacedItem = string.Empty;


    private void BuildAllValidQuestions()
    {
        activeQuestions.Clear();
        answersList.Clear();
        userChoices.Clear();
        currentQuestion = 0;

        allItems = ItemsManager.getItemList()
            .Where(item => item != null && !string.IsNullOrWhiteSpace(item.itemName))
            .Select(item => item.itemName)
            .Distinct()
            .ToList();

        var context = new SurveyContextData
        {
            AllItems = allItems,
            ShoppingHistory = shopping_history,
            ReplacedItem = replacedItem,
            OptionNum = NUMBER_OF_OPTIONS,
            Log = Logging,
            IsSideQuestCompleted = true
        };

        // Xáo trộn danh sách SO mỗi lần build câu hỏi
        var shuffledPool = questionPool
            .Where(q => q != null)
            .OrderBy(_ => UnityEngine.Random.value)
            .ToList();

        foreach (var questionSO in shuffledPool)
        {
            QuestionResult result = questionSO.Generate(context);
            if (result != null && result.Answers != null && result.Answers.Count > 0)
            {
                activeQuestions.Add(result.QuestionText);
                answersList.Add(new Answers { answers = result.Answers });
            }
        }

        Logging($"Đã sinh thành công {activeQuestions.Count}/{shuffledPool.Count} câu hỏi hợp lệ.");
    }

    public void Show()
    {
        if (isShowing) return;
        isShowing = true;

        GameObject root = transform.root.gameObject;
        root.SetActive(true);

        Canvas canvas = root.GetComponent<Canvas>();
        if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceCamera && canvas.worldCamera == null)
            canvas.worldCamera = Camera.main;

        CenterPanel();
        WireConfirmButton();
        BuildAllValidQuestions();

        if (answersList.Count == 0)
            Complete();
        else
            prepareQuestions();
    }

    public void prepareQuestions()
    {
        if (currentQuestion >= answersList.Count)
        {
            Complete();
            return;
        }

        QuestObject.text = activeQuestions[currentQuestion];

        List<string> shuffled = answersList[currentQuestion].answers
            .OrderBy(_ => UnityEngine.Random.value)
            .ToList();

        for (int i = 0; i < Options.Count; i++)
        {
            bool hasAnswer = i < shuffled.Count;
            Options[i].text = hasAnswer ? shuffled[i] : string.Empty;
            Button optionButton = Options[i].GetComponentInParent<Button>(true);
            if (optionButton != null)
                optionButton.gameObject.SetActive(hasAnswer);
        }

        Logging($"Kết thúc chuẩn bị câu hỏi: {activeQuestions[currentQuestion]}");
    }

    public void getAnswerInput(int index)
    {
        if (index < 0 || index >= Options.Count || answersList.Count <= currentQuestion) return;

        if (userChoices.Count <= currentQuestion)
            userChoices.Add(index);
        else
            userChoices[currentQuestion] = index;

        bool isCorrect = Options[index].text == answersList[currentQuestion].answers[0];
        Logging($"Câu {currentQuestion + 1} trả lời {(isCorrect ? "đúng" : "sai")}");
    }

    public void confirm()
    {
        Logging($"Bắt đầu Confirm câu hỏi {currentQuestion}");
        if (userChoices.Count <= currentQuestion)
        {
            Logging("Chưa chọn đáp án.");
            return;
        }

        currentQuestion++;
        prepareQuestions(); // Hàm này đã tự gọi Complete() nếu currentQuestion >= answersList.Count
    }

    private void Complete()
    {
        Logging("Hoàn thành khảo sát.");
        isShowing = false;
        transform.root.gameObject.SetActive(false);
        OnCompleted?.Invoke();
    }

    private void CenterPanel()
    {
        Transform root = transform.root;
        Transform panel = transform;
        while (panel.parent != null && panel.parent != root)
            panel = panel.parent;

        if (panel == root || !(panel is RectTransform rect)) return;

        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition3D = Vector3.zero;
    }

    private void WireConfirmButton()
    {
        if (confirmButton == null)
        {
            Logging("Chưa gán Confirm Button trong Inspector!");
            return;
        }

        for (int i = 0; i < confirmButton.onClick.GetPersistentEventCount(); i++)
            confirmButton.onClick.SetPersistentListenerState(i, UnityEventCallState.Off);

        confirmButton.onClick.RemoveListener(confirm);
        confirmButton.onClick.AddListener(confirm);
    }

    public static void updateShoppingHist(List<string> list)
    {
        shopping_history = list;
    }

    public static void updateReplacedItem(string item)
    {
        replacedItem = item;
    }

    public void Logging(string msg)
    {
        Debug.Log($"[Survey] {msg}");
    }
}