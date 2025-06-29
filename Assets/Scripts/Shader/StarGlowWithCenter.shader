Shader "Custom/StarGlowWithCenter"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _GlowColor ("Glow Color", Color) = (1,1,1,1)
        _GlowIntensity ("Glow Intensity", Float) = 2.0
        _PulseSpeed ("Pulse Speed", Float) = 2.0
        _Center ("Glow Center (UV)", Vector) = (0.5, 0.5, 0, 0) // 中心位置
        _Range ("Glow Range", Float) = 0.3                         // 发光范围
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            ZWrite Off
            Blend SrcAlpha One
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _GlowColor;
            float _GlowIntensity;
            float _PulseSpeed;
            float4 _Center;
            float _Range;
            float4 _MainTex_ST;

            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // 计算当前像素到发光中心的距离
                float dist = distance(uv, _Center.xy);

                // 高斯型发光衰减
                float glow = exp(-pow(dist / _Range, 2));

                // 闪烁脉冲
                float pulse = 0.5 + 0.5 * sin(_Time.y * _PulseSpeed);
                glow *= (1.0 + pulse * _GlowIntensity);

                // 获取主纹理颜色
                fixed4 col = tex2D(_MainTex, uv);

                // 加上发光
                fixed4 final = col + _GlowColor * glow;
                final.a = col.a;

                return final;
            }
            ENDCG
        }
    }
}
