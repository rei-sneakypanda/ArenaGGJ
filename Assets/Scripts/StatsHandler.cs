using System;
using System.Collections.Generic;
using UniRx;

public class Stat : IStat
{
    public StatType StatType { get; private set; }
    private float _startingValue;
    private ReactiveProperty<float> _value = new();
    public IReadOnlyReactiveProperty<float> StatValue => _value;

    public Stat(StatType stat, float val)
    {
        StatType = stat;
        _startingValue = val;
        _value.Value = StartValue;
    }

    public float StartValue => _startingValue;

    public void SetValue(float val)
    {
        _value.Value = val;
    }

    public void Reset()
    {
        _value.Value = _startingValue;
    }
}

public class StatHandler : IStatHandler
    {
        private readonly Dictionary<StatType, IStat> _statDictionary;
        public StatHandler(params StatTemplate[] statEditor)
        {
            _statDictionary = new Dictionary<StatType, IStat>(statEditor.Length);
            foreach (var stat in statEditor)
            {
                _statDictionary.Add(stat.StatType, new Stat(stat.StatType, stat.StartingValue));
            }
        }

        public IStat this[StatType stat]
        {
            get
            {
                if (_statDictionary.TryGetValue(stat, out IStat istat))
                    return istat;
                throw new Exception($"Stat Handler: stat was not found!\nName: {stat}");
            }
        }
    }

    public enum StatType
    {
        MovementSpeed = 0,
        HP = 2,
        RotationSpeed = 3,
        Mass = 4,
        Scale = 5,
        SpawnTime,
        ScoreOverTime,
        DamageOverTime,
        DamageInterval,
        ScoreInterval,
        
    }
    public interface IStat
    {
        StatType StatType { get; }
        float StartValue { get; }
        void SetValue(float val);
        IReadOnlyReactiveProperty<float> StatValue { get; }
        void Reset();
    }
    public interface IStatHandler
    {
        IStat this[StatType stat] { get; }
    }
