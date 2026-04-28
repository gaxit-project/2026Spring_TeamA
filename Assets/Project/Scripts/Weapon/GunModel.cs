using UnityEngine;

public class GunModel
{
    private readonly GunData _data;
    public int CurrentAmmo { get; private set; }
    public int ReserveAmmo { get; private set; }
    public bool IsReloading { get; set; }

    public GunModel(GunData data)
    {
        _data = data;
        CurrentAmmo = _data.maxAmmo;
        ReserveAmmo = _data.initAmmo;
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
        int ammoToLoad = Mathf.Min(_data.maxAmmo - CurrentAmmo, ReserveAmmo);
        if (ammoToLoad > ReserveAmmo)
        {
            ammoToLoad = ReserveAmmo;
        }
        CurrentAmmo += ammoToLoad;
        ReserveAmmo -= ammoToLoad;
        IsReloading = false;
    }

    public int Damage => _data.damage;
}
