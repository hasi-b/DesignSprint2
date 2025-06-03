using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PatternValidator : MonoBehaviour
{
    public static bool ValidatePattern(List<WordNode> pattern, LieGameManager.PatternData[] correctPatterns, out LieGameManager.PatternData matchedPattern, out int score)
    {
        matchedPattern = null;
        score = 0;

        if (pattern == null || pattern.Count == 0)
            return false;

        string[] patternWords = new string[pattern.Count];
        for (int i = 0; i < pattern.Count; i++)
        {
            patternWords[i] = pattern[i].GetWord();
        }

        foreach (var correctPattern in correctPatterns)
        {
            if (IsExactMatch(patternWords, correctPattern.wordSequence))
            {
                matchedPattern = correctPattern;
                score = correctPattern.scoreValue;
                return true;
            }
        }

        return false;
    }

    private static bool IsExactMatch(string[] pattern1, string[] pattern2)
    {
        if (pattern1.Length != pattern2.Length)
            return false;

        for (int i = 0; i < pattern1.Length; i++)
        {
            if (!pattern1[i].Equals(pattern2[i], System.StringComparison.OrdinalIgnoreCase))
                return false;
        }

        return true;
    }
}