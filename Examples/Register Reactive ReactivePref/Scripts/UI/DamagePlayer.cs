using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class DamagePlayer : MonoBehaviour
{
    [SerializeField] private int _value;

    private Button _button;
    private PlayerDataSO _playerData;

    private void Awake()
    {
        _playerData = DK.Data.Get<PlayerDataSO>();
        _button = GetComponent<Button>();
    }
    
    private void OnEnable()
    {
        _button.onClick.AddListener(Damage);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(Damage);
    }
    
    private void Damage()
    {
        if (_value > 0)
        {
            GameLog.Instance.Heal(_value.ToString());
        }
        else
        {
            GameLog.Instance.Damage(_value.ToString());
        }
        
        _playerData.PlayerHealth.Value += _value;
    }
}
