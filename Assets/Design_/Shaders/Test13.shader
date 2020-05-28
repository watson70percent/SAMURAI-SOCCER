Shader "Custom/Test13"
{
	Properties
	{
		 _Threshold("Threshold", Range(0.0,3.0)) = 0.6 // sliders
		 _Freqency("Freaquency",Float) = 1
		_TimeRate("TimeRate",Float)=1
	}



		SubShader
	{
		Tags {
		"Queue" = "Transparent"
		"RenderType" = "Opaque" }
		LOD 200

	   Pass{

			 Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#define PI 3.141592

			

			struct appdata
			{
				float4 vertex: POSITION;
			};

			struct v2f
			{
				float4 vertex: SV_POSITION;
				float4 pos:POSITION1;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.pos = mul(unity_ObjectToWorld, v.vertex);
				return o;
			}

			float _Threshold;
			float _Freqency;
			float _TimeRate;


			//レイマーチングはこの距離関数で模様を決める
			bool isInObject(float3 pos) {
				const float PI2 = 6.28318530718;
				float dist = length(pos);
				return sin(PI2 * dist * _Freqency + PI2 * _Time * _TimeRate) < _Threshold;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col;

				col.xyz = 0;
				col.w = 1;

				float3 pos = i.pos.xyz;
				float3 forward = normalize(pos.xyz - _WorldSpaceCameraPos);


				const int StepNum = 70;
				const float MarchingDist = 0.03;
				for (int i = 0; i < StepNum; i++)
				{
					if (isInObject(pos)) {
						col.xyz = 1.0 - i * 0.03;
						break;
					}
					pos.xyz += MarchingDist * forward;
					

				}
				col.w = saturate(StepNum-i-1);
				//col.w = 1-i* 0.02;





				return col;
			}

			ENDCG
		}
	}
		FallBack "Diffuse"
}
