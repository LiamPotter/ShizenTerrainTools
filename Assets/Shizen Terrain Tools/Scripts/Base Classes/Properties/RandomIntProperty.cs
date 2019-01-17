namespace Shizen.Base.Properties
{
    [System.Serializable]
    public class RandomIntProperty : Property
    {
        private int min;
        private int max;
        public int Min { get { return min; } }
        public int Max { get { return max; } }
        public virtual void SetMinMax(int min, int max)
        {
            this.min = min;
            this.max = max;
        }
    }
}
