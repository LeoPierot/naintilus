using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Spawner : MonoBehaviour
{

    public Camera cam;
    public Transform player;
    public float depthFromCam = .301f;
    public GameObject requinPrefab;
    public GameObject murenePrefab;
    public GameObject mainPrefab;
    public GameObject medusePrefab;
    public GameObject calmarPrefab;
    private List<FrustumBorder> frustumBorders = new List<FrustumBorder>();
    public bool debug;
    public bool debuging;
    private Queue<FrustumBorder> _InactiveBorders = new Queue<FrustumBorder>();
    private List<FrustumBorder> _ActiveBorders = new List<FrustumBorder>();
public float timeBetweenSpawn = 2;
    private void Start() {
        InitiateSpawner();
    }

    public void Update()
    {
        Debug.DrawLine(transform.position, transform.forward * depthFromCam, Color.red);
        if(cam)
        {
            if(debug)
            {
                debug = false;
                StartCoroutine(DebugCoroutine());
            }
            //Debug.DrawLine(cam.ViewportToWorldPoint(new Vector3(.5f, .5f,depthFromCam)), spawnPos, Color.red);
            foreach(FrustumBorder border in frustumBorders)
            {
                border.CheckSpawnedEnemies();
            }
        }
    }
private Vector3 spawnPos;
    private IEnumerator DebugCoroutine()
    {
        debuging = true;
        while(debuging)
        {
            SpawnRandomEnemy();
            yield return new WaitForSeconds(timeBetweenSpawn);
        }
    }

    private void InitiateSpawner()
    {
        //stock frustum borders
        Vector3 bottomLeft = new Vector3(0, 0, depthFromCam);
        Vector3 topLeft = new Vector3(0, 1, depthFromCam);
        Vector3 topRight = new Vector3(1, 1, depthFromCam);
        Vector3 bottomright = new Vector3(1, 0, depthFromCam);
        
        //setup enemies that can spawn at each border of the screen
        RequinScie roquin = new RequinScie();
        roquin.prefab = requinPrefab;
        CalmarNouille calmar = new CalmarNouille();
        calmar.prefab = calmarPrefab;
        SeauMeduse meduse = new SeauMeduse();
        meduse.prefab = medusePrefab;
        MureneChaussette chaussette = new MureneChaussette();
        chaussette.prefab = murenePrefab;
        Main main = new Main();
        main.prefab = mainPrefab;

        List<Enemy> leftEnemies = new List<Enemy>();
        leftEnemies.Add(roquin);
        leftEnemies.Add(calmar);
        leftEnemies.Add(chaussette);
        List<Enemy> topEnemies = new List<Enemy>();
        topEnemies.Add(main);
        List<Enemy> rightEnemies = new List<Enemy>();
        rightEnemies.Add(roquin);
        rightEnemies.Add(calmar);
        rightEnemies.Add(chaussette);
        List<Enemy> bottomEnemies = new List<Enemy>();
        bottomEnemies.Add(meduse);
        bottomEnemies.Add(calmar);
        bottomEnemies.Add(roquin);
        
        //save borders paramas as a whole object
        FrustumBorder left = new FrustumBorder(bottomLeft, topLeft, cam, leftEnemies, player, 0, .22f);
        FrustumBorder top = new FrustumBorder(topLeft, topRight, cam, topEnemies, player, 0, .66f);
        FrustumBorder right = new FrustumBorder(bottomright, topRight, cam, rightEnemies, player, 0, .22f);
        FrustumBorder bottom = new FrustumBorder(bottomright, bottomLeft, cam, bottomEnemies, player, 0, .66f); 

        frustumBorders.Add(left);
        frustumBorders.Add(top);
        frustumBorders.Add(right);
        frustumBorders.Add(bottom);
        _ActiveBorders.Add(left);
        _ActiveBorders.Add(top);
        _ActiveBorders.Add(right);
        _ActiveBorders.Add(bottom);
    }

    public void SpawnRandomEnemy()
    {
        if(frustumBorders.Count == 0)
        {
            InitiateSpawner();
        }
        int randIndex= Random.Range(0, _ActiveBorders.Count);
        FrustumBorder randomBorder = _ActiveBorders[randIndex];
        _ActiveBorders.Remove(randomBorder);
        _InactiveBorders.Enqueue(randomBorder);
        if(_InactiveBorders.Count > frustumBorders.Count/2)
        {
            _ActiveBorders.Add(_InactiveBorders.Dequeue());
        }

        randomBorder.Spawn(depthFromCam);
    }

    public class FrustumBorder
    {
        public const int SECTION_START = 0, SECTION_MIDDLE = 1, SECTION_END=2;

        public Transform player;
        public Vector3 start;
        public Vector3 end;
        public List<Enemy> possibleEnemies = new List<Enemy>(); 

        public Vector3 worldStart
        {
            get{return cam.ViewportToWorldPoint(start);}
        }
        public Vector3 worldEnd
        {
            get{return cam.ViewportToWorldPoint(end);}
        }
        private int _LastSpawn; 

        private Camera cam;
        private List<Transform> _SpawnedEnemies = new List<Transform>();

        private float minSection;
        private float maxSection;

        public FrustumBorder(Vector3 start, Vector3 end, Camera cam, List<Enemy> enemies, Transform player, float minSection, float maxSection)
        {
            this.start = start;
            this.end = end;
            this.cam = cam;
            this.possibleEnemies = enemies;
            this.player = player;
            this.minSection = minSection;
            this.maxSection = maxSection;
        }

        public Vector3 GetRandomSpawnPoint(float depthFromCam)
        {
            float margin = 0f;
            float posOnBorder = Random.Range(minSection + margin, maxSection - margin);
            switch(_LastSpawn)
            {
                case 0: 
                    posOnBorder += .33f;
                    break;
                case 1:
                    posOnBorder += (posOnBorder > .33f - margin) ? .33f : 0;
                    break;
            }

            if(posOnBorder < .33f - margin)
            {
                _LastSpawn = SECTION_START;
            }
            else if(posOnBorder < .66f - margin)
            {
                _LastSpawn = SECTION_MIDDLE;
            }
            else{
                _LastSpawn = SECTION_END;
            }

            Vector3 pos = (end - start).normalized * posOnBorder + start;
            pos.z = depthFromCam;
            return cam.ViewportToWorldPoint(pos);
        }

        public void Spawn(float depthFromCam)
        {
            Vector3 spawnPos = GetRandomSpawnPoint(depthFromCam);
            Vector3 spawnForward = cam.ViewportToWorldPoint(new Vector3(.5f, .2f, depthFromCam)) - spawnPos;
            int randomEnemy = Random.Range(0, possibleEnemies.Count);
            Transform instance = possibleEnemies[randomEnemy].Instantiate(spawnPos, spawnForward).transform;
            _SpawnedEnemies.Add(instance);
        }

        public void CheckSpawnedEnemies()
        {
            List<Transform> toDelete = new List<Transform>();
            foreach(Transform enemy in _SpawnedEnemies)
            {
                
                if(enemy)
                {
                    if(Vector3.Distance(cam.transform.position, enemy.position)>30)
                    {
                        toDelete.Add(enemy);
                    }
                }
            }

            foreach(Transform item in toDelete)
            {
                _SpawnedEnemies.Remove(item);
                GameObject.Destroy(item.gameObject);
            }
        }
    }

}
