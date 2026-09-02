using UnityEngine;


/// <summary>
/// 점수 및 콤보 계산할 담당자 인터페이스
/// </summary>
public interface IScoreHandler
{
    void ApplyScore(int score);
}
public abstract class EnemyBase : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    private IScoreHandler scorUI;

    [SerializeField]
    // 접근 속도
    protected float speed = 0.1f;
    [SerializeField]
    // 최대 접근 거리
    protected float maxDistance = 2;


    private void Awake()
    {
        Init();
    }

    private void Update()
    {
        Movement();
    }

    public void SetData(Transform target, IScoreHandler scoreHandler)
    {
        scorUI = scoreHandler;
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
        scorUI?.ApplyScore(score);
    }
}
