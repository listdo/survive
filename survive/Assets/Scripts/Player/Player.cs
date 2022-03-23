using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    // Singelton
    private static Player _instance;
    public static Player Instance { get { return _instance; } }

    #region PlayerComponents
    [Header("PlayerComponents")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterController characterController;

    [SerializeField] public Inventory inventory;

    [SerializeField] private UIBarScript healthBar;
    [SerializeField] private UIBarScript saturationBar;
    [SerializeField] private UIBarScript hydrationBar;
    #endregion

    #region Equipment

    [SerializeField][Tooltip("Helmet Slot")] public EquipmentSlot helmet;
    [SerializeField][Tooltip("Breast Slot")] public EquipmentSlot breast;
    [SerializeField][Tooltip("Legs Slot")] public EquipmentSlot legs;
    [SerializeField][Tooltip("Boots Slot")] public EquipmentSlot boots;
    [SerializeField][Tooltip("Gauntlets Slot")] public EquipmentSlot gauntlets;
    [SerializeField][Tooltip("Tool Slot")] public EquipmentSlot mainHand;

    #endregion

    #region Player Movement Stats
    [Header("Movement Stats")]
    [SerializeField]
    private float walkingSpeed = 7.5f;

    [SerializeField]
    private float runningSpeed = 11.5f;

    [SerializeField]
    private float swimmingSpeed = 3.5f;

    [SerializeField]
    private float jumpSpeed = 8.0f;

    [SerializeField]
    private float attackDamage = 5.0f;

    private float gravity = 20.0f;
    private float pickupDistance = 3.5f;

    private float lookSpeed = 2.0f;
    private float lookXLimit = 45.0f;

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    #endregion

    #region PlayerProperties

    [Header("Status Properties")]
    // Health
    [SerializeField] public float maxHP = 100F;
    [SerializeField] public float currentHP = 100F;

    // Saturation
    [SerializeField] public float maxSaturation = 100F;
    [SerializeField] public float currentSaturation = 100F;

    private bool isStarving = false;

    // Hydration
    [SerializeField] public float maxHydration = 100F;
    [SerializeField] public float currentHydration = 100F;

    private bool isDehydrated = false;

    // Consumption
    [SerializeField] public float saturationDecreaseRate = 0.01F;
    [SerializeField] public float hydrationDecreaseRate = 0.005F;
    [SerializeField] public float healthDecreaseRate = 0.5F;

    // Regeneration
    [SerializeField] public float regenerationHp = 0.1F;
    #endregion

    #region AnimationProperties
    private bool canMove = true;
    private bool isSwimming = false;
    private bool isDiving = false;
    #endregion

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    void Start()
    {
        healthBar.SetMaxValue((int) this.maxHP);
        saturationBar.SetMaxValue((int) this.maxSaturation);
        hydrationBar.SetMaxValue((int) this.maxHydration);

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Move();

        if (Input.GetKeyDown("e") && !inventory.IsOpen)
            this.Interact();

        if (Input.GetKeyDown("i"))
            this.inventory.ToggleInventory();

        if (Input.GetMouseButtonDown(0) && !inventory.IsOpen)
            this.Hit();

        if (canMove)
            this.RotateCamera();

        Starve();
        Dehydrate();
        Die();

        healthBar.SetValue((int) this.currentHP);
        saturationBar.SetValue((int) this.currentSaturation);
        hydrationBar.SetValue((int) this.currentHydration);
    }

    void FixedUpdate()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Water>())
        {
            this.isSwimming = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<Water>())
        {
            this.isSwimming = false;
        }
    }

    private void Move()
    {
        // if the CharacterController is not instantiated this scene does not own the player
        if (this.GetComponent<CharacterController>() == null)
            return;

        // We are grounded, so recalculate move direction based on axes
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        // Press Left Shift to run
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = Input.GetAxis("Vertical");
        float curSpeedY = Input.GetAxis("Horizontal");

        if(animator != null)
            animator.SetFloat("vertical", curSpeedX);

        if (isRunning && !isSwimming && !isDiving)
        {
            curSpeedX *= runningSpeed;
            curSpeedY *= runningSpeed;
        } else if (isSwimming)
        {
            curSpeedX *= swimmingSpeed;
            curSpeedY *= swimmingSpeed;
        } else
        {
            curSpeedX *= walkingSpeed;
            curSpeedY *= walkingSpeed;
        }

        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetKeyDown("space") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (!characterController.isGrounded)
            moveDirection.y -= gravity * Time.deltaTime;

        // Move the controller
        if (!moveDirection.Equals(this.transform.position))
        {
            characterController.Move(moveDirection * Time.deltaTime);
        }
    }

    private void Starve()
    {
        if (this.currentSaturation > 0)
            this.currentSaturation -= this.saturationDecreaseRate * Time.deltaTime;
        else
            isStarving = true;
    }

    private void Dehydrate()
    {
        if (this.currentHydration > 0)
            this.currentHydration -= this.hydrationDecreaseRate * Time.deltaTime;
        else
            Die();
    }

    private void Die()
    {
        if (this.currentHP > 0 && (isDehydrated || isStarving))
            this.currentHP -= this.healthDecreaseRate * Time.deltaTime;

        if (this.currentHP <= 0)
            Time.timeScale = 0;
    }

    private void Interact()
    {
        var currentLookingRaycastHit = GetObjectLookingAt();

        PickUpIfItem(currentLookingRaycastHit);
        DrinkIfWater(currentLookingRaycastHit);
    }

    private void DrinkIfWater(RaycastHit hit)
    {
        Component interactable = hit.collider?.gameObject.GetComponent<Water>();

        if (interactable != null)
        {
            animator.SetTrigger("isPicking");
            this.currentHydration = this.maxHydration;
        }
    }

    private void PickUpIfItem(RaycastHit hit)
    {
        var gameObject = hit.collider?.gameObject;
        InventoryItem item = ItemDatabase.Instance.Items.Find((i) => gameObject != null && gameObject.GetComponent<Item>()?.Id == i?.ItemId);

        if (item != null)
        {
            animator.SetTrigger("isPicking");
            inventory.AddItem(item);

            Destroy(gameObject);
        }
    }

    public RaycastHit GetObjectLookingAt()
    {
        RaycastHit hit;
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out hit, pickupDistance);

        return hit;
    }

    private void Hit()
    {
        RaycastHit hit;
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

        animator.SetTrigger("isHitting");

        if (Physics.Raycast(ray, out hit, pickupDistance))
        {
            Animal animal = hit.collider.gameObject.GetComponent<Animal>();

            if (animal != null)
            {
                animal.TakeDamage(this.attackDamage, this.transform);
                Debug.Log("DEAL DAMAGE TO " + animal.tag);
            }

            Ressource ressource = hit.collider.gameObject.GetComponent<Ressource>();

            if (ressource != null)
            {
                inventory.AddItem(ressource.TakeDamage(this.attackDamage, this.mainHand.inventoryItem));
            }
        }
    }

    private void RotateCamera()
    {
        if (!this.inventory.IsOpen)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }
}