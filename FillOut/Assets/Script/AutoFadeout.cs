using UnityEngine;
using System.Collections;

public class AutoFadeout : MonoBehaviour {
	
	float life = 1f;
	
	void Start () {
		
	}
	
	void Update () {
		life-= 0.1f;
		
		if(life < 0){
			GameObject.Destroy(this.gameObject);	
		}
	}
	
	
}
