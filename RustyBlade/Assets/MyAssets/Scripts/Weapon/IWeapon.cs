using System.Collections.Generic;

public interface IWeapon
{
    int CurrentDamage { get; set; }
    void PerformAction(int _dmg);
    bool GetIsInUse();
	void ToggleWeaponDisplay(bool _displayStatus);
    List<BaseStat> Stats {get; set;}
}