Shader "Custom/RimLighting"
{
	Properties
	{
		_Color("Color",Color) = (255,0,0,0)
		[Header(Rim Lighting)]
		_RimLightColor("Lim Right Color", Color) = (1, 1, 1, 1)
		_RimLightPower("Lim Right Power", Range(1, 20.0)) = 5.0
			_DiffuseShade("DiffuseShade",Range(0.1,1))=0.5
	}


		SubShader{



		Pass{
			CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

			fixed4 _RimLightColor;
		int _RimLightPower;
		fixed4 _Color;
		float _DiffuseShade;

		struct appdata {
			float4 vertex : POSITION;
			float3 normal :NORMAL;

		};


			struct v2f
		{
			float4 pos :SV_POSITION;
			float4 posWS :TEXCOORD0;
			float3 normal : TEXCOORD1;
};

		v2f vert(appdata v) {

			v2f o;
			o.pos = UnityObjectToClipPos(v.vertex);
			o.posWS = mul(unity_ObjectToWorld, v.vertex);
			o.normal = v.normal;
			return o;
		}

			fixed4 frag(v2f i) : SV_Target
		{
				float3 L = normalize(-_WorldSpaceLightPos0.xyz);
		float3 N = normalize( i.normal);
		float3 V = normalize(i.posWS - _WorldSpaceCameraPos);
			float3 H = normalize(-L - V);

				fixed4 col=_Color;

			fixed3 diffColor = max(0, dot(N, -L) * _DiffuseShade + (1 - _DiffuseShade)) * col.rgb;
			col.rgb = diffColor;

			float rim = 1.0 - saturate(dot(N, -V));
			rim *= saturate(1.0 - saturate(dot(N, L)) + dot(V, -L));
			fixed3 rimColor = pow(rim, _RimLightPower)* _RimLightColor;
			col.rgb += rimColor;



			return col;
		}
				ENDCG
		}
		}



}