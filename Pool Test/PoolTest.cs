using DataKeeper.Attributes;
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
}
