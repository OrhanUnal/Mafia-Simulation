using UnityEngine;

public class TestObject : MonoBehaviour, IInteractable, IGraspable
{
    [SerializeField] float lerpSpeed = 10f;

    private bool _isOnFloor;
    private Transform _pickUpTransform;
    private Rigidbody _rigidBody;

    private void Awake()
    {
        _isOnFloor = true;
        _rigidBody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (!_isOnFloor)
        {
            Vector3 newPosition = Vector3.Lerp(transform.position, _pickUpTransform.position, lerpSpeed * Time.fixedDeltaTime);
            _rigidBody.MovePosition(newPosition);
        }
    }

    void IInteractable.InteractLogic()
    {
        Debug.Log("BENI OKUDULAR");
        EventManager.Instance.GetAvailableDialogueList();
    }

    bool IGraspable.PickOrDropObject(UnityEngine.Transform transform)
    {
        Debug.Log("WORKING");
        if (_isOnFloor)
        {
            Debug.Log("PICKEDUP");
            _pickUpTransform = transform;
            _isOnFloor = false;
            _rigidBody.useGravity = false;
            _rigidBody.detectCollisions = false;

            return true;
        }
        else
        {
            Debug.Log("Dropped Down");
            _pickUpTransform = null;
            _isOnFloor = true;
            _rigidBody.useGravity = true;
            _rigidBody.detectCollisions = true;

            return false;
        }

    }
}
