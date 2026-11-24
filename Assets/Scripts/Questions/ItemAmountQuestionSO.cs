using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Q_ItemAmount", menuName = "Survey/Questions/12 - ItemAmount")]
public class ItemAmountQuestionSO : DynamicQuestionSO
{
    public override string questionTemplate => "Bạn cần phải mua bao nhiêu món {X} ?";
    public override string answerFormat => "{Value}";

    public override QuestionResult Generate(SurveyContextData context)
    {
        return null;
    }
}