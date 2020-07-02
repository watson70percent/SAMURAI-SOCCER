Shader "Custom/Mosaic"
{
	Properties
	{
	   _BlockSize("BlockSize",Float) = 10
	}
		SubShader
	{
		Tags {
		"Queue" = "Transparent"
		"RenderType" = "Opaque" }
		LOD 200
		//Blend SrcAlpha OneMinusSrcAlpha
		GrabPass{}

		Pass{

		//Cull Off

		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

		sampler2D _GrabTexture;

	float _BlockSize;

	struct appdata {
		float4 vertex : POSITION;
	};

	struct v2f {
		float4 grabPos :TEXCOOD0;
			float4 pos : SV_POSITION;
	};

	v2f vert(appdata v) {

		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.grabPos = ComputeGrabScreenPos(o.pos);
		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		
	//float xblock = ceil(i.grabPos.x*_BlockSize)/_BlockSize; //こっちを使うとモザイクの四角が斜めになる
	//float yblock = ceil(i.grabPos.y*_BlockSize)/_BlockSize;
	float xblock = ceil(i.grabPos.x/i.grabPos.w*_BlockSize)/ _BlockSize;
	float yblock = ceil(i.grabPos.y/i.grabPos.w*_BlockSize)/ _BlockSize;
	fixed4 col = tex2D(_GrabTexture, float4(xblock, yblock, i.grabPos.z, i.grabPos.w));
	//float4 col = tex2D(_GrabTexture, float4(1.5, 2, 0,0));
	col += (0.9, 0, 0, 0.2);
	return col;
	
	}
		ENDCG
}

	
	}
		FallBack "Diffuse"
}
