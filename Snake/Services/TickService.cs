using System.Threading;
using System.Threading.Tasks;

namespace Snake.Services
{
    public delegate void TickDelegate();

    public class TickService
    {
        private event TickDelegate _tickEvent = () => { };

        private int _tickTime = 700;

        public void AddDelegate(TickDelegate func)
        {
            _tickEvent += func;
        }

        public void ResetTickTime()
        {
            _tickTime = 700;
        }

        public void ReduceTickTime()
        {
            if (_tickTime > 250)
                _tickTime -= 10;
        }

        public void StartTickTask(CancellationToken cancellationToken)
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    _tickEvent();

                    await Task.Delay(_tickTime);
                }
            }, cancellationToken);
        }
    }
}
