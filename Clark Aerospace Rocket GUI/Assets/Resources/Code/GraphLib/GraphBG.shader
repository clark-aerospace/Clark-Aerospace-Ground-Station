Shader "Unlit/GraphBG"
{
    Properties
    {
        _ColTop ("Color top", Color) = (1,1,1,1)
        _ColBottom ("Color bottom", Color) = (0,0,0,1)
        _HeightInc ("Height div", Float) = 75.0
        _HeightOffset ("Height offset", Float) = 0.3
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent"}
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha 

        Pass
        {
            Cull off
            
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
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float height : TEXCOORD1;

                float4 color : COLOR;
            };

            float4 _ColBottom;
            float4 _ColTop;
            float _HeightInc;
            float _HeightOffset;

            v2f vert (appdata v)
            {
                v2f o;
                o.height = -v.vertex.y;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                //o.height = o.vertex.y;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                //fixed4 col = lerp(_ColTop, _ColBottom, clamp((i.height / _HeightInc) + _HeightOffset, 0.0, 1.0));

                fixed4 col = i.color;
                //col.rgb = i.screenPos;
                //col.r = i.screenPos.x;
                //col.a = 1;
                //col.a = clamp((i.height / _HeightInc) + _HeightOffset, 0.0, 1.0);
                return col;
            }
            ENDCG
        }
    }
}
