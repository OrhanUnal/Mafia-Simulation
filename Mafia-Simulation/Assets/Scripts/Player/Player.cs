using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField]
    float speed = 3.0f, maxInteractDistance = 10f, drag = 30f, maxSpeed = 100f, jumpSpeed = 1f, gravityForPlayer = -9.81f, sensivity = 1f;

    [SerializeField]
    AnimationCurve jumpCurve;

    [SerializeField]
    Transform cameraPickUpPosition, head;

    private CharacterController _characterController;
    private PlayerInput _playerInput;
    private float _pitch;
    private bool _canMove, _holdingSomething;
    private GameObject _grabbedObject;


    //inputs
    private InputAction _mouseInput, _interact, _walk, _jump, _pickUpDropDown;

    //variables for movement
    private Vector3 _move;
    private bool _inJump;
    private float _elapsedJumpTime;
    private float _velocityAtJumpStart;

    void Awake()
    {
        _pitch = 0;
        _elapsedJumpTime = 0;
        _inJump = false;
        _holdingSomething = false;
        _characterController = GetComponent<CharacterController>();
        _playerInput = GetComponent<PlayerInput>();
        //_interact = _playerInput.actions["Interract"];
        _walk = _playerInput.actions["Move"];
        _jump = _playerInput.actions["Jump"];
        _mouseInput = _playerInput.actions["Look"];
        //_pickUpDropDown = _playerInput.actions["PickUp DropDown"];
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        //_interact.performed += InteractSomething;
        //_pickUpDropDown.performed += PickUpAndDropDownSomething;
        _jump.started += JumpLogic;
        _jump.canceled += JumpLogic;
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
        if (_inJump && _elapsedJumpTime < 0.4f)
        {
            _elapsedJumpTime += Time.deltaTime;
            _move.y = Mathf.Lerp(_velocityAtJumpStart, jumpSpeed, jumpCurve.Evaluate(_elapsedJumpTime));
        }
        _move.x = _walk.ReadValue<Vector2>().x;
        _move.z = _walk.ReadValue<Vector2>().y;
        _move = transform.rotation * _move;
        Vector3 newVelocity = _characterController.velocity + _move * speed;
        Vector3 currentDrag = newVelocity.normalized * drag * Time.deltaTime;
        newVelocity = (newVelocity.magnitude > drag * Time.deltaTime) ? newVelocity - currentDrag : Vector3.zero;
        newVelocity = Vector3.ClampMagnitude(newVelocity, maxSpeed);
        _characterController.Move(newVelocity * Time.deltaTime);

    }

    private void GravityLogic()
    {
        if (!_characterController.isGrounded)
            _move.y += gravityForPlayer * Time.deltaTime;
        else _move.y = -0.2f;
    }

    private void JumpLogic(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            _inJump = false;
            _move.y = 0;
            _elapsedJumpTime = 0;
        }
        else
        {
            _velocityAtJumpStart = _characterController.velocity.y;
            _inJump = true;
        }
    }

    private void InteractSomething(InputAction.CallbackContext context)
    {
        IInteractable interactableObject;
        if (_holdingSomething)
        {
            interactableObject = _grabbedObject.GetComponent<IInteractable>();
            if (interactableObject != null)
                interactableObject.InteractLogic();
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

        Ray rayOfInteract = new Ray
        {
            origin = head.transform.position,
            direction = transform.forward,
        };
        RaycastHit hitInfo;
        Physics.Raycast(rayOfInteract, out hitInfo, maxInteractDistance);
        return hitInfo;
    }
    public bool CanMove { get => _canMove; set => _canMove = value; }
}

