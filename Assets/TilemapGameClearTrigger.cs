using UnityEngine;

public class TilemapGameClearTrigger : MonoBehaviour
{
    private bool isCleared = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        CheckGoal(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        CheckGoal(collider.gameObject);
    }

    private void CheckGoal(GameObject hitObject)
    {
        if (isCleared) return; // 二重発火防止

        // 接触したオブジェクトの親(root)から PlayerController を取得
        PlayerController player = hitObject.transform.root.GetComponent<PlayerController>();

        if (player != null)
        {
            isCleared = true;

            if (GameClearManager.Instance != null)
            {
                GameClearManager.Instance.ExecuteGameClear(player);
            }
            else
            {
                Debug.LogWarning("GameClearManager がシーン内に存在しません！");
            }
        }
    }
}