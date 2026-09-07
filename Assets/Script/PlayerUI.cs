using UnityEngine;
using System.Collections.Generic;

public class PlayerUI : MonoBehaviour, GameScene.IHealthUI
{
    private float health;
    private float maxHealth;
    [SerializeField]
    private GameObject prefab;

    [SerializeField]
    Transform pool;

    [SerializeField] 
    private List<GameObject> playerHPObjects;

    public void SetData(int maxHealth)
    {
        this.maxHealth = maxHealth;
    }

    public void ApplyHealth(int health)
    {
        this.health += health;
        if(health > 0)
        {
            var obj = Instantiate(prefab, pool);
            obj.SetActive(true);
            playerHPObjects.Add(obj);
        }
        else
        {
            var temp = playerHPObjects[playerHPObjects.Count - 1];
            playerHPObjects.Remove(temp);
            Destroy(temp);
        }
    }

    public void SetHealth(int health)
    {
        this.health = health;
    }
}
