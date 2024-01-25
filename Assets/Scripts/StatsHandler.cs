using System;
using System.Collections.Generic;
    public class Stat : IStat
    {
        public event Action OnReset;
        public event Action<float> OnValueChanged;

        public StatType StatType { get;private set; }
        private float _startingValue;
        private float _value;
        public Stat(StatType stat, float val)
        { StatType = stat;
            _startingValue = val;
            Value = StartValue;
        }
        public float StartValue => _startingValue;

        public float Value
        {
            get => _value;
            set
            {
                if (_value == value)
                    return;

                _value = value;
                OnValueChanged?.Invoke(Value);
            }
        }

        public void Reset()
        {
            OnReset?.Invoke();
            _value = _startingValue;
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
       Score = 1,
       HP = 2,
       RotationSpeed = 3,
        Mass = 4,
        Scale = 5,
        SpawnTime

    }
    public interface IStat
    {
        event Action OnReset;
        event Action<float> OnValueChanged;

        StatType StatType { get; }
        float StartValue { get; }
        float Value { get; set; }
        void Reset();
    }
    public interface IStatHandler
    {
        IStat this[StatType stat] { get; }
    }
