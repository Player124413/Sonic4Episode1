using System;
using System.Buffers.Binary;
using System.Diagnostics;
using System.IO;

internal partial class AppMain
{

    public static uint GMD_MAP_DRAW_WIDTH => gm_map_draw_size[0];

    public static uint GMD_MAP_DRAW_HEIGHT => gm_map_draw_size[1];

    public static bool GMM_MAP_IS_RANGE(int _src, int _min, int _max)
    {
        return _min < _src && _src < _max;
    }

    public static bool GMM_MAP_IS_RANGE(float _src, float _min, float _max)
    {
        return _min < (double)_src && _src < (double)_max;
    }

    private static void GmMapBuildDataInit()
    {
        if (GMM_MAIN_GET_ZONE_TYPE() == 5)
            return;

        if (gm_map_variant.blocks_as_model)
        {
            gm_map_obj3d = new OBS_ACTION3D_NN_WORK[GMD_MAP_OBJECT_NUM];
            for (int index = 0; index < GMD_MAP_OBJECT_NUM; ++index)
                gm_map_obj3d[index] = new OBS_ACTION3D_NN_WORK();
        }

        gm_map_reg_obj3d_num = GMD_MAP_OBJECT_START;

        if (gm_map_variant.blocks_as_model)
        {
            gm_map_use_fall_shader = false;
        }
        else
        {
            gmMapBuildDrawMapTvxTexScroll();
            gm_map_tex_load_init = true;
        }
    }

    private static bool GmMapBuildDataLoop()
    {
        return gm_map_variant.blocks_as_model
            ? gmMapBuildDataLoopModel()
            : gmMapBuildDataLoopTvx();
    }

    private static bool gmMapBuildDataLoopTvx()
    {
        if (GMM_MAIN_GET_ZONE_TYPE() == GSD_MAIN_ZONE_TYPE_SS)
            return true;
        if (_am_displaylist_manager.regist_num >= gm_map_variant.registlist_num - 64)
            return false;
        if (gm_map_tex_load_init)
        {
            AoTexBuild(gm_map_texture, (AMS_AMB_HEADER)g_gm_gamedat_map[GMD_GAMEDAT_MAP_TEX]);
            AoTexLoad(gm_map_texture);
            gm_map_tex_load_init = false;
        }
        if (!AoTexIsLoaded(gm_map_texture))
            return false;
        gm_map_tex_draw_count = 1;
        return true;
    }

    // registration is resumable and budgeted: 259 models cannot register in one frame, and the
    // console list is half the size of the mobile one
    private static bool gmMapBuildDataLoopModel()
    {
        uint use_light_flag_and = uint.MaxValue;
        uint use_light_flag_or = 0U;
        uint use_ex_light_flag_and = uint.MaxValue;
        uint use_ex_light_flag_or = 0U;

        if (GMM_MAIN_GET_ZONE_TYPE() == GSD_MAIN_ZONE_TYPE_1
            || GMM_MAIN_GET_ZONE_TYPE() == GSD_MAIN_ZONE_TYPE_4)
        {
            use_light_flag_and = ~OBD_LIGHT_USE_FLAG_0;
            use_light_flag_or = OBD_LIGHT_USE_FLAG_5;
            use_ex_light_flag_and = ~OBD_LIGHT_USE_FLAG_0;
            use_ex_light_flag_or = OBD_LIGHT_USE_FLAG_5;
        }

        AMS_AMB_HEADER model_amb = (AMS_AMB_HEADER)g_gm_gamedat_map[GMD_GAMEDAT_MAP_MODEL];
        AMS_AMB_HEADER tex_amb = (AMS_AMB_HEADER)g_gm_gamedat_map[GMD_GAMEDAT_MAP_TEX];

        if (gm_map_reg_obj3d_num < model_amb.file_num)
        {
            if (GsMainSysGetDisplayListRegistNum() >= gm_map_variant.registlist_num - 64)
                return false;

            for (int index = gm_map_reg_obj3d_num; index < model_amb.file_num; ++index)
            {
                OBS_ACTION3D_NN_WORK obj_3d = gm_map_obj3d[index];
                uint user0 = (uint)model_amb.user0[index];

                ObjAction3dNNModelLoad(obj_3d, null, null, index, model_amb, null, tex_amb, 0U);

                // MATCTRL bits are the high half of NNF_DRAWOBJ, which this port keeps separately
                if ((user0 & GMD_MAP_MODEL_USER_FLAG0_WATER) != 0U)
                {
                    obj_3d.drawflag_ex |= NND_DRAWOBJ_MATCTRL_USERSAMPLER2D1;
                    gm_map_use_fall_shader = true;
                }

                if ((user0 & GMD_MAP_MODEL_USER_FLAG0_EX_LIGHT) != 0U)
                {
                    obj_3d.use_light_flag &= use_ex_light_flag_and;
                    obj_3d.use_light_flag |= use_ex_light_flag_or;
                }
                else
                {
                    obj_3d.use_light_flag &= use_light_flag_and;
                    obj_3d.use_light_flag |= use_light_flag_or;
                }

                obj_3d.drawflag |= NND_DRAWOBJ_DOUBLESIDE;

                ++gm_map_reg_obj3d_num;

                if (GsMainSysGetDisplayListRegistNum() >= gm_map_variant.registlist_num - 64)
                    break;
            }
        }

        if (gm_map_reg_obj3d_num < model_amb.file_num)
            return false;

        AMS_AMB_HEADER mtn_amb = (AMS_AMB_HEADER)g_gm_gamedat_map[GMD_GAMEDAT_MAP_MTN];
        AMS_AMB_HEADER mmtn_amb = (AMS_AMB_HEADER)g_gm_gamedat_map[GMD_GAMEDAT_MAP_MMTN];

        for (int index = GMD_MAP_OBJECT_START; index < gm_map_reg_obj3d_num; ++index)
        {
            OBS_ACTION3D_NN_WORK obj_3d = gm_map_obj3d[index];
            if (!ObjAction3dNNModelLoadCheck(obj_3d))
                return false;

            if (obj_3d.mtn[0] != null || obj_3d.mat_mtn[0] != null)
                continue;

            if (mtn_amb != null && index < mtn_amb.file_num
                && amBindGet(mtn_amb, index, out string _) != null)
            {
                ObjAction3dNNMotionLoad(obj_3d, 0, false, null, null, index, mtn_amb, 1, 1);
                ObjDrawAction3dActionSet3DNN(obj_3d, 0, 0);
            }

            if (mmtn_amb != null && index < mmtn_amb.file_num
                && amBindGet(mmtn_amb, index, out string _) != null)
            {
                ObjAction3dNNMaterialMotionLoad(obj_3d, 0, null, null, index, mmtn_amb, 1, 1);
                ObjDrawAction3dActionSet3DNNMaterial(obj_3d, 0);
            }
        }

        if (gm_map_use_fall_shader)
        {
            if (GMM_MAIN_GET_ZONE_TYPE() == GSD_MAIN_ZONE_TYPE_2)
                gm_map_fall_rander_command_state = OBD_DRAW_CMD_STATE_WATER_BACK;
            else if (GMM_MAIN_GET_ZONE_TYPE() == GSD_MAIN_ZONE_TYPE_FINAL)
                gm_map_fall_rander_command_state = OBD_DRAW_CMD_STATE_WATER_MAPMID;
            else
                gm_map_use_fall_shader = false;
        }

        return true;
    }

    private static ushort gmMapReadU16(byte[] data, int offset, bool big_endian)
    {
        return big_endian
            ? BinaryPrimitives.ReadUInt16BigEndian(data.AsSpan(offset))
            : BinaryPrimitives.ReadUInt16LittleEndian(data.AsSpan(offset));
    }

    private static uint gmMapReadU32(byte[] data, int offset, bool big_endian)
    {
        return big_endian
            ? BinaryPrimitives.ReadUInt32BigEndian(data.AsSpan(offset))
            : BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(offset));
    }

    // .mp and .md open with map_w and map_h, and the cells that follow have to fill the member
    // exactly. read the wrong way round 280x32 becomes 6145x8192, which does not. the container is
    // only consulted when both readings fit or neither does, same as amAmaIsBigEndian
    private static bool gmMapGridIsBigEndian(AmbChunk data, int cell_size)
    {
        if (data.length < 4)
            return data.amb.big_endian;

        var little = gmMapGridFits(data, cell_size, big_endian: false);
        var big = gmMapGridFits(data, cell_size, big_endian: true);

        return little == big ? data.amb.big_endian : big;
    }

    private static bool gmMapGridFits(AmbChunk data, int cell_size, bool big_endian)
    {
        long w = gmMapReadU16(data.array, data.offset, big_endian);
        long h = gmMapReadU16(data.array, data.offset + 2, big_endian);

        return 4 + w * h * cell_size == data.length;
    }

    // .ev, .rg and .dc are one format with three payloads: a header in 256x256 cells rather than the
    // map's 64x64 blocks, then a u32 offset per cell, then {count, info[count]} wherever the offset
    // is non-zero. map_file_format.h. Measured: the walk lands exactly on the end of the member in
    // every build, so it is the same length identity the grids have.
    private static bool gmMapCellsFit(AmbChunk data, int record_size, bool big_endian)
    {
        if (data.length < 4)
            return false;

        int map_w = gmMapReadU16(data.array, data.offset, big_endian);
        int map_h = gmMapReadU16(data.array, data.offset + 2, big_endian);
        long table = 4 + (long)map_w * map_h * 4;

        if (map_w == 0 || map_h == 0 || table > data.length)
            return false;

        long span = table;
        for (int i = 0; i < map_w * map_h; i++)
        {
            long ofst = gmMapReadU32(data.array, data.offset + 4 + i * 4, big_endian);
            if (ofst == 0)
                continue;
            if (ofst + 2 > data.length)
                return false;

            long num = gmMapReadU16(data.array, data.offset + (int)ofst, big_endian);
            long end = ofst + 2 + num * record_size;
            if (end > data.length)
                return false;

            span = Math.Max(span, end);
        }

        return span == data.length;
    }

    private static bool gmMapCellsAreBigEndian(AmbChunk data, int record_size)
    {
        var little = gmMapCellsFit(data, record_size, big_endian: false);
        var big = gmMapCellsFit(data, record_size, big_endian: true);

        return little == big ? data.amb.big_endian : big;
    }

    // one cell per 256 world units each way, so a record's x and y are a byte apiece
    private static T[][] gmMapReadCells<T>(
      AmbChunk data,
      int record_size,
      out ushort map_w,
      out ushort map_h,
      Func<BinaryReader, T> read)
    {
        bool bigEndian = gmMapCellsAreBigEndian(data, record_size);

        using (MemoryStream stream = new MemoryStream(data.array, data.offset, data.length))
        using (BinaryReader reader = bigEndian
            ? new EndianBinaryReader(stream, bigEndian: true)
            : new BinaryReader(stream))
        {
            map_w = reader.ReadUInt16();
            map_h = reader.ReadUInt16();

            int count = map_w * map_h;
            uint[] offsets = new uint[count];
            for (int i = 0; i < count; ++i)
                offsets[i] = reader.ReadUInt32();

            T[][] cells = new T[count][];
            for (int i = 0; i < count; ++i)
            {
                if (offsets[i] == 0)
                {
                    cells[i] = [];
                    continue;
                }

                stream.Position = offsets[i];
                int num = reader.ReadUInt16();
                cells[i] = new T[num];
                for (int j = 0; j < num; ++j)
                    cells[i][j] = read(reader);
            }

            return cells;
        }
    }

    // .at, .df and .di open with block_num and are fixed size per block, so the same trick works
    private static bool gmMapBlockIsBigEndian(AmbChunk data, int block_size)
    {
        if (data.length < 4)
            return data.amb.big_endian;

        var little = 4 + (long)gmMapReadU32(data.array, data.offset, false) * block_size == data.length;
        var big = 4 + (long)gmMapReadU32(data.array, data.offset, true) * block_size == data.length;

        return little == big ? data.amb.big_endian : big;
    }

    internal static DF_HEADER readDFFile(AmbChunk data)
    {
        DF_HEADER dfHeader = new DF_HEADER()
        {
            block_num = gmMapReadU32(data.array, data.offset, gmMapBlockIsBigEndian(data, 8 * 8 * 8))
        };
        dfHeader.blocks = new DF_BLOCK[(int)dfHeader.block_num];
        int num = data.offset + 4;
        for (int index1 = 0; index1 < dfHeader.block_num; ++index1)
        {
            DF_BLOCK dfBlock = new DF_BLOCK();
            dfHeader.blocks[index1] = dfBlock;
            for (int index2 = 0; index2 < 8; ++index2)
            {
                for (int index3 = 0; index3 < 8; ++index3)
                {
                    dfBlock.df[index2, index3].Data = data.array;
                    dfBlock.df[index2, index3].Offset = num;
                    num += 8;
                }
            }
        }
        return dfHeader;
    }

    internal static DI_HEADER readDIFile(AmbChunk data)
    {
        DI_HEADER diHeader = new DI_HEADER();
        bool bigEndian = gmMapBlockIsBigEndian(data, 8 * 8);
        using (MemoryStream memoryStream = new MemoryStream(data.array, data.offset, data.length))
        {
            using (BinaryReader binaryReader = bigEndian
                ? new EndianBinaryReader(memoryStream, bigEndian: true)
                : new BinaryReader(memoryStream))
            {
                diHeader.block_num = binaryReader.ReadUInt32();
                diHeader.blocks = new DI_BLOCK[(int)diHeader.block_num];
                for (int index1 = 0; index1 < diHeader.block_num; ++index1)
                {
                    diHeader.blocks[index1] = new DI_BLOCK();
                    for (int index2 = 0; index2 < 8; ++index2)
                        diHeader.blocks[index1].di[index2] = binaryReader.ReadBytes(8);
                }
            }
        }
        return diHeader;
    }

    internal static AT_HEADER readATFile(AmbChunk data)
    {
        AT_HEADER atHeader = new AT_HEADER();
        bool bigEndian = gmMapBlockIsBigEndian(data, 8 * 8);
        using (MemoryStream memoryStream = new MemoryStream(data.array, data.offset, data.length))
        {
            using (BinaryReader binaryReader = bigEndian
                ? new EndianBinaryReader(memoryStream, bigEndian: true)
                : new BinaryReader(memoryStream))
            {
                atHeader.block_num = binaryReader.ReadUInt32();
                atHeader.blocks = new AT_BLOCK[(int)atHeader.block_num];
                for (int index1 = 0; index1 < atHeader.block_num; ++index1)
                {
                    atHeader.blocks[index1] = new AT_BLOCK();
                    for (int index2 = 0; index2 < 8; ++index2)
                        atHeader.blocks[index1].at[index2] = binaryReader.ReadBytes(8);
                }
            }
        }
        return atHeader;
    }

    internal static MP_HEADER readMPFile(AmbChunk data)
    {
        if (data.length == 0)
            return null;
        MP_HEADER mpHeader = new MP_HEADER();
        bool bigEndian = gmMapGridIsBigEndian(data, 2);
        using (MemoryStream memoryStream = new MemoryStream(data.array, data.offset, data.length))
        {
            using (BinaryReader binaryReader = bigEndian
                ? new EndianBinaryReader(memoryStream, bigEndian: true)
                : new BinaryReader(memoryStream))
            {
                mpHeader.map_w = binaryReader.ReadUInt16();
                mpHeader.map_h = binaryReader.ReadUInt16();
                int length = mpHeader.map_w * mpHeader.map_h;
                mpHeader.blocks = new MP_BLOCK[length];
                for (int index = 0; index < length; ++index)
                    mpHeader.blocks[index] = new MP_BLOCK(binaryReader.ReadUInt16());
            }
        }
        return mpHeader;
    }

    internal static DC_HEADER readDCFile(AmbChunk data)
    {
        DC_HEADER dcHeader = new DC_HEADER();
        dcHeader.cells = gmMapReadCells(data, 4, out dcHeader.map_w, out dcHeader.map_h, reader =>
            new DC_INFO
            {
                x = reader.ReadByte(),
                y = reader.ReadByte(),
                id = reader.ReadUInt16(),
            });
        return dcHeader;
    }

    internal static MD_HEADER readMDFile(AmbChunk data)
    {
        if (data.length == 0)
            return null;
        MD_HEADER mdHeader = new MD_HEADER();
        bool bigEndian = gmMapGridIsBigEndian(data, 1);
        using (MemoryStream memoryStream = new MemoryStream(data.array, data.offset, data.length))
        {
            using (BinaryReader binaryReader = bigEndian
                ? new EndianBinaryReader(memoryStream, bigEndian: true)
                : new BinaryReader(memoryStream))
            {
                mdHeader.map_w = binaryReader.ReadUInt16();
                mdHeader.map_h = binaryReader.ReadUInt16();
                int length = mdHeader.map_w * mdHeader.map_h;
                mdHeader.blocks = new MD_BLOCK[length];
                for (int index = 0; index < length; ++index)
                    mdHeader.blocks[index] = new MD_BLOCK(binaryReader.ReadSByte());
            }
        }
        return mdHeader;
    }

    internal static RG_HEADER readRGFile(AmbChunk data)
    {
        RG_HEADER rgHeader = new RG_HEADER();
        rgHeader.cells = gmMapReadCells(data, 2, out rgHeader.map_w, out rgHeader.map_h, reader =>
            new RG_INFO
            {
                x = reader.ReadByte(),
                y = reader.ReadByte(),
            });
        return rgHeader;
    }

    internal static EV_HEADER readEVFile(AmbChunk data)
    {
        EV_HEADER evHeader = new EV_HEADER();
        evHeader.cells = gmMapReadCells(data, 12, out evHeader.map_w, out evHeader.map_h, reader =>
            new EV_INFO
            {
                x = reader.ReadByte(),
                y = reader.ReadByte(),
                id = reader.ReadUInt16(),
                flag = reader.ReadUInt16(),
                rect_x = reader.ReadSByte(),
                rect_y = reader.ReadSByte(),
                rect_w = reader.ReadByte(),
                rect_h = reader.ReadByte(),
                user = reader.ReadUInt16(),
            });
        return evHeader;
    }

    private static void GmMapBuildColData()
    {
        MP_HEADER gGmGamedatMap1 = (MP_HEADER)g_gm_gamedat_map_set[4];
        MP_HEADER gGmGamedatMap2 = (MP_HEADER)g_gm_gamedat_map_set[5];
        g_gm_main_system.map_fcol.map_block_num_x = gGmGamedatMap1.map_w;
        g_gm_main_system.map_fcol.map_block_num_y = gGmGamedatMap1.map_h;
        g_gm_main_system.map_fcol.block_map_datap[0] = gGmGamedatMap1.blocks;
        g_gm_main_system.map_fcol.block_map_datap[1] = gGmGamedatMap2.blocks;
        DF_HEADER dfHeader = readDFFile((AmbChunk)g_gm_gamedat_map_attr_set[1]);
        DI_HEADER diHeader = readDIFile((AmbChunk)g_gm_gamedat_map_attr_set[2]);
        AT_HEADER atHeader = readATFile((AmbChunk)g_gm_gamedat_map_attr_set[0]);
        if (g_gs_main_sys_info.stage_id == 9)
        {
            for (int index1 = 0; index1 < atHeader.block_num; ++index1)
            {
                for (int index2 = 0; index2 < 8; ++index2)
                {
                    for (int index3 = 0; index3 < 8; ++index3)
                        atHeader.blocks[index1].at[index2][index3] &= 253;
                }
            }
        }
        g_gm_main_system.map_fcol.diff_block_num = dfHeader.block_num;
        g_gm_main_system.map_fcol.dir_block_num = diHeader.block_num;
        g_gm_main_system.map_fcol.attr_block_num = atHeader.block_num;
        g_gm_main_system.map_fcol.cl_diff_datap = dfHeader.blocks;
        g_gm_main_system.map_fcol.direc_datap = diHeader.blocks;
        g_gm_main_system.map_fcol.char_attr_datap = atHeader.blocks;
        g_gm_main_system.map_fcol.left = 0;
        g_gm_main_system.map_fcol.top = 0;
        g_gm_main_system.map_fcol.right = g_gm_main_system.map_fcol.map_block_num_x * 64;
        g_gm_main_system.map_fcol.bottom = g_gm_main_system.map_fcol.map_block_num_y * 64;
        g_gm_main_system.map_size[0] = g_gm_main_system.map_fcol.map_block_num_x * 64;
        g_gm_main_system.map_size[1] = g_gm_main_system.map_fcol.map_block_num_y * 64;
    }

    private static void GmMapFlushData()
    {
        gm_map_release_obj3d_num = GMD_MAP_OBJECT_START;
        if (gm_map_variant.blocks_as_model)
            return;

        if (GMM_MAIN_GET_ZONE_TYPE() == GSD_MAIN_ZONE_TYPE_SS)
            return;
        AoTexRelease(gm_map_texture);
        gmMapFlushDrawMapTvxTexScroll();
    }

    private static bool GmMapFlushDataLoop()
    {
        return gm_map_variant.blocks_as_model
            ? gmMapFlushDataLoopModel()
            : GMM_MAIN_GET_ZONE_TYPE() == GSD_MAIN_ZONE_TYPE_SS || AoTexIsReleased(gm_map_texture);
    }

    private static bool gmMapFlushDataLoopModel()
    {
        if (gm_map_obj3d == null)
            return true;

        if (gm_map_release_obj3d_num < gm_map_reg_obj3d_num)
        {
            if (GsMainSysGetDisplayListRegistNum() >= gm_map_variant.registlist_num - 64)
                return false;

            for (int index = gm_map_release_obj3d_num; index < gm_map_reg_obj3d_num; ++index)
            {
                OBS_ACTION3D_NN_WORK obj_3d = gm_map_obj3d[index];
                if (obj_3d.motion != null)
                    ObjAction3dNNMotionRelease(obj_3d);

                ObjAction3dNNModelRelease(obj_3d);

                ++gm_map_release_obj3d_num;

                if (GsMainSysGetDisplayListRegistNum() >= gm_map_variant.registlist_num - 64)
                    break;
            }
        }

        if (gm_map_release_obj3d_num < gm_map_reg_obj3d_num)
            return false;

        for (int index = GMD_MAP_OBJECT_START; index < gm_map_reg_obj3d_num; ++index)
        {
            if (!ObjAction3dNNModelReleaseCheck(gm_map_obj3d[index]))
                return false;
        }

        gm_map_obj3d = null;
        gm_map_reg_obj3d_num = 0;
        return true;
    }

    private static void GmMapFlushColData()
    {
    }

    private static void GmMapRelease()
    {
        for (int index = 0; index < GMD_GAMEDAT_MAP_MAX; ++index)
            g_gm_gamedat_map[index] = null;
    }

    private static void GmMapInit()
    {
        ObjSetDiffCollision(g_gm_main_system.map_fcol);
        gm_map_z32_share_frame = 0.0f;
        // port-original, no C++ origin: it shifts the TVX map one unit in x and makes
        // ObjDraw3DNNModel scale every NN model by 1.005 to close seams between batched quads.
        // console draws blocks as models and passes trans_x 0, so it has no business on that arm
        gmMapTransX = gm_map_variant.blocks_as_model || g_gs_main_sys_info.stage_id > 2 ? 0.0f : 1f;
        gm_map_tcb = MTM_TASK_MAKE_TCB(new GSF_TASK_PROCEDURE(gmMapMain), new GSF_TASK_PROCEDURE(gmMapDest), 0U, 0, 12288U, 5, () => new GMS_MAP_SYS_WORK(), "GM_MAP_MAIN");
        GMS_MAP_SYS_WORK gmsMapSysWork = new GMS_MAP_SYS_WORK();
        gm_map_tcb.work = gmsMapSysWork;
        gm_map_draw_command_state = 0U;
        gm_map_draw_margin_adjust = 0U;

        // shared rather than TVX only, because both arms of gmMapMain still drive the iPhone
        // deferred-BGM handshake. see the note there
        gm_map_draw_bgm_timer = GMD_MAP_DRAW_BGM_TIMER;

        uint stageId = g_gs_main_sys_info.stage_id;
        if (gm_map_variant.blocks_as_model)
        {
            gm_map_draw_size[0] = GMD_MAP_DRAW_WIDTH_DEF;
            gm_map_draw_size[1] = GMD_MAP_DRAW_HEIGHT_DEF;
        }
        else
            gmMapInitTvx(gmsMapSysWork, stageId);

        GMS_MAP_OTHER_MAP_STATE[] mapState = gmsMapSysWork.map_state;
        for (int index = 0; index < 5; ++index)
        {
            if (g_gm_gamedat_map_set_add[index * 2] != null && g_gm_gamedat_map_set_add[index * 2 + 1] != null)
            {
                gmsMapSysWork.flag |= (uint)(1 << index);
                mapState[index].pos_z = gm_map_addmap_pos_z_tbl[index];

                // The card stage pulls its near layer 500 forward, to 756. With the camera 1000
                // back that leaves it 244 away, just past that stage's own znear of 224, and it is
                // why the layer is worth drawing in perspective at all. At the table's 256 it sits
                // three times too far and comes out three times too small
                if (index == 0 && gmMapNearLayerIsPerspective())
                    mapState[index].pos_z += 500f;

                MP_HEADER mpHeader = (MP_HEADER)g_gm_gamedat_map_set_add[index * 2];
                mapState[index].map_block_num[0] = mpHeader.map_w;
                mapState[index].map_block_num[1] = mpHeader.map_h;
                mapState[index].map_size[0] = mapState[index].map_block_num[0] * 64;
                mapState[index].map_size[1] = mapState[index].map_block_num[1] * 64;
                mapState[index].scrl_scale[0] = (float)((mapState[index].map_size[0] - (double)OBD_LCD_X) / (g_gm_main_system.map_size[0] - (double)OBD_LCD_X));
                mapState[index].scrl_scale[1] = (float)((mapState[index].map_size[1] - (double)OBD_LCD_Y) / (g_gm_main_system.map_size[1] - (double)OBD_LCD_Y));
                mapState[index].command_state = g_gs_main_sys_info.stage_id == 1 || g_gs_main_sys_info.stage_id == 2 || (g_gs_main_sys_info.stage_id == 8 || g_gs_main_sys_info.stage_id == 10) ? gm_map_addmap_command_state_z1_act2_3_z3_act1_3_tbl[index] : (g_gs_main_sys_info.stage_id != 16 ? gm_map_addmap_command_state_tbl[index] : gm_map_addmap_command_state_zf_tbl[index]);
            }
        }
        if (g_gs_main_sys_info.stage_id == 9)
        {
            gmsMapSysWork.map_state[2].pos_z = 160f;
            gmsMapSysWork.map_state[3].pos_z = -96f;
        }
        else
        {
            if (g_gs_main_sys_info.stage_id != 16)
                return;
            gmsMapSysWork.map_state[1].pos_z += -64f;
            gmsMapSysWork.map_state[2].pos_z += -64f;
            gmsMapSysWork.map_state[3].pos_z += -64f;
        }
    }

    // the draw-size and auto-resize rules, the prim matrices, the per-stage tint and the deferred
    // BGM timer are all iPhone only; console keeps a flat 6 by 6 window
    private static void gmMapInitTvx(GMS_MAP_SYS_WORK gmsMapSysWork, uint stageId)
    {
        GmMapSetMapDrawSize(1);
        gmsMapSysWork.auto_resize = true;
        switch (stageId)
        {
            case 0:
            case 1:
            case 2:
            case 4:
            case 6:
                GmMapSetMapDrawSize(0);
                gmsMapSysWork.auto_resize = false;
                break;
            case 3:
            case 7:
            case 11:
            case 15:
            case 16:
                gmsMapSysWork.auto_resize = false;
                break;
        }
        gmMapCreateUsePrimMatrix();
        uint num = 3772834047;
        if (stageId == 0U || stageId == 1U)
            num = uint.MaxValue;
        if (stageId == 2U || stageId == 3U)
            num = 3767175935U;
        else if (stageId == 14U)
            num = 1616929023U;
        gm_map_prim_draw_tvx_color = num;
        gm_map_prim_draw_tvx_alpha_set = null;
        if (stageId == 4U || stageId == 5U || (stageId == 6U || stageId == 7U))
            gm_map_prim_draw_tvx_alpha_set = gm_map_prim_draw_tvx_alpha_set_z2;
    }

    private static void GmMapExit()
    {
        if (gm_map_tcb == null)
            return;
        mtTaskClearTcb(gm_map_tcb);
    }

    private static void GmMapSetDrawState(uint command_state)
    {
        gm_map_draw_command_state = command_state;
    }

    private static void GmMapSetDrawMarginNormal()
    {
        gm_map_draw_margin_adjust = 0U;
    }

    private static void GmMapSetDrawMarginMag()
    {
        gm_map_draw_margin_adjust = 1U;
    }

    private static void GmMapDrawMap(
      OBS_ACTION3D_NN_WORK[] obj3d_tbl,
      MP_HEADER mp_header,
      MD_HEADER md_header,
      float pos_x,
      float pos_y,
      float trans_x,
      float trans_y,
      float trans_z,
      bool loop_h)
    {
        float num1 = 0.0f;
        float num2 = 0.0f;
        int num3 = (int)pos_x >> 6;
        int num4 = (int)pos_y >> 6;
        int num5 = (int)GMD_MAP_DRAW_WIDTH + 2 + (int)gm_map_draw_margin_adjust * 2;
        int num6 = (int)GMD_MAP_DRAW_HEIGHT + 2 + (int)gm_map_draw_margin_adjust * 2;
        int block_left = num3 - (num5 >> 1);
        int block_right = num3 + (num5 >> 1);
        int block_top = num4 - (num6 >> 1);
        int block_bottom = num4 + (num6 >> 1);
        if (block_left < 0)
        {
            if (!loop_h)
            {
                block_left = 0;
            }
            else
            {
                block_left += mp_header.map_w;
                num1 = -(mp_header.map_w << 6);
            }
        }
        else if (block_left >= mp_header.map_w)
        {
            if (!loop_h)
            {
                block_left = mp_header.map_w - num5;
            }
            else
            {
                block_left -= mp_header.map_w;
                num1 = mp_header.map_w << 6;
            }
        }
        if (block_top < 0)
            block_top = 0;
        if (block_right >= mp_header.map_w)
        {
            if (!loop_h)
            {
                block_right = mp_header.map_w - 1;
            }
            else
            {
                block_right -= mp_header.map_w;
                num2 = mp_header.map_w << 6;
            }
        }
        else if (block_right < 0)
        {
            if (!loop_h)
            {
                block_right = num5;
            }
            else
            {
                block_right += mp_header.map_w;
                num2 = -(mp_header.map_w << 6);
            }
        }
        if (block_bottom >= mp_header.map_h)
            block_bottom = mp_header.map_h - 1;
        if (block_left < block_right)
        {
            if (loop_h)
                gmMapDrawMapRange(obj3d_tbl, mp_header, md_header, trans_x + num1, trans_y, trans_z, block_left, block_right, block_top, block_bottom);
            else
                gmMapDrawMapRange(obj3d_tbl, mp_header, md_header, trans_x, trans_y, trans_z, block_left, block_right, block_top, block_bottom);
        }
        else
        {
            gmMapDrawMapRange(obj3d_tbl, mp_header, md_header, trans_x + num1, trans_y, trans_z, block_left, mp_header.map_w - 1, block_top, block_bottom);
            gmMapDrawMapRange(obj3d_tbl, mp_header, md_header, trans_x + num2, trans_y, trans_z, 0, block_right, block_top, block_bottom);
        }
    }

    private static void gmMapDrawMapRange(
      OBS_ACTION3D_NN_WORK[] obj3d_tbl,
      MP_HEADER mp_header,
      MD_HEADER md_header,
      float trans_x,
      float trans_y,
      float trans_z,
      int block_left,
      int block_right,
      int block_top,
      int block_bottom)
    {
        for (int index1 = 0; index1 < GMD_MAP_DRAW_CHECK_WIDTH; ++index1)
        {
            for (int index2 = 0; index2 < GMD_MAP_DRAW_CHECK_HEIGHT; ++index2)
                gm_map_block_check[index1, index2] = -1;
        }

        VecFx32 pos = new VecFx32(0, 0, 0);
        VecU16 dir = new VecU16();
        VecFx32 scale = new VecFx32(FX32_ONE, FX32_ONE, FX32_ONE);

        for (int x_cnt = block_left; x_cnt <= block_right; ++x_cnt)
        {
            for (int y_cnt = block_top; y_cnt <= block_bottom; ++y_cnt)
            {
                int block_no = y_cnt * mp_header.map_w + x_cnt;
                MP_BLOCK mp_block = mp_header.blocks[block_no];
                float dx = x_cnt;
                float dy = y_cnt;
                int obj_id = mp_block.id;

                if (obj_id == 0)
                {
                    MD_BLOCK md_block = md_header.blocks[block_no];
                    int ofst_x = md_block.ofst_x;
                    int ofst_y = md_block.ofst_y;
                    if ((ofst_x | ofst_y) == 0)
                        continue;

                    block_no += mp_header.map_w * ofst_y + ofst_x;
                    mp_block = mp_header.blocks[block_no];
                    obj_id = mp_block.id;
                    dx += ofst_x;
                    dy += ofst_y;
                }

                // feeding mobile .mp data against console models overruns the registration cursor
                if (obj_id == 0 || obj_id >= gm_map_reg_obj3d_num)
                    continue;

                int check_x = GMD_MAP_DRAW_DF_SIZE + (int)dx - block_left;
                int check_y = GMD_MAP_DRAW_DF_SIZE + (int)dy - block_top;
                if (gm_map_block_check[check_x, check_y] != -1)
                    continue;

                gm_map_block_check[check_x, check_y] = (short)obj_id;

                OBS_ACTION3D_NN_WORK obj_3d = obj3d_tbl[obj_id];
                obj_3d.command_state = gm_map_draw_command_state;

                nnMakeUnitMatrix(obj_3d.user_obj_mtx);
                nnTranslateMatrix(
                    obj_3d.user_obj_mtx,
                    obj_3d.user_obj_mtx,
                    trans_x + (dx + 0.5f) * GMD_MAP_BLOCK_SIZE,
                    -trans_y + (-dy - 0.5f) * GMD_MAP_BLOCK_SIZE,
                    trans_z);
                nnRotateZMatrix(obj_3d.user_obj_mtx, obj_3d.user_obj_mtx, mp_block.rot * 0x4000);

                float scale_x = mp_block.flip_h != 0 ? -GMD_OBJ_DRAW_SCALE : GMD_OBJ_DRAW_SCALE;
                float scale_y = mp_block.flip_v != 0 ? -GMD_OBJ_DRAW_SCALE : GMD_OBJ_DRAW_SCALE;
                nnScaleMatrix(obj_3d.user_obj_mtx, obj_3d.user_obj_mtx, scale_x, scale_y, GMD_OBJ_DRAW_SCALE);

                // nnTranslateMatrix post-multiplies, so this runs in the block's pre-scale local
                // space where -10 is -32 world after the 3.2
                NNS_VECTOR node = obj_3d._object.pNodeList[0].Translation;
                nnTranslateMatrix(
                    obj_3d.user_obj_mtx,
                    obj_3d.user_obj_mtx,
                    -node.x - GMD_MAP_BLOCK_LOCAL_SIZE * 0.5f,
                    -node.y - GMD_MAP_BLOCK_LOCAL_SIZE * 0.5f,
                    -node.z);

                uint disp_flag = OBD_DISP_NOPOS | OBD_DISP_NOSCALE | OBD_DISP_NODRAWSCALE
                    | OBD_DISP_NODIR | OBD_DISP_NODIRFLIP | OBD_DISP_USERMTX | OBD_DISP_NOUPDATE;

                if ((obj_3d.drawflag_ex & NND_DRAWOBJ_MATCTRL_USERSAMPLER2D1) != 0U)
                {
                    ObjDraw3DNNUserFunc(
                        gmMapDrawFallShaderPrioPreMidMapUserFunc, null, 0, obj_3d.command_state);
                    gm_map_fall_shader_drawn = true;
                }

                ObjDrawAction3DNN(obj_3d, pos, dir, scale, ref disp_flag);
            }
        }
    }

    private static void GmMapSetAddMapXLoop()
    {
        if (gm_map_tcb == null)
            return;
        GMS_MAP_SYS_WORK work = (GMS_MAP_SYS_WORK)gm_map_tcb.work;
        work.flag |= 2147483648U;
        GMS_MAP_OTHER_MAP_STATE[] mapState = work.map_state;
        for (int index = 0; index < 5; ++index)
        {
            if (((int)work.flag & 1 << index) != 0 && mapState[index].map_size[0] != g_gm_main_system.map_size[0])
                mapState[index].scrl_scale[0] = mapState[index].map_size[0] / (float)g_gm_main_system.map_size[0];
        }
    }

    private static void GmMapEnableAddMapUserScrlX()
    {
        if (gm_map_tcb == null)
            return;
        GMS_MAP_SYS_WORK work = (GMS_MAP_SYS_WORK)gm_map_tcb.work;
        if (((int)work.flag & 536870912) != 0)
            return;
        work.flag |= 536870912U;
        OBS_CAMERA obsCamera = ObjCameraGet(g_obj.glb_camera_id);
        work.main_cam_user_disp[0] = obsCamera.disp_pos.x;
        work.main_cam_user_disp[1] = obsCamera.disp_pos.y;
        work.main_cam_user_target[0] = obsCamera.disp_pos.x;
        work.main_cam_user_target[1] = obsCamera.disp_pos.y;
        work.main_cam_user_ofst[0] = work.main_cam_user_ofst[1] = 0.0f;
    }

    private static void GmMapDisenableAddMapUserScrlX()
    {
        if (gm_map_tcb == null)
            return;
        GMS_MAP_SYS_WORK work = (GMS_MAP_SYS_WORK)gm_map_tcb.work;
        if (((int)work.flag & 536870912) == 0)
            return;
        work.flag &= 3758096383U;
        OBS_CAMERA obsCamera = ObjCameraGet(g_obj.glb_camera_id);
        float num = work.main_cam_user_disp[0] + work.main_cam_user_ofst[0] - obsCamera.disp_pos.x;
        for (int index = 0; index < 5; ++index)
        {
            if (index != 1)
                work.map_state[index].cam_ofst[0] += num * work.map_state[index].scrl_scale[0];
        }
    }

    private static void GmMapSetAddMapScrlScaleMagX(int map_type, int mag)
    {
        if (gm_map_tcb == null || map_type == 1 || (uint)map_type >= 5U)
            return;
        if (mag == 0)
            mag = 1;
        GMS_MAP_SYS_WORK work = (GMS_MAP_SYS_WORK)gm_map_tcb.work;
        GMS_MAP_OTHER_MAP_STATE mapOtherMapState = work.map_state[map_type];
        if ((work.flag & 2147483648U) > 0U)
            mapOtherMapState.scrl_scale[0] = mapOtherMapState.map_size[0] / (float)g_gm_main_system.map_size[0] / mag;
        else
            mapOtherMapState.scrl_scale[0] = (float)((mapOtherMapState.map_size[0] - (double)OBD_LCD_X) / (g_gm_main_system.map_size[0] - (double)OBD_LCD_X)) / mag;
    }

    private static void GmMapSetAddMapUserScrlXAddSize(float move_size)
    {
        if (gm_map_tcb == null)
            return;
        GMS_MAP_SYS_WORK work = (GMS_MAP_SYS_WORK)gm_map_tcb.work;
        work.main_cam_user_ofst[0] += move_size;
        float num = g_gm_main_system.map_size[0];
        if (work.main_cam_user_disp[0] + (double)work.main_cam_user_ofst[0] - OBD_LCD_X >= num)
        {
            work.main_cam_user_ofst[0] -= num;
        }
        else
        {
            if (work.main_cam_user_disp[0] + (double)work.main_cam_user_ofst[0] + OBD_LCD_X > -num)
                return;
            work.main_cam_user_ofst[0] += num;
        }
    }

    private static void GmMapGetAddMapCameraPos(
      NNS_VECTOR main_disp_pos,
      NNS_VECTOR main_target_pos,
      NNS_VECTOR dest_disp_pos,
      NNS_VECTOR dest_target_pos,
      int camera_id)
    {
        int i;
        GMS_MAP_SYS_WORK map_sys_work;
        float limit_left, limit_right, limit_top, limit_bottom;
        float cam_disp_width_half, cam_disp_height_half;
        GMS_MAP_OTHER_MAP_STATE map_state;
        NNS_VECTOR[] main_camera_pos = new NNS_VECTOR[2] { new NNS_VECTOR(), new NNS_VECTOR() };
        NNS_VECTOR[] dest_camera_pos = new NNS_VECTOR[2];

        MTM_ASSERT(main_disp_pos);
        MTM_ASSERT(main_target_pos);
        MTM_ASSERT(dest_disp_pos);
        MTM_ASSERT(dest_target_pos);

        if (gm_map_tcb == null)
        {
            Debug.Assert(false, "gmMap.cpp:GmMapGetAddMapCameraPos() Error map sys no init\n");
            return;
        }
        map_sys_work = (GMS_MAP_SYS_WORK)gm_map_tcb.work;
        main_camera_pos[0].Assign(main_disp_pos);
        main_camera_pos[1].Assign(main_target_pos);
        dest_camera_pos[0] = dest_disp_pos;
        dest_camera_pos[1] = dest_target_pos;
        if ((map_sys_work.flag & GMD_MAP_FLAG_ADD_MAP_MAIN_ADD_SCRL_H) == GMD_MAP_FLAG_ADD_MAP_MAIN_ADD_SCRL_H)
        {
            // メインカメラ位置ユーザースクロール対応
            main_camera_pos[0].x = map_sys_work.main_cam_user_disp[MTD_X] + map_sys_work.main_cam_user_ofst[MTD_X];
            main_camera_pos[1].x = map_sys_work.main_cam_user_target[MTD_X] + map_sys_work.main_cam_user_ofst[MTD_X];
        }
        cam_disp_width_half = (float)(OBD_LCD_X / 2);
        cam_disp_height_half = (float)(OBD_LCD_Y / 2);
        // 限界位置
        limit_left = 0 + cam_disp_width_half;
        limit_right = g_gm_main_system.map_size[MTD_X] - cam_disp_width_half;
        limit_top = 0 + cam_disp_height_half;
        limit_bottom = g_gm_main_system.map_size[MTD_Y] - cam_disp_height_half;


        if (camera_id == GME_CAMERA_NO_SNEAR)
        {
            map_state = map_sys_work.map_state[0];
        }
        else if (camera_id >= GME_CAMERA_NO_MID1)
        {
            map_state = map_sys_work.map_state[(camera_id - GME_CAMERA_NO_MID1) + 2/*超近景と中景を飛ばす*/];
        }
        else
        {
            MTM_ASSERT(0);
            map_state = map_sys_work.map_state[0];    // 仮に0番にしておく
        }

        for (i = 0; i < 2; i++)
        {
            if ((map_sys_work.flag & GMD_MAP_FLAG_ADD_MAP_LOOP_X) == 0)
            {
                // X
                if (main_camera_pos[i].x <= limit_left)
                {
                    // 限界位置
                    (dest_camera_pos[i]).x = cam_disp_width_half;
                }
                else if (main_camera_pos[i].x >= limit_right)
                {
                    // 限界位置
                    (dest_camera_pos[i]).x = map_state.map_size[MTD_X] - cam_disp_width_half;
                }
                else
                {
                    (dest_camera_pos[i]).x = cam_disp_width_half +
                                (main_camera_pos[i].x - cam_disp_width_half) * map_state.scrl_scale[MTD_X];
                }
            }
            else
            {
                // ループあり
                //(*dest_camera_pos[i]).x = cam_disp_width_half +
                //			(main_camera_pos[i].x - cam_disp_width_half) * map_state.scrl_scale[MTD_X];
                (dest_camera_pos[i]).x = main_camera_pos[i].x * map_state.scrl_scale[MTD_X];
            }

            // ユーザースクロール後のオフセット値反映
            (dest_camera_pos[i]).x += map_state.cam_ofst[MTD_X];

            {
                // Y
                if (-main_camera_pos[i].y <= limit_top)
                {
                    // 限界位置
                    (dest_camera_pos[i]).y = -cam_disp_height_half;
                }
                else if (-main_camera_pos[i].y >= limit_bottom)
                {
                    // 限界位置
                    (dest_camera_pos[i]).y = -(map_state.map_size[MTD_Y] - cam_disp_height_half);
                }
                else
                {
                    (dest_camera_pos[i]).y = -(cam_disp_height_half +
                                (-main_camera_pos[i].y - cam_disp_height_half) * map_state.scrl_scale[MTD_Y]);
                }
            }

            //// ユーザースクロール後のオフセット値反映
            //(*dest_camera_pos[i]).y += map_state.cam_ofst[MTD_Y];

            // Z
            (dest_camera_pos[i]).z = main_camera_pos[i].z;
        }
    }

    private static void GmMapSetDispB(bool disp)
    {
        if (gm_map_tcb == null)
            return;
        GMS_MAP_SYS_WORK work = (GMS_MAP_SYS_WORK)gm_map_tcb.work;
        if (disp)
            work.flag &= 4026531839U;
        else
            work.flag |= 268435456U;
    }

    public static void GmMapSetDisp(bool disp)
    {
        if (gm_map_tcb == null)
            return;
        GMS_MAP_SYS_WORK work = (GMS_MAP_SYS_WORK)gm_map_tcb.work;
        if (disp)
            work.flag &= 4160749567U;
        else
            work.flag |= 134217728U;
    }

    private static bool GmMapIsDrawEnableMMapBack()
    {
        bool flag = true;
        OBS_CAMERA obsCamera = ObjCameraGet(g_obj.glb_camera_id);
        float x = obsCamera.disp_pos.x;
        float _src = -obsCamera.disp_pos.y;
        switch (g_gs_main_sys_info.stage_id)
        {
            case 0:
                if (GMM_MAP_IS_RANGE(x, 5450f, 5725f) && GMM_MAP_IS_RANGE(_src, 1010f, 1520f))
                {
                    flag = false;
                    break;
                }
                if (GMM_MAP_IS_RANGE(x, 8010f, 8500f) && GMM_MAP_IS_RANGE(_src, 1200f, 1650f))
                {
                    flag = false;
                    break;
                }
                if (GMM_MAP_IS_RANGE(x, 10055f, 10695f) && GMM_MAP_IS_RANGE(_src, 1025f, 1400f))
                {
                    flag = false;
                    break;
                }
                break;
            case 1:
                if (GMM_MAP_IS_RANGE(x, 3975f, 4650f) && GMM_MAP_IS_RANGE(_src, 1555f, 2200f))
                {
                    flag = false;
                    break;
                }
                if (x > 12415.0)
                {
                    flag = false;
                    break;
                }
                break;
            case 16:
                if (x < 2450.0)
                {
                    flag = false;
                    break;
                }
                if (GMM_MAP_IS_RANGE(x, 3020f, 5600f))
                {
                    flag = false;
                    break;
                }
                if (GMM_MAP_IS_RANGE(x, 6590f, 9200f))
                {
                    flag = false;
                    break;
                }
                break;
        }
        return flag;
    }

    private static void GmMapSetLight()
    {
        NNS_RGBA col = new NNS_RGBA(1f, 1f, 1f, 1f);
        NNS_VECTOR nnsVector = new NNS_VECTOR(1f, 1f, 1f);
        float intensity = 1f;
        if (GMM_MAIN_GET_ZONE_TYPE() == 0)
        {
            nnsVector.x = -1f;
            nnsVector.y = -1f;
            nnsVector.z = -1f;
            col.r = 1f;
            col.g = 1f;
            col.b = 1f;
            col.a = 1f;
            intensity = 1f;
        }
        else if (GMM_MAIN_GET_ZONE_TYPE() == 3)
        {
            nnsVector.x = -0.2f;
            nnsVector.y = 0.25f;
            nnsVector.z = -1f;
            col.r = 1f;
            col.g = 1f;
            col.b = 1f;
            col.a = 1f;
            intensity = g_gs_main_sys_info.stage_id != 14 ? 1f : 0.4f;
        }
        nnNormalizeVector(nnsVector, nnsVector);
        ObjDrawSetParallelLight(NNE_LIGHT_5, ref col, intensity, nnsVector);
    }

    private static void GmMapSetMapDrawSize(int size)
    {
        gm_map_draw_size[0] = gm_map_set_draw_size[size, 0];
        gm_map_draw_size[1] = gm_map_set_draw_size[size, 1];
    }

    private static void gmMapDest(MTS_TASK_TCB tcb)
    {
        gm_map_tcb = null;
    }

    private static void gmMapMain(MTS_TASK_TCB tcb)
    {
        if (gm_map_variant.blocks_as_model)
            gmMapMainModel(tcb);
        else
            gmMapMainTvx(tcb);
    }

    // the zone 2-2 near layer registers one of these before its camera, and it does nothing beyond
    // sitting in the command list
    // PC's gmMapMain passes the projection to sub_4B71F0 in ecx: the card stage alone passes 0
    // (perspective) for add layer 0 and 1 (ortho) for the rest, where every other stage passes a
    // literal 1 for all five. So the near layer is a perspective pass over an ortho stage, and only
    // here. ObjDraw's bucket loop needs that fact as much as the draw does
    internal static bool gmMapNearLayerIsPerspective()
    {
        return g_gs_main_sys_info.stage_id == GSD_MAIN_STAGE_ID_2_2;
    }

    // PC submits one of these into the near layer's bucket and it is nullsub_31 there too, so the
    // empty body is the port rather than a gap
    private static void gmMapMainNearLayerUserFunc(object data)
    {
    }

    // blocks draw with OBD_DISP_NOUPDATE because one model is submitted many times a frame, so the
    // motion has to be advanced here instead, exactly once per model
    private static void gmMapMainModel(MTS_TASK_TCB tcb)
    {
        GMS_MAP_SYS_WORK work = (GMS_MAP_SYS_WORK)tcb.work;
        if ((work.flag & GMD_MAP_FLAG_DISP_OFF) != 0U)
            return;

        // TODO(port): console has no BGM timer here, it starts the music in gmMainGameStart
        // instead. our GmMain is still the iPhone one and waits on this handshake, so dropping it
        // leaves the stage silent. remove once GmMain's console arm lands
        if ((g_gm_main_system.game_flag & GMD_GAME_FLAG_BGM_PLAY_WAIT) != 0U
            && --gm_map_draw_bgm_timer <= 0)
        {
            g_gm_main_system.game_flag |= GMD_GAME_FLAG_BGM_PLAY_ENABLE;
            g_gm_main_system.game_flag &= ~GMD_GAME_FLAG_BGM_PLAY_WAIT;
        }

        if (ObjObjectPauseCheck(0U) == 0U)
            gmMapMainUpdateMotion();

        OBS_CAMERA main_camera = ObjCameraGet(g_obj.glb_camera_id);
        ObjDraw3DNNSetCameraEx(g_obj.glb_camera_id, g_obj.glb_camera_type, main_camera.command_state);

        if (gm_map_use_fall_shader)
        {
            ObjDraw3DNNUserFunc(
                gmMapFallShaderSettingPrioPreMidMapUserFunc, null, 0, gm_map_fall_rander_command_state);
        }
        gm_map_fall_shader_drawn = false;

        float pos_x = main_camera.disp_pos.x;
        float pos_y = -main_camera.disp_pos.y;

        GMS_MAP_OTHER_MAP_STATE[] map_state = work.map_state;

        void DrawAPlane()
        {
            MP_HEADER a_mp = (MP_HEADER)g_gm_gamedat_map_set[GMD_GAMEDAT_MAPSET_A_MP];
            MD_HEADER a_md = (MD_HEADER)g_gm_gamedat_map_set[GMD_GAMEDAT_MAPSET_A_MD];
            GmMapDrawMap(gm_map_obj3d, a_mp, a_md, pos_x, pos_y, 0.0f, 0.0f, GMD_MAP_A_POS_Z, false);
        }

        void DrawBPlane()
        {
            if ((work.flag & GMD_MAP_FLAG_DISP_OFF_B) != 0U)
                return;

            MP_HEADER b_mp = (MP_HEADER)g_gm_gamedat_map_set[GMD_GAMEDAT_MAPSET_B_MP];
            MD_HEADER b_md = (MD_HEADER)g_gm_gamedat_map_set[GMD_GAMEDAT_MAPSET_B_MD];

            // TODO(port): zone 3 passes the A plane as a check map here and drops any block the two
            // agree on. gmMapDrawMapRangeChk is in neither the leak nor our tree, so zone 3's B
            // plane currently draws without that dedupe
            GmMapDrawMap(gm_map_obj3d, b_mp, b_md, pos_x, pos_y, 0.0f, 0.0f, GMD_MAP_B_POS_Z, false);
        }

        void DrawAddLayer(int index)
        {
            if ((work.flag & GMD_MAP_FLAG_ADD_MAP_1 << index) == 0U)
                return;

            bool loop_h = index != 1 && (work.flag & GMD_MAP_FLAG_ADD_MAP_LOOP_X) != 0U;

            if (gmMapNearLayerIsPerspective())
            {
                int draw_size = GMD_MAP_DRAW_WIDTH_DEF;
                int projection = NNE_PROJECTION_TYPE_ORTHO;
                if (index == 0)
                {
                    ObjDraw3DNNUserFunc(
                        gmMapMainNearLayerUserFunc, null, 0, map_state[0].command_state);
                    projection = NNE_PROJECTION_TYPE_PERSPECTIVE;
                    draw_size = GMD_MAP_DRAW_WIDTH_MAX;
                }

                ObjDraw3DNNSetCameraEx(
                    gm_map_addmap_camera_tbl[index], projection, map_state[index].command_state);
                gm_map_draw_size[0] = (uint)draw_size;
                gm_map_draw_size[1] = (uint)draw_size;
            }
            else
            {
                ObjDraw3DNNSetCameraEx(
                    gm_map_addmap_camera_tbl[index],
                    NNE_PROJECTION_TYPE_ORTHO,
                    map_state[index].command_state);
            }

            OBS_CAMERA camera = ObjCameraGet(gm_map_addmap_camera_tbl[index]);
            GmMapSetDrawState(map_state[index].command_state);

            MP_HEADER mp_header = (MP_HEADER)g_gm_gamedat_map_set_add[index * 2];
            MD_HEADER md_header = (MD_HEADER)g_gm_gamedat_map_set_add[1 + index * 2];
            GmMapDrawMap(
                gm_map_obj3d,
                mp_header,
                md_header,
                camera.disp_pos.x,
                -camera.disp_pos.y,
                0.0f,
                0.0f,
                map_state[index].pos_z,
                loop_h);
        }

        // front to back, recovered from the retail binary at 0x4CFD90. Safe because every
        // transparent subobject is deferred to the sorted pass, see objDraw3DNNRegistSortDraw
        DrawAPlane();
        DrawBPlane();
        for (int index = 0; index < GME_MAP_ADD_MAP_MAX; ++index)
            DrawAddLayer(index);

        // leaves the state where the next frame's A and B planes want it, which is why neither of
        // them sets it
        GmMapSetDrawState(OBD_DRAW_CMD_STATE_3DNN);
        OBS_CAMERA restore_camera = ObjCameraGet(g_obj.glb_camera_id);
        ObjDraw3DNNSetCameraEx(
            g_obj.glb_camera_id, g_obj.glb_camera_type, restore_camera.command_state);
    }

    private static void gmMapMainUpdateMotion()
    {
        AMS_AMB_HEADER model_amb = (AMS_AMB_HEADER)g_gm_gamedat_map[GMD_GAMEDAT_MAP_MODEL];
        if (model_amb == null || gm_map_obj3d == null)
            return;

        for (int index = GMD_MAP_OBJECT_START; index < model_amb.file_num; ++index)
        {
            OBS_ACTION3D_NN_WORK obj_3d = gm_map_obj3d[index];
            if (obj_3d.motion == null)
                continue;

            uint disp_flag = OBD_DISP_REPEAT;

            if (obj_3d.mtn[0] != null)
            {
                ObjDrawAction3DNNMotionUpdate(obj_3d, ref disp_flag);

                if (g_gs_main_sys_info.stage_id == GSD_MAIN_STAGE_ID_3_2
                    && obj_3d.command_state == OBD_DRAW_CMD_STATE_NEAR_MAP)
                {
                    // TODO(port): the three nn frame accessors this uses were not recovered from the
                    // binary; obj_3d.frame[0] against the action's end frame is the natural mapping
                    float end_frame = amMotionGetEndFrame(obj_3d.motion, obj_3d.act_id[0]);
                    if (gm_map_z32_share_frame < end_frame)
                    {
                        if (obj_3d.frame[0] >= gm_map_z32_share_frame)
                            gm_map_z32_share_frame = obj_3d.frame[0];
                    }
                    else
                        gm_map_z32_share_frame = end_frame;

                    obj_3d.frame[0] = gm_map_z32_share_frame;
                }
            }

            if (obj_3d.mat_mtn[0] != null)
                ObjDrawAction3DNNMaterialUpdate(obj_3d, ref disp_flag);
        }
    }

    private static void gmMapMainTvx(MTS_TASK_TCB tcb)
    {
        int zoneType = GMM_MAIN_GET_ZONE_TYPE();
        if (zoneType == 5)
        {
            if (((int)g_gm_main_system.game_flag & 268435456) == 0 || --gm_map_draw_bgm_timer > 0)
                return;
            g_gm_main_system.game_flag |= 134217728U;
            g_gm_main_system.game_flag &= 4026531839U;
        }
        else
        {
            int num1 = 0;
            int num2 = gm_map_add_tbl_use_no[zoneType] + 1;
            GMS_MAP_SYS_WORK work = (GMS_MAP_SYS_WORK)tcb.work;
            if (((int)work.flag & 134217728) != 0)
                return;
            if (ObjObjectPauseCheck(0U) == 0U)
                gmMapUpdateDrawMapTvxTexScroll();
            if (!GmMainIsDrawEnable())
                return;
            if (((int)g_gm_main_system.game_flag & 268435456) != 0 && --gm_map_draw_bgm_timer <= 0)
            {
                g_gm_main_system.game_flag |= 134217728U;
                g_gm_main_system.game_flag &= 4026531839U;
            }
            TVX_FILE[] gGmGamedat = (TVX_FILE[])g_gm_gamedat_map[1];
            NNS_TEXLIST texList = AoTexGetTexList(gm_map_texture);
            NNS_MATRIX gmMapMainMtx = gmMapMain_mtx;
            nnMakeUnitMatrix(gmMapMainMtx);
            if (work.auto_resize)
            {
                OBS_CAMERA obsCamera = ObjCameraGet(g_obj.glb_camera_id);
                if ((obsCamera.roll & 262143) != 0)
                    GmMapSetMapDrawSize(0);
                else if ((obsCamera.roll & 16384) != 0)
                    GmMapSetMapDrawSize(2);
                else
                    GmMapSetMapDrawSize(1);
            }
            GMS_MAP_OTHER_MAP_STATE[] mapState = work.map_state;
            for (int index = num2 - 1; index >= num1; --index)
            {
                if (((int)work.flag & 1 << index) != 0 && (index < 2 || GmMapIsDrawEnableMMapBack()))
                {
                    bool loop_h = false;
                    if (index != 1 && ((int)work.flag & int.MinValue) != 0)
                        loop_h = true;
                    ObjDraw3DNNSetCameraEx(gm_map_addmap_camera_tbl[index], 1, mapState[index].command_state);
                    OBS_CAMERA obsCamera = ObjCameraGet(gm_map_addmap_camera_tbl[index]);
                    float x = obsCamera.disp_pos.x;
                    float pos_y = -obsCamera.disp_pos.y;
                    GmMapSetDrawState(mapState[index].command_state);
                    MP_HEADER mp_header = (MP_HEADER)g_gm_gamedat_map_set_add[index * 2];
                    MD_HEADER md_header = (MD_HEADER)g_gm_gamedat_map_set_add[1 + index * 2];
                    gmMapInitDrawMapTvx();
                    gmMapSetDrawMapTvx(gGmGamedat, mp_header, md_header, x, pos_y, gmMapTransX, 0.0f, mapState[index].pos_z, loop_h, mp_header.blocks[0], md_header.blocks[0]);
                    gmMapExecuteDrawMapTvx(gmMapMainMtx, texList);
                }
            }
            GmMapSetDrawState(0U);
            ObjDraw3DNNSetCameraEx(g_obj.glb_camera_id, g_obj.glb_camera_type, 0U);
            OBS_CAMERA obsCamera1 = ObjCameraGet(g_obj.glb_camera_id);
            float x1 = obsCamera1.disp_pos.x;
            float pos_y1 = -obsCamera1.disp_pos.y;
            gmMapInitDrawMapTvx();
            if (((int)work.flag & 268435456) == 0)
            {
                MP_HEADER gGmGamedatMap1 = (MP_HEADER)g_gm_gamedat_map_set[1];
                MD_HEADER gGmGamedatMap2 = (MD_HEADER)g_gm_gamedat_map_set[3];
                gmMapSetDrawMapTvx(gGmGamedat, gGmGamedatMap1, gGmGamedatMap2, x1, pos_y1, gmMapTransX, 0.0f, sbyte.MinValue, false, gGmGamedatMap1.blocks[0], gGmGamedatMap2.blocks[0]);
            }
            MP_HEADER gGmGamedatMap3 = (MP_HEADER)g_gm_gamedat_map_set[0];
            MD_HEADER gGmGamedatMap4 = (MD_HEADER)g_gm_gamedat_map_set[2];
            gmMapSetDrawMapTvx(gGmGamedat, gGmGamedatMap3, gGmGamedatMap4, x1, pos_y1, gmMapTransX, 0.0f, 128f, false, gGmGamedatMap3.blocks[0], gGmGamedatMap4.blocks[0]);
            gmMapExecuteDrawMapTvx(gmMapMainMtx, texList);
            if (gm_map_tex_draw_count <= 0)
                return;
            --gm_map_tex_draw_count;
            ObjLoadInitDraw();
            if (gm_map_tex_draw_count == 0)
                ObjLoadClearDraw();
            for (int index = 0; index < gm_map_texture.texlist.nTex; ++index)
            {
                AMS_PARAM_DRAW_PRIMITIVE prim = GlobalPool<AMS_PARAM_DRAW_PRIMITIVE>.Alloc();
                prim.Clear();
                prim.type = 0;
                prim.ablend = 1;
                prim.bldSrc = 770;
                prim.bldDst = 771;
                prim.aTest = 1;
                prim.zMask = 0;
                prim.zTest = 1;
                prim.noSort = 1;
                prim.uwrap = 1;
                prim.vwrap = 1;
                prim.format3D = 4;
                prim.texlist = texList;
                NNS_PRIM3D_PCT_ARRAY nnsPriM3DPctArray = amDrawAlloc_NNS_PRIM3D_PCT(4);
                NNS_PRIM3D_PCT[] buffer = nnsPriM3DPctArray.buffer;
                int offset = nnsPriM3DPctArray.offset;
                prim.vtxPCT3D = nnsPriM3DPctArray;
                prim.count = 4;
                prim.texId = index;
                buffer[offset].Pos.x = buffer[offset + 1].Pos.x = -1f;
                buffer[offset + 2].Pos.x = buffer[offset + 3].Pos.x = 1f;
                buffer[offset].Pos.y = buffer[offset + 2].Pos.y = -1f;
                buffer[offset + 1].Pos.y = buffer[offset + 3].Pos.y = 1f;
                buffer[offset].Pos.z = buffer[offset + 1].Pos.z = buffer[offset + 2].Pos.z = buffer[offset + 3].Pos.z = -1f;
                buffer[offset].Tex.u = buffer[offset + 1].Tex.u = 0.0f;
                buffer[offset + 2].Tex.u = buffer[offset + 3].Tex.u = 1f;
                buffer[offset].Tex.v = buffer[offset + 2].Tex.v = 0.0f;
                buffer[offset + 1].Tex.v = buffer[offset + 3].Tex.v = 1f;
                buffer[offset].Col = buffer[offset + 1].Col = buffer[offset + 2].Col = buffer[offset + 3].Col = uint.MaxValue;
                ObjDraw3DNNDrawPrimitive(prim, gm_map_draw_command_state, 0, 0);
                GlobalPool<AMS_PARAM_DRAW_PRIMITIVE>.Release(prim);
            }
        }
    }

    private static void gmMapInitDrawMapTvx()
    {
        GMS_MAP_PRIM_DRAW_WORK[] gmMapPrimDrawWork = gm_map_prim_draw_work;
        for (int index = 0; index < 16; ++index)
        {
            gmMapPrimDrawWork[index].tex_id = -1;
            gmMapPrimDrawWork[index].all_vtx_num = 0U;
            gmMapPrimDrawWork[index].stack_num = 0U;
        }
    }

    private static void gmMapSetDrawMapTvx(
      TVX_FILE[] tvxamb,
      MP_HEADER mp_header,
      MD_HEADER md_header,
      float pos_x,
      float pos_y,
      float trans_x,
      float trans_y,
      float trans_z,
      bool loop_h,
      MP_BLOCK mp_block,
      MD_BLOCK md_block)
    {
        float num1 = 0.0f;
        float num2 = 0.0f;
        int num3 = (int)pos_x >> 6;
        int num4 = (int)pos_y >> 6;
        int num5 = (int)GMD_MAP_DRAW_WIDTH + 2 + (int)gm_map_draw_margin_adjust * 2;
        int num6 = (int)GMD_MAP_DRAW_HEIGHT + 2 + (int)gm_map_draw_margin_adjust * 2;
        int block_left = num3 - (num5 >> 1);
        int block_right = num3 + (num5 >> 1);
        int block_top = num4 - (num6 >> 1);
        int block_bottom = num4 + (num6 >> 1);
        if (block_left < 0)
        {
            if (!loop_h)
            {
                block_left = 0;
            }
            else
            {
                block_left += mp_header.map_w;
                num1 = -(mp_header.map_w << 6);
            }
        }
        else if (block_left >= mp_header.map_w)
        {
            if (!loop_h)
            {
                block_left = mp_header.map_w - num5;
            }
            else
            {
                block_left -= mp_header.map_w;
                num1 = mp_header.map_w << 6;
            }
        }
        if (block_top < 0)
            block_top = 0;
        if (block_right >= mp_header.map_w)
        {
            if (!loop_h)
            {
                block_right = mp_header.map_w - 1;
            }
            else
            {
                block_right -= mp_header.map_w;
                num2 = mp_header.map_w << 6;
            }
        }
        else if (block_right < 0)
        {
            if (!loop_h)
            {
                block_right = num5;
            }
            else
            {
                block_right += mp_header.map_w;
                num2 = -(mp_header.map_w << 6);
            }
        }
        if (block_bottom >= mp_header.map_h)
            block_bottom = mp_header.map_h - 1;
        if (block_left < block_right)
        {
            if (loop_h)
                gmMapSetDrawMapRangeTvx(tvxamb, mp_header, md_header, trans_x + num1, trans_y, trans_z, block_left, block_right, block_top, block_bottom, mp_block, md_block);
            else
                gmMapSetDrawMapRangeTvx(tvxamb, mp_header, md_header, trans_x, trans_y, trans_z, block_left, block_right, block_top, block_bottom, mp_block, md_block);
        }
        else
        {
            gmMapSetDrawMapRangeTvx(tvxamb, mp_header, md_header, trans_x + num1, trans_y, trans_z, block_left, mp_header.map_w - 1, block_top, block_bottom, mp_block, md_block);
            gmMapSetDrawMapRangeTvx(tvxamb, mp_header, md_header, trans_x + num2, trans_y, trans_z, 0, block_right, block_top, block_bottom, mp_block, md_block);
        }
    }

    private static void gmMapSetDrawMapRangeTvx(
      TVX_FILE[] tvxamb,
      MP_HEADER mp_header,
      MD_HEADER md_header,
      float trans_x,
      float trans_y,
      float trans_z,
      int block_left,
      int block_right,
      int block_top,
      int block_bottom,
      MP_BLOCK _mp_block,
      MD_BLOCK _md_block)
    {
        for (int index1 = 0; index1 < GMD_MAP_DRAW_CHECK_WIDTH; ++index1)
        {
            for (int index2 = 0; index2 < GMD_MAP_DRAW_CHECK_HEIGHT; ++index2)
                gm_map_block_check[index1, index2] = -1;
        }
        int gmdMapDrawWidth = (int)GMD_MAP_DRAW_WIDTH;
        int gmdMapDrawHeight = (int)GMD_MAP_DRAW_HEIGHT;
        GMS_MAP_PRIM_DRAW_WORK[] gmMapPrimDrawWork = gm_map_prim_draw_work;
        for (int index1 = block_left; index1 <= block_right; ++index1)
        {
            for (int index2 = block_top; index2 <= block_bottom; ++index2)
            {
                int index3 = index2 * mp_header.map_w + index1;
                MP_BLOCK block1 = mp_header.blocks[index3];
                float num1 = index1;
                float num2 = index2;
                int id = block1.id;
                if (id == 0)
                {
                    MD_BLOCK block2 = md_header.blocks[index3];
                    int ofstX = block2.ofst_x;
                    int ofstY = block2.ofst_y;
                    if ((ofstX | ofstY) != 0)
                    {
                        int index4 = index3 + (mp_header.map_w * ofstY + ofstX);
                        block1 = mp_header.blocks[index4];
                        id = block1.id;
                        num1 += ofstX;
                        num2 += ofstY;
                    }
                    else
                        continue;
                }
                if (id != 0 && gm_map_block_check[GMD_MAP_DRAW_DF_SIZE + (int)num1 - block_left, GMD_MAP_DRAW_DF_SIZE + (int)num2 - block_top] == -1)
                {
                    gm_map_block_check[GMD_MAP_DRAW_DF_SIZE + (int)num1 - block_left, GMD_MAP_DRAW_DF_SIZE + (int)num2 - block_top] = (short)id;
                    int index4 = id - 1;
                    TVX_FILE file = tvxamb[index4];
                    uint textureNum = AoTvxGetTextureNum(file);
                    for (uint tex_no = 0; tex_no < textureNum; ++tex_no)
                    {
                        uint vertexNum = AoTvxGetVertexNum(file, tex_no);
                        int textureId = AoTvxGetTextureId(file, tex_no);
                        for (int index5 = 0; index5 < 16; ++index5)
                        {
                            if (gmMapPrimDrawWork[index5].tex_id == -1 || gmMapPrimDrawWork[index5].tex_id == textureId)
                            {
                                gmMapPrimDrawWork[index5].tex_id = textureId;
                                gmMapPrimDrawWork[index5].all_vtx_num += vertexNum;
                                GMS_MAP_PRIM_DRAW_STACK mapPrimDrawStack = gmMapPrimDrawWork[index5].stack[(int)gmMapPrimDrawWork[index5].stack_num];
                                mapPrimDrawStack.vtx = AoTvxGetVertex(file, tex_no);
                                mapPrimDrawStack.vtx_num = (ushort)vertexNum;
                                mapPrimDrawStack.mp = block1;
                                mapPrimDrawStack.dx = trans_x + (float)((num1 + 0.5) * 64.0);
                                mapPrimDrawStack.dy = (float)(-trans_y + (-num2 - 0.5) * 64.0);
                                mapPrimDrawStack.dz = trans_z;
                                ++gmMapPrimDrawWork[index5].stack_num;
                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    private static void gmMapExecuteDrawMapTvx(NNS_MATRIX mtx, NNS_TEXLIST texlist)
    {
        AMS_PARAM_DRAW_PRIMITIVE dat = GlobalPool<AMS_PARAM_DRAW_PRIMITIVE>.Alloc();
        GMS_MAP_PRIM_DRAW_WORK[] gmMapPrimDrawWork = gm_map_prim_draw_work;
        int[] primDrawTvxAlphaSet = gm_map_prim_draw_tvx_alpha_set;
        GMS_MAP_PRIM_DRAW_WORK[] gmsMapPrimDrawWorkArray = amDraw_GMS_MAP_PRIM_DRAW_WORK_Array_Pool.AllocArray(8);
        uint num = 0;
        uint primDrawTvxColor = gm_map_prim_draw_tvx_color;
        dat.type = 1;
        dat.ablend = 0;
        dat.bldMode = 32774;
        dat.aTest = 1;
        dat.zMask = 0;
        dat.zTest = 1;
        dat.noSort = 1;
        dat.uwrap = 1;
        dat.vwrap = 1;
        dat.format3D = 4;
        dat.texlist = texlist;
        for (uint index = 0; index < 16U && gmMapPrimDrawWork[(int)index].tex_id != -1; ++index)
        {
            if (primDrawTvxAlphaSet != null && primDrawTvxAlphaSet[gmMapPrimDrawWork[(int)index].tex_id] != 0)
            {
                if (num < 8U)
                {
                    gmsMapPrimDrawWorkArray[(int)num] = gmMapPrimDrawWork[(int)index];
                    gmsMapPrimDrawWorkArray[(int)num].op = (uint)primDrawTvxAlphaSet[gmMapPrimDrawWork[(int)index].tex_id];
                    ++num;
                }
                else
                    break;
            }
            else
            {
                dat.count = (int)gmMapPrimDrawWork[(int)index].all_vtx_num + (int)gmMapPrimDrawWork[(int)index].stack_num * 2 - 2;
                NNS_PRIM3D_PCT_ARRAY v_tbl_array = amDrawAlloc_NNS_PRIM3D_PCT(dat.count);
                gmMapExecuteDrawMapTvxCore(mtx, gmMapPrimDrawWork[(int)index], dat, v_tbl_array, primDrawTvxColor);
            }
        }
        if (primDrawTvxAlphaSet != null)
        {
            for (int index = 0; index < num; ++index)
            {
                switch (gmsMapPrimDrawWorkArray[index].op)
                {
                    case 1:
                        dat.bldSrc = 770;
                        dat.bldDst = 771;
                        dat.ablend = 1;
                        dat.aTest = 1;
                        break;
                    case 2:
                        dat.bldSrc = 770;
                        dat.bldDst = 1;
                        dat.ablend = 1;
                        dat.aTest = 0;
                        break;
                }
                dat.count = (int)gmsMapPrimDrawWorkArray[index].all_vtx_num + (int)gmsMapPrimDrawWorkArray[index].stack_num * 2 - 2;
                NNS_PRIM3D_PCT_ARRAY v_tbl_array = amDrawAlloc_NNS_PRIM3D_PCT(dat.count);
                gmMapExecuteDrawMapTvxCore(mtx, gmsMapPrimDrawWorkArray[index], dat, v_tbl_array, primDrawTvxColor);
            }
        }
        GlobalPool<AMS_PARAM_DRAW_PRIMITIVE>.Release(dat);
    }

    private static void gmMapExecuteDrawMapTvxCore(
      NNS_MATRIX mtx,
      GMS_MAP_PRIM_DRAW_WORK work,
      AMS_PARAM_DRAW_PRIMITIVE dat,
      NNS_PRIM3D_PCT_ARRAY v_tbl_array,
      uint color)
    {
        int offset = v_tbl_array.offset;
        NNS_PRIM3D_PCT[] buffer = v_tbl_array.buffer;
        dat.vtxPCT3D = v_tbl_array;
        dat.texId = work.tex_id;
        NNS_TEXCOORD scr_uv;
        gmMapGetDrawMapTvxTexScrollUV(dat.texId, out scr_uv);
        SNNS_VECTOR src = new SNNS_VECTOR();
        for (uint index1 = 0; index1 < work.stack_num; ++index1)
        {
            GMS_MAP_PRIM_DRAW_STACK mapPrimDrawStack = work.stack[(int)index1];
            int num1 = mapPrimDrawStack.vtx_num / 3;
            float dx = mapPrimDrawStack.dx;
            float dy = mapPrimDrawStack.dy;
            float dz = mapPrimDrawStack.dz;
            NNS_MATRIX usePrimMatrix = gmMapGetUsePrimMatrix(mapPrimDrawStack.mp.rot, mapPrimDrawStack.mp.flip_h | mapPrimDrawStack.mp.flip_v << 1);
            int num2 = offset;
            AOS_TVX_VERTEX[] vtx = mapPrimDrawStack.vtx;
            for (int index2 = 0; index2 < mapPrimDrawStack.vtx_num; ++index2)
            {
                src.x = vtx[index2].x;
                src.y = vtx[index2].y;
                src.z = vtx[index2].z;
                int index3 = num2 + index2;
                nnTransformVector(ref buffer[index3].Pos, usePrimMatrix, ref src);
                buffer[index3].Pos.x += dx;
                buffer[index3].Pos.y += dy;
                buffer[index3].Pos.z += dz;
                buffer[index3].Tex.u = vtx[index2].u + scr_uv.u;
                buffer[index3].Tex.v = vtx[index2].v + scr_uv.v;
                buffer[index3].Col = vtx[index2].c & color;
            }
            offset += mapPrimDrawStack.vtx_num + 2;
            if (index1 != 0U)
            {
                int index2 = num2 - 1;
                buffer[index2] = buffer[index2 + 1];
            }
            if ((int)index1 != (int)work.stack_num - 1)
            {
                int index2 = num2 + (mapPrimDrawStack.vtx_num - 1);
                buffer[index2 + 1] = buffer[index2];
            }
        }
        amMatrixPush(mtx);
        ObjDraw3DNNDrawPrimitive(dat, gm_map_draw_command_state, 0, 0);
        amMatrixPop();
    }

    private static void gmMapBuildDrawMapTvxTexScroll()
    {
        GMS_MAP_PRIM_DRAW_TVX_UV_WORK mapPrimDrawUvWork = gm_map_prim_draw_uv_work;
        int length1 = 0;
        int length2 = 0;
        int num1 = g_gm_gamedat_zone_type_tbl[g_gs_main_sys_info.stage_id];
        switch (num1)
        {
            case 0:
                return;
            case 1:
                length1 = gm_map_prim_draw_tvx_mgr_index_tbl_z2_num;
                length2 = 21;
                break;
            case 2:
                length1 = gm_map_prim_draw_tvx_mgr_index_tbl_z3_num;
                length2 = 4;
                break;
            case 3:
                length1 = gm_map_prim_draw_tvx_mgr_index_tbl_z4_num;
                length2 = 9;
                break;
            case 4:
                length1 = gm_map_prim_draw_tvx_mgr_index_tbl_zf_num;
                length2 = 8;
                break;
            case 5:
                return;
        }
        uint num2 = (uint)((uint)(32UL + (ulong)(4 * length1)) + (ulong)(4 * length1));
        GMS_MAP_PRIM_DRAW_TVX_UV_WORK primDrawTvxUvWork = new GMS_MAP_PRIM_DRAW_TVX_UV_WORK();
        gm_map_prim_draw_uv_work = primDrawTvxUvWork;
        primDrawTvxUvWork.mgr_index_tbl_num = length1;
        primDrawTvxUvWork.frame_index_tbl = new uint[length1];
        primDrawTvxUvWork.frame_tbl = new uint[length1];
        primDrawTvxUvWork.tex_uv_index_tbl = new int[length2];
        int index1 = 0;
        for (int length3 = primDrawTvxUvWork.tex_uv_index_tbl.Length; index1 < length3; ++index1)
            primDrawTvxUvWork.tex_uv_index_tbl[index1] = -1;
        switch (num1)
        {
            case 0:
                return;
            case 1:
                primDrawTvxUvWork.mgr_index_tbl_addr = gm_map_prim_draw_tvx_mgr_index_tbl_z2;
                primDrawTvxUvWork.mgr_tbl_num = gm_map_prim_draw_tvx_mgr_tbl_z2_num;
                primDrawTvxUvWork.mgr_tbl_addr = gm_map_prim_draw_tvx_mgr_tbl_z2;
                primDrawTvxUvWork.uv_mgr_tbl_addr = gm_map_prim_draw_tvx_uv_mgr_tbl_z2;
                break;
            case 2:
                primDrawTvxUvWork.mgr_index_tbl_addr = gm_map_prim_draw_tvx_mgr_index_tbl_z3;
                primDrawTvxUvWork.mgr_tbl_num = gm_map_prim_draw_tvx_mgr_tbl_z3_num;
                primDrawTvxUvWork.mgr_tbl_addr = gm_map_prim_draw_tvx_mgr_tbl_z3;
                primDrawTvxUvWork.uv_mgr_tbl_addr = gm_map_prim_draw_tvx_uv_mgr_tbl_z3;
                break;
            case 3:
                primDrawTvxUvWork.mgr_index_tbl_addr = gm_map_prim_draw_tvx_mgr_index_tbl_z4;
                primDrawTvxUvWork.mgr_tbl_num = gm_map_prim_draw_tvx_mgr_tbl_z4_num;
                primDrawTvxUvWork.mgr_tbl_addr = gm_map_prim_draw_tvx_mgr_tbl_z4;
                primDrawTvxUvWork.uv_mgr_tbl_addr = gm_map_prim_draw_tvx_uv_mgr_tbl_z4;
                break;
            case 4:
                primDrawTvxUvWork.mgr_index_tbl_addr = gm_map_prim_draw_tvx_mgr_index_tbl_zf;
                primDrawTvxUvWork.mgr_tbl_num = gm_map_prim_draw_tvx_mgr_tbl_zf_num;
                primDrawTvxUvWork.mgr_tbl_addr = gm_map_prim_draw_tvx_mgr_tbl_zf;
                primDrawTvxUvWork.uv_mgr_tbl_addr = gm_map_prim_draw_tvx_uv_mgr_tbl_zf;
                break;
            case 5:
                return;
        }
        DoubleType<uint[], GMS_MAP_PRIM_DRAW_TVX_MGR_INDEX[]> mgrIndexTblAddr = primDrawTvxUvWork.mgr_index_tbl_addr;
        int[] texUvIndexTbl = primDrawTvxUvWork.tex_uv_index_tbl;
        for (int index2 = 0; index2 < length1; ++index2)
        {
            ushort texId = ((GMS_MAP_PRIM_DRAW_TVX_MGR_INDEX[])mgrIndexTblAddr)[index2].tex_id;
            ushort mgrId = ((GMS_MAP_PRIM_DRAW_TVX_MGR_INDEX[])mgrIndexTblAddr)[index2].mgr_id;
            for (int index3 = 0; index3 < length2; ++index3)
            {
                if (texId == index3)
                    texUvIndexTbl[index3] = mgrId;
            }
        }
    }

    private static void gmMapFlushDrawMapTvxTexScroll()
    {
        gm_map_prim_draw_uv_work = null;
    }

    private static void gmMapUpdateDrawMapTvxTexScroll()
    {
        GMS_MAP_PRIM_DRAW_TVX_UV_WORK mapPrimDrawUvWork = gm_map_prim_draw_uv_work;
        if (mapPrimDrawUvWork == null)
            return;
        int mgrIndexTblNum = mapPrimDrawUvWork.mgr_index_tbl_num;
        int[] mgrTblNum = mapPrimDrawUvWork.mgr_tbl_num;
        uint[] frameIndexTbl = mapPrimDrawUvWork.frame_index_tbl;
        uint[] frameTbl = mapPrimDrawUvWork.frame_tbl;
        GMS_MAP_PRIM_DRAW_TVX_MGR_INDEX[] mgrIndexTblAddr = (GMS_MAP_PRIM_DRAW_TVX_MGR_INDEX[])mapPrimDrawUvWork.mgr_index_tbl_addr;
        for (int index = 0; index < mgrIndexTblNum; ++index)
        {
            ushort mgrId = mgrIndexTblAddr[index].mgr_id;
            GMS_MAP_PRIM_DRAW_TVX_MGR[] mapPrimDrawTvxMgrArray = ((GMS_MAP_PRIM_DRAW_TVX_MGR[][])mapPrimDrawUvWork.mgr_tbl_addr)[mgrId];
            if (++frameTbl[mgrId] >= mapPrimDrawTvxMgrArray[(int)frameIndexTbl[mgrId]].time)
            {
                frameTbl[mgrId] = 0U;
                frameIndexTbl[mgrId] = (uint)((frameIndexTbl[mgrId] + 1U) % (ulong)mgrTblNum[mgrId]);
            }
        }
    }

    private static void gmMapGetDrawMapTvxTexScrollUV(int tex_id, out NNS_TEXCOORD scr_uv)
    {
        GMS_MAP_PRIM_DRAW_TVX_UV_WORK mapPrimDrawUvWork = gm_map_prim_draw_uv_work;
        scr_uv.u = 0.0f;
        scr_uv.v = 0.0f;
        if (mapPrimDrawUvWork == null)
            return;
        int index = mapPrimDrawUvWork.tex_uv_index_tbl[tex_id];
        if (-1 == index)
            return;
        GMS_MAP_PRIM_DRAW_TVX_MGR[] mapPrimDrawTvxMgrArray = ((GMS_MAP_PRIM_DRAW_TVX_MGR[][])mapPrimDrawUvWork.mgr_tbl_addr)[index];
        NNS_TEXCOORD[] nnsTexcoordArray = ((NNS_TEXCOORD[][])mapPrimDrawUvWork.uv_mgr_tbl_addr)[index];
        uint num = mapPrimDrawUvWork.frame_index_tbl[index];
        ushort motionId = mapPrimDrawTvxMgrArray[(int)num].motion_id;
        scr_uv.u = nnsTexcoordArray[motionId].u;
        scr_uv.v = nnsTexcoordArray[motionId].v;
    }

    private static void gmMapCreateUsePrimMatrix()
    {
        NNS_MATRIX nnsMatrix1 = GlobalPool<NNS_MATRIX>.Alloc();
        for (int index1 = 0; index1 < 4; ++index1)
        {
            nnMakeUnitMatrix(nnsMatrix1);
            nnRotateZMatrix(nnsMatrix1, nnsMatrix1, index1 * 16384);
            for (int index2 = 0; index2 < 4; ++index2)
            {
                NNS_MATRIX nnsMatrix2 = gm_map_use_prim_mtx[index1 * 4 + index2];
                nnCopyMatrix(nnsMatrix2, nnsMatrix1);
                switch (index2)
                {
                    case 0:
                        nnScaleMatrix(nnsMatrix2, nnsMatrix2, 1f, 1f, 1f);
                        break;
                    case 1:
                        nnScaleMatrix(nnsMatrix2, nnsMatrix2, -1f, 1f, 1f);
                        break;
                    case 2:
                        nnScaleMatrix(nnsMatrix2, nnsMatrix2, 1f, -1f, 1f);
                        break;
                    case 3:
                        nnScaleMatrix(nnsMatrix2, nnsMatrix2, -1f, -1f, 1f);
                        break;
                }
                nnTranslateMatrix(nnsMatrix2, nnsMatrix2, -32f, -32f, 0.0f);
            }
        }
    }

    private static NNS_MATRIX gmMapGetUsePrimMatrix(int rot, int flip)
    {
        return gm_map_use_prim_mtx[rot * 4 + flip];
    }

    private static void gmMapFallShaderSettingPrioPreMidMapUserFunc(object data)
    {
        if (!gm_map_fall_shader_drawn)
            return;

        NNS_RGBA_U8 color = new NNS_RGBA_U8(0, 0, 0, byte.MaxValue);
        AMS_RENDER_TARGET target = _am_render_manager.targetp != _gm_mapFar_render_work ? _gm_mapFar_render_work : _am_draw_target;
        if (target.width == 0)
            return;
        amRenderCopyTarget(target, color);
    }

    private static void gmMapDrawFallShaderPrioPreMidMapUserFunc(object data)
    {
        AMS_RENDER_TARGET render_target = _am_render_manager.targetp != _gm_mapFar_render_work
            ? _gm_mapFar_render_work
            : _am_draw_target;
        if (render_target.width == 0)
            return;

        mppBindFallShaderUserSampler(render_target);
    }

    private static readonly uint[] mppFallShaderUserSamplerState = new uint[NNE_SAMPLERSTATETYPE_MAX];

    // gmMap.cpp:3232-3260, and the same block verbatim in gmMapFar, gmDeco, dmTitleOp and
    // gmEneMereon
    internal static void mppBindFallShaderUserSampler(AMS_RENDER_TARGET render_target)
    {
        NNS_MATRIX projection = amDrawGetProjectionMatrix();
        if (projection == null)
            return;

        uint[] state = mppFallShaderUserSamplerState;
        nnInitMaterialControlUserSamplerDXG20();
        nnGetMaterialControlUserSamplerDefaultStateDXG20(state);
        state[NNE_SAMPLERSTATETYPE_ADDRESSU] = D3DTADDRESS_CLAMP;
        state[NNE_SAMPLERSTATETYPE_ADDRESSV] = D3DTADDRESS_CLAMP;
        state[NNE_SAMPLERSTATETYPE_ADDRESSW] = D3DTADDRESS_CLAMP;
        state[NNE_SAMPLERSTATETYPE_MAGFILTER] = D3DTEXF_LINEAR;
        state[NNE_SAMPLERSTATETYPE_MINFILTER] = D3DTEXF_LINEAR;
        state[NNE_SAMPLERSTATETYPE_MIPFILTER] = D3DTEXF_NONE;

        nnSetMaterialControlUserSamplerDXG20(
            NNE_USER_SAMPLER.NNE_USER_SAMPLER_2D_1, render_target.target, state);

        mppSetProjectionUserUniforms();
    }

}