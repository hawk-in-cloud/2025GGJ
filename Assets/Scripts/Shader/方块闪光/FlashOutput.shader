Shader "Unlit/FlashOutput"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _ExpandCenter ("Expand Center (UV)", Vector) = (0.5, 0.5, 0, 0)
        _ExpandStrength ("Expand Strength", Range(0, 1)) = 0.2
        _Color ("Tint", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        Lighting Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _ExpandCenter;
            float _ExpandStrength;
            fixed4 _Color;
            float _RippleFreq;
            float _RippleSpeed;
            float _RippleStrength;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 dir = i.uv - _ExpandCenter.xy;
                float dist = length(dir);

                // dir: ��ǰ����ָ�����ĵ�����
                // dist: ��ǰ���������ĵľ���

                // ʹ�� atan ʵ�ַ����� UV ���죨��Ե���͸����ң�
                float expandFactor = atan(dist * _ExpandStrength * 10.0) * 0.5 / atan(_ExpandStrength * 5.0);

                // ��� sin ����ģ���Ŷ�
                float ripple = sin(dist * _RippleFreq - _Time.y * _RippleSpeed) * _RippleStrength;

                // �������� = ���� + ���� + ����
                float2 distortedUV = _ExpandCenter.xy + normalize(dir) * (expandFactor + ripple);


                fixed4 col = tex2D(_MainTex, distortedUV) * i.color * _Color;
                return col;
            }
            ENDCG
        }
    }
}
