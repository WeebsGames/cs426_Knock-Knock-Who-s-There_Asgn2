using TMPro;
using UnityEngine;

public class scoreManager : MonoBehaviour
{
    public TMP_Text scoreText;
    int score;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        score = 0;
        scoreText.text = "Score: " + score;
    }

    // Update is called once per frame
    public void AddPoint(int points)
    {
        Debug.Log("recieved message with point value " + points);
        score+=points;
        scoreText.text = "Score: " + score;
    }
}
