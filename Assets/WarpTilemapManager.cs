using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class WarpTilemapManager : MonoBehaviour
{
    [Header("ワープタイルのあるTilemap")]
    public Tilemap warpTilemap;

    [System.Serializable]
    public struct WarpData
    {
        [Header("入口タイルのマス座標 (X, Y, Z)")]
        public Vector3Int tilePosition;

        [Header("出口のワールド座標 (X, Y, Z)")]
        public Vector3 destinationPosition;
    }

    [Header("ワープ設定リスト")]
    public List<WarpData> warpList = new List<WarpData>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 自分自身（warpオブジェクト）との接触は無視する
        if (other.gameObject == gameObject) return;

        // 接触した相手、または自分自身から「Player」タグを持つオブジェクトを探す
        GameObject playerObj = null;

        if (other.CompareTag("Player"))
        {
            playerObj = other.gameObject;
        }
        else if (other.transform.root != null && other.transform.root.CompareTag("Player"))
        {
            playerObj = other.transform.root.gameObject;
        }
        else if (gameObject.CompareTag("Player"))
        {
            // このスクリプトがPlayer自身に付いている場合
            playerObj = gameObject;
        }

        // プレイヤー関係以外の接触は無視
        if (playerObj == null) return;

        Debug.Log($"【判定成功】プレイヤー({playerObj.name})がワープゾーンに接触しました");

        // プレイヤーの足元・全体の範囲（Bounds）からマス座標を取得
        Collider2D playerCollider = playerObj.GetComponent<Collider2D>();
        Bounds bounds = (playerCollider != null) ? playerCollider.bounds : other.bounds;

        Vector3Int minCell = warpTilemap.WorldToCell(bounds.min);
        Vector3Int maxCell = warpTilemap.WorldToCell(bounds.max);

        Debug.Log($"プレイヤーの範囲: Min{minCell} ~ Max{maxCell}");

        foreach (var data in warpList)
        {
            Debug.Log($"リストの設定座標: {data.tilePosition}");

            // プレイヤーの当たり判定領域内に設定したtilePositionが含まれているか確認
            if (data.tilePosition.x >= minCell.x && data.tilePosition.x <= maxCell.x &&
                data.tilePosition.y >= minCell.y && data.tilePosition.y <= maxCell.y)
            {
                // ワープ実行
                playerObj.transform.position = new Vector3(
                    data.destinationPosition.x,
                    data.destinationPosition.y,
                    playerObj.transform.position.z
                );

                // 物理速度のリセット
                Rigidbody2D rb = playerObj.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector2.zero;
                }

                Debug.Log("★ワープ成功！");
                return;
            }
        }

        Debug.Log("タイルの座標が一致しませんでした。");
    }
}