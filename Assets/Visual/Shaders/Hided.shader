Shader "Samurai/Hided"
{
	Properties
	{
		// �e�N�X�`������v�����ꍇ�Ƀo�b�`���O�������悤�ɂ��邽�߁A
		// �e�N�X�`���� Material ���璼�ڂł͂Ȃ� MaterialPropertyBlock �o�R�Őݒ肷��
		// �i�Ȃ��AUI �V�F�[�_�[�ł͊�{�I�� MaterialPropertyBlock ���g�����Ƃ͂ł��Ȃ��j
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}

	// �F
	_Color("Tint", Color) = (1,1,1,1)

		// �X�e���V����r�֐�
		// UnityEngine.Rendering.CompareFunction �Œ�`����Ă���
		// https://docs.unity3d.com/ja/current/ScriptReference/Rendering.CompareFunction.html
		// 8 �� Always �ł���A��ɃX�e���V���e�X�g����������
		_StencilComp("Stencil Comparison", Float) = 8

		// �X�e���V���e�X�g�̊�l�i0 �` 255�j
		_Stencil("Stencil ID", Float) = 0

		// �X�e���V���e�X�g�������̋���
		// UnityEngine.Rendering.StencilOp �Œ�`����Ă���
		// https://docs.unity3d.com/ja/current/ScriptReference/Rendering.StencilOp.html
		// 0 �� Keep �ł���A�ύX���s��Ȃ�
		_StencilOp("Stencil Operation", Float) = 0

		// �X�e���V���e�X�g���s������Ƀo�b�t�@�Ɋ�l���������ރr�b�g���w�肷��}�X�N
		// 0xFF �Ȃ̂Ŋ�l���o�b�t�@�̓��e�����̂܂ܔ�r����
		_StencilWriteMask("Stencil Write Mask", Float) = 255

		// �X�e���V���e�X�g���s���O�Ɋ�l�ƃo�b�t�@�̓��e�̗����ɂ�����_���a�}�X�N
		// 0xFF �Ȃ̂Ŋ�l���o�b�t�@�̓��e�����̂܂ܔ�r����
		_StencilReadMask("Stencil Read Mask", Float) = 255

		// �`��𔽉f���Ȃ��J���[�`�����l���̐ݒ�
		// UnityEngine.Rendering.ColorWriteMask �Œ�`����Ă���
		// https://docs.unity3d.com/ja/current/ScriptReference/Rendering.ColorWriteMask.html
		// 15 �́@All �ł���A�S�Ẵ`�����l��(A/B/G/R)���o�͂���
		_ColorMask("Color Mask", Float) = 15

		// UNITY_UI_ALPHACLIP �� define ���邩�ǂ���
		// 0 �Ȃ� define ���Ȃ�
		// Mask ���g��Ȃ��̂ł���ΕK�v�Ȃ�
		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0
	}

		SubShader
	{
		// �^�O���g���Ă���/�ǂ̂悤�Ƀ����_�����O���邩���w�肷��
		// https://docs.unity3d.com/ja/current/Manual/SL-SubShaderTags.html
		Tags
		{
		// UI �p�Ȃ̂� RenderQueue �� Transparent
		"Queue" = "Transparent"

		// Projector �R���|�[�l���g�̉e�����󂯂Ȃ�
		"IgnoreProjector" = "True"

		// �V�F�[�_�̕��ށBRenderQueue �Ƃ͕�
		// Shader Replacement ���g��Ȃ��Ȃ�K�v�Ȃ����ꉞ�����Ă���
		"RenderType" = "Transparent"

		// Inspector �̉��̃}�e���A���r���[�̕\������
		// �f�t�H���g�� Sphere�i���́j���� Plane�i2D�j�܂��� Skybox�i�X�J�C�{�b�N�X�j���I�ׂ�
		"PreviewType" = "Plane"

		// ���̃V�F�[�_�[�� Sprite �p���A�g���X�����ꂽ�ꍇ�ɂ͓��삵�Ȃ����Ƃ𖾎��������ꍇ�ɂ� False �ɂ���
		// ��{�I�ɂ� True �ŗǂ�
		"CanUseSpriteAtlas" = "True"
	}

		// �v���p�e�B�Ŏw�肳�ꂽ�X�e���V���̐ݒ�l�����ۂɐݒ肷��
		// https://docs.unity3d.com/ja/current/Manual/SL-Stencil.html
		Stencil
		{
		// �X�e���V���e�X�g�̊�l
		Ref[_Stencil]

		// ��r�֐�
		Comp[_StencilComp]

		// �X�e���V���e�X�g�������̋���
		Pass[_StencilOp]

		// �o�b�t�@�ǂݍ��ݎ��r�b�g�}�X�N
		ReadMask[_StencilReadMask]

		// �o�b�t�@�������ݎ��r�b�g�}�X�N
		WriteMask[_StencilWriteMask]
	}

		// https://docs.unity3d.com/ja/current/Manual/SL-CullAndDepth.html
		// UI �Ȃ̂ŃJ�����O�s�v
		Cull Off

		// ���K�V�[�ȌŒ�@�\���C�e�B���O�i�񐄏��j
		// https://docs.unity3d.com/ja/current/Manual/SL-Material.html
		// ���݂ł́iUI �Ɍ��炸�j��{�I�ɂ� Off �ŗǂ�
		Lighting Off

		// �[�x�o�b�t�@�ւ̏�������
		// https://docs.unity3d.com/ja/current/Manual/SL-CullAndDepth.html
		// Transparent �Ȃ̂� ZWrite �͕s�v 
		ZWrite Off

		// �[�x�e�X�g�̕��@
		// https://docs.unity3d.com/ja/current/Manual/SL-CullAndDepth.html
		// Canvas �� Overlay �Ȃ� Always�i��ɕ`��j
		// ����ȊO�Ȃ� LEqual�i�`��ς݃I�u�W�F�N�g�Ƃ̋������������������܂��͂��߂��ꍇ�ɕ`��j
		ZTest Always

		// https://docs.unity3d.com/ja/current/Manual/SL-Blend.html
		// Unity 2020.1 ����s�N�Z���u�����h�͏�Z�ςݓ����iPremultiplied transparency�j�ɂȂ���
	// https://issuetracker.unity3d.com/issues/transparent-ui-gameobject-ignores-opaque-ui-gameobject-when-using-rendertexture
		// �ȑO�̃u�����h�ɂ������Ȃ� Blend SrcAlpha OneMinusSrcAlpha �ɂ��ăt���O�����g�V�F�[�_�[�̏�Z�ςݓ����̏���������
	Blend One OneMinusSrcAlpha

		// �`��𔽉f���Ȃ��J���[�`�����l���̐ݒ�̓v���p�e�B�Őݒ肵���l���g��
		ColorMask[_ColorMask]

		Pass
		{
		// UsePass �Ŏg�����O
		// https://docs.unity3d.com/ja/current/Manual/SL-Name.html
		Name "Default"

		// Cg/HLSL �J�n
		CGPROGRAM
		// HLSL �X�j�y�b�g
		// https://docs.unity3d.com/ja/2018.4/Manual/SL-ShaderPrograms.html

		// ���_�V�F�[�_�[�̊֐������w��
		#pragma vertex vert

		// �t���O�����g�V�F�[�_�[�̊֐������w��
		#pragma fragment frag

		// �^�[�Q�b�g���x���͑S�v���b�g�t�H�[������
		// https://docs.unity3d.com/ja/current/Manual/SL-ShaderCompileTargets.html
		#pragma target 2.0

		// �C���N���[�h�t�@�C���̎w��
		// �C���N���[�h�t�@�C���̏ꏊ�� (Unity �̃C���X�g�[����)/Editor/Data/CGIncludes
		#include "UnityCG.cginc"
		#include "UnityUI.cginc"

		// Mask �� RectMask2D ���L�����ǂ����ŃN���b�s���O�@�\�̗L����؂�ւ��邽�߂̃V�F�[�_�[�o���A���g
		// UNITY_UI_CLIP_RECT �L�[���[�h�̓O���[�o���ł͂Ȃ��A���̃V�F�[�_�[�̂ݑΏۂ� OK �Ȃ̂Ń��[�J��
		#pragma multi_compile_local _ UNITY_UI_CLIP_RECT

		// �A���t�@�l�ŃN���b�s���O����@�\�����ƗL��� 2 ��ނ̃V�F�[�_�[�o���A���g��p��
		// UNITY_UI_ALPHACLIP �L�[���[�h�̓O���[�o���ł͂Ȃ��A���̃V�F�[�_�[�̂ݑΏۂ� OK �Ȃ̂Ń��[�J��
		#pragma multi_compile_local _ UNITY_UI_ALPHACLIP

		// ���b�V���̒��_�f�[�^�̒�`
		struct appdata_t
		{
		// �ʒu
		float4 vertex   : POSITION;

		// ���_�J���[
		float4 color    : COLOR;

		// 1 �Ԗڂ� UV ���W
		float2 texcoord : TEXCOORD0;

		// �C���X�^���V���O���L���ȏꍇ��
		//    uint instanceID : SV_InstanceID
		// �Ƃ�����`���t����������B
		// �ڍׂ� UnityInstancing.cginc ���Q�Ƃ̂���
		UNITY_VERTEX_INPUT_INSTANCE_ID
	};

	// ���_�V�F�[�_�[����t���O�����g�V�F�[�_�[�ɓn���f�[�^
	struct v2f
	{
		// ���_�̃N���b�v���W
		// �V�X�e�����g���iGPU �����X�^���C�Y�Ɏg���j�l�Ȃ̂� SV (System Value) ���t��
		float4 vertex   : SV_POSITION;

		// �F
		fixed4 color : COLOR;

		// 1 �Ԗڂ� UV ���W
		float2 texcoord  : TEXCOORD0;

		// 2 �Ԗڂ� UV ���W�ɒ��_�̃��[���h��Ԃł̈ʒu���i�[���ēn��
		float4 worldPosition : TEXCOORD1;

		// 3 �Ԗڂ� UV ���W�Ƀ}�X�N�̃f�[�^���i�[���ēn��
		half4  mask : TEXCOORD2;

		// VR �p�B�V���O���p�X�ŗ��ڂ̃����_�����O���\�ɂ���B
		UNITY_VERTEX_OUTPUT_STEREO
	};

	// �e�N�X�`���f�[�^���Q�Ƃ��邽�߂ɂ̓e�N�X�`���T���v���^�̒l���v���p�e�B�o�R�Ŏ󂯎��
	sampler2D _MainTex;

	// �F
	fixed4 _Color;

	// UI �p�� Unity �ɂ���Ď����I�ɐݒ肳���B
	// �g�p����e�N�N�`���� Alpha8 �^�Ȃ� (1,1,1,0) �A����ȊO�Ȃ� (0,0,0,0) �ɂȂ�
	fixed4 _TextureSampleAdd;

	// MaskableGraphic.SetClipRect() ������ CanvasRenderer.EnableRectClipping() �Őݒ�
	float4 _ClipRect;

	// �e�N�X�`���ϐ��� �� _ST ��ǉ������ Tiling �� Offset �̒l�������Ă���
	// x, y �� Tiling �l�� x, y �ŁAz, w �� Offset �l�� z, w ���������
	float4 _MainTex_ST;

	// Unity 2020.1 ���瓱�����ꂽ Soft Mask�i�[���ڂ����@�\�j�͈̔�
	// MaskableGraphic.SetClipSoftness() ������ CanvasRenderer.clippingSoftness �Őݒ�
	float _MaskSoftnessX;
	float _MaskSoftnessY;

	// ���_�V�F�[�_�[
	// appdata_t ���󂯎���� v2f ��Ԃ�
	v2f vert(appdata_t v)
	{
		// �t���O�����g�V�F�[�_�[�ɓn���ϐ�
		v2f OUT;

		// VR �p�̖ڂ̏��ƁAGPU �C���X�^���V���O�̂��߂̃C���X�^���V���O���Ƃ̍��W�𔽉f������
		// UnityInstancing.cginc ���Q�Ƃ̂���
		UNITY_SETUP_INSTANCE_ID(v);

		// VR �p�̃e�N�X�`���z��̖ڂ� GPU �ɓ`����
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

		// �I�u�W�F�N�g��Ԃ̒��_�̍��W���J�����̃N���b�v��Ԃɕϊ�����
		// UnityShaderUtilities.cginc ���
		// mul(UNITY_MATRIX_VP, float4(mul(unity_ObjectToWorld, float4(inPos, 1.0)).xyz, 1.0));
		//   UNITY_MATRIX_VP : ���݂̃r���[ * �v���W�F�N�V�����s��
		//   unity_ObjectToWorld : ���݂̃��f���s��
		//   https://docs.unity3d.com/ja/2018.4/Manual/SL-UnityShaderVariables.html
		// ���ԂƂ��Ă� mul(UNITY_MATRIX_MVP, v.vertex) �Ɠ�����
		float4 vPosition = UnityObjectToClipPos(v.vertex);

		// 2 �Ԗڂ� UV ���W�ɒ��_�̃��[���h��Ԃ�n��
		OUT.worldPosition = v.vertex;

		// �ϊ��������_�̃N���b�v���W��n��
		OUT.vertex = vPosition;

		// w �̓J��������̋���
		float2 pixelSize = vPosition.w;

		// _ScreenParams : ���݂̃X�N���[���i�����_�[�^�[�Q�b�g�j�T�C�Y
		// UNITY_MATRIX_P : ���݂̃v���W�F�N�V�����s��
		// 1 �s�N�Z���ɑ�������傫�������߂�
		pixelSize /= float2(1, 1) * abs(mul((float2x2)UNITY_MATRIX_P, _ScreenParams.xy));

		// ���x�𗎂Ƃ��� Mask �e�N�X�`���� UV ���쐬
		float4 clampedRect = clamp(_ClipRect, -2e10, 2e10);
		float2 maskUV = (v.vertex.xy - clampedRect.xy) / (clampedRect.zw - clampedRect.xy);

		// �}�X�N���ꂽ�����؂����ăe�N�X�`���� UV ���쐬
		OUT.texcoord = float4(v.texcoord.x, v.texcoord.y, maskUV.x, maskUV.y);

		// �\�t�g�}�X�N���l�������N���b�s���O�p�̃f�[�^
		OUT.mask = half4(v.vertex.xy * 2 - clampedRect.xy - clampedRect.zw,
		0.25 / (0.25 * half2(_MaskSoftnessX, _MaskSoftnessY) + abs(pixelSize.xy)));

		// ���_�J���[�Ƀv���p�e�B�̃J���[����Z
		OUT.color = v.color * _Color;

		return OUT;
	}

	// �t���O�����g�V�F�[�_�[
	fixed4 frag(v2f IN) : SV_Target
	{
		// �e�N�X�`������F�̃T���v�����O
		half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;

		#ifdef UNITY_UI_CLIP_RECT
		// �\�t�g�}�X�N���l�������N���b�s���O
		half2 m = saturate((_ClipRect.zw - _ClipRect.xy - abs(IN.mask.xy)) * IN.mask.zw);
		color.a *= m.x * m.y;
		#endif

		#ifdef UNITY_UI_ALPHACLIP
		// �A���t�@�� 0.001 �ȉ��Ȃ�i���قړ����Ȃ�j�s�N�Z����j������
		// �G�b�W�������ꍇ�͐����𑝂₵�Ă�������������Ȃ�
		clip(color.a - 0.001);
		#endif

		// ��Z�ςݓ����iPremultiplied transparency�j�Ȃ̂� RGB �� Alpha ����Z���Ă���
		color.rgb *= color.a;

		return color;
	}
		// Cg/HLSL �I��
		ENDCG
		}
	}
}