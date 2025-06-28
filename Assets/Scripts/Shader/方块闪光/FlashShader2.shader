Shader "Unlit/FlashShader2"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _DistortCenter ("Distort Center (UV)", Vector) = (0.5, 0.5, 0, 0)
        _ZoomStrength ("Zoom Strength", Range(1, 3)) = 1.0
        _DistortStrength ("Distort Strength", Range(0, 0.3)) = 0.1
        _Color ("Tint", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 _DistortCenter;
            float _ZoomStrength;
            float _DistortStrength;
            fixed4 _Color;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 dir = i.uv - _DistortCenter.xy;
                float dist = length(dir);

                // 放大基础（线性）
                float2 uvZoom = _DistortCenter.xy + dir / _ZoomStrength;

                // 非线性扰动（扭曲边缘）
                float distort = sin(dist * 20 - _Time.y * 10) * _DistortStrength;
                float2 offset = normalize(dir) * distort;

                float2 finalUV = uvZoom + offset;

                fixed4 col = tex2D(_MainTex, finalUV) * i.color * _Color;
                return col;
            }
            ENDCG
        }
    }
}
