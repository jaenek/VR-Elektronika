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
    public enum Property { Resistance, Voltage, Capacity };
    // Start is called before the first frame update

    private void Start()
    {
        resistanceUIObject = transform.GetChild(0).gameObject;
        voltageUIObject = transform.GetChild(1).gameObject;
        capacityUIObject = transform.GetChild(2).gameObject;

        resistanceSlider = resistanceUIObject.transform.GetChild(0).GetComponent<Slider>();
        resistanceSlider.onValueChanged.AddListener(delegate { onValueChanged(Property.Resistance); });
        voltageSlider = voltageUIObject.transform.GetChild(0).GetComponent<Slider>();
        voltageSlider.onValueChanged.AddListener(delegate { onValueChanged(Property.Voltage); });
        capacitySlider = capacityUIObject.transform.GetChild(0).GetComponent<Slider>();
        capacitySlider.onValueChanged.AddListener(delegate { onValueChanged(Property.Capacity); });
    }


    private void onValueChanged(Property propertyName)
    {
        var itemClass = placeholderCollider.objectIn.GetComponent<ItemClass>();
        switch (propertyName)
        {
            case Property.Resistance:
                itemClass.resistance = resistanceSlider.value;
                resistanceUIObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = resistanceSlider.value.ToString();
                break;

            case Property.Voltage:
                itemClass.voltage = voltageSlider.value;
                voltageUIObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = voltageSlider.value.ToString();
                break;
            case Property.Capacity:
                itemClass.capacity = capacitySlider.value;
                capacityUIObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = capacitySlider.value.ToString();
                break;

        }
    }

    public void SetValue(ItemClass objectIn, Property property, bool isActive)
    {
        switch (property)
        {
            case Property.Resistance:
                resistanceUIObject.gameObject.SetActive(isActive);
                if(isActive)
                {
                    resistanceUIObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = objectIn.resistance.ToString();
                    resistanceUIObject.transform.GetChild(0).GetComponent<Slider>().value = objectIn.resistance;
                }
                break;

            case Property.Voltage:
                voltageUIObject.gameObject.SetActive(isActive);
                if (isActive)
                {
                    voltageUIObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = objectIn.voltage.ToString();
                    voltageUIObject.transform.GetChild(0).GetComponent<Slider>().value = objectIn.voltage;
                }
                break;

            case Property.Capacity:
                capacityUIObject.gameObject.SetActive(isActive);
                if (isActive)
                {
                    capacityUIObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = objectIn.capacity.ToString();
                    capacityUIObject.transform.GetChild(0).GetComponent<Slider>().value = objectIn.capacity;
                }
                break;
        }
    }
}
