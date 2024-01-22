using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float speed = 30f;
    [SerializeField]
    private float gravity = -30f;
    [SerializeField]
    private LayerMask whatIsGround;
    private AudioSource audioSource;

    public AudioClip jump;
    public AudioClip death;
    public AudioClip trinketOrCheckpoint;

    public GameManager gameManager;

    public float moveInput;
    public float waitForDeath;
    public bool isDying = false;
    public bool isGrounded = true;

    //saving positions for player and camera 
    private Vector3 startPosition = new Vector3(-15f, -3f, 0f);
    private Vector3 lvl4Pos = new Vector3(-17f, -28f, 0f);
    private Vector3 checkpointPos = new Vector3(-8f, -14f, 0f);
    private Vector3 startCameraPosition = new Vector3(0f, 0f, -10f);
    private Vector3 checkpointCameraPosition = new Vector3(0f, -20f, -10f);
    private Vector3 cameraPosition = new Vector3(0f, 0f, -10f);
    public float cameraMoveAmount = 35f;

    private Rigidbody2D rb;
    private Collider2D mCollider;
    private Animator playerAnimator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mCollider = GetComponent<Collider2D>();
        playerAnimator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!isDying) {
            moveInput = Input.GetAxis("Horizontal");
            isGrounded = checkIsGrounded();

            if (Input.GetButtonDown("Vertical") && isGrounded && !isDying)
            {
                audioSource.PlayOneShot(jump);
                gravity = -gravity;
                //Apply fake gravity constantly based on direction 
                rb.gravityScale = gravity > 0 ? 1f : 0f;
                FlipVertically();
            }

            if (moveInput > 0.0f || moveInput < 0.0f)
            {
                playerAnimator.SetBool("isWalking", true);
            }
            else
            {
                playerAnimator.SetBool("isWalking", false);
            }

            rb.velocity = new Vector2(moveInput * speed, isGrounded ? 0.0f : gravity);

        }

        
      
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Spikes"))
        {
            //Debug.Log("DEAD");
            //StartCoroutine(ManageDeath());
            playerAnimator.SetBool("isDying", true);
            
            StartCoroutine(ManageDeath());
            //playerAnimator.SetBool("isDying", false);
        }

        if (collision.gameObject.CompareTag("trans1"))
        { 
            cameraPosition.x += 35f;
            Camera.main.transform.position = cameraPosition;
        }

        if (collision.gameObject.CompareTag("trans2"))
        {
            //change player position
            transform.position = lvl4Pos;
            cameraPosition.x -= 70f;
            cameraPosition.y -= 20f;
            Camera.main.transform.position = cameraPosition;
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("coin"))
        {
            Destroy(collision.gameObject);
            audioSource.PlayOneShot(trinketOrCheckpoint);
            gameManager.AddTrinket();

        }
        else if (collision.gameObject.CompareTag("checkpoint"))
        {
            //Debug.Log("CHECKPOINT");
            audioSource.PlayOneShot(trinketOrCheckpoint);
            startPosition = checkpointPos;
            startCameraPosition = checkpointCameraPosition;

        }
    }

    bool checkIsGrounded()
    {
        Vector2 direction = gravity > 0 ? Vector2.up : Vector2.down;
        RaycastHit2D raycastHit = Physics2D.BoxCast(mCollider.bounds.center, mCollider.bounds.size, 0f, direction, 0.1f, whatIsGround);
        return raycastHit.collider != null;
    }


    void FlipVertically()
    {
        Vector2 Scalar = transform.localScale;
        Scalar.y *= -1;
        transform.localScale = Scalar;

    }

    IEnumerator ManageDeath()
    {
        isDying = true;
        //playerAnimator.SetBool("isDying", true);
        audioSource.PlayOneShot(death);
        //playerAnimator.SetBool("isDying", false);
        rb.Sleep();


        yield return new WaitForSeconds(waitForDeath);
        if (gravity > 0.0f)
        {
            FlipVertically();
            gravity *= -1;
        }
        playerAnimator.SetBool("isDying", false);

        transform.position = startPosition;
     
        rb.WakeUp();
        cameraPosition = startCameraPosition;
        Camera.main.transform.position = startCameraPosition;

        isDying = false;
    }

}
