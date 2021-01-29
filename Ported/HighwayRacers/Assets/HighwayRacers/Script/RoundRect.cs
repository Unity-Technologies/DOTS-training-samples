using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor.Build.Pipeline;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.WSA.Input;
using Random = Unity.Mathematics.Random;

public class RoundRect
{
    private static RoundRect _Singleton;
    public static RoundRect Instance
    {
        get
        {
            if (_Singleton == null)
            {
                _Singleton = new RoundRect(128, 32);
            }

            return _Singleton;
        }
    }
    
    private float _perimeter;
    private float _straight;
    private float _curved;
    private float _size;
    private float _radius;
    public float Size {
        get
        {
            return _size;
        }
        
        set
        {
            _size = value;
            UpdatePerimeter();
        } 
    }

    public float Radius
    {
        get { return _radius; }
        set
        {
            _radius = value;
            UpdatePerimeter();
        }
    }

    public float Perimeter
    {
        get => _perimeter;
    }
    
    
    private RoundRect(float size, float radius)
    {
        Size = size;
        Radius = radius;
    }

    private void UpdatePerimeter()
    {
        _curved = Mathf.PI * _radius / 2.0f;
        _straight = _size - (_curved *2);
        _perimeter = 4 * (_straight + _curved);
    }

    public RectData GetRectData()
    {
        return new RectData()
        {
            Size = _size,
            Straight = _straight,
            Curved = _curved,
            Perimeter = _perimeter,
            Radius = _radius,
        };
    }
    
    public struct RectData
    {
        public float Size;
        public float Straight;
        public float Curved;
        public float Perimeter;
        public float Radius;
    }
    
    
    public static void Interpolate(in float coord, in RectData rectData, out float3 pos, out float yaw)
    {
        float R =  rectData.Curved / rectData.Size;
        float total = rectData.Straight + rectData.Curved;
        float tls = math.saturate(rectData.Straight/total);
        float tlr = math.saturate(rectData.Curved/total);

        int q = (int)(coord * 4.0f);

        float x = 0;
        float y = 0;
        float a = 0;

        if(q == 0)
        {
            float n = coord * 4.0f;
            x = R;
            y = math.lerp(R, 1.0f - R, math.saturate(n/tls));

            a = 0.5f * math.PI * math.saturate((n - tls)/tlr);
            x -= math.cos(a) * R;
            y += math.sin(a) * R;
        }
        else if(q == 1)
        {
            float n = (coord - 0.25f) * 4.0f;
            y = 1.0f - R;
            x = math.lerp(R, 1.0f - R, math.saturate(n/tls));

            a = 0.5f * math.PI * math.saturate((n - tls)/tlr);
            y += math.cos(a) * R;
            x += math.sin(a) * R;
            a += math.PI/2.0f;
        }
        else if(q == 2)
        {
            float n = (coord - 0.5f) * 4.0f;
            x = 1.0f - R;
            y = math.lerp(1.0f - R, R, math.saturate(n/tls));

            a = 0.5f * math.PI * math.saturate((n - tls)/tlr);
            x += math.cos(a) * R;
            y -= math.sin(a) * R;
            a -= math.PI;
        }
        else
        {
            float n = (coord - 0.75f) * 4.0f;
            y = R;
            x = math.lerp(1.0f - R, R, math.saturate(n/tls));

            a = 0.5f * math.PI * math.saturate((n - tls)/tlr);
            y -= math.cos(a) * R;
            x -= math.sin(a) * R;
            a -= math.PI/2.0f;
        }

        x -= 0.5f;
        y -= 0.5f;
        x *= rectData.Size;
        y *= rectData.Size;
        
        pos.x = x;
        pos.z = y;
        pos.y = math.frac(coord) * .01f; // to prevent zfighting
        yaw = a;
    }

    static void InterpolateRealDist(in float realDistance, in RectData rectData, out float3 pos, out float yaw)
    {
        // the divisor here is effectively a global speed multiplier
        // the 4 is eyeballed
        float normalized = realDistance / (rectData.Perimeter / 4f);
        
        Interpolate(normalized % 1f, rectData, out pos, out yaw);
    }

    public static void InterpolateRoundRectRealDist(in float coord, in RectData rectData, out float3 pos, out float yaw)
    {
        InterpolateRealDist(in coord, in rectData, out pos,out yaw);
    }



    
}
