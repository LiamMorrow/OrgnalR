using System;
namespace OrgnalR.Core
{
    public class OrgnalRSiloConfig
    {
        private int maxMessageRewind = 10;

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
    }
}
