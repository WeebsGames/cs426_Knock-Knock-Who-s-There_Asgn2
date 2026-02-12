using System;
using UnityEngine;

[Serializable]
public class QuestionData
{
    [TextArea] public string questionText;
    public string[] answers = new string[4];
    public int correctIndex;
}
