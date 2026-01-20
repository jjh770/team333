using UnityEngine;

public class SkyboxSettings : MonoBehaviour
{
    [SerializeField] private Material[] _skyboxMaterials;
    [SerializeField] private Vector3 _rotationSpeed = new Vector3(0, 1, 0);
    private float _rotationAngle;
    private int _randomNum;
    private float _rotationMagnitude;

    private void Awake()
    {
        _randomNum = UnityEngine.Random.Range(0, _skyboxMaterials.Length);
        _rotationMagnitude = _rotationSpeed.magnitude;
    }
    private void Start()
    {
        RenderSettings.skybox = _skyboxMaterials[_randomNum];
    }
    private void Update()
    {
        _rotationAngle += Time.deltaTime * _rotationMagnitude;
        RenderSettings.skybox.SetFloat("_Rotation", _rotationAngle);
    }
}