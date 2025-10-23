using UnityEngine;
using UnityEngine.UI;

public class CardShop : MonoBehaviour
{
    [SerializeField] private CarData carData;
    [SerializeField] private Image image;
    [SerializeField] private BtnSelectShop btnSelectShop;

    private void Awake()
    {
        btnSelectShop.carData = carData;
        UpdateImage();
    }

    private void UpdateImage()
    {
        image.sprite = carData.imageShopCar;
    }
}
