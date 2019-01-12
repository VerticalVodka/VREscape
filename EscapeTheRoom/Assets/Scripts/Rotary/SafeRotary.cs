
namespace VREscape
{
    public class SafeRotary : Rotary
    {
        public int LastState;

        public void Start()
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
