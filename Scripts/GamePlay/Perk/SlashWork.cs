using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashWork : MonoBehaviour
{
    public ParticleSystem _slash;

    private SlashPerk _slashPerk;

    public void GetPerk(SlashPerk perk) => _slashPerk = perk;

    private const int _slashDamage = 3;         // 3의 고정 데미지

    public void SlashPerkWork()
    {
        if (_slash.IsAlive()) return;

        SlashRotate();
        _slash.Play();
    }

    void SlashRotate()
    {
        //int[] rotateNum = new int[] { 0, 45, -45, 90 };
        //int randRotateZ = rotateNum[Random.Range(0, rotateNum.Length)];

        this.transform.rotation = Quaternion.LookRotation(Vector3.forward);

        this.transform.localRotation = Quaternion.Euler(this.transform.rotation.x, this.transform.rotation.y, 90);
    }

    private void OnParticleCollision(GameObject other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        int dmg = _slashDamage * _slashPerk._curLevel;
        enemy.PerkDamage(dmg);
    }
}
