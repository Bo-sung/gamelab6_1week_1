using TMPro;
using UnityEngine;
using static GameScene;

public class UIWave : MonoBehaviour, IWaveUI
{
    [SerializeField]
    TextMeshProUGUI waveText;
    [SerializeField]
    TextMeshProUGUI waveTimeText;
    [SerializeField]
    float waveInterval = 20f;

    int maxWave = 10;
    WaveInfo curWave;


    float waveTimer = 0f;
    public void SetData(int maxWave)
    {
        this.maxWave = maxWave;
    }

    public void UpdateData(WaveInfo info)
    {
        curWave = info;
        waveText.text = $"{curWave.wave}/{maxWave}";
        waveTimer = waveInterval + (info.enemySpawnTable.Length * 0.5f);
    }

    void Start()
    {
        waveTimer = waveInterval;
    }

    void Update()
    {
        waveTimer -= Time.deltaTime;

        waveTimer = Mathf.Max(0, waveTimer);
        waveTimeText.text = $"RemainTime : {waveTimer}";
    }
}
