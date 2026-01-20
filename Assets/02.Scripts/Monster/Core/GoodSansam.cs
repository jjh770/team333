using UnityEngine;

public class GoodSansam : MonoBehaviour
{
    [SerializeField] private MonsterSound _monsterSound;

    public void PlayEatSound()
    {
        if (_monsterSound != null)
        {
            _monsterSound.EatSansam();
        }
    }
}
