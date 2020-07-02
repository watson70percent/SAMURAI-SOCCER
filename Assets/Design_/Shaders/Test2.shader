Shader "Custom/Test2"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
	}
		SubShader
	{
		Pass{


		CGPROGRAM

		#pragma vertex vert
		#pragma fragment frag
#include "UnityCG.cginc"
		fixed4 _Color;

	struct v2f {
		half3 worldNormal : TEXCOORDO;
		float4 pos : SV_POSITION;
	};

	v2f vert(float4 vertex : POSITION, float3 normal : NORMAL)
	{
		v2f o;
		o.pos = UnityObjectToClipPos(vertex);
		o.worldNormal = UnityObjectToWorldNormal(normal);
		return o;
	}

		fixed4 frag(v2f i) : SV_Target
	{
		fixed4 c = 0;
		c.r = i.worldNormal.y*0.5 + 0.5;
		return c;
	}
		ENDCG
	}
	}
}
