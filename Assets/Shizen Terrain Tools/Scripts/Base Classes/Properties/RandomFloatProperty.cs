namespace Shizen.Base.Properties
{
    [System.Serializable]
    public class RandomFloatProperty : Property
    {
        private float min;
        private float max;
        public float Min { get { return min; } }
        public float Max { get { return max; } }
        public virtual void SetMinMax(float min, float max)
        {
            this.min = min;
            this.max = max;
        }      
    }
}
