using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
public class Survey : MonoBehaviour
{
    private int currentQuestion = 0;
    private List<string> questions = new List<string>
    {
        "Có bao nhiêu món đang giảm giá ?",
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

    private List<string> allItems = new List<string>();
    private List<string> discountedItems = new List<string>();



    private void getAnswer1() {
        List<SelectableItem> itemList = ItemsManager.getItemList();
        if(itemList.Count <= 0) return;
        foreach (SelectableItem item in itemList)
        {
            allItems.Add(item.name);
        }

        List<SelectableItem> discountedList = ItemsManager.getDiscountedList();
        if(discountedList.Count <= 0) return;
        foreach (SelectableItem item in discountedList)
        {
            discountedItems.Add(item.name);
        }

        // Logging("All items: " + allItems.Count.ToString());
        // Logging("Discounted Items: " + discountedItems.Count.ToString());

        List<string> notDiscountedList = allItems.Except(discountedItems).ToList();
        int randomCorrectAnswer = UnityEngine.Random.Range(0, discountedItems.Count);

        answersList.Add(new Answers());
        answersList[currentQuestion].answers.Add(discountedItems[randomCorrectAnswer]);
        for (int i = 0; i < 3; i++)
        {
            int randomDistractAnswer = UnityEngine.Random.Range(0, notDiscountedList.Count);
            Logging($"Random distract ans: {randomDistractAnswer}");
            answersList[currentQuestion].answers.Add(notDiscountedList[randomDistractAnswer]);
            notDiscountedList.RemoveAt(randomDistractAnswer);
        }
    }
    public void prepareQuestions()
    {
        getAnswer1();
        QuestObject.text = questions[currentQuestion];
        int count = answersList[currentQuestion].answers.Count;
        Logging($"{count}");

        for(int i = 0; i < count; i++){
            // NOTE: Thực hiện cơ chế sáo đáp án ở đây
            Options[i].text = answersList[currentQuestion].answers[i];
        }
        
    }


    public void getAnswerInput(int index)
    {
        if(index < 0) return;
        if(userChoices.Count <= currentQuestion){
            userChoices.Add(index);
        } else {
            userChoices[currentQuestion] = index;
        }
        Logging($"Câu {currentQuestion + 1} trả lời {((Options[index].text == answersList[currentQuestion].answers[0]) ? "đúng" : "sai")}");
    }

    public void confirm(){
        currentQuestion++;
    }
    public void Logging(string msg) {
        Debug.Log($"[Survey] {msg}");
    }
}