Shader "Pillage/OutsideMapEdgeFade"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _EdgeFadeX ("Edge Fade X", Range(0,1)) = 0.05
        _EdgeFadeY ("Edge Fade Y", Range(0,1)) = 0.05
        _ProtectedLeftRatio ("Protected Left Ratio", Range(0,1)) = 1
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
                fixed4 color : COLOR;
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
            float _EdgeFadeX;
            float _EdgeFadeY;
            float _ProtectedLeftRatio;

            v2f vert(appdata_t input)
            {
                v2f output;
                output.vertex = UnityObjectToClipPos(input.vertex);
                output.texcoord = input.texcoord;
                output.color = input.color * _Color;
                return output;
            }

            fixed4 frag(v2f input) : SV_Target
            {
                fixed4 color = tex2D(_MainTex, input.texcoord) * input.color;
                float isProtectedLeft = 1.0 - step(_ProtectedLeftRatio, input.texcoord.x);
                float xEdge = min(input.texcoord.x, 1.0 - input.texcoord.x);
                float yEdge = min(input.texcoord.y, 1.0 - input.texcoord.y);
                float xFade = saturate(xEdge / max(_EdgeFadeX, 0.0001));
                float yFade = saturate(yEdge / max(_EdgeFadeY, 0.0001));
                float fade = smoothstep(0.0, 1.0, min(xFade, yFade));
                color.a *= lerp(fade, 1.0, isProtectedLeft);
                return color;
            }
            ENDCG
        }
    }
}
