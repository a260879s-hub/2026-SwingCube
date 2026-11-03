using UnityEngine;
using TMPro; // TextMeshProを使用する場合

public class GameClearManager : MonoBehaviour
{
    public static GameClearManager Instance { get; private set; }

    [Header("クリア時に表示するUI要素")]
    public GameObject gameClearUI;      // クリア画面の親オブジェクト（Panelなど）
    public TextMeshProUGUI clearText;   // クリアテキスト（省略可）

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
        // 開始時はクリアUIを非表示にしておく
        if (gameClearUI != null)
        {
            gameClearUI.SetActive(false);
        }
    }

    // ゲームクリア処理
    public void ExecuteGameClear(PlayerController player)
    {
        // 1. プレイヤーの操作を完全停止＆移動をピタッと止める
        if (player != null)
        {
            player.SetControlActive(false);
        }

        // 2. クリアUIを表示
        if (gameClearUI != null)
        {
            gameClearUI.SetActive(true);
        }

        if (clearText != null)
        {
            clearText.text = "ゲームクリア！";
        }

        Debug.Log("【ゲームクリア】プレイヤーの操作を停止しました。");
    }
}