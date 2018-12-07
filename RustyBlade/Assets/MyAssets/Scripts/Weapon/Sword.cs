using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour, IWeapon {
	private PlayerAnimationController _animator;
	public int base_dmg;
	public Renderer _weapon;

    public List<BaseStat> Stats {get; set;}
	public CharacterStats CharacterStats { get; set; }
	public int CurrentDamage { get; set; }
	MeleeWeaponTrail _trail;
	PlayerEffectsController _effectsController;
	Collider m_Collider;
    private void Awake()
	{
		_animator=FindObjectOfType<PlayerAnimationController>();
		_effectsController = FindObjectOfType<PlayerEffectsController>();
		 m_Collider = GetComponent<Collider>();
		 m_Collider.enabled = false;
	}
    public void PerformAction(int _dmg)
    {
		CurrentDamage = _dmg;
		_animator.SwordAttack();
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
		//_animator.SetTrigger("wall_hit");
	}
	public void EnableHitBox(){
		m_Collider.enabled = true;		
	}
	public void DisableHitBox(){
		m_Collider.enabled = false;
	}
    public bool GetIsInUse()
    {
        return (_animator.getIsSwordAttack());
    }
	void ShowEffect()
	{
		_effectsController.DisplaySwordSwingEffect();
	}

	public void ToggleWeaponDisplay(bool _displayStatus){
		_weapon.enabled = _displayStatus;
	}
}
