using UnityEngine;

public class CameraObstructionFader : MonoBehaviour
{
    public Material red;
    public Material transRed;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            other.GetComponent<Renderer>().material = transRed;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            other.GetComponent<Renderer>().material = red;
        }
    }
}
