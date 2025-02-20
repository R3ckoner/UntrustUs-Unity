using UnityEngine;

public class Flashlight : MonoBehaviour
{
    [Header("Flashlight Settings")]
    public Light flashlight;  // Assign a Spotlight in the Inspector
    public KeyCode toggleKey = KeyCode.F; // Key to toggle flashlight
    public bool startsOn = false; // Should the flashlight start ON?
    
    [Header("Battery Settings (Optional)")]
    public bool useBattery = false; // Enable/Disable battery usage
    public float maxBattery = 100f; // Maximum battery life
    private float currentBattery;
    public float batteryDrainRate = 5f; // Battery drain per second

    private bool isOn;

    void Start()
    {
        isOn = startsOn;
        flashlight.enabled = isOn;
        currentBattery = maxBattery;
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleFlashlight();
        }

        if (useBattery && isOn)
        {
            DrainBattery();
        }
    }

    void ToggleFlashlight()
    {
        if (useBattery && currentBattery <= 0)
        {
            Debug.Log("🔋 Flashlight battery is dead!");
            return;
        }

        isOn = !isOn;
        flashlight.enabled = isOn;
        Debug.Log($"🔦 Flashlight {(isOn ? "ON" : "OFF")}");
    }

    void DrainBattery()
    {
        if (currentBattery > 0)
        {
            currentBattery -= batteryDrainRate * Time.deltaTime;
            if (currentBattery <= 0)
            {
                currentBattery = 0;
                flashlight.enabled = false;
                isOn = false;
                Debug.Log("⚠️ Flashlight turned off due to dead battery!");
            }
        }
    }

    public void RechargeBattery(float amount)
    {
        currentBattery = Mathf.Clamp(currentBattery + amount, 0, maxBattery);
        Debug.Log($"🔋 Battery recharged to {currentBattery}%");
    }
}
