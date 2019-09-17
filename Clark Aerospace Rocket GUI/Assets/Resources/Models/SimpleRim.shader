// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/SimpleRim"
{
    Properties
    {
        _RimColor ("Rim color", Color) = (1,1,1,1)
        _InnerColor("Inner color", Color) = (0,0,0,0)

        _RimPower ("Rim Power", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                float2 viewDir : TEXCOORD1;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
                float2 viewDir : TEXCOORD1;
            };

            float4 _InnerColor, _RimColor;
            float _RimPower;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.normal = v.normal;
                o.viewDir = v.viewDir;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);

                fixed4 col;
                float3 viewPos = normalize(UnityObjectToViewPos(i.vertex));
                float3 worldNorm = UnityObjectToWorldNormal(i.normal);
                float3 viewNorm = mul((float3x3)UNITY_MATRIX_V, worldNorm);

                float dotP = abs(dot(viewNorm, viewPos));
                col.rgb = dotP;
                return col;
            }
            ENDCG
        }
    }
}
