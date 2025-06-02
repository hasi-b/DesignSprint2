using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Dialogue/Node")]
public class LieNode : ScriptableObject
{
    public string NodeQuestion;
    public string NodeAnswer;
    public List<LieNode> Answers;
    public float TimeToAnswer;
    public float ExpressionWeight;
}
