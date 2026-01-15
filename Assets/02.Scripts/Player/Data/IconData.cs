using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InteractionIconEntry
{
    public IconType Type;
    public Sprite Icon;
}

[CreateAssetMenu(menuName = "Game/Player/IconData", fileName = "IconData")]
public class IconData : ScriptableObject
{
    [Header("Interaction Icons")]
    [SerializeField] private List<InteractionIconEntry> _iconEntries;

    private Dictionary<IconType, Sprite> _iconMap;

    public void Initialize()
    {
        _iconMap = new Dictionary<IconType, Sprite>();
        foreach (var entry in _iconEntries)
        {
            _iconMap[entry.Type] = entry.Icon;
        }
    }

    public Sprite GetIcon(IconType type)
    {
        if (_iconMap == null)
        {
            Initialize();
        }

        return _iconMap.TryGetValue(type, out var icon) ? icon : null;
    }
}
