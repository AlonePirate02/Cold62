using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
 
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float walkSpeed = 6f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float stamina = 100f;
    [SerializeField] private float staminaRecovery = 10f;
    [SerializeField] private float staminaStunDuration = 3f;
    [SerializeField] private float jumpPower = 7f;
    [SerializeField] private float gravity = 10f;


    [SerializeField] private float lookSpeed = 2f;
    [SerializeField] private float lookXLimit = 70f;


    public Image staminaBarRight;
    public Image staminaBarLeft;
 
 
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;
 
    public bool canMove = true;
    public bool canRun = true;
    public bool fillStamina = true;
    
    
    CharacterController characterController;
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked; // Doing this because script starts disabled
        Cursor.visible = false;
    }

    RaycastHit upHit;

    void Update()
    {
        // MAIN MOVEMENT CALCULATIONS
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
 
        bool isCrouching = Input.GetKey(KeyCode.LeftControl);
        bool isRunning = Input.GetKey(KeyCode.LeftShift) && !isCrouching && canRun;

        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);
 
        if (Input.GetButton("Jump") && canMove && characterController.isGrounded && !isCrouching)
        {
            moveDirection.y = jumpPower;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        //RUNNING & STAMINA
        if (isRunning)
        {
            stamina -= Time.deltaTime * 30f;
            if (stamina <= 0f)
            {
                stamina = 0f;
                isRunning = false;

                StartCoroutine(DisableRunningUntilStamina());
            }
        }
        else
        {
            if (fillStamina)
            {
                stamina += Time.deltaTime * staminaRecovery;
                if (stamina > 100f) stamina = 100f;
            }
        }

        staminaBarRight.fillAmount = stamina / 100f;
        staminaBarLeft.fillAmount = stamina / 100f;

        // CROUCHING & CEILING CHECK
        if (isCrouching)
        {
            characterController.height = 1f;
        }
        else
        {
            // Ceiling check
            bool ceilingBlocked = Physics.Raycast(
                transform.position,
                Vector3.up,
                out upHit,
                1.1f // Crouch'tan kalkarken gereken mesafe
            );

            if (!ceilingBlocked)
            {
                characterController.height = 2f;
            }
        }

        // GRAVITY & MOVEMENT
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
 
        characterController.Move(moveDirection * Time.deltaTime);
 
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }

    }

    IEnumerator DisableRunningUntilStamina()
    {
        canRun = false;
        fillStamina = false;
        yield return new WaitForSeconds(staminaStunDuration);
        fillStamina = true;
        yield return new WaitUntil(() => stamina >= 20f);
        canRun = true;
    }
}
