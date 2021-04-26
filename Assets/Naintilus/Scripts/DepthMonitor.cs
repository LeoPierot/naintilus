using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DepthMonitor : MonoBehaviour
{
    [SerializeField] private Transform _player = default;
    [SerializeField] private float _maxDepth = 2000.0f;
    [Header("HUD Elements")]
    [SerializeField] private TextMeshProUGUI _depthCounter = default;
    [SerializeField] private Gradient _counterGradient = new Gradient();
    [SerializeField, Range(0.1f, 1.0f)] private float _depthScaleFactor = 0.2f;
    [Header("Lighting Settings")]
    [SerializeField] private Light _globalLight = default;
    [SerializeField, Range(0.0f, 2.0f)] private float _globalLightMinValue = 0.0f;
    [SerializeField, Range(0.5f, 2.5f)] private float _globalLightMaxValue = 1.5f;
    [SerializeField] private Light _localLight = default;
    [SerializeField, Range(0.0f, 10.0f)] private float _localLightMinValue = 0.0f;
    [SerializeField, Range(0.5f, 10.5f)] private float _localLightMaxValue = 1.5f;
    [SerializeField] private float _lightModifier = 0.01f;

    private float _bestDepthAchieved;

    private float CurrentDepth => -_player.position.y;
    public float ScaledCurrentDepth => CurrentDepth * _depthScaleFactor;
    public float MaxDepth => _maxDepth;
    public float DepthLightFactor => (ScaledCurrentDepth / _maxDepth) * _lightModifier * Time.fixedDeltaTime;


    private void Start()
    {
        _depthCounter.text = "0.0m";
        _bestDepthAchieved = float.MinValue;
    }

    private void LateUpdate()
    {
        _bestDepthAchieved = Mathf.Max(_bestDepthAchieved, ScaledCurrentDepth);
        _depthCounter.text = (_bestDepthAchieved).ToString("0.0") + "m";
        _depthCounter.color = _counterGradient.Evaluate(DepthLightFactor);

        _globalLight.intensity = Mathf.Lerp(_globalLightMaxValue, _globalLightMinValue, DepthLightFactor);
        _localLight.intensity = Mathf.Lerp(_localLightMinValue, _localLightMaxValue, DepthLightFactor);
    }
}
