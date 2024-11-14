using UnityEngine;
using TMPro;

public class PlayerMoneyManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI moneyText; // Reference to the TMP UI for money display

    private int totalMoney = 0;

    void Start()
    {
        UpdateMoneyUI();
    }

    public void AddMoney(int amount)
    {
        totalMoney += amount;
        UpdateMoneyUI();
    }

    private void UpdateMoneyUI()
    {
        moneyText.text = totalMoney.ToString(); // Display only the number
    }
}
