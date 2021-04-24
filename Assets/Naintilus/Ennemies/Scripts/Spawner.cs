using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Spawner : MonoBehaviour
{

    public Camera cam;
    public float depthFromCam = .301f;
    private List<FrustumBorder> frustumBorders = new List<FrustumBorder>();
    public bool debug;
    public bool debuging;
    private Queue<FrustumBorder> _InactiveBorders = new Queue<FrustumBorder>();
    private List<FrustumBorder> _ActiveBorders = new List<FrustumBorder>();

    private void Start() {
        ComputeFrustumBorders();
    }

    public void Update()
    {
        if(cam)
        {
            if(debug)
            {
                debug = false;
                StartCoroutine(DebugCoroutine());
            }
            Debug.DrawLine(cam.ViewportToWorldPoint(new Vector3(.5f, .5f,depthFromCam)), spawnPos, Color.red);
        }
    }
private Vector3 spawnPos;
    private IEnumerator DebugCoroutine()
    {
        debuging = true;
        while(debuging)
        {
            spawnPos = GetRandomSpawnPos();
            yield return new WaitForSeconds(.5f);
        }
    }

    private void ComputeFrustumBorders()
    {
        Vector3 bottomLeft = new Vector3(0, 0, depthFromCam);
        Vector3 topLeft = new Vector3(0, 1, depthFromCam);
        Vector3 topRight = new Vector3(1, 1, depthFromCam);
        Vector3 bottomright = new Vector3(1, 0, depthFromCam);
        
        FrustumBorder left = new FrustumBorder(bottomLeft, topLeft, cam);
        FrustumBorder top = new FrustumBorder(topLeft, topRight, cam);
        FrustumBorder right = new FrustumBorder(topRight, bottomright, cam);
        FrustumBorder bottom = new FrustumBorder(bottomright, bottomLeft, cam); 

        
        frustumBorders = new List<FrustumBorder>();
        frustumBorders.Add(left);
        frustumBorders.Add(top);
        frustumBorders.Add(right);
        frustumBorders.Add(bottom);
        _ActiveBorders.Add(left);
        _ActiveBorders.Add(top);
        _ActiveBorders.Add(right);
        _ActiveBorders.Add(bottom);
    }

    public Vector3 GetRandomSpawnPos()
    {
        if(frustumBorders.Count == 0)
        {
            ComputeFrustumBorders();
        }
        int randIndex= Random.Range(0, _ActiveBorders.Count);
        FrustumBorder randomBurder = _ActiveBorders[randIndex];
        _ActiveBorders.Remove(randomBurder);
        _InactiveBorders.Enqueue(randomBurder);
        if(_InactiveBorders.Count > 2)
        {
            _ActiveBorders.Add(_InactiveBorders.Dequeue());
        }

        return randomBurder.GetRandomSpawnPoint();
    }

    public class FrustumBorder
    {
        public const int SECTION_START = 0, SECTION_MIDDLE = 1, SECTION_END=2;

        public Vector3 start;
        public Vector3 end;

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

        public FrustumBorder(Vector3 start, Vector3 end, Camera cam)
        {
            this.start = start;
            this.end = end;
            this.cam = cam;
        }

        public Vector3 GetRandomSpawnPoint()
        {
            float margin = .1f;
            float posOnBorder = Random.Range(0 + margin, .66f - margin);
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
            return cam.ViewportToWorldPoint(pos);
        }
    }

}
