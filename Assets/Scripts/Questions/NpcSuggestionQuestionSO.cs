using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Q_NpcSuggestion", menuName = "Survey/Questions/09 - Npc Suggestion")]
public class NpcSuggestionQuestionSO : DynamicQuestionSO
{
    public override string questionTemplate => "Nhân viên siêu thị đã gợi ý bạn mua món nào ?";
    public override string answerFormat => "{Value}";

    public override QuestionResult Generate(SurveyContextData context)
    {
        return null;
    }
}