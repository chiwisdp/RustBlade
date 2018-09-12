using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour, IWeapon {
	private Animator _animator;
	public int base_dmg;

    public List<BaseStat> Stats {get; set;}
	public CharacterStats CharacterStats { get; set; }
	public int CurrentDamage { get; set; }

    private void Awake()
	{
		_animator=GetComponent<Animator>();
	}
    public void PerformAction(int _dmg)
    {
		CurrentDamage = _dmg;
		if(_animator.GetCurrentAnimatorStateInfo(0).IsName("Sword_Idle"))
		{
			_animator.SetTrigger("base_attack");
		}
	}
	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Enviornment"){
			SwordRecoil();
		}
		if(other.tag == "Enemy")
		{
			//SwordRecoil();
			other.GetComponent<IEnemy>().TakeDamage(CurrentDamage);
		}
	}
	void SwordRecoil(){
		_animator.SetTrigger("wall_hit");
	}

    public bool GetIsInUse()
    {
        return (_animator.GetCurrentAnimatorStateInfo(0).IsName("Sword_Swing"));
    }
}
