using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePooling : MonoBehaviour
{
    [Header("폰트 프리펩")]
    [SerializeField] private GameObject _prefabDmg;                 // 프리펩 오브젝트

    private Queue<GameObject> _poolDmg = new Queue<GameObject>();   // 큐 풀링

    private Canvas _canvas;
    private int _poolCount;                                         // 풀 갯수

    private void Start()
    {
        InitClass();
    }

    private void InitClass()
    {
        _canvas = GetComponent<Canvas>();
        _poolCount = 20;

        for (int i = 0; i < _poolCount; i++)                        // 풀링 수 만큼 넣고 만들기
        {
            GameObject instance = Instantiate(_prefabDmg, this.transform);

            DamageFontEffect fontEffect = instance.GetComponent<DamageFontEffect>();                // rect넣어주기
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
