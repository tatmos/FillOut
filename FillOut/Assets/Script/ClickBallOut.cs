using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClickBallOut : MonoBehaviour {
	
	public GameObject ballPrefab;
	public GameObject wallPrefab;
	public AudioClip clearLevel;	
	public AudioClip gameOver;
	public AudioClip beat1;
	public AudioClip breakWall;
	public AudioClip breakWall2;
	
	GameObject[] crashSoundGoList = new GameObject[48];
	AudioSource[] crashSoundList = new AudioSource[48];
	
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
	public int endLevel = 8;
	
	int level = 0;
	
	ParticleSystem ps;
	
	WallManager wm;
	
	// Use this for initialization
	void Start () {		
		
		wm = gameObject.AddComponent<WallManager>();
		wm.wallPrefab = wallPrefab;
		
		for(int i = 0 ;i<crashSoundList.Length;i++){
			crashSoundGoList[i] = new GameObject();
			crashSoundGoList[i].transform.parent = this.transform;
			AudioSource crashSound = crashSoundGoList[i].AddComponent<AudioSource>();
			switch(i%2)
			{
				case 0:
					crashSound.clip = breakWall;break;
				case 1:
					crashSound.clip = breakWall2;break;
			}
			crashSound.dopplerLevel = 0;
			//crashSound.rolloffMode = AudioRolloffMode.Linear;
			crashSoundList[i] = crashSound;
		}
		
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
		Restart();
		
		
	}
	public GameObject eye_target;
	
	bool game = false;
	
	bool createBallEnable = false;
	float createBallStartTime;
	List<GameObject> balls;
	
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
				
				wm.CreateWall_Stage1(5+level,level);
				gameStartTime = Time.timeSinceLevelLoad;
				gameTimeRest = gameTime;
				if(level == endLevel){
					gameTimeRest*=2;	//	The last 2 times 
				}
				
				if(level > 4)
				{
					audio.Stop();
					audio.clip = beat1;	// beat from level5
					audio.bypassListenerEffects = true;
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
		
		if(Vector3.Distance(lastPlayPos,pos) > 2){
			//AudioSource.PlayClipAtPoint(breakWall,pos);	
			
			crashSoundGoList[playCount%(crashSoundList.Length-1)].transform.position = pos;
			crashSoundList[playCount%(crashSoundList.Length-1)].pitch = pos.z*0.0001f + Mathf.Pow(2,(posToScale((int)(pos.y))+level)/12.0f); // ピッチから再生速度
			
			int sample_double = (int)((((beat1.samples%88200) - (audio.timeSamples%88200)) % 44100)*(120f/180f));
			if(sample_double < 0)sample_double = 0;
			
			crashSoundList[playCount%(crashSoundList.Length-1)].Play((ulong)sample_double);
			
			lastPlayPos = pos;
			playCount++;
			Debug.Log("score pos :" + pos.ToString() +" " + sample_double.ToString() + " " + (playCount%(crashSoundList.Length-1)));
		}
//		crashSound.Play();	
	}
	Vector3 lastPlayPos = Vector3.zero;
	int playCount = 0;
	
	int posToScale(int pos)
	{
		int oct = 0;
		if(pos < -1)oct = -1;
		if(pos > 1)oct = 1;
		
		int[] scale = {0,2,4,7,9,10};
		
		return scale[Mathf.Abs(pos)%(scale.Length-1)]+(oct*7);
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
			
			
			if(level >endLevel){
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
		if(wm.backWalls != null){
			
			StartCoroutine(wm.DestroyBackWallEvent(0f));
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
				
				if(level== endLevel){
					str += string.Format("\n\n\n\nWait {0:F2}   Final Level ({1} / {2})\n",waitTime-(Time.timeSinceLevelLoad - waiteStartTime),level,endLevel);
					for(int i =0;i<(int)(2f*(waitTime-(Time.timeSinceLevelLoad - waiteStartTime)));i++){
						str += "■";
					}
				} else {
					str += string.Format("\n\n\n\nWait {0:F2}   Next Level ({1} / {2})\n",waitTime-(Time.timeSinceLevelLoad - waiteStartTime),level,endLevel);	
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
		
		GUILayout.Label(hudText.text);
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
				StartCoroutine(wm.DestroyBackWallEvent(0f));
				
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
	

	
	IEnumerator LoadLevelEvent(float length)
	{
		{
	        yield return  new WaitForSeconds(length);
			while(wm.backWalls.Count!=0){
	            yield return  new WaitForSeconds(0.001f);
			}			
			Application.LoadLevel(Application.loadedLevelName);				
		}
	}
	
	void CheckInput()
	{
		
		if((Input.GetMouseButton(0) || Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Space)) && !createBallEnable){
			createBallStartTime = Time.timeSinceLevelLoad;
			
			CreateBall();
		}
		if(((Input.GetMouseButtonUp(0) || Input.GetButtonUp("Fire1") || Input.GetKeyUp(KeyCode.Space)) || (Time.timeSinceLevelLoad - createBallStartTime) > RateOfFire) 
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
		
		
		go.transform.rotation = eye_target.transform.rotation;//Camera.main.transform.rotation;
		
		float force = Time.timeSinceLevelLoad - createBallStartTime;
		force*= 100f;
		go.rigidbody.AddRelativeForce(new Vector3(0,0,force),ForceMode.Impulse);
		
		go.AddComponent<ReflectHit>();
		
		//Debug.Log("CreateBall" + force);
		
		createBallEnable = false;
	}
	

}
