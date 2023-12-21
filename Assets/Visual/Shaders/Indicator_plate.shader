Shader"Custom/Indicator_plate"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _Level ("Level", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200

	    ZWrite Off
        Blend OneMinusDstColor One

	    Pass {

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv: TEXCOORD;
            };

            struct v2f{
                float4 vertex: SV_POSITION;
                float2 uv: TEXCOORD;
            };

            sampler2D _MainTex;

            fixed4 _Color;
            float _Level;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_TARGET
            {
                float4 col;
                float visible;
                col = tex2D(_MainTex, i.uv) * _Color;
                visible = step(i.uv.y, _Level);
                visible = min(visible, col.a);
                col = col * visible;
                return col;
            }

            ENDCG

        }

    }
    FallBack OFF
}
