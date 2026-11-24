using UnityEngine;

[CreateAssetMenu(fileName = "Q_FirstPurchased", menuName = "Survey/Questions/02 - First Purchased Item")]
public class FirstPurchasedQuestionSO : DynamicQuestionSO
{
    public override string questionTemplate => "Món nào đang được mua đầu tiên ?";
    public override string answerFormat => "{Value}";
    public override QuestionResult Generate(SurveyContextData context)
    {
        if (context.ShoppingHistory.Count <= 0) return null;

        string firstItem = context.ShoppingHistory[0];
        return BuildOptions(questionTemplate, firstItem, context.AllItems, context.OptionNum, context.Log);
    }
}