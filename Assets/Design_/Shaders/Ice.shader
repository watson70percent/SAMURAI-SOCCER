Shader "Custom/Ice"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Radius", Range(0.0,1.0)) = 0.9999
        _Smooth("Smooth", Range(1, 100)) = 10
        _Threshold ("Threshold", Range(0.0, 1.0)) = 0.75
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Tags { "LightMode"="ForwardBase" }

        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Ice

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        half _Glossiness;
        half _Smooth;
        half _Threshold;

        struct Input
        {
            float2 uv_MainTex;
        };

        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutput o)
        {
            // Albedo comes from a texture tinted by color
            o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
        }

        half4 LightingIce(SurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
        {
            float3 H = normalize(lightDir + viewDir);
            float3 spec = fixed4(1, 1, 1, 1) * pow(smoothstep(_Glossiness,1,max(0, dot(H, normalize(s.Normal)))), _Smooth);

            half4 c;
            c.rgb = (s.Albedo + spec) * ((atten > _Threshold) ? 1 : atten);
            c.a = s.Alpha;
            return c;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
