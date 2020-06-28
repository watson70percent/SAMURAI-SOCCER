Shader "Custom/Crystal"
{
	Properties
	{
	   _Refraction("Refraction",Range(0,0.5)) = 0.05
	   _MainColor("MainColor",Color) = (255,0,0,0)
		_SubColor("SubColor",Color)=(255,0,0,0)
				_RimLightPower("Lim Right Power", Range(1, 20.0)) = 5.0
	}
		SubShader
	{
		Tags {
		"Queue" = "Transparent"
		"RenderType" = "Opaque" }
		LOD 200
		
		GrabPass{}

		Pass{

		Cull Off

		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

		sampler2D _GrabTexture;

	float _Refraction;
	fixed4 _MainColor;
	fixed4 _SubColor;
	int _RimLightPower;


	struct appdata {
		float4 vertex : POSITION;
		float3 normal :NORMAL;

	};

	struct v2f {
		float4 grabPos :TEXCOOD0;
			float4 pos : SV_POSITION;
			float4 posWS :TEXCOORD1;
			float3 normal : TEXCOORD2;

	};

	v2f vert(appdata v) {

		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.grabPos = ComputeGrabScreenPos(o.pos);
		o.posWS = mul(unity_ObjectToWorld, v.vertex);
		o.normal = UnityObjectToWorldNormal(v.normal);
		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		float x = i.grabPos.x / i.grabPos.w;
	float y = i.grabPos.y / i.grabPos.w;

	float3 refract = mul(UNITY_MATRIX_VP, i.normal)*_Refraction;

		fixed4 col = tex2D(_GrabTexture, float4(x - refract.x, y + refract.y, i.grabPos.z, i.grabPos.w));

		float3 L = normalize(-_WorldSpaceLightPos0.xyz);
		float3 N = i.normal;
		float3 V = normalize(i.posWS - _WorldSpaceCameraPos);
			float3 H = normalize(-L - V);

			float attent = abs(dot(V, N));
			//col += _SubColor * (1 - attent);
				col.rgb = _SubColor.rgb * (1.04 - attent) + col.rgb*attent;
			col.rgb *= _MainColor.rgb;
			//col.r = 0;

			float rim = 1.1 - saturate(dot(N, -V));
			rim *= saturate(1.0 - saturate(dot(N, L)) + dot(V, -L)*0.2);
			fixed3 rimColor = pow(rim, _RimLightPower)* fixed4(1,1,1,1);
			col.rgb += rimColor;



			float specular = dot(H, N);
			col += fixed4(1, 1, 1, 1)*max(0, specular - 0.8) * 5;



			return col;
			}
				ENDCG
		}



	}

	
		FallBack "Diffuse"
}

