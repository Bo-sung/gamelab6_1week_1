using UnityEngine;
using System.Collections.Generic;

public class PlayerUI : MonoBehaviour
{
    private Player player;
    [SerializeField] private List<GameObject> playerHPObjects;

    public void Initialize(Player player)
    {
        this.player = player;
        player.OnPlayerHit += UpdatePlayerHP;
    }

    public void UpdatePlayerHP()
    {
        GameObject disableHp = playerHPObjects[player.hp];
        disableHp.SetActive(false);
    }
}
