using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    int score = 0;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Make sure your ball object is tagged "Ball" in the Inspector!
        if (other.CompareTag("Ball"))
        {
            score++;
            Debug.Log("SWISH! Total Score: " + score);
        }
    }
}