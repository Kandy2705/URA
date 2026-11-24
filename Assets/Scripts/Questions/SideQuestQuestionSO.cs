using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Q_SideQuest", menuName = "Survey/Questions/06 - Side Quest")]
public class SideQuestQuestionSO : DynamicQuestionSO
{
    public override string questionTemplate => "Bạn đã hoàn thành nhiệm vụ phụ chưa ?";
    public override string answerFormat => "{Value}";
    [SerializeField] private string yesLabel = "Có";
    [SerializeField] private string noLabel = "Không";

    public override QuestionResult Generate(SurveyContextData context)
    {
        string correctAnswer = context.IsSideQuestCompleted ? yesLabel : noLabel;
        List<string> allChoices = new List<string> { yesLabel, noLabel };

        return BuildOptions(questionTemplate, correctAnswer, allChoices, context.OptionNum, context.Log);
    }
}