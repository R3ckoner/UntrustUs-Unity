Shader "Custom/RippleShaderWithTransparency"
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "white" {}
        _RippleSpeed("Ripple Speed", Range(0, 10)) = 1
        _RippleFrequency("Ripple Frequency", Range(0, 10)) = 1
        _RippleAmplitude("Ripple Amplitude", Range(0, 1)) = 0.1
        _Tiling("Texture Tiling", Range(0.1, 10)) = 1  // Adjusted the range for tiling
        _TextureScale("Texture Scale", Range(0.1, 10)) = 1  // New property for texture scaling
        _Transparency("Transparency", Range(0, 1)) = 1  // Transparency slider
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _RippleSpeed;
            float _RippleFrequency;
            float _RippleAmplitude;
            float _Tiling;
            float _TextureScale;
            float _Transparency;

            v2f vert(appdata_t v)
            {
                v2f o;

                float3 offset = float3(
                    _RippleAmplitude * sin(_Time.y * _RippleSpeed + v.vertex.x * _RippleFrequency),
                    _RippleAmplitude * sin(_Time.y * _RippleSpeed + v.vertex.y * _RippleFrequency),
                    _RippleAmplitude * sin(_Time.y * _RippleSpeed + v.vertex.z * _RippleFrequency)
                );

                offset += _RippleAmplitude * sin(_Time.y * _RippleSpeed + (v.vertex.x + v.vertex.y) * _RippleFrequency);

                // Add second sine wave in the opposite direction
                offset -= float3(
                    _RippleAmplitude * sin(_Time.y * _RippleSpeed + v.vertex.z * _RippleFrequency),
                    _RippleAmplitude * sin(_Time.y * _RippleSpeed + v.vertex.x * _RippleFrequency),
                    _RippleAmplitude * sin(_Time.y * _RippleSpeed + v.vertex.y * _RippleFrequency)
                );

                offset -= _RippleAmplitude * sin(_Time.y * _RippleSpeed + (v.vertex.x + v.vertex.z) * _RippleFrequency);

                o.uv = v.uv * _Tiling * _TextureScale;
                o.vertex = UnityObjectToClipPos(v.vertex + float4(offset, 0));
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                col.a *= _Transparency;  // Apply transparency
                return col;
            }
            ENDCG
        }
    }
}
