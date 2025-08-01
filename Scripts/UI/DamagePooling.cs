using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePooling : MonoBehaviour
{
    [Header("��Ʈ ������")]
    [SerializeField] private GameObject _prefabDmg;                 // ������ ������Ʈ

    private Queue<GameObject> _poolDmg = new Queue<GameObject>();   // ť Ǯ��

    private Canvas _canvas;
    private int _poolCount;                                         // Ǯ ����

    private void Start()
    {
        InitClass();
    }

    private void InitClass()
    {
        _canvas = GetComponent<Canvas>();
        _poolCount = 20;

        for (int i = 0; i < _poolCount; i++)                        // Ǯ�� �� ��ŭ �ְ� �����
        {
            GameObject instance = Instantiate(_prefabDmg, this.transform);

            DamageFontEffect fontEffect = instance.GetComponent<DamageFontEffect>();                // rect�־��ֱ�
            fontEffect.InitClass(_canvas);

            _poolDmg.Enqueue(instance);
        }
    }

    public GameObject ReturnObjectPopPool()
    {
        GameObject usePoolObject = _poolDmg.Peek();
        _poolDmg.Dequeue();
        return usePoolObject;
    }

    public void SetPushPool(GameObject gameObject) => _poolDmg.Enqueue(gameObject);
}
