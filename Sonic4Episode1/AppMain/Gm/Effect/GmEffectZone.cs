using System;

internal partial class AppMain
{
    private static readonly int[] gm_efct_zone_texamb_index_tbl;

    // first anim dwork per zone. Contiguous from 511 except zones 2 and 3, which live past the end
    // of the block because PC's anims do not fit where mobile reserved room. See GMD_DWORK_NO
    private static readonly int[] gm_efct_zone_ame_base_tbl =
    {
        511,
        GMD_DWORK_NO_EFFECT_ZONE2_AME_START,
        GMD_DWORK_NO_EFFECT_ZONE3_AME_START,
        558,
        562,
        563,
    };

    // EFF_ZONE3 is sorted by name, so PC's 18 extra anims land among the 36 mobile has rather than
    // after them and every index moves. Keyed by mobile index, measured off both archives
    internal static readonly int[] gm_efct_zone03_pc_ame_idx =
    {
         1,  2,  3,  4,  5,  6,  7,  8,  9, 20,
        21, 22, 23, 24, 25, 26, 27, 28, 29, 30,
        31, 32, 33, 34, 35, 36, 37, 38, 39, 40,
        41, 42, 47, 51, 52, 53,
    };

    private static readonly GMS_EFCT_ZONE_CREATE_PARAM[] gm_efct_zone01_create_param_tbl;
    private static readonly GMS_EFCT_ZONE_CREATE_PARAM[] gm_efct_zone02_create_param_tbl;
    private static readonly GMS_EFCT_ZONE_CREATE_PARAM[] gm_efct_zone02_pc_create_param_tbl;
    private static GMS_EFCT_ZONE_CREATE_INFO gm_efct_zone02_create_info;
    private static GMS_EFCT_ZONE_CREATE_INFO gm_efct_zone02_pc_create_info;
    internal static readonly GMS_EFCT_ZONE_CREATE_PARAM[] gm_efct_zone03_create_param_tbl;
    private static readonly GMS_EFCT_ZONE_CREATE_PARAM[] gm_efct_zone03_pc_extra_create_param_tbl;
    internal static readonly GMS_EFCT_ZONE_CREATE_PARAM[] gm_efct_zone03_pc_create_param_tbl;
    private static GMS_EFCT_ZONE_CREATE_INFO gm_efct_zone03_create_info;
    internal static GMS_EFCT_ZONE_CREATE_INFO gm_efct_zone03_pc_create_info;
    private static readonly GMS_EFCT_ZONE_CREATE_PARAM[] gm_efct_zone04_create_param_tbl;
    private static readonly GMS_EFCT_ZONE_CREATE_PARAM[] gm_efct_zone_final_create_param_tbl;
    private static readonly GMS_EFCT_ZONE_CREATE_PARAM[] gm_efct_zone_ss_create_param_tbl;
    public static readonly GMS_EFCT_ZONE_CREATE_INFO[] gm_efct_zone_create_info;
    private static int gm_efct_zone_proc_state;
    private static int gm_efct_zone_tex_reg_id;
    private static int[] gm_efct_zone_model_reg_id_list;
    private static int gm_efct_zone_model_reg_num;
    private static int[] gm_efct_zone_mdl_tex_reg_id_list;
    private static int gm_efct_zone_target_zone_no;

    private static void GmEfctZoneBuildDataInit(int zone_no)
    {
        gm_efct_zone_proc_state = 1;
        gm_efct_zone_target_zone_no = zone_no;
    }

    private static void GmEfctZoneBuildDataLoopInit()
    {
        object texlist_buf = null;
        int index1 = 0;
        AMS_AMB_HEADER amb = readAMBFile(ObjDataGetInc(ObjDataGet(6)));
        gmEfctZoneSelectVariant(amb);
        GMS_EFCT_ZONE_CREATE_INFO efctZoneCreateInfo = gm_efct_zone_create_info[gm_efct_zone_target_zone_no];
        gm_efct_zone_model_reg_num = GMD_DWORK_NO_EFFECT_ZONE_MODELDAT_END - GMD_DWORK_NO_EFFECT_ZONE_MODELDAT_START;
        if (gm_efct_zone_model_reg_num > 0)
        {
            gm_efct_zone_model_reg_id_list = new int[gm_efct_zone_model_reg_num];
            gm_efct_zone_mdl_tex_reg_id_list = new int[gm_efct_zone_model_reg_num];
            for (int index2 = 0; index2 < gm_efct_zone_model_reg_num; ++index2)
            {
                gm_efct_zone_model_reg_id_list[index2] = -1;
                gm_efct_zone_mdl_tex_reg_id_list[index2] = -1;
            }
        }
        OBS_DATA_WORK data_work1 = ObjDataGet(509);
        ObjDataLoadAmbIndex(data_work1, gmEfctZoneTexAmbIndex(amb), amb);
        gm_efct_zone_tex_reg_id = ObjAction3dESTextureLoadToDwork(ObjDataGet(510), readAMBFile(data_work1.pData), ref texlist_buf);
        for (int index2 = 0; index2 < efctZoneCreateInfo.num; ++index2)
        {
            int modelIdx = efctZoneCreateInfo.zone_create_param[index2].create_param.model_idx;
            int modelDworkNo = efctZoneCreateInfo.zone_create_param[index2].model_dwork_no;
            int objectDworkNo = gmEfctZoneGetObjectDworkNo(modelDworkNo);
            int mdlAmbtexIdx = efctZoneCreateInfo.zone_create_param[index2].mdl_ambtex_idx;
            int mdlAmbtexDworkNo = gmEfctZoneGetMdlAmbtexDworkNo(modelDworkNo);
            int mdlTexlistDworkNo = gmEfctZoneGetMdlTexlistDworkNo(modelDworkNo);
            if (modelIdx != -1)
            {
                OBS_DATA_WORK data_work2 = ObjDataGet(mdlAmbtexDworkNo);
                ObjDataLoadAmbIndex(data_work2, mdlAmbtexIdx, amb);
                gm_efct_zone_mdl_tex_reg_id_list[index1] = ObjAction3dESTextureLoadToDwork(ObjDataGet(mdlTexlistDworkNo), readAMBFile(data_work2.pData), ref texlist_buf);
                ObjDataLoadAmbIndex(ObjDataGet(modelDworkNo), modelIdx, amb);
                gm_efct_zone_model_reg_id_list[index1] = ObjAction3dESModelLoadToDwork(ObjDataGet(objectDworkNo), (AmbChunk)ObjDataGet(modelDworkNo).pData, 0U);
                ++index1;
            }
        }
    }

    private static bool GmEfctZoneBuildDataLoop()
    {
        bool flag = true;
        if (gm_efct_zone_target_zone_no == -1)
            return flag;
        switch (gm_efct_zone_proc_state)
        {
            case 0:
                return true;
            case 1:
                if (GsMainSysGetDisplayListRegistNum() < 208)
                {
                    GmEfctZoneBuildDataLoopInit();
                    gm_efct_zone_proc_state = 2;
                }
                return false;
            default:
                if (gm_efct_zone_tex_reg_id != -1)
                {
                    if (amDrawIsRegistComplete(gm_efct_zone_tex_reg_id))
                        gm_efct_zone_tex_reg_id = -1;
                    else
                        flag = false;
                }
                for (int index = 0; index < gm_efct_zone_model_reg_num; ++index)
                {
                    if (gm_efct_zone_mdl_tex_reg_id_list[index] != -1)
                    {
                        if (amDrawIsRegistComplete(gm_efct_zone_mdl_tex_reg_id_list[index]))
                            gm_efct_zone_mdl_tex_reg_id_list[index] = -1;
                        else
                            flag = false;
                    }
                    if (gm_efct_zone_model_reg_id_list[index] != -1)
                    {
                        if (amDrawIsRegistComplete(gm_efct_zone_model_reg_id_list[index]))
                            gm_efct_zone_model_reg_id_list[index] = -1;
                        else
                            flag = false;
                    }
                }
                if (flag)
                {
                    gm_efct_zone_target_zone_no = -1;
                    gm_efct_zone_proc_state = 0;
                }
                return flag;
        }
    }

    private static void GmEfctZoneFlushDataInit(int zone_no)
    {
        gm_efct_zone_proc_state = 1;
        gm_efct_zone_target_zone_no = zone_no;
    }

    private static void GmEfctZoneFlushDataLoopInit()
    {
        GMS_EFCT_ZONE_CREATE_INFO efctZoneCreateInfo = gm_efct_zone_create_info[gm_efct_zone_target_zone_no];
        int index1 = 0;
        for (int index2 = 0; index2 < efctZoneCreateInfo.num; ++index2)
        {
            GMS_EFCT_ZONE_CREATE_PARAM efctZoneCreateParam = efctZoneCreateInfo.zone_create_param[index2];
            int modelDworkNo = efctZoneCreateParam.model_dwork_no;
            int objectDworkNo = gmEfctZoneGetObjectDworkNo(modelDworkNo);
            if (efctZoneCreateParam.create_param.model_idx != -1)
            {
                gm_efct_zone_model_reg_id_list[index1] = ObjAction3dESModelReleaseDwork(ObjDataGet(objectDworkNo));
                ObjDataRelease(ObjDataGet(modelDworkNo));
                gm_efct_zone_mdl_tex_reg_id_list[index1] = ObjAction3dESTextureReleaseDwork(ObjDataGet(gmEfctZoneGetMdlTexlistDworkNo(modelDworkNo)));
                ObjDataRelease(ObjDataGet(gmEfctZoneGetMdlAmbtexDworkNo(modelDworkNo)));
                ++index1;
            }
        }
        gm_efct_zone_tex_reg_id = ObjAction3dESTextureReleaseDwork(ObjDataGet(510));
        ObjDataRelease(ObjDataGet(509));
        ObjDataRelease(ObjDataGet(6));
    }

    private static bool GmEfctZoneFlushDataLoop()
    {
        bool flag = true;
        int index1 = 0;
        switch (gm_efct_zone_proc_state)
        {
            case 0:
                return true;
            case 1:
                if (GsMainSysGetDisplayListRegistNum() < 248)
                {
                    GmEfctZoneFlushDataLoopInit();
                    gm_efct_zone_proc_state = 2;
                }
                return false;
            default:
                GMS_EFCT_ZONE_CREATE_INFO efctZoneCreateInfo = gm_efct_zone_create_info[gm_efct_zone_target_zone_no];
                if (gm_efct_zone_model_reg_num != 0)
                {
                    for (int index2 = 0; index2 < efctZoneCreateInfo.num; ++index2)
                    {
                        GMS_EFCT_ZONE_CREATE_PARAM efctZoneCreateParam = efctZoneCreateInfo.zone_create_param[index2];
                        int modelDworkNo = efctZoneCreateParam.model_dwork_no;
                        int objectDworkNo = gmEfctZoneGetObjectDworkNo(modelDworkNo);
                        if (efctZoneCreateParam.create_param.model_idx != -1)
                        {
                            if (gm_efct_zone_model_reg_id_list[index1] != -1)
                            {
                                if (ObjAction3dESModelReleaseDworkCheck(ObjDataGet(objectDworkNo), gm_efct_zone_model_reg_id_list[index1]))
                                    gm_efct_zone_model_reg_id_list[index1] = -1;
                                else
                                    flag = false;
                            }
                            if (gm_efct_zone_mdl_tex_reg_id_list[index1] != -1)
                            {
                                if (ObjAction3dESTextureReleaseDworkCheck(ObjDataGet(gmEfctZoneGetMdlTexlistDworkNo(modelDworkNo)), gm_efct_zone_mdl_tex_reg_id_list[index1]))
                                    gm_efct_zone_mdl_tex_reg_id_list[index1] = -1;
                                else
                                    flag = false;
                            }
                            ++index1;
                        }
                    }
                }
                if (gm_efct_zone_tex_reg_id != -1)
                {
                    if (ObjAction3dESTextureReleaseDworkCheck(ObjDataGet(510), gm_efct_zone_tex_reg_id))
                        gm_efct_zone_tex_reg_id = -1;
                    else
                        flag = false;
                }
                if (flag)
                {
                    if (gm_efct_zone_mdl_tex_reg_id_list != null)
                        gm_efct_zone_mdl_tex_reg_id_list = null;
                    if (gm_efct_zone_model_reg_id_list != null)
                    {
                        gm_efct_zone_model_reg_id_list = null;
                        gm_efct_zone_model_reg_num = 0;
                    }
                    gm_efct_zone_target_zone_no = -1;
                    gm_efct_zone_proc_state = 0;
                }
                return flag;
        }
    }

    private static GMS_EFFECT_3DES_WORK GmEfctZoneEsCreate(
      OBS_OBJECT_WORK parent_obj,
      int zone_no,
      int efct_zone_idx)
    {
        GMS_EFCT_ZONE_CREATE_PARAM efctZoneCreateParam = gm_efct_zone_create_info[zone_no].zone_create_param[efct_zone_idx];
        OBS_DATA_WORK model_dwork;
        OBS_DATA_WORK object_dwork;
        OBS_DATA_WORK ambtex_dwork;
        OBS_DATA_WORK texlist_dwork;
        if (efctZoneCreateParam.create_param.model_idx != -1)
        {
            int modelDworkNo = efctZoneCreateParam.model_dwork_no;
            int objectDworkNo = gmEfctZoneGetObjectDworkNo(modelDworkNo);
            model_dwork = ObjDataGet(modelDworkNo);
            object_dwork = ObjDataGet(objectDworkNo);
            ambtex_dwork = ObjDataGet(gmEfctZoneGetMdlAmbtexDworkNo(modelDworkNo));
            texlist_dwork = ObjDataGet(gmEfctZoneGetMdlTexlistDworkNo(modelDworkNo));
        }
        else
        {
            model_dwork = null;
            object_dwork = null;
            ambtex_dwork = ObjDataGet(509);
            texlist_dwork = ObjDataGet(510);
        }
        return GmEffect3dESCreateByParam(efctZoneCreateParam.create_param, parent_obj, ObjDataGet(6).pData, ObjDataGet(gmEfctZoneGetAmeDworkNo(zone_no, efctZoneCreateParam.create_param.ame_idx)), ambtex_dwork, texlist_dwork, model_dwork, object_dwork);
    }

    // every zone effect archive is its anims plus one texture, texture last, so the anim count and
    // the texture's member index are the same number. Taken from the archive rather than the table
    // so a zone reads correctly whichever variant resolved
    // every zone's texture archive is EFF_ZONE<n>_TEX.AMB and every archive but one ends with it.
    // PC's EFF_ZONE3 appends three effect models and their own EFF_ZONE3_TEX_MD_WALL.AMB after it,
    // so the last member is the wrong archive there
    private static int gmEfctZoneTexAmbIndex(AMS_AMB_HEADER amb)
    {
        for (int index = amb.file_num - 1; index >= 0; --index)
        {
            var name = mppAmbMemberBaseName(amb.files[index]);
            if (name != null && name.EndsWith("_TEX", StringComparison.OrdinalIgnoreCase))
                return index;
        }

        return amb.file_num - 1;
    }

    // zone 2 ships two ways round: mobile's 2 anims or PC's 8. The rest of the table is the same
    // either way, so this only has to pick how many of it are real.
    // zone 3 is the other shape: the same 36 effects both ways, at different member indices,
    // because EFF_ZONE3 is sorted by name and PC's 18 additions land among them
    private static void gmEfctZoneSelectVariant(AMS_AMB_HEADER amb)
    {
        if (gm_efct_zone_target_zone_no == GSD_MAIN_ZONE_TYPE_2)
        {
            gm_efct_zone_create_info[GSD_MAIN_ZONE_TYPE_2] =
                gmEfctZoneTexAmbIndex(amb) >= gm_efct_zone02_pc_create_param_tbl.Length
                    ? gm_efct_zone02_pc_create_info
                    : gm_efct_zone02_create_info;
        }
        else if (gm_efct_zone_target_zone_no == GSD_MAIN_ZONE_TYPE_3)
        {
            // mobile's archive is exactly those 36 anims plus its texture; PC's carries 18 more
            gm_efct_zone_create_info[GSD_MAIN_ZONE_TYPE_3] =
                amb.file_num > gm_efct_zone03_create_param_tbl.Length + 1
                    ? gm_efct_zone03_pc_create_info
                    : gm_efct_zone03_create_info;
        }
    }

    // the PC table is the mobile one with every ame_idx moved to where PC's archive keeps that
    // effect. Written out rather than transcribed so the two cannot drift apart
    internal static GMS_EFCT_ZONE_CREATE_PARAM[] gmEfctZoneRemapAmeIndices(
      GMS_EFCT_ZONE_CREATE_PARAM[] table,
      int[] ame_idx_map)
    {
        var remapped = new GMS_EFCT_ZONE_CREATE_PARAM[table.Length];

        for (int index = 0; index < table.Length; ++index)
        {
            var row = table[index];
            var param = row.create_param;

            remapped[index] = new GMS_EFCT_ZONE_CREATE_PARAM(
                new GMS_EFFECT_CREATE_PARAM(
                    ame_idx_map[param.ame_idx], param.pos_type, param.init_flag, param.disp_ofst,
                    param.disp_rot, param.scale, param.main_func, param.model_idx),
                row.model_dwork_no,
                row.mdl_ambtex_idx);
        }

        return remapped;
    }

    private static int gmEfctZoneGetAmeDworkNo(int zone_no, int ame_amb_idx)
    {
        return gm_efct_zone_ame_base_tbl[zone_no] + ame_amb_idx;
    }

    internal static int gmEfctZoneGetObjectDworkNo(int model_dwork_no)
    {
        return GMD_DWORK_NO_EFFECT_ZONE_OBJECT_START + (model_dwork_no - GMD_DWORK_NO_EFFECT_ZONE_MODELDAT_START);
    }

    // the leak subtracts this block's own start here and in the texlist one below, which returns
    // model_dwork_no unchanged. PC does it right, and nothing ever caught it because no zone table
    // in the mobile line has a model row
    internal static int gmEfctZoneGetMdlAmbtexDworkNo(int model_dwork_no)
    {
        return GMD_DWORK_NO_EFFECT_ZONE_MDL_AMBTEX_START + (model_dwork_no - GMD_DWORK_NO_EFFECT_ZONE_MODELDAT_START);
    }

    internal static int gmEfctZoneGetMdlTexlistDworkNo(int model_dwork_no)
    {
        return GMD_DWORK_NO_EFFECT_ZONE_MDL_TEXLIST_START + (model_dwork_no - GMD_DWORK_NO_EFFECT_ZONE_MODELDAT_START);
    }
}