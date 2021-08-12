Shader "Samurai/Ground"
{
	Properties
	{
		[NoScaleOffset]_MainTex("Texture", 2D) = "white" {}
		[Normal]_BumpMap("NormalMap",2D)="bump"{}
		_BumpScale("Normal Scale", Range(0, 1)) = 1.0
		_Color("Color", Color) = (1,1,1,1)
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" }
			LOD 100


			



			Pass{

				Tags {"LightMode" = "ForwardBase"} //このタグがないと影が描写されない時がある(描写順?)
				

				CGPROGRAM
				
				#pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight //影の描写のために追加 
				#include "Lighting.cginc" //影の描写のために追加 
				#include "AutoLight.cginc"//影の描写のために追加 
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"

				sampler2D _MainTex;
				sampler2D _BumpMap;
				half _BumpScale;
				fixed4 _Color;

				struct appdata
				{
					float4 vertex: POSITION;
					float3 normal:NORMAL;
					float3 tangent :TANGENT;
					float2 uv:TEXCOORD0;
				};

				struct v2f
				{
					float4 pos: SV_POSITION;
					float2 uv:TEXCOORD0;
					float3 normal : TEXCOORD1;
					float3 lightDir : TEXCOORD2;
					SHADOW_COORDS(3)  //影の描写のために追加. 
				};

				float4x4 InvTangentMatrix(float3 tan, float3 bin, float3 nor)
				{
					float4x4 mat = float4x4(
						float4(tan, 0),
						float4(bin, 0),
						float4(nor, 0),
						float4(0, 0, 0, 1)
						);

					// 正規直交系行列なので、逆行列は転置行列で求まる
					return transpose(mat);
				}

				v2f vert(appdata v)
				{
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;
					// ローカル空間上での接空間ベクトルの方向を求める
					float3 n = normalize(v.normal);
					float3 t = v.tangent;
					float3 b = cross(n, t);
					// ワールド位置にあるライトをローカル空間へ変換する
					
					float3 localLight = mul(unity_WorldToObject, _WorldSpaceLightPos0);
					localLight.y *= 0.05; //あえて光の角度を横向きにすることで凹凸の陰影が出やすくなっている
					// ローカルライトを接空間へ変換する（行列の掛ける順番に注意）
					o.lightDir = mul(localLight, InvTangentMatrix(t, b, n));
					TRANSFER_SHADOW(o)//影の描写のために追加
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					

					fixed4 col;
					col = tex2D(_MainTex,i.uv);
					fixed4 shadow = SHADOW_ATTENUATION(i); //影を計算
					float3 normal = float4(UnpackNormal(lerp(fixed4(0.5,0.5,1,1), tex2D(_BumpMap, i.uv), _BumpScale)), 1);
					float3 light = normalize(i.lightDir);
					float diff = max(0, dot(normal, light));
					col *= shadow * (diff*0.5+0.9);
					col *= _Color;

					return col;
				}

					ENDCG
			}


		}

}
