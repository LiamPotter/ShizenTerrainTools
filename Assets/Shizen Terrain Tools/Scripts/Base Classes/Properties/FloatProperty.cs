using UnityEngine;

namespace Shizen.Base.Properties
{
    [System.Serializable]
    public class FloatProperty : RandomFloatProperty
    {
        private float value;
        public float Value { get { return value; } }

        public void SetValue(float toValue)
        {
            value = toValue;
        }      

        public override bool CreateRandomValue()
        {
            if (!Randomized)
                return false;
            value = Random.Range(Min, Max);
            return true;
        }

        public FloatProperty(string name)
        {
            SetName(name);
            value = 0;
        }
        public FloatProperty(string name,float toValue)
        {
            SetName(name);
            value = toValue;
        }
    }
}
