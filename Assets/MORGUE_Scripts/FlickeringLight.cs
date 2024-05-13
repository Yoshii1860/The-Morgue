using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickeringLight : MonoBehaviour
{
    #region Variables

    [Tooltip("External light to flicker; you can leave this null if you attach script to a light")]
    [SerializeField] private Light _light;
    [Tooltip("Minimum random light intensity")]
    [SerializeField] private float _minIntensity = 0f;
    [Tooltip("Maximum random light intensity")]
    [SerializeField] private float _maxIntensity = 1f;
    [Tooltip("How much to smooth out the randomness; lower values = sparks, higher = lantern")]
    [Range(1, 50)]
    [SerializeField] private int _smoothing = 5;

    Queue<float> _smoothQueue;
    float _lastSum = 0;

    #endregion




    #region Unity Methods

    void Start() {
         _smoothQueue = new Queue<float>(_smoothing);
         // External or internal light?
         if (_light == null) {
            _light = GetComponent<Light>();
         }
    }

    void Update() {
        if (GetComponent<Light>() == null)
            return;

        // pop off an item if too big
        while (_smoothQueue.Count >= _smoothing) {
            _lastSum -= _smoothQueue.Dequeue();
        }

        // Generate random new item, calculate new average
        float newVal = Random.Range(_minIntensity, _maxIntensity);
        _smoothQueue.Enqueue(newVal);
        _lastSum += newVal;

        // Calculate new smoothed average
        GetComponent<Light>().intensity = _lastSum / (float)_smoothQueue.Count;
    }

    #endregion




    #region Public Methods

    public void Reset() 
    {
        _smoothQueue.Clear();
        _lastSum = 0;
    }

    #endregion
}