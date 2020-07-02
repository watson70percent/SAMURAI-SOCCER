Shader "Custom/Sample2"
{
	Properties{
		_Color("Color",Color) = (1,1,1,1)
	}

		SubShader{

		Tags {
				"Queue" = "Transparent"
				"RenderType" = "Transparent"
			}

			Blend SrcAlpha OneMinusSrcAlpha


			Cull Off

			CGPROGRAM
			#pragma surface surf Ramp
			#pragma target 3.0

			fixed4 _Color;

		struct Input {
			float3 worldNormal;
				float3 viewDir;
		};

		fixed4 LightingRamp(SurfaceOutput s, fixed lightDir, fixed atten) {
			fixed4 c;
			c.rgb = s.Albedo;
			c.a = 1;
			return c;
		}

		void surf(Input IN, inout SurfaceOutput o) {
			o.Albedo = _Color;
			float alpha = 1 - (abs(dot(IN.viewDir, IN.worldNormal)));
			o.Alpha = alpha * 1.5;
		}
		ENDCG




	}


		FallBack "Standard"
}
