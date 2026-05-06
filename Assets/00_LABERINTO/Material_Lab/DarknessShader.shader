Shader "Custom/DarknessShader"
{
    Properties
    {
        _PlayerPosition("Player Position", Vector) = (0, 0, 0, 0)
        _Radius("Radius", Float) = 1.0
        _Color("Color", Color) = (0, 0, 0, 1)
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Overlay" "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            float4 _PlayerPositions[6];
            float _PlayerRadii[6];
            float4 _Color;

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.worldPos = TransformObjectToWorld(IN.positionOS.xyz);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float minDist = 999999;
                int closestIndex = 0;
                
                for (int i = 0; i < 6; i++)
                {
                    float dist = distance(IN.worldPos.xy, _PlayerPositions[i].xy);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        closestIndex = i;
                    }
                }
                
                float alpha = minDist > _PlayerRadii[closestIndex] ? 1.0 : 0.0;
                return half4(_Color.rgb, alpha);
            }
            ENDHLSL
        }
    }
}