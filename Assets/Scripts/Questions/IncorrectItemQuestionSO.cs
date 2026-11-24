using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Q_IncorrectItem", menuName = "Survey/Questions/10 - Incorrect Item")]
public class IncorrectItemQuestionSO : DynamicQuestionSO
{
    public override string questionTemplate => "Món nào bạn mua không có trong danh sách sau khi đổi ?";
    public override string answerFormat => "{Value}";

    public override QuestionResult Generate(SurveyContextData context)
    {
        return null;
    }
}