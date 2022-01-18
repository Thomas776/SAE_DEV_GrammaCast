
using System;

namespace GrammaCast
{
    public class Timer
    {
        private float tick;
        private float maxTick;

        public Timer(float maxTick)
        {
            MaxTick = maxTick;
        }
        public bool AddTick(float deltaSeconds)
        {
            this.Tick += deltaSeconds;
            if (this.Tick >= this.MaxTick)
            {
                return false;
            }
            else return true;
        }
        public float Tick
        {
            get => tick;
            set => tick = value;
        }
        public float MaxTick
        {
            get => maxTick;
            set => maxTick = value;
        }
    }
}
