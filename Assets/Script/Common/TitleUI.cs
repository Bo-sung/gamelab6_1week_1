using UnityEngine;
using UnityEngine.UI;

public class TitleUI : MonoBehaviour
{
    [SerializeField]
    private Button GameStartBtn;
    [SerializeField]
    private Button GameExitBtn;

    // 이벤트 영역
    public System.Action OnGameStartBtnClicked;
    public System.Action OnGameExitBtnClicked;


    public void AddEvent()
    {
        GameStartBtn.onClick.AddListener(() => OnGameStartBtnClicked?.Invoke());
        GameExitBtn.onClick.AddListener(() => OnGameExitBtnClicked?.Invoke());
    }
}

