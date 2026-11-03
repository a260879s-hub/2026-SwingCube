using UnityEngine;

public class TriggerTest : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.LogWarning($"【絶対に出るはずのログ】接触相手: {other.gameObject.name}");
    }
}