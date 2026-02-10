Shader "Custom/SpineGradientY"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _HeadColor ("Head Color", Color) = (1,0,0,1)
        _BodyY ("Body Y", Float) = 0
        _GradientHeight ("Gradient Height", Float) = 1
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend One OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
                float4 vertex : SV_POSITION;
                float worldY : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _HeadColor;
            float _BodyY;
            float _GradientHeight;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                o.worldY = mul(unity_ObjectToWorld, v.vertex).y;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;

                float t = saturate((i.worldY - _BodyY) / _GradientHeight);
                col.rgb = lerp(col.rgb, _HeadColor.rgb, t);

                return col;
            }
            ENDCG
        }
    }
}
