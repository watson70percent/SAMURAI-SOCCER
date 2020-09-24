Shader "Custom/Mirror"
{
	Properties
	{
		_MainTex("MainTex", 2D) = "white"{}

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

			sampler2D _MainTex;

			struct appdata
			{
				float4 vertex: POSITION;
				float3 normal :NORMAL;
				float2 uv:TEXCOORD0;
			};


			struct v2f
			{
				float4 pos :SV_POSITION;
				float3 normal  :NORMAL;
				float2 uv:TEXCOORD0;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.normal = v.normal;
				o.uv = v.uv;
				return o;
			}

			fixed4 frag(v2f i) : COLOR
			{
				fixed4 col;
				col = tex2D(_MainTex,i.uv);

				float3 N = normalize(i.normal);
				float3 L = -_WorldSpaceLightPos0;

				float light = dot(N, -L)*0.5 + 0.5;
				//col *= (1, 1, 1, 1)*light;
				return col;
			}

			ENDCG
		}
		
		Pass{


			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;

			struct appdata
			{
				float4 vertex: POSITION;
				float3 normal :NORMAL;
				float2 uv:TEXCOORD0;
			};


			struct v2f
			{
				float4 pos :SV_POSITION;
				float3 normal  :NORMAL;
				float2 uv:TEXCOORD0;
			};

			v2f vert(appdata v)
			{
				v2f o;
				float4 wp = mul(unity_ObjectToWorld, v.vertex);
				wp.y *= -1;
				o.pos = mul(UNITY_MATRIX_VP, wp);
				
				o.normal = v.normal;
				o.uv = v.uv;
				return o;
			}

			fixed4 frag(v2f i) : COLOR
			{
				fixed4 col;
				col = tex2D(_MainTex,i.uv);

				float3 N = normalize(i.normal);
				float3 L = -_WorldSpaceLightPos0;

				float light = dot(N, -L)*0.5 + 0.5;
				//col *= (1, 1, 1, 1)*light;
				return col;
		}

		ENDCG
}
	}
		FallBack "Diffuse"
}
