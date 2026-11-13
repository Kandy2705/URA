using UnityEngine;

[CreateAssetMenu(fileName = "DynamicQuestion_", menuName = "Survey/Dynamic Question SO")]
public class DynamicQuestionSO : ScriptableObject
{
    [TextArea(2, 4)]
    public string questionTemplate = "Món nào đang được giảm giá ?";
    public string answerFormat = "{Value}";
}