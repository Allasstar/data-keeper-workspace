using DataKeeper.Attributes;
using DataKeeper.Signals;
using UnityEngine;
using Random = UnityEngine.Random;

public class TestEvent : MonoBehaviour
{
    public SignalChanelInt signalChanelInt;

    private void Start()
    {
        signalChanelInt.AddListener(ListenEvent);
    }

    private void ListenEvent(int value)
    {
        Debug.Log($"Event:  value: {value}");
    }

    [Button, ContextMenu("invoke")]
    private void Invoke()
    {
        signalChanelInt?.Invoke(Random.Range(0, 101));
    }
}
