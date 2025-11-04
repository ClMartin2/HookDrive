using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProposeSkinScreen : CustomScreen
{
    [SerializeField] private Transform giftEffect;
    [SerializeField] private float speedRotationGIftEffect;
    [SerializeField] private Image imageCar;
    [SerializeField] private UnLockCarProposition unLockCarProposition;

    private List<CarData> carDataRemaining = new();

    private void Start()
    {
        Hide();
    }

    public override void Show()
    {
        base.Show();
        ChooseCarToUnlock();
    }

    private void ChooseCarToUnlock()
    {
        if (carDataRemaining.Count == 0)
        {
            RefreshRemainingCars();
        }

        int randomIndex = Random.Range(0, carDataRemaining.Count);
        CarData choosedCar = carDataRemaining[randomIndex];
        carDataRemaining.RemoveAt(randomIndex);

        imageCar.sprite = choosedCar.imageShopCar;
        unLockCarProposition.carData = choosedCar;
    }

    private void RefreshRemainingCars()
    {
        carDataRemaining = GameManager.Instance.allCars;
    }

    private void Update()
    {
        giftEffect.rotation = giftEffect.rotation * Quaternion.AngleAxis(speedRotationGIftEffect * Time.deltaTime,Vector3.forward);       
    }
}
