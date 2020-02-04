using UnityEngine;

namespace Installers
{
    internal class PositionData
    {
        public float[] position;

        public PositionData()
        {
            position = new float[3];
        }

        public PositionData(Vector3 position)
        {
            this.position = new[] {position.x, position.y, position.z};
        }

        public Vector3 GetPosition()
        {
            return new Vector3(position[0], position[1], position[2]);
        }
    }
}