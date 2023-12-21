Shader "Custom/Gaming"
{
	Properties
	{
		 _Freqency("Freaquency",Float) = 1
		_Length("Length",Range(0.0,1)) = 1
		_Width("Width",Range(0.0,1)) =0.05
		_BaseColor("BaseColor",Color)=(1,1,1,1)
	}



		SubShader
	{
		Tags {
		"Queue" = "Transparent"
		"RenderType" = "Opaque" }
		LOD 200

	   Pass{

			Cull Back

			CGPROGRAM


			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#define PI 3.141592



			struct appdata
			{
				float4 vertex: POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex: SV_POSITION;
				float3 pos:POSITION1;
				float3 forward : TEXCOORD1;
				float2 uv : TEXCOORD0;
			};

			struct rm {

				float distance : TEXCOORD0;
				fixed4 color : TEXCOORD1;
			};



			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.pos = _WorldSpaceCameraPos;
				o.forward = mul(unity_ObjectToWorld, v.vertex).xyz - _WorldSpaceCameraPos;
				o.uv = v.uv;
				return o;
			}


			float _Freqency;
			float _Length;
			fixed4 _BaseColor;
			fixed4 _FloorColor;
			float _Width;

			//レイマーチングはこの距離関数で模様を決める
			float2 HoneycombBlock(float2 pos) {
				//ハニカム構造を並進対称性のある長方形に区切る 
				//どの長方形にいるのかを判定し、長方形の中でどの六角形に属するのかを次に判定
				float xl = _Length * 3;
				float yl = _Length * sqrt(3);
				float Xblock = floor(pos.x / xl)*xl;
				float Zblock = floor(pos.y / yl)*yl;

				float2 pos1 = float2(Xblock + xl * 0.5, Zblock);
				float2 pos2 = float2(Xblock, Zblock + yl * 0.5);
				float2 pos3 = float2(Xblock + xl, Zblock + yl * 0.5);
				float2 pos4 = float2(Xblock + xl * 0.5, Zblock + yl);

				float2 pos5 = pos2 * step(0, Xblock + xl * 0.5 - pos.x) + pos3 * step(0,pos.x - xl * 0.5 - Xblock);
				float2 pos6 = pos1 * step(0, Zblock + yl * 0.5 - pos.y) + pos4 * step(0, pos.y - yl * 0.5 - Zblock);

				float dis1 = pow(pos5.x - pos.x, 2) + pow(pos5.y - pos.y,2);
				float dis2 = pow(pos6.x - pos.x, 2) + pow(pos6.y - pos.y, 2);

				float dis = min(dis1, dis2);

				float2 blockPos = pos5 * step(dis1, dis) + pos6 * step(dis2, dis);

				return blockPos;



			}


			




			float func(float x,float y, float x1, float y1, float l){
			
				return  0.5*(x - x1) + (y1 + l) - y;
			}

			float func2(float x, float y, float x1, float y1, float l) {

				return  -0.5*(x - x1) + (y1 + l) - y;
			}


			rm rayMarching(float2 uv) {

				rm o;


				float timerate = _Freqency;
				float t1 = abs(_Time*timerate-floor( _Time*timerate )-0.5 )*2 ;
				float t2 = abs((_Time+0.33) * timerate - floor((_Time+0.33) * timerate) - 0.5 ) * 2;
				float t3 = abs((_Time+0.66) * timerate - floor((_Time+0.66) * timerate) - 0.5 ) * 2;

				float t4 = abs((_Time + 0.33) * timerate + uv.x - floor((_Time + 0.33) * timerate + uv.x) - 0.5) * 5;
				t4 = min(0.5, t4);
				fixed4 floorColor = fixed4(
					saturate(t1*2 -t4 *0.7),
					saturate(t2*2 -t4 *0.7),
					saturate(t3*2  -t4 *0.7),
					1);

				//uv = float2(uv.x * 100, uv.y * 100);

				float2 hcPos = HoneycombBlock(uv);
				float l = _Length * (1-_Width);
				float l2 = l * sqrt(3)*0.5;
				//float isin = step(0, 0.001 - (hcPos.x - uv.x)*(hcPos.x - uv.x) - (hcPos.y - uv.y)*(hcPos.y - uv.y));
				float isin = step(hcPos.y - uv.y, l2)*step(uv.y -hcPos.y , l2)*step(0, func(uv.y, uv.x, hcPos.y, hcPos.x, l))*step( func(uv.y, uv.x, hcPos.y, hcPos.x, -l),0)*step(0, func2(uv.y, uv.x, hcPos.y, hcPos.x, l))*step(func2(uv.y, uv.x, hcPos.y, hcPos.x, -l), 0);

				o.color =  floorColor*(1-isin) + _BaseColor * isin;
				//o.color = getHoneycombColor(uv);
				return o;
			}






			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col;

				col.xyz = 1;
				col.w = 1;

				float3 pos = float3(i.pos.x, i.pos.y, i.pos.z);
				float3 forward = normalize(i.forward);
				forward = float3(forward.z, -forward.x, forward.y);



				float3 normal = float3(0,1,0);

				
				rm op = rayMarching(i.uv);
				col.rgb *= dot(normal, normalize(_WorldSpaceLightPos0))*0.6 + 0.4;
				col = op.color;
				//col.rgb = normal;
				
				//col.rgb*=saturate(pos.y+1);
				/*col.w = saturate(StepNum - i - 1);*/
				//col.w = 1-i* 0.02;





				return col;
			}

			ENDCG
		}
	}
}