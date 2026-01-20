using UnityEngine;

public class GoodSansam : MonoBehaviour, IConsumeEffect
{
    [SerializeField] private MonsterSound _monsterSound;

    public void OnConsumed()
    {
        if (_monsterSound != null)
        {
            _monsterSound.EatSansam();
        }
    }
}
