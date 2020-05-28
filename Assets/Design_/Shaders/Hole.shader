Shader "Custom/Hole"
{
	Properties
	{
		 _Threshold("Threshold", Range(0.0,3.0)) = 0.6 // sliders
		 _Freqency("Freaquency",Float) = 1
		_TimeRate("TimeRate",Float) = 1
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
			float Hole(float3 pos) {
				float block = 10;
				float xblock = floor(pos.x / block)*block + block * 0.5;
				float zblock = floor(pos.z / block)*block + block * 0.5;


				//return sqrt(10 - (pow(pos.x - xblock, 2) + pow(pos.z - zblock, 2)));
				float radius = sqrt((xblock - pos.x)*(xblock - pos.x) + (zblock - pos.z)*(zblock - pos.z));
				return 3 - radius;
			}

			float Floor(float3 pos) {

				return pos.y;
			}


			float CylinderSide(float3 pos) {
				float block = 10;
				float xblock = floor(pos.x / block)*block + block * 0.5;
				float zblock = floor(pos.z / block)*block + block * 0.5;

				float radius = sqrt((xblock - pos.x)*(xblock - pos.x) + (zblock - pos.z)*(zblock - pos.z));
				return radius-2;
			}

			float CylinderCeil(float3 pos) {
				float block = 10;
				float xblock = floor(pos.x / block)*block + block * 0.5;
				float zblock = floor(pos.z / block)*block + block * 0.5;
				float ceilHeight= pos.y +15 + sin(_Time*40  + xblock * zblock * 20) * (sin( xblock*zblock )*0.4+0.6 )*25;
				return min(3, ceilHeight);
				//return pos.y+_SinTime*100;
			}


			float getDis(float3 pos) {
				//return max(Hole(pos), Floor(pos));
				//return max(CylinderCeil(pos), CylinderSide(pos));
				return min(max(Hole(pos), Floor(pos)), max(CylinderCeil(pos), CylinderSide(pos)));
			}

			float3 getNormal(float3 pos) {
				float d = 0.1;
				return normalize(float3(
					getDis(pos + float3(d, 0, 0)) - getDis(pos + float3(-d, 0, 0)),
					getDis(pos + float3(0, d, 0)) - getDis(pos + float3(0, -d, 0)),
					getDis(pos + float3(0, 0, d)) - getDis(pos + float3(0, 0, -d))
					));
			}


			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col;

				col.xyz = 1;
				col.w = 1;

				float3 pos = i.pos.xyz;
				float3 forward = normalize(pos.xyz - _WorldSpaceCameraPos);



				float3 normal = float3(0,1,0);

				const int StepNum = 200;
				for (int i = 0; i < StepNum; i++)
				{
					float dis = getDis(pos);
					if (dis < 0.1) {
						//col.xyz = 1.0 - i * 0.04;
						normal = getNormal(pos);
						break;
					}
					pos.xyz += forward * dis;


				}

				//col.rgb = normal;
				col.rgb *= dot(normal , normalize(_WorldSpaceLightPos0))*0.6+0.4 ;
				//col.rgb*=saturate(pos.y+1);
				col.w = saturate(StepNum - i - 1);
				//col.w = 1-i* 0.02;





				return col;
			}

			ENDCG
		}
	}
		FallBack "Diffuse"
}
