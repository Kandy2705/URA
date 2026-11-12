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

    const int NUMBER_OF_OPTIONS = 4;
    const int MAX_RANDOM_RETRY = 5;
    // Gọi khi người chơi trả  lời xong câu cuối (hoặc không có câu hỏi nào để hỏi).
    public event Action OnCompleted;

    private List<string> allItems = new List<string>();
    // private List<DynamicQuestionSO> questionTemplates;

    private bool isShowing;

    public bool HasQuestions => answersList.Count > 0;
    private void Awake()
    {
        // DynamicQuestionSO[] loadedAssets = Resources.LoadAll<DynamicQuestionSO>("SurveyTemplates");
        // questionTemplates = new List<DynamicQuestionSO>(loadedAssets);

        allItems = ItemsManager.getItemList()
            .Where(item => item != null && !string.IsNullOrWhiteSpace(item.itemName))
            .Select(item => item.itemName)
            .Distinct()
            .ToList();
    }

    // private void generateOptions(string correctOption, List<string> optionSource, int optionNum = 4)
    // {

    //     answersList.Add(new Answers());
    //     answersList[currentQuestion].answers.Add(correctOption);

    //     List<string> wrongOptions = optionSource;
    //     wrongOptions.Remove(correctOption);

    //     for (int i = 0; i < optionNum - 1 && optionSource.Count > 0; i++)
    //     {
    //         int randomDistractAnswer = UnityEngine.Random.Range(0, optionSource.Count);
    //         Logging($"Random distract ans: {optionSource[randomDistractAnswer]}");
    //         answersList[currentQuestion].answers.Add(optionSource[randomDistractAnswer]);
    //         optionSource.RemoveAt(randomDistractAnswer);
    //     }
    // }
    // Question01
    private void getAnswer1()
    {
        if (allItems.Count <= 0) return;

        List<string> discountedItems = ItemsManager.getDiscountedList()
            .Where(item => item != null && !string.IsNullOrWhiteSpace(item.itemName))
            .Select(item => item.itemName)
            .Distinct()
            .ToList();
        if (discountedItems.Count <= 0) return;

        // Logging("All items: " + allItems.Count.ToString());
        // Logging("Discounted Items: " + discountedItems.Count.ToString());

        List<string> notDiscountedList = allItems.Except(discountedItems).ToList();
        int randomCorrectAnswer = UnityEngine.Random.Range(0, discountedItems.Count);

        answersList.Add(new Answers());
        answersList[currentQuestion].answers.Add(discountedItems[randomCorrectAnswer]);
        for (int i = 0; i < Options.Count - 1 && notDiscountedList.Count > 0; i++)
        {
            int randomDistractAnswer = UnityEngine.Random.Range(0, notDiscountedList.Count);
            Logging($"Random distract ans: {notDiscountedList[randomDistractAnswer]}");
            answersList[currentQuestion].answers.Add(notDiscountedList[randomDistractAnswer]);
            notDiscountedList.RemoveAt(randomDistractAnswer);
        }
        Logging("getAnswer1");
    }
    // Quest02
    private void getAnswer2()
    {
        string firstItem = shopping_history.Count > 0 ? shopping_history[0] : "";

        answersList.Add(new Answers());
        answersList[currentQuestion].answers.Add(firstItem);
        
        int[] distractIndexes = new int[NUMBER_OF_OPTIONS - 1];
        for (int i = 0; i < NUMBER_OF_OPTIONS - 1; i++)
        {
            int randomDistractAnswer = 0;
            do
            {
                randomDistractAnswer = UnityEngine.Random.Range(0, allItems.Count);
            } while (allItems[randomDistractAnswer] == firstItem && !distractIndexes.Contains(randomDistractAnswer));
            Logging($"Random distract ans: {allItems[randomDistractAnswer]}");
            answersList[currentQuestion].answers.Add(allItems[randomDistractAnswer]);
            distractIndexes[i] = randomDistractAnswer;
        }
        Logging($"Câu hỏi {currentQuestion} có {answersList[currentQuestion].answers.Count} đáp án, đáp án đầu tiên {(answersList[currentQuestion].answers.Count > 0 ? answersList[currentQuestion].answers[0] : "không có")}");
        Logging("getAnswer2");
    }
    // Question03
    private void getAnswer3()
    {
        answersList.Add(new Answers());
        answersList[currentQuestion].answers.Add(replacedItem);
        
        int[] distractIndexes = new int[NUMBER_OF_OPTIONS - 1];
        for (int i = 0; i < NUMBER_OF_OPTIONS - 1; i++)
        {
            int retry = 0;
            int randomDistractAnswer = 0;
            do
            {
                retry++;
                randomDistractAnswer = UnityEngine.Random.Range(0, allItems.Count);
            } while (retry < MAX_RANDOM_RETRY && allItems[randomDistractAnswer] == replacedItem && !distractIndexes.Contains(randomDistractAnswer));
            Logging($"Random distract ans: {allItems[randomDistractAnswer]}");
            answersList[currentQuestion].answers.Add(allItems[randomDistractAnswer]);
            distractIndexes[i] = randomDistractAnswer;
        }
        Logging("getAnswer3");
    }
    // Question04
    private void getAnswer4()
    {
        int randomChosenItem = UnityEngine.Random.Range(1, shopping_history.Count);
        string chosenItem = shopping_history[randomChosenItem];
        questions[currentQuestion].Replace("{X}", chosenItem);

        answersList.Add(new Answers());
        answersList[currentQuestion].answers.Add(shopping_history[randomChosenItem - 1]);

        int[] distractIndexes = new int[NUMBER_OF_OPTIONS - 1];
        for (int i = 0; i < NUMBER_OF_OPTIONS - 1; i++)
        {
            int retry = 0;
            int randomDistractAnswer = 0;
            do
            {
                retry++;
                randomDistractAnswer = UnityEngine.Random.Range(0, allItems.Count);
            } while (retry < MAX_RANDOM_RETRY && allItems[randomDistractAnswer] == chosenItem && !distractIndexes.Contains(randomDistractAnswer));
            Logging($"Random distract ans: {allItems[randomDistractAnswer]}");
            answersList[currentQuestion].answers.Add(allItems[randomDistractAnswer]);
            distractIndexes[i] = randomDistractAnswer;
        }
        Logging("getAnswer4");
    }

    // Question05
    private void getAnswer5()
    {
        int randomChosenItem = UnityEngine.Random.Range(0, shopping_history.Count);
        string chosenItem = shopping_history[randomChosenItem];
        questions[currentQuestion].Replace("{X}", chosenItem);

        answersList.Add(new Answers());
        answersList[currentQuestion].answers.Add((randomChosenItem + 1).ToString());

        int[] distraction = new int[NUMBER_OF_OPTIONS - 1];
        for (int i = 0; i < NUMBER_OF_OPTIONS - 1; i++)
        {
            int retry = 0;
            int randomDistractAnswer = 0;
            do
            {
                retry++;
                randomDistractAnswer = UnityEngine.Random.Range(0, shopping_history.Count);
            } while (retry < MAX_RANDOM_RETRY && randomDistractAnswer != randomChosenItem && !distraction.Contains(randomDistractAnswer));
            Logging($"Random distract ans: {randomDistractAnswer}");
            answersList[currentQuestion].answers.Add((randomDistractAnswer + 1).ToString());
            distraction[i] = randomDistractAnswer;
        }
        Logging("getAnswer5");
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
        // Chuyển sang cơ chế chọn hàm động sau
        switch (currentQuestion)
        {
            case 0:
                getAnswer1();
                break;
            case 1:
                getAnswer2();
                break;
            case 2:
                getAnswer3();
                break;
            case 3:
                getAnswer4();
                break;
            case 4:
                getAnswer5();
                break;
            default:
                break;
        }

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
        prepareQuestions();
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
