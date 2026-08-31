using UnityEngine;

public class TrackEdge : MonoBehaviour
{
    [SerializeField]
    private BoxCollider trigger;

    public System.Action OnTrigger;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Car"))
        {
            OnTrigger?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Car"))
        {
            OnTrigger?.Invoke();
        }
    }
}
