using System;
using System.Collections.Generic;
using UnityEngine;

public class SurveyContextData
{
    public List<string> AllItems;
    public List<string> ShoppingHistory;
    public string ReplacedItem;
    public int OptionNum;
    public Action<string> Log;
    public bool IsSideQuestCompleted;
}

public class QuestionResult
{
    public string QuestionText;
    public List<string> Answers; // Quy ước: Answers[0] luôn là đáp án đúng
}

public abstract class DynamicQuestionSO : ScriptableObject
{
    public abstract string questionTemplate { get; }
    [TextArea(2, 4)]
    public abstract string answerFormat { get; }

    public abstract QuestionResult Generate(SurveyContextData context);

    protected string FormatAnswer(string rawValue)
    {
        if (string.IsNullOrEmpty(answerFormat))
            return rawValue;

        return answerFormat.Replace("{Value}", rawValue);
    }

    protected QuestionResult BuildOptions(string questionText, string correctOption, List<string> optionSource, int optionNum, Action<string> log)
    {
        if (string.IsNullOrWhiteSpace(correctOption)) return null;

        string formattedCorrect = FormatAnswer(correctOption);
        List<string> answers = new List<string> { formattedCorrect };

        List<string> wrongOptions = new List<string>(optionSource);
        wrongOptions.RemoveAll(item => item == correctOption);

        for (int i = 0; i < optionNum - 1 && wrongOptions.Count > 0; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, wrongOptions.Count);
            string formattedWrong = FormatAnswer(wrongOptions[randomIndex]);
            log?.Invoke($"Random distract ans: {formattedWrong}");
            answers.Add(formattedWrong);
            wrongOptions.RemoveAt(randomIndex);
        }

        log?.Invoke("KẾT THÚC gen Ans");
        return new QuestionResult
        {
            QuestionText = questionText,
            Answers = answers
        };
    }
}