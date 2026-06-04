Shader "Pillage/OutsideMapEdgeFade"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _FadeX ("Horizontal Fade", Range(0.0001, 0.5)) = 0.035
        _FadeY ("Vertical Fade", Range(0.0001, 0.5)) = 0.08
        _ProtectedLeftRatio ("Protected Left Ratio", Range(0, 1)) = 1
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "CanUseSpriteAtlas" = "True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            sampler2D _MainTex;
            fixed4 _Color;
            float _FadeX;
            float _FadeY;
            float _ProtectedLeftRatio;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                o.color = v.color * _Color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 color = tex2D(_MainTex, i.texcoord) * i.color;
                float edgeRight = 1.0 - i.texcoord.x;
                float edgeY = min(i.texcoord.y, 1.0 - i.texcoord.y);
                float fadeX = smoothstep(0.0, 1.0, saturate(edgeRight / max(_FadeX, 0.0001)));
                float fadeY = smoothstep(0.0, 1.0, saturate(edgeY / max(_FadeY, 0.0001)));
                float extensionArea = step(_ProtectedLeftRatio, i.texcoord.x);
                color.a *= min(lerp(1.0, fadeX, extensionArea), lerp(1.0, fadeY, extensionArea));
                return color;
            }
            ENDCG
        }
    }
}
