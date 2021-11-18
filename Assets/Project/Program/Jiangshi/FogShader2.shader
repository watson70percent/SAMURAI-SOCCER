Shader "Custom/FogShader2"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)

	}
		SubShader
	{
		Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		LOD 200

	   Pass{

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			fixed4 _Color;


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

				fixed4 col=fixed4(1,1,1,0);

				float x = i.uv.x - 0.5;
				float y = i.uv.y - 0.5;
				float parameter = sqrt(x*x + y * y)*2;


				parameter = saturate(parameter);
				return lerp(col,fixed4(0.3,0.3,0.5,1),0.6);
			}

			ENDCG
		}
	}
		FallBack "Diffuse"
}
