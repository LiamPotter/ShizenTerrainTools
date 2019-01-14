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

    public float Float { get { return _float; } }
    public float FloatMin { get { return _floatMin; } set { _floatMin = value; } }
    public float FloatMax { get { return _floatMax; } set { _floatMax = value; } }

    private int _int;
    private int _intMin;
    private int _intMax;
    public int Int { get { return _int; } }
    public int IntMin { get { return _intMin; } set { _intMin = value; } }
    public int IntMax { get { return _intMax; } set { _intMax = value; } }

    private Vector3 _vector3;
    public Vector3 Vector3 { get { return _vector3; } }

    public void SetRandomized(bool toValue)
    {
        _randomized = toValue;
    }
    public void SetFloat(float toValue)
    {
        _float = toValue;
    }
    public void SetInt(int toValue)
    {
        _int = toValue;
    }
    public void SetVector3(Vector3 toValue)
    {
        _vector3 = toValue;
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
