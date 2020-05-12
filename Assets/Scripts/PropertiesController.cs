using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PropertiesController : MonoBehaviour
{
    public GameObject resistanceUIObject;
    public GameObject voltageUIObject;
    public GameObject capacityUIObject;
    public ColliderState placeholderCollider;

    private Slider resistanceSlider;
    private Slider voltageSlider;
    private Slider capacitySlider;
    // Start is called before the first frame update
    void Awake()
    {
        resistanceUIObject = transform.GetChild(0).gameObject;
        voltageUIObject = transform.GetChild(1).gameObject;
        capacityUIObject = transform.GetChild(2).gameObject;
    }

    private void Start()
    {
        resistanceSlider = resistanceUIObject.transform.GetChild(0).GetComponent<Slider>();
        resistanceSlider.onValueChanged.AddListener(delegate { onResistanceChanged(); });
        voltageSlider = voltageUIObject.transform.GetChild(0).GetComponent<Slider>();
        voltageSlider.onValueChanged.AddListener(delegate { onVoltageChanged(); });
        capacitySlider = capacityUIObject.transform.GetChild(0).GetComponent<Slider>();
        capacitySlider.onValueChanged.AddListener(delegate { onCapacityChanged(); });
    }

    private void onResistanceChanged()
    {
        placeholderCollider.objectIn.GetComponent<ItemClass>().resistance = resistanceSlider.value;
        resistanceUIObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = resistanceSlider.value.ToString();
    }

    private void onVoltageChanged()
    {
        placeholderCollider.objectIn.GetComponent<ItemClass>().voltage = voltageSlider.value;
        voltageUIObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = voltageSlider.value.ToString();
    }

    private void onCapacityChanged()
    {
        placeholderCollider.objectIn.GetComponent<ItemClass>().capacity = capacitySlider.value;
        capacityUIObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = capacitySlider.value.ToString();
    }
}
