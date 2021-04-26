using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthSpawner : MonoBehaviour
{
    private const int LEFT = 0, TOP = 1, RIGHT = 2, BOTTOM = 3;

    [Header("External components")]
    [SerializeField] private Camera _cam;
    [SerializeField] private GameObject _player;
    [SerializeField] private DepthMonitor _depthMonitor = default;
    [SerializeField, Space(10.0f)] private float _distanceFromCamera = 9.0f;
    [Header("Spawn settings")]
    [SerializeField] private List<GameObject> _rightEnemies = new List<GameObject>();
    [SerializeField] private List<GameObject> _leftEnemies = new List<GameObject>();
    [SerializeField] private List<GameObject> _bottomEnemies = new List<GameObject>();
    [SerializeField] private List<GameObject> _topEnemies = new List<GameObject>();
    [SerializeField] private float _baseSpawnCooldown = 2.5f;
    [SerializeField] private AnimationCurve _spawnRateCurve;
    [SerializeField] private float _spawnDelay;

    private Transform _playerTransform;
    private Rigidbody _playerRigidbody;
    private Coroutine _spawnCoroutine = null;
    private List<FrustumBorder> _frustrumBorders = new List<FrustumBorder>();
    private Queue<FrustumBorder> _inactiveBorders = new Queue<FrustumBorder>();
    private List<FrustumBorder> _activeBorders = new List<FrustumBorder>();
    private Vector3 _spawnPos;
    private int _bottomSpeedSpawn = 0;


    private float SpawnCooldown => _spawnRateCurve.Evaluate(_depthMonitor.ScaledCurrentDepth / _depthMonitor.MaxDepth) * _baseSpawnCooldown
                                   + _spawnRateCurve.Evaluate(_depthMonitor.ScaledCurrentDepth / _depthMonitor.MaxDepth) * Random.Range(-_spawnDelay, _spawnDelay);

    private void Start()
    {
        _playerTransform = _player.GetComponent<Transform>();
        _playerRigidbody = _player.GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        Player.OnPlayerDeath += DisableSpawn;

        InitiateSpawner();
        if(_spawnCoroutine == null)
        {
            _spawnCoroutine = StartCoroutine(SpawnCoroutine());
        }
    }

    private void OnDisable()
    {
        Player.OnPlayerDeath -= DisableSpawn;

        if (_spawnCoroutine != null)
        {
            StopCoroutine(_spawnCoroutine);
            _spawnCoroutine = null;
        }
    }

    private void Update()
    {
        if(_cam != null)
        {
            foreach(FrustumBorder border in _frustrumBorders)
            {
                border.CheckSpawnedEnemies();
            }
        }
    }

    private void DisableSpawn()
    {
        StopCoroutine(_spawnCoroutine);
    }

    private IEnumerator SpawnCoroutine()
    {
        while(_depthMonitor.ScaledCurrentDepth < _depthMonitor.MaxDepth)
        {
            yield return new WaitForSeconds(SpawnCooldown);
            Debug.Log("Current depth % : " + _depthMonitor.ScaledCurrentDepth / _depthMonitor.MaxDepth);
            SpawnRandomEnemy();
        }
    }

    private void InitiateSpawner()
    {
        var bottomLeft = new Vector3(0f, 0f, _distanceFromCamera);
        var topLeft = new Vector3(0f, 1f, _distanceFromCamera);
        var topRight = new Vector3(1f, 1f, _distanceFromCamera);
        var bottomRight = new Vector3(1f, 0f, _distanceFromCamera);

        var left = new FrustumBorder(bottomLeft, topLeft, _cam, _leftEnemies, _playerTransform, 0, .22f);
        _frustrumBorders.Add(left);
        _activeBorders.Add(left);

        var top = new FrustumBorder(topLeft, topRight, _cam, _topEnemies, _playerTransform, 0, .66f);
        _frustrumBorders.Add(top);
        _activeBorders.Add(top);

        var right = new FrustumBorder(bottomRight, topRight, _cam, _rightEnemies, _playerTransform, 0, .22f);
        _frustrumBorders.Add(right);
        _activeBorders.Add(right);

        var bottom = new FrustumBorder(bottomRight, bottomLeft, _cam, _bottomEnemies, _playerTransform, 0, .66f);
        _frustrumBorders.Add(bottom);
        _activeBorders.Add(bottom);
    }

    private void SpawnRandomEnemy()
    {
        if(_frustrumBorders.Count == 0)
        {
            InitiateSpawner();
        }

        FrustumBorder randomBorder;
        if(Vector3.Dot(_playerRigidbody.velocity, Vector3.down) > 0.0f && _playerRigidbody.velocity.magnitude > 9.0f)
        {
            randomBorder = _frustrumBorders[BOTTOM];
            _bottomSpeedSpawn++;
            if(_bottomSpeedSpawn > 4)
            {
                _frustrumBorders[TOP].Spawn(_distanceFromCamera);
                _bottomSpeedSpawn = 0;
            }
        }
        else
        {
            int randIndex = Random.Range(0, _activeBorders.Count);
            randomBorder = _activeBorders[randIndex];
            _activeBorders.Remove(randomBorder);
            _inactiveBorders.Enqueue(randomBorder);
            if(_inactiveBorders.Count > _frustrumBorders.Count / 2)
            {
                _activeBorders.Add(_inactiveBorders.Dequeue());
            }
        }

        randomBorder.Spawn(_distanceFromCamera);
    }

    private class FrustumBorder
    {
        public const int SECTION_START = 0, SECTION_MIDDLE = 1, SECTION_END = 2;

        public Transform player;
        public Vector3 start;
        public Vector3 end;
        public List<GameObject> possibleEnemies = new List<GameObject>();

        public Vector3 WorldStart
        {
            get { return _cam.ViewportToWorldPoint(start); }
        }
        public Vector3 WorldEnd
        {
            get { return _cam.ViewportToWorldPoint(end); }
        }
        private int _lastSpawn;

        private Camera _cam;
        private List<Transform> _spawnedEnemies = new List<Transform>();

        private float _minSection;
        private float _maxSection;

        public FrustumBorder(Vector3 start, Vector3 end, Camera cam, List<GameObject> enemies, Transform player, float minSection, float maxSection)
        {
            this.start = start;
            this.end = end;
            this._cam = cam;
            this.possibleEnemies = enemies;
            this.player = player;
            this._minSection = minSection;
            this._maxSection = maxSection;
        }

        public Vector3 GetRandomSpawnPoint(float depthFromCam)
        {
            float margin = 0f;
            float posOnBorder = Random.Range(_minSection + margin, _maxSection - margin);
            switch (_lastSpawn)
            {
                case 0:
                    posOnBorder += .33f;
                    break;
                case 1:
                    posOnBorder += (posOnBorder > .33f - margin) ? .33f : 0;
                    break;
            }

            if (posOnBorder < .33f - margin)
            {
                _lastSpawn = SECTION_START;
            }
            else if (posOnBorder < .66f - margin)
            {
                _lastSpawn = SECTION_MIDDLE;
            }
            else
            {
                _lastSpawn = SECTION_END;
            }

            Vector3 pos = (end - start).normalized * posOnBorder + start;
            pos.z = depthFromCam;
            return _cam.ViewportToWorldPoint(pos);
        }

        public void Spawn(float depthFromCam)
        {
            if(possibleEnemies.Count <= 0)
            {
                return;
            } 

            Vector3 spawnPos = GetRandomSpawnPoint(depthFromCam);
            int randomEnemy = Random.Range(0, possibleEnemies.Count);
            Transform spawnedEnemy = InstantiateEnemy(possibleEnemies[randomEnemy], spawnPos);
            _spawnedEnemies.Add(spawnedEnemy);
        }

        public Transform InstantiateEnemy(GameObject prefab, Vector3 position)
        {
            GameObject instance = Instantiate(prefab, position, Quaternion.Euler(0f,0f,0f));

            return instance.transform;
        }

        public void CheckSpawnedEnemies()
        {
            List<Transform> toDelete = new List<Transform>();
            foreach (Transform enemy in _spawnedEnemies)
            {

                if (enemy)
                {
                    if (Vector3.Distance(_cam.transform.position, enemy.position) > 30)
                    {
                        toDelete.Add(enemy);
                    }
                }
            }

            foreach (Transform item in toDelete)
            {
                _spawnedEnemies.Remove(item);
                GameObject.Destroy(item.gameObject);
            }
        }
    }
}
