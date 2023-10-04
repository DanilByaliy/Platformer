using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hero : Entity
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private int lives;
    [SerializeField] private float jumpForce = 10f;
    private bool isGrounded = false;
    [SerializeField] private Image[] hearts;
    [SerializeField] private Sprite aliveHeart;
    [SerializeField] private Sprite deadHeart;
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprite;
    public static Hero Instance { get; set; }

    private States State {
        get { return (States)anim.GetInteger("state"); }
        set { anim.SetInteger("state", (int)value); }
    }

    private void Awake() {
        lives = 5;
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        if (isGrounded) State = States.idle;
        if (Input.GetButton("Horizontal")) Run();
        if (isGrounded && (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.UpArrow))) Jump();
        UpdateLives();
    }

    private void FixedUpdate() {
        CheckGround();
    }

    private void Run() {
        if (isGrounded) State = States.run;

        Vector3 dir = transform.right * Input.GetAxis("Horizontal");
        transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, speed * Time.deltaTime);
        sprite.flipX = dir.x < 0.0f;
    }

    private void Jump() {
        rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
    }

    private void CheckGround() {
        Collider2D[] collider = Physics2D.OverlapCircleAll(transform.position, 0.3f);
        isGrounded = collider.Length > 1;

        if (!isGrounded) State = States.jump;
    }

    private void UpdateLives() {
        for (int i = lives; i < 5; i++) {
            hearts[i].sprite = deadHeart;
        }
    }

    public override void GetDamage() { // - override
        lives -= 1;

        if (lives > 0) return;
        foreach (var h in hearts) h.sprite = deadHeart;
        Die();
    }
}

public enum States {
    idle,
    run,
    jump
}
