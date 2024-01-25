using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TagGroup", menuName = "ArenaGGJ/Tags/New Tag Group")]
public class TagGroupSO : TagSO
{
    [SerializeField]
    private List<TagSO> _tags;
    public IReadOnlyList<TagSO> Tags => _tags;
    
    public static bool operator ==(TagGroupSO obj1, TagSO obj2)
    {
        if (ReferenceEquals(obj1, obj2)) 
            return true;
        if (ReferenceEquals(obj1, null))
            return false;
        
        if (ReferenceEquals(obj2, null))
            return false;
        
        var result = false;

        foreach (var tag in obj1.Tags)
        {
            result |= obj2 == tag;
            if (result)
            {
                break;
            }
        }

        return result;
    }
    
    public static bool operator !=(TagGroupSO obj1, TagSO obj2)
    {
        if (ReferenceEquals(obj1, obj2)) 
            return false;
        if (ReferenceEquals(obj1, null)) 
            return true;
        if (ReferenceEquals(obj2, null))
            return true;

        var result = true;

        foreach (var tag in obj1.Tags)
        {
            result &= obj2 == tag;
            if (!result)
            {
                break;
            }
        }

        return result;
    }
}