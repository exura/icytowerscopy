using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour {

    public Transform player;

    public GameObject platformPrefab;
    public GameObject checkpointPrefab;
    public GameObject leftWallPrefab;
    public GameObject rightWallPrefab;
    public GameObject backgroundPrefab;

   
    private List<GameObject> platforms = new List<GameObject>();
    private List<GameObject> walls = new List<GameObject>();
    private List<GameObject> backgrounds = new List<GameObject>();

    public int maxPlatforms;
    public float levelRightEdge;
    public float levelLeftEdge;
    public float distance;
    private int createdPlatforms;
    private int createdWalls;
    private int createdBackgrounds;
    private float screenHeight = 9f;
    

	// Use this for initialization
	void Start () {
        createdPlatforms = 0;
        createdWalls = 0;
        maxPlatforms = 10;
        levelRightEdge = 6f;
        levelLeftEdge = -6f;
        distance = 2.5f;
        createdBackgrounds = 0;




        for (int i = 0; i < maxPlatforms; i++)
        {
            if(createdPlatforms == 0)
            {
                Vector3 spawnPos = new Vector3(-0.1111517f, distance * i, 0);
                platforms.Add(Instantiate(checkpointPrefab, spawnPos, Quaternion.identity)); //we wont rotate the object a all
                createdPlatforms++;
            }
            else
            {
                Vector3 spawnPos = new Vector3(Random.Range(levelLeftEdge, levelRightEdge), distance * i, 0);
                platforms.Add(Instantiate(platformPrefab, spawnPos, Quaternion.identity)); //we wont rotate the object a all
                createdPlatforms++;
            }
            
        }

       //instantiate the first walls 
       for(int i = 0; i < 2; i++)
        {
            createWalls();          
        }

       //instantiating the first backgrounds
        for(int i = 0; i <2; i++ ){
            createBackground();
        }
        
     
        
        
		
	}
	
	// Update is called once per frame
	void Update () {

        Vector2 platform = platforms[0].transform.position;

        if(player.position.y > platform.y + screenHeight)
        {
            //Generate new platforms
            Destroy(platforms[0]);
            platforms.RemoveAt(0);
            createPlatform();
        }

        Vector2 wallPosition = walls[0].transform.position;
		
        if(player.position.y > wallPosition.y + screenHeight * 2)
        {
            //generate new walls
            Destroy(walls[0]);
            Destroy(walls[1]);
            walls.RemoveAt(1); //remove at index 1 first or remove index 0 two times.
            walls.RemoveAt(0);
            createWalls();
        }

        Vector2 backgroundPosition = backgrounds[0].transform.position;
        if (player.position.y > backgroundPosition.y + 2*screenHeight)
        {
            Destroy(backgrounds[0]);
            backgrounds.RemoveAt(0);
            createBackground();
        }

	}

    void createPlatform()
    {
        float ySpawn = distance * (createdPlatforms );
        createdPlatforms++;
        if ((createdPlatforms - 1) % 50 == 0)
        {            
            Vector3 spawnPos = new Vector3(-0.1111517f, ySpawn, 0);
            platforms.Add(Instantiate(checkpointPrefab, spawnPos, Quaternion.identity)); //we wont rotate the object a all            
        }
        else
        {
            Vector3 spawnPos = new Vector3(Random.Range(levelLeftEdge, levelRightEdge), ySpawn, 0);
            platforms.Add(Instantiate(platformPrefab, spawnPos, Quaternion.identity)); //we wont rotate the object a all
        }
    }

    void createWalls()
    {
        Vector3 spawnPosLeft = new Vector3(-9.5f, createdWalls * 19.2f, 0);
        Vector3 spawnPosRight = new Vector3(9.5f, createdWalls * 19.2f, 0);
        walls.Add(Instantiate(leftWallPrefab, spawnPosLeft, Quaternion.identity));
        walls.Add(Instantiate(rightWallPrefab, spawnPosRight, Quaternion.identity));
        createdWalls++;
    }

    void createBackground()
    {
        
        Vector3 spawnPos = new Vector3(0, 19.21f * createdBackgrounds, 0);
        backgrounds.Add(Instantiate(backgroundPrefab, spawnPos, Quaternion.identity));
        createdBackgrounds++;
    }
}
