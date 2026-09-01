
#ifndef __NNDUV_DEF_H__
#define __NNDUV_DEF_H__

#define NND_UV_NUM					0

#if ( NND_OPT_TEX_NORMAL != -1 )
	#if   NND_UV_NUM == 0
		#define NND_UV_NO_NORMAL 0
		#define NND_UV_SUFFIX_NORMAL			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 1
	#elif NND_UV_NUM == 1
		#define NND_UV_NO_NORMAL 0
		#define NND_UV_SUFFIX_NORMAL			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 2
	#elif NND_UV_NUM == 2
		#define NND_UV_NO_NORMAL 1
		#define NND_UV_SUFFIX_NORMAL			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 3
	#elif NND_UV_NUM == 3
		#define NND_UV_NO_NORMAL 1
		#define NND_UV_SUFFIX_NORMAL			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 4
	#elif NND_UV_NUM == 4
		#define NND_UV_NO_NORMAL 2
		#define NND_UV_SUFFIX_NORMAL			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 5
	#elif NND_UV_NUM == 5
		#define NND_UV_NO_NORMAL 2
		#define NND_UV_SUFFIX_NORMAL			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 6
	#elif NND_UV_NUM == 6
		#define NND_UV_NO_NORMAL 3
		#define NND_UV_SUFFIX_NORMAL			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 7
	#elif NND_UV_NUM == 7
		#define NND_UV_NO_NORMAL 3
		#define NND_UV_SUFFIX_NORMAL			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 8
	#elif NND_UV_NUM == 8
		#define NND_UV_NO_NORMAL 4
		#define NND_UV_SUFFIX_NORMAL			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 9
	#elif NND_UV_NUM == 9
		#define NND_UV_NO_NORMAL 4
		#define NND_UV_SUFFIX_NORMAL			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 10
	#elif NND_UV_NUM == 10
		#define NND_UV_NO_NORMAL 5
		#define NND_UV_SUFFIX_NORMAL			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 11
	#elif NND_UV_NUM == 11
		#define NND_UV_NO_NORMAL 5 
		#define NND_UV_SUFFIX_NORMAL			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 12
	#elif NND_UV_NUM == 12
		#define NND_UV_NO_NORMAL 6 
		#define NND_UV_SUFFIX_NORMAL			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 13
	#elif NND_UV_NUM == 13
		#define NND_UV_NO_NORMAL 6
		#define NND_UV_SUFFIX_NORMAL			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 14
	#elif NND_UV_NUM == 14
		#define NND_UV_NO_NORMAL 7
		#define NND_UV_SUFFIX_NORMAL			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 15
	#elif NND_UV_NUM == 15
		#define NND_UV_NO_NORMAL 7
		#define NND_UV_SUFFIX_NORMAL			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 16
	#else
		#error NND_UV_NO_NORMAL overflow
	#endif
#endif

#if ( NND_OPT_TEX_BASE != -1 )
	#if   NND_UV_NUM == 0
		#define NND_UV_NO_BASE 0
		#define NND_UV_SUFFIX_BASE			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 1
	#elif NND_UV_NUM == 1
		#define NND_UV_NO_BASE 0
		#define NND_UV_SUFFIX_BASE			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 2
	#elif NND_UV_NUM == 2
		#define NND_UV_NO_BASE 1
		#define NND_UV_SUFFIX_BASE			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 3
	#elif NND_UV_NUM == 3
		#define NND_UV_NO_BASE 1
		#define NND_UV_SUFFIX_BASE			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 4
	#elif NND_UV_NUM == 4
		#define NND_UV_NO_BASE 2
		#define NND_UV_SUFFIX_BASE			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 5
	#elif NND_UV_NUM == 5
		#define NND_UV_NO_BASE 2
		#define NND_UV_SUFFIX_BASE			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 6
	#elif NND_UV_NUM == 6
		#define NND_UV_NO_BASE 3
		#define NND_UV_SUFFIX_BASE			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 7
	#elif NND_UV_NUM == 7
		#define NND_UV_NO_BASE 3
		#define NND_UV_SUFFIX_BASE			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 8
	#elif NND_UV_NUM == 8
		#define NND_UV_NO_BASE 4
		#define NND_UV_SUFFIX_BASE			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 9
	#elif NND_UV_NUM == 9
		#define NND_UV_NO_BASE 4
		#define NND_UV_SUFFIX_BASE			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 10
	#elif NND_UV_NUM == 10
		#define NND_UV_NO_BASE 5
		#define NND_UV_SUFFIX_BASE			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 11
	#elif NND_UV_NUM == 11
		#define NND_UV_NO_BASE 5 
		#define NND_UV_SUFFIX_BASE			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 12
	#elif NND_UV_NUM == 12
		#define NND_UV_NO_BASE 6 
		#define NND_UV_SUFFIX_BASE			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 13
	#elif NND_UV_NUM == 13
		#define NND_UV_NO_BASE 6
		#define NND_UV_SUFFIX_BASE			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 14
	#elif NND_UV_NUM == 14
		#define NND_UV_NO_BASE 7
		#define NND_UV_SUFFIX_BASE			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 15
	#elif NND_UV_NUM == 15
		#define NND_UV_NO_BASE 7
		#define NND_UV_SUFFIX_BASE			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 16
	#else
		#error NND_UV_NO_BASE overflow
	#endif
#endif

#if ( NND_OPT_TEX_DECAL != -1 )
	#if   NND_UV_NUM == 0
		#define NND_UV_NO_DECAL 0
		#define NND_UV_SUFFIX_DECAL			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 1
	#elif NND_UV_NUM == 1
		#define NND_UV_NO_DECAL 0
		#define NND_UV_SUFFIX_DECAL			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 2
	#elif NND_UV_NUM == 2
		#define NND_UV_NO_DECAL 1
		#define NND_UV_SUFFIX_DECAL			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 3
	#elif NND_UV_NUM == 3
		#define NND_UV_NO_DECAL 1
		#define NND_UV_SUFFIX_DECAL			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 4
	#elif NND_UV_NUM == 4
		#define NND_UV_NO_DECAL 2
		#define NND_UV_SUFFIX_DECAL			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 5
	#elif NND_UV_NUM == 5
		#define NND_UV_NO_DECAL 2
		#define NND_UV_SUFFIX_DECAL			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 6
	#elif NND_UV_NUM == 6
		#define NND_UV_NO_DECAL 3
		#define NND_UV_SUFFIX_DECAL			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 7
	#elif NND_UV_NUM == 7
		#define NND_UV_NO_DECAL 3
		#define NND_UV_SUFFIX_DECAL			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 8
	#elif NND_UV_NUM == 8
		#define NND_UV_NO_DECAL 4
		#define NND_UV_SUFFIX_DECAL			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 9
	#elif NND_UV_NUM == 9
		#define NND_UV_NO_DECAL 4
		#define NND_UV_SUFFIX_DECAL			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 10
	#elif NND_UV_NUM == 10
		#define NND_UV_NO_DECAL 5
		#define NND_UV_SUFFIX_DECAL			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 11
	#elif NND_UV_NUM == 11
		#define NND_UV_NO_DECAL 5 
		#define NND_UV_SUFFIX_DECAL			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 12
	#elif NND_UV_NUM == 12
		#define NND_UV_NO_DECAL 6 
		#define NND_UV_SUFFIX_DECAL			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 13
	#elif NND_UV_NUM == 13
		#define NND_UV_NO_DECAL 6
		#define NND_UV_SUFFIX_DECAL			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 14
	#elif NND_UV_NUM == 14
		#define NND_UV_NO_DECAL 7
		#define NND_UV_SUFFIX_DECAL			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 15
	#elif NND_UV_NUM == 15
		#define NND_UV_NO_DECAL 7
		#define NND_UV_SUFFIX_DECAL			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 16
	#else
		#error NND_UV_NO_DECAL overflow
	#endif
#endif

#if ( NND_OPT_TEX_DECAL2 != -1 )
	#if   NND_UV_NUM == 0
		#define NND_UV_NO_DECAL2 0
		#define NND_UV_SUFFIX_DECAL2			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 1
	#elif NND_UV_NUM == 1
		#define NND_UV_NO_DECAL2 0
		#define NND_UV_SUFFIX_DECAL2			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 2
	#elif NND_UV_NUM == 2
		#define NND_UV_NO_DECAL2 1
		#define NND_UV_SUFFIX_DECAL2			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 3
	#elif NND_UV_NUM == 3
		#define NND_UV_NO_DECAL2 1
		#define NND_UV_SUFFIX_DECAL2			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 4
	#elif NND_UV_NUM == 4
		#define NND_UV_NO_DECAL2 2
		#define NND_UV_SUFFIX_DECAL2			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 5
	#elif NND_UV_NUM == 5
		#define NND_UV_NO_DECAL2 2
		#define NND_UV_SUFFIX_DECAL2			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 6
	#elif NND_UV_NUM == 6
		#define NND_UV_NO_DECAL2 3
		#define NND_UV_SUFFIX_DECAL2			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 7
	#elif NND_UV_NUM == 7
		#define NND_UV_NO_DECAL2 3
		#define NND_UV_SUFFIX_DECAL2			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 8
	#elif NND_UV_NUM == 8
		#define NND_UV_NO_DECAL2 4
		#define NND_UV_SUFFIX_DECAL2			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 9
	#elif NND_UV_NUM == 9
		#define NND_UV_NO_DECAL2 4
		#define NND_UV_SUFFIX_DECAL2			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 10
	#elif NND_UV_NUM == 10
		#define NND_UV_NO_DECAL2 5
		#define NND_UV_SUFFIX_DECAL2			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 11
	#elif NND_UV_NUM == 11
		#define NND_UV_NO_DECAL2 5 
		#define NND_UV_SUFFIX_DECAL2			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 12
	#elif NND_UV_NUM == 12
		#define NND_UV_NO_DECAL2 6 
		#define NND_UV_SUFFIX_DECAL2			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 13
	#elif NND_UV_NUM == 13
		#define NND_UV_NO_DECAL2 6
		#define NND_UV_SUFFIX_DECAL2			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 14
	#elif NND_UV_NUM == 14
		#define NND_UV_NO_DECAL2 7
		#define NND_UV_SUFFIX_DECAL2			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 15
	#elif NND_UV_NUM == 15
		#define NND_UV_NO_DECAL2 7
		#define NND_UV_SUFFIX_DECAL2			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 16
	#else
		#error NND_UV_NO_DECAL2 overflow
	#endif
#endif

#if ( NND_OPT_TEX_DECAL3 != -1 )
	#if   NND_UV_NUM == 0
		#define NND_UV_NO_DECAL3 0
		#define NND_UV_SUFFIX_DECAL3			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 1
	#elif NND_UV_NUM == 1
		#define NND_UV_NO_DECAL3 0
		#define NND_UV_SUFFIX_DECAL3			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 2
	#elif NND_UV_NUM == 2
		#define NND_UV_NO_DECAL3 1
		#define NND_UV_SUFFIX_DECAL3			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 3
	#elif NND_UV_NUM == 3
		#define NND_UV_NO_DECAL3 1
		#define NND_UV_SUFFIX_DECAL3			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 4
	#elif NND_UV_NUM == 4
		#define NND_UV_NO_DECAL3 2
		#define NND_UV_SUFFIX_DECAL3			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 5
	#elif NND_UV_NUM == 5
		#define NND_UV_NO_DECAL3 2
		#define NND_UV_SUFFIX_DECAL3			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 6
	#elif NND_UV_NUM == 6
		#define NND_UV_NO_DECAL3 3
		#define NND_UV_SUFFIX_DECAL3			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 7
	#elif NND_UV_NUM == 7
		#define NND_UV_NO_DECAL3 3
		#define NND_UV_SUFFIX_DECAL3			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 8
	#elif NND_UV_NUM == 8
		#define NND_UV_NO_DECAL3 4
		#define NND_UV_SUFFIX_DECAL3			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 9
	#elif NND_UV_NUM == 9
		#define NND_UV_NO_DECAL3 4
		#define NND_UV_SUFFIX_DECAL3			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 10
	#elif NND_UV_NUM == 10
		#define NND_UV_NO_DECAL3 5
		#define NND_UV_SUFFIX_DECAL3			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 11
	#elif NND_UV_NUM == 11
		#define NND_UV_NO_DECAL3 5 
		#define NND_UV_SUFFIX_DECAL3			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 12
	#elif NND_UV_NUM == 12
		#define NND_UV_NO_DECAL3 6 
		#define NND_UV_SUFFIX_DECAL3			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 13
	#elif NND_UV_NUM == 13
		#define NND_UV_NO_DECAL3 6
		#define NND_UV_SUFFIX_DECAL3			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 14
	#elif NND_UV_NUM == 14
		#define NND_UV_NO_DECAL3 7
		#define NND_UV_SUFFIX_DECAL3			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 15
	#elif NND_UV_NUM == 15
		#define NND_UV_NO_DECAL3 7
		#define NND_UV_SUFFIX_DECAL3			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 16
	#else
		#error NND_UV_NO_DECAL3 overflow
	#endif
#endif

#if ( NND_OPT_TEX_SPECULAR != -1 )
	#if   NND_UV_NUM == 0
		#define NND_UV_NO_SPECULAR 0
		#define NND_UV_SUFFIX_SPECULAR			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 1
	#elif NND_UV_NUM == 1
		#define NND_UV_NO_SPECULAR 0
		#define NND_UV_SUFFIX_SPECULAR			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 2
	#elif NND_UV_NUM == 2
		#define NND_UV_NO_SPECULAR 1
		#define NND_UV_SUFFIX_SPECULAR			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 3
	#elif NND_UV_NUM == 3
		#define NND_UV_NO_SPECULAR 1
		#define NND_UV_SUFFIX_SPECULAR			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 4
	#elif NND_UV_NUM == 4
		#define NND_UV_NO_SPECULAR 2
		#define NND_UV_SUFFIX_SPECULAR			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 5
	#elif NND_UV_NUM == 5
		#define NND_UV_NO_SPECULAR 2
		#define NND_UV_SUFFIX_SPECULAR			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 6
	#elif NND_UV_NUM == 6
		#define NND_UV_NO_SPECULAR 3
		#define NND_UV_SUFFIX_SPECULAR			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 7
	#elif NND_UV_NUM == 7
		#define NND_UV_NO_SPECULAR 3
		#define NND_UV_SUFFIX_SPECULAR			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 8
	#elif NND_UV_NUM == 8
		#define NND_UV_NO_SPECULAR 4
		#define NND_UV_SUFFIX_SPECULAR			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 9
	#elif NND_UV_NUM == 9
		#define NND_UV_NO_SPECULAR 4
		#define NND_UV_SUFFIX_SPECULAR			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 10
	#elif NND_UV_NUM == 10
		#define NND_UV_NO_SPECULAR 5
		#define NND_UV_SUFFIX_SPECULAR			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 11
	#elif NND_UV_NUM == 11
		#define NND_UV_NO_SPECULAR 5 
		#define NND_UV_SUFFIX_SPECULAR			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 12
	#elif NND_UV_NUM == 12
		#define NND_UV_NO_SPECULAR 6 
		#define NND_UV_SUFFIX_SPECULAR			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 13
	#elif NND_UV_NUM == 13
		#define NND_UV_NO_SPECULAR 6
		#define NND_UV_SUFFIX_SPECULAR			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 14
	#elif NND_UV_NUM == 14
		#define NND_UV_NO_SPECULAR 7
		#define NND_UV_SUFFIX_SPECULAR			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 15
	#elif NND_UV_NUM == 15
		#define NND_UV_NO_SPECULAR 7
		#define NND_UV_SUFFIX_SPECULAR			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 16
	#else
		#error NND_UV_NO_SPECULAR overflow
	#endif
#endif

#if ( NND_OPT_TEX_SHININESS != -1 )
	#if   NND_UV_NUM == 0
		#define NND_UV_NO_SHININESS 0
		#define NND_UV_SUFFIX_SHININESS			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 1
	#elif NND_UV_NUM == 1
		#define NND_UV_NO_SHININESS 0
		#define NND_UV_SUFFIX_SHININESS			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 2
	#elif NND_UV_NUM == 2
		#define NND_UV_NO_SHININESS 1
		#define NND_UV_SUFFIX_SHININESS			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 3
	#elif NND_UV_NUM == 3
		#define NND_UV_NO_SHININESS 1
		#define NND_UV_SUFFIX_SHININESS			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 4
	#elif NND_UV_NUM == 4
		#define NND_UV_NO_SHININESS 2
		#define NND_UV_SUFFIX_SHININESS			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 5
	#elif NND_UV_NUM == 5
		#define NND_UV_NO_SHININESS 2
		#define NND_UV_SUFFIX_SHININESS			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 6
	#elif NND_UV_NUM == 6
		#define NND_UV_NO_SHININESS 3
		#define NND_UV_SUFFIX_SHININESS			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 7
	#elif NND_UV_NUM == 7
		#define NND_UV_NO_SHININESS 3
		#define NND_UV_SUFFIX_SHININESS			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 8
	#elif NND_UV_NUM == 8
		#define NND_UV_NO_SHININESS 4
		#define NND_UV_SUFFIX_SHININESS			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 9
	#elif NND_UV_NUM == 9
		#define NND_UV_NO_SHININESS 4
		#define NND_UV_SUFFIX_SHININESS			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 10
	#elif NND_UV_NUM == 10
		#define NND_UV_NO_SHININESS 5
		#define NND_UV_SUFFIX_SHININESS			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 11
	#elif NND_UV_NUM == 11
		#define NND_UV_NO_SHININESS 5 
		#define NND_UV_SUFFIX_SHININESS			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 12
	#elif NND_UV_NUM == 12
		#define NND_UV_NO_SHININESS 6 
		#define NND_UV_SUFFIX_SHININESS			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 13
	#elif NND_UV_NUM == 13
		#define NND_UV_NO_SHININESS 6
		#define NND_UV_SUFFIX_SHININESS			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 14
	#elif NND_UV_NUM == 14
		#define NND_UV_NO_SHININESS 7
		#define NND_UV_SUFFIX_SHININESS			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 15
	#elif NND_UV_NUM == 15
		#define NND_UV_NO_SHININESS 7
		#define NND_UV_SUFFIX_SHININESS			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 16
	#else
		#error NND_UV_NO_SHININESS overflow
	#endif
#endif

#if 0 /* 2008.06.23(必要ない) */
#if ( NND_OPT_TEX_DUALPARABOLOID	!= -1 && NND_OPT_TEXCOORD_DUALPARABOLOID	!= -1 )
	#if   NND_UV_NUM == 0
		#define NND_UV_NO_DUALPARABOLOID 0
		#define NND_UV_SUFFIX_DUALPARABOLOID			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 1
	#elif NND_UV_NUM == 1
		#define NND_UV_NO_DUALPARABOLOID 0
		#define NND_UV_SUFFIX_DUALPARABOLOID			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 2
	#elif NND_UV_NUM == 2
		#define NND_UV_NO_DUALPARABOLOID 1
		#define NND_UV_SUFFIX_DUALPARABOLOID			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 3
	#elif NND_UV_NUM == 3
		#define NND_UV_NO_DUALPARABOLOID 1
		#define NND_UV_SUFFIX_DUALPARABOLOID			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 4
	#elif NND_UV_NUM == 4
		#define NND_UV_NO_DUALPARABOLOID 2
		#define NND_UV_SUFFIX_DUALPARABOLOID			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 5
	#elif NND_UV_NUM == 5
		#define NND_UV_NO_DUALPARABOLOID 2
		#define NND_UV_SUFFIX_DUALPARABOLOID			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 6
	#elif NND_UV_NUM == 6
		#define NND_UV_NO_DUALPARABOLOID 3
		#define NND_UV_SUFFIX_DUALPARABOLOID			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 7
	#elif NND_UV_NUM == 7
		#define NND_UV_NO_DUALPARABOLOID 3
		#define NND_UV_SUFFIX_DUALPARABOLOID			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 8
	#elif NND_UV_NUM == 8
		#define NND_UV_NO_DUALPARABOLOID 4
		#define NND_UV_SUFFIX_DUALPARABOLOID			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 9
	#elif NND_UV_NUM == 9
		#define NND_UV_NO_DUALPARABOLOID 4
		#define NND_UV_SUFFIX_DUALPARABOLOID			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 10
	#elif NND_UV_NUM == 10
		#define NND_UV_NO_DUALPARABOLOID 5
		#define NND_UV_SUFFIX_DUALPARABOLOID			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 11
	#elif NND_UV_NUM == 11
		#define NND_UV_NO_DUALPARABOLOID 5 
		#define NND_UV_SUFFIX_DUALPARABOLOID			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 12
	#elif NND_UV_NUM == 12
		#define NND_UV_NO_DUALPARABOLOID 6 
		#define NND_UV_SUFFIX_DUALPARABOLOID			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 13
	#elif NND_UV_NUM == 13
		#define NND_UV_NO_DUALPARABOLOID 6
		#define NND_UV_SUFFIX_DUALPARABOLOID			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 14
	#elif NND_UV_NUM == 14
		#define NND_UV_NO_DUALPARABOLOID 7
		#define NND_UV_SUFFIX_DUALPARABOLOID			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 15
	#elif NND_UV_NUM == 15
		#define NND_UV_NO_DUALPARABOLOID 7
		#define NND_UV_SUFFIX_DUALPARABOLOID			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 16
	#else
		#error NND_UV_NO_DUALPARABOLOID overflow
	#endif
#endif
#endif

#if ( NND_OPT_TEX_ENVMASK != -1 )
	#if   NND_UV_NUM == 0
		#define NND_UV_NO_ENVMASK 0
		#define NND_UV_SUFFIX_ENVMASK			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 1
	#elif NND_UV_NUM == 1
		#define NND_UV_NO_ENVMASK 0
		#define NND_UV_SUFFIX_ENVMASK			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 2
	#elif NND_UV_NUM == 2
		#define NND_UV_NO_ENVMASK 1
		#define NND_UV_SUFFIX_ENVMASK			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 3
	#elif NND_UV_NUM == 3
		#define NND_UV_NO_ENVMASK 1
		#define NND_UV_SUFFIX_ENVMASK			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 4
	#elif NND_UV_NUM == 4
		#define NND_UV_NO_ENVMASK 2
		#define NND_UV_SUFFIX_ENVMASK			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 5
	#elif NND_UV_NUM == 5
		#define NND_UV_NO_ENVMASK 2
		#define NND_UV_SUFFIX_ENVMASK			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 6
	#elif NND_UV_NUM == 6
		#define NND_UV_NO_ENVMASK 3
		#define NND_UV_SUFFIX_ENVMASK			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 7
	#elif NND_UV_NUM == 7
		#define NND_UV_NO_ENVMASK 3
		#define NND_UV_SUFFIX_ENVMASK			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 8
	#elif NND_UV_NUM == 8
		#define NND_UV_NO_ENVMASK 4
		#define NND_UV_SUFFIX_ENVMASK			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 9
	#elif NND_UV_NUM == 9
		#define NND_UV_NO_ENVMASK 4
		#define NND_UV_SUFFIX_ENVMASK			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 10
	#elif NND_UV_NUM == 10
		#define NND_UV_NO_ENVMASK 5
		#define NND_UV_SUFFIX_ENVMASK			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 11
	#elif NND_UV_NUM == 11
		#define NND_UV_NO_ENVMASK 5 
		#define NND_UV_SUFFIX_ENVMASK			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 12
	#elif NND_UV_NUM == 12
		#define NND_UV_NO_ENVMASK 6 
		#define NND_UV_SUFFIX_ENVMASK			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 13
	#elif NND_UV_NUM == 13
		#define NND_UV_NO_ENVMASK 6
		#define NND_UV_SUFFIX_ENVMASK			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 14
	#elif NND_UV_NUM == 14
		#define NND_UV_NO_ENVMASK 7
		#define NND_UV_SUFFIX_ENVMASK			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 15
	#elif NND_UV_NUM == 15
		#define NND_UV_NO_ENVMASK 7
		#define NND_UV_SUFFIX_ENVMASK			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 16
	#else
		#error NND_UV_NO_ENVMASK overflow
	#endif
#endif

#if ( NND_OPT_TEX_MODULATE != -1 )
	#if   NND_UV_NUM == 0
		#define NND_UV_NO_MODULATE 0
		#define NND_UV_SUFFIX_MODULATE			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 1
	#elif NND_UV_NUM == 1
		#define NND_UV_NO_MODULATE 0
		#define NND_UV_SUFFIX_MODULATE			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 2
	#elif NND_UV_NUM == 2
		#define NND_UV_NO_MODULATE 1
		#define NND_UV_SUFFIX_MODULATE			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 3
	#elif NND_UV_NUM == 3
		#define NND_UV_NO_MODULATE 1
		#define NND_UV_SUFFIX_MODULATE			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 4
	#elif NND_UV_NUM == 4
		#define NND_UV_NO_MODULATE 2
		#define NND_UV_SUFFIX_MODULATE			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 5
	#elif NND_UV_NUM == 5
		#define NND_UV_NO_MODULATE 2
		#define NND_UV_SUFFIX_MODULATE			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 6
	#elif NND_UV_NUM == 6
		#define NND_UV_NO_MODULATE 3
		#define NND_UV_SUFFIX_MODULATE			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 7
	#elif NND_UV_NUM == 7
		#define NND_UV_NO_MODULATE 3
		#define NND_UV_SUFFIX_MODULATE			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 8
	#elif NND_UV_NUM == 8
		#define NND_UV_NO_MODULATE 4
		#define NND_UV_SUFFIX_MODULATE			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 9
	#elif NND_UV_NUM == 9
		#define NND_UV_NO_MODULATE 4
		#define NND_UV_SUFFIX_MODULATE			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 10
	#elif NND_UV_NUM == 10
		#define NND_UV_NO_MODULATE 5
		#define NND_UV_SUFFIX_MODULATE			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 11
	#elif NND_UV_NUM == 11
		#define NND_UV_NO_MODULATE 5 
		#define NND_UV_SUFFIX_MODULATE			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 12
	#elif NND_UV_NUM == 12
		#define NND_UV_NO_MODULATE 6 
		#define NND_UV_SUFFIX_MODULATE			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 13
	#elif NND_UV_NUM == 13
		#define NND_UV_NO_MODULATE 6
		#define NND_UV_SUFFIX_MODULATE			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 14
	#elif NND_UV_NUM == 14
		#define NND_UV_NO_MODULATE 7
		#define NND_UV_SUFFIX_MODULATE			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 15
	#elif NND_UV_NUM == 15
		#define NND_UV_NO_MODULATE 7
		#define NND_UV_SUFFIX_MODULATE			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 16
	#else
		#error NND_UV_NO_MODULATE overflow
	#endif
#endif

#if ( NND_OPT_TEX_ADD != -1 )
	#if   NND_UV_NUM == 0
		#define NND_UV_NO_ADD 0
		#define NND_UV_SUFFIX_ADD			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 1
	#elif NND_UV_NUM == 1
		#define NND_UV_NO_ADD 0
		#define NND_UV_SUFFIX_ADD			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 2
	#elif NND_UV_NUM == 2
		#define NND_UV_NO_ADD 1
		#define NND_UV_SUFFIX_ADD			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 3
	#elif NND_UV_NUM == 3
		#define NND_UV_NO_ADD 1
		#define NND_UV_SUFFIX_ADD			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 4
	#elif NND_UV_NUM == 4
		#define NND_UV_NO_ADD 2
		#define NND_UV_SUFFIX_ADD			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 5
	#elif NND_UV_NUM == 5
		#define NND_UV_NO_ADD 2
		#define NND_UV_SUFFIX_ADD			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 6
	#elif NND_UV_NUM == 6
		#define NND_UV_NO_ADD 3
		#define NND_UV_SUFFIX_ADD			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 7
	#elif NND_UV_NUM == 7
		#define NND_UV_NO_ADD 3
		#define NND_UV_SUFFIX_ADD			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 8
	#elif NND_UV_NUM == 8
		#define NND_UV_NO_ADD 4
		#define NND_UV_SUFFIX_ADD			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 9
	#elif NND_UV_NUM == 9
		#define NND_UV_NO_ADD 4
		#define NND_UV_SUFFIX_ADD			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 10
	#elif NND_UV_NUM == 10
		#define NND_UV_NO_ADD 5
		#define NND_UV_SUFFIX_ADD			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 11
	#elif NND_UV_NUM == 11
		#define NND_UV_NO_ADD 5 
		#define NND_UV_SUFFIX_ADD			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 12
	#elif NND_UV_NUM == 12
		#define NND_UV_NO_ADD 6 
		#define NND_UV_SUFFIX_ADD			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 13
	#elif NND_UV_NUM == 13
		#define NND_UV_NO_ADD 6
		#define NND_UV_SUFFIX_ADD			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 14
	#elif NND_UV_NUM == 14
		#define NND_UV_NO_ADD 7
		#define NND_UV_SUFFIX_ADD			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 15
	#elif NND_UV_NUM == 15
		#define NND_UV_NO_ADD 7
		#define NND_UV_SUFFIX_ADD			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 16
	#else
		#error NND_UV_NO_ADD overflow
	#endif
#endif

#if ( NND_OPT_TEX_OPACITY != -1 )
	#if   NND_UV_NUM == 0
		#define NND_UV_NO_OPACITY 0
		#define NND_UV_SUFFIX_OPACITY			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 1
	#elif NND_UV_NUM == 1
		#define NND_UV_NO_OPACITY 0
		#define NND_UV_SUFFIX_OPACITY			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 2
	#elif NND_UV_NUM == 2
		#define NND_UV_NO_OPACITY 1
		#define NND_UV_SUFFIX_OPACITY			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 3
	#elif NND_UV_NUM == 3
		#define NND_UV_NO_OPACITY 1
		#define NND_UV_SUFFIX_OPACITY			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 4
	#elif NND_UV_NUM == 4
		#define NND_UV_NO_OPACITY 2
		#define NND_UV_SUFFIX_OPACITY			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 5
	#elif NND_UV_NUM == 5
		#define NND_UV_NO_OPACITY 2
		#define NND_UV_SUFFIX_OPACITY			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 6
	#elif NND_UV_NUM == 6
		#define NND_UV_NO_OPACITY 3
		#define NND_UV_SUFFIX_OPACITY			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 7
	#elif NND_UV_NUM == 7
		#define NND_UV_NO_OPACITY 3
		#define NND_UV_SUFFIX_OPACITY			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 8
	#elif NND_UV_NUM == 8
		#define NND_UV_NO_OPACITY 4
		#define NND_UV_SUFFIX_OPACITY			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 9
	#elif NND_UV_NUM == 9
		#define NND_UV_NO_OPACITY 4
		#define NND_UV_SUFFIX_OPACITY			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 10
	#elif NND_UV_NUM == 10
		#define NND_UV_NO_OPACITY 5
		#define NND_UV_SUFFIX_OPACITY			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 11
	#elif NND_UV_NUM == 11
		#define NND_UV_NO_OPACITY 5 
		#define NND_UV_SUFFIX_OPACITY			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 12
	#elif NND_UV_NUM == 12
		#define NND_UV_NO_OPACITY 6 
		#define NND_UV_SUFFIX_OPACITY			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 13
	#elif NND_UV_NUM == 13
		#define NND_UV_NO_OPACITY 6
		#define NND_UV_SUFFIX_OPACITY			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 14
	#elif NND_UV_NUM == 14
		#define NND_UV_NO_OPACITY 7
		#define NND_UV_SUFFIX_OPACITY			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 15
	#elif NND_UV_NUM == 15
		#define NND_UV_NO_OPACITY 7
		#define NND_UV_SUFFIX_OPACITY			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 16
	#else
		#error NND_UV_NO_OPACITY overflow
	#endif
#endif

#if ( NND_OPT_TEX_USER1 != -1 )
	#if   NND_UV_NUM == 0
		#define NND_UV_NO_USER1 0
		#define NND_UV_SUFFIX_USER1			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 1
	#elif NND_UV_NUM == 1
		#define NND_UV_NO_USER1 0
		#define NND_UV_SUFFIX_USER1			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 2
	#elif NND_UV_NUM == 2
		#define NND_UV_NO_USER1 1
		#define NND_UV_SUFFIX_USER1			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 3
	#elif NND_UV_NUM == 3
		#define NND_UV_NO_USER1 1
		#define NND_UV_SUFFIX_USER1			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 4
	#elif NND_UV_NUM == 4
		#define NND_UV_NO_USER1 2
		#define NND_UV_SUFFIX_USER1			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 5
	#elif NND_UV_NUM == 5
		#define NND_UV_NO_USER1 2
		#define NND_UV_SUFFIX_USER1			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 6
	#elif NND_UV_NUM == 6
		#define NND_UV_NO_USER1 3
		#define NND_UV_SUFFIX_USER1			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 7
	#elif NND_UV_NUM == 7
		#define NND_UV_NO_USER1 3
		#define NND_UV_SUFFIX_USER1			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 8
	#elif NND_UV_NUM == 8
		#define NND_UV_NO_USER1 4
		#define NND_UV_SUFFIX_USER1			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 9
	#elif NND_UV_NUM == 9
		#define NND_UV_NO_USER1 4
		#define NND_UV_SUFFIX_USER1			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 10
	#elif NND_UV_NUM == 10
		#define NND_UV_NO_USER1 5
		#define NND_UV_SUFFIX_USER1			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 11
	#elif NND_UV_NUM == 11
		#define NND_UV_NO_USER1 5 
		#define NND_UV_SUFFIX_USER1			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 12
	#elif NND_UV_NUM == 12
		#define NND_UV_NO_USER1 6 
		#define NND_UV_SUFFIX_USER1			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 13
	#elif NND_UV_NUM == 13
		#define NND_UV_NO_USER1 6
		#define NND_UV_SUFFIX_USER1			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 14
	#elif NND_UV_NUM == 14
		#define NND_UV_NO_USER1 7
		#define NND_UV_SUFFIX_USER1			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 15
	#elif NND_UV_NUM == 15
		#define NND_UV_NO_USER1 7
		#define NND_UV_SUFFIX_USER1			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 16
	#else
		#error NND_UV_NO_USER1 overflow
	#endif
#endif

#if ( NND_OPT_TEX_USER2 != -1 )
	#if   NND_UV_NUM == 0
		#define NND_UV_NO_USER2 0
		#define NND_UV_SUFFIX_USER2			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 1
	#elif NND_UV_NUM == 1
		#define NND_UV_NO_USER2 0
		#define NND_UV_SUFFIX_USER2			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 2
	#elif NND_UV_NUM == 2
		#define NND_UV_NO_USER2 1
		#define NND_UV_SUFFIX_USER2			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 3
	#elif NND_UV_NUM == 3
		#define NND_UV_NO_USER2 1
		#define NND_UV_SUFFIX_USER2			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 4
	#elif NND_UV_NUM == 4
		#define NND_UV_NO_USER2 2
		#define NND_UV_SUFFIX_USER2			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 5
	#elif NND_UV_NUM == 5
		#define NND_UV_NO_USER2 2
		#define NND_UV_SUFFIX_USER2			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 6
	#elif NND_UV_NUM == 6
		#define NND_UV_NO_USER2 3
		#define NND_UV_SUFFIX_USER2			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 7
	#elif NND_UV_NUM == 7
		#define NND_UV_NO_USER2 3
		#define NND_UV_SUFFIX_USER2			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 8
	#elif NND_UV_NUM == 8
		#define NND_UV_NO_USER2 4
		#define NND_UV_SUFFIX_USER2			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 9
	#elif NND_UV_NUM == 9
		#define NND_UV_NO_USER2 4
		#define NND_UV_SUFFIX_USER2			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 10
	#elif NND_UV_NUM == 10
		#define NND_UV_NO_USER2 5
		#define NND_UV_SUFFIX_USER2			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 11
	#elif NND_UV_NUM == 11
		#define NND_UV_NO_USER2 5 
		#define NND_UV_SUFFIX_USER2			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 12
	#elif NND_UV_NUM == 12
		#define NND_UV_NO_USER2 6 
		#define NND_UV_SUFFIX_USER2			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 13
	#elif NND_UV_NUM == 13
		#define NND_UV_NO_USER2 6
		#define NND_UV_SUFFIX_USER2			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 14
	#elif NND_UV_NUM == 14
		#define NND_UV_NO_USER2 7
		#define NND_UV_SUFFIX_USER2			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 15
	#elif NND_UV_NUM == 15
		#define NND_UV_NO_USER2 7
		#define NND_UV_SUFFIX_USER2			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 16
	#else
		#error NND_UV_NO_USER2 overflow
	#endif
#endif

#if ( NND_OPT_TEX_USER3 != -1 )
	#if   NND_UV_NUM == 0
		#define NND_UV_NO_USER3 0
		#define NND_UV_SUFFIX_USER3			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 1
	#elif NND_UV_NUM == 1
		#define NND_UV_NO_USER3 0
		#define NND_UV_SUFFIX_USER3			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 2
	#elif NND_UV_NUM == 2
		#define NND_UV_NO_USER3 1
		#define NND_UV_SUFFIX_USER3			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 3
	#elif NND_UV_NUM == 3
		#define NND_UV_NO_USER3 1
		#define NND_UV_SUFFIX_USER3			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 4
	#elif NND_UV_NUM == 4
		#define NND_UV_NO_USER3 2
		#define NND_UV_SUFFIX_USER3			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 5
	#elif NND_UV_NUM == 5
		#define NND_UV_NO_USER3 2
		#define NND_UV_SUFFIX_USER3			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 6
	#elif NND_UV_NUM == 6
		#define NND_UV_NO_USER3 3
		#define NND_UV_SUFFIX_USER3			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 7
	#elif NND_UV_NUM == 7
		#define NND_UV_NO_USER3 3
		#define NND_UV_SUFFIX_USER3			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 8
	#elif NND_UV_NUM == 8
		#define NND_UV_NO_USER3 4
		#define NND_UV_SUFFIX_USER3			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 9
	#elif NND_UV_NUM == 9
		#define NND_UV_NO_USER3 4
		#define NND_UV_SUFFIX_USER3			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 10
	#elif NND_UV_NUM == 10
		#define NND_UV_NO_USER3 5
		#define NND_UV_SUFFIX_USER3			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 11
	#elif NND_UV_NUM == 11
		#define NND_UV_NO_USER3 5 
		#define NND_UV_SUFFIX_USER3			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 12
	#elif NND_UV_NUM == 12
		#define NND_UV_NO_USER3 6 
		#define NND_UV_SUFFIX_USER3			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 13
	#elif NND_UV_NUM == 13
		#define NND_UV_NO_USER3 6
		#define NND_UV_SUFFIX_USER3			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 14
	#elif NND_UV_NUM == 14
		#define NND_UV_NO_USER3 7
		#define NND_UV_SUFFIX_USER3			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 15
	#elif NND_UV_NUM == 15
		#define NND_UV_NO_USER3 7
		#define NND_UV_SUFFIX_USER3			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 16
	#else
		#error NND_UV_NO_USER3 overflow
	#endif
#endif

#if ( NND_OPT_TEX_USER4 != -1 )
	#if   NND_UV_NUM == 0
		#define NND_UV_NO_USER4 0
		#define NND_UV_SUFFIX_USER4			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 1
	#elif NND_UV_NUM == 1
		#define NND_UV_NO_USER4 0
		#define NND_UV_SUFFIX_USER4			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 2
	#elif NND_UV_NUM == 2
		#define NND_UV_NO_USER4 1
		#define NND_UV_SUFFIX_USER4			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 3
	#elif NND_UV_NUM == 3
		#define NND_UV_NO_USER4 1
		#define NND_UV_SUFFIX_USER4			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 4
	#elif NND_UV_NUM == 4
		#define NND_UV_NO_USER4 2
		#define NND_UV_SUFFIX_USER4			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 5
	#elif NND_UV_NUM == 5
		#define NND_UV_NO_USER4 2
		#define NND_UV_SUFFIX_USER4			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 6
	#elif NND_UV_NUM == 6
		#define NND_UV_NO_USER4 3
		#define NND_UV_SUFFIX_USER4			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 7
	#elif NND_UV_NUM == 7
		#define NND_UV_NO_USER4 3
		#define NND_UV_SUFFIX_USER4			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 8
	#elif NND_UV_NUM == 8
		#define NND_UV_NO_USER4 4
		#define NND_UV_SUFFIX_USER4			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 9
	#elif NND_UV_NUM == 9
		#define NND_UV_NO_USER4 4
		#define NND_UV_SUFFIX_USER4			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 10
	#elif NND_UV_NUM == 10
		#define NND_UV_NO_USER4 5
		#define NND_UV_SUFFIX_USER4			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 11
	#elif NND_UV_NUM == 11
		#define NND_UV_NO_USER4 5 
		#define NND_UV_SUFFIX_USER4			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 12
	#elif NND_UV_NUM == 12
		#define NND_UV_NO_USER4 6 
		#define NND_UV_SUFFIX_USER4			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 13
	#elif NND_UV_NUM == 13
		#define NND_UV_NO_USER4 6
		#define NND_UV_SUFFIX_USER4			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 14
	#elif NND_UV_NUM == 14
		#define NND_UV_NO_USER4 7
		#define NND_UV_SUFFIX_USER4			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 15
	#elif NND_UV_NUM == 15
		#define NND_UV_NO_USER4 7
		#define NND_UV_SUFFIX_USER4			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 16
	#else
		#error NND_UV_NO_USER4 overflow
	#endif
#endif

#if ( NND_OPT_TEX_USER5 != -1 )
	#if   NND_UV_NUM == 0
		#define NND_UV_NO_USER5 0
		#define NND_UV_SUFFIX_USER5			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 1
	#elif NND_UV_NUM == 1
		#define NND_UV_NO_USER5 0
		#define NND_UV_SUFFIX_USER5			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 2
	#elif NND_UV_NUM == 2
		#define NND_UV_NO_USER5 1
		#define NND_UV_SUFFIX_USER5			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 3
	#elif NND_UV_NUM == 3
		#define NND_UV_NO_USER5 1
		#define NND_UV_SUFFIX_USER5			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 4
	#elif NND_UV_NUM == 4
		#define NND_UV_NO_USER5 2
		#define NND_UV_SUFFIX_USER5			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 5
	#elif NND_UV_NUM == 5
		#define NND_UV_NO_USER5 2
		#define NND_UV_SUFFIX_USER5			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 6
	#elif NND_UV_NUM == 6
		#define NND_UV_NO_USER5 3
		#define NND_UV_SUFFIX_USER5			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 7
	#elif NND_UV_NUM == 7
		#define NND_UV_NO_USER5 3
		#define NND_UV_SUFFIX_USER5			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 8
	#elif NND_UV_NUM == 8
		#define NND_UV_NO_USER5 4
		#define NND_UV_SUFFIX_USER5			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 9
	#elif NND_UV_NUM == 9
		#define NND_UV_NO_USER5 4
		#define NND_UV_SUFFIX_USER5			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 10
	#elif NND_UV_NUM == 10
		#define NND_UV_NO_USER5 5
		#define NND_UV_SUFFIX_USER5			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 11
	#elif NND_UV_NUM == 11
		#define NND_UV_NO_USER5 5 
		#define NND_UV_SUFFIX_USER5			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 12
	#elif NND_UV_NUM == 12
		#define NND_UV_NO_USER5 6 
		#define NND_UV_SUFFIX_USER5			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 13
	#elif NND_UV_NUM == 13
		#define NND_UV_NO_USER5 6
		#define NND_UV_SUFFIX_USER5			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 14
	#elif NND_UV_NUM == 14
		#define NND_UV_NO_USER5 7
		#define NND_UV_SUFFIX_USER5			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 15
	#elif NND_UV_NUM == 15
		#define NND_UV_NO_USER5 7
		#define NND_UV_SUFFIX_USER5			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 16
	#else
		#error NND_UV_NO_USER5 overflow
	#endif
#endif

#if ( NND_OPT_TEX_USER6 != -1 )
	#if   NND_UV_NUM == 0
		#define NND_UV_NO_USER6 0
		#define NND_UV_SUFFIX_USER6			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 1
	#elif NND_UV_NUM == 1
		#define NND_UV_NO_USER6 0
		#define NND_UV_SUFFIX_USER6			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 2
	#elif NND_UV_NUM == 2
		#define NND_UV_NO_USER6 1
		#define NND_UV_SUFFIX_USER6			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 3
	#elif NND_UV_NUM == 3
		#define NND_UV_NO_USER6 1
		#define NND_UV_SUFFIX_USER6			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 4
	#elif NND_UV_NUM == 4
		#define NND_UV_NO_USER6 2
		#define NND_UV_SUFFIX_USER6			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 5
	#elif NND_UV_NUM == 5
		#define NND_UV_NO_USER6 2
		#define NND_UV_SUFFIX_USER6			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 6
	#elif NND_UV_NUM == 6
		#define NND_UV_NO_USER6 3
		#define NND_UV_SUFFIX_USER6			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 7
	#elif NND_UV_NUM == 7
		#define NND_UV_NO_USER6 3
		#define NND_UV_SUFFIX_USER6			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 8
	#elif NND_UV_NUM == 8
		#define NND_UV_NO_USER6 4
		#define NND_UV_SUFFIX_USER6			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 9
	#elif NND_UV_NUM == 9
		#define NND_UV_NO_USER6 4
		#define NND_UV_SUFFIX_USER6			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 10
	#elif NND_UV_NUM == 10
		#define NND_UV_NO_USER6 5
		#define NND_UV_SUFFIX_USER6			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 11
	#elif NND_UV_NUM == 11
		#define NND_UV_NO_USER6 5 
		#define NND_UV_SUFFIX_USER6			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 12
	#elif NND_UV_NUM == 12
		#define NND_UV_NO_USER6 6 
		#define NND_UV_SUFFIX_USER6			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 13
	#elif NND_UV_NUM == 13
		#define NND_UV_NO_USER6 6
		#define NND_UV_SUFFIX_USER6			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 14
	#elif NND_UV_NUM == 14
		#define NND_UV_NO_USER6 7
		#define NND_UV_SUFFIX_USER6			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 15
	#elif NND_UV_NUM == 15
		#define NND_UV_NO_USER6 7
		#define NND_UV_SUFFIX_USER6			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 16
	#else
		#error NND_UV_NO_USER6 overflow
	#endif
#endif

#if ( NND_OPT_TEX_USER7 != -1 )
	#if   NND_UV_NUM == 0
		#define NND_UV_NO_USER7 0
		#define NND_UV_SUFFIX_USER7			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 1
	#elif NND_UV_NUM == 1
		#define NND_UV_NO_USER7 0
		#define NND_UV_SUFFIX_USER7			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 2
	#elif NND_UV_NUM == 2
		#define NND_UV_NO_USER7 1
		#define NND_UV_SUFFIX_USER7			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 3
	#elif NND_UV_NUM == 3
		#define NND_UV_NO_USER7 1
		#define NND_UV_SUFFIX_USER7			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 4
	#elif NND_UV_NUM == 4
		#define NND_UV_NO_USER7 2
		#define NND_UV_SUFFIX_USER7			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 5
	#elif NND_UV_NUM == 5
		#define NND_UV_NO_USER7 2
		#define NND_UV_SUFFIX_USER7			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 6
	#elif NND_UV_NUM == 6
		#define NND_UV_NO_USER7 3
		#define NND_UV_SUFFIX_USER7			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 7
	#elif NND_UV_NUM == 7
		#define NND_UV_NO_USER7 3
		#define NND_UV_SUFFIX_USER7			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 8
	#elif NND_UV_NUM == 8
		#define NND_UV_NO_USER7 4
		#define NND_UV_SUFFIX_USER7			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 9
	#elif NND_UV_NUM == 9
		#define NND_UV_NO_USER7 4
		#define NND_UV_SUFFIX_USER7			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 10
	#elif NND_UV_NUM == 10
		#define NND_UV_NO_USER7 5
		#define NND_UV_SUFFIX_USER7			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 11
	#elif NND_UV_NUM == 11
		#define NND_UV_NO_USER7 5 
		#define NND_UV_SUFFIX_USER7			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 12
	#elif NND_UV_NUM == 12
		#define NND_UV_NO_USER7 6 
		#define NND_UV_SUFFIX_USER7			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 13
	#elif NND_UV_NUM == 13
		#define NND_UV_NO_USER7 6
		#define NND_UV_SUFFIX_USER7			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 14
	#elif NND_UV_NUM == 14
		#define NND_UV_NO_USER7 7
		#define NND_UV_SUFFIX_USER7			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 15
	#elif NND_UV_NUM == 15
		#define NND_UV_NO_USER7 7
		#define NND_UV_SUFFIX_USER7			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 16
	#else
		#error NND_UV_NO_USER7 overflow
	#endif
#endif

#if ( NND_OPT_TEX_USER8 != -1 )
	#if   NND_UV_NUM == 0
		#define NND_UV_NO_USER8 0
		#define NND_UV_SUFFIX_USER8			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 1
	#elif NND_UV_NUM == 1
		#define NND_UV_NO_USER8 0
		#define NND_UV_SUFFIX_USER8			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 2
	#elif NND_UV_NUM == 2
		#define NND_UV_NO_USER8 1
		#define NND_UV_SUFFIX_USER8			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 3
	#elif NND_UV_NUM == 3
		#define NND_UV_NO_USER8 1
		#define NND_UV_SUFFIX_USER8			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 4
	#elif NND_UV_NUM == 4
		#define NND_UV_NO_USER8 2
		#define NND_UV_SUFFIX_USER8			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 5
	#elif NND_UV_NUM == 5
		#define NND_UV_NO_USER8 2
		#define NND_UV_SUFFIX_USER8			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 6
	#elif NND_UV_NUM == 6
		#define NND_UV_NO_USER8 3
		#define NND_UV_SUFFIX_USER8			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 7
	#elif NND_UV_NUM == 7
		#define NND_UV_NO_USER8 3
		#define NND_UV_SUFFIX_USER8			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 8
	#elif NND_UV_NUM == 8
		#define NND_UV_NO_USER8 4
		#define NND_UV_SUFFIX_USER8			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 9
	#elif NND_UV_NUM == 9
		#define NND_UV_NO_USER8 4
		#define NND_UV_SUFFIX_USER8			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 10
	#elif NND_UV_NUM == 10
		#define NND_UV_NO_USER8 5
		#define NND_UV_SUFFIX_USER8			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 11
	#elif NND_UV_NUM == 11
		#define NND_UV_NO_USER8 5 
		#define NND_UV_SUFFIX_USER8			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 12
	#elif NND_UV_NUM == 12
		#define NND_UV_NO_USER8 6 
		#define NND_UV_SUFFIX_USER8			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 13
	#elif NND_UV_NUM == 13
		#define NND_UV_NO_USER8 6
		#define NND_UV_SUFFIX_USER8			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 14
	#elif NND_UV_NUM == 14
		#define NND_UV_NO_USER8 7
		#define NND_UV_SUFFIX_USER8			xy
		#undef  NND_UV_NUM
		#define NND_UV_NUM 15
	#elif NND_UV_NUM == 15
		#define NND_UV_NO_USER8 7
		#define NND_UV_SUFFIX_USER8			zw
		#undef  NND_UV_NUM
		#define NND_UV_NUM 16
	#else
		#error NND_UV_NO_USER8 overflow
	#endif
#endif

#if (NND_UV_NUM/2) == ((NND_UV_NUM+1)/2)
#define NND_MAX_UV	(NND_UV_NUM/2)
#else
#define NND_MAX_UV	( (NND_UV_NUM/2) + 1 )
#endif

#endif // ifndef __NNDUV_DEF_H__
