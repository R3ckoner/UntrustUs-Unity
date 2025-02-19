using UnityEngine;
using TMPro;

public class PlayerMoneyManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI moneyText; // Reference to the TMP UI for money display

    private static int totalMoney = 0; // Money persists across scenes

    private void Start()
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
        if (moneyText != null)
        {
            moneyText.text = totalMoney.ToString(); // Display only the number
        }
    }

    public void ResetMoney()
    {
        totalMoney = 0;
        UpdateMoneyUI();
    }

    public int GetTotalMoney() => totalMoney;
}
