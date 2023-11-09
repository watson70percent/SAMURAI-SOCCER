Shader"Custom/Indicator_plate"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _Level ("Level", Range(0,1)) = 0.0
        _Metallic ("Metallic", Range(0, 1)) = 0.0
        _Smoothness ("Smoothness", Range(0, 1)) = 1.0

        [HDR]
        _EmissionColor("Emission", color) = (0, 0, 0, 0)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }

        CGPROGRAM
        #pragma surface surf Standard alpha noshadow nofog noshadowmask finalcolor:final

        #include "UnityCG.cginc"

        struct Input
        {
            float2 uv_MainTex;
        };

        sampler2D _MainTex;
            
        fixed4 _Color;
        fixed4 _EmissionColor;
        float _Level;
        fixed _Metallic;
        fixed _Smoothness;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float visible;
            float4 col;
            col = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            visible = step(IN.uv_MainTex.y, _Level);
            visible = min(visible, col.a);
            o.Albedo = col.rgb;
            o.Emission = _EmissionColor.rgb * visible;
            o.Metallic = _Metallic;
            o.Smoothness = _Smoothness;
            o.Alpha = visible;
        }

        void final(Input IN, SurfaceOutputStandard o, inout fixed4 color)
        {
            float visible;
            visible = step(_Color.a * 0.5, o.Alpha);
            color = color * visible;
        }

        ENDCG
    }
    FallBack OFF
}
