using UnityEngine;
using System.Collections;

public class ReflectHit : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnCollisionEnter(Collision collision) {
		/*
        ContactPoint contact = collision.contacts[0];
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
        Vector3 pos = contact.point;
        Instantiate(explosionPrefab, pos, rot) as Transform;
        */
		//if(collision.gameObject.name != this.gameObject.name)
		{ 		
			
        	//Destroy(gameObject);
			//UnityEditor.EditorApplication.Beep();
			
			this.gameObject.transform.LookAt(collision.transform.position);
			this.gameObject.transform.InverseTransformDirection(Vector3.forward);
			
			float force = collision.relativeVelocity.sqrMagnitude*0.012f;
			//Debug.Log("ReflectHit " + force);
			this.gameObject.rigidbody.AddRelativeForce(new Vector3(0,0,force),ForceMode.Acceleration);
			
			

		
		}
    }
}
