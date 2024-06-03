using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NRand
{
    class UniformVector2AnnulusDistribution
    {
        UniformFloatDistribution _dAngle = new UniformFloatDistribution(0, 2 * Mathf.PI);
        UniformFloatDistribution _dRadius = new UniformFloatDistribution(0, 1);
        float _minRadius;
        float _maxRadius;

        public UniformVector2AnnulusDistribution()
        {
            _minRadius = 1.0f;
            _maxRadius = 2.0f;
        }

        public UniformVector2AnnulusDistribution(float min, float max)
        {
            _minRadius = min;
            _maxRadius = max;
        }

        public void SetParams(float min = 1.0f, float max = 2.0f)
        {
            _minRadius = min;
            _maxRadius = max;
        }

        public Vector2 Max()
        {
            return new Vector2(_maxRadius, 0);
        }

        public Vector2 Min()
        {
            return new Vector2(_minRadius, 0);
        }

        public Vector2 Next(IRandomGenerator generator)
        {
            float r = _dRadius.Next(generator);
            float radius = Mathf.Sqrt(r * _maxRadius * _maxRadius + (1 - r) * _minRadius * _minRadius);

            float angle = _dAngle.Next(generator);
            return new Vector3(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle));
        }
    }
}
