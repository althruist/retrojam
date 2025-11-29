using UnityEngine;
using UnityEngine.Timeline;

public class Player : MonoBehaviour
{
    // player index
    [SerializeField] private int playerIndex;
    [SerializeField] private Animator animator;

    public int _playerIndex
    {
        get
        {
            return playerIndex;
        }
        set
        {
            playerIndex = value;
        }
    }

    // variables

    [SerializeField] private float crouchYOffset = -0.5f;
    private float originalY;


    [SerializeField] private float speed = 3f;
    private Vector3 normalScale = new Vector3(1f, 1f, 1f);
    private Vector3 crouchScale = new Vector3(1f, 0.5f, 1f);

    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float jumpDuration = 0.5f; // total jump time
    private bool isJumping = false;
    private float jumpStartTime;
    private float jumpStartY;

    private int health;

    void Start()
    {
        originalY = transform.position.y;

        health = 100;

        // Set initial facing direction
        if (playerIndex == 1)
        {
            transform.localScale = new Vector3(-1, 1, 1); // face right
        }
        else if (playerIndex == 2)
        {
            transform.localScale = new Vector3(1, 1, 1); // face left
        }
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Crouch();
        Jump();
        SimpleAttack();
    }


    // Movement 
    void Move()
    {
        if (playerIndex == 1)
        {
            if (Input.GetKey(KeyCode.A))
            {
                transform.Translate(Vector2.left * speed * Time.deltaTime, Space.World);
                transform.localScale = new Vector3(-1, 1, 1); // face left
            }

            if (Input.GetKey(KeyCode.D))
            {
                transform.Translate(Vector2.right * speed * Time.deltaTime, Space.World);
                transform.localScale = new Vector3(1, 1, 1); // face right
            }
        }

        if (playerIndex == 2)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                transform.Translate(Vector2.left * speed * Time.deltaTime, Space.World);
                transform.localScale = new Vector3(-1, 1, 1); // face left
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                transform.Translate(Vector2.right * speed * Time.deltaTime, Space.World);
                transform.localScale = new Vector3(1, 1, 1); // face right
            }
        }
    }


    void Crouch()
    {
        bool isCrouched = false;

        if (playerIndex == 1)
            isCrouched = Input.GetKey(KeyCode.S);
        else if (playerIndex == 2)
            isCrouched = Input.GetKey(KeyCode.DownArrow);

        animator.SetBool("isCrouched", isCrouched);

        Vector3 pos = transform.position;

        if (isCrouched)
            pos.y = originalY + crouchYOffset;
        else
            pos.y = originalY;

        transform.position = pos;
    }


    void Jump()
    {
        bool isJump = false;

        if (playerIndex == 1)
        {
            if (!isJumping && Input.GetKeyDown(KeyCode.W))
            {
                isJump = true;
                isJumping = true;
                jumpStartTime = Time.time;
                jumpStartY = transform.position.y;
            }

            if (isJumping)
            {
                float t = (Time.time - jumpStartTime) / jumpDuration;
                if (t >= 1f)
                {
                    t = 1f;
                    isJumping = false;

                    // Update ground level after landing
                    originalY = transform.position.y;
                }

                float newY = jumpStartY + 4f * jumpHeight * t * (1 - t);
                Vector3 pos = transform.position;
                pos.y = newY;
                transform.position = pos;
            }




        }

        if (playerIndex == 2)
        {
            if (!isJumping && Input.GetKeyDown(KeyCode.UpArrow))
            {
                isJump = true;
                isJumping = true;
                jumpStartTime = Time.time;
                jumpStartY = transform.position.y;
            }

            if (isJumping)
            {
                float t = (Time.time - jumpStartTime) / jumpDuration;
                if (t >= 1f)
                {
                    t = 1f;
                    isJumping = false;

                    // Update ground level after landing
                    originalY = transform.position.y;
                }

                float newY = jumpStartY + 4f * jumpHeight * t * (1 - t);
                Vector3 pos = transform.position;
                pos.y = newY;
                transform.position = pos;
            }


            if (animator.GetBool("isJump") != isJump)
                animator.SetBool("isJump", isJump);

        }
    }

    // Attacks

    public void TakeDamage(int amount)
    {
        health -= amount;
        Debug.Log("Player " + playerIndex + " health: " + health);
        if (health <= 0)
        {
            Debug.Log("Player " + playerIndex + " is KO!");
            // Handle death/respawn here
        }
    }

    void SimpleAttack()
    {
        bool isPunching = false;
        if (playerIndex == 1 && Input.GetKeyDown(KeyCode.C))
        {
            CheckAttack();
            isPunching = true;
        }

        if (playerIndex == 2 && Input.GetKeyDown(KeyCode.K))
        {
            CheckAttack();
            isPunching = true;
        }

        if (animator.GetBool("isPunching") != isPunching)
            animator.SetBool("isPunching", isPunching);
    }

    void CheckAttack()
    {
        Debug.Log("Attacking... checking for hit.");

        BoxCollider2D col = GetComponent<BoxCollider2D>();
        float direction = transform.localScale.x > 0 ? 1f : -1f;

        Vector2 attackPos = new Vector2(
            col.bounds.center.x + direction * col.bounds.size.x * 0.6f,
            col.bounds.center.y
        );

        Vector2 attackSize = new Vector2(
            col.bounds.size.x * 0.6f,
            col.bounds.size.y * 0.8f
        );

        Collider2D[] hits = Physics2D.OverlapBoxAll(attackPos, attackSize, 0f, LayerMask.GetMask("Player"));
        foreach (Collider2D hit in hits)
        {
            if (hit.attachedRigidbody != null && hit.attachedRigidbody.gameObject == gameObject)
                continue; // skip self

            Player p = hit.GetComponentInParent<Player>();
            if (p != null && p._playerIndex != playerIndex)
            {
                p.TakeDamage(10);
                Debug.Log("Hit player " + p._playerIndex);
            }
        }

    }
    void OnDrawGizmosSelected()
    {
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col == null) return;

        float direction = transform.localScale.x > 0 ? 0f : 0f;

        Vector2 attackCenter = new Vector2(
            col.bounds.center.x + direction * col.bounds.size.x * 0.6f,
            col.bounds.center.y
        );

        Vector2 attackSize = new Vector2(
            col.bounds.size.x * 0.6f,
            col.bounds.size.y * 0.8f
        );

        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(attackCenter, attackSize);
    }





}

