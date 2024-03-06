﻿Shader "Samurai/Samurai_Default"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		[Enum(UnityEngine.Rendering.CompareFunction)]_Shadow("Shadow_ZTest",Float)=1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

		Pass{

			Tags { "LightMode" = "ForwardBase"}

			Cull Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;

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
				o.pos = UnityObjectToClipPos(v.vertex);
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

		Pass {
			
			Tags{ "LightMode"="ShadowCaster" }

     		CGPROGRAM
     		#pragma vertex vert
     		#pragma fragment frag
     		#pragma multi_compile_shadowcaster

     		#include "UnityCG.cginc"

     		struct appdata
     		{
         		float4 vertex : POSITION;
         		float3 normal : NORMAL;
         		float2 texcoord : TEXCOORD0;
     		};

     		struct v2f
     		{
         		V2F_SHADOW_CASTER;
     		};

     		v2f vert (appdata v)
     		{
         		v2f o;
         		TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
         		return o;
     		}

     		fixed4 frag (v2f i) : SV_Target
     		{
        		 SHADOW_CASTER_FRAGMENT(i)
     		}
     		ENDCG
		}
    }
	//FallBack "Diffuse"		
}
