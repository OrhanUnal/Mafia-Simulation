using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField]
    float acceleration = 70f, maxInteractDistance = 10f, drag = 25f, maxSpeed = 4, gravityForPlayer = -9.81f, sensivity = 1f, radiusOfRay = 0.3f;

    [SerializeField]
    Transform cameraPickUpPosition, head;

    private CharacterController _characterController;
    private PlayerInput _playerInput;
    private Vector3 _velocity;
    private float _pitch;
    private bool _canMove, _holdingSomething;
    private GameObject _grabbedObject;


    //inputs
    private InputAction _mouseInput, _interact, _walk, _pickUpDropDown;

    //variables for movement
    private Vector3 _move;

    void Awake()
    {
        _velocity = Vector3.zero;
        _pitch = 0;
        _holdingSomething = false;
        _characterController = GetComponent<CharacterController>();
        _playerInput = GetComponent<PlayerInput>();
        _interact = _playerInput.actions["Interact"];
        _walk = _playerInput.actions["Move"];
        _mouseInput = _playerInput.actions["Look"];
        _pickUpDropDown = _playerInput.actions["PickUp DropDown"];
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _interact.performed += InteractSomething;
        _pickUpDropDown.performed += PickUpAndDropDownSomething;
    }
    void FixedUpdate()
    {
        RotationLogic();
        MovementLogic();
        GravityLogic();
    }

    private void RotationLogic()
    {
        float mouseXRotation = _mouseInput.ReadValue<Vector2>().x * sensivity;
        _pitch -= _mouseInput.ReadValue<Vector2>().y * sensivity;
        _pitch = Mathf.Clamp(_pitch, -90f, 90f);
        transform.Rotate(0, mouseXRotation, 0);
        head.localRotation = Quaternion.Euler(_pitch, 0, 0);
    }

    private void MovementLogic()
    {
        Vector2 input = _walk.ReadValue<Vector2>();
        Vector3 moveDirection = new Vector3(input.x, 0, input.y);
        moveDirection = transform.TransformDirection(moveDirection);

        _velocity.x += moveDirection.x * acceleration * Time.deltaTime;
        _velocity.z += moveDirection.z * acceleration * Time.deltaTime;

        Vector3 horizontalVelocity = new Vector3(_velocity.x, 0, _velocity.z);
        Vector3 dragForce = horizontalVelocity.normalized * drag * Time.deltaTime;
        horizontalVelocity = (horizontalVelocity.magnitude - dragForce.magnitude > 0) ? horizontalVelocity - dragForce : Vector3.zero;
        horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, maxSpeed);

        _velocity.x = horizontalVelocity.x;
        _velocity.z = horizontalVelocity.z;
        _characterController.Move(_velocity * Time.deltaTime);
    }

    private void GravityLogic()
    {
        if (_characterController.isGrounded)
            _velocity.y = -2f;
        else _velocity.y += gravityForPlayer * Time.deltaTime;
    }

    private void InteractSomething(InputAction.CallbackContext context)
    {
        IInteractable interactableObject;
        if (_holdingSomething)
        {
            interactableObject = _grabbedObject.GetComponent<IInteractable>();
            if (interactableObject != null)
            {
                interactableObject.InteractLogic();
                return;
            }
        }
        else
        {
            RaycastHit interactInfo = ShootRayFromCamera();
            if (interactInfo.collider != null)
            {
                interactableObject = interactInfo.collider.GetComponent<IInteractable>();
                if (interactableObject != null)
                    interactableObject.InteractLogic();
            }
        }
    }

    private void PickUpAndDropDownSomething(InputAction.CallbackContext context)
    {
        if (_holdingSomething)
        {
            _holdingSomething = false;
            IGraspable grasped = _grabbedObject.GetComponent<IGraspable>();
            grasped.PickOrDropObject(cameraPickUpPosition);
            _grabbedObject = null;
            return;
        }
        RaycastHit rayInfo = ShootRayFromCamera();
        if (rayInfo.collider != null)
        {
            IGraspable grasped = rayInfo.collider.GetComponent<IGraspable>();
            if (grasped != null)
            {
                _grabbedObject = rayInfo.collider.gameObject;
                _holdingSomething = grasped.PickOrDropObject(cameraPickUpPosition);
            }
        }
    }

    private RaycastHit ShootRayFromCamera()
    {
        Debug.DrawRay(head.position, head.forward * maxInteractDistance, Color.red, 0.3f);
        Ray rayOfInteract = new Ray
        {
            origin = head.position,
            direction = head.forward,
        };
        RaycastHit hitInfo;
        Physics.SphereCast(rayOfInteract, radiusOfRay, out hitInfo, maxInteractDistance);
        return hitInfo;
    }
    public bool CanMove { get => _canMove; set => _canMove = value; }
}

