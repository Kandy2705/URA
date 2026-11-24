using UnityEngine;

[CreateAssetMenu(fileName = "Q_ReplacedItem", menuName = "Survey/Questions/03 - Replaced Item")]
public class ReplacedItemQuestionSO : DynamicQuestionSO
{
    public override string questionTemplate => "Món nào đã được thay thế ?";
    public override string answerFormat => "{Value}";
    public override QuestionResult Generate(SurveyContextData context)
    {
        if (string.IsNullOrEmpty(context.ReplacedItem)) return null;

        return BuildOptions(questionTemplate, context.ReplacedItem, context.AllItems, context.OptionNum, context.Log);
    }
}