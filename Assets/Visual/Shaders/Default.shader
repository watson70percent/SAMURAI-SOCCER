Shader "Custom/Default"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 200
		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			fixed4 _Color;

			struct appdata
			{
				float4 vertex: POSITION;
			};

			struct v2f
			{
				float4 pos: SV_POSITION;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.pos= UnityObjectToClipPos(v.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col;
				col = _Color;
				return col;
			}

			ENDCG
		}
	}
	FallBack "Diffuse"
}
