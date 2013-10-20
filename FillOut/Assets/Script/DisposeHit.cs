using UnityEngine;
using System.Collections;

public class DisposeHit : MonoBehaviour {
	
	public GameObject clashLight;
		
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnCollisionEnter(Collision collision) {
		
		if(collision.gameObject.name == "Ball_Cube(Clone)"){ 		
			//Debug.Log("DisposeHit " + collision.gameObject.name);
        	/*
			GameObject go = Instantiate(clashLight,
				new Vector3(this.transform.position.x + Random.Range(-0.2f,0.2f),
				this.transform.position.y + Random.Range(-0.2f,0.2f),
				this.transform.position.z + Random.Range(-0.2f,0.2f)),
				new Quaternion(Random.Range(0,360),Random.Range(0,360),Random.Range(0,360),0)) as GameObject;
			go.transform.localScale*=Random.Range(0.5f,1.0f);
			//go.rigidbody.AddRelativeTorque(Random.Range(1,2),Random.Range(1,2),Random.Range(1,2),ForceMode.Impulse);
			
			AutoFadeout af = go.GetComponent<AutoFadeout>();
			af.enabled = true;
			*/
			
			//Destroy(gameObject);
			
			gameObject.SetActive(false);
			
			//UnityEditor.EditorApplication.Beep();
		
			//collision.gameObject.audio.Play();
			
			if(ClickBallOut.singletone != null){
				ClickBallOut.singletone.AddScore(transform.position);
			}
		} 
    }
	
	Color[] _newColor;
	
	private void OnTriggerEnter(Collider collision)
	{
		/*
		Mesh _mesh = this.GetComponent<MeshFilter>().mesh;
		_newColor = _mesh.colors;
		
		for(int i=0; i < _mesh.colors.Length; i++)
		{
			_newColor[i] = new Color(_newColor[i].r,_newColor[i].g,_newColor[i].b,0);
		}
		
		_mesh.colors = _newColor;
		*/
		
		//Debug.Log("OnTriggerEnter " + collision.gameObject.name);
	}
	 
	private void OnTriggerExit(Collider collision)
	{
		/*
		Mesh _mesh = this.GetComponent<MeshFilter>().mesh;
		_newColor = _mesh.colors;
		
		for(int i=0; i < _mesh.colors.Length; i++)
		{
			_newColor[i] = new Color(_newColor[i].r,_newColor[i].g,_newColor[i].b,1);
		}
		
		_mesh.colors = _newColor;
		*/
		
		//Debug.Log("OnTriggerExit " + collision.gameObject.name);
	}	
}
