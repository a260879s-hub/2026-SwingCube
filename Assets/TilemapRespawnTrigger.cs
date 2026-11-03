using UnityEngine;

public class TilemapRespawnTrigger : MonoBehaviour
{
    public enum TriggerType
    {
        KillZone,       // 触れると即リスポーン（トゲ、マグマなど）
        CheckPoint      // 触れるとリスポーン地点更新
    }

    [Header("機能の選択")]
    public TriggerType triggerType = TriggerType.KillZone;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleTrigger(collision.gameObject, collision.contacts[0].point);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        HandleTrigger(collider.gameObject, collider.transform.position);
    }

    private void HandleTrigger(GameObject hitObject, Vector3 contactPoint)
    {
        // 接触したオブジェクトの親(root)から PlayerController を探す
        PlayerController player = hitObject.transform.root.GetComponent<PlayerController>();

        if (player == null) return; // プレイヤー以外（敵や攻撃など）なら無視

        if (RespawnManager.Instance == null)
        {
            Debug.LogWarning("RespawnManager がシーン内に存在しません！");
            return;
        }

        // タイプごとの処理
        if (triggerType == TriggerType.KillZone)
        {
            // リスポーン実行
            RespawnManager.Instance.RespawnPlayer(player.gameObject);
        }
        else if (triggerType == TriggerType.CheckPoint)
        {
            // チェックポイント更新（接触したタイルの位置、またはプレイヤーの位置）
            RespawnManager.Instance.UpdateRespawnPoint(contactPoint);
        }
    }
}