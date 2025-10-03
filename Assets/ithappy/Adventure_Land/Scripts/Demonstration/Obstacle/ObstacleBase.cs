using System;
using UnityEngine;

namespace ithappy.Adventure_Land
{
    [Serializable]
    public class ObstacleInfo
    {
    }
    
    public abstract class ObstacleBase : MonoBehaviour
    {
        public abstract Transform StartPoint { get; }
        public abstract Transform EndPoint { get; }
        
        public abstract ObstacleInfo ObstacleInfo { get; }
        public abstract ObstacleType ObstacleType { get; }
    }
}
