using System.Collections;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

[RequireComponent(typeof(CharacterController))]
public abstract class Animal : MonoBehaviour
{
    // Name
    [SerializeField] public string Name;

    // Health
    [SerializeField] public float maxHP;
    [SerializeField] public float currentHP;

    [SerializeField] private float walkingSpeed;
    [SerializeField] private float runningSpeed;
    [SerializeField] private float rotationSpeed;

    [SerializeField] protected Behaviour behaviour;
    [SerializeField] protected Diet diet;
    [SerializeField] protected Ressource ressource;

    [SerializeField] protected float despawnTime;

    [SerializeField] public GameObject dinoHUD;
    [SerializeField] public Text dinoName;

    [SerializeField] private Animator animator;

    // Current Status
    protected bool isPlayerNearby = false;

    // Wander Variables
    protected bool isAlive = true;

    [SerializeField] protected bool isRotatingLeft = false;
    [SerializeField] protected bool isRotatingRight = false;
    [SerializeField] protected bool isWalking = false;

    // Despawn
    protected Stopwatch despawnTimer;

    void Awake()
    {
        dinoHUD.SetActive(false);
        dinoName.text = this.Name;
        ressource.enabled = false;
    }

    // TODO here we can do more logic if player is close to dino
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.tag);

        if (other.gameObject.tag == "Player")
        {
            isPlayerNearby = true;
            dinoHUD.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            isPlayerNearby = false;
            dinoHUD.SetActive(false);
        }
    }

    void Update()
    {
        if (!isAlive)
        {
            dinoName.text = "(DEAD)\n" + this.Name + "\nDisposing in:" + (despawnTime - despawnTimer.Elapsed.Seconds);

            if (despawnTimer.Elapsed.Seconds > despawnTime)
                Destroy(this.gameObject);

            return;
        }

        Move();
        RotateGUIRelativeToPlayer();

        animator.SetFloat("currentHealth", this.currentHP);
    }

    private void Move()
    {
        var isGrounded = this.GetComponent<CharacterController>().isGrounded;
        float verticalVelosity = isGrounded ? 0 : -1;

        var vec = new Vector3(0, verticalVelosity, 0);
        this.GetComponent<CharacterController>().Move(vec);
        
        StartCoroutine(Wander());

        if (isRotatingRight)
            transform.Rotate(transform.up * Time.deltaTime * rotationSpeed);
        
        if (isRotatingLeft)
            transform.Rotate(transform.up * Time.deltaTime * -rotationSpeed);

        if (isWalking)
        {
            transform.position += transform.forward * Time.deltaTime * walkingSpeed;
            animator.SetBool("isWandering", true);
        }
        else
        {
            animator.SetBool("isWandering", false);
        }
            
    }
    private void RotateGUIRelativeToPlayer()
    {
        var mainCamera = GameObject.FindGameObjectsWithTag("MainCamera").FirstOrDefault();

        System.Diagnostics.Debug.Assert(mainCamera != null, nameof(mainCamera) + " != null");

        var v = mainCamera.transform.position - transform.position;
        v.x = v.z = 0.0f;
        dinoHUD.transform.LookAt(mainCamera.transform.position - v);
        dinoHUD.transform.Rotate(0, 180, 0);
    }

    protected IEnumerator Wander()
    {
        int rotationTime = Random.Range(5, 10);
        int rotationWaitTime = Random.Range(5, 10);
        bool rotateDirection = Random.Range(0, 2) == 1;
        int walkWaitTime = Random.Range(1, 6);
        int walkTime = Random.Range(10, 20);

        yield return new WaitForSeconds(walkWaitTime);
        isWalking = true;
        //animator.SetBool("isWandering", isWalking);
        yield return new WaitForSeconds(walkTime);
        //animator.SetBool("isWandering", isWalking);
        isWalking = false;
        yield return new WaitForSeconds(rotationWaitTime);

        if (rotateDirection)
        {
            isRotatingRight = true;
            yield return new WaitForSeconds(rotationTime);
            isRotatingRight = false;
        }
        else
        {
            isRotatingLeft = true;
            yield return new WaitForSeconds(rotationTime);
            isRotatingLeft = false;
        }
    }

    public void TakeDamage(float damage, Transform transform)
    {
        if (!isAlive)
            return;

        this.currentHP = Mathf.Clamp(this.currentHP - damage, 0, this.maxHP);

        if (this.currentHP <= 0)
        {
            this.isAlive = false;
            despawnTimer = new Stopwatch();
            despawnTimer.Start();

            if (ressource != null)
            {
                ressource.enabled = true;
            }
        }
    }
}
