using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatsController : MonoBehaviour {

	// Use this for initialization public CharacterStats characterStats;
	public CharacterStats characterStats;
	public int currentHealth;
    public int maxHealth;
    public PlayerLevel PlayerLevel { get; set; }

    void Start()
    {
        PlayerLevel = GetComponent<PlayerLevel>();
        this.currentHealth = this.maxHealth;
        characterStats = new CharacterStats(10, 10, 100,10);
        Debug.Log("characterStats: " +characterStats);
        getPlayerMaxEnergy();
    }

    public float getPlayerHealthPercentage(){
        return ((float)this.currentHealth/(float)this.maxHealth);
    }
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
            Die();
        //UIEventHandler.HealthChanged(this.currentHealth, this.maxHealth);
    }

    private void Die()
    {
        Debug.Log("Player dead. Reset health.");
        this.currentHealth = this.maxHealth;
        //UIEventHandler.HealthChanged(this.currentHealth, this.maxHealth);
    }
     public void getPlayerMaxEnergy(){
        Debug.Log( "getPlayerMaxEnergy: "+characterStats.GetStat(BaseStat.BaseStatType.Energy).GetCalculatedStatValue());
    }
}
