using System.Collections.Generic;

public interface IWeapon
{
    int CurrentDamage { get; set; }
    void PerformAction(int _dmg);
    bool GetIsInUse();
	void ToggleWeaponDisplay(bool _displayStatus);
    void EnableHitBox();
    void DisableHitBox();
    List<BaseStat> Stats {get; set;}
}