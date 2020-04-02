using System;

namespace DaemonStageDispatcher
{
    class StagesCycleException : Exception
    {
        public StagesCycleException()
        {
        }

        public StagesCycleException(string message)
            : base(message)
        {
        }

        public StagesCycleException(string message, Exception inner)
            : base(message, inner)
        {
        }

    }
}
