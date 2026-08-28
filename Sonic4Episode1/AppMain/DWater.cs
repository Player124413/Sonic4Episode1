using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using mpp;

internal partial class AppMain
{
    private static readonly VertexPositionColorTexture[] _dwater_verts = new VertexPositionColorTexture[4];

    private static int dwaterInit()
    {
        if (_dmap_water == null)
            _dmap_water = new DMAP_WATER();
        _dmap_water.repeat_u = 1f;
        _dmap_water.repeat_v = 1f;
        _dmap_water.speed_u = 1f / 1000f;
        _dmap_water.speed_v = -1f / 1000f;
        _dmap_water.speed_surface = 1f / 1000f;
        _dmap_water.regist_index = -1;
        _dmap_water.repeat_pos_x = 280f;
        dwaterSetColor(0.65f, 1f, 0.75f);
        _dmap_water.regist_index = amShaderBuild("water", _dmap_water.shader);
        return _dmap_water.regist_index;
    }

    private static void dwaterExit()
    {
        if (_dmap_water != null)
        {
            for (int index = 0; index < 2; ++index)
            {
                if (_dmap_water._object[index]._object != null)
                    _dmap_water._object[index]._object = null;
                if (_dmap_water._object[index].texlistbuf != null)
                    _dmap_water._object[index].texlistbuf = null;
                if (_dmap_water._object[index].motion != null)
                    amMotionDelete(_dmap_water._object[index].motion);
            }
        }
        _dmap_water = null;
    }

    private static void dwaterSetObjectAMB(
      AMS_AMB_HEADER amb_obj,
      AMS_AMB_HEADER amb_tex)
    {
        _dmap_water.amb_object = amb_obj;
        _dmap_water.amb_texture = amb_tex;
    }

    private static int dwaterLoadObject(uint objflag)
    {
        int num1 = 1;
        ArrayPointer<DMAP_WATER_OBJ> arrayPointer = new ArrayPointer<DMAP_WATER_OBJ>(_dmap_water._object);
        if (_dmap_water.regist_index != -1)
        {
            if (!amDrawIsRegistComplete(_dmap_water.regist_index))
                return 0;
            _dmap_water.regist_index = -1;
            if ((~arrayPointer)._object != null)
            {
                int num2 = 0;
                while (num2 < 2)
                {
                    object file = _dmap_water.amb_object.buf[2 + num2];
                    (~arrayPointer).motion = amMotionCreate((~arrayPointer)._object);
                    amMotionMaterialRegistFile((~arrayPointer).motion, 0, file);
                    amMotionMaterialSet((~arrayPointer).motion, 0);
                    amMotionMaterialSetFrame((~arrayPointer).motion, amMotionMaterialGetStartFrame((~arrayPointer).motion, 0));
                    ++num2;
                    ++arrayPointer;
                }
                return 1;
            }
        }
        int index = 0;
        while (index < 2)
        {
            object buf = amBindGet(_dmap_water.amb_object, index);
            _dmap_water.regist_index = amObjectLoad(out (~arrayPointer)._object, out (~arrayPointer).texlist, out (~arrayPointer).texlistbuf, buf, objflag, null, _dmap_water.amb_texture);
            num1 = 0;
            ++index;
            ++arrayPointer;
        }
        return num1;
    }

    // The C++ reaches these by index 4 and 5 (gmWaterSurface.cpp:482), which only holds for the
    // 11-entry PC/360 archive. Ours has 8 and, as shipped, has neither texture in it at all.
    private static int dwaterLoadTexture(AMS_AMB_HEADER amb)
    {
        _dmap_water.tex_water = dwaterLoadNamedTexture(amb, "WATER_A", AMD_WRAP_REPEAT);
        _dmap_water.tex_color = dwaterLoadNamedTexture(amb, "WATERCOLOR", AMD_WRAP_CLAMP);
        return _dmap_water.tex_water != null && _dmap_water.tex_color != null ? 1 : 0;
    }

    // The shaded pass needs its two DDS textures and a compiled effect. WP7 has no custom
    // shaders at all, and any build whose water archive predates them lacks the textures, so
    // both fall back to the flat surface quad.
    private static bool dwaterCanDrawShaded()
    {
        return amShaderIsValid(_dmap_water.shader)
            && _dmap_water.tex_water != null && _dmap_water.tex_water.texture != null
            && _dmap_water.tex_color != null && _dmap_water.tex_color.texture != null;
    }

    private static AMS_PARAM_LOAD_TEXTURE_IMAGE dwaterLoadNamedTexture(
      AMS_AMB_HEADER amb,
      string basename,
      int wrap)
    {
        int index = amBindSearchBaseName(amb, basename);
        if (index < 0)
        {
            amSystemLog($"[WARN] water texture '{basename}' not in {amb.file_num}-entry archive\n");
            return null;
        }

        AMS_PARAM_LOAD_TEXTURE_IMAGE dst = new AMS_PARAM_LOAD_TEXTURE_IMAGE();
        amTextureLoad(dst, amBindGet(amb, index, out string _),
            System.IO.Path.GetExtension(amb.files[index]),
            AMD_FILTER_LINEAR, AMD_FILTER_LINEAR, wrap, wrap);
        return dst;
    }

    private static int dwaterRelease()
    {
        int num = -1;
        if (_dmap_water != null)
        {
            for (int index = 0; index < 2; ++index)
            {
                if (_dmap_water._object[index]._object != null)
                    num = amObjectRelease(_dmap_water._object[index]._object, _dmap_water._object[index].texlist);
            }
        }
        return num;
    }

    // ARGB, matching the _PC | _XBOX | _WII packing at gmWaterSurface.cpp:2038
    private static void dwaterSetColor(float r, float g, float b)
    {
        uint cr = (uint)(r * 255f) & byte.MaxValue;
        uint cg = (uint)(g * 255f) & byte.MaxValue;
        uint cb = (uint)(b * 255f) & byte.MaxValue;
        _dmap_water.color = (cr << 16) | (cg << 8) | cb | 0xFF000000U;
    }

    private static void dwaterUpdate(
      float speed,
      float pos_x,
      float pos_y,
      float dy,
      int rot_z,
      float scale)
    {
        float num1 = 0.006666667f;
        _dmap_water.speed_surface += 0.0005f;
        if (_dmap_water.speed_surface > 1.0)
            --_dmap_water.speed_surface;
        _dmap_water.pos_x = pos_x;
        _dmap_water.pos_y = pos_y;
        _dmap_water.pos_dy = dy;
        _dmap_water.rot_z = rot_z;
        _dmap_water.scale = scale;
        float f1 = _dmap_water.ofst_u + _dmap_water.speed_u * speed;
        float num2 = f1 < 0.0 ? (float)(1.0 - (-f1 - (double)floorf(-f1))) : f1 - floorf(f1);
        _dmap_water.ofst_u = num2;
        float f2 = _dmap_water.ofst_v + _dmap_water.speed_v * speed;
        float num3 = f2 < 0.0 ? (float)(1.0 - (-f2 - (double)floorf(-f2))) : f2 - floorf(f2);
        _dmap_water.ofst_v = num3;
        float num4 = (float)(num1 * (double)_dmap_water.repeat_u / 1.14999997615814);
        float f3 = _dmap_water.ofst_u + pos_x * num4;
        float num5 = f3 < 0.0 ? (float)(1.0 - (-f3 - (double)floorf(-f3))) : f3 - floorf(f3);
        _dmap_water.draw_u = num5;
        _dmap_water.draw_v = _dmap_water.ofst_v;
        ArrayPointer<DMAP_WATER_OBJ> arrayPointer = new ArrayPointer<DMAP_WATER_OBJ>(_dmap_water._object);
        int num6 = 0;
        while (num6 < 2)
        {
            AMS_MOTION motion = (~arrayPointer).motion;
            float num7 = (~arrayPointer).frame + speed;
            float startFrame = amMotionMaterialGetStartFrame(motion, 0);
            float endFrame = amMotionMaterialGetEndFrame(motion, 0);
            while (num7 >= (double)endFrame)
                num7 = startFrame + (num7 - endFrame);
            (~arrayPointer).frame = num7;
            ++num6;
            ++arrayPointer;
        }
    }

    private static void dwaterGetParam(DMAP_PARAM_WATER param)
    {
        param.frame[0] = _dmap_water._object[0].frame;
        param.frame[1] = _dmap_water._object[1].frame;
        param.draw_u = _dmap_water.draw_u;
        param.draw_v = _dmap_water.draw_v;
        param.scale = _dmap_water.scale;
        param.pos_x = _dmap_water.pos_x;
        param.pos_y = _dmap_water.pos_y;
        param.pos_dy = _dmap_water.pos_dy;
        param.repeat_u = _dmap_water.repeat_u;
        param.repeat_v = _dmap_water.repeat_v;
        param.rot_z = _dmap_water.rot_z;
        param.color = _dmap_water.color;
    }

    private static void dwaterSetParam()
    {
        DMAP_PARAM_WATER dmapParamWater = amDrawAlloc_DMAP_PARAM_WATER();
        dwaterGetParam(dmapParamWater);
        amDrawMakeTask(new TaskProc(_dwaterSetParam), 0, dmapParamWater);
    }

    private static void dwaterDrawReflection(uint state, uint drawflag)
    {
        amMatrixPush();
        NNS_MATRIX current = amMatrixGetCurrent();
        DMAP_WATER_OBJ dmapWaterObj = _dmap_water._object[1];
        AMS_MOTION motion = dmapWaterObj.motion;
        amMotionMaterialSetFrame(motion, _dmap_water._object[1].frame);
        amMotionMaterialCalc(motion);
        float x = floorf(_dmap_water.pos_x / (_dmap_water.repeat_pos_x * _dmap_water.scale) - 0.5f) * (_dmap_water.repeat_pos_x * _dmap_water.scale);
        float y = _dmap_water.pos_y + _dmap_water.pos_dy;
        for (int index = 0; index < 2; ++index)
        {
            nnMakeTranslateMatrix(current, x, y, FX_FX32_TO_F32(-1179648));
            nnScaleMatrix(current, current, _dmap_water.scale, _dmap_water.scale, 1f);
            ObjDraw3DNNMotionMaterialMotion(motion, dmapWaterObj.texlist, drawflag, 0U, null, null, state, null, null, null, null, null, 1U);
            x += _dmap_water.repeat_pos_x * _dmap_water.scale;
        }
        amMatrixPop();
    }

    private static void dwaterDrawSurface(uint state, uint drawflag)
    {
        amMatrixPush();
        NNS_MATRIX current = amMatrixGetCurrent();
        DMAP_WATER_OBJ dmapWaterObj = _dmap_water._object[0];
        AMS_MOTION motion = dmapWaterObj.motion;
        amMotionMaterialSetFrame(motion, _dmap_water._object[0].frame);
        amMotionMaterialCalc(motion);
        float x = floorf(_dmap_water.pos_x / (_dmap_water.repeat_pos_x * _dmap_water.scale) - 0.5f) * (_dmap_water.repeat_pos_x * _dmap_water.scale);
        float y = _dmap_water.pos_y + _dmap_water.pos_dy;
        for (int index = 0; index < 2; ++index)
        {
            nnMakeTranslateMatrix(current, x, y, FX_FX32_TO_F32(917504));
            nnScaleMatrix(current, current, _dmap_water.scale, _dmap_water.scale * 2f, 1f);
            ObjDraw3DNNMotionMaterialMotion(motion, dmapWaterObj.texlist, drawflag, 0U, null, null, state, null, null, null, null, null, 1U);
            x += _dmap_water.repeat_pos_x * _dmap_water.scale;
        }
        amMatrixPop();
    }

    // gmWaterSurface.cpp:2355, the _PC | _XBOX branch
    private static void dwaterDrawWater(AMS_RENDER_TARGET texture)
    {
        DMAP_PARAM_WATER param = _dmap_water.draw_param;
        if (param == null || texture == null || texture.target == null)
            return;
        if (!amShaderIsValid(_dmap_water.shader))
            return;
        if (_dmap_water.tex_water == null || _dmap_water.tex_water.texture == null)
            return;
        if (_dmap_water.tex_color == null || _dmap_water.tex_color.texture == null)
            return;

        amDrawPushState();

        NNS_VECTOR pos = new NNS_VECTOR(0.0f, param.pos_dy, -0.5f);
        nnTransformVector(pos, amDrawGetProjectionMatrix(), pos);

        float ofst_u = param.draw_u;
        float ofst_v = param.draw_v;
        float water_u = param.repeat_u * 0.5f * param.scale;
        float water_v = param.repeat_v * param.scale;
        // The C++ assumes pos.y lands in [-1,1], but this port's ortho cameras yield half-range
        // NDC (+/-0.5 across the visible height), so everything downstream is 2x short.
        // Measured: ndc.y 0.363 -> surface at ~148px, -0.505 -> ~1086px on a 1080 backbuffer.
        float dy = pos.y * 2.0f;
        float water_ou = 0.0f;

        float w = AMD_SCREEN_WIDTH;
        float h = AMD_SCREEN_HEIGHT;
        float dv = dy * -0.5f + 0.5f;
        float ox = w * 0.5f;
        float oy = h * 0.5f;

        NNS_MATRIX mtx = GlobalPool<NNS_MATRIX>.Alloc();
        nnMakeRotateZMatrix(mtx, param.rot_z);

        // top pair carry alpha 0, bottom pair alpha 255; the vertex shader reads that as the
        // ramp coordinate, the way the PS3 build does (gmWaterSurface.cpp:372-380)
        Color top = dwaterUnpackColor(param.color, 0);
        Color bottom = dwaterUnpackColor(param.color, byte.MaxValue);

        NNS_VECTOR src = GlobalPool<NNS_VECTOR>.Alloc();
        NNS_VECTOR dst = GlobalPool<NNS_VECTOR>.Alloc();
        float baseX = -1.15f * 0.5f * w;

        src.x = baseX;
        src.y = dy * 0.5f * h;
        src.z = 0.0f;
        nnTransformVector(dst, mtx, src);
        _dwater_verts[0] = new VertexPositionColorTexture(
            new Vector3(ox + dst.x, oy - dst.y, 0.0f), top,
            new Vector2(water_ou - water_u + ofst_u, dv + ofst_v));

        src.x = -src.x;
        nnTransformVector(dst, mtx, src);
        _dwater_verts[1] = new VertexPositionColorTexture(
            new Vector3(ox + dst.x, oy - dst.y, 0.0f), top,
            new Vector2(water_ou + water_u + ofst_u, dv + ofst_v));

        src.x = baseX;
        src.y += src.x * 2.0f;
        nnTransformVector(dst, mtx, src);
        _dwater_verts[2] = new VertexPositionColorTexture(
            new Vector3(ox + dst.x, oy - dst.y, 0.0f), bottom,
            new Vector2(water_ou - water_u + ofst_u, water_v + dv + ofst_v));

        src.x = -src.x;
        nnTransformVector(dst, mtx, src);
        _dwater_verts[3] = new VertexPositionColorTexture(
            new Vector3(ox + dst.x, oy - dst.y, 0.0f), bottom,
            new Vector2(water_ou + water_u + ofst_u, water_v + dv + ofst_v));

        Effect effect = _dmap_water.shader.effect;
        effect.Parameters["MatrixTransform"].SetValue(Matrix.CreateOrthographicOffCenter(0f, w, h, 0f, 0f, 1f));
        effect.Parameters["TexColor"].SetValue(texture.target);
        effect.Parameters["TexOffset"].SetValue(_dmap_water.tex_water.texture);
        effect.Parameters["TexGray"].SetValue(_dmap_water.tex_color.texture);
        OpenGL.mppglDrawUserQuad(effect, _dwater_verts);

        GlobalPool<NNS_VECTOR>.Release(dst);
        GlobalPool<NNS_VECTOR>.Release(src);
        GlobalPool<NNS_MATRIX>.Release(mtx);

        amDrawPopState();
    }

    private static Color dwaterUnpackColor(uint argb, byte alpha)
    {
        return new Color((byte)(argb >> 16), (byte)(argb >> 8), (byte)argb, alpha);
    }

    private static void dwaterReleaseTexture()
    {
        if (_dmap_water == null)
            return;
        if (_dmap_water.tex_water != null && _dmap_water.tex_water.texture != null)
        {
            _dmap_water.tex_water.texture.Dispose();
            _dmap_water.tex_water = null;
        }
        if (_dmap_water.tex_color != null && _dmap_water.tex_color.texture != null)
        {
            _dmap_water.tex_color.texture.Dispose();
            _dmap_water.tex_color = null;
        }
    }

    private static void _dwaterSetParam(AMS_TCB tcbp)
    {
        DMAP_PARAM_WATER work = (DMAP_PARAM_WATER)amTaskGetWork(tcbp);
        _dmap_water.draw_param = work;
    }

}