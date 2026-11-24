using UnityEngine;

[CreateAssetMenu(fileName = "Q_PurchasedAfter", menuName = "Survey/Questions/04 - Purchased After Item")]
public class PurchasedAfterQuestionSO : DynamicQuestionSO
{
    public override string questionTemplate => "Món {X} được mua sau món nào ?";
    public override string answerFormat => "{Value}";
    public override QuestionResult Generate(SurveyContextData context)
    {
        if (context.ShoppingHistory.Count < 2) return null;

        int randomIndex = UnityEngine.Random.Range(1, context.ShoppingHistory.Count);
        string chosenItem = context.ShoppingHistory[randomIndex];
        string questionText = questionTemplate.Replace("{X}", chosenItem);

        return BuildOptions(questionText, context.ShoppingHistory[randomIndex - 1], context.AllItems, context.OptionNum, context.Log);
    }
}