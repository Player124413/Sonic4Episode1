
#ifndef __SAMPLER_DEF_H__
#define __SAMPLER_DEF_H__

#define SAMPLER_TAIL 0

#if ( NND_OPT_TEX_NORMAL != -1 )
#define SAMPLER_NORMAL     s0
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       1
#endif
#if ( NND_OPT_TEX_BASE != -1 )
#if   SAMPLER_TAIL == 0
#define SAMPLER_BASE       s0
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       1
#elif SAMPLER_TAIL == 1
#define SAMPLER_BASE       s1
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       2
#endif
#endif
#if ( NND_OPT_TEX_DECAL != -1 )
#if   SAMPLER_TAIL == 0
#define SAMPLER_DECAL      s0
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       1
#elif SAMPLER_TAIL == 1
#define SAMPLER_DECAL      s1
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       2
#elif SAMPLER_TAIL == 2
#define SAMPLER_DECAL      s2
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       3
#endif
#endif
#if ( NND_OPT_TEX_DECAL2 != -1 )
#if   SAMPLER_TAIL == 0
#define SAMPLER_DECAL2     s0
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       1
#elif SAMPLER_TAIL == 1
#define SAMPLER_DECAL2     s1
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       2
#elif SAMPLER_TAIL == 2
#define SAMPLER_DECAL2     s2
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       3
#elif SAMPLER_TAIL == 3
#define SAMPLER_DECAL2     s3
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       4
#endif
#endif
#if ( NND_OPT_TEX_DECAL3 != -1 )
#if   SAMPLER_TAIL == 0
#define SAMPLER_DECAL3     s0
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       1
#elif SAMPLER_TAIL == 1
#define SAMPLER_DECAL3     s1
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       2
#elif SAMPLER_TAIL == 2
#define SAMPLER_DECAL3     s2
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       3
#elif SAMPLER_TAIL == 3
#define SAMPLER_DECAL3     s3
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       4
#elif SAMPLER_TAIL == 4
#define SAMPLER_DECAL3     s4
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       5
#endif
#endif

#if ( NND_OPT_TEX_SPECULAR != -1 )
#if   SAMPLER_TAIL == 0
#define SAMPLER_SPECULAR  s0
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       1
#elif SAMPLER_TAIL == 1
#define SAMPLER_SPECULAR  s1
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       2
#elif SAMPLER_TAIL == 2
#define SAMPLER_SPECULAR  s2
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       3
#elif SAMPLER_TAIL == 3
#define SAMPLER_SPECULAR  s3
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       4
#elif SAMPLER_TAIL == 4
#define SAMPLER_SPECULAR  s4
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       5
#elif SAMPLER_TAIL == 5
#define SAMPLER_SPECULAR  s5
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       6
#endif
#endif
#if ( NND_OPT_TEX_SHININESS != -1 )
#if   SAMPLER_TAIL == 0
#define SAMPLER_SHININESS  s0
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       1
#elif SAMPLER_TAIL == 1
#define SAMPLER_SHININESS  s1
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       2
#elif SAMPLER_TAIL == 2
#define SAMPLER_SHININESS  s2
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       3
#elif SAMPLER_TAIL == 3
#define SAMPLER_SHININESS  s3
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       4
#elif SAMPLER_TAIL == 4
#define SAMPLER_SHININESS  s4
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       5
#elif SAMPLER_TAIL == 5
#define SAMPLER_SHININESS  s5
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       6
#elif SAMPLER_TAIL == 6
#define SAMPLER_SHININESS  s6
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       7
#endif
#endif
#if ( NND_OPT_TEX_DUALPARABOLOID != -1 )
#if   SAMPLER_TAIL == 0
#define SAMPLER_DUALPARA   s0
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       1
#elif SAMPLER_TAIL == 1
#define SAMPLER_DUALPARA   s1
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       2
#elif SAMPLER_TAIL == 2
#define SAMPLER_DUALPARA   s2
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       3
#elif SAMPLER_TAIL == 3
#define SAMPLER_DUALPARA   s3
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       4
#elif SAMPLER_TAIL == 4
#define SAMPLER_DUALPARA   s4
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       5
#elif SAMPLER_TAIL == 5
#define SAMPLER_DUALPARA   s5
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       6
#elif SAMPLER_TAIL == 6
#define SAMPLER_DUALPARA   s6
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       7
#elif SAMPLER_TAIL == 7
#define SAMPLER_DUALPARA   s7
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       8
#endif
#endif
#if ( NND_OPT_TEX_ENVMASK != -1 )
#if   SAMPLER_TAIL == 0
#define SAMPLER_ENVMASK    s0
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       1
#elif SAMPLER_TAIL == 1
#define SAMPLER_ENVMASK    s1
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       2
#elif SAMPLER_TAIL == 2
#define SAMPLER_ENVMASK    s2
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       3
#elif SAMPLER_TAIL == 3
#define SAMPLER_ENVMASK    s3
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       4
#elif SAMPLER_TAIL == 4
#define SAMPLER_ENVMASK    s4
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       5
#elif SAMPLER_TAIL == 5
#define SAMPLER_ENVMASK    s5
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       6
#elif SAMPLER_TAIL == 6
#define SAMPLER_ENVMASK    s6
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       7
#elif SAMPLER_TAIL == 7
#define SAMPLER_ENVMASK    s7
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       8
#elif SAMPLER_TAIL == 8
#define SAMPLER_ENVMASK    s8
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       9
#endif
#endif
#if ( NND_OPT_TEX_MODULATE != -1 )
#if   SAMPLER_TAIL == 0
#define SAMPLER_MODULATE   s0
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       1
#elif SAMPLER_TAIL == 1
#define SAMPLER_MODULATE   s1
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       2
#elif SAMPLER_TAIL == 2
#define SAMPLER_MODULATE   s2
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       3
#elif SAMPLER_TAIL == 3
#define SAMPLER_MODULATE   s3
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       4
#elif SAMPLER_TAIL == 4
#define SAMPLER_MODULATE   s4
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       5
#elif SAMPLER_TAIL == 5
#define SAMPLER_MODULATE   s5
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       6
#elif SAMPLER_TAIL == 6
#define SAMPLER_MODULATE   s6
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       7
#elif SAMPLER_TAIL == 7
#define SAMPLER_MODULATE   s7
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       8
#elif SAMPLER_TAIL == 8
#define SAMPLER_MODULATE   s8
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       9
#elif SAMPLER_TAIL == 9
#define SAMPLER_MODULATE   s9
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       10
#endif
#endif
#if ( NND_OPT_TEX_ADD != -1 )
#if   SAMPLER_TAIL == 0
#define SAMPLER_ADD        s0
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       1
#elif SAMPLER_TAIL == 1
#define SAMPLER_ADD        s1
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       2
#elif SAMPLER_TAIL == 2
#define SAMPLER_ADD        s2
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       3
#elif SAMPLER_TAIL == 3
#define SAMPLER_ADD        s3
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       4
#elif SAMPLER_TAIL == 4
#define SAMPLER_ADD        s4
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       5
#elif SAMPLER_TAIL == 5
#define SAMPLER_ADD        s5
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       6
#elif SAMPLER_TAIL == 6
#define SAMPLER_ADD        s6
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       7
#elif SAMPLER_TAIL == 7
#define SAMPLER_ADD        s7
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       8
#elif SAMPLER_TAIL == 8
#define SAMPLER_ADD        s8
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       9
#elif SAMPLER_TAIL == 9
#define SAMPLER_ADD        s9
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       10
#elif SAMPLER_TAIL == 10
#define SAMPLER_ADD        s10
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       11
#endif
#endif

#if ( NND_OPT_TEX_OPACITY != -1 )
#if   SAMPLER_TAIL == 0
#define SAMPLER_OPACITY    s0
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       1
#elif SAMPLER_TAIL == 1
#define SAMPLER_OPACITY    s1
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       2
#elif SAMPLER_TAIL == 2
#define SAMPLER_OPACITY    s2
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       3
#elif SAMPLER_TAIL == 3
#define SAMPLER_OPACITY    s3
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       4
#elif SAMPLER_TAIL == 4
#define SAMPLER_OPACITY    s4
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       5
#elif SAMPLER_TAIL == 5
#define SAMPLER_OPACITY    s5
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       6
#elif SAMPLER_TAIL == 6
#define SAMPLER_OPACITY    s6
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       7
#elif SAMPLER_TAIL == 7
#define SAMPLER_OPACITY    s7
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       8
#elif SAMPLER_TAIL == 8
#define SAMPLER_OPACITY    s8
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       9
#elif SAMPLER_TAIL == 9
#define SAMPLER_OPACITY    s9
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       10
#elif SAMPLER_TAIL == 10
#define SAMPLER_OPACITY    s10
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       11
#elif SAMPLER_TAIL == 11
#define SAMPLER_OPACITY    s11
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       12
#elif SAMPLER_TAIL == 12
#define SAMPLER_OPACITY    s12
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       13
#endif
#endif
#if ( NND_OPT_TEX_USER1 != -1 )
#if   SAMPLER_TAIL == 0
#define SAMPLER_USER1      s0
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       1
#elif SAMPLER_TAIL == 1
#define SAMPLER_USER1      s1
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       2
#elif SAMPLER_TAIL == 2
#define SAMPLER_USER1      s2
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       3
#elif SAMPLER_TAIL == 3
#define SAMPLER_USER1      s3
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       4
#elif SAMPLER_TAIL == 4
#define SAMPLER_USER1      s4
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       5
#elif SAMPLER_TAIL == 5
#define SAMPLER_USER1      s5
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       6
#elif SAMPLER_TAIL == 6
#define SAMPLER_USER1      s6
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       7
#elif SAMPLER_TAIL == 7
#define SAMPLER_USER1      s7
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       8
#elif SAMPLER_TAIL == 8
#define SAMPLER_USER1      s8
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       9
#elif SAMPLER_TAIL == 9
#define SAMPLER_USER1      s9
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       10
#elif SAMPLER_TAIL == 10
#define SAMPLER_USER1      s10
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       11
#elif SAMPLER_TAIL == 11
#define SAMPLER_USER1      s11
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       12
#elif SAMPLER_TAIL == 12
#define SAMPLER_USER1      s12
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       13
#elif SAMPLER_TAIL == 13
#define SAMPLER_USER1      s13
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       14
#elif SAMPLER_TAIL == 14
#define SAMPLER_USER1      s14
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       15
#endif
#endif
#if ( NND_OPT_TEX_USER2 != -1 )
#if   SAMPLER_TAIL == 0
#define SAMPLER_USER2      s0
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       1
#elif SAMPLER_TAIL == 1
#define SAMPLER_USER2      s1
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       2
#elif SAMPLER_TAIL == 2
#define SAMPLER_USER2      s2
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       3
#elif SAMPLER_TAIL == 3
#define SAMPLER_USER2      s3
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       4
#elif SAMPLER_TAIL == 4
#define SAMPLER_USER2      s4
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       5
#elif SAMPLER_TAIL == 5
#define SAMPLER_USER2      s5
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       6
#elif SAMPLER_TAIL == 6
#define SAMPLER_USER2      s6
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       7
#elif SAMPLER_TAIL == 7
#define SAMPLER_USER2      s7
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       8
#elif SAMPLER_TAIL == 8
#define SAMPLER_USER2      s8
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       9
#elif SAMPLER_TAIL == 9
#define SAMPLER_USER2      s9
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       10
#elif SAMPLER_TAIL == 10
#define SAMPLER_USER2      s10
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       11
#elif SAMPLER_TAIL == 11
#define SAMPLER_USER2      s11
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       12
#elif SAMPLER_TAIL == 12
#define SAMPLER_USER2      s12
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       13
#elif SAMPLER_TAIL == 13
#define SAMPLER_USER2      s13
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       14
#elif SAMPLER_TAIL == 14
#define SAMPLER_USER2      s14
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       15
#elif SAMPLER_TAIL == 15
#define SAMPLER_USER2      s15
#undef  SAMPLER_TAIL
#endif
#endif
#if ( NND_OPT_TEX_USER3 != -1 )
#if   SAMPLER_TAIL == 0
#define SAMPLER_USER3      s0
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       1
#elif SAMPLER_TAIL == 1
#define SAMPLER_USER3      s1
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       2
#elif SAMPLER_TAIL == 2
#define SAMPLER_USER3      s2
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       3
#elif SAMPLER_TAIL == 3
#define SAMPLER_USER3      s3
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       4
#elif SAMPLER_TAIL == 4
#define SAMPLER_USER3      s4
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       5
#elif SAMPLER_TAIL == 5
#define SAMPLER_USER3      s5
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       6
#elif SAMPLER_TAIL == 6
#define SAMPLER_USER3      s6
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       7
#elif SAMPLER_TAIL == 7
#define SAMPLER_USER3      s7
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       8
#elif SAMPLER_TAIL == 8
#define SAMPLER_USER3      s8
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       9
#elif SAMPLER_TAIL == 9
#define SAMPLER_USER3      s9
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       10
#elif SAMPLER_TAIL == 10
#define SAMPLER_USER3      s10
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       11
#elif SAMPLER_TAIL == 11
#define SAMPLER_USER3      s11
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       12
#elif SAMPLER_TAIL == 12
#define SAMPLER_USER3      s12
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       13
#elif SAMPLER_TAIL == 13
#define SAMPLER_USER3      s13
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       14
#elif SAMPLER_TAIL == 14
#define SAMPLER_USER3      s14
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       15
#elif SAMPLER_TAIL == 15
#define SAMPLER_USER3      s15
#undef  SAMPLER_TAIL
#endif
#endif
#if ( NND_OPT_TEX_USER4 != -1 )
#if   SAMPLER_TAIL == 0
#define SAMPLER_USER4      s0
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       1
#elif SAMPLER_TAIL == 1
#define SAMPLER_USER4      s1
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       2
#elif SAMPLER_TAIL == 2
#define SAMPLER_USER4      s2
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       3
#elif SAMPLER_TAIL == 3
#define SAMPLER_USER4      s3
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       4
#elif SAMPLER_TAIL == 4
#define SAMPLER_USER4      s4
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       5
#elif SAMPLER_TAIL == 5
#define SAMPLER_USER4      s5
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       6
#elif SAMPLER_TAIL == 6
#define SAMPLER_USER4      s6
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       7
#elif SAMPLER_TAIL == 7
#define SAMPLER_USER4      s7
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       8
#elif SAMPLER_TAIL == 8
#define SAMPLER_USER4      s8
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       9
#elif SAMPLER_TAIL == 9
#define SAMPLER_USER4      s9
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       10
#elif SAMPLER_TAIL == 10
#define SAMPLER_USER4      s10
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       11
#elif SAMPLER_TAIL == 11
#define SAMPLER_USER4      s11
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       12
#elif SAMPLER_TAIL == 12
#define SAMPLER_USER4      s12
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       13
#elif SAMPLER_TAIL == 13
#define SAMPLER_USER4      s13
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       14
#elif SAMPLER_TAIL == 14
#define SAMPLER_USER4      s14
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       15
#elif SAMPLER_TAIL == 15
#define SAMPLER_USER4      s15
#endif
#endif
#if ( NND_OPT_TEX_USER5 != -1 )
#if   SAMPLER_TAIL == 0
#define SAMPLER_USER5      s0
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       1
#elif SAMPLER_TAIL == 1
#define SAMPLER_USER5      s1
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       2
#elif SAMPLER_TAIL == 2
#define SAMPLER_USER5      s2
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       3
#elif SAMPLER_TAIL == 3
#define SAMPLER_USER5      s3
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       4
#elif SAMPLER_TAIL == 4
#define SAMPLER_USER5      s4
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       5
#elif SAMPLER_TAIL == 5
#define SAMPLER_USER5      s5
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       6
#elif SAMPLER_TAIL == 6
#define SAMPLER_USER5      s6
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       7
#elif SAMPLER_TAIL == 7
#define SAMPLER_USER5      s7
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       8
#elif SAMPLER_TAIL == 8
#define SAMPLER_USER5      s8
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       9
#elif SAMPLER_TAIL == 9
#define SAMPLER_USER5      s9
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       10
#elif SAMPLER_TAIL == 10
#define SAMPLER_USER5      s10
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       11
#elif SAMPLER_TAIL == 11
#define SAMPLER_USER5      s11
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       12
#elif SAMPLER_TAIL == 12
#define SAMPLER_USER5      s12
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       13
#elif SAMPLER_TAIL == 13
#define SAMPLER_USER5      s13
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       14
#elif SAMPLER_TAIL == 14
#define SAMPLER_USER5      s14
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       15
#elif SAMPLER_TAIL == 15
#define SAMPLER_USER5      s15
#endif
#endif
#if ( NND_OPT_TEX_USER6 != -1 )
#if   SAMPLER_TAIL == 0
#define SAMPLER_USER6      s0
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       1
#elif SAMPLER_TAIL == 1
#define SAMPLER_USER6      s1
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       2
#elif SAMPLER_TAIL == 2
#define SAMPLER_USER6      s2
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       3
#elif SAMPLER_TAIL == 3
#define SAMPLER_USER6      s3
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       4
#elif SAMPLER_TAIL == 4
#define SAMPLER_USER6      s4
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       5
#elif SAMPLER_TAIL == 5
#define SAMPLER_USER6      s5
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       6
#elif SAMPLER_TAIL == 6
#define SAMPLER_USER6      s6
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       7
#elif SAMPLER_TAIL == 7
#define SAMPLER_USER6      s7
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       8
#elif SAMPLER_TAIL == 8
#define SAMPLER_USER6      s8
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       9
#elif SAMPLER_TAIL == 9
#define SAMPLER_USER6      s9
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       10
#elif SAMPLER_TAIL == 10
#define SAMPLER_USER6      s10
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       11
#elif SAMPLER_TAIL == 11
#define SAMPLER_USER6      s11
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       12
#elif SAMPLER_TAIL == 12
#define SAMPLER_USER6      s12
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       13
#elif SAMPLER_TAIL == 13
#define SAMPLER_USER6      s13
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       14
#elif SAMPLER_TAIL == 14
#define SAMPLER_USER6      s14
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       15
#elif SAMPLER_TAIL == 15
#define SAMPLER_USER6      s15
#endif
#endif
#if ( NND_OPT_TEX_USER7 != -1 )
#if   SAMPLER_TAIL == 0
#define SAMPLER_USER7      s0
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       1
#elif SAMPLER_TAIL == 1
#define SAMPLER_USER7      s1
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       2
#elif SAMPLER_TAIL == 2
#define SAMPLER_USER7      s2
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       3
#elif SAMPLER_TAIL == 3
#define SAMPLER_USER7      s3
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       4
#elif SAMPLER_TAIL == 4
#define SAMPLER_USER7      s4
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       5
#elif SAMPLER_TAIL == 5
#define SAMPLER_USER7      s5
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       6
#elif SAMPLER_TAIL == 6
#define SAMPLER_USER7      s6
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       7
#elif SAMPLER_TAIL == 7
#define SAMPLER_USER7      s7
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       8
#elif SAMPLER_TAIL == 8
#define SAMPLER_USER7      s8
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       9
#elif SAMPLER_TAIL == 9
#define SAMPLER_USER7      s9
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       10
#elif SAMPLER_TAIL == 10
#define SAMPLER_USER7      s10
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       11
#elif SAMPLER_TAIL == 11
#define SAMPLER_USER7      s11
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       12
#elif SAMPLER_TAIL == 12
#define SAMPLER_USER7      s12
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       13
#elif SAMPLER_TAIL == 13
#define SAMPLER_USER7      s13
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       14
#elif SAMPLER_TAIL == 14
#define SAMPLER_USER7      s14
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       15
#elif SAMPLER_TAIL == 15
#define SAMPLER_USER7      s15
#endif
#endif
#if ( NND_OPT_TEX_USER8 != -1 )
#if   SAMPLER_TAIL == 0
#define SAMPLER_USER8      s0
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       1
#elif SAMPLER_TAIL == 1
#define SAMPLER_USER8      s1
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       2
#elif SAMPLER_TAIL == 2
#define SAMPLER_USER8      s2
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       3
#elif SAMPLER_TAIL == 3
#define SAMPLER_USER8      s3
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       4
#elif SAMPLER_TAIL == 4
#define SAMPLER_USER8      s4
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       5
#elif SAMPLER_TAIL == 5
#define SAMPLER_USER8      s5
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       6
#elif SAMPLER_TAIL == 6
#define SAMPLER_USER8      s6
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       7
#elif SAMPLER_TAIL == 7
#define SAMPLER_USER8      s7
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       8
#elif SAMPLER_TAIL == 8
#define SAMPLER_USER8      s8
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       9
#elif SAMPLER_TAIL == 9
#define SAMPLER_USER8      s9
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       10
#elif SAMPLER_TAIL == 10
#define SAMPLER_USER8      s10
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       11
#elif SAMPLER_TAIL == 11
#define SAMPLER_USER8      s11
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       12
#elif SAMPLER_TAIL == 12
#define SAMPLER_USER8      s12
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       13
#elif SAMPLER_TAIL == 13
#define SAMPLER_USER8      s13
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       14
#elif SAMPLER_TAIL == 14
#define SAMPLER_USER8      s14
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       15
#elif SAMPLER_TAIL == 15
#define SAMPLER_USER8      s15
#endif
#endif
#if ( NND_OPT_TEX_SHADOW != -1 )
#if   SAMPLER_TAIL == 0
#define SAMPLER_SHADOW     s0
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       1
#elif SAMPLER_TAIL == 1
#define SAMPLER_SHADOW     s1
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       2
#elif SAMPLER_TAIL == 2
#define SAMPLER_SHADOW     s2
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       3
#elif SAMPLER_TAIL == 3
#define SAMPLER_SHADOW     s3
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       4
#elif SAMPLER_TAIL == 4
#define SAMPLER_SHADOW     s4
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       5
#elif SAMPLER_TAIL == 5
#define SAMPLER_SHADOW     s5
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       6
#elif SAMPLER_TAIL == 6
#define SAMPLER_SHADOW     s6
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       7
#elif SAMPLER_TAIL == 7
#define SAMPLER_SHADOW     s7
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       8
#elif SAMPLER_TAIL == 8
#define SAMPLER_SHADOW     s8
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       9
#elif SAMPLER_TAIL == 9
#define SAMPLER_SHADOW     s9
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       10
#elif SAMPLER_TAIL == 10
#define SAMPLER_SHADOW     s10
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       11
#elif SAMPLER_TAIL == 11
#define SAMPLER_SHADOW     s11
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       12
#elif SAMPLER_TAIL == 12
#define SAMPLER_SHADOW     s12
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       13
#elif SAMPLER_TAIL == 13
#define SAMPLER_SHADOW     s13
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       14
#elif SAMPLER_TAIL == 14
#define SAMPLER_SHADOW     s14
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       15
#elif SAMPLER_TAIL == 15
#define SAMPLER_SHADOW     s15
#endif
#endif
#if ( NND_OPT_TEX_SHADOW2 != -1 )
#if   SAMPLER_TAIL == 0
#define SAMPLER_SHADOW2    s0
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       1
#elif SAMPLER_TAIL == 1
#define SAMPLER_SHADOW2    s1
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       2
#elif SAMPLER_TAIL == 2
#define SAMPLER_SHADOW2    s2
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       3
#elif SAMPLER_TAIL == 3
#define SAMPLER_SHADOW2    s3
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       4
#elif SAMPLER_TAIL == 4
#define SAMPLER_SHADOW2    s4
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       5
#elif SAMPLER_TAIL == 5
#define SAMPLER_SHADOW2    s5
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       6
#elif SAMPLER_TAIL == 6
#define SAMPLER_SHADOW2    s6
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       7
#elif SAMPLER_TAIL == 7
#define SAMPLER_SHADOW2    s7
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       8
#elif SAMPLER_TAIL == 8
#define SAMPLER_SHADOW2    s8
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       9
#elif SAMPLER_TAIL == 9
#define SAMPLER_SHADOW2    s9
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       10
#elif SAMPLER_TAIL == 10
#define SAMPLER_SHADOW2    s10
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       11
#elif SAMPLER_TAIL == 11
#define SAMPLER_SHADOW2    s11
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       12
#elif SAMPLER_TAIL == 12
#define SAMPLER_SHADOW2    s12
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       13
#elif SAMPLER_TAIL == 13
#define SAMPLER_SHADOW2    s13
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       14
#elif SAMPLER_TAIL == 14
#define SAMPLER_SHADOW2    s14
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL       15
#elif SAMPLER_TAIL == 15
#define SAMPLER_SHADOW2    s15
#endif
#endif

#if ( NND_OPT_TEX_SAMPLER2D1 != -1 )
#if   SAMPLER_TAIL == 0
#define SAMPLER_SAMPLER2D1   s0
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         1
#elif SAMPLER_TAIL == 1
#define SAMPLER_SAMPLER2D1   s1
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         2
#elif SAMPLER_TAIL == 2
#define SAMPLER_SAMPLER2D1   s2
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         3
#elif SAMPLER_TAIL == 3
#define SAMPLER_SAMPLER2D1   s3
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         4
#elif SAMPLER_TAIL == 4
#define SAMPLER_SAMPLER2D1   s4
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         5
#elif SAMPLER_TAIL == 5
#define SAMPLER_SAMPLER2D1   s5
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         6
#elif SAMPLER_TAIL == 6
#define SAMPLER_SAMPLER2D1   s6
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         7
#elif SAMPLER_TAIL == 7
#define SAMPLER_SAMPLER2D1   s7
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         8
#elif SAMPLER_TAIL == 8
#define SAMPLER_SAMPLER2D1   s8
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         9
#elif SAMPLER_TAIL == 9
#define SAMPLER_SAMPLER2D1   s9
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         10
#elif SAMPLER_TAIL == 10
#define SAMPLER_SAMPLER2D1   s10
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         11
#elif SAMPLER_TAIL == 11
#define SAMPLER_SAMPLER2D1   s11
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         12
#elif SAMPLER_TAIL == 12
#define SAMPLER_SAMPLER2D1   s12
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         13
#elif SAMPLER_TAIL == 13
#define SAMPLER_SAMPLER2D1   s13
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         14
#elif SAMPLER_TAIL == 14
#define SAMPLER_SAMPLER2D1   s14
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         15
#elif SAMPLER_TAIL == 15
#define SAMPLER_SAMPLER2D1   s15
#endif
#endif

#if ( NND_OPT_TEX_SAMPLER2D2 != -1 )
#if   SAMPLER_TAIL == 0
#define SAMPLER_SAMPLER2D2   s0
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         1
#elif SAMPLER_TAIL == 1
#define SAMPLER_SAMPLER2D2   s1
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         2
#elif SAMPLER_TAIL == 2
#define SAMPLER_SAMPLER2D2   s2
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         3
#elif SAMPLER_TAIL == 3
#define SAMPLER_SAMPLER2D2   s3
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         4
#elif SAMPLER_TAIL == 4
#define SAMPLER_SAMPLER2D2   s4
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         5
#elif SAMPLER_TAIL == 5
#define SAMPLER_SAMPLER2D2   s5
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         6
#elif SAMPLER_TAIL == 6
#define SAMPLER_SAMPLER2D2   s6
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         7
#elif SAMPLER_TAIL == 7
#define SAMPLER_SAMPLER2D2   s7
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         8
#elif SAMPLER_TAIL == 8
#define SAMPLER_SAMPLER2D2   s8
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         9
#elif SAMPLER_TAIL == 9
#define SAMPLER_SAMPLER2D2   s9
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         10
#elif SAMPLER_TAIL == 10
#define SAMPLER_SAMPLER2D2   s10
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         11
#elif SAMPLER_TAIL == 11
#define SAMPLER_SAMPLER2D2   s11
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         12
#elif SAMPLER_TAIL == 12
#define SAMPLER_SAMPLER2D2   s12
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         13
#elif SAMPLER_TAIL == 13
#define SAMPLER_SAMPLER2D2   s13
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         14
#elif SAMPLER_TAIL == 14
#define SAMPLER_SAMPLER2D2   s14
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         15
#elif SAMPLER_TAIL == 15
#define SAMPLER_SAMPLER2D2   s15
#endif
#endif

#if ( NND_OPT_TEX_SAMPLER3D1 != -1 )
#if   SAMPLER_TAIL == 0
#define SAMPLER_SAMPLER3D1   s0
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         1
#elif SAMPLER_TAIL == 1
#define SAMPLER_SAMPLER3D1   s1
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         2
#elif SAMPLER_TAIL == 2
#define SAMPLER_SAMPLER3D1   s2
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         3
#elif SAMPLER_TAIL == 3
#define SAMPLER_SAMPLER3D1   s3
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         4
#elif SAMPLER_TAIL == 4
#define SAMPLER_SAMPLER3D1   s4
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         5
#elif SAMPLER_TAIL == 5
#define SAMPLER_SAMPLER3D1   s5
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         6
#elif SAMPLER_TAIL == 6
#define SAMPLER_SAMPLER3D1   s6
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         7
#elif SAMPLER_TAIL == 7
#define SAMPLER_SAMPLER3D1   s7
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         8
#elif SAMPLER_TAIL == 8
#define SAMPLER_SAMPLER3D1   s8
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         9
#elif SAMPLER_TAIL == 9
#define SAMPLER_SAMPLER3D1   s9
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         10
#elif SAMPLER_TAIL == 10
#define SAMPLER_SAMPLER3D1   s10
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         11
#elif SAMPLER_TAIL == 11
#define SAMPLER_SAMPLER3D1   s11
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         12
#elif SAMPLER_TAIL == 12
#define SAMPLER_SAMPLER3D1   s12
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         13
#elif SAMPLER_TAIL == 13
#define SAMPLER_SAMPLER3D1   s13
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         14
#elif SAMPLER_TAIL == 14
#define SAMPLER_SAMPLER3D1   s14
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         15
#elif SAMPLER_TAIL == 15
#define SAMPLER_SAMPLER3D1   s15
#endif
#endif

#if ( NND_OPT_TEX_SAMPLER3D2 != -1 )
#if   SAMPLER_TAIL == 0
#define SAMPLER_SAMPLER3D2   s0
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         1
#elif SAMPLER_TAIL == 1
#define SAMPLER_SAMPLER3D2   s1
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         2
#elif SAMPLER_TAIL == 2
#define SAMPLER_SAMPLER3D2   s2
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         3
#elif SAMPLER_TAIL == 3
#define SAMPLER_SAMPLER3D2   s3
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         4
#elif SAMPLER_TAIL == 4
#define SAMPLER_SAMPLER3D2   s4
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         5
#elif SAMPLER_TAIL == 5
#define SAMPLER_SAMPLER3D2   s5
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         6
#elif SAMPLER_TAIL == 6
#define SAMPLER_SAMPLER3D2   s6
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         7
#elif SAMPLER_TAIL == 7
#define SAMPLER_SAMPLER3D2   s7
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         8
#elif SAMPLER_TAIL == 8
#define SAMPLER_SAMPLER3D2   s8
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         9
#elif SAMPLER_TAIL == 9
#define SAMPLER_SAMPLER3D2   s9
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         10
#elif SAMPLER_TAIL == 10
#define SAMPLER_SAMPLER3D2   s10
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         11
#elif SAMPLER_TAIL == 11
#define SAMPLER_SAMPLER3D2   s11
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         12
#elif SAMPLER_TAIL == 12
#define SAMPLER_SAMPLER3D2   s12
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         13
#elif SAMPLER_TAIL == 13
#define SAMPLER_SAMPLER3D2   s13
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         14
#elif SAMPLER_TAIL == 14
#define SAMPLER_SAMPLER3D2   s14
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         15
#elif SAMPLER_TAIL == 15
#define SAMPLER_SAMPLER3D2   s15
#endif
#endif

#if ( NND_OPT_TEX_SAMPLERCUBE1 != -1 )
#if   SAMPLER_TAIL == 0
#define SAMPLER_SAMPLERCUBE1 s0
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         1
#elif SAMPLER_TAIL == 1
#define SAMPLER_SAMPLERCUBE1 s1
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         2
#elif SAMPLER_TAIL == 2
#define SAMPLER_SAMPLERCUBE1 s2
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         3
#elif SAMPLER_TAIL == 3
#define SAMPLER_SAMPLERCUBE1 s3
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         4
#elif SAMPLER_TAIL == 4
#define SAMPLER_SAMPLERCUBE1 s4
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         5
#elif SAMPLER_TAIL == 5
#define SAMPLER_SAMPLERCUBE1 s5
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         6
#elif SAMPLER_TAIL == 6
#define SAMPLER_SAMPLERCUBE1 s6
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         7
#elif SAMPLER_TAIL == 7
#define SAMPLER_SAMPLERCUBE1 s7
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         8
#elif SAMPLER_TAIL == 8
#define SAMPLER_SAMPLERCUBE1 s8
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         9
#elif SAMPLER_TAIL == 9
#define SAMPLER_SAMPLERCUBE1 s9
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         10
#elif SAMPLER_TAIL == 10
#define SAMPLER_SAMPLERCUBE1 s10
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         11
#elif SAMPLER_TAIL == 11
#define SAMPLER_SAMPLERCUBE1 s11
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         12
#elif SAMPLER_TAIL == 12
#define SAMPLER_SAMPLERCUBE1 s12
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         13
#elif SAMPLER_TAIL == 13
#define SAMPLER_SAMPLERCUBE1 s13
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         14
#elif SAMPLER_TAIL == 14
#define SAMPLER_SAMPLERCUBE1 s14
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         15
#elif SAMPLER_TAIL == 15
#define SAMPLER_SAMPLERCUBE1 s15
#endif
#endif

#if ( NND_OPT_TEX_SAMPLERCUBE2 != -1 )
#if   SAMPLER_TAIL == 0
#define SAMPLER_SAMPLERCUBE2 s0
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         1
#elif SAMPLER_TAIL == 1
#define SAMPLER_SAMPLERCUBE2 s1
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         2
#elif SAMPLER_TAIL == 2
#define SAMPLER_SAMPLERCUBE2 s2
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         3
#elif SAMPLER_TAIL == 3
#define SAMPLER_SAMPLERCUBE2 s3
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         4
#elif SAMPLER_TAIL == 4
#define SAMPLER_SAMPLERCUBE2 s4
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         5
#elif SAMPLER_TAIL == 5
#define SAMPLER_SAMPLERCUBE2 s5
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         6
#elif SAMPLER_TAIL == 6
#define SAMPLER_SAMPLERCUBE2 s6
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         7
#elif SAMPLER_TAIL == 7
#define SAMPLER_SAMPLERCUBE2 s7
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         8
#elif SAMPLER_TAIL == 8
#define SAMPLER_SAMPLERCUBE2 s8
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         9
#elif SAMPLER_TAIL == 9
#define SAMPLER_SAMPLERCUBE2 s9
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         10
#elif SAMPLER_TAIL == 10
#define SAMPLER_SAMPLERCUBE2 s10
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         11
#elif SAMPLER_TAIL == 11
#define SAMPLER_SAMPLERCUBE2 s11
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         12
#elif SAMPLER_TAIL == 12
#define SAMPLER_SAMPLERCUBE2 s12
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         13
#elif SAMPLER_TAIL == 13
#define SAMPLER_SAMPLERCUBE2 s13
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         14
#elif SAMPLER_TAIL == 14
#define SAMPLER_SAMPLERCUBE2 s14
#undef  SAMPLER_TAIL
#define SAMPLER_TAIL         15
#elif SAMPLER_TAIL == 15
#define SAMPLER_SAMPLERCUBE2 s15
#endif
#endif

#endif  // ifndef __SAMPLER_DEF_H__
