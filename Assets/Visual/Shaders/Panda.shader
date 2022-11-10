Shader "Samurai/Panda"
{
	Properties
	{
		_MainTex("MainTex", 2D) = "white"{}
		[PowerSlider(1.0)]_CullingDistance("CullingDistance",Range(0,1))=0.1 

	}
		SubShader
	{
		Tags {
		"RenderType" = "Opaque" "Queue"="Geometry+501" }
		LOD 200
		
	   Pass{

			Cull Back
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float _CullingDistance;

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
				clip(_CullingDistance-i.pos.z);
				col = tex2D(_MainTex,i.uv);
				return col;
			}

				ENDCG
	}
		GrabPass{"_PandaTexture"}
		Pass{

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _PandaTexture;
			struct appdata
			{
				float4 vertex: POSITION;
				float4 grabPos : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos: SV_POSITION;
				float4 grabPos : TEXCOORD0;
				float shadow : TEXCOORD1;
			};

			v2f vert(appdata v)
			{
				v2f o;
				float4 wPos = mul(unity_ObjectToWorld, v.vertex);
				float y = max(mul(unity_ObjectToWorld, float4(0, 0, 0, 1)).y, 0.001);
				wPos.y = min(wPos.y , 0.2-y*0.0005);
				float4 oPos = mul(unity_WorldToObject, wPos);
				o.pos = UnityObjectToClipPos(oPos);
				o.grabPos = ComputeGrabScreenPos(o.pos);

				//float XS = length(unity_WorldToObject[0].xyz);
				//o.shadow= 1/mul(unity_ObjectToWorld, v.vertex).y/XS*0.03;
				
				o.shadow = (1-y/100)*0.8;
				//o.shadow = min(o.shadow, 0.3);
				
				return o;
			}

			float random(float2 i) {
				float t = sin(i.x * 400 + i.y * 100) *sin(i.x*i.y * 5);
				return t;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				//clip(i.shadow);
				fixed4 col;
				col = tex2D(_PandaTexture, i.grabPos.xy/i.grabPos.w);
				col -= fixed4(1, 1, 1, 0)*i.shadow ;
				return col;
			}

			ENDCG
		}

		/*Pass{

			Tags{ "LightMode" = "ShadowCaster" "RenderType" = "Transparent"}   //影を落とすにはこの1文を追加したPassを用意すればよい(シェーダー内で形を変化させてないときはもっと短く書くコードがある)
			
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"


			struct appdata
			{
				float4 vertex: POSITION;
				float2 uv :TEXCOORD0;
			};

			struct v2f
			{
				float4 pos: SV_POSITION;
				float2 uv:TEXCOORD0;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.uv = v.uv;
				float4 wPos = mul(unity_ObjectToWorld, v.vertex);
				wPos.y = min( wPos.y , 0+0.01*wPos.y ) ;
				float4 oPos = mul(unity_WorldToObject, wPos);
				o.pos = UnityObjectToClipPos(oPos);
				return o;
			}

			float random(float2 i) {
				float t = sin(i.x*400 + i.y*100 ) *sin(i.x*i.y * 5);
				return t;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col;
				col = fixed4(1,0,0,0);
				return col;
			}

			ENDCG
		}*/
	}
		
}
