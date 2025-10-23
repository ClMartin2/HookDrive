using UnityEngine;

[CreateAssetMenu(fileName = "CarData", menuName = "Scriptable Objects/CarData")]
public class CarData : ScriptableObject
{
    public GameObject carModel;
    public Vector3 offsetPositionCar;
    public Sprite imageShopCar;
}
