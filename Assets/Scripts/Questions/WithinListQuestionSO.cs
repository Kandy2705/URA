using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Q_WithinList", menuName = "Survey/Questions/10 - Within List")]
public class WithinListQuestionSO : DynamicQuestionSO
{
    public override string questionTemplate => "Món nào có trong danh sách sau khi đổi ?";
    public override string answerFormat => "{Value}";

    public override QuestionResult Generate(SurveyContextData context)
    {
        return null;
    }
}