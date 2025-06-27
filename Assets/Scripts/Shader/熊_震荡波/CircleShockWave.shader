Shader "Custom/CircleShockWave"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _Radius ("Radius", Float) = 0
        _Width ("Width", Float) = 0.1
        _Distortion ("Distortion", Float) = 0.05
        _Center ("Center", Vector) = (0.5,0.5,0,0)
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }

        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off

        GrabPass { "_GrabTex" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _GrabTex;
            float4 _GrabTex_TexelSize;

            fixed4 _Color;
            float _Radius;
            float _Width;
            float _Distortion;
            float4 _Center;

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv       : TEXCOORD0;
                float4 vertex   : SV_POSITION;
                float4 screenUV : TEXCOORD1;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.screenUV = ComputeScreenPos(o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 screenUV = i.screenUV.xy / i.screenUV.w;

                float2 dir = screenUV - _Center.xy;
                float dist = length(dir);

                float wave = smoothstep(_Radius, _Radius - _Width, dist);

                float2 offset = normalize(dir) * wave * _Distortion;
                float2 distortedUV = screenUV + offset;

                fixed4 grab = tex2D(_GrabTex, distortedUV);
                fixed4 tex = tex2D(_MainTex, i.uv) * _Color;

                // ÓÃ Sprite alpha ×öÕÚÕÖ£¬ÈÃ Sprite ÏÔÊ¾·¶Î§ÄÚÅ¤Çú
                return fixed4(grab.rgb, tex.a * wave);
            }
            ENDCG
        }
    }
}
