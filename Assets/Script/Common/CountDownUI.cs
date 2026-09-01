using JetBrains.Annotations;
using System.Collections;
using TMPro;
using UnityEngine;

public class CountDownUI : MonoBehaviour
{
    private TextMeshProUGUI CountDownText;
    public int time = 3;
    public System.Action OnCountDownEnd;
    void Start()
    {
        CountDownText = GetComponent<TextMeshProUGUI>();
        StartCoroutine(CountDown(time));
    }


    private IEnumerator CountDown(int time)
    {
        for (int i = 0; i < time; i++)
        {
            CountDownText.text = (time - i).ToString();
            CountDownText.fontSize = 400;
            for (int j = 0; j < 100; j++)
            {
                CountDownText.fontSize = 400 - j;
                yield return new WaitForSeconds(0.005f);
            }
            yield return new WaitForSeconds(0.5f);
        }
        CountDownText.text = "";
        OnCountDownEnd?.Invoke();
    }
}
