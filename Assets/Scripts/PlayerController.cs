using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
	private Material placeholderPossibleMaterial;
	[SerializeField]
	private Material placeholderImpossibleMaterial;

	public bool isPlacing = false;

	void Awake()
    {
		rg = GetComponent<Rigidbody>();
		inventory.SetActive(false);
    }

	void Update()
	{
		if(!inventory.activeSelf)
		{
			Move();
			HighlightSelection();

			if (Input.GetButtonDown("Jump"))
			{
				TryJump();
			}

			if(isPlacing) //Is in placing state
			{
				Place();
			}
		}
		else
		{
			//Stops player while in inventory
			rg.velocity = Vector3.zero;
		}

		if(Input.GetKeyDown(KeyCode.E))
		{
			OpenInventory();
		}
	}

	void Move()
	{
		float xinput = Input.GetAxis("Horizontal");
		float yinput = Input.GetAxis("Jump");
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

	void Place() //Fires when user is in placing state
	{
		var placeholder = selection.transform.GetChild(0).GetChild(0);
		var placeholderCollider = placeholder.GetComponent<ColliderState>();
		var selectionController = selection.GetComponent<SelectionController>();
		if(Input.GetAxis("Mouse ScrollWheel") > 0f)
		{
			selection.transform.Rotate(new Vector3(0,1,0), 90);
		}
		else if(Input.GetAxis("Mouse ScrollWheel") < 0f)
		{
			selection.transform.Rotate(new Vector3(0, 1, 0), -90);
		}

		if (!placeholderCollider.isSomethingWithin)
		{
			placeholder.GetComponent<MeshRenderer>().material = placeholderPossibleMaterial; //Set material to green
			if (Input.GetButtonDown("Fire1")) //Place item and leave placing state
			{
				selectionController.PlaceItem();
				ChangePlacing(false);
				placeholderCollider.isSomethingWithin = false;
			}
			else if (Input.GetButtonDown("Fire2")) //Abandon placing state
			{
				selectionController.DiscardPlacing();
				ChangePlacing(false);
				placeholderCollider.isSomethingWithin = false;
			}
		}
		else if (Input.GetButtonDown("Fire2")) //Abandon placing state
		{
			selectionController.DiscardPlacing();
			ChangePlacing(false);
			placeholderCollider.isSomethingWithin = false;
		}
		else
		{
			placeholder.GetComponent<MeshRenderer>().material = placeholderImpossibleMaterial; //Set material to red
		}
	}

	public void ChangePlacing(bool state) //Change placing state
	{
		isPlacing = state;
	}

	public void OpenInventory() //Open or hide inventory menu
	{
		if (inventory.activeSelf)
		{
			inventory.SetActive(false);
		}
		else
		{
			inventory.SetActive(true);
		}
	}
}
