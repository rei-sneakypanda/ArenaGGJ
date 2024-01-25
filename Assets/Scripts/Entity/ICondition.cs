using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Serialization;
using UnityEngine;

public interface ICondition<in T>
{
    bool IsTrueFor(T args);
}

[Serializable]
public class AlwaysSameCondition : ICondition<EntitySO>
{
    [SerializeField] private bool _isAlwaysTrue;
    public bool IsTrueFor(EntitySO args)
    {
        return _isAlwaysTrue;
    }
}

[Serializable]
public class CheckEqualEntity : ICondition<EntitySO>
{
    [SerializeField]private EntitySO _selfEntity;
    [SerializeField]
    private bool _isEqual;

    public bool IsTrueFor(EntitySO args)
    {
        return _isEqual ? args == _selfEntity : args != _selfEntity;
    }
}


[Serializable]
public abstract class ConditionalValue<TArgs, TValue> : ICondition<TArgs>
{
    [SerializeField] private TValue _value;

    [SerializeField] private ICondition<TArgs> _condition;
    public TValue Value => _value;

    public bool IsTrueFor(TArgs args)
    {
        return _condition.IsTrueFor(args);
    }
}

[Serializable]
public class ConditionalCollection<TArgs> : ICondition<TArgs>
{
    [SerializeField] private ConditionOperator _conditionOperation;
    [OdinSerialize] private ICondition<TArgs>[] _conditionsCollections;
    public bool IsTrueFor(TArgs args)
    {
        return _conditionOperation.CheckCondition(
            _conditionsCollections.Select(condition => condition.IsTrueFor(args)));
    }
}

public enum ConditionOperator
{
    And,
    Or
}

public static class ConditionTypeExtensions
{
    public static bool CheckCondition(this ConditionOperator @operator, IEnumerable<bool> conditions)
    {
        switch (@operator)
        {
            case ConditionOperator.And:
                return conditions.All(boolValue => boolValue);
            case ConditionOperator.Or:
                return conditions.Any(boolValue => boolValue);
            default:
                throw new ArgumentOutOfRangeException(nameof(@operator), @operator, null);
        }
    }
}