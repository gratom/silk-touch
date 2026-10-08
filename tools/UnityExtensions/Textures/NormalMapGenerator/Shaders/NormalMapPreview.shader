// Normal map generation math ported from NormalMap-Online (https://github.com/cpetry/NormalMap-Online)
// Copyright (c) 2014 Christian Petry, MIT License. See NormalMapSettings.cs for full license text.
Shader "Hidden/NormalMapGenerator/Preview"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;

            float _Strength;
            float _Level;
            float _FilterType;    // 0 = sobel, 1 = scharr
            float _InvertHeight;  // 0 / 1 - inverts the sampled height before computing the gradient
            float _InvertRed;     // 0 / 1 - flips the X component of the resulting normal
            float _InvertGreen;   // 0 / 1 - flips the Y component of the resulting normal
            float _Seamless;      // 0 / 1
            float _UseAlpha;      // 0 / 1

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float2 WrapUV(float2 uv)
            {
                if (_Seamless > 0.5)
                {
                    return frac(uv);
                }
                return saturate(uv);
            }

            // Height sample in the same 0..255 "byte" domain the original algorithm was tuned for.
            float SampleHeight(float2 uv)
            {
                fixed4 c = tex2D(_MainTex, WrapUV(uv));
                float v = _UseAlpha > 0.5 ? c.a : dot(c.rgb, float3(0.2126, 0.7152, 0.0722));
                v *= 255.0;
                return _InvertHeight > 0.5 ? (255.0 - v) : v;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 texel = _MainTex_TexelSize.xy;

                float tl = SampleHeight(i.uv + texel * float2(-1, -1));
                float l  = SampleHeight(i.uv + texel * float2(-1,  0));
                float bl = SampleHeight(i.uv + texel * float2(-1,  1));
                float t  = SampleHeight(i.uv + texel * float2( 0, -1));
                float b  = SampleHeight(i.uv + texel * float2( 0,  1));
                float tr = SampleHeight(i.uv + texel * float2( 1, -1));
                float r  = SampleHeight(i.uv + texel * float2( 1,  0));
                float br = SampleHeight(i.uv + texel * float2( 1,  1));

                float dX, dY;
                if (_FilterType > 0.5)
                {
                    // scharr
                    dX = tl * 3.0 + l * 10.0 + bl * 3.0 - tr * 3.0 - r * 10.0 - br * 3.0;
                    dY = tl * 3.0 + t * 10.0 + tr * 3.0 - bl * 3.0 - b * 10.0 - br * 3.0;
                }
                else
                {
                    // sobel
                    dX = tl + l * 2.0 + bl - tr - r * 2.0 - br;
                    dY = tl + t * 2.0 + tr - bl - b * 2.0 - br;
                }

                dX *= (_InvertRed > 0.5) ? -1.0 : 1.0;
                dY *= (_InvertGreen > 0.5) ? -1.0 : 1.0;

                float strength = max(_Strength, 0.0001);
                float dZ = 1.0 / strength * (1.0 + pow(2.0, _Level));
                float len = sqrt(dX * dX + dY * dY + dZ * dZ);

                fixed4 original = tex2D(_MainTex, WrapUV(i.uv));
                fixed4 col;
                col.r = dX / len * 0.5 + 0.5;
                col.g = dY / len * 0.5 + 0.5;
                col.b = dZ / len;
                col.a = original.a;
                return col;
            }
            ENDCG
        }
    }
}
