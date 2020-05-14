using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
	[SerializeField]
	private float moveSpeed;
	[SerializeField]
	private float jumpForce;
	[SerializeField]
	private float maxInteractionDistance;
	[SerializeField]
	private GameObject selection;
	[SerializeField]
	private Rigidbody rg;
	[SerializeField]
	private GameObject inventory;
	[SerializeField]
	private GameObject circuit;
	[SerializeField]
	private GameObject properties;
	[SerializeField]
	private Material placeholderPossibleMaterial;
	[SerializeField]
	private Material placeholderPossibleConnectionMaterial;
	[SerializeField]
	private Material placeholderPropertiesMaterial;
	[SerializeField]
	private Material placeholderImpossibleMaterial;
	[SerializeField]
	private PropertiesController propertiesController;

	[HideInInspector]
	public bool isPlacing = false;
	[HideInInspector]
	public bool isDestroying = false;
	[HideInInspector]
	public bool isConnecting = false;
	[HideInInspector]
	public bool isSettingProperties = false;

	private Transform placeholder;
	private ColliderState placeholderCollider;
	private SelectionController selectionController;
	private CircuitController circuitController;
	
	private Vector3 defaultOriginLocalPosition;

	public enum State {isPlacing, isDestroying, isConnecting, isSettingProperties };
	void Awake()
    {
		rg = GetComponent<Rigidbody>();
		inventory.SetActive(false);
		placeholder = selection.transform.GetChild(0).GetChild(0);
		placeholderCollider = placeholder.GetComponent<ColliderState>();
		selectionController = selection.GetComponent<SelectionController>();
		circuitController = circuit.GetComponent<CircuitController>();
		defaultOriginLocalPosition = selectionController.placeholderOrigin.transform.localPosition;
	}

	void Update()
	{
		if(!inventory.activeSelf)
		{
			if(!properties.activeSelf)
			{
				Move();
				HighlightSelection();

				if (Input.GetButtonDown("Jump"))
				{
					TryJump();
				}
				else if (Input.GetKeyDown(KeyCode.Q))
				{
					ChangeState(true, State.isDestroying);
				}
				else if (Input.GetKeyDown(KeyCode.C))
				{
					ChangeState(true, State.isConnecting);
				}
				else if (Input.GetKeyDown(KeyCode.R))
				{
					ChangeState(true, State.isSettingProperties);
				}

				if (isPlacing) //Is in placing state
				{
					PlaceItem();
				}
				else if (isDestroying) //Is in destroing state
				{
					DeleteItem();
				}
				else if (isConnecting) //Is in connecting state
				{
					ConnectItem();
				}
			}
			
			if (isSettingProperties) //Is in connecting state
			{
				SetPropertiesOfItem();
			}
		}
		else
		{
			//Stops player while in inventory
			rg.velocity = Vector3.zero;
		}

		if(Input.GetKeyDown(KeyCode.E) && !properties.activeSelf)
		{
			OpenInventory();
		}
	}

	void Move()
	{
		float xinput = Input.GetAxis("Horizontal");
		float zinput = Input.GetAxis("Vertical");

		Vector3 dir = new Vector3(xinput, 0, zinput) * moveSpeed;
		dir.y = rg.velocity.y;

		rg.velocity = transform.TransformDirection(dir);
	}

	void TryJump()
	{
		Ray ray1 = new Ray(transform.position + new Vector3(-0.5f, 0,  0.5f), Vector3.down);
		Ray ray2 = new Ray(transform.position + new Vector3( 0.5f, 0,  0.5f), Vector3.down);
		Ray ray3 = new Ray(transform.position + new Vector3(-0.5f, 0, -0.5f), Vector3.down);
		Ray ray4 = new Ray(transform.position + new Vector3( 0.5f, 0, -0.5f), Vector3.down);

		bool cast1 = Physics.Raycast(ray1, 1.2f);
		bool cast2 = Physics.Raycast(ray2, 1.2f);
		bool cast3 = Physics.Raycast(ray3, 1.2f);
		bool cast4 = Physics.Raycast(ray4, 1.2f);

		if (cast1 || cast2 || cast3 || cast4)
		{
			rg.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
		}
	}

	void HighlightSelection()
	{
        RaycastHit hit;
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, maxInteractionDistance))
        {
			// TODO: if intersects an item call item.Highlight
			selection.SetActive(true);
			SelectionController.instance.PointTo(hit.point);
        }
		else
		{
			selection.SetActive(false);
			SelectionController.instance.enabled = true;
		}
	}

	void PlaceItem() //Fires when user is in placing state
	{
		if(Input.GetAxis("Mouse ScrollWheel") > 0f)
		{
			selection.GetComponent<SelectionController>().RotateSelectionBy(90); //Rotate item
		}
		else if(Input.GetAxis("Mouse ScrollWheel") < 0f)
		{
			selection.GetComponent<SelectionController>().RotateSelectionBy(-90); //Rotate item
		}

		if (!placeholderCollider.isSomethingWithin)
		{
			placeholder.GetComponent<MeshRenderer>().material = placeholderPossibleMaterial; //Set material to green
			if (Input.GetButtonDown("Fire1") && selection.activeSelf) //Place item and leave placing state
			{
				selectionController.PlaceItem();
				ChangeState(false, State.isPlacing);
				
			}
			else if (Input.GetButtonDown("Fire2")) //Abandon placing state
			{
				selectionController.DiscardPlacing();
				ChangeState(false, State.isPlacing);
			}
		}
		else if (Input.GetButtonDown("Fire2")) //Abandon placing state
		{
			selectionController.DiscardPlacing();
			ChangeState(false, State.isPlacing);
		}
		else
		{
			placeholder.GetComponent<MeshRenderer>().material = placeholderImpossibleMaterial; //Set material to red
		}
	}

	public void DeleteItem() //Fires when user is in destroying state
	{
		var objectIn = placeholderCollider.objectIn;

		SetPlaceholderTransform(objectIn);

		if (Input.GetButtonDown("Fire1") && objectIn != null) //Place item and leave placing state
		{
			Destroy(placeholderCollider.objectIn.gameObject); //Destroy object
		}
		else if (Input.GetButtonDown("Fire2")) //Abandon destroying state
		{
			ChangeState(false, State.isDestroying);
		}

	}

	public void ConnectItem() //Fires when user is in connecting state
	{
		var objectIn = placeholderCollider.objectIn;

		SetPlaceholderTransform(objectIn);

		if (objectIn != null && placeholderCollider.isSomethingWithin)
		{
			placeholder.GetComponent<MeshRenderer>().material = placeholderPossibleConnectionMaterial; //Set material to blue
			if (Input.GetButtonDown("Fire1")) //Add item to circuit
			{
				circuitController.AddConnection(ref objectIn);
			}
		}
		else
		{
			placeholder.GetComponent<MeshRenderer>().material = placeholderImpossibleMaterial; //Set material to red
		}

		if (Input.GetButtonDown("Fire2")) //Abandon connecting state
		{
			ChangeState(false, State.isConnecting);
			circuitController.DiscardConnection();
		}
	}

	public void SetPropertiesOfItem()
	{
		var objectIn = placeholderCollider.objectIn;

		SetPlaceholderTransform(objectIn); //Set postition, scale and rotation of placeholder

		if (Input.GetButtonDown("Fire2") && !properties.activeSelf) //Abandon setting state
		{
			ChangeState(false, State.isSettingProperties); //exit from setting properties state
		}

		if (objectIn != null && placeholderCollider.isSomethingWithin ) 
		{
			placeholder.GetComponent<MeshRenderer>().material = placeholderPropertiesMaterial; //Set material to yellow
			if((Input.GetButtonDown("Fire1") && !properties.activeSelf) || (Input.GetButtonDown("Fire2") && properties.activeSelf))
			{
				OpenProperties(); //Open properties menu
			}
		}
		else
		{
			placeholder.GetComponent<MeshRenderer>().material = placeholderImpossibleMaterial; //Set material to red
		}
	}

	public void SetPlaceholderTransform(GameObject objectIn)
	{
		if (objectIn != null && placeholderCollider.isSelectionPlaneWithin)//If selection plane is in placeholder and placeholder is assigned to placed object 
		{
			selectionController.placeholderOrigin.transform.position = objectIn.GetComponent<ItemClass>().originTransform; //assign origin of selection placeholder to object origin point
		}
		else
		{
			selectionController.placeholderOrigin.transform.localPosition = defaultOriginLocalPosition; //unassign placeholder from object
		}
		selectionController.SetPlaceholderSize(objectIn != null ? objectIn.GetComponent<ItemClass>().size : Vector3.one); //If any object in, set placeholder size to this object size, otherwise 1
		selectionController.RotateSelectionTo(0, objectIn != null ? objectIn.gameObject.transform.rotation.eulerAngles.y : 0, 0); //Rotate placeholder to object direction
	}

	public void ChangeState(bool value, State state) //Change setting properties state
	{
		switch (state) //sets boolean to specified state
		{
			case State.isPlacing:
				isPlacing = value;
				placeholderCollider.isSomethingWithin = false;
				if (!value)
				{
					selectionController.SetPlaceholderSize(Vector3.one);
				}
				placeholder.GetComponent<MeshRenderer>().material = value ? placeholderImpossibleMaterial : placeholderPossibleMaterial;
				break;

			case State.isDestroying:
				isDestroying = value;
				placeholder.GetComponent<MeshRenderer>().material = placeholderImpossibleMaterial;
				break;

			case State.isConnecting:
				isConnecting = value;
				placeholder.GetComponent<MeshRenderer>().material = value ? placeholderImpossibleMaterial : placeholderPossibleConnectionMaterial;
				break;

			case State.isSettingProperties:
				isSettingProperties = value;
				placeholder.GetComponent<MeshRenderer>().material = value ? placeholderImpossibleMaterial : placeholderPropertiesMaterial;
				break;
		}
		selection.transform.GetChild(0).gameObject.SetActive(value);
	}

	public void OpenInventory() //Open or hide inventory menu
	{
		if (inventory.activeSelf)
		{
			inventory.SetActive(false);
		}
		else
		{
			selectionController.ToggleState(false); //Hide placeholder
			if (selection.transform.childCount > 2)
			{
				Destroy(selection.transform.GetChild(2).gameObject); //Destroy previous selection object
			}
			isPlacing = false; //sets default settings
			isDestroying = false; //sets default settings
			isConnecting = false; //sets default settings
			isSettingProperties = false; //sets default settings
			selectionController.SetPlaceholderSize(Vector3.one); //sets default settings
			inventory.SetActive(!inventory.activeSelf);
		}
		placeholderCollider.objectIn = null; //sets default settings
		placeholder.GetComponent<MeshRenderer>().material = placeholderPossibleMaterial; //sets default settings
	}

	public void OpenProperties() //Open or hide items properties menu
	{
		var item = placeholderCollider.objectIn.GetComponent<ItemClass>(); 
		
		if (!properties.activeSelf)
		{
			propertiesController.SetValue(item, PropertiesController.Property.Resistance, item.itemType == ItemClass.Type.Resistor ? true : false); //show and sets or hide property bar
			propertiesController.SetValue(item, PropertiesController.Property.Voltage, item.itemType == ItemClass.Type.Power_Supply ? true : false); //show and sets or hide property bar
			propertiesController.SetValue(item, PropertiesController.Property.Capacity, item.itemType == ItemClass.Type.Capacitor ? true : false); //show and sets or hide property bar
		}
		properties.SetActive(!properties.activeSelf);
	}

}
