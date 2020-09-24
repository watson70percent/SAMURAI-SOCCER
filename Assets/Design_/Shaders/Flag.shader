Shader "Custom/Flag"
{
	Properties
	{
		_Texture("Texture",2D)="white"{}
		_WaveSize("WaveSize",Int) = 5
		_Frequency("Frequency", Int) = 5
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
			#include "UnityCG.cginc"

			sampler2D _Texture;
			int _WaveSize, _Frequency;

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
				v.vertex.z = v.vertex.z + sin((v.vertex.x * 40 + _Time * 20)*_Frequency)*min(abs(v.vertex.x - 0.01),0.001*_WaveSize);
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col;
			col = tex2D(_Texture, i.uv);
				return col;
			}

			ENDCG
		}
		Pass
	 {
		 Tags{ "LightMode" = "ShadowCaster" }
		Cull Off

		 CGPROGRAM
		 #pragma vertex vert
		 #pragma fragment frag
		 #pragma multi_compile_shadowcaster

		 #include "UnityCG.cginc"

		 struct appdata
		 {
			 float4 vertex : POSITION;
			 float3 normal : NORMAL;
			 float2 texcoord : TEXCOORD0;
		 };

			int _WaveSize, _Frequency;

		 struct v2f
		 {
			 V2F_SHADOW_CASTER;
		 };

		 v2f vert(appdata v)
		 {
			 v2f o;
			 v.vertex.z = v.vertex.z + sin((v.vertex.x * 40+_Time*20)*_Frequency)*min(abs(v.vertex.x - 0.01),0.001*_WaveSize);
			 TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
			 return o;
		 }

		 fixed4 frag(v2f i) : SV_Target
		 {
			 SHADOW_CASTER_FRAGMENT(i)
		 }
		 ENDCG
	 }
	}
	
}