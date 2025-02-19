using UnityEngine;
using TMPro;

public class PlayerMoneyManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI moneyText; // Reference to the TMP UI for money display

    // Static variable to persist money across scenes
    public static int savedTotalMoney = 0;

    private int totalMoney;

    void Start()
    {
        // Use savedTotalMoney if it's greater than 0; otherwise, initialize to 0
        totalMoney = savedTotalMoney;

        UpdateMoneyUI();
    }

public void AddMoney(int amount)
{
    totalMoney += amount;
    savedTotalMoney = totalMoney; // Save the updated money amount
    UpdateMoneyUI();
}

    private void UpdateMoneyUI()
    {
        if (moneyText != null)
        {
            moneyText.text = totalMoney.ToString(); // Display only the number
        }
    }

    private void OnDisable()
    {
        // Save the current total money when switching scenes
        savedTotalMoney = totalMoney;
    }

    public void ResetMoney()
    {
        totalMoney = 0;
        savedTotalMoney = 0;
        UpdateMoneyUI();
    }
}