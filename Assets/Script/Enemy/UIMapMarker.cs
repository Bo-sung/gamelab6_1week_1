using UnityEngine;
using UnityEngine.UI;
using static GameScene;

public class UIMapMarker : MonoBehaviour
{
    [SerializeField]
    Image image;
    private Color color;
    private float size;
    private Transform target;

    public void SetData(IMapElement data)
    {
        this.color = data.GetMarkercolor();
        this.size = data.GetMarkerSize();
        this.target = data.GetTransform();
        image.color = color;
        
    }
}
