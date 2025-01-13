//
// Copyright 2023 Sony Corporation
//
using UnityEngine;

namespace Mocopi.Sensor.DataReader.Sample
{
    public class GridObjectCollection : MonoBehaviour
    {
        public int Rows = 2;
        public int Columns = 3;
        public Vector2 CellSize = new Vector2(0.1f, 0.1f);
        public Vector2 CellSpacing = new Vector2(0.02f, 0.02f);

        private void Start()
        {
            UpdateCollection();
        }

        public void UpdateCollection()
        {
            var childCount = transform.childCount;

            float gridWidth = (Columns * CellSize.x) + ((Columns - 1) * CellSpacing.x);
            float gridHeight = (Rows * CellSize.y) + ((Rows - 1) * CellSpacing.y);
            var startPosition = new Vector3(-gridWidth / 2 + CellSize.x / 2, gridHeight / 2 - CellSize.y / 2, 0);

            for (int i = 0; i < childCount; i++)
            {
                int row = i / Columns;
                int column = i % Columns;

                if (row < Rows)
                {
                    var child = transform.GetChild(i);
                    var position = startPosition + new Vector3(column * (CellSize.x + CellSpacing.x), -row * (CellSize.y + CellSpacing.y), 0);
                    child.localPosition = position;
                }
            }
        }
    }
}
