using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Shizen
{
    public class SimplexNoise
    {
        private static int[] noiseHash = {
        151,160,137, 91, 90, 15,131, 13,201, 95, 96, 53,194,233,  7,225,
        140, 36,103, 30, 69,142,  8, 99, 37,240, 21, 10, 23,190,  6,148,
        247,120,234, 75,  0, 26,197, 62, 94,252,219,203,117, 35, 11, 32,
         57,177, 33, 88,237,149, 56, 87,174, 20,125,136,171,168, 68,175,
         74,165, 71,134,139, 48, 27,166, 77,146,158,231, 83,111,229,122,
         60,211,133,230,220,105, 92, 41, 55, 46,245, 40,244,102,143, 54,
         65, 25, 63,161,  1,216, 80, 73,209, 76,132,187,208, 89, 18,169,
        200,196,135,130,116,188,159, 86,164,100,109,198,173,186,  3, 64,
         52,217,226,250,124,123,  5,202, 38,147,118,126,255, 82, 85,212,
        207,206, 59,227, 47, 16, 58, 17,182,189, 28, 42,223,183,170,213,
        119,248,152,  2, 44,154,163, 70,221,153,101,155,167, 43,172,  9,
        129, 22, 39,253, 19, 98,108,110, 79,113,224,232,178,185,112,104,
        218,246, 97,228,251, 34,242,193,238,210,144, 12,191,179,162,241,
         81, 51,145,235,249, 14,239,107, 49,192,214, 31,181,199,106,157,
        184, 84,204,176,115,121, 50, 45,127,  4,150,254,138,236,205, 93,
        222,114, 67, 29, 24, 72,243,141,128,195, 78, 66,215, 61,156,180,

        151,160,137, 91, 90, 15,131, 13,201, 95, 96, 53,194,233,  7,225,
        140, 36,103, 30, 69,142,  8, 99, 37,240, 21, 10, 23,190,  6,148,
        247,120,234, 75,  0, 26,197, 62, 94,252,219,203,117, 35, 11, 32,
         57,177, 33, 88,237,149, 56, 87,174, 20,125,136,171,168, 68,175,
         74,165, 71,134,139, 48, 27,166, 77,146,158,231, 83,111,229,122,
         60,211,133,230,220,105, 92, 41, 55, 46,245, 40,244,102,143, 54,
         65, 25, 63,161,  1,216, 80, 73,209, 76,132,187,208, 89, 18,169,
        200,196,135,130,116,188,159, 86,164,100,109,198,173,186,  3, 64,
         52,217,226,250,124,123,  5,202, 38,147,118,126,255, 82, 85,212,
        207,206, 59,227, 47, 16, 58, 17,182,189, 28, 42,223,183,170,213,
        119,248,152,  2, 44,154,163, 70,221,153,101,155,167, 43,172,  9,
        129, 22, 39,253, 19, 98,108,110, 79,113,224,232,178,185,112,104,
        218,246, 97,228,251, 34,242,193,238,210,144, 12,191,179,162,241,
         81, 51,145,235,249, 14,239,107, 49,192,214, 31,181,199,106,157,
        184, 84,204,176,115,121, 50, 45,127,  4,150,254,138,236,205, 93,
        222,114, 67, 29, 24, 72,243,141,128,195, 78, 66,215, 61,156,180
    };

        private const int noiseHashMask = 255;

        private static Vector3[] noiseGradients =
        {
            new Vector3( 1f, 1f, 0f),
            new Vector3(-1f, 1f, 0f),
            new Vector3( 1f,-1f, 0f),
            new Vector3(-1f,-1f, 0f),
            new Vector3( 1f, 0f, 1f),
            new Vector3(-1f, 0f, 1f),
            new Vector3( 1f, 0f,-1f),
            new Vector3(-1f, 0f,-1f),
            new Vector3( 0f, 1f, 1f),
            new Vector3( 0f,-1f, 1f),
            new Vector3( 0f, 1f,-1f),
            new Vector3( 0f,-1f,-1f),

            new Vector3( 1f, 1f, 0f),
            new Vector3(-1f, 1f, 0f),
            new Vector3( 0f,-1f, 1f),
            new Vector3( 0f,-1f,-1f)
        };

        private const int noiseGradientsMask = 15;

        private static float sqr2 = Mathf.Sqrt(2);

        public static float BaseValue(Vector3 _point, float _frequency)
        {
           
            _point *= _frequency;
            int ix0 = Mathf.FloorToInt(_point.x);
            int iy0 = Mathf.FloorToInt(_point.y);
            int iz0 = Mathf.FloorToInt(_point.z);
            float tx0 = _point.x - ix0;
            float ty0 = _point.y - iy0;
            float tz0 = _point.z - iz0;
            float tx1 = tx0 - 1f;
            float ty1 = ty0 - 1f;
            float tz1 = tz0 - 1f;
            ix0 &= noiseHashMask;
            iy0 &= noiseHashMask;
            iz0 &= noiseHashMask;
            int ix1 = ix0 + 1;
            int iy1 = iy0 + 1;
            int iz1 = iz0 + 1;

            int h0 = noiseHash[ix0];
            int h1 = noiseHash[ix1];
            int h00 = noiseHash[h0 + iy0];
            int h10 = noiseHash[h1 + iy0];
            int h01 = noiseHash[h0 + iy1];
            int h11 = noiseHash[h1 + iy1];

            Vector3 g000 = noiseGradients[noiseHash[h00 + iz0] & noiseGradientsMask];
            Vector3 g100 = noiseGradients[noiseHash[h10 + iz0] & noiseGradientsMask];
            Vector3 g010 = noiseGradients[noiseHash[h01 + iz0] & noiseGradientsMask];
            Vector3 g110 = noiseGradients[noiseHash[h11 + iz0] & noiseGradientsMask];
            Vector3 g001 = noiseGradients[noiseHash[h00 + iz1] & noiseGradientsMask];
            Vector3 g101 = noiseGradients[noiseHash[h10 + iz1] & noiseGradientsMask];
            Vector3 g011 = noiseGradients[noiseHash[h01 + iz1] & noiseGradientsMask];
            Vector3 g111 = noiseGradients[noiseHash[h11 + iz1] & noiseGradientsMask];

            float v000 = DotProduct(g000, tx0, ty0, tz0);
            float v100 = DotProduct(g100, tx1, ty0, tz0);
            float v010 = DotProduct(g010, tx0, ty1, tz0);
            float v110 = DotProduct(g110, tx1, ty1, tz0);
            float v001 = DotProduct(g001, tx0, ty0, tz1);
            float v101 = DotProduct(g101, tx1, ty0, tz1);
            float v011 = DotProduct(g011, tx0, ty1, tz1);
            float v111 = DotProduct(g111, tx1, ty1, tz1);

            float tx = SmoothFloat(tx0);
            float ty = SmoothFloat(ty0);
            float tz = SmoothFloat(tz0);
            return Mathf.Lerp(
                Mathf.Lerp(Mathf.Lerp(v000, v100, tx), Mathf.Lerp(v010, v110, tx), ty),
                Mathf.Lerp(Mathf.Lerp(v001, v101, tx), Mathf.Lerp(v011, v111, tx), ty),
                tz);

        }

        public static float SampleSum(Vector3 _point, float _frequency ,float _amplitude,float _octaves,float _lacunarity, float _persistance)
        {
            float _sum = BaseValue(_point, _frequency);
            float _range = 1f;
            for (int o = 0; o < _octaves; o++)
            {
                _frequency *= _lacunarity;
                _amplitude *= _persistance;
                _range += _amplitude;
                _sum += BaseValue(_point, _frequency) * _amplitude;
            }
            return _sum / _range;
        }

        private static float SmoothFloat(float _value)
        {
            return _value * _value * _value * (_value * (_value * 6f - 15f) + 10f);
        }
        private static float DotProduct(Vector3 _vectorInput, float x, float y,float z)
        {
            return _vectorInput.x * x + _vectorInput.y * y+_vectorInput.z*z;
        }

    }
}