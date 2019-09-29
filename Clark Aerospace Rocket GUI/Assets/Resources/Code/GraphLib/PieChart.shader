// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "UI/Graph/Pie Chart"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}

        _Color ("Color", Color) = (1,1,1,1)

        _DonutThickness ("Donut thickness", Range(0,0.5)) = 0

        _Mult ("Mult", Range(0, 6.283185307179586476924)) = 0
        _Add ("Add", Range(0, 6.283185307179586476924)) = 0

        _StartAngle ("Start Angle", Range(0, 6.283185307179586476924)) = 0
        _EndAngle ("End Angle", Range(0, 6.283185307179586476924)) = 6.283185307179586476924




        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "Default"
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;

            float _DonutThickness;
            fixed4 _Color;

            float _StartAngle, _EndAngle;

            float _Mult, _Add;

            const float pi = 3.141592653589793238462;
            const float pi2 = 6.283185307179586476924;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

                OUT.texcoord = v.texcoord;

                OUT.color = v.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {

                //color.rgb = IN.texcoord.y;

                IN.texcoord.x -= 0.5;
                IN.texcoord.y -= 0.5;

                float r = sqrt(IN.texcoord.x * IN.texcoord.x + IN.texcoord.y * IN.texcoord.y);
                float t = atan2(IN.texcoord.x, IN.texcoord.y) + 3.14159265358;

                fixed4 color = (0,0,0,1);

                //color.rgb = r;

                int a = (int)(r <= 0.5 && r >= _DonutThickness && t >= _StartAngle && t <= _EndAngle);
                float dd = 1-((ddx(a) + ddy(a)) * 0.5);

                color.rgb = a;
                color.a *= dd;

                color.a *= color.r;

                color.rgb = _Color;

                //color.rgb = t;


                #ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip (color.a - 0.001);
                #endif

                //color.rgb = ddy(IN.texcoord.x);
                return color;
            }
        ENDCG
        }
    }
}
