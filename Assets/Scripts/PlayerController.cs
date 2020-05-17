using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
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

	public bool newConnection = false;

	private Transform placeholder;
	private ColliderState2 trigger;
	private ColliderState placeholderCollider;
	private SelectionController selectionController;
	private CircuitController circuitController;
	
	private Vector3 defaultOriginLocalPosition;

	public enum State { Default, Placing, Destroying, Connecting, SettingProperties };
	public State state = State.Default;
	void Awake()
    {
		rg = GetComponent<Rigidbody>();
		inventory.SetActive(false);
		placeholder = selection.transform.GetChild(0).GetChild(0);
		placeholderCollider = placeholder.GetComponent<ColliderState>();
		trigger = selection.transform.GetChild(1).GetComponent<ColliderState2>();
		selectionController = selection.GetComponent<SelectionController>();
		circuitController = circuit.GetComponent<CircuitController>();
		defaultOriginLocalPosition = selectionController.placeholderOrigin.transform.localPosition;
	}

	void Update()
	{
		if (!inventory.activeSelf)
		{
			if (!properties.activeSelf)
			{
				Move();
				HighlightSelection();
				if (Input.GetButtonDown("Jump"))
				{
					TryJump();
				}
				else if (Input.GetKeyDown(KeyCode.Q)) //set destroing state
				{
					ChangeState(State.Destroying);
				}
				else if (Input.GetKeyDown(KeyCode.C)) //set coneccting state
				{
					ChangeState(State.Connecting);
				}

				if (state == State.Placing) //Is in placing state
				{
					PlaceItem();
				}
				else if (state == State.Destroying) //Is in destroing state
				{
					DeleteItem();
				}
				else if (state == State.Connecting) //Is in connecting state
				{
					ConnectItem();
				}
				else if (Input.GetButtonDown("Fire2") && trigger.objectIn)
				{
					ChangeState(State.SettingProperties);
				}
				else if (Input.GetButtonDown("Fire1") && trigger.objectIn && trigger.objectIn.GetComponent<ItemClass>().itemType == ItemClass.Type.Switch)
				{
					trigger.objectIn.GetComponent<ItemClass>().SwitchOn();
				}
				
			}
			else
			{
				if (Input.GetButtonDown("Fire2")) //exit from setting properties state
				{
					ChangeState(State.Default);
				}
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
				ChangeState(State.Default);
				
			}
			else if (Input.GetButtonDown("Fire2")) //Abandon placing state
			{
				selectionController.DiscardPlacing();
				ChangeState(State.Default);
			}
		}
		else if (Input.GetButtonDown("Fire2")) //Abandon placing state
		{
			selectionController.DiscardPlacing();
			ChangeState(State.Default);
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
			Destroy(objectIn.gameObject); //Destroy object
		}
		else if (Input.GetButtonDown("Fire2")) //Abandon destroying state
		{
			ChangeState(State.Default);
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
				var element = objectIn.GetComponent<ItemClass>();
				circuitController.AddConnection(element, newConnection);
				newConnection = false;
			}
		}
		else
		{
			placeholder.GetComponent<MeshRenderer>().material = placeholderImpossibleMaterial; //Set material to red
		}

		if (Input.GetButtonDown("Fire2")) //Abandon connecting state
		{
			ChangeState(State.Default);
			circuitController.DiscardConnection();
		}
	}

	public void SetPropertiesOfItem(GameObject objectIn)
	{
		if (!objectIn) return;
		var type = objectIn ? objectIn.GetComponent<ItemClass>().itemType : ItemClass.Type.Other;
		if(type == ItemClass.Type.Capacitor || type == ItemClass.Type.Resistor || type == ItemClass.Type.Power_Supply)
		{
			OpenProperties(objectIn); //open properties menu
		}
		else
		{
			ChangeState(State.Default);
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

	public void ChangeState(State newState) //Change setting properties state
	{
		state = newState;
		
		switch (newState) //sets boolean to specified state
		{
			case State.Placing:
				placeholderCollider.isSomethingWithin = false;
				placeholder.GetComponent<MeshRenderer>().material = placeholderImpossibleMaterial ;
				break;

			case State.Destroying:
				placeholder.GetComponent<MeshRenderer>().material = placeholderImpossibleMaterial;
				break;

			case State.Connecting:
				placeholder.GetComponent<MeshRenderer>().material = placeholderImpossibleMaterial;
				break;

			case State.SettingProperties:
				SetPropertiesOfItem(trigger.objectIn);
				break;

			case State.Default:
				placeholder.GetComponent<MeshRenderer>().material = placeholderImpossibleMaterial;
				selectionController.SetPlaceholderSize(Vector3.one);
				OpenProperties(null);
				break;
		}

		selection.transform.GetChild(0).gameObject.SetActive(newState == State.Default || newState == State.SettingProperties ? false : true);
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
			state = State.Default;
			selectionController.SetPlaceholderSize(Vector3.one); //sets default settings
			inventory.SetActive(!inventory.activeSelf);
		}
		placeholderCollider.objectIn = null; //sets default settings
		placeholder.GetComponent<MeshRenderer>().material = placeholderPossibleMaterial; //sets default settings
	}

	public void OpenProperties(GameObject objectIn) //Open or hide items properties menu
	{
		
		if (!properties.activeSelf && objectIn != null)
		{
			var item = objectIn.GetComponent<ItemClass>(); 
			propertiesController.SetValue(item, PropertiesController.Property.Resistance, item.itemType == ItemClass.Type.Resistor ? true : false); //show and sets or hide property bar
			propertiesController.SetValue(item, PropertiesController.Property.Voltage, item.itemType == ItemClass.Type.Power_Supply ? true : false); //show and sets or hide property bar
			propertiesController.SetValue(item, PropertiesController.Property.Capacity, item.itemType == ItemClass.Type.Capacitor ? true : false); //show and sets or hide property bar
			properties.SetActive(true);
		}
		else
		{
			properties.SetActive(false);
		}
		
	}

}
