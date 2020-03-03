Shader "Custom/BlurLight"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_Rudius("Rudius",Range(-1,30))=0
		_LightIntensity("Light Intensity",Range(0,2)) = 0.6

	}
		SubShader
	{
		Tags {
		"Queue" = "Transparent"
		"RenderType" = "Transparent" }
		LOD 200
		Blend SrcAlpha OneMinusSrcAlpha

	   Pass{

			Cull Back
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			fixed4 _Color;
			float _Rudius;
			float _LightIntensity;

			struct appdata
			{
				float4 vertex: POSITION;
				float3 normal:NORMAL;
			};

			struct v2f
			{
				float4 pos: SV_POSITION;
				float3 normal : TEXCOORD0;
				float4 posWS : TEXCOORD1;
			};

			v2f vert(appdata v)
			{
				v2f o;
				v.normal = normalize(v.normal);
				v.vertex.xyz += v.normal*_Rudius;
				o.normal = v.normal;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.posWS = mul(unity_ObjectToWorld, v.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float3 V = normalize(i.posWS - _WorldSpaceCameraPos);
				float3 N = i.normal;

				fixed4 col;
				col = _Color;
				col.a = pow(abs(dot(V, N)), 4)*_LightIntensity;
				return col;
			}

			ENDCG
		}
/*
		Pass{

			Cull Front
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			fixed4 _Color;
			float _Rudius;

			struct appdata
			{
				float4 vertex: POSITION;
				float3 normal:NORMAL;
			};

			struct v2f
			{
				float4 pos: SV_POSITION;
				float3 normal : TEXCOORD0;
				float4 posWS : TEXCOORD1;
			};

			v2f vert(appdata v)
			{
				v2f o;
				v.normal = normalize(v.normal);
				v.vertex.xyz += v.normal*_Rudius;
				o.normal = v.normal;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.posWS = mul(unity_ObjectToWorld, v.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float3 V = normalize(i.posWS - _WorldSpaceCameraPos);
				float3 N = i.normal;

				fixed4 col;
				col = _Color;
				col.a = pow(abs(dot(V, N)),4)*0.7;
				return col;
			}

			ENDCG
		}
*/


	}
		FallBack "Diffuse"
}
