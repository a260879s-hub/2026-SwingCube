using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerController : MonoBehaviour
{
    private bool canControl = true; // 操作可能フラグ

    [Header("ステータス")]
    public float hp = 100f;
    public float mp = 100f;
    public float speed = 10f;
    public float jumpPower = 10f;

    [Header("戦闘移行設定")]
    [Tooltip("移行先の戦闘シーン名")]
    public string battleSceneName = "BattleScene";

    [Header("落下速度調整")]
    [Tooltip("落下時のみにかける重力の倍率（標準: 2.5〜4.0）")]
    public float fallMultiplier = 2.5f;

    [Header("参照")]
    public TextMeshProUGUI scoreText;
    public LineRenderer lineRenderer;      // ワイヤー表示用
    public LayerMask groundLayer;          // フックが刺さる地形のレイヤー

    [Header("フック設定")]
    public float maxHookDistance = 15f;    // フックが届く最大距離
    public float pullSpeed = 20f;          // 右クリックで引き寄せられる速度
    public float minSpeedToLoop = 8f;      // キー入力で追加加速できる上限スピード

    private Rigidbody2D rb;
    private DistanceJoint2D distanceJoint;
    private bool isGrounded = false;       // 足元が地面に着いているか
    private bool isTouchingGroundThisFrame = false; // 今フレーム足元に触れているか

    // フックの状態管理
    private Vector2 hookTarget;
    private bool isHooked = false;
    private bool isPulling = false;
    private float originalDrag;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        distanceJoint = GetComponent<DistanceJoint2D>();

        if (rb != null) originalDrag = rb.linearDamping;

        if (distanceJoint != null) distanceJoint.enabled = false;
        if (lineRenderer != null) lineRenderer.enabled = false;
    }

    void Update()
    {
        if (!canControl) return; // 操作不能時は入力処理をすべてスキップ

        if (!isPulling)
        {
            float x = Input.GetAxis("Horizontal");

            bool canAddForce = true;

            if (isHooked)
            {
                if (rb.linearVelocity.magnitude > minSpeedToLoop)
                {
                    canAddForce = false;
                }
            }

            if (canAddForce)
            {
                rb.AddForce(new Vector2(x * speed, 0));
            }

            // ジャンプ処理（足元が着地している場合のみ許可）
            bool isJumpPressed = Input.GetKeyDown(KeyCode.W) ||
                                 Input.GetKeyDown(KeyCode.UpArrow) ||
                                 Input.GetButtonDown("Jump");

            if (isJumpPressed && isGrounded)
            {
                rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
                isGrounded = false; // ジャンプした瞬間に空中判定へ
                isTouchingGroundThisFrame = false;
            }
        }

        HandleHookInput();
        UpdateHookVisual();
    }

    void FixedUpdate()
    {
        if (!canControl) return; // 操作不能時は移動更新をスキップ

        // 毎物理フレームの開始時に足元判定をチェック＆リセット
        isGrounded = isTouchingGroundThisFrame;
        isTouchingGroundThisFrame = false;

        if (isPulling)
        {
            Vector2 direction = (hookTarget - (Vector2)transform.position).normalized;
            rb.linearVelocity = direction * pullSpeed;

            if (Vector2.Distance(transform.position, hookTarget) < 0.5f)
            {
                rb.linearVelocity = Vector2.zero;
            }
        }
        else if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    private void HandleHookInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryShootHook(isPullMode: false);
        }
        if (Input.GetMouseButtonUp(0) && !isPulling)
        {
            ReleaseHook();
        }

        if (Input.GetMouseButtonDown(1))
        {
            TryShootHook(isPullMode: true);
        }
        if (Input.GetMouseButtonUp(1) && isPulling)
        {
            ReleaseHook();
        }
    }

    private void TryShootHook(bool isPullMode)
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 origin = transform.position;
        Vector2 direction = ((Vector2)mousePos - origin).normalized;

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, maxHookDistance, groundLayer);

        if (hit.collider != null)
        {
            hookTarget = hit.point;
            isHooked = true;
            isGrounded = false;
            isTouchingGroundThisFrame = false;

            if (isPullMode)
            {
                isPulling = true;
                if (distanceJoint != null) distanceJoint.enabled = false;
                rb.linearDamping = originalDrag;
            }
            else
            {
                isPulling = false;
                if (distanceJoint != null)
                {
                    distanceJoint.connectedAnchor = hookTarget;
                    distanceJoint.distance = Vector2.Distance(transform.position, hookTarget);
                    distanceJoint.maxDistanceOnly = true;
                    distanceJoint.enabled = true;
                }

                rb.linearDamping = 0f;
            }

            if (lineRenderer != null) lineRenderer.enabled = true;
        }
    }

    // フックを解除する内部処理
    public void ReleaseHook()
    {
        isHooked = false;
        isPulling = false;

        if (rb != null) rb.linearDamping = originalDrag;

        if (distanceJoint != null) distanceJoint.enabled = false;
        if (lineRenderer != null) lineRenderer.enabled = false;
    }

    private void UpdateHookVisual()
    {
        if (isHooked && lineRenderer != null)
        {
            lineRenderer.SetPosition(0, new Vector3(transform.position.x, transform.position.y, -1f));
            lineRenderer.SetPosition(1, new Vector3(hookTarget.x, hookTarget.y, -1f));
        }
    }

    // 物理的に接触している間、足元判定（法線ベクトル）が合っていれば「今フレーム接触あり」とする
    private void OnCollisionStay2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                isTouchingGroundThisFrame = true;
                return;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            SceneManager.LoadScene(battleSceneName);
        }
    }

    // 外部から操作権限を切り替えるメソッド
    public void SetControlActive(bool active)
    {
        canControl = active;
        if (!active)
        {
            // 慣性を消して完全に停止
            if (rb != null) rb.linearVelocity = Vector2.zero;
            // フック中であれば強制解除
            ReleaseHook();
        }
    }
}