Shader "Samurai/Pettanco"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

		Pass{

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;;

			struct appdata
			{
				float4 vertex: POSITION;
				float2 uv:TEXCOORD0;
			};

			struct v2f
			{
				float4 pos: SV_POSITION;
				float2 uv:TEXCOORD0;
			};

			v2f vert(appdata v)
			{
				v2f o;
				v.vertex.x *= 3;
				v.vertex.z *= 3;
				float4 wPos = mul(unity_ObjectToWorld, v.vertex);
				wPos.y *= 0.16;
				float4 oPos = mul(unity_WorldToObject, wPos);
				o.pos = UnityObjectToClipPos(oPos);
				o.uv = v.uv;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col;
				col = tex2D(_MainTex,i.uv);
				return col;
			}


			ENDCG
		}
    }
}
