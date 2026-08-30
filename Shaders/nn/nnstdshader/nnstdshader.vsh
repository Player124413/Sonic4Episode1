/*---------------------------------------------------------------------------

    NN Library for DirectX G2.0 Standard Vertex Shader

    Copyright (C) 2008 SEGA Corporation Creative Center Graphics Sect.
    All Rights Reserved.

    Module  : NN Standard Shader
    File    : nnstdshader.vsh
    Create  : 2008/05/12 Ver.0.01.00
    Modify  : 2008/09/05 Ver.1.00.01
                ドライバの不具合と思われる原因不明の現象が発生した為 for 文をコメントして
                以下のように展開して回避した。
                不具合が発生したビデオカードは
                ・Quadro FX 4500(ドライバ169.96)
                ・GeForce 6800(ドライバ93.71, 175.19)
                です。
    Modify  : 2008/09/19 Ver.1.00.02 シェーダエンベロープ処理のとき頂点をWで割る計算が抜けていたので修正。
    Modify  : 2008/11/07 Ver.1.00.03 ２面ライティングの不具合修正。
    Modify  : 2009/06/16 Ver.1.00.04 整理。
    Version : 1.00.04
    Note    : 

---------------------------------------------------------------------------*/
/*------ Declarations (プロファイル定数) ----------------------------------*/

/* 変換後法線正規化 */
#ifndef NND_PROF_NORMALIZE_VTX_NORMAL
	#define NND_PROF_NORMALIZE_VTX_NORMAL		(1)			/* bool */
#endif

/* 変換後法線再スケール */
#ifndef NND_PROF_RESCALE_NORMAL
	#define NND_PROF_RESCALE_NORMAL				(0)			/* bool */
#endif

/* 頂点スペキュラーカラー */
#ifndef NND_PROF_VTX_SPECULAR
	#define NND_PROF_VTX_SPECULAR				(1)			/* bool */
#endif

/* 最大パラレルライト数 */
#ifndef NND_PROF_MAX_PARALLEL_LIGHT
	#define NND_PROF_MAX_PARALLEL_LIGHT			(4)			/* int 0～4 */
#endif

/* 最大ポイントライト数 */
#ifndef NND_PROF_MAX_POINT_LIGHT
	#define NND_PROF_MAX_POINT_LIGHT			(2)			/* int 0～4 */
#endif

/* 最大スポットライト数 */
#ifndef NND_PROF_MAX_SPOT_LIGHT
	#define NND_PROF_MAX_SPOT_LIGHT				(1)			/* int 0～4 */
#endif

#define NND_PROF_MAX_LIGHT						( NND_PROF_MAX_PARALLEL_LIGHT + NND_PROF_MAX_POINT_LIGHT + NND_PROF_MAX_SPOT_LIGHT )

/* ライトアンビエントパラメータ有効 */
#ifndef NND_PROF_LIGHT_AMBIENT
	#define NND_PROF_LIGHT_AMBIENT				(0)			/* bool */
#endif

/* ポイントライト距離減衰モデル */
#ifndef NND_PROF_POINTLIGHT_DISTATTEN
	#define NND_PROF_POINTLIGHT_DISTATTEN		(0)
										/* 0: 減衰なし 1: 逆 2: 逆２乗 */
#endif

/* スポットライト距離減衰モデル */
#ifndef NND_PROF_SPOTLIGHT_DISTATTEN
	#define NND_PROF_SPOTLIGHT_DISTATTEN		(0)
										/* 0: 減衰なし 1: 逆 2: 逆２乗 */
#endif

/* 従法線を法線接線から計算 */
#ifndef NND_PROF_CALC_VTX_BINORMAL
	#define NND_PROF_CALC_VTX_BINORMAL			(1)			/* bool */
#endif

/* フォグモデル */
#ifndef NND_PROF_FOG_MODEL	
	#define NND_PROF_FOG_MODEL					(0)			/* 0:フォグなし 1:LINEAR 2:EXP 3:EXP2 */
#endif

/* 距離フォグ */
#ifndef NND_PROF_DISTANCE_FOG
	#define NND_PROF_DISTANCE_FOG				(1)			/* bool */
#endif

/* フラグメント単位のフォグ計算 */
#ifndef NND_PROF_FRAG_FOG
	#define NND_PROF_FRAG_FOG					(1)			/* bool */
#endif

/* 最大ユーザーユニフォーム数 */
#ifndef NND_PROF_MAX_USER_UNIFORM
	#define NND_PROF_MAX_USER_UNIFORM			(4)			/* int */
#endif

/* 法線エンベロープスケールなし */
#ifndef NND_PROF_NO_SCALE_ENVELOPE
	#define NND_PROF_NO_SCALE_ENVELOPE			(0)			/* bool */
#endif

/*------ Declarations (シェーダ最適化プロファイル定数) --------------------*/

/* 頂点カラーあるなし */
#ifndef NND_OPT_VTX_COLOR
	#define NND_OPT_VTX_COLOR					(0)			/* bool 0:頂点カラーなし 1:頂点カラーあり */
#endif

/* 頂点カラーにスペキュラーカラーあるなし */
#ifndef NND_OPT_VTX_COLOR_SPECULAR
	#define NND_OPT_VTX_COLOR_SPECULAR			(0)			/* bool 0:頂点スペキュラカラーなし 1:頂点スペキュラカラーあり */
#endif

/* 法線あるなし */
#ifndef NND_OPT_VTX_NORMAL
	#define NND_OPT_VTX_NORMAL					(1)			/* bool 0:法線なし 1:法線あり*/
#endif

/* オブジェクトスペース法線 */
#ifndef NND_OPT_VTX_OBJECT_SPACE_NORMAL
	#define NND_OPT_VTX_OBJECT_SPACE_NORMAL		(0)			/* bool 0:タンジェントスペース 1:オブジェクトスペース */
#endif

/* 接線の反転 */
#ifndef NND_OPT_VTX_REVERSE_TANGENT
	#define	NND_OPT_VTX_REVERSE_TANGENT			(0)			/* bool 0:通常 1:反転 */
#endif

/* 頂点変換方式 */
#ifndef NND_OPT_VTX_TFM_MODE
	#define NND_OPT_VTX_TFM_MODE				(0)
#endif
#define NND_OPT_VTX_TFM_MODE_RIGID				(0)		// リジット
#define NND_OPT_VTX_TFM_MODE_PLIABLE_B4			(1)		// プライアブル
#define NND_OPT_VTX_TFM_MODE_PLIABLE_B4_MTXID	(2)		// プライアブル
#define NND_OPT_VTX_TFM_MODE_TFM				(3)		// -

/* ライティングをする */
#ifndef NND_OPT_LIGHTING
	#define NND_OPT_LIGHTING					(1)			/* bool */
#endif

/* フラグメント単位ライティングをする */
#ifndef NND_OPT_FRAG_LIGHTING
	#define NND_OPT_FRAG_LIGHTING				(1)			/* bool */
#endif

/* フラグメント単位ライティングパラレルライト数 */
#ifndef NND_OPT_NUM_FRAG_PARALLEL_LIGHT
	#define NND_OPT_NUM_FRAG_PARALLEL_LIGHT		(1)
#endif

/* フラグメント単位ライティングポイントライト数 */
#ifndef NND_OPT_NUM_FRAG_POINT_LIGHT
	#define NND_OPT_NUM_FRAG_POINT_LIGHT		(0)
#endif

/* フラグメント単位ライティングスポットライト数 */
#ifndef NND_OPT_NUM_FRAG_SPOT_LIGHT
	#define NND_OPT_NUM_FRAG_SPOT_LIGHT			(0)
#endif

/* スペキュラ有効 */
#ifndef NND_OPT_SPECULAR
	#define NND_OPT_SPECULAR					(1)			/* bool */
#endif

/* 二面ライティングを行なう */
#ifndef NND_OPT_TWOSIDED_LIGHTING
	#define NND_OPT_TWOSIDED_LIGHTING			(0)			/* bool */
#endif

/* 接線、従法線を計算する */
#ifndef NND_OPT_TANGENT_SPACE
	#define NND_OPT_TANGENT_SPACE				(0)			/* bool */
#endif

/* テクスチャ座標の最大数 */
#ifndef NND_OPT_MAX_TEXCOORD
	#define NND_OPT_MAX_TEXCOORD				(0)			/* int 0～4 */
#endif

/* テクスチャマップの最大数 */
#ifndef NND_OPT_MAX_TEXMAP
	#define NND_OPT_MAX_TEXMAP					(0)
#endif

/* 法線マップインデックス */
#ifndef NND_OPT_TEX_NORMAL
	#define NND_OPT_TEX_NORMAL					(-1)
#endif

/* 法線マップテクスチャ座標種別 */
#ifndef NND_OPT_TEXCOORD_NORMAL
	#define NND_OPT_TEXCOORD_NORMAL				(-1)
#endif

/* ベースディフューズマップインデックス */
#ifndef NND_OPT_TEX_BASE
	#define NND_OPT_TEX_BASE					(-1)
#endif

/* ベースディフューズテクスチャ座標種別 */
#ifndef NND_OPT_TEXCOORD_BASE
	#define NND_OPT_TEXCOORD_BASE				(-1)
#endif

/* デカールマップインデックス */
#ifndef NND_OPT_TEX_DECAL
	#define NND_OPT_TEX_DECAL					(-1)
#endif

/* デカールマップテクスチャ座標種別 */
#ifndef NND_OPT_TEXCOORD_DECAL
	#define NND_OPT_TEXCOORD_DECAL				(-1)
#endif

/* デカールマップ２インデックス */
#ifndef NND_OPT_TEX_DECAL2
	#define NND_OPT_TEX_DECAL2					(-1)
#endif

/* デカールマップ２テクスチャ座標種別 */
#ifndef NND_OPT_TEXCOORD_DECAL2
	#define NND_OPT_TEXCOORD_DECAL2				(-1)
#endif

/* デカールマップ３インデックス */
#ifndef NND_OPT_TEX_DECAL3
	#define NND_OPT_TEX_DECAL3					(-1)
#endif

/* デカールマップ３テクスチャ座標種別 */
#ifndef NND_OPT_TEXCOORD_DECAL3
	#define NND_OPT_TEXCOORD_DECAL3				(-1)
#endif

/* スペキュラマップインデクス */
#ifndef NND_OPT_TEX_SPECULAR
	#define NND_OPT_TEX_SPECULAR				(-1)
#endif

/* スペキュラマップテクスチャ座標種別 */
#ifndef NND_OPT_TEXCOORD_SPECULAR
	#define NND_OPT_TEXCOORD_SPECULAR			(-1)
#endif

/* スペキュラ光沢度マップインデクス */
#ifndef NND_OPT_TEX_SHININESS
	#define NND_OPT_TEX_SHININESS				(-1)
#endif

/* スペキュラ光沢度マップテクスチャ座標種別 */
#ifndef NND_OPT_TEXCOORD_SHININESS
	#define NND_OPT_TEXCOORD_SHININESS			(-1)
#endif

/* 双方物面環境マップインデクス */
#ifndef NND_OPT_TEX_DUALPARABOLOID
	#define NND_OPT_TEX_DUALPARABOLOID			(-1)
#endif

#if 0 /* 2008.06.23(必要ない) */
/* 双方物面環境マップテクスチャ座標種別 */
#ifndef NND_OPT_TEXCOORD_DUALPARABOLOID
	#define NND_OPT_TEXCOORD_DUALPARABOLOID		(-1)
#endif
#endif

/* 環境マスクマップインデクス */
#ifndef NND_OPT_TEX_ENVMASK
	#define NND_OPT_TEX_ENVMASK					(-1)
#endif

/* 環境マスクマップテクスチャ座標種別 */
#ifndef NND_OPT_TEXCOORD_ENVMASK
	#define NND_OPT_TEXCOORD_ENVMASK			(-1)
#endif

/* 乗算テクスチャインデックス */
#ifndef NND_OPT_TEX_MODULATE
	#define NND_OPT_TEX_MODULATE				(-1)
#endif

/* 乗算テクスチャ座標種別 */
#ifndef NND_OPT_TEXCOORD_MODULATE
	#define NND_OPT_TEXCOORD_MODULATE			(-1)
#endif

/* 加算テクスチャインデックス */
#ifndef NND_OPT_TEX_ADD
	#define NND_OPT_TEX_ADD						(-1)
#endif

/* 加算テクスチャ座標種別 */
#ifndef NND_OPT_TEXCOORD_ADD
	#define NND_OPT_TEXCOORD_ADD				(-1)
#endif

/* 不透明度テクスチャインデックス */
#ifndef NND_OPT_TEX_OPACITY
	#define NND_OPT_TEX_OPACITY					(-1)
#endif

/* 不透明度テクスチャ座標インデックス */
#ifndef NND_OPT_TEXCOORD_OPACITY
	#define NND_OPT_TEXCOORD_OPACITY			(-1)
#endif

/* シャドウマップインデックス */
#ifndef NND_OPT_TEX_SHADOW
	#define NND_OPT_TEX_SHADOW					(-1)
#endif

/* シャドウマップ２インデックス */
#ifndef NND_OPT_TEX_SHADOW2
	#define NND_OPT_TEX_SHADOW2					(-1)
#endif

/* ユーザーマップ１インデックス */
#ifndef NND_OPT_TEX_USER1
	#define NND_OPT_TEX_USER1					(-1)
#endif

/* ユーザーマップ１テクスチャ座標インデックス */
#ifndef NND_OPT_TEXCOORD_USER1
	#define NND_OPT_TEXCOORD_USER1				(-1)
#endif

/* ユーザーマップ２インデックス */
#ifndef NND_OPT_TEX_USER2
	#define NND_OPT_TEX_USER2					(-1)
#endif

/* ユーザーマップ２テクスチャ座標インデックス */
#ifndef NND_OPT_TEXCOORD_USER2
	#define NND_OPT_TEXCOORD_USER2				(-1)
#endif

/* ユーザーマップ３インデックス */
#ifndef NND_OPT_TEX_USER3
	#define NND_OPT_TEX_USER3					(-1)
#endif

/* ユーザーマップ３テクスチャ座標インデックス */
#ifndef NND_OPT_TEXCOORD_USER3
	#define NND_OPT_TEXCOORD_USER3				(-1)
#endif

/* ユーザーマップ４インデックス */
#ifndef NND_OPT_TEX_USER4
	#define NND_OPT_TEX_USER4					(-1)
#endif

/* ユーザーマップ４テクスチャ座標インデックス */
#ifndef NND_OPT_TEXCOORD_USER4
	#define NND_OPT_TEXCOORD_USER4				(-1)
#endif

/* ユーザーマップ５インデックス */
#ifndef NND_OPT_TEX_USER5
	#define NND_OPT_TEX_USER5					(-1)
#endif

/* ユーザーマップ５テクスチャ座標インデックス */
#ifndef NND_OPT_TEXCOORD_USER5
	#define NND_OPT_TEXCOORD_USER5				(-1)
#endif

/* ユーザーマップ６インデックス */
#ifndef NND_OPT_TEX_USER6
	#define NND_OPT_TEX_USER6					(-1)
#endif

/* ユーザーマップ６テクスチャ座標インデックス */
#ifndef NND_OPT_TEXCOORD_USER6
	#define NND_OPT_TEXCOORD_USER6				(-1)
#endif

/* ユーザーマップ７インデックス */
#ifndef NND_OPT_TEX_USER7
	#define NND_OPT_TEX_USER7					(-1)
#endif

/* ユーザーマップ７テクスチャ座標インデックス */
#ifndef NND_OPT_TEXCOORD_USER7
	#define NND_OPT_TEXCOORD_USER7				(-1)
#endif

/* ユーザーマップ８インデックス */
#ifndef NND_OPT_TEX_USER8
	#define NND_OPT_TEX_USER8					(-1)
#endif

/* ユーザーマップ８テクスチャ座標インデックス */
#ifndef NND_OPT_TEXCOORD_USER8
	#define NND_OPT_TEXCOORD_USER8				(-1)
#endif

/* ユーザーサンプラー２Ｄマップ１インデックス */
#ifndef NND_OPT_TEX_SAMPLER2D1
	#define NND_OPT_TEX_SAMPLER2D1				(-1)
#endif

/* ユーザーサンプラー２Ｄマップ２インデックス */
#ifndef NND_OPT_TEX_SAMPLER2D2
	#define NND_OPT_TEX_SAMPLER2D2				(-1)
#endif

/* ユーザーサンプラー３Ｄマップ１インデックス */
#ifndef NND_OPT_TEX_SAMPLER3D1
	#define NND_OPT_TEX_SAMPLER3D1				(-1)
#endif

/* ユーザーサンプラー３Ｄマップ２インデックス */
#ifndef NND_OPT_TEX_SAMPLER3D2
	#define NND_OPT_TEX_SAMPLER3D2				(-1)
#endif

/* ユーザーサンプラーキューブマップ１インデックス */
#ifndef NND_OPT_TEX_SAMPLERCUBE1
	#define NND_OPT_TEX_SAMPLERCUBE1			(-1)
#endif

/* ユーザーサンプラーキューブマップ２インデックス */
#ifndef NND_OPT_TEX_SAMPLERCUBE2
	#define NND_OPT_TEX_SAMPLERCUBE2			(-1)
#endif

/****************************/
/* ユーザープロファイル定数 */
/****************************/
#ifndef NND_USER_PROFILE
	#define NND_USER_PROFILE					(0)			/* 0 .. 15 */
#endif

/* スペキュラカラー定数 */
#if NND_OPT_SPECULAR
#define NND_OUTPUT_COLOR_NUM					(2)
#else
#define NND_OUTPUT_COLOR_NUM					(1)
#endif

/* 双方物面マップ */
#if ( NND_OPT_TEX_DUALPARABOLOID != -1 || \
    ( ( NND_OPT_TEXCOORD_ADD == -3 ) || \
      ( NND_OPT_TEXCOORD_BASE == -3 ) || \
      ( NND_OPT_TEXCOORD_DECAL == -3 ) || \
      ( NND_OPT_TEXCOORD_DECAL2 == -3 ) || \
      ( NND_OPT_TEXCOORD_DECAL3 == -3 ) || \
      ( NND_OPT_TEXCOORD_SPECULAR == -3 ) || \
      ( NND_OPT_TEXCOORD_SHININESS == -3 ) || \
      ( NND_OPT_TEXCOORD_MODULATE == -3 ) || \
      ( NND_OPT_TEXCOORD_ADD == -3 ) || \
      ( NND_OPT_TEXCOORD_OPACITY == -3 ) || \
      ( NND_OPT_TEXCOORD_USER1 == -3 ) || \
      ( NND_OPT_TEXCOORD_USER2 == -3 ) || \
      ( NND_OPT_TEXCOORD_USER3 == -3 ) || \
      ( NND_OPT_TEXCOORD_USER4 == -3 ) || \
      ( NND_OPT_TEXCOORD_USER5 == -3 ) || \
      ( NND_OPT_TEXCOORD_USER6 == -3 ) || \
      ( NND_OPT_TEXCOORD_USER7 == -3 ) || \
      ( NND_OPT_TEXCOORD_USER8 == -3 ) ) )
#define NND_FRAG_DUALPARAMAP (1)
#else
#define NND_FRAG_DUALPARAMAP (0)
#endif

/*------ Declarations (出力ＵＶ座標の算出) --------------------------------*/

#include "nnstdshader/nnduv_def.h" /* 2008.06.03(追加) */

/*------ Declarations (セマンティックス番号の算出) ------------------------*/

#define TEXCOORD_TAIL		0

#if NND_OPT_FRAG_LIGHTING || NND_PROF_FOG_MODEL || NND_FRAG_DUALPARAMAP
#define TEXCOORD_VIEWPOSFOG		TEXCOORD0
#undef  TEXCOORD_TAIL
#define TEXCOORD_TAIL			1
#endif

#if NND_OPT_TWOSIDED_LIGHTING
#if   TEXCOORD_TAIL == 0
	#define TEXCOORD_BACKCOL	TEXCOORD0
	#undef  TEXCOORD_TAIL
	#if NND_OPT_SPECULAR
	#define TEXCOORD_TAIL			2
	#else
	#define TEXCOORD_TAIL			1
	#endif
#elif TEXCOORD_TAIL == 1
	#define TEXCOORD_BACKCOL	TEXCOORD1
	#undef  TEXCOORD_TAIL
	#if NND_OPT_SPECULAR
	#define TEXCOORD_TAIL			3
	#else
	#define TEXCOORD_TAIL			2
	#endif
#endif
#endif

#if NND_OPT_FRAG_LIGHTING || NND_FRAG_DUALPARAMAP
#if   TEXCOORD_TAIL == 0
	#define TEXCOORD_NRMVEC		TEXCOORD0
	#undef  TEXCOORD_TAIL
	#define TEXCOORD_TAIL		1
#elif TEXCOORD_TAIL == 1
	#define TEXCOORD_NRMVEC		TEXCOORD1
	#undef  TEXCOORD_TAIL
	#define TEXCOORD_TAIL		2
#elif TEXCOORD_TAIL == 2
	#define TEXCOORD_NRMVEC		TEXCOORD2
	#undef  TEXCOORD_TAIL
	#define TEXCOORD_TAIL		3
#elif TEXCOORD_TAIL == 3
	#define TEXCOORD_NRMVEC		TEXCOORD3
	#undef  TEXCOORD_TAIL
	#define TEXCOORD_TAIL		4
#else
	// TEXCOORD の最大値は８
	#error TEXCOORD_NRMVEC セマンティックスが不正です
#endif
#endif

#if NND_OPT_TANGENT_SPACE
#if   TEXCOORD_TAIL == 0
	#define TEXCOORD_TAN		TEXCOORD0
	#define TEXCOORD_BNRM		TEXCOORD1
	#undef  TEXCOORD_TAIL
	#define TEXCOORD_TAIL		2
#elif TEXCOORD_TAIL == 1
	#define TEXCOORD_TAN		TEXCOORD1
	#define TEXCOORD_BNRM		TEXCOORD2
	#undef  TEXCOORD_TAIL
	#define TEXCOORD_TAIL		3
#elif TEXCOORD_TAIL == 2
	#define TEXCOORD_TAN		TEXCOORD2
	#define TEXCOORD_BNRM		TEXCOORD3
	#undef  TEXCOORD_TAIL
	#define TEXCOORD_TAIL		4
#elif TEXCOORD_TAIL == 3
	#define TEXCOORD_TAN		TEXCOORD3
	#define TEXCOORD_BNRM		TEXCOORD4
	#undef  TEXCOORD_TAIL
	#define TEXCOORD_TAIL		5
#elif TEXCOORD_TAIL == 4
	#define TEXCOORD_TAN		TEXCOORD4
	#define TEXCOORD_BNRM		TEXCOORD5
	#undef  TEXCOORD_TAIL
	#define TEXCOORD_TAIL		6
#else
	// TEXCOORD の最大値は８
	#error TEXCOORD_TAN TEXCOORD_BNRM セマンティックスが不正です
#endif
#endif

#if NND_OPT_MAX_TEXMAP > 0
#if   TEXCOORD_TAIL == 0
	#define TEXCOORD_UV		TEXCOORD0
	#undef  TEXCOORD_TAIL
	#define TEXCOORD_TAIL	( 0 + NND_MAX_UV )
#elif TEXCOORD_TAIL == 1
	#define TEXCOORD_UV		TEXCOORD1
	#undef  TEXCOORD_TAIL
	#define TEXCOORD_TAIL	( 1 + NND_MAX_UV )
#elif TEXCOORD_TAIL == 2
	#define TEXCOORD_UV		TEXCOORD2
	#undef  TEXCOORD_TAIL
	#define TEXCOORD_TAIL	( 2 + NND_MAX_UV )
#elif TEXCOORD_TAIL == 3
	#define TEXCOORD_UV		TEXCOORD3
	#undef  TEXCOORD_TAIL
	#define TEXCOORD_TAIL	( 3 + NND_MAX_UV )
#elif TEXCOORD_TAIL == 4
	#define TEXCOORD_UV		TEXCOORD4
	#undef  TEXCOORD_TAIL
	#define TEXCOORD_TAIL	( 4 + NND_MAX_UV )
#elif TEXCOORD_TAIL == 5
	#define TEXCOORD_UV		TEXCOORD5
	#undef  TEXCOORD_TAIL
	#define TEXCOORD_TAIL	( 5 + NND_MAX_UV )
#elif TEXCOORD_TAIL == 6
	#define TEXCOORD_UV		TEXCOORD6
	#undef  TEXCOORD_TAIL
	#define TEXCOORD_TAIL	( 6 + NND_MAX_UV )
#else
	// TEXCOORD の最大値は８
	#error TEXCOORD_UV セマンティックスが不正です
#endif
#endif

////////////////////////////////////////////////////////////////////////////////

#if ( NND_OPT_TEX_SHADOW != -1 )
#if   TEXCOORD_TAIL == 0
	#define TEXCOORD_SHADOW	TEXCOORD0
	#undef  TEXCOORD_TAIL
	#define TEXCOORD_TAIL	1
#elif TEXCOORD_TAIL == 1
	#define TEXCOORD_SHADOW	TEXCOORD1
	#undef  TEXCOORD_TAIL
	#define TEXCOORD_TAIL	2
#elif TEXCOORD_TAIL == 2
	#define TEXCOORD_SHADOW	TEXCOORD2
	#undef  TEXCOORD_TAIL
	#define TEXCOORD_TAIL	3
#elif TEXCOORD_TAIL == 3
	#define TEXCOORD_SHADOW	TEXCOORD3
	#undef  TEXCOORD_TAIL
	#define TEXCOORD_TAIL	4
#elif TEXCOORD_TAIL == 4
	#define TEXCOORD_SHADOW	TEXCOORD4
	#undef  TEXCOORD_TAIL
	#define TEXCOORD_TAIL	5
#elif TEXCOORD_TAIL == 5
	#define TEXCOORD_SHADOW	TEXCOORD5
	#undef  TEXCOORD_TAIL
	#define TEXCOORD_TAIL	6
#elif TEXCOORD_TAIL == 6
	#define TEXCOORD_SHADOW	TEXCOORD6
	#undef  TEXCOORD_TAIL
	#define TEXCOORD_TAIL	7
#elif TEXCOORD_TAIL == 7
	#define TEXCOORD_SHADOW	TEXCOORD7
	#undef  TEXCOORD_TAIL
	#define TEXCOORD_TAIL	8
#else
	// TEXCOORD の最大値は８
	#error TEXCOORD_UV セマンティックスが不正です
#endif
#endif

#if ( NND_OPT_TEX_SHADOW2 != -1 )
#if   TEXCOORD_TAIL == 0
	#define TEXCOORD_SHADOW2	TEXCOORD0
	#undef  TEXCOORD_TAIL
	#define TEXCOORD_TAIL	1
#elif TEXCOORD_TAIL == 1
	#define TEXCOORD_SHADOW2	TEXCOORD1
	#undef  TEXCOORD_TAIL
	#define TEXCOORD_TAIL	2
#elif TEXCOORD_TAIL == 2
	#define TEXCOORD_SHADOW2	TEXCOORD2
	#undef  TEXCOORD_TAIL
	#define TEXCOORD_TAIL	3
#elif TEXCOORD_TAIL == 3
	#define TEXCOORD_SHADOW2	TEXCOORD3
	#undef  TEXCOORD_TAIL
	#define TEXCOORD_TAIL	4
#elif TEXCOORD_TAIL == 4
	#define TEXCOORD_SHADOW2	TEXCOORD4
	#undef  TEXCOORD_TAIL
	#define TEXCOORD_TAIL	5
#elif TEXCOORD_TAIL == 5
	#define TEXCOORD_SHADOW2	TEXCOORD5
	#undef  TEXCOORD_TAIL
	#define TEXCOORD_TAIL	6
#elif TEXCOORD_TAIL == 6
	#define TEXCOORD_SHADOW2	TEXCOORD6
	#undef  TEXCOORD_TAIL
	#define TEXCOORD_TAIL	7
#elif TEXCOORD_TAIL == 7
	#define TEXCOORD_SHADOW2	TEXCOORD7
	#undef  TEXCOORD_TAIL
	#define TEXCOORD_TAIL	8
#else
	// TEXCOORD の最大値は８
	#error TEXCOORD_UV セマンティックスが不正です
#endif
#endif

////////////////////////////////////////////////////////////////////////////////

/*------ Declarations (共有出力レジスター割り当て) -------------------------*/

#define NND_SUFFIX_VPOS			xyz
#define NND_SUFFIX_FOG			w

/*------ Vertex Shader Input Structure ------------------------------------*/

/* 頂点入力構造体 */
struct VS_INPUT{
	float4 Pos							: POSITION;		// attribute vec4 gl_Vertex
	
#if NND_OPT_VTX_TFM_MODE==NND_OPT_VTX_TFM_MODE_PLIABLE_B4 || NND_OPT_VTX_TFM_MODE==NND_OPT_VTX_TFM_MODE_PLIABLE_B4_MTXID
	float3 Wt							: BLENDWEIGHT0;	// xenon extend
#endif

#if NND_OPT_VTX_TFM_MODE==NND_OPT_VTX_TFM_MODE_PLIABLE_B4_MTXID
	int4 BoneIdx						: BLENDINDICES;
#endif

#if NND_OPT_VTX_NORMAL
	float3 Nrm							: NORMAL;		// attribute vec3 gl_Normal
#endif
	
#if NND_OPT_VTX_COLOR
#if !( NND_PROF_VTX_SPECULAR )
	float4 Col[1]						: COLOR0;		// attribute vec4 gl_Color
#else	
	float4 Col[2]						: COLOR0;		// 0:attribute vec4 gl_Color, 1:attribute vec4 gl_SecondaryColor
#endif
#endif
	
#if NND_OPT_MAX_TEXCOORD > 0
	float2 UV[ NND_OPT_MAX_TEXCOORD ]	: TEXCOORD0;	// attribute vec4 gl_MultiTexCoord[NND_OPT_MAX_TEXCOORD]
#endif
	
#if NND_OPT_TANGENT_SPACE
	float3 Tan							: TANGENT;		// attribute vec3 nnglaTangent
#if !( NND_PROF_CALC_VTX_BINORMAL )
	float3 BNrm							: BINORMAL;		// attribute vec3 nnglaBinormal
#endif
#endif
};

/*------ Vertex Shader Output Structure -----------------------------------*/

/* 頂点シェーダ出力構造体 */
struct VS_OUTPUT{
	float4 Pos								: POSITION;				// varying vec4 gl_Position
	
#if NND_OPT_FRAG_LIGHTING || NND_PROF_FOG_MODEL || NND_FRAG_DUALPARAMAP
	float4 ViewPosFog						: TEXCOORD_VIEWPOSFOG;	// varying vec4 nnglvViewPositionFog;
#endif

#if !( NND_OPT_TWOSIDED_LIGHTING )
	float4 FrontCol[ NND_OUTPUT_COLOR_NUM ]	: COLOR0;				// 0:varying vec4 gl_FrontColor, 1:varying vec4 gl_FrontSecondaryColor
#else
	float4 FrontCol[ NND_OUTPUT_COLOR_NUM ]	: COLOR0;				// 0:varying vec4 gl_FrontColor, 1:varying vec4 gl_FrontSecondaryColor
	float4 BackCol[ NND_OUTPUT_COLOR_NUM ]	: TEXCOORD_BACKCOL;		// 0:varying vec4 gl_BackColor, 1:varying vec4 gl_BackSecondaryColor
#endif

#if ( NND_OPT_FRAG_LIGHTING || NND_FRAG_DUALPARAMAP )
	float3 NrmVec							: TEXCOORD_NRMVEC;		// varying vec3 nnglvNormal;
#endif

#if NND_OPT_TANGENT_SPACE
	float3 Tan								: TEXCOORD_TAN;			// varying vec3 nnglvTangent;
	float3 BNrm								: TEXCOORD_BNRM;		// varying vec3 nnglvBinormal;
#endif

#if NND_UV_NUM > 0
	float4 UV[ NND_MAX_UV ]					: TEXCOORD_UV;			// varying vec4 gl_TexCoord[0..1] 
#endif

#if ( NND_OPT_TEX_SHADOW != -1 )
	float4 TexShadow						: TEXCOORD_SHADOW;
#endif

#if ( NND_OPT_TEX_SHADOW2 != -1 )
	float4 TexShadow2						: TEXCOORD_SHADOW2;
#endif
};

/*------ Fixed Structure --------------------------------------------------*/

/* マテリアル */
struct MATERIAL_PARAMETERS{
	float4	ambient;
	float4	diffuse;
	float4	specular;
	float4	emission;
	float	shininess;
};

/* ライト */
struct LIGHT_SOURCE_PARAMETERS{
	float4	ambient;
	float4	diffuse;
	float4	specular;
	float4	position;
	float4	halfVector;
	float4	spotDirection;
	float	spotExponent;
	float	spotCutoff;
	float	spotCosCutoff;
	float	constantAttenuation;
	float	linearAttenuation;
	float	quadraticAttenuation;
};

/*  */
struct LIGHT_MODEL_PRODUCTS{
	float4	sceneColor;
};


/* フォッグ */
struct FOG_PARAMETERS{
	float4	color;
	float	density;
	float	start;
	float	end;
	float	scale;
};

/*------ Fixed Variable ( GL Built-in ) -----------------------------------*/

#if NND_OPT_VTX_TFM_MODE==NND_OPT_VTX_TFM_MODE_RIGID
uniform float4x4				u_ModelViewMatrix;
uniform float3x3				u_NormalMatrix;
uniform float4x4				u_ModelViewProjectionMatrix;
#else
#if NND_OPT_VTX_TFM_MODE==NND_OPT_VTX_TFM_MODE_PLIABLE_B4
uniform float4x4				u_ModelViewBoneMatrix[4];
uniform float3x3				u_ModelViewBoneNormalMatrix[4];
#elif NND_OPT_VTX_TFM_MODE==NND_OPT_VTX_TFM_MODE_PLIABLE_B4_MTXID
#define NN_MAX_MATRIX_INDEX		(16)
uniform float4x4				u_ModelViewBoneMatrix[NN_MAX_MATRIX_INDEX];
uniform float3x3				u_ModelViewBoneNormalMatrix[NN_MAX_MATRIX_INDEX];
#endif
uniform float4x4				u_ProjectionMatrix;
#endif

#if NND_OPT_MAX_TEXMAP
uniform float4x4				u_TextureMatrix[NND_OPT_MAX_TEXMAP];
#endif

#if NND_PROF_RESCALE_NORMAL
uniform float					u_NormalScale;
#endif

uniform MATERIAL_PARAMETERS		u_FrontMaterial;

#if NND_PROF_MAX_LIGHT
uniform LIGHT_SOURCE_PARAMETERS u_LightSource [NND_PROF_MAX_LIGHT];
#endif

#if !( NND_OPT_FRAG_LIGHTING ) && NND_OPT_LIGHTING
uniform LIGHT_MODEL_PRODUCTS	u_FrontLightModelProduct;
#endif

#if ( !( NND_PROF_FRAG_FOG ) && ( NND_PROF_FOG_MODEL ) )
uniform	FOG_PARAMETERS			u_Fog;
#endif

/*------ Fixed Variable ( GL User-define ) --------------------------------*/

// uniform bool					u_Specular;												// スペキュラ有効
uniform int						u_NumParallelLight;										// 頂点パラレルライト数
uniform int						u_NumPointLight;										// 頂点ポイントライト数
uniform int						u_NumSpotLight;											// 頂点スポットライト数

#if NND_PROF_MAX_POINT_LIGHT
uniform float					u_PointLightFallOffEnd[ NND_PROF_MAX_POINT_LIGHT ];		// ポイントライト減衰終了距離
uniform float					u_PointLightFallOffScale[ NND_PROF_MAX_POINT_LIGHT ];	// ポイントライト減衰スケール
#endif

#if NND_PROF_MAX_SPOT_LIGHT
uniform float					u_SpotLightFallOffEnd[ NND_PROF_MAX_SPOT_LIGHT ];		// スポットライト距離減衰終了距離
uniform float					u_SpotLightFallOffScale[ NND_PROF_MAX_SPOT_LIGHT ];		// スポットライト距離減衰スケール
uniform float					u_SpotLightAngleScale[ NND_PROF_MAX_SPOT_LIGHT ];		// スポットライト角度減衰スケール
#endif

#if NND_OPT_TEX_USER1 != -1
uniform float					u_TexUser1Param;	// ユーザーマップ１パラメータ
#endif
#if NND_OPT_TEX_USER2 != -1
uniform float					u_TexUser2Param;	// ユーザーマップ２パラメータ
#endif
#if NND_OPT_TEX_USER3 != -1
uniform float 					u_TexUser3Param;	// ユーザーマップ３パラメータ
#endif
#if NND_OPT_TEX_USER4 != -1
uniform float 					u_TexUser4Param;	// ユーザーマップ４パラメータ
#endif
#if NND_OPT_TEX_USER5 != -1
uniform float 					u_TexUser5Param;	// ユーザーマップ５パラメータ
#endif
#if NND_OPT_TEX_USER6 != -1
uniform float 					u_TexUser6Param;	// ユーザーマップ６パラメータ
#endif
#if NND_OPT_TEX_USER7 != -1
uniform float 					u_TexUser7Param;	// ユーザーマップ７パラメータ
#endif
#if NND_OPT_TEX_USER8 != -1
uniform float 					u_TexUser8Param;	// ユーザーマップ８パラメータ
#endif

#if ( NND_PROF_MAX_USER_UNIFORM > 0 )
uniform float4    u_UserUniform[NND_PROF_MAX_USER_UNIFORM];	// ユーザーユニフォーム変数
#endif

/*------ Global Variable --------------------------------------------------*/

static  VS_INPUT  g_Input;									// 頂点シェーダー入力
static  VS_OUTPUT g_Output;									// 頂点シェーダー出力

static  float4    g_VertexPosition;							// 変換前頂点座標
static  float4    g_ViewPos;								// 頂点座標のビュー座標上の位置
static  float3    g_Normal;									// 変換済み法線
static  float3    g_EyeDir;									// カメラから頂点への視線方向
static  float3    g_Diffuse  = float3( 0.0, 0.0, 0.0 );		// 計算中ディフューズ寄与度
static  float3    g_Specular = float3( 0.0, 0.0, 0.0 );		// 計算中スペキュラ寄与度

#if NND_OPT_TWOSIDED_LIGHTING
static  float3    g_BackDiffuse  = float3( 0.0, 0.0, 0.0 );	// 計算中裏面ディフューズ寄与度
static  float3    g_BackSpecular = float3( 0.0, 0.0, 0.0 );	// 計算中裏面スペキュラ寄与度
#endif

#if NND_PROF_LIGHT_AMBIENT
static  float3    g_Ambient = float3( 0.0, 0.0, 0.0 );		// 計算中アンビエント寄与度
#endif

#if NND_OPT_TANGENT_SPACE
static  float3    g_Tangent;								// 変換済み接線
static  float3    g_Binormal;								// 変換済み従法線
#endif

/*------ Functions --------------------------------------------------------*/

/* ボーンマトリックスの統合 */

#define _UnityMulMtxAndWt(_rt,_m0,_m1,_m2,_m3,_w0,_w1,_w2,_w3) \
 _rt  = 0.0f; \
 _rt += _m0*(_w0); \
 _rt += _m1*(_w1); \
 _rt += _m2*(_w2); \
 _rt += _m3*(_w3);

#define _UnityMulMtxAryAndWt(_rt,_m,_w0,_w1,_w2,_w3) \
 _UnityMulMtxAndWt(_rt,_m[0],_m[1],_m[2],_m[3],_w0,_w1,_w2,_w3) \
 _rt[3].w = 1.0f;

#define _UnityMulMtxIdAryAndWt(_rt,_m,_mid,_w0,_w1,_w2,_w3) \
 _UnityMulMtxAndWt(_rt,_m[_mid[0]],_m[_mid[1]],_m[_mid[2]],_m[_mid[3]],_w0,_w1,_w2,_w3) \
 _rt[3].w = 1.0f;

#define _UnityMulMtx33AryAndWt(_rt,_m,_w0,_w1,_w2,_w3) \
 _UnityMulMtxAndWt(_rt,_m[0],_m[1],_m[2],_m[3],_w0,_w1,_w2,_w3)

#define _UnityMulMtx33IdAryAndWt(_rt,_m,_mid,_w0,_w1,_w2,_w3) \
 _UnityMulMtxAndWt(_rt,_m[_mid[0]],_m[_mid[1]],_m[_mid[2]],_m[_mid[3]],_w0,_w1,_w2,_w3)


#if NND_OPT_VTX_TFM_MODE==NND_OPT_VTX_TFM_MODE_PLIABLE_B4

#if ( NND_PROF_NO_SCALE_ENVELOPE )
float4x4 CalcUnityBoneMtx(float4x4 Bone[4], float3 Wt)
{
	float4x4 rt;
	_UnityMulMtxAryAndWt(rt, Bone, Wt[0], Wt[1], Wt[2], 1.0f-(Wt[0]+Wt[1]+Wt[2]));
	return(rt);
}
#else
void CalcUnityBoneMtx(float4x4 BoneMtx[4], float3x3 BoneNmMtx[4], float3 Wt,
					  out float4x4 outBoneMtx, out float3x3 outBoneNmMtx)
{
	float Wt3 = 1.0f - ( Wt[0] + Wt[1] + Wt[2] );
	_UnityMulMtxAryAndWt(outBoneMtx, BoneMtx, Wt[0], Wt[1], Wt[2], Wt3);
	_UnityMulMtx33AryAndWt(outBoneNmMtx, BoneNmMtx, Wt[0], Wt[1], Wt[2], Wt3);
}
#endif

#elif NND_OPT_VTX_TFM_MODE==NND_OPT_VTX_TFM_MODE_PLIABLE_B4_MTXID

#if ( NND_PROF_NO_SCALE_ENVELOPE )
float4x4 CalcUnityBoneMtx( float4x4 Bone[NN_MAX_MATRIX_INDEX], float3 Wt, int4 BoneIdx )
{
	float4x4 rt;
	_UnityMulMtxIdAryAndWt(rt, Bone, BoneIdx, Wt[0], Wt[1], Wt[2], 1.0f-(Wt[0]+Wt[1]+Wt[2]));
	return(rt);
}
#else
void CalcUnityBoneMtx(float4x4 BoneMtx[NN_MAX_MATRIX_INDEX], float3x3 BoneNmMtx[NN_MAX_MATRIX_INDEX], float3 Wt, int4 BoneIdx,
					  out float4x4 outBoneMtx, out float3x3 outBoneNmMtx)
{
	float Wt3 = 1.0f - ( Wt[0] + Wt[1] + Wt[2] );
	_UnityMulMtxIdAryAndWt(outBoneMtx, BoneMtx, BoneIdx, Wt[0], Wt[1], Wt[2], Wt3);
	_UnityMulMtx33IdAryAndWt(outBoneNmMtx, BoneNmMtx, BoneIdx, Wt[0], Wt[1], Wt[2], Wt3);
}
#endif

#endif

/********************/
/* 頂点位置法線変換 */
/********************/
void TransformVertexPositionNormal( void )
{
	/* 変換前頂点座標 */
	g_VertexPosition = float4(g_Input.Pos.xyz / g_Input.Pos.w, 1.0f );


#if NND_OPT_VTX_OBJECT_SPACE_NORMAL
	/* オブジェクトスペース法線 */
	float3 l_Nrm = float3(0.0, 0.0, 1.0);
#else
	#if NND_OPT_VTX_NORMAL
		/* 法線があるとき */
		float3 l_Nrm = g_Input.Nrm;
	#else
		/* 法線がないとき */
		float3 l_Nrm = float3(0.0, 0.0, 1.0); /* 0 は良くないと思うので取りあえず */
	#endif
#endif

#if NND_OPT_TANGENT_SPACE
	/* 法線マップ用の接線と従法線 */
	#if NND_OPT_VTX_OBJECT_SPACE_NORMAL
		/* オブジェクトスペース法線 */
		#if NND_OPT_VTX_REVERSE_TANGENT	/* オブジェクトスペース法線のときのみ使用可能 */
			float3 l_Tan  = float3(-1.0, 0.0, 0.0);	/* 接線の反転 */
		#else
			float3 l_Tan  = float3(1.0, 0.0, 0.0);
		#endif

		#if !( NND_PROF_CALC_VTX_BINORMAL )	/* 従法線計算時は必要なし */
			float3 l_BNrm = float3(0.0, 1.0, 0.0);
		#endif
	#else
		/* タンジェントスペース法線 */
		float3 l_Tan  = float3(g_Input.Tan);
		#if !( NND_PROF_CALC_VTX_BINORMAL )	/* 従法線計算時は必要なし */
			float3 l_BNrm = g_Input.BNrm;
		#endif
	#endif
#endif


#if NND_OPT_VTX_TFM_MODE==NND_OPT_VTX_TFM_MODE_RIGID
	/* エンベロープでない場合 */

	/* 頂点位置変換 */
	g_Output.Pos = mul( g_Input.Pos, u_ModelViewProjectionMatrix );

	/* ビュー座標での頂点位置 */
	g_ViewPos = mul( g_VertexPosition, u_ModelViewMatrix );

	/* 法線変換 */
	g_Normal = mul( l_Nrm, u_NormalMatrix );

	/* 法線マップ用属性変換 */
	#if NND_OPT_TANGENT_SPACE
		/* 接線変換 */
		g_Tangent =  mul( l_Tan, u_NormalMatrix );
		#if NND_PROF_CALC_VTX_BINORMAL
			/* 従法線計算 */
			g_Binormal = cross( g_Normal, g_Tangent );
		#else
			g_Binormal = mul( l_BNrm, u_NormalMatrix );
		#endif
	#endif

#else
	/* エンベロープの場合 */

	/* ボーン統合マトリックスの算出 */
	#if NND_OPT_VTX_TFM_MODE==NND_OPT_VTX_TFM_MODE_PLIABLE_B4
	    #if (NND_PROF_NO_SCALE_ENVELOPE)
			float4x4 UnityBoneMtx = CalcUnityBoneMtx( u_ModelViewBoneMatrix, g_Input.Wt );
			#define UnityBoneNmMtx (float3x3)UnityBoneMtx
		#else
			float4x4 UnityBoneMtx;
			float3x3 UnityBoneNmMtx;
			CalcUnityBoneMtx(u_ModelViewBoneMatrix, u_ModelViewBoneNormalMatrix, g_Input.Wt, UnityBoneMtx, UnityBoneNmMtx);
		#endif
	#else
	    #if (NND_PROF_NO_SCALE_ENVELOPE)
			float4x4 UnityBoneMtx = CalcUnityBoneMtx( u_ModelViewBoneMatrix, g_Input.Wt, g_Input.BoneIdx );
        	#define UnityBoneNmMtx (float3x3)UnityBoneMtx
		#else
			float4x4 UnityBoneMtx;
			float3x3 UnityBoneNmMtx;
			CalcUnityBoneMtx(u_ModelViewBoneMatrix, u_ModelViewBoneNormalMatrix, g_Input.Wt, g_Input.BoneIdx, UnityBoneMtx, UnityBoneNmMtx);
		#endif
	#endif

	/* ビュー座標での頂点位置 */
	g_ViewPos = mul( g_Input.Pos, UnityBoneMtx );

	/* 頂点位置変換 */
	g_Output.Pos = mul( g_ViewPos, u_ProjectionMatrix );

	/* 2008.09.19(抜けていたので修正) */
	g_ViewPos = float4(g_ViewPos.xyz / g_ViewPos.w, 1.0);

	/* 法線変換 */
	g_Normal = mul( l_Nrm, UnityBoneNmMtx );

	/* 法線マップ用属性変換 */
	#if NND_OPT_TANGENT_SPACE
		/* 接線変換 */
		g_Tangent =  mul( l_Tan, UnityBoneNmMtx );
		#if NND_PROF_CALC_VTX_BINORMAL
			/* 従法線計算 */
			g_Binormal = cross( g_Normal, g_Tangent );
		#else
			g_Binormal = mul( l_BNrm, UnityBoneNmMtx );
		#endif
	#endif

#endif

	/* カメラから頂点への視線方向 */
	g_EyeDir = normalize( g_ViewPos.xyz );

	/* 正規化 */
	#if NND_PROF_NORMALIZE_VTX_NORMAL
		g_Normal = normalize( g_Normal );
		#if NND_OPT_TANGENT_SPACE
			g_Tangent = normalize( g_Tangent );
			g_Binormal = normalize( g_Binormal );
		#endif
	#endif
	/* 再スケール */
	#if NND_PROF_RESCALE_NORMAL
		g_Normal *= u_NormalScale;
		#if NND_OPT_TANGENT_SPACE
			g_Tangent *= u_NormalScale;
			g_Binormal *= u_NormalScale;
		#endif
	#endif
}

/* スフィアマップ */
float4 CalcSphereMapTexCoord( void )
{
	float3 r;
	float2 r2;
	r = reflect( g_EyeDir, g_Normal );
	r.z += 1.0;
	r2 = ( normalize( r ).xy ) * float2( 0.5, 0.5 ) + float2( 0.5, 0.5 );
	return float4( r2.xy, 0.0, 1.0 );
}

/**********************/
/* テクスチャ座標計算 */
/**********************/
void CalcTextureCoordinate( void )
{
	/* 法線マップ */
	#if ( NND_OPT_TEX_NORMAL != -1 )
		/* UVマップの場合 */
		#if ( 0 <= NND_OPT_TEXCOORD_NORMAL && NND_OPT_TEXCOORD_NORMAL <=3 )
			g_Output.UV[ NND_UV_NO_NORMAL ].NND_UV_SUFFIX_NORMAL
				= mul( float3( g_Input.UV[ NND_OPT_TEXCOORD_NORMAL ].x,g_Input.UV[ NND_OPT_TEXCOORD_NORMAL ].y,1.0f ), u_TextureMatrix[ NND_OPT_TEX_NORMAL ] ).xy;
		/* スフィアマップの場合 */
		#elif ( NND_OPT_TEXCOORD_NORMAL == -1 )
			g_Output.UV[ NND_UV_NO_NORMAL ].NND_UV_SUFFIX_NORMAL
				= mul( CalcSphereMapTexCoord(), u_TextureMatrix[ NND_OPT_TEX_NORMAL ] ).xy;
		/* プロジェクションマップの場合 */
		#elif ( NND_OPT_TEXCOORD_NORMAL == -2 )
			g_Output.UV[ NND_UV_NO_NORMAL ].NND_UV_SUFFIX_NORMAL
				= mul( g_ViewPos, u_TextureMatrix[ NND_OPT_TEX_NORMAL ] ).xy;
		#endif
	#endif

	/* ベースディフューズマップ */
	#if ( NND_OPT_TEX_BASE != -1 )
		/* UVマップの場合 */
		#if ( 0 <= NND_OPT_TEXCOORD_BASE && NND_OPT_TEXCOORD_BASE <= 3 )
			g_Output.UV[ NND_UV_NO_BASE ].NND_UV_SUFFIX_BASE
			 	= mul( float3(g_Input.UV[ NND_OPT_TEXCOORD_BASE ].x,g_Input.UV[ NND_OPT_TEXCOORD_BASE ].y,1.0f), u_TextureMatrix[ NND_OPT_TEX_BASE ] ).xy;
		/* スフィアマップの場合 */
		#elif ( NND_OPT_TEXCOORD_BASE == -1 )
			g_Output.UV[ NND_UV_NO_BASE ].NND_UV_SUFFIX_BASE
				= mul( CalcSphereMapTexCoord(), u_TextureMatrix[ NND_OPT_TEX_BASE ] ).xy;
		/* プロジェクションマップの場合 */
		#elif ( NND_OPT_TEXCOORD_BASE == -2 )
			g_Output.UV[ NND_UV_NO_BASE ].NND_UV_SUFFIX_BASE
				= mul( g_ViewPos, u_TextureMatrix[ NND_OPT_TEX_BASE ] ).xy;
		#endif
	#endif

	/* デカールマップ１ */
	#if ( NND_OPT_TEX_DECAL != -1 )
		/* UVマップの場合 */
		#if ( 0 <= NND_OPT_TEXCOORD_DECAL && NND_OPT_TEXCOORD_DECAL <= 3 )
			g_Output.UV[ NND_UV_NO_DECAL ].NND_UV_SUFFIX_DECAL
				= mul( float3(g_Input.UV[ NND_OPT_TEXCOORD_DECAL ].x,g_Input.UV[ NND_OPT_TEXCOORD_DECAL ].y,1.0f), u_TextureMatrix[ NND_OPT_TEX_DECAL ] ).xy;
		/* スフィアマップの場合 */
		#elif ( NND_OPT_TEXCOORD_DECAL == -1 )
			g_Output.UV[ NND_UV_NO_DECAL ].NND_UV_SUFFIX_DECAL
				= mul( CalcSphereMapTexCoord(), u_TextureMatrix[ NND_OPT_TEX_DECAL ] ).xy;
		/* プロジェクションマップの場合 */
		#elif ( NND_OPT_TEXCOORD_DECAL == -2 )
			g_Output.UV[ NND_UV_NO_DECAL ].NND_UV_SUFFIX_DECAL
				= mul( g_ViewPos, u_TextureMatrix[ NND_OPT_TEX_DECAL ] ).xy;
		#endif
	#endif

	/* デカールマップ２ */
	#if ( NND_OPT_TEX_DECAL2 != -1 )
		/* UVマップの場合 */
		#if ( 0 <= NND_OPT_TEXCOORD_DECAL2 && NND_OPT_TEXCOORD_DECAL2 <= 3 )
			g_Output.UV[ NND_UV_NO_DECAL2 ].NND_UV_SUFFIX_DECAL2
				= mul( float3(g_Input.UV[ NND_OPT_TEXCOORD_DECAL2 ].x,g_Input.UV[ NND_OPT_TEXCOORD_DECAL2 ].y,1.0f), u_TextureMatrix[ NND_OPT_TEX_DECAL2 ] ).xy;
		/* スフィアマップの場合 */
		#elif ( NND_OPT_TEXCOORD_DECAL2 == -1 )
			g_Output.UV[ NND_UV_NO_DECAL2 ].NND_UV_SUFFIX_DECAL2
				= mul( CalcSphereMapTexCoord(), u_TextureMatrix[ NND_OPT_TEX_DECAL2 ] ).xy;
		/* プロジェクションマップの場合 */
		#elif ( NND_OPT_TEXCOORD_DECAL2 == -2 )
			g_Output.UV[ NND_UV_NO_DECAL2 ].NND_UV_SUFFIX_DECAL2
				= mul( g_ViewPos, u_TextureMatrix[ NND_OPT_TEX_DECAL2 ] ).xy;
		#endif
	#endif

	/* デカールマップ３ */
	#if ( NND_OPT_TEX_DECAL3 != -1 )
		/* UVマップの場合 */
		#if ( 0 <= NND_OPT_TEXCOORD_DECAL3 && NND_OPT_TEXCOORD_DECAL3 <= 3 )
			g_Output.UV[ NND_UV_NO_DECAL3 ].NND_UV_SUFFIX_DECAL3
				= mul( float3(g_Input.UV[ NND_OPT_TEXCOORD_DECAL3 ].x,g_Input.UV[ NND_OPT_TEXCOORD_DECAL3 ].y,1.0f), u_TextureMatrix[ NND_OPT_TEX_DECAL3 ] ).xy;
		/* スフィアマップの場合 */
		#elif ( NND_OPT_TEXCOORD_DECAL3 == -1 )
			g_Output.UV[ NND_UV_NO_DECAL3 ].NND_UV_SUFFIX_DECAL3
				= mul( CalcSphereMapTexCoord(), u_TextureMatrix[ NND_OPT_TEX_DECAL3 ] ).xy;
		/* プロジェクションマップの場合 */
		#elif ( NND_OPT_TEXCOORD_DECAL3 == -2 )
			g_Output.UV[ NND_UV_NO_DECAL3 ].NND_UV_SUFFIX_DECAL3
				= mul( g_ViewPos, u_TextureMatrix[ NND_OPT_TEX_DECAL3 ] ).xy;
		#endif
	#endif

	/* スペキュラマップ */
	#if ( NND_OPT_TEX_SPECULAR != -1 )
		/* UVマップの場合 */
		#if ( 0 <= NND_OPT_TEXCOORD_SPECULAR && NND_OPT_TEXCOORD_SPECULAR <= 3 )
			g_Output.UV[ NND_UV_NO_SPECULAR ].NND_UV_SUFFIX_SPECULAR
				= mul( float3(g_Input.UV[ NND_OPT_TEXCOORD_SPECULAR ].x,g_Input.UV[ NND_OPT_TEXCOORD_SPECULAR ].y,1.0f), u_TextureMatrix[ NND_OPT_TEX_SPECULAR ] ).xy;
		/* スフィアマップの場合 */
		#elif ( NND_OPT_TEXCOORD_SPECULAR == -1 )
			g_Output.UV[ NND_UV_NO_SPECULAR ].NND_UV_SUFFIX_SPECULAR
				= mul( CalcSphereMapTexCoord(), u_TextureMatrix[ NND_OPT_TEX_SPECULAR ] ).xy;
		/* プロジェクションマップの場合 */
		#elif ( NND_OPT_TEXCOORD_SPECULAR == -2 )
			g_Output.UV[ NND_UV_NO_SPECULAR ].NND_UV_SUFFIX_SPECULAR
				= mul( g_ViewPos, u_TextureMatrix[ NND_OPT_TEX_SPECULAR ] ).xy;
		#endif
	#endif

	/* スペキュラ光沢度マップ */
	#if ( NND_OPT_TEX_SHININESS != -1 )
		/* UVマップの場合 */
		#if ( 0 <= NND_OPT_TEXCOORD_SHININESS  && NND_OPT_TEXCOORD_SHININESS <= 3 )
			g_Output.UV[ NND_UV_NO_SHININESS ].NND_UV_SUFFIX_SHININESS
				= mul( float3(g_Input.UV[ NND_OPT_TEXCOORD_SHININESS ].x,g_Input.UV[ NND_OPT_TEXCOORD_SHININESS ].y,1.0f), u_TextureMatrix[ NND_OPT_TEX_SHININESS ] ).xy;
		/* スフィアマップの場合 */
		#elif ( NND_OPT_TEXCOORD_SHININESS == -1 )
			g_Output.UV[ NND_UV_NO_SHININESS ].NND_UV_SUFFIX_SHININESS
				= mul( CalcSphereMapTexCoord(), u_TextureMatrix[ NND_OPT_TEX_SHININESS ] ).xy;
		/* プロジェクションマップの場合 */
		#elif ( NND_OPT_TEXCOORD_SHININESS == -2 )
			g_Output.UV[ NND_UV_NO_SHININESS ].NND_UV_SUFFIX_SHININESS
				= mul( g_ViewPos, u_TextureMatrix[ NND_OPT_TEX_SHININESS ] ).xy;
		#endif
	#endif

	/* 双方物面環境マップ */
	/* テクスチャ座標生成はしない */

	/* 環境マスクマップ */
	#if ( NND_OPT_TEX_ENVMASK != -1 )
		/* UVマップの場合 */
		#if ( 0 <= NND_OPT_TEXCOORD_ENVMASK && NND_OPT_TEXCOORD_ENVMASK <= 3 )
			g_Output.UV[ NND_UV_NO_ENVMASK ].NND_UV_SUFFIX_ENVMASK
				= mul( float3(g_Input.UV[ NND_OPT_TEXCOORD_ENVMASK ].x,g_Input.UV[ NND_OPT_TEXCOORD_ENVMASK ].y,1.0f), u_TextureMatrix[ NND_OPT_TEX_ENVMASK ] ).xy;
		/* スフィアマップの場合 */
		#elif ( NND_OPT_TEXCOORD_ENVMASK == -1 )
			g_Output.UV[ NND_UV_NO_ENVMASK ].NND_UV_SUFFIX_ENVMASK
				= mul( CalcSphereMapTexCoord(), u_TextureMatrix[ NND_OPT_TEX_ENVMASK ] ).xy;
		/* プロジェクションマップの場合 */
		#elif ( NND_OPT_TEXCOORD_ENVMASK == -2 )
			g_Output.UV[ NND_UV_NO_ENVMASK ].NND_UV_SUFFIX_ENVMASK
				= mul( g_ViewPos, u_TextureMatrix[ NND_OPT_TEX_ENVMASK ] ).xy;
		#endif
	#endif

	/* 乗算テクスチャ */
	#if ( NND_OPT_TEX_MODULATE != -1 )
		/* UVマップの場合 */
		#if ( 0 <= NND_OPT_TEXCOORD_MODULATE && NND_OPT_TEXCOORD_MODULATE <= 3 )
			g_Output.UV[ NND_UV_NO_MODULATE ].NND_UV_SUFFIX_MODULATE
				= mul( float3(g_Input.UV[ NND_OPT_TEXCOORD_MODULATE ].x,g_Input.UV[ NND_OPT_TEXCOORD_MODULATE ].y,1.0f), u_TextureMatrix[ NND_OPT_TEX_MODULATE ] ).xy;
		/* スフィアマップの場合 */
		#elif ( NND_OPT_TEXCOORD_MODULATE == -1 )
			g_Output.UV[ NND_UV_NO_MODULATE ].NND_UV_SUFFIX_MODULATE
				= mul( CalcSphereMapTexCoord(), u_TextureMatrix[ NND_OPT_TEX_MODULATE ] ).xy;
		/* プロジェクションマップの場合 */
		#elif ( NND_OPT_TEXCOORD_MODULATE == -2 )
			g_Output.UV[ NND_UV_NO_MODULATE ].NND_UV_SUFFIX_MODULATE
				= mul( g_ViewPos, u_TextureMatrix[ NND_OPT_TEX_MODULATE ] ).xy;
		#endif
	#endif

	/* 加算テクスチャ */
	#if ( NND_OPT_TEX_ADD != -1 )
		/* UVマップの場合 */
		#if ( 0 <= NND_OPT_TEXCOORD_ADD && NND_OPT_TEXCOORD_ADD <= 3 )
			g_Output.UV[ NND_UV_NO_ADD ].NND_UV_SUFFIX_ADD
				= mul( float3(g_Input.UV[ NND_OPT_TEXCOORD_ADD ].x,g_Input.UV[ NND_OPT_TEXCOORD_ADD ].y,1.0f), u_TextureMatrix[ NND_OPT_TEX_ADD ] ).xy;
		/* スフィアマップの場合 */
		#elif ( NND_OPT_TEXCOORD_ADD == -1 )
			g_Output.UV[ NND_UV_NO_ADD ].NND_UV_SUFFIX_ADD
				= mul( CalcSphereMapTexCoord(), u_TextureMatrix[ NND_OPT_TEX_ADD ] ).xy;
		/* プロジェクションマップの場合 */
		#elif ( NND_OPT_TEXCOORD_ADD == -2 )
			g_Output.UV[ NND_UV_NO_ADD ].NND_UV_SUFFIX_ADD
				= mul( g_ViewPos, u_TextureMatrix[ NND_OPT_TEX_ADD ] ).xy;
		#endif
	#endif

	/* 不透明度テクスチャ */
	#if ( NND_OPT_TEX_OPACITY != -1 )
		/* UVマップの場合 */
		#if ( 0 <= NND_OPT_TEXCOORD_OPACITY && NND_OPT_TEXCOORD_OPACITY <= 3 )
			g_Output.UV[ NND_UV_NO_OPACITY ].NND_UV_SUFFIX_OPACITY
				= mul( float3(g_Input.UV[ NND_OPT_TEXCOORD_OPACITY ].x,g_Input.UV[ NND_OPT_TEXCOORD_OPACITY ].y,1.0f), u_TextureMatrix[ NND_OPT_TEX_OPACITY ] ).xy;
		/* スフィアマップの場合 */
		#elif ( NND_OPT_TEXCOORD_OPACITY == -1 )
			g_Output.UV[ NND_UV_NO_OPACITY ].NND_UV_SUFFIX_OPACITY
				= mul( CalcSphereMapTexCoord(), u_TextureMatrix[ NND_OPT_TEX_OPACITY ] ).xy;
		/* プロジェクションマップの場合 */
		#elif ( NND_OPT_TEXCOORD_OPACITY == -2 )
			g_Output.UV[ NND_UV_NO_OPACITY ].NND_UV_SUFFIX_OPACITY
				= mul( g_ViewPos, u_TextureMatrix[ NND_OPT_TEX_OPACITY ] ).xy;
		#endif
	#endif
	
	/* ユーザーマップ１ */
	#if ( NND_OPT_TEX_USER1 != -1 )
		/* UVマップの場合 */
		#if ( 0 <= NND_OPT_TEXCOORD_USER1 && NND_OPT_TEXCOORD_USER1 <= 3 )
			g_Output.UV[ NND_UV_NO_USER1 ].NND_UV_SUFFIX_USER1
				= mul( float3(g_Input.UV[ NND_OPT_TEXCOORD_USER1 ].x,g_Input.UV[ NND_OPT_TEXCOORD_USER1 ].y,1.0f), u_TextureMatrix[ NND_OPT_TEX_USER1 ] ).xy;
		/* スフィアマップの場合 */
		#elif ( NND_OPT_TEXCOORD_USER1 == -1 )
			g_Output.UV[ NND_UV_NO_USER1 ].NND_UV_SUFFIX_USER1
				= mul( CalcSphereMapTexCoord(), u_TextureMatrix[ NND_OPT_TEX_USER1 ] ).xy;
		/* プロジェクションマップの場合 */
		#elif ( NND_OPT_TEXCOORD_USER1 == -2 )
			g_Output.UV[ NND_UV_NO_USER1 ].NND_UV_SUFFIX_USER1
				= mul( g_ViewPos, u_TextureMatrix[ NND_OPT_TEX_USER1 ] ).xy;
		#endif
	#endif
	
	/* ユーザーマップ２ */
	#if ( NND_OPT_TEX_USER2 != -1 )
		/* UVマップの場合 */
		#if ( 0 <= NND_OPT_TEXCOORD_USER2 && NND_OPT_TEXCOORD_USER2 <= 3 )
			g_Output.UV[ NND_UV_NO_USER2 ].NND_UV_SUFFIX_USER2
				= mul( float3(g_Input.UV[ NND_OPT_TEXCOORD_USER2 ].x,g_Input.UV[ NND_OPT_TEXCOORD_USER2 ].y,1.0f), u_TextureMatrix[ NND_OPT_TEX_USER2 ] ).xy;
		/* スフィアマップの場合 */
		#elif ( NND_OPT_TEXCOORD_USER2 == -1 )
			g_Output.UV[ NND_UV_NO_USER2 ].NND_UV_SUFFIX_USER2
				= mul( CalcSphereMapTexCoord(), u_TextureMatrix[ NND_OPT_TEX_USER2 ] ).xy;
		/* プロジェクションマップの場合 */
		#elif ( NND_OPT_TEXCOORD_USER2 == -2 )
			g_Output.UV[ NND_UV_NO_USER2 ].NND_UV_SUFFIX_USER2
				= mul( g_ViewPos, u_TextureMatrix[ NND_OPT_TEX_USER2 ] ).xy;
		#endif
	#endif
	
	/* ユーザーマップ３ */
	#if ( NND_OPT_TEX_USER3 != -1 )
		/* UVマップの場合 */
		#if ( 0 <= NND_OPT_TEXCOORD_USER3 && NND_OPT_TEXCOORD_USER3 <= 3 )
			g_Output.UV[ NND_UV_NO_USER3 ].NND_UV_SUFFIX_USER3
				= mul( float3(g_Input.UV[ NND_OPT_TEXCOORD_USER3 ].x,g_Input.UV[ NND_OPT_TEXCOORD_USER3 ].y,1.0f), u_TextureMatrix[ NND_OPT_TEX_USER3 ] ).xy;
		/* スフィアマップの場合 */
		#elif ( NND_OPT_TEXCOORD_USER3 == -1 )
			g_Output.UV[ NND_UV_NO_USER3 ].NND_UV_SUFFIX_USER3
				= mul( CalcSphereMapTexCoord(), u_TextureMatrix[ NND_OPT_TEX_USER3 ] ).xy;
		/* プロジェクションマップの場合 */
		#elif ( NND_OPT_TEXCOORD_USER3 == -2 )
			g_Output.UV[ NND_UV_NO_USER3 ].NND_UV_SUFFIX_USER3
				= mul( g_ViewPos, u_TextureMatrix[ NND_OPT_TEX_USER3 ] ).xy;
		#endif
	#endif
	
	/* ユーザーマップ４ */
	#if ( NND_OPT_TEX_USER4 != -1 )
		/* UVマップの場合 */
		#if ( 0 <= NND_OPT_TEXCOORD_USER4 && NND_OPT_TEXCOORD_USER4 <= 3 )
			g_Output.UV[ NND_UV_NO_USER4 ].NND_UV_SUFFIX_USER4
				= mul( float3(g_Input.UV[ NND_OPT_TEXCOORD_USER4 ].x,g_Input.UV[ NND_OPT_TEXCOORD_USER4 ].y,1.0f), u_TextureMatrix[ NND_OPT_TEX_USER4 ] ).xy;
		/* スフィアマップの場合 */
		#elif ( NND_OPT_TEXCOORD_USER4 == -1 )
			g_Output.UV[ NND_UV_NO_USER4 ].NND_UV_SUFFIX_USER4
				= mul( CalcSphereMapTexCoord(), u_TextureMatrix[ NND_OPT_TEX_USER4 ] ).xy;
		/* プロジェクションマップの場合 */
		#elif ( NND_OPT_TEXCOORD_USER4 == -2 )
			g_Output.UV[ NND_UV_NO_USER4 ].NND_UV_SUFFIX_USER4
				= mul( g_ViewPos, u_TextureMatrix[ NND_OPT_TEX_USER4 ] ).xy;
		#endif
	#endif
	
	/* ユーザーマップ５ */
	#if ( NND_OPT_TEX_USER5 != -1 )
		/* UVマップの場合 */
		#if ( 0 <= NND_OPT_TEXCOORD_USER5 && NND_OPT_TEXCOORD_USER5 <= 3 )
			g_Output.UV[ NND_UV_NO_USER5 ].NND_UV_SUFFIX_USER5
				= mul( float3(g_Input.UV[ NND_OPT_TEXCOORD_USER5 ].x,g_Input.UV[ NND_OPT_TEXCOORD_USER5 ].y,1.0f), u_TextureMatrix[ NND_OPT_TEX_USER5 ] ).xy;
		/* スフィアマップの場合 */
		#elif ( NND_OPT_TEXCOORD_USER5 == -1 )
			g_Output.UV[ NND_UV_NO_USER5 ].NND_UV_SUFFIX_USER5
				= mul( CalcSphereMapTexCoord(), u_TextureMatrix[ NND_OPT_TEX_USER5 ] ).xy;
		/* プロジェクションマップの場合 */
		#elif ( NND_OPT_TEXCOORD_USER5 == -2 )
			g_Output.UV[ NND_UV_NO_USER5 ].NND_UV_SUFFIX_USER5
				= mul( g_ViewPos, u_TextureMatrix[ NND_OPT_TEX_USER5 ] ).xy;
		#endif
	#endif
	
	/* ユーザーマップ６ */
	#if ( NND_OPT_TEX_USER6 != -1 )
		/* UVマップの場合 */
		#if ( 0 <= NND_OPT_TEXCOORD_USER6 && NND_OPT_TEXCOORD_USER6 <= 3 )
			g_Output.UV[ NND_UV_NO_USER6 ].NND_UV_SUFFIX_USER6
				= mul( float3(g_Input.UV[ NND_OPT_TEXCOORD_USER6 ].x,g_Input.UV[ NND_OPT_TEXCOORD_USER6 ].y,1.0f), u_TextureMatrix[ NND_OPT_TEX_USER6 ] ).xy;
		/* スフィアマップの場合 */
		#elif ( NND_OPT_TEXCOORD_USER6 == -1 )
			g_Output.UV[ NND_UV_NO_USER6 ].NND_UV_SUFFIX_USER6
				= mul( CalcSphereMapTexCoord(), u_TextureMatrix[ NND_OPT_TEX_USER6 ] ).xy;
		/* プロジェクションマップの場合 */
		#elif ( NND_OPT_TEXCOORD_USER6 == -2 )
			g_Output.UV[ NND_UV_NO_USER6 ].NND_UV_SUFFIX_USER6
				= mul( g_ViewPos, u_TextureMatrix[ NND_OPT_TEX_USER6 ] ).xy;
		#endif
	#endif
	
	/* ユーザーマップ７ */
	#if ( NND_OPT_TEX_USER7 != -1 )
		/* UVマップの場合 */
		#if ( 0 <= NND_OPT_TEXCOORD_USER7 && NND_OPT_TEXCOORD_USER7 <= 3 )
			g_Output.UV[ NND_UV_NO_USER7 ].NND_UV_SUFFIX_USER7
				= mul( float3(g_Input.UV[ NND_OPT_TEXCOORD_USER7 ].x,g_Input.UV[ NND_OPT_TEXCOORD_USER7 ].y,1.0f), u_TextureMatrix[ NND_OPT_TEX_USER7 ] ).xy;
		/* スフィアマップの場合 */
		#elif ( NND_OPT_TEXCOORD_USER7 == -1 )
			g_Output.UV[ NND_UV_NO_USER7 ].NND_UV_SUFFIX_USER7
				= mul( CalcSphereMapTexCoord(), u_TextureMatrix[ NND_OPT_TEX_USER7 ] ).xy;
		/* プロジェクションマップの場合 */
		#elif ( NND_OPT_TEXCOORD_USER7 == -2 )
			g_Output.UV[ NND_UV_NO_USER7 ].NND_UV_SUFFIX_USER7
				= mul( g_ViewPos, u_TextureMatrix[ NND_OPT_TEX_USER7 ] ).xy;
		#endif
	#endif
	
	/* ユーザーマップ８ */
	#if ( NND_OPT_TEX_USER8 != -1 )
		/* UVマップの場合 */
		#if ( 0 <= NND_OPT_TEXCOORD_USER8 && NND_OPT_TEXCOORD_USER8 <= 3 )
			g_Output.UV[ NND_UV_NO_USER8 ].NND_UV_SUFFIX_USER8
				= mul( float3(g_Input.UV[ NND_OPT_TEXCOORD_USER8 ].x,g_Input.UV[ NND_OPT_TEXCOORD_USER8 ].y,1.0f), u_TextureMatrix[ NND_OPT_TEX_USER8 ] ).xy;
		/* スフィアマップの場合 */
		#elif ( NND_OPT_TEXCOORD_USER8 == -1 )
			g_Output.UV[ NND_UV_NO_USER8 ].NND_UV_SUFFIX_USER8
				= mul( CalcSphereMapTexCoord(), u_TextureMatrix[ NND_OPT_TEX_USER8 ] ).xy;
		/* プロジェクションマップの場合 */
		#elif ( NND_OPT_TEXCOORD_USER8 == -2 )
			g_Output.UV[ NND_UV_NO_USER8 ].NND_UV_SUFFIX_USER8
				= mul( g_ViewPos, u_TextureMatrix[ NND_OPT_TEX_USER8 ] ).xy;
		#endif
	#endif

	/* シャドウマップ */
	#if ( NND_OPT_TEX_SHADOW != -1 )
		g_Output.TexShadow
			= mul( g_ViewPos, u_TextureMatrix[ NND_OPT_TEX_SHADOW ] );
	#endif

	/* シャドウマップ２ */
	#if ( NND_OPT_TEX_SHADOW2 != -1 )
		g_Output.TexShadow2
			= mul( g_ViewPos, u_TextureMatrix[ NND_OPT_TEX_SHADOW2 ] );
	#endif
}

#if NND_PROF_MAX_PARALLEL_LIGHT > 0 || NND_PROF_MAX_POINT_LIGHT > 0 || NND_PROF_MAX_SPOT_LIGHT > 0
void CalcLightOne( in int litno )
{
	float diffcnt, speccnt;
	#if NND_OPT_TWOSIDED_LIGHTING
	float backdiffcnt, backspeccnt;
	#endif
	float3 litdir;
	float  attn;
	#if NND_PROF_MAX_POINT_LIGHT > 0 || NND_PROF_MAX_SPOT_LIGHT > 0
	float3 litpos;
	int unino;
	float dist;
	#endif
	#if NND_PROF_MAX_SPOT_LIGHT > 0
	float spotdot;
	#endif

	#if NND_PROF_MAX_PARALLEL_LIGHT > 0 && ( NND_PROF_MAX_POINT_LIGHT > 0 || NND_PROF_MAX_SPOT_LIGHT > 0 )
	if ( litno < u_NumParallelLight )
	#endif
	#if NND_PROF_MAX_PARALLEL_LIGHT > 0
	{
		/* パラレルライト */

		/* ライト方向 */
		/* （normalizeは、CPU側で行なう） */
		litdir = u_LightSource[ litno ].position.xyz;

		/* 減衰なし */
		attn = 1.0;
	}
	#endif
	#if NND_PROF_MAX_PARALLEL_LIGHT > 0 && ( NND_PROF_MAX_POINT_LIGHT > 0 || NND_PROF_MAX_SPOT_LIGHT > 0 )
	else
	#endif
	#if NND_PROF_MAX_POINT_LIGHT > 0 || NND_PROF_MAX_SPOT_LIGHT > 0
	{
		/* ライト位置 */
		litpos = u_LightSource[ litno ].position.xyz - g_ViewPos.xyz;

		/* ライト方向 */
		litdir = normalize( litpos );

		/* 減衰度 */
		dist = length( litpos );
		#if ( NND_PROF_POINTLIGHT_DISTATTEN == 0 )
		/* 減衰なし */
		attn = 1.0;
		#endif
		#if ( NND_PROF_POINTLIGHT_DISTATTEN == 1 )
		/* 逆 */
		attn = 1.0 / dist;
		#endif
		#if ( NND_PROF_POINTLIGHT_DISTATTEN == 2 )
		/* 逆２乗 */
		attn = 1.0 / ( dist * dist );
		#endif

		#if NND_PROF_MAX_POINT_LIGHT > 0 && NND_PROF_MAX_SPOT_LIGHT > 0
		if ( litno < u_NumParallelLight + u_NumPointLight )
		#endif
		#if NND_PROF_MAX_POINT_LIGHT > 0
		{
			/* ポイントライト */
			/* フォールオフ */
			unino = litno - u_NumParallelLight;
			attn *= clamp(
					( u_PointLightFallOffEnd[ unino ] - dist )
						* u_PointLightFallOffScale[ unino ], 0.0, 1.0 );

		}
		#endif
		#if NND_PROF_MAX_POINT_LIGHT > 0 && NND_PROF_MAX_SPOT_LIGHT > 0
		else
		#endif
		#if NND_PROF_MAX_SPOT_LIGHT > 0
		{
			/* スポットライト */
			/* 距離減衰 */
			unino = litno - u_NumParallelLight - u_NumPointLight;
			attn *= clamp(
					( u_SpotLightFallOffEnd[ unino ] - dist )
					* u_SpotLightFallOffScale[ unino ], 0.0, 1.0 );

			/* 角度減衰 */
			spotdot = dot( -litdir, (float3)(u_LightSource[ litno ].spotDirection) );
			attn *= clamp( ( u_LightSource[ litno ].spotCosCutoff - spotdot ) * 
										u_SpotLightAngleScale[ unino ], 0.0, 1.0 );

		}
		#endif
	}
	#endif

	/* ディフューズ成分 */
	diffcnt = dot( g_Normal, litdir );
	#if NND_OPT_TWOSIDED_LIGHTING
	//backdiffcnt = -min( 0.0, diffcnt );            // 次の行の記述のほうが速い
	backdiffcnt = clamp( -diffcnt, 0.0, 1.0 );	// saturate命令(clamp(*,0,1))は他の命令と同時に実行されることが期待できる
	#endif
	//diffcnt = max( 0.0, diffcnt );            // 次の行の記述のほうが速い
	diffcnt = clamp( diffcnt, 0.0, 1.0 );	// saturate命令(clamp(*,0,1))は他の命令と同時に実行されることが期待できる
	g_Diffuse += u_LightSource[litno].diffuse.rgb * diffcnt * attn;
	#if NND_OPT_TWOSIDED_LIGHTING
	g_BackDiffuse += u_LightSource[litno].diffuse.rgb * backdiffcnt * attn;
	#endif

	#if NND_PROF_LIGHT_AMBIENT
	/* ライトのアンビエント */
	g_Ambient += u_LightSource[litno].ambient.rgb * attn;
	#endif

	/* スペキュラ成分 */
	#if NND_OPT_SPECULAR
		float RE;
		RE = dot( normalize( reflect( litdir, g_Normal ) ), g_EyeDir );
		//speccnt = max( 0.0, RE ); //次の行の記述のほうが速い
		speccnt = clamp( RE, 0.0, 1.0 );
		#if NND_OPT_TWOSIDED_LIGHTING
		//backspeccnt = -min( 0.0, RE );           // 次の行の記述のほうが速い
		//backspeccnt = clamp( -RE, 0.0, 1.0 );	// saturate命令(clamp(*,0,1))は他の命令と同時に実行されることが期待できる
		backspeccnt = speccnt;
		#endif
		if ( diffcnt != 0.0 ){ /* 表面のみ */
			g_Specular += 
				pow( speccnt, u_FrontMaterial.shininess ) * u_LightSource[litno].specular.rgb * attn;
		}
		#if NND_OPT_TWOSIDED_LIGHTING
		if ( backdiffcnt != 0.0 ){
			g_BackSpecular += 
				pow( backspeccnt, u_FrontMaterial.shininess ) * u_LightSource[litno].specular.rgb * attn;
		}
		#endif
	#endif
}
#endif

/**********************/
/* 頂点単位ライト計算 */
/**********************/
#if NND_PROF_MAX_PARALLEL_LIGHT > 0 || NND_PROF_MAX_POINT_LIGHT > 0 || NND_PROF_MAX_SPOT_LIGHT > 0
	#if( 8 > (NND_PROF_MAX_PARALLEL_LIGHT + NND_PROF_MAX_POINT_LIGHT + NND_PROF_MAX_SPOT_LIGHT))
		#define NND_MAX_LIGHT_NUM (NND_PROF_MAX_PARALLEL_LIGHT + NND_PROF_MAX_POINT_LIGHT + NND_PROF_MAX_SPOT_LIGHT)
	#else
		#define NND_MAX_LIGHT_NUM (8)	//ライトの最大数を８に制限
	#endif
#endif

void CalcLight( void )
{
	#if NND_PROF_MAX_PARALLEL_LIGHT > 0 || NND_PROF_MAX_POINT_LIGHT > 0 || NND_PROF_MAX_SPOT_LIGHT > 0

	#if 0 /* 2008.09.05 */
	for ( int litno = 0; litno < NND_MAX_LIGHT_NUM; litno++ ){
		if ( litno <
			#if NND_PROF_MAX_PARALLEL_LIGHT > 0
				+ u_NumParallelLight
			#endif
			#if NND_PROF_MAX_POINT_LIGHT > 0
				+ u_NumPointLight
			#endif
			#if NND_PROF_MAX_SPOT_LIGHT > 0
				+ u_NumSpotLight
			#endif
		) {
			CalcLightOne( litno );
		}
	}
	#else
	{	/*
		   ドライバの不具合と思われる原因不明の現象が発生した為 for 文をコメントして
		   以下のように展開して回避した。
		   不具合が発生したビデオカードは
		   ・Quadro FX 4500(ドライバ169.96)
           ・GeForce 6800(ドライバ93.71, 175.19)
		   です。
		 */
		int num = 0;
		#if (NND_PROF_MAX_PARALLEL_LIGHT > 0)
			num += u_NumParallelLight;
		#endif
		#if (NND_PROF_MAX_POINT_LIGHT > 0)
			num += u_NumPointLight;
        #endif
		#if (NND_PROF_MAX_SPOT_LIGHT > 0)
			num += u_NumSpotLight;
		#endif

		#if (0 < NND_MAX_LIGHT_NUM)
			if(0 < num)	CalcLightOne( 0 );
		#endif
		#if (1 < NND_MAX_LIGHT_NUM)
			if(1 < num)	CalcLightOne( 1 );
		#endif
		#if (2 < NND_MAX_LIGHT_NUM)
			if(2 < num)	CalcLightOne( 2 );
		#endif
		#if (3 < NND_MAX_LIGHT_NUM)
			if(3 < num)	CalcLightOne( 3 );
		#endif
		#if (4 < NND_MAX_LIGHT_NUM)
			if(4 < num)	CalcLightOne( 4 );
		#endif
		#if (5 < NND_MAX_LIGHT_NUM)
			if(5 < num)	CalcLightOne( 5 );
		#endif
		#if (6 < NND_MAX_LIGHT_NUM)
			if(6 < num)	CalcLightOne( 6 );
		#endif
		#if (7 < NND_MAX_LIGHT_NUM)
			if(7 < num)	CalcLightOne( 7 );
		#endif
	}
	#endif

	#endif
}



/********************************/
/* 頂点ライティング／マテリアル */
/********************************/
void CalcLightAndMaterial(void)
{
	/* ライティング計算 */
	#if !( NND_OPT_FRAG_LIGHTING )
		/**********************************/
		/* 頂点単位ライティング計算の場合 */
		/**********************************/
		#if NND_OPT_LIGHTING
			/* 各ライト計算 */
			CalcLight();

			/* マテリアルのディフューズカラーと乗算 */
			g_Diffuse *= u_FrontMaterial.diffuse.rgb;

			/* アンビエントとエミッシブを加算 */
			g_Diffuse += u_FrontLightModelProduct.sceneColor.rgb;

			/* 二面ライティングの場合 */
			#if NND_OPT_TWOSIDED_LIGHTING
				g_BackDiffuse *= u_FrontMaterial.diffuse.rgb;
				g_BackDiffuse += u_FrontLightModelProduct.sceneColor.rgb;
			#endif

			/* ライトのアンビエントを加算 */
			#if NND_PROF_LIGHT_AMBIENT
			g_Diffuse += g_Ambient;
				#if NND_OPT_TWOSIDED_LIGHTING
					g_BackDiffuse += g_Ambient;
				#endif
			#endif

		#else
			/* ライティングしない */
			g_Diffuse  = u_FrontMaterial.diffuse.rgb;
			#if NND_OPT_TWOSIDED_LIGHTING
				g_BackDiffuse = u_FrontMaterial.diffuse.rgb;
			#endif
		#endif
	#else
		/**************************************/
		/* フラグメント単位ライティングの場合 */
		/**************************************/
		g_Diffuse = u_FrontMaterial.diffuse.rgb;
		#if NND_OPT_SPECULAR
			// g_Specular = u_FrontMaterial.specular.rgb;
			g_Specular = float3(1.0f,1.0f,1.0f);
		#endif
	#endif

	
	/* 頂点カラー */
	#if NND_OPT_VTX_COLOR
		g_Diffuse *= g_Input.Col[0].rgb;
		#if NND_OPT_TWOSIDED_LIGHTING
			g_BackDiffuse *= g_Input.Col[0].rgb;
		#endif
		#if NND_OPT_SPECULAR && NND_PROF_VTX_SPECULAR
			/* スペキュラ、頂点スペキュラカラーが有効のとき */

			#if NND_OPT_VTX_COLOR_SPECULAR
				/* 頂点カラーにスペキュラカラーがあるとき */
				float3 l_col2 = g_Input.Col[1].rgb;
			#else
				/* 頂点カラーにスペキュラカラーがないとき */
				float3 l_col2 = float3(0.0, 0.0, 0.0);	/* 0 にする */
			#endif

			g_Specular *= l_col2;
			#if NND_OPT_TWOSIDED_LIGHTING
				g_BackSpecular *= l_col2;
			#endif
		#endif
	#endif
}

/**********/
/* フォグ */
/**********/
void CalcFog(void)
{
	#if NND_PROF_FOG_MODEL
		#if NND_PROF_DISTANCE_FOG
		/* 距離フォグ */
			float fogdist = length( g_ViewPos.xyz );
		#else
			/* Ｚフォグ */
			float fogdist = abs( g_ViewPos.z );
		#endif

		#if ( !( NND_PROF_FRAG_FOG ) )
			/* 頂点フォグの場合 */
			#if ( NND_PROF_FOG_MODEL == 1 )
				fogdist = ( u_Fog.end - fogdist ) * u_Fog.scale;
			#elif ( NND_PROF_FOG_MODEL == 2 )
				fogdist = exp2( -u_Fog.density * fogdist * 1.442695 );
			#elif ( NND_PROF_FOG_MODEL == 3 )
				fogdist = exp2( -u_Fog.density * u_Fog.density
								* fogdist * fogdist * 1.442695 );
			#endif
			g_Output.ViewPosFog.NND_SUFFIX_FOG = clamp( fogdist, 0.0, 1.0 );
		#else
			g_Output.ViewPosFog.NND_SUFFIX_FOG = fogdist;
		#endif
	#endif
}

/********************/
/* 出力カラーを設定 */
/********************/
void SetFinalOutput(void)
{
	/* ポジション */
	#if NND_OPT_FRAG_LIGHTING || NND_PROF_FOG_MODEL || NND_FRAG_DUALPARAMAP
		g_Output.ViewPosFog.NND_SUFFIX_VPOS = g_ViewPos.xyz;
	#endif

	/* ディフューズカラー */
	g_Output.FrontCol[0].rgb = g_Diffuse;
	#if NND_OPT_VTX_COLOR
		g_Output.FrontCol[0].a = u_FrontMaterial.diffuse.a * g_Input.Col[0].a;
	#else
		g_Output.FrontCol[0].a = u_FrontMaterial.diffuse.a;
	#endif
	#if NND_OPT_TWOSIDED_LIGHTING
		g_Output.BackCol[0].rgb = g_BackDiffuse;
		g_Output.BackCol[0].a = g_Output.FrontCol[0].a;
	#endif

	/* スペキュラカラー */
	#if NND_OPT_SPECULAR
		g_Output.FrontCol[1].rgb = g_Specular;
		#if NND_OPT_TWOSIDED_LIGHTING
			g_Output.BackCol[1].rgb = g_BackSpecular;
		#endif
	#endif

	/* フラグメント単位ライティング用出力 */
	#if ( NND_OPT_FRAG_LIGHTING || NND_FRAG_DUALPARAMAP )
	g_Output.NrmVec = g_Normal;
		#if ( NND_OPT_TANGENT_SPACE )
		g_Output.Tan  = g_Tangent;
		g_Output.BNrm = g_Binormal;
		#endif
	#endif
}

/**********************/
/* 頂点シェーダメイン */
/**********************/
VS_OUTPUT main(VS_INPUT Input)
{
	g_Input = Input;
	
	/******************/
	/* 位置、法線変換 */
	/******************/
	TransformVertexPositionNormal();

	/**********************/
	/* テクスチャ座標生成 */
	/**********************/
	CalcTextureCoordinate();

	/********************************/
	/* 頂点ライティング／マテリアル */
	/********************************/
	CalcLightAndMaterial();

	/**********/
	/* フォグ */
	/**********/
	CalcFog();

	/********************/
	/* 出力カラーを設定 */
	/********************/
	SetFinalOutput();

	return( g_Output );
}

/*-------------------------------------------------------------------------*/
