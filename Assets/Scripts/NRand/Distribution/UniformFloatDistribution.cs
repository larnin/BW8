﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NRand
{
    public class UniformFloatDistribution : IRandomDistribution<float>
    {
        private float _minValue;
        private float _maxValue;

        public UniformFloatDistribution()
        {
            _minValue = 0.0f;
            _maxValue = 1.0f;
        }

        public UniformFloatDistribution(float max)
        {
            _minValue = Math.Min(0, max);
            _maxValue = Math.Max(0, max);
        }

        public UniformFloatDistribution(float min, float max)
        {
            _minValue = Math.Min(min, max);
            _maxValue = Math.Max(min, max);
        }

        public void SetParams(float max = 1.0f)
        {
            _minValue = 0.0f;
            _maxValue = max;
        }

        public void SetParams(float min, float max)
        {
            _minValue = min;
            _maxValue = max;
        }

        public float Max()
        {
            return _maxValue;
        }

        public float Min()
        {
            return _minValue;
        }

        public float Next(IRandomGenerator generator)
        {
            return ((float)generator.Next() - generator.Min()) / (generator.Max() - generator.Min()) * (_maxValue - _minValue) + _minValue;
        }
    }
}
