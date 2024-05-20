using UnityEngine;
using System.Collections;
 
public class ExtendedFlycam : MonoBehaviour
{
 
	/*
	FEATURES
		WASD/Arrows:    Movement
		          Q:    Climb
		          E:    Drop
                      Shift:    Move faster
                    Control:    Move slower
                        End:    Toggle cursor locking to screen (you can also press Ctrl+P to toggle play mode on and off).
	*/
 
	public float cameraSensitivity = 90;
	public float climbSpeed = 20;
	public float normalMoveSpeed = 20;
	public float slowMoveFactor = 0.25f;
	public float fastMoveFactor = 5;
 
	private float rotationX = 0.0f;
	private float rotationY = 0.0f;
 
	void Start ()
	{
		//Screen.lockCursor = true;
	}
 
	void Update ()
	{
		if(Input.GetMouseButton(1)){
			rotationX += Input.GetAxis("Mouse X") * cameraSensitivity * Time.deltaTime;
			rotationY += Input.GetAxis("Mouse Y") * cameraSensitivity * Time.deltaTime;
			rotationY = Mathf.Clamp (rotationY, -90, 90);
		}
		
		transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
		transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);

		bool shift_down = false;
 
	 	if (Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift))
	 	{
			transform.position += transform.forward * (normalMoveSpeed * fastMoveFactor) * Input.GetAxis("Vertical") * Time.deltaTime;
			transform.position += transform.right * (normalMoveSpeed * fastMoveFactor) * Input.GetAxis("Horizontal") * Time.deltaTime;
			shift_down = true;
	 	}
	 	else if (Input.GetKey (KeyCode.LeftControl) || Input.GetKey (KeyCode.RightControl))
	 	{
			transform.position += transform.forward * (normalMoveSpeed * slowMoveFactor) * Input.GetAxis("Vertical") * Time.deltaTime;
			transform.position += transform.right * (normalMoveSpeed * slowMoveFactor) * Input.GetAxis("Horizontal") * Time.deltaTime;
	 	}
	 	else
	 	{
	 		transform.position += transform.forward * normalMoveSpeed * Input.GetAxis("Vertical") * Time.deltaTime;
			transform.position += transform.right * normalMoveSpeed * Input.GetAxis("Horizontal") * Time.deltaTime;
	 	}
 
		float climbSpeed_final = climbSpeed;
		if(shift_down){
			climbSpeed_final *= fastMoveFactor;
		}
		if (Input.GetKey (KeyCode.Q)) {transform.position -= (transform.up * climbSpeed_final * Time.deltaTime);}
		if (Input.GetKey (KeyCode.E)) {transform.position += transform.up * climbSpeed_final * Time.deltaTime;}
 
		// if (Input.GetKeyDown (KeyCode.End))
		// {
		// 	Screen.lockCursor = (Screen.lockCursor == false) ? true : false;
		// }
	}
}