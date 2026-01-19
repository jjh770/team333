using UnityEngine;

[CreateAssetMenu(menuName = "Game/Sound/FloraSoundData", fileName = "FloraSoundData")]
public class FloraSoundData : ScriptableObject
{
    [field: SerializeField]
    [Tooltip("속도업 사운드")]
    public SoundInfo SpeedUpSFX { get; private set; }

}
