using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    public GameObject character;
    public GameObject terrain;
    public GameObject[] toSpawn;

    GameObject          rootObj;
    List<GameObject>    spawnedObjets;

    public float nbObjectsPerSqMeter = 0.001f;
    public float terrainOriginalWidth = 100.0f;
    public float terrainOriginalDistance = 200.0f;

    public float        distanceStepSpawn = 10.0f;
    private float       lastSpawnDistance;
    private float       nextCharacterDistanceSpawn;
    private Vector3     characterPosition;
    private Quaternion  characterOrientation;

    private float time = 0.0f;
    public float timetofullspeedInSec = 180.0f;

    private void Awake()
    {
        characterPosition = character.transform.position;
        characterOrientation = character.transform.rotation;

        Reset();
    }

    // Use this for initialization
    void Start ()
    {
        rootObj = new GameObject("RootSpawnedObjects");
        spawnedObjets = new List<GameObject>();

        float charOffset = 10.0f;
        Vector3 A = new Vector3(character.transform.position.x, terrain.transform.position.y, character.transform.position.z - charOffset);
        Vector3 B = A + new Vector3(-terrainOriginalWidth* .5f, 0, terrainOriginalDistance);
        Vector3 C = A + new Vector3( terrainOriginalWidth* .5f, 0, terrainOriginalDistance);

        GenerateObjectsTriangle(A, B, C, character.transform.position.z + 5.0f);

        lastSpawnDistance = B.z;
        nextCharacterDistanceSpawn = character.transform.position.z + distanceStepSpawn;
    }

    void SpawnMore()
    {
        nextCharacterDistanceSpawn += distanceStepSpawn;

        float characterXpos = character.transform.position.x;
        Vector3 A = new Vector3(characterXpos - terrainOriginalWidth * .5f, terrain.transform.position.y, lastSpawnDistance);
        Vector3 B = new Vector3(characterXpos - terrainOriginalWidth * .5f, terrain.transform.position.y, lastSpawnDistance + distanceStepSpawn);
        Vector3 C = new Vector3(characterXpos + terrainOriginalWidth * .5f, terrain.transform.position.y, lastSpawnDistance + distanceStepSpawn);
        Vector3 D = new Vector3(characterXpos + terrainOriginalWidth * .5f, terrain.transform.position.y, lastSpawnDistance);

        lastSpawnDistance += distanceStepSpawn;

        GenerateObjectsTriangle(A, B, C);
        GenerateObjectsTriangle(B, C, D);
    }

    private void GenerateObjectsTriangle(Vector3 A, Vector3 B, Vector3 C, float minz = float.NegativeInfinity)
    {
        float a = (B-C).magnitude;
        float b = (A - C).magnitude;
        float c = (A - B).magnitude;
        float s = 0.5f*(a+b+c); ;
        float surface = Mathf.Sqrt(s*(s-a)*(s-b)*(s-c));

        int nbObjs = Mathf.FloorToInt(surface * nbObjectsPerSqMeter);
        //Debug.Log(nbObjs + " Objects created");

        for (int i = 0; i < nbObjs; ++i)
        {
            float r1sqrt = Mathf.Sqrt(Random.Range(0.0f, 1.0f));
            float r2 = Random.Range(0.0f, 1.0f);
            Vector3 pos = (1.0f - r1sqrt) * A + r1sqrt * (1.0f - r2) * B + r2 * r1sqrt * C;

            if (pos.z > minz)
            {
                int r = Random.Range(0, toSpawn.Length);
                GameObject g = GameObject.Instantiate(toSpawn[r], pos, toSpawn[r].transform.rotation);
                g.transform.parent = rootObj.transform;
                spawnedObjets.Add(g);
            }
        }
    }

    public void Reset()
    {
        character.transform.position = characterPosition;
        character.transform.rotation = characterOrientation;
        character.GetComponent<Animator>().Rebind();

        character.GetComponent<Animator>().SetBool("isWalking", false);
        character.GetComponent<Animator>().SetFloat("Speed", 0.0f);

        time = 0.0f;
    }

    // Update is called once per frame
    void Update ()
    {
        if (character.GetComponent<Animator>().GetBool("isWalking"))
        {
            time += Time.deltaTime;
            float speed = Mathf.Min(time / timetofullspeedInSec, 1.0f);
            character.GetComponent<Animator>().SetFloat("Speed", speed);
        }

        if(character.transform.position.z > nextCharacterDistanceSpawn)
            SpawnMore();
        // SeparateThread.Instance.ExecuteInThread(execute => { nextCharacterDistanceSpawn += distanceStepSpawn; SpawnMore(); }, null);        

        if (Input.GetKeyDown(KeyCode.Space))
        {
            character.GetComponent<Animator>().SetTrigger("Walk");
            character.GetComponent<Animator>().SetBool("isWalking", true);
        }
        else if (Input.GetKeyDown(KeyCode.R))
            Reset();
    }
}
