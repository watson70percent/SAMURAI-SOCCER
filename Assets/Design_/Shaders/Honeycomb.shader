Shader "Custom/Honeycomb"
{
	Properties
	{
		 _Threshold("Threshold", Range(0.0,3.0)) = 0.6 // sliders
		 _Freqency("Freaquency",Float) = 1
		_TimeRate("TimeRate",Float) = 1
		_Length("Lemgth",Float) = 1
		_FloorColor("FloorColor",Color)=(1,1,1,1)
		_CylinderColor("CylinderColor",Color)=(1,1,1,1)
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

			struct rm {

				float distance : TEXCOORD0;
				fixed4 color : TEXCOORD1;
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
			float _Length;
			fixed4 _CylinderColor;
			fixed4 _FloorColor;

			//レイマーチングはこの距離関数で模様を決める
			float2 HoneycombBlock(float3 pos) {

				float xl = _Length * 3;
				float zl = _Length * sqrt(3);
				float Xblock = floor(pos.x / xl)*xl;
				float Zblock = floor(pos.z / zl)*zl;

				float2 pos1 = float2(Xblock + xl * 0.5, Zblock);
				float2 pos2 = float2(Xblock, Zblock + zl * 0.5);
				float2 pos3 = float2(Xblock + xl, Zblock + zl * 0.5);
				float2 pos4 = float2(Xblock + xl * 0.5, Zblock + zl);

				float2 pos5 = pos2 * step(0, Xblock + xl * 0.5 - pos.x) + pos3 * step(0,pos.x - xl * 0.5 - Xblock);
				float2 pos6 = pos1 * step(0, Zblock + zl * 0.5 - pos.z) + pos4 * step(0, pos.z - zl * 0.5 - Zblock);

				float dis1 = pow(pos5.x - pos.x, 2) + pow(pos5.y - pos.z,2);
				float dis2 = pow(pos6.x - pos.x, 2) + pow(pos6.y - pos.z, 2);

				float dis = min(dis1, dis2);

				float2 blockPos = pos5 * step(dis1, dis) + pos6 * step(dis2, dis);

				return blockPos;



			}

			float HoneycombWall(float3 pos) {

				float edgeLength = _Length;
				const float root3 = sqrt(3);

				float2 blockPos = HoneycombBlock(pos);

				float2 Point = blockPos + float2(edgeLength * 1, 0) * (step(0, pos.x - blockPos.x) * 2 - 1);
				float slope = root3 * (step(0, (pos.x - blockPos.x)*(pos.z - blockPos.y)) * (-2) + 1);



				float dis1 = abs(slope*pos.x - pos.z - slope * Point.x + Point.y)*0.5;
				float dis2 = edgeLength * root3*0.5 - abs(pos.z - blockPos.y);

					return min(dis1, dis2) - 1;
			}

			float Floor(float3 pos) {

				return pos.y;
			}

			float HoneycombPillerSide(float3 pos) {

				float edgeLength = _Length;
				const float root3 = sqrt(3);

				float2 blockPos = HoneycombBlock(pos);

				float2 Point = blockPos + float2(edgeLength * 1, 0) * (step(0, pos.x - blockPos.x) * 2 - 1);
				float slope = root3 * (step(0, (pos.x - blockPos.x)*(pos.z - blockPos.y)) * (-2) + 1);



				float dis1 = abs(slope*pos.x - pos.z - slope * Point.x + Point.y)*0.5;
				float dis2 = edgeLength * root3*0.5 - abs(pos.z - blockPos.y);

				return 2 - min(dis1, dis2);

			}
			float HonecombPillerCeiling(float3 pos) {


				float2 blockPos = HoneycombBlock(pos);

	
				float T = _Time * 5+blockPos.x*blockPos.y;
				float t = T - floor(T);

				float d =pow((t - 0.2) * 20 , 4)*step(0 , 0.2 - t) + pow((t - 0.3)*5 , 2)*step(0 , t - 0.3);
				d *= 10;
				d -=  35;
				d *= step(1, sin(blockPos.x*blockPos.y * 40+blockPos.x));
				d = min(10, d+10);
				//float ceilHeight = pos.y + 15 + sin(_Time * 60 + blockPos.x * blockPos.y * 100) * (sin(blockPos.x*blockPos.y)*0.4 + 0.6) * 25;
				float ceilHeight = pos.y + d;
				return min(_Length*0.2, ceilHeight);
				//return ceilHeight;
			}

			//float HoneycombLine(float3 pos) {

			//	const float root3 = sqrt(3);
			//	float edgeLength = _Length;
			//	float xl = _Length;
			//	float zl = _Length * root3;

			//	//まずハニカム構造の横線をつくる
			//	float D1 =  pos.x- floor(pos.x / (xl*3))*xl * 3 -xl; 
			//	float dis1 = step(0, D1)*min(D1, 2*xl - D1);  
			//	float dis2 = max(dis1, abs(pos.z - round(pos.z / zl)*zl));

			//	float D2= pos.x-1.5*xl - floor((pos.x-1.5*xl) / (xl * 3))*xl * 3 - xl;
			//	float dis3 = step(0, D2)*min(D2, 2 * xl - D2);
			//	float dis4 = max(dis3, abs(pos.z-zl*0.5 - round((pos.z-zl*0.5) / zl)*zl));



			//	//次に斜めの線を作る
			//	float X = floor(pos.x / xl)*xl + xl * 0.5;
			//	float Z = floor(pos.z / zl)*zl + zl * 0.5;

			//	//点(X,Z)を通って傾き√3,-√3の直線と点(pos.x, pos.z)との距離の短い方を取得
			//	//float dis3 = step(0, -D)*min(abs(root3*pos.x - pos.z - root3 * X + Z) / sqrt(10), abs(root3*pos.x + pos.z - root3 * X - Z) / sqrt(10));

			//	return min( dis2 , dis4 ) ;
			//}


			float getDis(float3 pos) {

				rm o;
				float dFloor= max( Floor(pos) , HoneycombWall(pos) ) ;
				float dCylinder= max(HonecombPillerCeiling(pos), HoneycombPillerSide(pos));
				return min(dFloor, dCylinder) ;
				
				
			}

			rm rayMarching(float3 pos) {

				rm o;
				float dFloor = max(Floor(pos), HoneycombWall(pos));
				float dCylinder = max(HonecombPillerCeiling(pos), HoneycombPillerSide(pos));
				o.distance = min(dFloor, dCylinder);

				float timerate = 4;
				float t1 = abs(_Time*timerate-floor( _Time*timerate )-0.5)*2 ;
				float t2 = abs((_Time+0.33) * timerate - floor((_Time+0.33) * timerate) - 0.5) * 2;
				float t3 = abs((_Time+0.66) * timerate - floor((_Time+0.66) * timerate) - 0.5) * 2;
				fixed4 floorColor = fixed4(
					saturate(t1 * 3 - 1),
					saturate(t2 * 3 - 1),
					saturate(t3 * 3 - 1),
					1);

				o.color = (dFloor < dCylinder) ? floorColor : _CylinderColor;
				return o;
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

				const int StepNum = 255;
				for (int i = 0; i < StepNum; i++)
				{
					rm op =rayMarching(pos);
					if (op.distance < 0.1) {
						//col.xyz = 1.0 - i * 0.04;
						normal = getNormal(pos);
						col.rgb *= dot(normal, normalize(_WorldSpaceLightPos0))*0.6 + 0.4;
						col*= op.color;
						break;
					}
					pos.xyz += forward * op.distance;


				}

				//col.rgb = normal;
				
				//col.rgb*=saturate(pos.y+1);
				col.w = saturate(StepNum - i - 1);
				//col.w = 1-i* 0.02;





				return col;
			}

			ENDCG
		}
	}
}