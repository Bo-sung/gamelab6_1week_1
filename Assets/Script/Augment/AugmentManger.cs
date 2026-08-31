using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using NUnit.Framework;
using UnityEngine.DedicatedServer;

[DisallowMultipleComponent]
public class AugmentManger : MonoBehaviour
{
    public bool isActive = false;
    // public GameObject gameManager; 게임매니저 참조?
    public GameObject augmentPanel;
    public GameObject[] augmentButtons;
    public List<Augment> augmentList = new List<Augment>();
    public List<Augment> selectedAugments = new List<Augment>();
    public System.Action<Augment> OnSelectAugment;

    [ContextMenu("강제 증강 출현")]
    public void ActiveScreen()
    {

        Time.timeScale = 0.0f;
        selectedAugments = GetRandomAugment(augmentList, 3);
        for (int i = 0; i < augmentButtons.Length; i++)
        {
            augmentButtons[i].SetActive(true);
            augmentButtons[i].transform.Find("NameText").GetComponent<TextMeshProUGUI>().text = selectedAugments[i].augmentName;
            augmentButtons[i].transform.Find("DescriptionText").GetComponent<TextMeshProUGUI>().text = selectedAugments[i].description;
        }
        augmentPanel.SetActive(true);
    }

    public void DisableScreen()
    {
        Time.timeScale = 1.0f;
        for (int i = 0; i < augmentButtons.Length; i++)
        {
            augmentButtons[i].SetActive(false);
        }
        augmentPanel.SetActive(false);
    }


    public List<Augment> GetRandomAugment(List<Augment> augmentList, int count)
    {
        List<Augment> randomAugments = new List<Augment>();
        List<int> selectedIndex = new List<int>();
        while (randomAugments.Count < count)
        {
            int randomIndex = Random.Range(0, augmentList.Count);
            if (!selectedIndex.Contains(randomIndex))
            {
                selectedIndex.Add(randomIndex);
                randomAugments.Add(augmentList[randomIndex]);
            }
        }
        return randomAugments;
    }

    public void SelectAgument(int index)
    {
        Augment data = selectedAugments[index];
        OnSelectAugment?.Invoke(data);
        //증강실행코드는 따로 설정?

        DisableScreen();
    }

    //게임매니저의 업데이트 스텟으로 자동차 스탯관리할 것
}
