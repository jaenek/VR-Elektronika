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

    void Awake()
    {
		rg = GetComponent<Rigidbody>();
    }

	void Update()
	{
		Move();
		HighlightSelection();

		if (Input.GetButtonDown("Jump"))
		{
			TryJump();
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
}
