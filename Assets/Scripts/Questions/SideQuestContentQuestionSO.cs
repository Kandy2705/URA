using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Q_SideQuestContent", menuName = "Survey/Questions/07 - Side Quest Content")]
public class SideQuestContentQuestionSO : DynamicQuestionSO
{
    public override string questionTemplate => "Nhiệm vụ mà bạn được dặn thực hiện vào lúc kết thúc chuyến mua sắm ở Màn 3 là gì?";
    public override string answerFormat => "{Value}";

    public override QuestionResult Generate(SurveyContextData context)
    {
        return null;
    }
}