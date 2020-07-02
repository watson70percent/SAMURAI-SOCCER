Shader "Custom/IFS_Default"
{
	Properties
	{
		_OffsetX("OffsetX",Range(0,5)) = 1
		_OffsetY("OffsetY",Range(0,5)) = 1
		_OffsetZ("OffsetZ",Range(0,5)) = 1
		_Scale("Scale",Range(1,10)) = 3
		_Fold("Fold",Range(1,20)) = 6
		_Rounded("Rounded",Range(0,1)) = 0.05
		_Tex("Texture",2D) = "white"{}
		_Color("Color",Color) = (1,1,1,1)
			
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
				float _Rounded;
				sampler2D _Tex;
				float4 _Tex_ST;
				fixed4 _Color;

				struct appdata
				{
					float4 vertex: POSITION;
				};

				struct v2f
				{
					float4 vertex: SV_POSITION;
					float4 pos:POSITION1;
					float3 forward:TEXCOORD0;
					float4x4 onlyRotation:TEXCOORD1;
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
					o.pos = v.vertex;
					float4 t = mul(unity_ObjectToWorld, float4(0, 0, 0,1));
					o.forward = v.vertex.xyz-mul(unity_WorldToObject,_WorldSpaceCameraPos-t).xyz;


					/*float4x4 q = unity_WorldToObject;
					float XS = length(q[0].xyz);
					float YS = length(q[1].xyz);
					float ZS = length(q[2].xyz);

					q = float4x4(float4(q[0].xyz / XS, q[0].w), float4(q[1].xyz / YS, q[1].w), float4(q[2].xyz / ZS, q[2].w), q[3]);
					o.onlyRotation = q;
					o.pos.x *= 1 / XS;
					o.pos.y *= 1 / YS;
					o.pos.z *= 1 / ZS;
					o.forward = o.pos.xyz - mul(o.onlyRotation, _WorldSpaceCameraPos - t).xyz;*/  //サイズの変更に合わせて変化させたくない場合はこっち
					

					return o;
				}

				

				float2x2 rotate(in float a) {
					float s = sin(a), c = cos(a);
					return float2x2(c, s, -s, c);
				}


				float2 foldRotate(in float2 p, in float s) {
					float a = atan2(p.x, p.y) - PI / s;
					float n = PI * 2 / s;
					a = floor(a / n) * n + n;
					p = mul(rotate(-a), p);
					return p;
				}




				rm RayMarching(float3 pos) {

					pos.yx = foldRotate(pos.yx, _Fold);


					float3 offset = float3(_OffsetX,_OffsetY,_OffsetZ);
					float scale = _Scale;

					rm o;

					float4 p = float4(pos, 1);

					for (int n = 0; n < 2; n++) {
						p = abs(p);
						p.xy = (p.x < p.y) ? p.yx : p.xy;  //ここはy=xとかで折り返している
						p.xz = (p.x < p.z) ? p.zx : p.xz;
						p.yz = (p.y < p.z) ? p.zy : p.yz;




						p *= scale;
						p.xyz -= offset * (scale - 1)*0.5;

						if (p.z < -0.25*offset.z*(scale - 1)) {
							p.z += offset * (scale - 1.0)*0.5; //ここはフラクタル構造をいじっている

						}

					}





					float dis1 = (length(max(abs(p.xyz) - float3(0.5, 0.5, 0.5), 0.0)) - _Rounded) / p.w; //最後の引き算は丸みをつけるため
					dis1 = (dis1 <= 0) ? max(abs(p.x), max(abs(p.y), abs(p.z))) - 2 : dis1;

					o.dis = dis1;

					o.col = fixed4(1, 1, 1, 1);

					return o;
	}






	float3 getNormal(float3 pos) {
		float d = 0.001;
		return normalize(float3(
			RayMarching(pos + float3(d, 0, 0)).dis - RayMarching(pos + float3(-d, 0, 0)).dis,
			RayMarching(pos + float3(0, d, 0)).dis - RayMarching(pos + float3(0, -d, 0)).dis,
			RayMarching(pos + float3(0, 0, d)).dis - RayMarching(pos + float3(0, 0, -d)).dis
			));
	}

	float2 getUV(float3 pos) {

		float d = max(abs(pos.x), max(abs(pos.y), abs(pos.z)));

		float2 uv =
			pos.xy*step(0,abs(pos.z) - d) + pos.yz*step(0,abs(pos.x) - d) + pos.zx*step(0,abs(pos.y) - d);
		uv += float2(0.5,0.5);

		uv = uv * _Tex_ST.xy + _Tex_ST.zw;
		return uv ;
	}


	fixed4 frag(v2f i) : SV_Target
	{
		fixed4 col;

		col = _Color;
		col.w = 0;

		float3 pos = i.pos.xyz;
		float3 forward = normalize(i.forward.xyz);

		float3 light = normalize(mul(unity_WorldToObject, _WorldSpaceLightPos0));

		const int StepNum = 256;
		for (int i = 0; i < StepNum; i++)
		{
			rm o = RayMarching(pos);
			if (o.dis < 0.01) {

				float3 normal = getNormal(pos);
				col.rgb *= dot(normal, light)*0.3 + 0.7;

				/*float3 normal = (o.dis == 0) ? normalize(pos) : getNormal(pos);  
				col.rgb *= dot(normal, normalize(light))*0.3 + 0.7*((o.dis == 0) ? 0.2 : 1);*/   //内部のライティングがうまく行かないとき用
				//col.xyz *= 1.0 - i * 0.02;
				col.w = 1;


				break;
			}
			pos.xyz += o.dis * forward;


		}

		float2 uv = getUV(pos);
		col.rgb = lerp(tex2D(_Tex, uv),col,0.6);

		//col.w = 1-i* 0.02;





		return col;
	}

	ENDCG
	}
		}
			FallBack "Diffuse"
}