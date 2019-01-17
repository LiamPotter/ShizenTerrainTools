using UnityEngine;
namespace Shizen.Base.Properties
{
    [System.Serializable]
    public class Vector3Property : RandomFloatProperty
    {
        private Vector3 value;

        public Vector3 Value { get { return value; } }

        public void SetValue(Vector3 toValue)
        {
            value = toValue;
        }
        public void SetValue(float x, float y, float z)
        {
            value = new Vector3(x, y, z);
        }

       
        public override bool CreateRandomValue()
        {
            if (!Randomized)
                return false;
            SetValue(Random.Range(Min, Max), Random.Range(Min, Max), Random.Range(Min, Max));
            return true;
        }

        public Vector3Property(string name)
        {
            SetName(name);
            value = Vector3.zero;
        }
        public Vector3Property(string name,Vector3 toValue)
        {
            SetName(name);
            value = toValue;
        }
    }
}
