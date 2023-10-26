using System;
namespace OrgnalR.Core
{
    public class OrgnalRSiloConfig
    {
        private int maxMessageRewind = 10;
        private TimeSpan persistenceInterval = TimeSpan.FromSeconds(30);
        private bool persistenceEnabled = true;

        public int MaxMessageRewind
        {
            get => maxMessageRewind; set
            {
                if (value < 0)
                {
                    throw new ArgumentException($"{nameof(MaxMessageRewind)} must not be less than 0! Provided [{value}]");
                }
                maxMessageRewind = value;
            }
        }

        public TimeSpan PerstenceInterval
        {
            get => persistenceInterval; set
            {
                if (value < TimeSpan.Zero)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(PerstenceInterval)} most be TimeSpan.Zero or larger! Provided [{value}]");
                }
                persistenceInterval = value;
            }
        }

        public bool PersistenceEnabled
        {
            get => persistenceEnabled; set => persistenceEnabled = value;
        }
    }
}
