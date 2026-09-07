using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour, GameScene.IHealthUI
{
    [SerializeField]
    private GameObject prefab;

    [SerializeField]
    Transform pool;

    [SerializeField]
    private List<GameObject> playerHPObjects = new List<GameObject>();

    GameScene.IHealthUI.Presenterbase presenter;

    public void Initialize(GameScene.IHealthUI.Presenterbase _presenter)
    {
        this.presenter = _presenter;
    }

    public void Refresh()
    {
        int healthRemain = Mathf.Clamp(presenter.GetHealth(), 0, presenter.GetMaxHealth());
        while (playerHPObjects.Count < healthRemain)
        {
            AddHPObj();
        }

        for (int i = 0; i < playerHPObjects.Count; i++)
        {
            playerHPObjects[i].SetActive(i < healthRemain);
        }
    }
    void AddHPObj()
    {
        var obj = Instantiate(prefab, pool);
        obj.SetActive(true);
        playerHPObjects.Add(obj);
    }

    void EnableHPObj(int index)
    {
        if (playerHPObjects.Count - 1 > index)
            return; // 인덱스 오버

        playerHPObjects[index].SetActive(false);
    }

    void DisableHPObj(int index)
    {
        if (playerHPObjects.Count - 1 > index)
            return; // 인덱스 오버

        playerHPObjects[index].SetActive(false);
    }

    void DestroyHPObj(int index)
    {
        if (playerHPObjects.Count - 1 > index)
            return; // 인덱스 오버

        playerHPObjects.RemoveAt(index);
    }
}
