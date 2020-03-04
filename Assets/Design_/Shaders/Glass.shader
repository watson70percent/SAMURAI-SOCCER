// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Custom/Glass"
{
	Properties
	{
	   _Refraction("Refraction",Range(0,0.5)) = 0.05
	   _Color("Color",Color) = (255,0,0,0)
	}
		SubShader
	{
		Tags {
		"Queue" = "Transparent"
		"RenderType" = "Opaque" }
		LOD 200
		//Blend SrcAlpha OneMinusSrcAlpha
		GrabPass{}

		Pass{

		Cull Off

		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

		sampler2D _GrabTexture;

	float _Refraction;
	fixed4 _Color;


	struct appdata {
		float4 vertex : POSITION;
		float3 normal :NORMAL;

	};

	struct v2f {
		float4 grabPos :TEXCOOD0;
			float4 pos : SV_POSITION;
			float3 refract:TEXCOORD1;
			float attent :TEXCOORD2;
	};

	v2f vert(appdata v) {

		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.grabPos = ComputeGrabScreenPos(o.pos);
		//float3 normal = v.normal;//こっちでも下の書き方でも多分変わらない
		float3 normal = UnityObjectToWorldNormal(v.normal);
		float3 cameraDir = normalize(_WorldSpaceCameraPos - mul(unity_ObjectToWorld, v.vertex));
		o.attent = abs(mul(cameraDir, normal));
		o.refract = mul(UNITY_MATRIX_VP, normal)*_Refraction;
		//o.refract = normal * _Refraction;
		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		float x = i.grabPos.x / i.grabPos.w;
	float y = i.grabPos.y / i.grabPos.w;

		fixed4 col = tex2D(_GrabTexture, float4(x-i.refract.x, y+i.refract.y, i.grabPos.z, i.grabPos.w));
		col +=_Color* (1-i.attent)*2;
		col.r = 0;
		return col;
		}
			ENDCG
	}


	}
		FallBack "Diffuse"
}
