using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Q_OutOfStock", menuName = "Survey/Questions/08 - Out Of Stock")]
public class OutOfStockQuestionSO : DynamicQuestionSO
{
    public override string questionTemplate => "Món nào trong danh sách đã hết hàng ?";
    public override string answerFormat => "{Value}";

    public override QuestionResult Generate(SurveyContextData context)
    {
        return null;
    }
}