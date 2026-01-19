using UnityEngine;

public class MonsterSound : MonoBehaviour
{
    public static MonsterSound Instance { get; private set; }

    [SerializeField] private MonsterSoundData _soundData;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void Hit()
    {
        if (_soundData == null) return;
        PlaySoundInfo(_soundData.GetRandomHitSound());
    }

    public void PetAppear()
    {
        if (_soundData == null) return;
        PlaySoundInfo(_soundData.PetAppearSFX);
    }

    public void PetAttack()
    {
        if (_soundData == null) return;
        PlaySoundInfo(_soundData.PetAttackSFX);
    }

    private void PlaySoundInfo(MonsterSoundInfo info)
    {
        if (info.Clip == null) return;
        SoundManager.Instance.PlaySFX(info.Clip, info.StartTime, 1f);
    }
}
