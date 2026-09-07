using UnityEngine;
using static GameScene;


/// <summary>
/// 점수 및 콤보 계산할 담당자 인터페이스
/// </summary>
public interface IScoreHandler
{
    void ApplyScore(int score);
    int GetScore();
}


public abstract class EnemyBase : MonoBehaviour, IMapElement
{
    [SerializeField]
    protected Transform target;

    protected IScoreHandler scoreHandler;

    protected IMiniMap miniMapUI;

    [SerializeField]
    // 접근 속도
    protected float speed = 0.01f;
    [SerializeField]
    // 최대 접근 거리
    protected float maxDistance = 2;

    [SerializeField]
    Color markerColor = Color.green;

    [SerializeField]
    float markerSize = 1.0f;


    private void Awake()
    {
        Init();
    }

    private void Update()
    {
        Movement();
    }

    protected virtual void OnDisable()
    {
        
    }

    protected virtual void OnEnable()
    {

    }

    public void SetData(Transform target, IScoreHandler scoreHandler, IMiniMap miniMapUI)
    {
        this.scoreHandler = scoreHandler;
        this.target = target;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Arrow"))
        {
            OnHit(other);
        }
    }


    protected abstract void Init();
    protected abstract void OnHit(Collider other);
    protected abstract void Movement();
    protected abstract void OnDamage();

    protected virtual void ApplyScore(int score)
    {
        scoreHandler.ApplyScore(score);
    }

    public virtual void OnAttackPlayer()
    {
        this.gameObject.SetActive(false);
    }

    public Color GetMarkercolor() => markerColor;

    public float GetMarkerSize() => markerSize;

    Transform IMapElement.GetTransform()
    {
        return transform;
    }

    bool IMapElement.IsCenterTarget()
    {
        return false;
    }
}
