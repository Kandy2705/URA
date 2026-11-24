using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Q_PurchaseOrder", menuName = "Survey/Questions/05 - Purchase Order")]
public class PurchaseOrderQuestionSO : DynamicQuestionSO
{
    public override string questionTemplate => "Món {X} đang được mua thứ mấy ?";
    public override string answerFormat => "{Value}";
    public override QuestionResult Generate(SurveyContextData context)
    {
        if (context.ShoppingHistory.Count <= 0) return null;

        int randomIndex = UnityEngine.Random.Range(0, context.ShoppingHistory.Count);
        string chosenItem = context.ShoppingHistory[randomIndex];
        string questionText = questionTemplate.Replace("{X}", chosenItem);

        List<string> allOrderOptions = Enumerable.Range(1, context.ShoppingHistory.Count)
            .Select(i => i.ToString())
            .ToList();

        return BuildOptions(questionText, (randomIndex + 1).ToString(), allOrderOptions, context.OptionNum, context.Log);
    }
}