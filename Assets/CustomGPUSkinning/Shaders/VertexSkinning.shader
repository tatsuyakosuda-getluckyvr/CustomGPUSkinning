Shader "Avatar/Sandbox/VertexSkinning"
{
    Properties
    { 
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Color ("BaseColor", Color) = (1, 1, 1, 1)
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float2 uv : TEXCOORD0;
                float4 positionOS   : POSITION;
            };

            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float4 positionHCS  : SV_POSITION;
            };

            struct SVertInSkin
            {
                float weight0,weight1,weight2,weight3;
                int index0,index1,index2,index3;
            };

            StructuredBuffer<SVertInSkin> _Skin;
            StructuredBuffer<float4x4> _Bones;

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_ST;
            half4 _Color;
            CBUFFER_END

            Varyings vert(Attributes IN, uint vIdx : SV_VertexID)
            {
                SVertInSkin si = _Skin[vIdx];

                float3 vP = IN.positionOS.xyz;
                float3 vPacc = float3(0, 0, 0);

                vPacc += si.weight0*mul(_Bones[si.index0], float4(vP, 1)).xyz;
                vPacc += si.weight1*mul(_Bones[si.index1], float4(vP, 1)).xyz;
                vPacc += si.weight2*mul(_Bones[si.index2], float4(vP, 1)).xyz;
                vPacc += si.weight3*mul(_Bones[si.index3], float4(vP, 1)).xyz;

                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(vPacc);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv) * _Color;
                return col;
            }
            ENDHLSL
        }
    }
}