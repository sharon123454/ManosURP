using UnityEngine;

public class MouseWorld : MonoBehaviour
{
    private static MouseWorld instance;

    private string[] _activeLayerNames = { "MousePlane" };
    private LayerMask _activeMouseLayerMask;
    private bool _isActive = true;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);

        instance = this;
        _activeMouseLayerMask.value = LayerMask.GetMask(_activeLayerNames);
    }
    
    public static Vector3 GetPosition()
    {
        if (instance._isActive)
        {
            Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetPointerPosition());
            Physics.Raycast(ray, out RaycastHit rayCastHit, float.MaxValue, instance._activeMouseLayerMask);
            return rayCastHit.point;
        }
        return Vector3.zero;
    }

    private void ToggleMouseWorld() { _isActive = !_isActive; }

}