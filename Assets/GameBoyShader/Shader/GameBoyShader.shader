Shader "Retro/GameBoyEffect"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _DarkestColor("Darkest Color", Color) = (0, 0, 0, 1.0)
        _DarkColor("Dark Color", Color) = (0, 0, 0, 1.0)
        _LightColor("Light Color", Color) = (0, 0, 0, 1.0)
        _LightestColor("Lightest Color", Color) = (0, 0, 0, 1.0)
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _DarkestColor;
            float4 _DarkColor;
            float4 _LightColor;
            float4 _LightestColor;


            // Based on smo
            fixed4 frag(v2f_img i) : SV_Target
            {
                fixed4 tex = tex2D(_MainTex, i.uv);
                float lumaCalc = dot(tex, float3(0.3, 0.59, 0.11));

                int gbIff = lumaCalc * 3;
                float3 colors = lerp(_DarkestColor, _DarkColor, saturate(gbIff));
                colors = lerp(colors, _LightColor, saturate(gbIff - 1.0));
                colors = lerp(colors, _LightestColor, saturate(gbIff - 2.0));

                // RGBA
                return float4(colors, 1.0);
            }
            ENDCG
        }
    }
}