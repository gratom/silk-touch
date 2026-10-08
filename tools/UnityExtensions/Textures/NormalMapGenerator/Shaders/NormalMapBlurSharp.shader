// Ported from NormalMap-Online's HorizontalBlurShader.js / VerticalBlurShader.js
// (https://github.com/cpetry/NormalMap-Online) - 9-tap separable Gaussian (sigma 2.7).
// Copyright (c) 2014 Christian Petry, MIT License. See NormalMapSettings.cs for full license text.
Shader "Hidden/NormalMapGenerator/BlurSharp"
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

            // Blur/Sharp amount, already divided down to a UV-space step by the caller.
            // amount == 0 -> no-op, amount < 0 -> blur, amount > 0 -> sharpen (unsharp mask).
            float _Amount;
            float _Axis;      // 0 = horizontal (x), 1 = vertical (y)
            float _Seamless;  // 0 / 1

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

            fixed4 frag(v2f i) : SV_Target
            {
                float h = abs(_Amount);
                float2 dir = (_Axis > 0.5) ? float2(0, 1) : float2(1, 0);

                fixed4 sum = fixed4(0, 0, 0, 0);
                sum += tex2D(_MainTex, WrapUV(i.uv - dir * h * 4.0)) * 0.051;
                sum += tex2D(_MainTex, WrapUV(i.uv - dir * h * 3.0)) * 0.0918;
                sum += tex2D(_MainTex, WrapUV(i.uv - dir * h * 2.0)) * 0.12245;
                sum += tex2D(_MainTex, WrapUV(i.uv - dir * h * 1.0)) * 0.1531;
                sum += tex2D(_MainTex, WrapUV(i.uv))                 * 0.1633;
                sum += tex2D(_MainTex, WrapUV(i.uv + dir * h * 1.0)) * 0.1531;
                sum += tex2D(_MainTex, WrapUV(i.uv + dir * h * 2.0)) * 0.12245;
                sum += tex2D(_MainTex, WrapUV(i.uv + dir * h * 3.0)) * 0.0918;
                sum += tex2D(_MainTex, WrapUV(i.uv + dir * h * 4.0)) * 0.051;

                if (_Amount > 0.0)
                {
                    // Unsharp mask: push the pixel away from its blurred version.
                    fixed4 src = tex2D(_MainTex, WrapUV(i.uv));
                    sum = src + src - sum;
                }

                return sum;
            }
            ENDCG
        }
    }
}
