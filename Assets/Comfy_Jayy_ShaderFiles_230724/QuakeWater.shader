Shader "Custom/QuakeWater" {
    Properties{
        _MainTex("Water Texture", 2D) = "white" {}
        _Speed("Speed", Range(0, 10)) = 1
        _Amplitude("Amplitude", Range(0, 1)) = 0.1
        _Frequency("Frequency", Range(0, 10)) = 1
        _TextureScale("Texture Scale", Range(0.1, 10)) = 1
        _Transparency("Transparency", Range(0, 1)) = 1 // New property for transparency slider
    }

    SubShader{
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        LOD 100

        Pass {
            Blend SrcAlpha OneMinusSrcAlpha // Add this blend mode for transparency
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_fog
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 screenPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float _Speed;
            float _Amplitude;
            float _Frequency;
            float _TextureScale;
            float _Transparency; // New property for transparency slider

            v2f vert(appdata_t v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv * _TextureScale;
                o.screenPos = ComputeScreenPos(o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target {
                float2 uv = i.uv;
                // Sine wave along the X-axis
                uv.x += _Amplitude * sin(_Time.y * _Speed + uv.y * _Frequency);
                // Sine wave along the Y-axis
                uv.y += _Amplitude * sin(_Time.y * _Speed + uv.x * _Frequency);
                
                fixed4 texColor = tex2D(_MainTex, uv);
                texColor.a *= _Transparency; // Adjust alpha based on the transparency slider
                return texColor;
            }
            ENDCG
        }
    }
}
