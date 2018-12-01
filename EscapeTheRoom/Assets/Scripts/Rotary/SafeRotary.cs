
namespace VREscape
{
    public class SafeRotary : Rotary
    {
        public int LastState;

        public override void Start()
        {
            flips = true;
            base.Start();
            LastState = StartValue;
        }


        public override void Update()
        {
            LastState = CurrentState;
            base.Update();
        }

        public bool HasTurned()
        {
            return LastState != CurrentState;
        }
    }
}
