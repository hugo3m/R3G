using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnCharacterCollision : MonoBehaviour
{

    public Transform head;
    public Transform left_wrist;
    public Transform right_wrist;
    public Transform left_foot;
    public Transform right_foot;

    public GameLogic gl;

	public Rigidbody rigidbody;

    private void CheckCollision(Collision collision)
    {
        if (collision.gameObject.tag == "Obstacle")
        {
            bool coll = false;
            if (collision.collider.bounds.Contains(head.position))
            {
                coll = true;
                Debug.Log("Head Collision");
            }
            else if (collision.collider.bounds.Contains(left_foot.position))
            {
                coll = true;
                Debug.Log("Left foot Collision");
            }
            else if (collision.collider.bounds.Contains(right_foot.position))
            {
                coll = true;
                Debug.Log("Right foot Collision");
            }
            else if (collision.collider.bounds.Contains(left_wrist.position))
            {
                coll = true;
                Debug.Log("Left Wrist Collision");
            }
            else if (collision.collider.bounds.Contains(right_wrist.position))
            {
                coll = true;
                Debug.Log("Right wrist Collision");
            }
            else
                Debug.Log("No collision with body");

			if (coll) 
			{
				gl.Reset ();
				ResetRigidbody ();
			}
                
        }
    }
    [ContextMenu("ResetRigidbody")]
	void ResetRigidbody()
	{
		rigidbody.velocity = Vector3.zero; 
		rigidbody.angularVelocity = Vector3.zero;
		rigidbody.gameObject.transform.localRotation = Quaternion.identity;
		rigidbody.gameObject.transform.localPosition = Vector3.zero;         
	}

    private void OnCollisionEnter(Collision collision)
    {
        CheckCollision(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        //Debug.Log("Collision Remains" + collision.gameObject.name);
        CheckCollision(collision);
    }
}
