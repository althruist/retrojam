using UnityEngine;
using UnityEngine.Timeline;

public class Player : MonoBehaviour
{
    // player index
    [SerializeField] private int playerIndex;

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
        health = 100;
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
                transform.rotation = Quaternion.identity;
            }

            if (Input.GetKey(KeyCode.D))
            {
                transform.Translate(Vector2.right * speed * Time.deltaTime, Space.World);
                transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
            }
            
        }

        if (playerIndex == 2)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                transform.Translate(Vector2.left * speed * Time.deltaTime, Space.World);
                transform.rotation = Quaternion.identity;
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                transform.Translate(Vector2.right * speed * Time.deltaTime, Space.World);
                transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
            }
        }
    }

    void Crouch()
    {
        if (playerIndex == 1)
        {
            if (Input.GetKey(KeyCode.S))
            {
                transform.localScale = crouchScale;
            }
            else
            {
                transform.localScale = normalScale;
            }
        }

        if (playerIndex == 2)
        {
            if (Input.GetKey(KeyCode.DownArrow))
            {
                transform.localScale = crouchScale;
            }
            else
            {
                transform.localScale = normalScale;
            }
        }
    }

    void Jump()
    {
        if (playerIndex == 1)
        {

            if (!isJumping && Input.GetKeyDown(KeyCode.W))
            {
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
                }

                float newY = jumpStartY + 4f * jumpHeight * t * (1 - t);
                Vector3 pos = transform.position;
                pos.y = newY;
                transform.position = pos;
            }

        }
    }

    // Health



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
        if (playerIndex == 1 && Input.GetKeyDown(KeyCode.C))
        {
            CheckAttack();
        }

        if (playerIndex == 2 && Input.GetKeyDown(KeyCode.O)) 
        {
            CheckAttack();
        }
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

        Collider2D hit = Physics2D.OverlapBox(
            attackPos,
            attackSize,
            0f,
            LayerMask.GetMask("Player")
        );

        if (hit != null && hit.gameObject != gameObject)
        {
            Player p = hit.GetComponent<Player>();
            if (p != null && p._playerIndex != playerIndex)
            {
                p.TakeDamage(10);
            }
        }
    }


    private void OnDrawGizmosSelected()
    {
        BoxCollider2D col = GetComponent<BoxCollider2D>();

        float direction = transform.rotation == Quaternion.identity ? -1f : 1f;

        Vector2 attackPos = new Vector2(
            col.bounds.center.x + direction * (col.bounds.size.x * 0.6f),
            col.bounds.center.y
        );

        Vector2 attackSize = new Vector2(
            col.bounds.size.x * 0.6f,
            col.bounds.size.y * 0.8f
        );

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackPos, attackSize);
    }


}
