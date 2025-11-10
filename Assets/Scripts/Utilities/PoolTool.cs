using System.Collections.Generic;
using TXDCL.Character;
using TXDCL.XiuLian.FuShu;
using UnityEngine;
using UnityEngine.Pool;

public class PoolTool : Singleton<PoolTool>
{
    public GameObject FaShuDerivativePrefab;
    public ObjectPool<GameObject>  FaShuDerivativePool;

    protected override void Awake()
    {
        base.Awake();
        FaShuDerivativePool = new ObjectPool<GameObject>
        (createFunc: () => Instantiate(FaShuDerivativePrefab, transform),
            actionOnGet: (obj) => obj.SetActive(true),
            actionOnRelease: (obj) => obj.SetActive(false),
            actionOnDestroy: Destroy,
            collectionCheck: false,
            defaultCapacity: 10,
            maxSize: 40
        );
        PreFillFaShuDerivativePool(7);
    }
    private void PreFillFaShuDerivativePool(int count)
    {
        var preFillArray = new GameObject[count];
        for (var i = 0; i < count; i++)
        {
            preFillArray[i] = FaShuDerivativePool.Get();
        }
        foreach (var obj in preFillArray)
        {
            FaShuDerivativePool.Release(obj);
        }
    }
    
    public void GetFaShuDerivativeFromPool(FaShuData faShuData, Vector3 targetPosition, CharacterBase from, List<CharacterBase> targetCharacters)
    {
        if(faShuData == null) return;
        var FaShuDerivative = FaShuDerivativePool.Get();
        FaShuDerivative.GetComponent<FaShuDerivative>().Setup(faShuData, targetPosition, from, targetCharacters);
    }

    public void ReleaseFaShuDerivative(GameObject obj)
    {
        FaShuDerivativePool.Release(obj);
    }
}
