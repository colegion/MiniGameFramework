using UnityEngine;

namespace Interfaces
{
    public interface IBotStrategy
    {
        public void InjectBot(Bot bot);
        public void Play();
    }
}
