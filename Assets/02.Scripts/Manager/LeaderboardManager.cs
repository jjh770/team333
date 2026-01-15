using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class LeaderboardEntry
{
    public string Name;
    public float Time;

    public LeaderboardEntry(string name, float time)
    {
        Name = name;
        Time = time;
    }
}

[Serializable]
public class LeaderboardData
{
    public List<LeaderboardEntry> Entries = new List<LeaderboardEntry>();
}

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance { get; private set; }

    private const string SaveKey = "Leaderboard";
    private const int MaxEntries = 50;

    private LeaderboardData _data;

    public IReadOnlyList<LeaderboardEntry> Entries => _data.Entries;

    public event Action OnLeaderboardUpdated;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        Load();
    }

    public void AddEntry(string name, float time)
    {
        var entry = new LeaderboardEntry(name, time);
        _data.Entries.Add(entry);

        _data.Entries = _data.Entries
            .OrderBy(e => e.Time)
            .Take(MaxEntries)
            .ToList();

        Save();
        OnLeaderboardUpdated?.Invoke();
    }

    public int GetRank(float time)
    {
        int rank = 1;
        foreach (var entry in _data.Entries)
        {
            if (time > entry.Time)
            {
                rank++;
            }
        }
        return rank;
    }

    private void Save()
    {
        string json = JsonUtility.ToJson(_data);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
    }

    private void Load()
    {
        if (PlayerPrefs.HasKey(SaveKey))
        {
            string json = PlayerPrefs.GetString(SaveKey);
            _data = JsonUtility.FromJson<LeaderboardData>(json);
        }
        else
        {
            _data = new LeaderboardData();
        }
    }

    public void ClearAll()
    {
        _data = new LeaderboardData();
        Save();
        OnLeaderboardUpdated?.Invoke();
    }
}
