Shader "Custom/FogShader1"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Texture", 2D) = "white" {}

	}
		SubShader
	{
		Tags {
		"RenderType" = "Opaque" }
		LOD 200

	   Pass{

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			fixed4 _Color;
			sampler2D _CameraDepthTexture;
			sampler2D _MainTex;

			struct appdata
			{
				float4 vertex: POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos: SV_POSITION;
				float2 uv:TEXCOORD0;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{

				fixed4 col;
				col = tex2D(_MainTex, i.uv);
				
				float x = i.uv.x-0.5;
				float y = i.uv.y-0.5;
				float parameter = sqrt(x*x + y * y)*1.2+0.2;


				parameter = saturate(parameter);
				return lerp(col,fixed4(0.3,0.3,0.5,1),parameter);
			}

			ENDCG
}
	}
		FallBack "Diffuse"
}
