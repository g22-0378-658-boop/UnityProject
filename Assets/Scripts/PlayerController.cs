using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 5f;
    public float shootForce = 14f;
    public float upwardArcModifier = 1.5f; // Higher values make the ball go higher

    [Header("References")]
    public Transform dribblePoint;
    public GameObject ball;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isDribbling = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        // Shoot only if we have the ball
        if (Input.GetKeyDown(KeyCode.Space) && isDribbling)
        {
            ShootBall();
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveInput.normalized * moveSpeed;

        if (isDribbling && ball != null)
        {
            ball.transform.position = dribblePoint.position;
        }
    }

    void ShootBall()
    {
        // Aim for the ScoreZone specifically
        GameObject targetZone = GameObject.Find("ScoreZone");

        if (targetZone != null && ball != null)
        {
            isDribbling = false;
            GameObject currentBall = ball;
            Rigidbody2D ballRb = currentBall.GetComponent<Rigidbody2D>();
            Collider2D ballCollider = currentBall.GetComponent<Collider2D>();

            // Calculate direction toward the hoop
            Vector2 targetPos = targetZone.transform.position;
            Vector2 currentPos = currentBall.transform.position;

            // We aim for a point slightly ABOVE the hoop to create a "swish" path
            Vector2 aimPoint = new Vector2(targetPos.x, targetPos.y + 1.5f);
            Vector2 direction = (aimPoint - currentPos).normalized;

            // Reset ball physics before applying shot
            ballRb.linearVelocity = Vector2.zero;
            ballRb.angularVelocity = 0f;

            // Apply the force with an extra boost to the Y (vertical) axis for the arc
            Vector2 finalShootVector = new Vector2(direction.x, direction.y * upwardArcModifier);
            ballRb.AddForce(finalShootVector * shootForce, ForceMode2D.Impulse);

            // Start visual scaling (makes it look 3D)
            StartCoroutine(AnimateBallScale(currentBall));

            ball = null; // We no longer "own" the ball
        }
    }

    IEnumerator AnimateBallScale(GameObject ballObj)
    {
        float duration = 1.2f; // Time the ball stays "enlarged" in the air
        float elapsed = 0;

        Vector3 originalScale = Vector3.one;
        Vector3 peakScale = originalScale * 1.5f; // Make it 50% bigger at peak

        while (elapsed < duration && ballObj != null)
        {
            elapsed += Time.deltaTime;
            float percent = elapsed / duration;

            // Use a Sine wave to scale up then back down
            float curve = Mathf.Sin(percent * Mathf.PI);
            ballObj.transform.localScale = Vector3.Lerp(originalScale, peakScale, curve);

            yield return null;
        }

        if (ballObj != null)
        {
            ballObj.transform.localScale = originalScale;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Pick up the ball if we touch it and don't already have one
        if (collision.gameObject.CompareTag("Ball") && ball == null)
        {
            isDribbling = true;
            ball = collision.gameObject;
        }
    }
}