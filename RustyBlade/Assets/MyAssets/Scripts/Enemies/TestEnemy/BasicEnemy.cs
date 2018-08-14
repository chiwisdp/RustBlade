using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : MonoBehaviour, IEnemy
{
	public float maxHealth, power, toughness;
	private float currentHealth;
	private void Start() {
		currentHealth = maxHealth;
	}
    public void PreformAttack()
    {
        throw new System.NotImplementedException();
    }

    public void TakeDamage(int amount)
    {
		currentHealth -= amount;
		if(currentHealth <=0)
		{
			Kill();
		}
    }
	void Kill(){
		Destroy(gameObject);
	}
}
