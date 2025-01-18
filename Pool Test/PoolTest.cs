using System.Linq;
using DataKeeper.Attributes;
using DataKeeper.Between;
using DataKeeper.Extensions;
using DataKeeper.PoolSystem;
using UnityEngine;

public class PoolTest : MonoBehaviour
{
    public Pool<Collider> colliderPool = new Pool<Collider>();

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
}
