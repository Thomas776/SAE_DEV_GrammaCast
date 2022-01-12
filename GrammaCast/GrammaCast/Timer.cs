using System;
using System.Collections.Generic;
using System.Text;

namespace GrammaCast
{
    class Timer
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
            private set => tick = value;
        }
        public float MaxTick
        {
            get => maxTick;
            private set => maxTick = value;
        }
    }
}
