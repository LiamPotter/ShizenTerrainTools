using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HeightLayerProperty
{
    private bool _randomized=false;
    public bool Randomized { get { return _randomized; } }

    private float _float;
    private float _floatMin;
    private float _floatMax;

    public float Float
    {
        get
        {
           return _float;
        }
    }
    public float FloatMin { get { return _floatMin; } }
    public float FloatMax { get { return _floatMax; } }

    private int _int;
    private int _intMin;
    private int _intMax;
    public int Int
    {
        get
        {
            return _int;
        }
    }
    public int IntMin { get { return _intMin; } }
    public int IntMax { get { return _intMax; } }

    private Vector3 _vector3;
    public Vector3 Vector3
    {
        get
        {
           return _vector3;
        }
    }

    public void SetRandomized(bool toValue)
    {
        _randomized = toValue;
    }

    public void SetFloat(float toValue)
    {
        _float = toValue;
    }
    public void SetMinMaxFloats(float min,float max)
    {
        _floatMin = min;
        _floatMax = max;
        SetFloat(Random.Range(_floatMin, FloatMax));
    }

    public void SetInt(int toValue)
    {
        _int = toValue;
    }
    public void SetMinMaxInts(int min, int max)
    {
        _intMin = min;
        _intMax = max;
        SetInt(RoundToInt(Random.Range(_intMin, _intMax)));
    }

    public void SetVector3(Vector3 toValue)
    {
        _vector3 = toValue;
    }
    public void SetMinMaxVector3s(float min, float max)
    {
        _floatMin = min;
        _floatMax = max;
       
        _vector3 = new Vector3(
            Random.Range(_floatMin, _floatMax), Random.Range(_floatMin, _floatMax), Random.Range(_floatMin, _floatMax));
    }

    private int RoundToInt(float value)
    {
        return System.Convert.ToInt32(value);
    }

    public HeightLayerProperty()
    {

    }
    public HeightLayerProperty(int initialValue)
    {
        _int = initialValue;
    }
    public HeightLayerProperty(float initialValue)
    {
        _float = initialValue;
    }
    public HeightLayerProperty(Vector3 initialValue)
    {
        _vector3 = initialValue;
    }
   
}
