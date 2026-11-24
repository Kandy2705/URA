using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Q_DiscountedItem", menuName = "Survey/Questions/01 - Discounted Item")]
public class DiscountedItemQuestionSO : DynamicQuestionSO
{
    public override string questionTemplate => "Món nào đang được giảm giá ?";
    public override string answerFormat => "{Value}";
    public override QuestionResult Generate(SurveyContextData context)
    {
        if (context.AllItems.Count <= 0) return null;

        List<string> discountedItems = ItemsManager.getDiscountedList()
            .Where(item => item != null && !string.IsNullOrWhiteSpace(item.itemName))
            .Select(item => item.itemName)
            .Distinct()
            .ToList();

        if (discountedItems.Count <= 0) return null;

        List<string> notDiscountedItems = context.AllItems.Except(discountedItems).ToList();
        string correct = discountedItems[UnityEngine.Random.Range(0, discountedItems.Count)];

        return BuildOptions(questionTemplate, correct, notDiscountedItems, context.OptionNum, context.Log);
    }
}