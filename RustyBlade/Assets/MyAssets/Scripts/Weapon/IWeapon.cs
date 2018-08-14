using System.Collections.Generic;

public interface IWeapon
{
    int CurrentDamage { get; set; }
    void PerformAction(int _dmg);
    bool GetIsInUse();

    List<BaseStat> Stats {get; set;}
}