using UnityEngine;
using UniRx;
using DG.Tweening;
using UnityEngine.Rendering;

[RequireComponent(typeof(Collider2D))]
public class MovementComponent : MonoBehaviour
{
    [Header("Lane / Move")]
    public int laneIndex = 1;
    public float moveSpeed = 1.6f;      // 왼쪽(-x) 진행 속도
    public float radius = 0.35f;
    public float nearPadding = 0.2f;

    [SerializeField] private int _groundLayer = 0;
    [SerializeField] private LayerMask _groundMask;

    [Header("Jump (Physics)")]
    public float jumpImpulse = 7f;      // 점프 힘
    public float cooldown = 0.25f;       // 점프 쿨다운
    public float minAdvanceX = 0.05f;   // 직전 점프 지점 대비 최소 전진
    [SerializeField] float groundGrace = 0.08f;
    float _ignoreGroundUntil;

    Rigidbody2D rb;

    float nextJumpAt;
    [SerializeField] bool isGrounded = true;
    float lastJumpStartX;

    CompositeDisposable cd = new();

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.gravityScale = 1f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    void OnEnable()
    {
        if (_groundLayer == 0) _groundLayer = LaneLayers.groundLayer[laneIndex];
        if (_groundMask == 0) _groundMask = 1 << _groundLayer;

        nextJumpAt = 0f;
        lastJumpStartX = float.NegativeInfinity;

        LaneService.LaneRegister(laneIndex, this);

        Observable.EveryFixedUpdate()
            .Subscribe(_ => FixedTick())
            .AddTo(cd);
    }

    void OnDisable()
    {
        cd.Clear();
        LaneService.LaneUnregister(laneIndex, this);
    }

    void OnDestroy()
    {
        cd.Dispose();
    }

    void FixedTick()
    {
        float dt = Time.fixedDeltaTime;

        isGrounded = (Time.time >= _ignoreGroundUntil) && rb.IsTouchingLayers(_groundMask);

        // 1) X 이동
        if (isGrounded)
            rb.MovePosition(rb.position + Vector2.left * (moveSpeed * dt));

        // 2) 앞 좀비 확인
        var front = LaneService.GetMovementComponentInFrontLane(laneIndex, this);
        if (front == null) return;

        float gapX = transform.position.x - front.transform.position.x;
        float needX = radius + front.radius + nearPadding;
        bool close = gapX > 0f && gapX < needX;

        // 4) 재점프 가드: 쿨다운 + 최소 전진 + 지면
        bool cooldownOK = Time.time >= nextJumpAt;
        // 첫 점프는 무조건 통과, 이후에는 전진거리 체크
        float curX = rb.position.x; // MovePosition 쓰므로 rb 기준 추천
        bool advancedOK = float.IsNegativeInfinity(lastJumpStartX) ||
                          (Mathf.Abs(curX - lastJumpStartX) >= minAdvanceX);
        if (close && cooldownOK && advancedOK && isGrounded)
        {
            Jump();
        }
    }

    void Jump()
    {
        isGrounded = false;
        lastJumpStartX = rb.position.x;

        Vector2 dir = new Vector2(-0.4f, 1f).normalized;
        rb.AddForce(dir * jumpImpulse, ForceMode2D.Impulse);

        nextJumpAt = Time.time + cooldown;
        _ignoreGroundUntil = Time.time + groundGrace;
    }

    bool HasGroundContact(Collision2D c)
    {
        for (int i = 0; i < c.contactCount; i++)
            if (c.GetContact(i).normal.y > 0.5f) return true;
        return false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (HasGroundContact(collision))
        {
            isGrounded = true;
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (HasGroundContact(collision))
        {
            isGrounded = true;
        }
    }

    public void SetLane(int lane)
    {
        LaneService.LaneUnregister(laneIndex, this);
        laneIndex = Mathf.Clamp(lane, 0, LaneService.LaneCount - 1);

        // 레이어 변경
        if (LaneLayers.laneLayer != null && laneIndex < LaneLayers.laneLayer.Length)
        {
            int newLayer = LaneLayers.laneLayer[laneIndex];
            foreach (var t in GetComponentsInChildren<Transform>(true))
                t.gameObject.layer = newLayer;
        }

        _groundLayer = LaneLayers.groundLayer[laneIndex];
        _groundMask = 1 << _groundLayer;

        LaneService.LaneRegister(laneIndex, this);
    }
}