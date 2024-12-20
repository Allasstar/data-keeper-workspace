using System;
using System.Collections.Generic;
using UnityEngine;

namespace Riten.Windinator
{
    public class GameObjectPool<T> where T : Component
    {
        Queue<T> m_instances;

        List<T> m_active;

        GameObject m_prefab;

        public GameObjectPool(GameObject prefab)
        {
            m_instances = new Queue<T>();
            m_active = new List<T>();
            m_prefab = prefab;
        }

        /// <summary>
        /// Mark window free for reuse later.
        /// </summary>
        /// <param name="instance">Window instance</param>
        public void Free(T instance)
        {
            m_active.Remove(instance);
            m_instances.Enqueue(instance);
            Deactivate(instance);
        }

        public T Allocate(Transform parent)
        {
            if (m_instances.Count > 0)
            {
                var result = m_instances.Dequeue();

                if (result.transform.parent != parent)
                    result.transform.SetParent(parent, false);

                Activate(result);
                m_active.Add(result);
                return result;
            }
            else
            {
                var i = GameObject.Instantiate(m_prefab, parent).GetComponent<T>();
                m_active.Add(i);
                return i;
            }
        }

        public C Allocate<C>(Transform parent) where C : Component
        {
            return Allocate(parent).GetComponentInChildren<C>();
        }

        public T PreAllocate(Transform parent)
        {
            var instance = GameObject.Instantiate(m_prefab, parent).GetComponent<T>();
            Free(instance);
            return instance;
        }

        public void Activate(T instance)
        {
            instance.gameObject.SetActive(true);
        }

        public void Deactivate(T instance)
        {
            instance.gameObject.SetActive(false);
        }

        internal void DestroyAllFree()
        {
            foreach (var go in m_instances)
                GameObject.Destroy(go.gameObject);

            m_instances.Clear();
        }

        internal void DestroyAll()
        {
            DestroyAllFree();

            foreach (var go in m_active)
                GameObject.Destroy(go.gameObject);

            m_active.Clear();
        }
    }

    public class GenericPool<T>
    {
        Queue<T> m_instances;

        List<T> m_active;

        Func<T> m_contructor;

        public GenericPool(Func<T> contructor)
        {
            m_contructor = contructor;
            m_instances = new Queue<T>();
            m_active = new List<T>();
        }

        /// <summary>
        /// Mark window free for reuse later.
        /// </summary>
        /// <param name="instance">Window instance</param>
        public void Free(T instance)
        {
            m_active.Remove(instance);
            m_instances.Enqueue(instance);
        }

        public T Allocate()
        {
            if (m_instances.Count > 0)
            {
                var result = m_instances.Dequeue();
                Activate(result);
                m_active.Add(result);
                return result;
            }
            else
            {
                var i = m_contructor();
                m_active.Add(i);
                return i;
            }
        }

        public T PreAllocate(Transform parent)
        {
            var instance = m_contructor();
            Free(instance);
            return instance;
        }

        public void Activate(T instance)
        { }

        public void Deactivate(T instance)
        { }

        internal void DestroyAllFree()
        {
            m_instances.Clear();
        }

        internal void DestroyAll()
        {
            DestroyAllFree();
            m_active.Clear();
        }
    }
}
