using System;
using System.Collections.Generic;
using DataKeeper.Attributes;
using DataKeeper.Between;
using DataKeeper.Extensions;
using DataKeeper.Generic;
using DataKeeper.PoolSystem;
using UnityEngine;

public class PoolTest : MonoBehaviour
{
    public Pool<Collider> colliderPool = new Pool<Collider>();

    [field: SerializeReference, SerializeReferenceSelector] public ValueBase ValueBaseProp { get; private set; }
    [field: SerializeReference, SerializeReferenceSelector] public List<ValueBase> ValueBasePropList { get; private set; } = new List<ValueBase>();

    public Optional<int> TestOpt = new Optional<int>();
    
    [SerializeReference, SerializeReferenceSelector] public ValueBase valueBase;
    [SerializeReference, SerializeReferenceSelector] private ValueBase valueBase2;
    [SerializeReference, SerializeReferenceSelector] public List<ValueBase> valueBaseList = new List<ValueBase>();
    
    private void Awake()
    {
        colliderPool.Initialize();
    }
    
    [Button]
    private void TestGet()
    {
        var obj = colliderPool.Get();
        obj.transform.SetParent(transform);
    }
    
    [Button]
    private void TestRelease()
    {
        colliderPool.ReleaseAll();
    }

    [Button]
    public void TweenEaseInOutCircTest()
    {
        var target = colliderPool.Get();

        Tween
            .Move(target.transform)
            .From((Vector3.right * -5f) + target.transform.position)
            .To((Vector3.right * 5f) + target.transform.position)
            .Ease(EaseType.EaseInOutCirc)
            .Duration(2)
            .OnComplete(() => colliderPool.Release(target))
            .Start();
    }

    [Button]
    public void TweenTest()
    {
        for (int i = 0; i < 31; i++)
        {
            var target = colliderPool.Get();
            target.transform.position = Vector3.up * i;
            
            Tween
                .Move(target.transform)
                .From((Vector3.right * -5f) + target.transform.position)
                .To((Vector3.right * 5f) + target.transform.position)
                .Ease((EaseType)i)
                .Duration(2)
                .OnComplete(() => colliderPool.Release(target))
                .Start();
        }
    }
    
    [Button]
    public void TweenPlayerTest()
    {
        for (int i = 0; i < 31; i++)
        {
            var target = colliderPool.Get();
            target.transform.position = Vector3.up * i;
            
            var tween = new TweenPlayer(target.transform.position.x - 5, target.transform.position.x + 5, 2f)
                .OnValue(value => target.transform.SetPosX(value))
                .Ease((EaseType)i);
                
            tween.OnComplete(() =>
                {
                    colliderPool.Release(target);
                })
                .Start();
        }
    }

    [Button]
    public void Str()
    {
        valueBase = new ValueString();
    }
    
    [Button]
    public void Boo()
    {
        valueBase = new ValueBool();
    }
    
    [Button]
    public void Vec()
    {
        valueBase = new ValueVec();
    }
}

[Serializable]
public abstract class ValueBase
{
    public int valueInt;
    [SerializeField] private float valueFloat;
}

[Serializable]
public class ValueString : ValueBase
{
    public string valueString;
}

[Serializable]
public class ValueBool : ValueBase
{
    [SerializeField] private bool valueBool;
}

[Serializable]
public class ValueVec : ValueBase
{
    [field: SerializeField] public Vector3 valueVec { get; private set; }
}

[Serializable]
public class ValueSecondOrder : ValueVec
{
    [field: SerializeField, Range(0f, 1f)] public float valueRange { get; private set; }
}

[Serializable] public class ValueSecondOrder2 : ValueSecondOrder { }
[Serializable] public class ValueSecondOrder3 : ValueSecondOrder { }
[Serializable] public class ValueSecondOrder4 : ValueSecondOrder { }
[Serializable] public class ValueSecondOrder5 : ValueSecondOrder { }
[Serializable] public class ValueSecondOrder6 : ValueSecondOrder { }
[Serializable] public class ValueSecondOrder7 : ValueSecondOrder { }
[Serializable] public class ValueSecondOrder8 : ValueSecondOrder { }
[Serializable] public class ValueSecondOrder9 : ValueSecondOrder { }
[Serializable] public class ValueSecondOrder10 : ValueSecondOrder { }
[Serializable] public class ValueSecondOrder11 : ValueSecondOrder { }
[Serializable] public class ValueSecondOrder12 : ValueSecondOrder { }
[Serializable] public class ValueSecondOrder13 : ValueSecondOrder { }
[Serializable] public class ValueSecondOrder14 : ValueSecondOrder { }
[Serializable] public class ValueSecondOrder15 : ValueSecondOrder { }
[Serializable] public class ValueSecondOrder16 : ValueSecondOrder { }
[Serializable] public class ValueSecondOrder17 : ValueSecondOrder { }
[Serializable] public class ValueSecondOrder18 : ValueSecondOrder { }
[Serializable] public class ValueSecondOrder19 : ValueSecondOrder { }
[Serializable] public class ValueSecondOrder20 : ValueSecondOrder { }
