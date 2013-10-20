using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WallManager : MonoBehaviour {
	
	
	public GameObject wallPrefab;	
	Color[] _newColor;
	
	public List<GameObject> backWalls;
	
	// Use this for initialization
	void Start () {
	
		backWalls = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void CreateWall_Stage1(int startBoxSize,int level)
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
						
						CreateFragileWall(new Vector3(x,y,z),fragilWallColor,level);
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
	
	public void CreateFragileWall(Vector3 pos,Color inColor,int level)
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
	
	public void CreateWall(Vector3 pos,Color inColor)
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
	
	public IEnumerator DestroyBackWallEvent(float length)
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
}
