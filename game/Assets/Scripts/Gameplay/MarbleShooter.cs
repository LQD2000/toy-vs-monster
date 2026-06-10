using UnityEngine;

[RequireComponent(typeof(AttackComponent))]
public class MarbleShooter : Defender
{
    private AttackComponent _attackComponent;

    protected override void Awake()
    {
        base.Awake();
        _attackComponent = GetComponent<AttackComponent>();
    }

    public override void Initialize(DefenderData data, int row, int col)
    {
        base.Initialize(data, row, col);

        if (_attackComponent == null)
        {
            _attackComponent = GetComponent<AttackComponent>();
        }
        _attackComponent.Initialize(data.AttackPower, data.AttackSpeed, data.Range, row);
    }
}
