using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    // どこからでもアクセスできるようにシングルトン化
    public static RespawnManager Instance { get; private set; }

    [Header("初期リスポーン地点")]
    public Transform defaultRespawnPoint;

    // 現在のリスポーン位置
    private Vector3 currentRespawnPosition;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 初期リスポーン位置を設定（Transformが設定されていなければ自身の位置）
        if (defaultRespawnPoint != null)
        {
            currentRespawnPosition = defaultRespawnPoint.position;
        }
        else
        {
            currentRespawnPosition = transform.position;
        }
    }

    // リスポーン地点を更新する
    public void UpdateRespawnPoint(Vector3 newPosition)
    {
        currentRespawnPosition = newPosition;
        Debug.Log("チェックポイント更新: " + newPosition);
    }

    // プレイヤーをリスポーンさせる
    public void RespawnPlayer(GameObject playerObject)
    {
        // プレイヤーの本体（親オブジェクト）を取得
        Transform playerTransform = playerObject.transform.root;

        // プレイヤーの位置をリスポーン地点に移動
        playerTransform.position = currentRespawnPosition;

        // Rigidbody2Dの速度をリセット（落下慣性を消す）
        Rigidbody2D rb = playerTransform.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        Debug.Log("プレイヤーをリスポーンさせました。");
    }
}