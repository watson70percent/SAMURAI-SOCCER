Shader "Custom/FoldRotate"
{
	Properties
	{
		_OffsetX("OffsetX",Range(0,2)) = 1
		_OffsetY("OffsetY",Range(0,2)) = 1
		_OffsetZ("OffsetZ",Range(0,2)) = 1
		_Scale("Scale",Range(1,5)) = 3
		_Fold("Fold",Range(1,20)) = 6
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


			float _OffsetX,_OffsetY,_OffsetZ,_Scale;
			float _Fold;

			struct appdata
			{
				float4 vertex: POSITION;
			};

			struct v2f
			{
				float4 vertex: SV_POSITION;
				float4 pos:POSITION1;
				float3 cp:TEXCOORD0;
			};

			struct rm
			{
				float dis : TEXCOORD0;
				fixed4 col : TEXCOORD1;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.pos = mul(unity_ObjectToWorld, v.vertex);
				o.cp = mul(unity_ObjectToWorld, float4(0, 0, 0, 1));
				return o;
			}

			float2x2 rotate(in float a) {
				float s = sin(a), c = cos(a);
				return float2x2(c, s, -s, c);
			}


			float2 foldRotate(in float2 p, in float s) {
				float a = atan2(p.x, p.y) - PI / s  ;
				float n = PI * 2 / s;
				a = floor(a / n) * n+n;
				p = mul(rotate(-a), p);
				return p;
			}




			rm RayMarching(float3 pos) {

				pos.yx = foldRotate(pos.yx, _Fold);


				float3 offset = float3(_OffsetX,_OffsetY,_OffsetZ);
				float scale = _Scale;

				rm o;

				float4 p = float4(pos, 1);

				for (int n = 0; n < 1; n++) {
					p = abs(p);
					p.xy = (p.x < p.y) ? p.yx : p.xy;  //ここはy=xとかで折り返している
					p.xz = (p.x < p.z) ? p.zx : p.xz;
					p.yz = (p.y < p.z) ? p.zy : p.yz;

					/*p.xy = (p.x*p.x < p.y) ? float2(p.x*p.x,p.y) : float2(p.y,p.x*p.x);
					p.xz = (p.x*p.x < p.z) ? float2(p.x*p.x, p.z) : float2(p.z, p.x*p.x);
					p.yz = (p.y*p.y < p.z) ? float2(p.y*p.y, p.z) : float2(p.z, p.y*p.y);*/


					p *= scale;
					p.xyz -= offset * (scale - 1.0);

					if (p.z < -0.5*offset.z*(scale - 1.0)) {
						p.z += offset * (scale - 1.0); //ここはフラクタル構造をいじっている
					}

				}

				o.dis = (length(max(abs(p.xyz) - float3(1, 1, 1), 0.0)) - 0) / p.w; //最後の引き算は丸みをつけるため

				o.col = fixed4(1, 1, 1, 1);

				return o;
			}






			float3 getNormal(float3 pos) {
				float d = 0.01;
				return normalize(float3(
					RayMarching(pos + float3(d, 0, 0)).dis - RayMarching(pos + float3(-d, 0, 0)).dis,
					RayMarching(pos + float3(0, d, 0)).dis - RayMarching(pos + float3(0, -d, 0)).dis,
					RayMarching(pos + float3(0, 0, d)).dis - RayMarching(pos + float3(0, 0, -d)).dis
					));
			}


			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col;

				col.xyz = fixed3(1,1,1);
				col.w = 1;

				float3 pos = i.pos.xyz - i.cp.xyz;
				float3 forward = normalize(i.pos.xyz - _WorldSpaceCameraPos);


				const int StepNum = 256;
				for (int i = 0; i < StepNum; i++)
				{
					rm o = RayMarching(pos);
					if (o.dis < 0.01) {
						//col.xyz = 1.0 - i * 0.03;

						float3 normal = getNormal(pos);
						col.rgb *= dot(normal, normalize(_WorldSpaceLightPos0))*0.3 + 0.7;
						break;
					}
					pos.xyz += o.dis * forward;


				}
				col.w = saturate(StepNum - i - 1);
				//col.w = 1-i* 0.02;





				return col;
			}

			ENDCG
		}
	}
		FallBack "Diffuse"
}