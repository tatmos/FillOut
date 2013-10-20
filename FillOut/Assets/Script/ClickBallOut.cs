using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClickBallOut : MonoBehaviour {
	
	public GameObject ballPrefab;
	public GameObject wallPrefab;
	public GameObject clashLight;
	public AudioClip clearLevel;	
	public AudioClip gameOver;
	public AudioClip beat1;
	public AudioClip breakWall;
	
	AudioSource crashSound;
	
	
	public static ClickBallOut singletone;
	
	int score = 0;

	int hiScore = 1000000;
	
	float gameStartTime;
	float gameTimeRest;
	
	float waitTime = 5;
	float waiteStartTime;
	
	public float gameTime = 15f;
	
	public float RateOfFire = 0.5f;
	
	public int startLevel = 5;
	int level = 0;
	
	ParticleSystem ps;
	
	// Use this for initialization
	void Start () {		
		
		crashSound = gameObject.AddComponent<AudioSource>();
		crashSound.clip = breakWall;
		crashSound.rolloffMode = AudioRolloffMode.Linear;
		
		if(hudText == null)
		{
			hudText = gameObject.GetComponentInChildren<GUIText>();
		}
		
		ps = GetComponentInChildren<ParticleSystem>();
		
		level = startLevel;
		
		singletone = this;
		
		endFlag = false;
		score = 0;
		
		balls = new List<GameObject>();
		backWalls = new List<GameObject>();
		Restart();
		
	}
	bool game = false;
	
	bool createBallEnable = false;
	float createBallStartTime;
	List<GameObject> balls;
	List<GameObject> backWalls;
	
	void GameMain()
	{
		if(game){
			
			if(gameTimeRest-(Time.timeSinceLevelLoad - gameStartTime) < 0)
			{
				Restart();
			}		
			
			CheckInput();
			
		} else {
			if(waitTime -(Time.timeSinceLevelLoad - waiteStartTime) < 0){
				
				if(endFlag)score = 0;
				
				CreateWall_Stage1(5+level);
				gameStartTime = Time.timeSinceLevelLoad;
				gameTimeRest = gameTime;
				if(level == 10){
					gameTimeRest*=2;	//	The last 2 times 
				}
				
				if(level > 4)
				{
					audio.Stop();
					audio.clip = beat1;	// beat from level5
					audio.loop = true;
					audio.Play();
				}
				
				game = true;
			}
		}
	}
	
	int counter = 0;
	void Update () {
		
		GameMain();
		
		counter++;
		if(counter%5==0){
			DrawHUD();
		}
		
		if(crashStopTime <Time.timeSinceLevelLoad)
		{
			ps.emissionRate = 0;
			//ps.enableEmission = false;	
		}
	}
	
	bool endFlag = false;
	
	float crashStopTime = 0;
	
	public void AddScore(Vector3 pos)
	{
		score += 10 * level * level*2;	
		
		if(hiScore < score)
		{
			hiScore = score;	
		}
		
		//if(crashStopTime - 0.45 < Time.timeSinceLevelLoad)
		{
		//	AudioSource.PlayClipAtPoint(breakWall,pos*0.1f);
		}
		
		ps.transform.localPosition = pos;
		ps.startRotation = Random.Range(0,1);
		//ps.enableEmission = true;
		ps.emissionRate = 25;
		crashStopTime = Time.timeSinceLevelLoad + 0.5f;
		
		
		crashSound.Play();
	
	}
	
	void Restart()
	{
		game = false;
		waitTime = 3;
		waiteStartTime = Time.timeSinceLevelLoad;		
		
		if(balls != null){
			foreach(GameObject ball in balls){
				GameObject.Destroy(ball);	
			}
			balls.Clear();
			level++;
			
			
			if(level >10){
				endFlag = true;	
				level = 0;
				
				waitTime = 600;
				waiteStartTime = Time.timeSinceLevelLoad;
				
				audio.Stop();
				audio.loop = false;
				audio.PlayOneShot(gameOver);
			} else {
				audio.Stop();
				audio.loop = false;
				audio.PlayOneShot(clearLevel);
			}
		}
		if(backWalls != null){
			
			StartCoroutine(DestroyBackWallEvent(0f));
			//foreach(GameObject backWall in backWalls){
			//	GameObject.Destroy(backWall);	
			//}
			//backWalls.Clear();
		}
		createBallEnable = false;
	}
	
	GUIText hudText;
		
	void DrawHUD()		
	{
		
		string str
		= string.Format("SCORE {0} ",score) + string.Format("  HI-SCORE {0}",hiScore);
		if(game){
			str += string.Format("\n  TIME {0:F2} ",gameTimeRest-(Time.timeSinceLevelLoad - gameStartTime));
			
			for(int i =0;i<(int)(2f*(gameTimeRest-(Time.timeSinceLevelLoad - gameStartTime)));i++){
				str += "■";
			}
		} 
		if(endFlag)
		{
			str += "\nGame Over";
		} else {
			
			if(!game){
				
				if(level== 10){
					str += string.Format("\n\n\n\nWait {0:F2}   Final Level ({1} / 10)\n",waitTime-(Time.timeSinceLevelLoad - waiteStartTime),level);
					for(int i =0;i<(int)(2f*(waitTime-(Time.timeSinceLevelLoad - waiteStartTime)));i++){
						str += "■";
					}
				} else {
					str += string.Format("\n\n\n\nWait {0:F2}   Next Level ({1} / 10)\n",waitTime-(Time.timeSinceLevelLoad - waiteStartTime),level);	
					for(int i =0;i<(int)(2f*(waitTime-(Time.timeSinceLevelLoad - waiteStartTime)));i++){
						str += "■";
					}
				}
				
			}
		}
		
		hudText.text = str;
		
	}
	
	void OnGUI()
	{
		/*
		GUI.skin.label.fontSize = 24;
		GUI.skin.button.fontSize = 24;
		
		GUILayout.BeginHorizontal(GUILayout.Width(Screen.width));
		
		//GUILayout.Label(string.Format("SCORE {0}",score));
		//GUILayout.Label(string.Format("HI-SCORE {0}",hiScore));
		
		if(game){
			GUILayout.Label(string.Format("TIME {0:F2}",gameTimeRest-(Time.timeSinceLevelLoad - gameStartTime)));
		} else {
			//GUILayout.Label(string.Format("Wait {0:F2} Next Level{1}",waitTime-(Time.timeSinceLevelLoad - waiteStartTime),level));			
		}
		GUILayout.EndHorizontal();
		*/
		//if(endFlag)
		{
		//	GUILayout.Label("Game Over");
			
			GUILayout.BeginArea(new Rect(Screen.width-100,0,100,Screen.height));
			//GUILayout.BeginHorizontal(GUILayout.Width(Screen.width),GUILayout.Height(200));
			if(GUILayout.Button("ReStart")){
				StartCoroutine(DestroyBackWallEvent(0f));
				
				StartCoroutine(LoadLevelEvent(0f));
				
				//Application.LoadLevel("test");	
			}
			//GUILayout.EndHorizontal();
			GUILayout.EndArea();
		} /*else {
			
			if(!game){
				GUILayout.Label(string.Format("Wait {0:F2} Next Level{1}",waitTime-(Time.timeSinceLevelLoad - waiteStartTime),level));	
			}
		}
		*/
	}
	
	IEnumerator DestroyBackWallEvent(float length)
	{
		{
			
	        yield return  new WaitForSeconds(length);
			int i=0;
			foreach(GameObject backWall in backWalls){
				i++;
				GameObject.Destroy(backWall);	
				
				if(i%1000 == 0)
	            yield return  new WaitForSeconds(0.001f);
			}
			backWalls.Clear();
			
		}
	}
	
	IEnumerator LoadLevelEvent(float length)
	{
		{
	        yield return  new WaitForSeconds(length);
			while(backWalls.Count!=0){
	            yield return  new WaitForSeconds(0.001f);
			}
			
			Application.LoadLevel("test");	
			
		}
	}
	
	
	void CheckInput()
	{
		if(Input.GetMouseButton(0) && !createBallEnable){
			createBallStartTime = Time.timeSinceLevelLoad;
			
			CreateBall();
		}
		if((Input.GetMouseButtonUp(0) || (Time.timeSinceLevelLoad - createBallStartTime) > RateOfFire) 
			&& createBallEnable
			){
			this.ShootBall();	
		}
		
		if(Input.mousePosition.x < 40 || Input.GetKey(KeyCode.A)){
			Camera.main.camera.transform.Rotate(0,-0.3f,0);
		}
		if(Input.mousePosition.x > Screen.width-40 || Input.GetKey(KeyCode.D)){
			Camera.main.camera.transform.Rotate(0,0.3f,0);
		}
		if(Input.mousePosition.y < 40 || Input.GetKey(KeyCode.W)){
			Camera.main.camera.transform.Rotate(0.3f,0,0);
		}
		if(Input.mousePosition.y > Screen.height-40 || Input.GetKey(KeyCode.S)){
			Camera.main.camera.transform.Rotate(-0.3f,0,0);
		}
	}
	
	Color[] _newColor;
	
	void CreateBall()
	{
		createBallEnable = true;
		GameObject go = Instantiate(ballPrefab,Vector3.zero,Quaternion.identity) as GameObject;
		
		Mesh _mesh = go.GetComponent<MeshFilter>().mesh;
		_newColor = new Color[_mesh.normals.Length];//_mesh.colors;
		
		switch(balls.Count%5)
		{
			case 0:
				for(int i=0; i < _newColor.Length; i++)
				{
					_newColor[i] =Color.red;
				}
				//	go.audio.pitch = 1.0f;
			break;
			case 1:
				for(int i=0; i < _newColor.Length; i++)
				{
					_newColor[i] =Color.green;
				}
				//	go.audio.pitch = 1.5f;
			break;
			case 2:
				for(int i=0; i < _newColor.Length; i++)
				{
					_newColor[i] =Color.blue;
				}
				//	go.audio.pitch = 1.75f;
			break;		
			case 3:
				for(int i=0; i < _newColor.Length; i++)
				{
					_newColor[i] =Color.yellow;
				}
				//	go.audio.pitch = 1.95f;
			break;
			case 4:
			for(int i=0; i < _newColor.Length; i++)
				{
					_newColor[i] =Color.magenta;
				}
				//	go.audio.pitch = 2.00f;
			break;		
		}
		_mesh.colors = _newColor;
		
		balls.Add(go);
	}
	
	void ShootBall()
	{
		if(balls == null)return;
		
		GameObject go = balls[balls.Count-1];
		
		go.transform.rotation = Camera.main.transform.rotation;
		
		float force = Time.timeSinceLevelLoad - createBallStartTime;
		force*= 100f;
		go.rigidbody.AddRelativeForce(new Vector3(0,0,force),ForceMode.Impulse);
		
		go.AddComponent<ReflectHit>();
		
		//Debug.Log("CreateBall" + force);
		
		createBallEnable = false;
	}
	
	void CreateWall_Stage1(int startBoxSize)
	{
		//int startBoxSize = 6;
		int betweenBackWall = 3;
		
		for(int x = -startBoxSize+betweenBackWall;x<=startBoxSize-betweenBackWall;x++){		
			for(int y = -startBoxSize+betweenBackWall;y<=startBoxSize-betweenBackWall;y++){	
				for(int z = -startBoxSize+betweenBackWall;z<=startBoxSize-betweenBackWall;z++){	
					
					if((x==-startBoxSize+betweenBackWall || x ==startBoxSize-betweenBackWall) 
						||  
						(y==-startBoxSize+betweenBackWall || y ==startBoxSize-betweenBackWall) 
						
						|| 
						(z==-startBoxSize+betweenBackWall || z ==startBoxSize-betweenBackWall)
						
						){
						
						Color fragilWallColor = Color.white*0.9f;
						if((x+y+z)%2==0){
							fragilWallColor = Color.white;
						} else {
							fragilWallColor = new Color((float)x/startBoxSize*1f,(float)y/startBoxSize*1f,(float)z/startBoxSize*1f);
	
						}
						
						CreateFragileWall(new Vector3(x,y,z),fragilWallColor);
					}
				}
			}
		}
					
		for(int x = -startBoxSize;x<=startBoxSize;x++){		
			for(int y = -startBoxSize;y<=startBoxSize;y++){	
				for(int z = -startBoxSize;z<=startBoxSize;z++){	
					if((x==-startBoxSize || x ==startBoxSize) ||  
						(y==-startBoxSize || y ==startBoxSize) || 
						(z==-startBoxSize || z ==startBoxSize)){
						
						float wallColor = 0.75f;
						if((x+y+z)%2==0) wallColor = 0.25f;
						
						CreateWall(new Vector3(x,y,z),new Color((float)x/startBoxSize*1f+wallColor,(float)y/startBoxSize*1f+wallColor,(float)z/startBoxSize*1f+wallColor));
					}
					
				}
			}
		}
	}
	
	void CreateFragileWall(Vector3 pos,Color inColor)
	{
		GameObject go = Instantiate(wallPrefab,Vector3.zero,Quaternion.identity) as GameObject;
		//GameObject.CreatePrimitive(PrimitiveType.Cube);
		go.name = "Wall";
		//go.transform.localScale=new Vector3(0.98f,0.98f,0.98f);
		go.transform.localPosition = pos;
		
		//go.renderer.material.color = inColor;
		Mesh _mesh = go.GetComponent<MeshFilter>().mesh;
		_newColor = new Color[_mesh.normals.Length];
		for(int i=0; i < _newColor.Length; i++)
		{
			if(Random.Range(1,10)<level){
				_newColor[i] =inColor;
			} else {
				_newColor[i] =Color.white;
			}
		}
		_mesh.colors = _newColor;
		
		//DisposeHit disposeHit = 
		go.AddComponent<DisposeHit>();
		
		//disposeHit.clashLight = clashLight;
	}
	
	void CreateWall(Vector3 pos,Color inColor)
	{
		GameObject go = Instantiate(wallPrefab,Vector3.zero,Quaternion.identity) as GameObject;
		go.name = "Wall";
		//go.transform.localScale=new Vector3(0.98f,0.98f,0.98f);
		go.transform.localPosition = pos;
		
		//go.renderer.material.color = inColor;
		Mesh _mesh = go.GetComponent<MeshFilter>().mesh;
		_newColor = new Color[_mesh.normals.Length];
		for(int i=0; i < _newColor.Length; i++)
		{
			_newColor[i] =inColor;
		}
		_mesh.colors = _newColor;
		
		backWalls.Add(go);
		
	}
}
