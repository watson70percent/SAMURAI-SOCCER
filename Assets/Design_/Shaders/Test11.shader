Shader "Custom/Test11"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_LocalTime("Animation Time", Float) = 0.0

	}
		SubShader
	{
		Tags {
		"RenderType" = "Opaque" }
		LOD 200

	   Pass{

		Cull Off
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma geometry geom
			#include "UnityCG.cginc"

			fixed4 _Color;
			float _LocalTime;

			struct appdata
			{
				float4 vertex: POSITION;
				float3 normal :NORMAL;
			};

			struct v2g
			{
				float4 pos: SV_POSITION;
				float4 vertex:TEXCOORD0;
				float3 normal :NORMAL;
			};

			struct g2f
			{
				float4 pos :SV_POSITION;
				float3 normal  :NORMAL;
			};

			v2g vert(appdata v)
			{
				v2g o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.vertex = v.vertex;
				o.normal = v.normal;
				return o;
			}



			[maxvertexcount(3)]
			void geom(triangle v2g input[3], uint pid : SV_PrimitiveID, inout TriangleStream<g2f> outStream)
			{
				float3 wp0 = input[0].vertex.xyz;
				float3 wp1 = input[1].vertex.xyz;
				float3 wp2 = input[2].vertex.xyz;

				float3 normal = normalize(cross(wp0 - wp1, wp0 - wp2));

				g2f o;
				for (int i = 0; i < 3; i++) {
					//o.normal = normal;
					//o.normal = lerp( input[i].normal , normal , dot( input[i].normal , normal )  ) ;
					o.normal = input[i].normal;
					float3 wp= input[i].vertex.xyz + normal * 0.2;
					//オブジェクト座標系の中ではwは意味を持たないのでオブジェクト座標のままジオメトリーシェーダーで計算すると良い
					//o.pos = UnityObjectToClipPos(float4(input[i].vertex.xyz,1)); 
					o.pos = UnityObjectToClipPos(float4(wp, 1));
					//o.pos = float4(wp, input[i].pos.w);
					outStream.Append(o);
				}

				



				//float ext = saturate(0.4 - cos(_LocalTime*UNITY_PI * 2)*0.41);
				//ext *= 1 + 0.3*sin(pid*832.37843 + _LocalTime * 88.76);

				//float3 offs = normalize(cross( wp0-wp1 , wp0-wp2 ) )*ext; //法線を求める関数
				//float3 wp3 = wp0+offs;
				//float3 wp4 = wp1+offs;
				//float3 wp5 = wp2+offs;

				//float3 wn =normalize(cross(wp3 - wp4, wp3 - wp5));
				//float np = saturate(ext * 10);

				//float3 wn0 = lerp(input[0].normal, wn, np);
				//float3 wn1 = lerp(input[1].normal, wn, np);
				//float3 wn2 = lerp(input[2].normal, wn, np); //追加した各頂点の法線情報を設定

				//g2f output0;
				//output0.pos =UnityWorldToClipPos(float4(wp3,1));
				//output0.normal = wn0;
				//g2f output1;
				//output1.pos = UnityWorldToClipPos(float4(wp4, 1));
				//output1.normal = wn1;
				//g2f output2;
				//output2.pos = UnityWorldToClipPos(float4(wp5, 1));
				//output2.normal = wn2;

				//outStream.Append(output0);
				//outStream.Append(output1);
				//outStream.Append(output2); //頂点を出力
				//outStream.RestartStrip();//面を出力(たぶん)

				/*wn = normalize(cross(wp3 - wp0, wp3 - wp5));
				outStream.Append(VertexOutput(wp3, wn));
				outStream.Append(VertexOutput(wp0, wn));
				outStream.Append(VertexOutput(wp4, wn));
				outStream.Append(VertexOutput(wp1, wn));

				outStream.RestartStrip();*/
				


			}

			fixed4 frag(g2f i) : COLOR
			{
				fixed4 col;
				col = _Color;
				col.rgb *= i.normal;
				return col;
			}

			ENDCG
}
	}
		FallBack "Diffuse"
}
