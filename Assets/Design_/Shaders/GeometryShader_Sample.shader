Shader "Custom/GeometryShader_Sample"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_LocalTime("Animation Time", Float) = 0.0
		_Length("Length",Range(0,3)) = 1

	}
		SubShader
	{
		Tags {
		"RenderType" = "Opaque" }
		LOD 200

	   Pass{


			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma geometry geom
			#include "UnityCG.cginc"

			fixed4 _Color;
			float _LocalTime;
			float _Length;

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



			[maxvertexcount(15)]
			void geom(triangle v2g input[3], uint pid : SV_PrimitiveID, inout TriangleStream<g2f> outStream)
			{
				float3 wp0 = input[0].vertex.xyz;
				float3 wp1 = input[1].vertex.xyz;
				float3 wp2 = input[2].vertex.xyz;

				float3 normal = normalize(cross(wp1 - wp0, wp2 - wp0));





				//float3 d = normal*(sin( _Time*40)+1);

				float ext = saturate(0.4 - cos(_Time*5 * UNITY_PI * 2) * 0.41);
				ext *= 1 + 0.3 * sin(pid * 832.37843 + _Time * 88.76);
				ext *= _Length;

				float3 d = normal;
				d.x *= _Length * ext;
				d.y *= _Length * ext;
				d.z *= _Length * ext;





				float3 wp3 = wp0 + d;
				float3 wp4 = wp1 + d;
				float3 wp5 = wp2 + d;

				g2f o;
				//三角ポリゴンは反時計回りに頂点を追加していくと面のむきが正しくなる(はず)
				o.normal = normal;
				o.pos = UnityObjectToClipPos(float4(wp3, 1));
				outStream.Append(o);		//頂点の出力
				o.pos = UnityObjectToClipPos(float4(wp4, 1));
				outStream.Append(o);
				o.pos = UnityObjectToClipPos(float4(wp5, 1));
				outStream.Append(o);
				outStream.RestartStrip(); //ポリゴンの出力


				//四角のポリゴンはИの順番で設定していくと面の向きが正しくなる(はず)
				normal = normalize(cross(wp1 - wp4, wp5 - wp4));
				o.normal = input[1].normal;
				o.pos = UnityObjectToClipPos(input[1].vertex);
				outStream.Append(o);
				o.normal = input[2].normal;
				o.pos = UnityObjectToClipPos(input[2].vertex);
				outStream.Append(o);
				o.normal = normal;
				o.pos = UnityObjectToClipPos(float4(wp4, 1));
				outStream.Append(o);
				o.pos = UnityObjectToClipPos(float4(wp5, 1));
				outStream.Append(o);
				outStream.RestartStrip();

				normal = normalize(cross(wp2 - wp5, wp3 - wp5));
				o.normal = input[2].normal;
				o.pos = UnityObjectToClipPos(input[2].vertex);
				outStream.Append(o);
				o.normal = input[0].normal;
				o.pos = UnityObjectToClipPos(input[0].vertex);
				outStream.Append(o);
				o.normal = normal;
				o.pos = UnityObjectToClipPos(float4(wp5, 1));
				outStream.Append(o);
				o.pos = UnityObjectToClipPos(float4(wp3, 1));
				outStream.Append(o);
				outStream.RestartStrip();

				normal = normalize(cross(wp0 - wp3, wp4 - wp3));
				o.normal = input[0].normal;
				o.pos = UnityObjectToClipPos(input[0].vertex);
				outStream.Append(o);
				o.normal = input[1].normal;
				o.pos = UnityObjectToClipPos(input[1].vertex);
				outStream.Append(o);
				o.normal = normal;
				o.pos = UnityObjectToClipPos(float4(wp3, 1));
				outStream.Append(o);
				o.pos = UnityObjectToClipPos(float4(wp4, 1));
				outStream.Append(o);
				outStream.RestartStrip();



			}

			fixed4 frag(g2f i) : COLOR
			{
				fixed4 col;
				col = _Color;

				float3 N = normalize( i.normal);
				float3 L = -_WorldSpaceLightPos0;

				float light = dot(N, -L)*0.5 + 0.5;
				col *= (1, 1, 1, 1)*light;
				return col;
			}

			ENDCG
}
	}
		FallBack "Diffuse"
}