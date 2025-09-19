using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private Animator anim;

    public GameObject Trail;//冲刺粒子
    public ParticleSystem playerPS;//跳跃粒子
    public ParticleSystem dieparticle;//死亡粒子

    [SerializeField] private GameObject EButtonPrefab;
    private GameObject EButtonInstance;

   
    public float fadeSpeed = 1.0f; // 淡入淡出速度
    private SpriteRenderer spriteRenderer;
    private bool isFadingOut = false;
    
    [SerializeField] private PauseUI pauseUI;

    [Header("Audio Controller")]
    [SerializeField] private AudioController audioController; // 引用方案一的AudioPlayer脚本


        [Header("Movement info")]
    [SerializeField] private float moveSpeed = 1.1f;
    private float xInput;

    [Header("Dash info")]
    [SerializeField] private float dashDuration = 0.4f;// Time of the dash
    [SerializeField] private float dashSpeed = 3.3f;   // Speed of the dash
    [SerializeField] private float dashCoolDown = 2f;// Time allowed to dash again
    private float dashDurationCounter;
    private float dashCoolDownCounter;
    private int facingDirection = 1;  // 1 = right, -1 = left

    [Header("Jump info")]
    [SerializeField] private float jumpForce = 1.56f;
    [SerializeField] private float maxJumpTime = 0.2f;
    [SerializeField] private float maxJumpForce = 3.3f;
    [SerializeField] private float commonGravity = 1f;
    [SerializeField] private float multiplyGravity = 3.5f;
    [SerializeField] private float coyoteTime = 0.2f;  // Time allowed to jump after leaving ground
    [SerializeField] private int jumpCountLimit = 2;// Limit of jumps
    [SerializeField] private float jumpBufferTime = 0.2f; // 跳跃缓冲时间
    private int jumpCount;// Count of jumps
    private float jumpTimeCounter;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    private bool isDashing = false;
    private bool isFalling = false;
    private bool facingRight = true;
    private bool isRunning = false;
    [Header("Collision info")]
    [SerializeField] private float groundCheckDistance = 0.16f;
    [SerializeField] private LayerMask whatIsGround;
    private bool isGroundedBefore;
    [SerializeField] private bool isGrounded;
    private float _dontDestroyE = 0.1f;
    [Header("Falling Settings")]
    [SerializeField] private float maxFallSpeed = 15f; // 最大下落速度
    [SerializeField] private float hardLandThreshold = 12f; // 重落地速度阈值
    private bool _reachedMaxFallSpeed;
    private Transform _respawnPoint;
    private ResetScene _resetScene;
    private float _lastVerticalVelocity;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        _respawnPoint = GameObject.FindWithTag("Respawn").transform;
        //pauseUI = FindObjectOfType<PauseUI>();
        if (pauseUI == null)pauseUI = FindObjectOfType<PauseUI>();
        if (pauseUI == null){
            Debug.Log("No PauseUI found");
        }
        else Debug.Log("PauseUI found");
        _resetScene = FindObjectOfType<ResetScene>();
        if (_resetScene == null)
            Debug.Log("No ResetScene found");
        else Debug.Log("ResetScene found");
    }

    // Update is called once per frame
    void Update()
    {
        if (pauseUI.GetPauseStatus())
        {
            return;
        }
        // --- CRITICAL: Define Execution Order ---
        // 1. Read Input
        CheckInput();

        // 2. Check Environment State (Collisions) *BEFORE* acting on it
        CollisionChecks(); // Updates isGrounded

        // 3. Update Timers based on current state
        UpdateCoyoteTime(); // Uses isGrounded
        UpdateJumpBuffer(); // Reads input, updates buffer timer
        UpdateDashDuration(); // Updates dash timers

        // 4. Process Actions based on Input and State
        ProcessJumpInput(); // Uses buffer, coyote, jumpCount, isGrounded -> Calls PerformJump
        Move();             // Uses xInput, dash state, potentially affected by jump velocity change
        
        // 5. Apply Physics Adjustments and Secondary Effects
        ApplyExtraGravity(); // Adjusts gravity based on velocity and input state
        // LimitJumpHeight(); // Consider if this is still needed with ApplyExtraGravity logic - Keep commented for now
        FlipController();   // Visual flip based on velocity
        UpdateRunSound();   // Audio based on state
        UpdateFallingSound();// Audio based on state
        CheckLanding();     // Landing logic (uses isGroundedBefore, isGrounded, velocity) - MUST run after CollisionChecks
        Fade();             // Visual fade
        if (EButtonInstance != null)
        {
            if (_dontDestroyE > 0)
            {
                _dontDestroyE -= Time.deltaTime;
            }
            else
            {
                Destroy(EButtonInstance);
                _dontDestroyE = 0.1f;
            }
        }
        // --- Optional: Debugging ---
        // Place AFTER all state updates for the frame to see final values
        // Debug.Log($"Frame: {Time.frameCount} | isGrounded: {isGrounded} | jumpCount: {jumpCount} | coyoteCounter: {coyoteTimeCounter:F2} | jumpBuffer: {jumpBufferCounter:F2} | VelocityY: {rb.velocity.y:F2} | InputDown: {Input.GetButtonDown("Jump")} | InputHeld: {Input.GetButton("Jump")}");
    }

    private void CheckLanding() // Runs AFTER CollisionChecks in Update
    {
        if (!isGroundedBefore && isGrounded) { // Just landed
            jumpCount = 0; // Reset jump count here. This is the crucial reset point.
            // ... rest of landing audio logic ...
            float fallSpeed = Mathf.Abs(_lastVerticalVelocity);
            if (audioController!= null)
                if (_reachedMaxFallSpeed) {
                    audioController.Play("HardLand");
                }
                else if (fallSpeed > hardLandThreshold) {
                    audioController.Play("HardLand");
                }
                else if (fallSpeed > 2f) {
                    audioController.Play("SoftLand");
                }
            _reachedMaxFallSpeed = false;
        }
        _lastVerticalVelocity = rb.velocity.y;
    }
    void FixedUpdate() {
    // 限制最大下落速度
    if (rb.velocity.y < -maxFallSpeed) {
        rb.velocity = new Vector2(rb.velocity.x, -maxFallSpeed);
        _reachedMaxFallSpeed = true;
    }
}
    private void UpdateRunSound() {
        bool shouldPlay = isGrounded && Mathf.Abs(xInput) > 0.1f;
        float speedFactor = rb.velocity.magnitude / moveSpeed;
        if (audioController != null)
            audioController.PlayRunLoop(shouldPlay, speedFactor);
    }
private void CheckInput()
    {
        xInput = Input.GetAxis("Horizontal");
        if (xInput!=0)
        {
            playerPS.Play();
            if (!isRunning)
            {
                isRunning = true;
                anim.SetBool("IsRunning", true);
                
            }
        }
        else
        {
            if (isRunning)
            {
                isRunning = false;
                anim.SetBool("IsRunning", false);
            }
        }


        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if(dashCoolDownCounter <= 0)
            {
                dashDurationCounter = dashDuration;
                dashCoolDownCounter = dashCoolDown;
            }
            
        }
    }
    private void Move()// Move and dash
    {
        
        UpdateDashDuration();
        if (dashDurationCounter > 0)
        {
            if (!isDashing)
            {
                isDashing = true;
                anim.SetBool("IsDashing", true);
                Debug.Log("Dashing");
                Trail.SetActive(true);//激活冲刺粒子
               
            }
            rb.velocity = new Vector2(facingDirection * dashSpeed, 0);
        }
        else
        {
            if (isDashing)
            {
                isDashing = false;
                anim.SetBool("IsDashing", false);
                Trail.SetActive(false);//结束冲刺粒子
            }
            rb.velocity = new Vector2(xInput * moveSpeed, rb.velocity.y);
        }

        if (rb.velocity.y < -1e-4f)
        {
            //Debug.Log(rb.velocity.y);
            if (!isFalling)
            {
                isFalling = true;
                anim.SetBool("IsFalling", true);
                Debug.Log("Playing playerPS1");
                playerPS.Play();
            }
            
        }
        else
        {
            if (isFalling)
            {
                isFalling = false;
                anim.SetBool("IsFalling", false);
            }
        }

        
    }

    private void UpdateDashDuration()
    {
        if (dashDurationCounter >= 0)
        {
            
            dashDurationCounter -= Time.deltaTime;
        }
        else
        {
            if (dashCoolDownCounter >= 0)
            {
                dashCoolDownCounter -= Time.deltaTime;
            }
        }
    }
    
    private void UpdateFallingSound() {
        bool isFalling = !isGrounded && rb.velocity.y < -2f; // 2f为下落速度阈值
        float fallSpeedNormalized = Mathf.Abs(rb.velocity.y) / 10f; // 10f为最大下落速度参考值
        if (audioController != null)
         audioController.HandleFalling(isFalling, fallSpeedNormalized);
    }

    private void UpdateCoyoteTime() // Should run after CollisionChecks
    {
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
    }

    void UpdateJumpBuffer()
    {
        if (Input.GetButtonDown("Jump"))
        {
            Debug.Log("Playing playerPS2");
            playerPS.Play();//跳跃粒子
            jumpBufferCounter = jumpBufferTime; // 重置跳跃缓冲
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime; // 减少跳跃缓冲
        }
    }

    void ProcessJumpInput() // Should run after CollisionChecks and UpdateCoyoteTime
    {
        if (jumpBufferCounter > 0f)
        {
            //Debug.Log("Is Grounded: " + isGrounded);
            // Combined check: Can we jump via coyote OR via air jump allowance?
            bool canAirJump = !isGrounded && jumpCount < jumpCountLimit; // Check air jump condition separately for clarity
            if (coyoteTimeCounter > 0f || canAirJump)
            {
                PerformJump();          // Apply jump velocity

                // Consume resources AFTER successful jump
                jumpCount++;            // Increment jump count REGARDLESS of ground/air
                coyoteTimeCounter = 0f; // Consume coyote time (if any was left)
                jumpBufferCounter = 0f; // Consume the jump buffer
            }
        }
    }

    // 应用额外重力以加速下落
    void ApplyExtraGravity()
    {
        // Apply higher gravity if falling OR if moving up but jump key isn't held (variable jump height)
        if (rb.velocity.y < 0 || (rb.velocity.y > 0 && !Input.GetButton("Jump"))) {
            rb.gravityScale = multiplyGravity;
        } else { // Moving up and holding jump (or stationary with positive initial velocity)
            rb.gravityScale = commonGravity;
        }
    }
    void LimitJumpHeight()
    {
        if (Input.GetKeyUp(KeyCode.Space) && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f); // 减少向上的速度
            coyoteTimeCounter = 0f; // 确保不能再次触发Coyote时间跳跃
        }
    }
    // Perform a jump action
    private void PerformJump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        // jumpBufferCounter = 0f; // <-- 移除或注释掉这行
        if (audioController != null)
            audioController.Play("Jump");
        Debug.Log("Playing playerPS3");
        playerPS.Play(); // 确保跳跃粒子在这里播放（或者你原本的UpdateJumpBuffer位置也可以）
    }

    

    private void Flip()
    {
        facingDirection *= -1;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
    }

    private void FlipController()
    {
        if (rb.velocity.x > 0 && !facingRight)
        {
            Flip();
        }
        else if (rb.velocity.x < 0 && facingRight)
        {
            Flip();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }

    private void CollisionChecks() // Should run early in Update
    {
        isGroundedBefore = isGrounded;
        isGrounded = Physics2D.Raycast(
            transform.position,
            Vector2.down,
            groundCheckDistance,
            whatIsGround);

        UnityEngine.Debug.DrawRay(transform.position, Vector2.down * groundCheckDistance, isGrounded ? Color.green : Color.red);

        if (isGrounded ^ isGroundedBefore)
        {
            anim.SetBool("IsGrounded", isGrounded);
            // Reset jump count ONLY on landing (moved to CheckLanding for better context)
            // if (isGrounded) {
            //     // jumpCount = 0; // Moved
            // }
        }
    }

    private void Die()
    {
        if (audioController != null)
          audioController.Play("Death");
        isFadingOut = true;
        dieparticle.Play();
        rb.bodyType = RigidbodyType2D.Static;
     
        Invoke("RecodePoint", 1.5f);
        
    }
    public void RecodePoint()
    {
        if (_resetScene != null)
        {
            _resetScene.OnSceneReset();
        }
        // transform.position = new Vector3(
        //     _respawnPoint.position.x,
        //     _respawnPoint.position.y - 20 * (transform.position.y < -10 ? 1 : 0),
        //     _respawnPoint.position.z 
        // );
        transform.position = _respawnPoint.position;
        // 获取当前颜色

        // 获取当前颜色
        Color currentColor = spriteRenderer.color;
        currentColor.a = 1;                                                     
        spriteRenderer.color = currentColor;
        rb.bodyType = RigidbodyType2D.Dynamic;
    }
    private void Fade() //人物淡出
    {
        if (spriteRenderer != null)
        {
            
            // 获取当前颜色
            Color currentColor = spriteRenderer.color;

            // 根据isFadingOut状态调整Alpha值
            if (isFadingOut)
            {

                if (currentColor.a > 0)
                {
                    currentColor.a -= fadeSpeed * Time.deltaTime; // 逐渐透明
                    // 应用新的颜色
                    spriteRenderer.color = currentColor;
                }

                if (currentColor.a <= 0)
                {
                    currentColor.a = 0;
                    // 应用新的颜色
                    spriteRenderer.color = currentColor;
                    isFadingOut = false; 
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Trap")
        {
            
            Die();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("TimeWrapper"))
        {
            if (EButtonInstance == null)
                EButtonInstance = Instantiate(EButtonPrefab);
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("TimeWrapper"))
        {
            Debug.Log("Set _dontDestroyE = 1");
            _dontDestroyE = 0.1f;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("TimeWrapper"))
        {
            if (EButtonInstance != null)
            {
                Destroy(EButtonInstance);
            }
        }
    }
}
