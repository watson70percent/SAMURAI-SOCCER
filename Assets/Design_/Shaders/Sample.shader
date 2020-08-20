Shader "Custom/Sample"
{

	Properties{
		_Color("Color",Color) = (1,1,1,1)
	}

		SubShader{
		
		Tags {
				"Queue" = "Transparent"
				"RenderType" = "Transparent"
			}

			Blend One One


			Cull Off

			CGPROGRAM
			#pragma surface surf Standard alpha:fade
			#pragma target 3.0

			fixed4 _Color;

		struct Input {
			float3 worldNormal;
				float3 viewDir;
		};

		void surf(Input IN, inout SurfaceOutputStandard o) {
			o.Albedo = _Color;
			float alpha = 1 - (abs(dot(IN.viewDir, IN.worldNormal)));
			o.Alpha = alpha * 1.5;
		}
		ENDCG

	


}


		FallBack "Standard"
}
