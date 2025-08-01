using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SlashPerk", menuName = "Scriptable Object/Perks/SlashPerk")]

public class SlashPerk : PerkEffect
{

    public int _curLevel = 0;                            // �нú� ��� ���� ����

    public const int _maxLevel = 5;                      // �нú� ���  �ִ� ����

    public GameObject _slashPrefab;                      // ������ Prefab
    public override void ApplyEffect()
    {
        if (_curLevel == _maxLevel) return;

        _curLevel += 1;                     // �������ϱ�

        GameObject gameObj = Instantiate(_slashPrefab, PlayerManager.Instance.GetPlayer.transform);

        gameObj.transform.position = new Vector3(gameObj.transform.position.x, 1f, gameObj.transform.position.z);

        SlashWork work = gameObj.GetComponent<SlashWork>();

        GameManager.Instance.GetPM.GetSlash.Add(work);

        if (work != null) work.GetPerk(this);
    }

    public override string StringToLevel()
    {
        return $"{_curLevel} / {_maxLevel}";
    }

    public override string StringToMainToolTip()
    {
        return $"���� �� �˰��� �߰��� �����ϴ�. {_curLevel * 3}�� ���� �������� �ݴϴ�. {_curLevel} �� ��ŭ Ÿ���� �����մϴ�.";
    }

    public override string StringToPickToolTip()
    {
        int level = (_curLevel == _maxLevel) ? _maxLevel : _curLevel + 1;
        return $"���� �� �˰��� �߰��� �����ϴ�. {level * 3}�� ���� �������� �ݴϴ�. ���� ���� ���� ��ŭ Ÿ���� �����մϴ�.";
    }
    public override void InitPerk()
    {
        _curLevel = 0;
    }
}
