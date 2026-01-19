using UnityEngine;

[CreateAssetMenu(menuName = "Game/Sound/FloraSoundData", fileName = "FloraSoundData")]
public class FloraSoundData : ScriptableObject
{
    [field: SerializeField]
    [Tooltip("속도업 사운드")]
    public SoundInfo SpeedUpSFX { get; private set; }

    [field: SerializeField]
    [Tooltip("상호작용 사운드")]
    public SoundInfo[] InteractionSounds { get; private set; }

    public SoundInfo GetRandomInteractionSound()
    {
        if (InteractionSounds == null || InteractionSounds.Length == 0) return default;
        return InteractionSounds[Random.Range(0, InteractionSounds.Length)];
    }
}
