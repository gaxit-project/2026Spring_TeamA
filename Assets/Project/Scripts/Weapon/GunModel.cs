using UnityEngine;

public class GunModel
{
    private readonly GunData _data;
    public int CurrentAmmo { get; private set; }
    public bool IsReloading { get; set; }

    public GunModel(GunData data)
    {
        _data = data;
        CurrentAmmo = data.maxAmmo;
    }

    public bool CanShoot()
    {
        return CurrentAmmo > 0 && !IsReloading;
    }

    public void ConsumeAmmo()
    {
        if (CurrentAmmo > 0) CurrentAmmo--;
    }

    public void Reload()
    {
        CurrentAmmo = _data.maxAmmo;
        IsReloading = false;
    }

    public int Damage => _data.damage;
}
