using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// ==========================================
// 1. DATA & STRATEGY INTERFACE
// ==========================================

public class SurveyContextData
{
    public List<string> AllItems;
    public List<string> ShoppingHistory;
    public string ReplacedItem;
    public int OptionNum;
    public Action<string> Log;
    public bool IsSideQuestCompleted;
}

public class QuestionResult
{
    public string QuestionText;
    public List<string> Answers; // Quy ước: Answers[0] luôn là đáp án đúng
}


// ==========================================
// 2. BASE HELPER & CONCRETE STRATEGIES
// ==========================================

public abstract class BaseQuestionStrategy
{
    public abstract QuestionResult Generate(SurveyContextData context);
    protected readonly DynamicQuestionSO config;

    protected BaseQuestionStrategy(DynamicQuestionSO config)
    {
        this.config = config;
    }

    protected string FormatAnswer(string rawValue)
    {
        if (config == null || string.IsNullOrEmpty(config.answerFormat))
            return rawValue;

        return config.answerFormat.Replace("{Value}", rawValue);
    }

    protected QuestionResult BuildOptions(string questionText, string correctOption, List<string> optionSource, int optionNum, Action<string> log)
    {
        if (string.IsNullOrWhiteSpace(correctOption)) return null;
        string formattedCorrect = FormatAnswer(correctOption);
        List<string> answers = new List<string> { formattedCorrect };

        // Tạo bản sao để tránh làm thay đổi list gốc (allItems) của Survey khi xáo trộn thứ tự câu hỏi
        List<string> wrongOptions = new List<string>(optionSource);
        wrongOptions.RemoveAll(item => item == correctOption);

        for (int i = 0; i < optionNum - 1 && wrongOptions.Count > 0; i++)
        {
            int randomDistractAnswer = UnityEngine.Random.Range(0, wrongOptions.Count);
            string formattedWrong = FormatAnswer(wrongOptions[randomDistractAnswer]);
            log?.Invoke($"Random distract ans: {formattedWrong}");
            answers.Add(formattedWrong);
            wrongOptions.RemoveAt(randomDistractAnswer);
        }
        log.Invoke("KẾT THÚC gen Ans");
        return new QuestionResult
        {
            QuestionText = questionText,
            Answers = answers
        };
    }
}

// Question01: "Món nào đang được giảm giá ?"
public class Question01Strategy : BaseQuestionStrategy
{
    public Question01Strategy(DynamicQuestionSO config) : base(config) { }

    public override QuestionResult Generate(SurveyContextData context)
    {
        if (config == null || context.AllItems.Count <= 0) return null;

        List<string> discountedItems = ItemsManager.getDiscountedList()
            .Where(item => item != null && !string.IsNullOrWhiteSpace(item.itemName))
            .Select(item => item.itemName)
            .Distinct()
            .ToList();

        if (discountedItems.Count <= 0) return null;

        List<string> notDiscountedItems = context.AllItems.Except(discountedItems).ToList();
        int randomCorrectAnswer = UnityEngine.Random.Range(0, discountedItems.Count);

        var result = BuildOptions(config.questionTemplate, discountedItems[randomCorrectAnswer], notDiscountedItems, context.OptionNum, context.Log);
        context.Log?.Invoke("getAnswer1");
        return result;
    }
}

// Question02: "Món đầu tiên bạn mua là gì ?"
public class Question02Strategy : BaseQuestionStrategy
{
    public Question02Strategy(DynamicQuestionSO config) : base(config) { }

    public override QuestionResult Generate(SurveyContextData context)
    {
        context.Log?.Invoke("BẮT ĐẦU getAnswer2");
        if (config == null || context.ShoppingHistory.Count <= 0) return null;
        context.Log?.Invoke("BẮT ĐẦU lấy correctAnswer");
        string firstItem = context.ShoppingHistory[0];
        context.Log?.Invoke("BẮT ĐẦU gen Ans");
        var result = BuildOptions(config.questionTemplate, firstItem, context.AllItems, context.OptionNum, context.Log);
        context.Log?.Invoke("getAnswer2");
        return result;
    }
}

// Question03: "Món nào trong danh sách đã bị đổi sang món khác ?"
public class Question03Strategy : BaseQuestionStrategy
{
    public Question03Strategy(DynamicQuestionSO config) : base(config) { }

    public override QuestionResult Generate(SurveyContextData context)
    {
        if (config == null || string.IsNullOrEmpty(context.ReplacedItem)) return null;

        var result = BuildOptions(config.questionTemplate, context.ReplacedItem, context.AllItems, context.OptionNum, context.Log);
        context.Log?.Invoke("getAnswer3");
        return result;
    }
}

// Question04: "Bạn mua món {X} sau món nào ?"
public class Question04Strategy : BaseQuestionStrategy
{
    public Question04Strategy(DynamicQuestionSO config) : base(config) { }

    public override QuestionResult Generate(SurveyContextData context)
    {
        if (config == null || context.ShoppingHistory.Count < 2) return null;

        int randomChosenItem = UnityEngine.Random.Range(1, context.ShoppingHistory.Count);
        string chosenItem = context.ShoppingHistory[randomChosenItem];
        string questionText = config.questionTemplate.Replace("{X}", chosenItem);
        context.Log?.Invoke(questionText);

        var result = BuildOptions(questionText, context.ShoppingHistory[randomChosenItem - 1], context.AllItems, context.OptionNum, context.Log);
        context.Log?.Invoke("getAnswer4");
        return result;
    }
}

// Question05: "Món {X} bạn mua là món thứ mấy ?"
public class Question05Strategy : BaseQuestionStrategy
{
    public Question05Strategy(DynamicQuestionSO config) : base(config) { }

    public override QuestionResult Generate(SurveyContextData context)
    {
        if (config == null || context.ShoppingHistory.Count <= 0) return null;

        int randomChosenItem = UnityEngine.Random.Range(0, context.ShoppingHistory.Count);
        string chosenItem = context.ShoppingHistory[randomChosenItem];
        string questionText = config.questionTemplate.Replace("{X}", chosenItem);
        context.Log?.Invoke(questionText);

        List<string> wrongOptions = Enumerable.Range(1, context.ShoppingHistory.Count).Select(i => i.ToString()).ToList();

        var result = BuildOptions(questionText, (randomChosenItem + 1).ToString(), wrongOptions, context.OptionNum, context.Log);
        context.Log?.Invoke("getAnswer5");
        return result;
    }
}

// Question06: "Bạn có hoàn thành nhiệm vụ phụ không ?"
public class Question06Strategy : BaseQuestionStrategy
{
    public Question06Strategy(DynamicQuestionSO config) : base(config) { }

    public override QuestionResult Generate(SurveyContextData context)
    {
        if (config == null) return null;

        string correctAnswer = context.IsSideQuestCompleted ? "Có" : "Không";
        List<string> allChoices = new List<string> { "Có", "Không" };

        var result = BuildOptions(config.questionTemplate, correctAnswer, allChoices, context.OptionNum, context.Log);
        context.Log?.Invoke("getAnswer6");
        return result;
    }
}

// ==========================================
// 3. CONTEXT CLASS (SURVEY)
// ==========================================

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

    [Header("Question ScriptableObjects")]
    [SerializeField] private DynamicQuestionSO question01SO;
    [SerializeField] private DynamicQuestionSO question02SO;
    [SerializeField] private DynamicQuestionSO question03SO;
    [SerializeField] private DynamicQuestionSO question04SO;
    [SerializeField] private DynamicQuestionSO question05SO;
    [SerializeField] private DynamicQuestionSO question06SO;

    const int NUMBER_OF_OPTIONS = 4;
    const int MAX_RANDOM_RETRY = 5;

    public event Action OnCompleted;

    private List<string> allItems = new List<string>();
    private List<BaseQuestionStrategy> questionStrategies = new List<BaseQuestionStrategy>();
    private bool isShowing;

    public bool HasQuestions => answersList.Count > 0;

    private void Awake()
    {
        

        List<BaseQuestionStrategy> pool = new List<BaseQuestionStrategy>
        {
            new Question01Strategy(question01SO),
            new Question02Strategy(question02SO),
            new Question03Strategy(question03SO),
            new Question04Strategy(question04SO),
            new Question05Strategy(question05SO),
            new Question06Strategy(question06SO)
        };

        questionStrategies = pool;
        // questionStrategies = pool.OrderBy(_ => UnityEngine.Random.value).ToList();
    }

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

        foreach (var strategy in questionStrategies)
        {
            if (strategy == null) continue;

            QuestionResult result = strategy.Generate(context);
            if (result != null && result.Answers != null && result.Answers.Count > 0)
            {
                activeQuestions.Add(result.QuestionText);
                answersList.Add(new Answers { answers = result.Answers });
            }
        }

        Logging($"Đã sinh thành công {activeQuestions.Count}/{questionStrategies.Count} câu hỏi hợp lệ.");
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
        Logging($"Kết thúc cho câu hỏi {activeQuestions[currentQuestion]}");
    }

    public void getAnswerInput(int index)
    {
        if (index < 0 || index >= Options.Count || answersList.Count <= currentQuestion) return;
        if (userChoices.Count <= currentQuestion)
        {
            userChoices.Add(index);
        }
        else
        {
            userChoices[currentQuestion] = index;
        }
        Logging($"Câu {currentQuestion + 1} trả lời {((Options[index].text == answersList[currentQuestion].answers[0]) ? "đúng" : "sai")}");
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
        prepareQuestions();
        if (currentQuestion >= answersList.Count)
            Complete();
        Logging($"Kết thúc Confirm câu hỏi {currentQuestion}");
    }

    private void Complete()
    {
        Logging("Hoàn thành khảo sát.");
        transform.root.gameObject.SetActive(false);
        OnCompleted?.Invoke();
    }

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

    private static List<string> shopping_history = new List<string>();
    public static void updateShoppingHist(string item)
    {
        if (shopping_history.Count > 0 && shopping_history[shopping_history.Count - 1] == item) return;
        shopping_history.Add(item);
    }

    private static string replacedItem = string.Empty;
    public static void updateReplacedItem(string item)
    {
        replacedItem = item;
    }
    public void Logging(string msg)
    {
        Debug.Log($"[Survey] {msg}");
    }
}