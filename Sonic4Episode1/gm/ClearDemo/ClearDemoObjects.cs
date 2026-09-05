using static AppMain;

internal static partial class GmClearDemo
{
    private static void gmClearDemoCreateObjActScore(GMS_CLRDM_MAIN_WORK main_work)
    {
        TaskWorkFactoryDelegate work_size = () => new GMS_COCKPIT_2D_WORK();
        OBS_OBJECT_WORK obsObjectWork;
        for (int index = 0; index < 5; ++index)
        {
            OBS_OBJECT_WORK work1 = GmCockpit.GMM_COCKPIT_CREATE_WORK(work_size, null, 0, "CLRDM_UPTEXT");
            main_work.tex_up_act[index] = (GMS_COCKPIT_2D_WORK) work1;
            obsObjectWork = null;
            OBS_OBJECT_WORK work2 = GmCockpit.GMM_COCKPIT_CREATE_WORK(work_size, null, 0, "CLRDM_TIMENUM");
            main_work.time_num_act[index] = (GMS_COCKPIT_2D_WORK) work2;
            obsObjectWork = null;
            OBS_OBJECT_WORK work3 = GmCockpit.GMM_COCKPIT_CREATE_WORK(work_size, null, 0, "CLRDM_RINGNUM");
            main_work.ring_num_act[index] = (GMS_COCKPIT_2D_WORK) work3;
            obsObjectWork = null;
        }

        for (int index = 0; index < 9; ++index)
        {
            OBS_OBJECT_WORK work = GmCockpit.GMM_COCKPIT_CREATE_WORK(work_size, null, 0, "CLRDM_TOTALNUM");
            main_work.total_num_act[index] = (GMS_COCKPIT_2D_WORK) work;
            obsObjectWork = null;
        }

        for (int index = 0; index < 3; ++index)
        {
            OBS_OBJECT_WORK work = GmCockpit.GMM_COCKPIT_CREATE_WORK(work_size, null, 0, "CLRDM_LINE");
            main_work.line_act[index] = (GMS_COCKPIT_2D_WORK) work;
            obsObjectWork = null;
        }

        OBS_OBJECT_WORK work4 = GmCockpit.GMM_COCKPIT_CREATE_WORK(work_size, null, 0, "CLRDM_TIMETEXT");
        main_work.tex_time_act = (GMS_COCKPIT_2D_WORK) work4;
        obsObjectWork = null;
        OBS_OBJECT_WORK work5 = GmCockpit.GMM_COCKPIT_CREATE_WORK(work_size, null, 0, "CLRDM_RINGTEXT");
        main_work.tex_ring_act = (GMS_COCKPIT_2D_WORK) work5;
        obsObjectWork = null;
        OBS_OBJECT_WORK work6 = GmCockpit.GMM_COCKPIT_CREATE_WORK(work_size, null, 0, "CLRDM_TOTALTEXT");
        main_work.tex_total_act = (GMS_COCKPIT_2D_WORK) work6;
        obsObjectWork = null;
        OBS_OBJECT_WORK work7 = GmCockpit.GMM_COCKPIT_CREATE_WORK(work_size, null, 0, "CLRDM_SONICICON");
        main_work.sonic_icon_act = (GMS_COCKPIT_2D_WORK) work7;
        obsObjectWork = null;
        OBS_OBJECT_WORK work8 = GmCockpit.GMM_COCKPIT_CREATE_WORK(work_size, null, 0, "CLRDM_SONICICON2");
        main_work.sonic_icon_act2 = (GMS_COCKPIT_2D_WORK) work8;
        obsObjectWork = null;
        gmClearDemoCreateObjNextPromptWork(main_work);
    }

    private static void gmClearDemoCreateObjActTime(GMS_CLRDM_MAIN_WORK main_work)
    {
        TaskWorkFactoryDelegate work_size = () => new GMS_COCKPIT_2D_WORK();
        OBS_OBJECT_WORK obsObjectWork;
        for (int index = 0; index < 5; ++index)
        {
            OBS_OBJECT_WORK work = GmCockpit.GMM_COCKPIT_CREATE_WORK(work_size, null, 0, "CLRDM_UPTEXT");
            main_work.tex_up_act[index] = (GMS_COCKPIT_2D_WORK) work;
            obsObjectWork = null;
        }

        OBS_OBJECT_WORK work1 = GmCockpit.GMM_COCKPIT_CREATE_WORK(work_size, null, 0, "CLRDM_TIMEATK_TEXT");
        main_work.tex_big_time_act = (GMS_COCKPIT_2D_WORK) work1;
        obsObjectWork = null;
        for (int index = 0; index < 7; ++index)
        {
            OBS_OBJECT_WORK work2 = GmCockpit.GMM_COCKPIT_CREATE_WORK(work_size, null, 0, "CLRDM_TIME_DIGIT_NUM");
            main_work.record_time_num_act[index] = (GMS_COCKPIT_2D_WORK) work2;
            obsObjectWork = null;
        }

        OBS_OBJECT_WORK work3 = GmCockpit.GMM_COCKPIT_CREATE_WORK(work_size, null, 0, "CLRDM_TIMEATK_SONIC");
        main_work.time_sonic_icon_act = (GMS_COCKPIT_2D_WORK) work3;
        obsObjectWork = null;
        OBS_OBJECT_WORK work4 = GmCockpit.GMM_COCKPIT_CREATE_WORK(work_size, null, 0, "CLRDM_NEWRECORD_TEXT");
        main_work.tex_new_record_act = (GMS_COCKPIT_2D_WORK) work4;
        obsObjectWork = null;
        OBS_OBJECT_WORK work5 = GmCockpit.GMM_COCKPIT_CREATE_WORK(work_size, null, 0, "CLRDM_BG_RETRY");
        main_work.bg_retry = (GMS_COCKPIT_2D_WORK) work5;
        obsObjectWork = null;
        OBS_OBJECT_WORK work6 = GmCockpit.GMM_COCKPIT_CREATE_WORK(work_size, null, 0, "CLRDM_RETRY_TEXT");
        main_work.tex_retry_act = (GMS_COCKPIT_2D_WORK) work6;
        obsObjectWork = null;
        OBS_OBJECT_WORK work7 = GmCockpit.GMM_COCKPIT_CREATE_WORK(work_size, null, 0, "CLRDM_ACT_BACK_TEXT");
        main_work.tex_back_slct_act = (GMS_COCKPIT_2D_WORK) work7;
        obsObjectWork = null;
        int index1 = 0;
        for (int index2 = arrayof(main_work.btn_retry); index1 < index2; ++index1)
        {
            OBS_OBJECT_WORK work2 = GmCockpit.GMM_COCKPIT_CREATE_WORK(work_size, null, 0, "CLRDM_RETRY_BTN");
            main_work.btn_retry[index1] = (GMS_COCKPIT_2D_WORK) work2;
            obsObjectWork = null;
        }

        int index3 = 0;
        for (int index2 = arrayof(main_work.btn_back); index3 < index2; ++index3)
        {
            OBS_OBJECT_WORK work2 = GmCockpit.GMM_COCKPIT_CREATE_WORK(work_size, null, 0, "CLRDM_ACT_BACK_BTN");
            main_work.btn_back[index3] = (GMS_COCKPIT_2D_WORK) work2;
            obsObjectWork = null;
        }
    }

    private static void gmClearDemoCreateObjSpeScore(GMS_CLRDM_MAIN_WORK main_work)
    {
        TaskWorkFactoryDelegate work_size = () => new GMS_COCKPIT_2D_WORK();
        OBS_OBJECT_WORK obsObjectWork;
        for (int index = 0; index < 3; ++index)
        {
            OBS_OBJECT_WORK work = GmCockpit.GMM_COCKPIT_CREATE_WORK(work_size, null, 0, "CLRDM_UP_SPST_TEXT");
            main_work.tex_spst_up_act[index] = (GMS_COCKPIT_2D_WORK) work;
            obsObjectWork = null;
        }

        for (int index = 0; index < 7; ++index)
        {
            OBS_OBJECT_WORK work1 = GmCockpit.GMM_COCKPIT_CREATE_WORK(work_size, null, 0, "CLRDM_SPE_STAGE_NO");
            main_work.spst_num_act[index] = (GMS_COCKPIT_2D_WORK) work1;
            obsObjectWork = null;
            OBS_OBJECT_WORK work2 = GmCockpit.GMM_COCKPIT_CREATE_WORK(work_size, null, 0, "CLRDM_EMER_UP_ICON");
            main_work.icon_emer_up_act[index] = (GMS_COCKPIT_2D_WORK) work2;
            obsObjectWork = null;
            OBS_OBJECT_WORK work3 = GmCockpit.GMM_COCKPIT_CREATE_WORK(work_size, null, 0, "CLRDM_EMER_DOWN_ICON");
            main_work.icon_emer_down_act[index] = (GMS_COCKPIT_2D_WORK) work3;
            obsObjectWork = null;
        }

        OBS_OBJECT_WORK work4 = GmCockpit.GMM_COCKPIT_CREATE_WORK(work_size, null, 0, "CLRDM_LIGHT_EMER");
        main_work.icon_emer_light_act = (GMS_COCKPIT_2D_WORK) work4;
        obsObjectWork = null;
        OBS_OBJECT_WORK work5 = GmCockpit.GMM_COCKPIT_CREATE_WORK(work_size, null, 0, "CLRDM_EXTEND_TEXT");
        main_work.tex_extend_act = (GMS_COCKPIT_2D_WORK) work5;
        obsObjectWork = null;
        for (int index = 0; index < 5; ++index)
        {
            OBS_OBJECT_WORK work1 = GmCockpit.GMM_COCKPIT_CREATE_WORK(work_size, null, 0, "CLRDM_TIMENUM");
            main_work.time_num_act[index] = (GMS_COCKPIT_2D_WORK) work1;
            obsObjectWork = null;
            OBS_OBJECT_WORK work2 = GmCockpit.GMM_COCKPIT_CREATE_WORK(work_size, null, 0, "CLRDM_RINGNUM");
            main_work.ring_num_act[index] = (GMS_COCKPIT_2D_WORK) work2;
            obsObjectWork = null;
        }

        for (int index = 0; index < 9; ++index)
        {
            OBS_OBJECT_WORK work1 = GmCockpit.GMM_COCKPIT_CREATE_WORK(work_size, null, 0, "CLRDM_TOTALNUM");
            main_work.total_num_act[index] = (GMS_COCKPIT_2D_WORK) work1;
            obsObjectWork = null;
        }

        for (int index = 0; index < 3; ++index)
        {
            OBS_OBJECT_WORK work1 = GmCockpit.GMM_COCKPIT_CREATE_WORK(work_size, null, 0, "CLRDM_LINE");
            main_work.line_act[index] = (GMS_COCKPIT_2D_WORK) work1;
            obsObjectWork = null;
        }

        OBS_OBJECT_WORK work6 = GmCockpit.GMM_COCKPIT_CREATE_WORK(work_size, null, 0, "CLRDM_TIMETEXT");
        main_work.tex_time_act = (GMS_COCKPIT_2D_WORK) work6;
        obsObjectWork = null;
        OBS_OBJECT_WORK work7 = GmCockpit.GMM_COCKPIT_CREATE_WORK(work_size, null, 0, "CLRDM_RINGTEXT");
        main_work.tex_ring_act = (GMS_COCKPIT_2D_WORK) work7;
        obsObjectWork = null;
        OBS_OBJECT_WORK work8 = GmCockpit.GMM_COCKPIT_CREATE_WORK(work_size, null, 0, "CLRDM_TOTALTEXT");
        main_work.tex_total_act = (GMS_COCKPIT_2D_WORK) work8;
        obsObjectWork = null;
        OBS_OBJECT_WORK work9 = GmCockpit.GMM_COCKPIT_CREATE_WORK(work_size, null, 0, "CLRDM_SONICICON");
        main_work.sonic_icon_act = (GMS_COCKPIT_2D_WORK) work9;
        obsObjectWork = null;
        OBS_OBJECT_WORK work10 = GmCockpit.GMM_COCKPIT_CREATE_WORK(work_size, null, 0, "CLRDM_SONICICON2");
        main_work.sonic_icon_act2 = (GMS_COCKPIT_2D_WORK) work10;
        obsObjectWork = null;
        gmClearDemoCreateObjNextPromptWork(main_work);
    }

    private static void gmClearDemoCreateObjSpeTime(GMS_CLRDM_MAIN_WORK main_work)
    {
        TaskWorkFactoryDelegate work_size = () => new GMS_COCKPIT_2D_WORK();
        OBS_OBJECT_WORK obsObjectWork;
        for (int index = 0; index < 3; ++index)
        {
            OBS_OBJECT_WORK work = GmCockpit.GMM_COCKPIT_CREATE_WORK(work_size, null, 0, "CLRDM_UP_SPST_TEXT");
            main_work.tex_spst_up_act[index] = (GMS_COCKPIT_2D_WORK) work;
            obsObjectWork = null;
        }

        for (int index = 0; index < 7; ++index)
        {
            OBS_OBJECT_WORK work1 = GmCockpit.GMM_COCKPIT_CREATE_WORK(work_size, null, 0, "CLRDM_SPE_STAGE_NO");
            main_work.spst_num_act[index] = (GMS_COCKPIT_2D_WORK) work1;
            obsObjectWork = null;
            OBS_OBJECT_WORK work2 = GmCockpit.GMM_COCKPIT_CREATE_WORK(work_size, null, 0, "CLRDM_EMER_UP_ICON");
            main_work.icon_emer_up_act[index] = (GMS_COCKPIT_2D_WORK) work2;
            obsObjectWork = null;
            OBS_OBJECT_WORK work3 = GmCockpit.GMM_COCKPIT_CREATE_WORK(work_size, null, 0, "CLRDM_EMER_DOWN_ICON");
            main_work.icon_emer_down_act[index] = (GMS_COCKPIT_2D_WORK) work3;
            obsObjectWork = null;
        }

        OBS_OBJECT_WORK work4 = GmCockpit.GMM_COCKPIT_CREATE_WORK(work_size, null, 0, "CLRDM_LIGHT_EMER");
        main_work.icon_emer_light_act = (GMS_COCKPIT_2D_WORK) work4;
        obsObjectWork = null;
        OBS_OBJECT_WORK work5 = GmCockpit.GMM_COCKPIT_CREATE_WORK(work_size, null, 0, "CLRDM_TIMEATK_TEXT");
        main_work.tex_big_time_act = (GMS_COCKPIT_2D_WORK) work5;
        obsObjectWork = null;
        for (int index = 0; index < 7; ++index)
        {
            OBS_OBJECT_WORK work1 = GmCockpit.GMM_COCKPIT_CREATE_WORK(work_size, null, 0, "CLRDM_TIME_DIGIT_NUM");
            main_work.record_time_num_act[index] = (GMS_COCKPIT_2D_WORK) work1;
            obsObjectWork = null;
        }

        OBS_OBJECT_WORK work6 = GmCockpit.GMM_COCKPIT_CREATE_WORK(work_size, null, 0, "CLRDM_TIMEATK_SONIC");
        main_work.time_sonic_icon_act = (GMS_COCKPIT_2D_WORK) work6;
        obsObjectWork = null;
        OBS_OBJECT_WORK work7 = GmCockpit.GMM_COCKPIT_CREATE_WORK(work_size, null, 0, "CLRDM_NEWRECORD_TEXT");
        main_work.tex_new_record_act = (GMS_COCKPIT_2D_WORK) work7;
        obsObjectWork = null;
        OBS_OBJECT_WORK work8 = GmCockpit.GMM_COCKPIT_CREATE_WORK(work_size, null, 0, "CLRDM_BG_RETRY");
        main_work.bg_retry = (GMS_COCKPIT_2D_WORK) work8;
        obsObjectWork = null;
        OBS_OBJECT_WORK work9 = GmCockpit.GMM_COCKPIT_CREATE_WORK(work_size, null, 0, "CLRDM_RETRY_TEXT");
        main_work.tex_retry_act = (GMS_COCKPIT_2D_WORK) work9;
        obsObjectWork = null;
        OBS_OBJECT_WORK work10 = GmCockpit.GMM_COCKPIT_CREATE_WORK(work_size, null, 0, "CLRDM_ACT_BACK_TEXT");
        main_work.tex_back_slct_act = (GMS_COCKPIT_2D_WORK) work10;
        obsObjectWork = null;
        int index1 = 0;
        for (int index2 = arrayof(main_work.btn_retry); index1 < index2; ++index1)
        {
            OBS_OBJECT_WORK work1 = GmCockpit.GMM_COCKPIT_CREATE_WORK(work_size, null, 0, "CLRDM_RETRY_BTN");
            main_work.btn_retry[index1] = (GMS_COCKPIT_2D_WORK) work1;
            obsObjectWork = null;
        }

        int index3 = 0;
        for (int index2 = arrayof(main_work.btn_back); index3 < index2; ++index3)
        {
            OBS_OBJECT_WORK work1 = GmCockpit.GMM_COCKPIT_CREATE_WORK(work_size, null, 0, "CLRDM_ACT_BACK_BTN");
            main_work.btn_back[index3] = (GMS_COCKPIT_2D_WORK) work1;
            obsObjectWork = null;
        }
    }

    private static void gmClearDemoCreateObjAct(GMS_CLRDM_MAIN_WORK main_work)
    {
        gmClearDemoCreateObjActForStage(main_work);
        for (int index = 0; index < 5; ++index)
        {
            ObjObjectAction2dAMALoadSetTexlist((OBS_OBJECT_WORK) main_work.time_num_act[index], main_work.time_num_act[index].obj_2d, null, null, 30, GmGameDatGetCockpitData(), AoTexGetTexList(main_work.tex[0]), (uint) (4 + index), 0);
            ((OBS_OBJECT_WORK) main_work.time_num_act[index]).scale.x = 4096;
            ((OBS_OBJECT_WORK) main_work.time_num_act[index]).scale.y = 4096;
            ObjObjectAction2dAMALoadSetTexlist((OBS_OBJECT_WORK) main_work.ring_num_act[index], main_work.ring_num_act[index].obj_2d, null, null, 30, GmGameDatGetCockpitData(), AoTexGetTexList(main_work.tex[0]), (uint) (9 + index), 0);
            ((OBS_OBJECT_WORK) main_work.ring_num_act[index]).scale.x = 4096;
            ((OBS_OBJECT_WORK) main_work.ring_num_act[index]).scale.y = 4096;
        }

        for (int index = 0; index < 9; ++index)
        {
            ObjObjectAction2dAMALoadSetTexlist((OBS_OBJECT_WORK) main_work.total_num_act[index], main_work.total_num_act[index].obj_2d, null, null, 30, GmGameDatGetCockpitData(), AoTexGetTexList(main_work.tex[0]), (uint) (18 + index), 0);
            ((OBS_OBJECT_WORK) main_work.total_num_act[index]).scale.x = 4096;
            ((OBS_OBJECT_WORK) main_work.total_num_act[index]).scale.y = 4096;
        }

        for (int index = 0; index < 3; ++index)
        {
            ObjObjectAction2dAMALoadSetTexlist((OBS_OBJECT_WORK) main_work.line_act[index], main_work.line_act[index].obj_2d, null, null, 30, GmGameDatGetCockpitData(), AoTexGetTexList(main_work.tex[0]), (uint) (14 + index), 0);
            ((OBS_OBJECT_WORK) main_work.line_act[index]).scale.x = 4096;
            ((OBS_OBJECT_WORK) main_work.line_act[index]).scale.y = 4096;
        }

        ObjObjectAction2dAMALoadSetTexlist((OBS_OBJECT_WORK) main_work.tex_time_act, main_work.tex_time_act.obj_2d, null, null, g_gm_clear_demo_data_ama_id[gmClearDemoLanguage()], GmGameDatGetCockpitData(), AoTexGetTexList(main_work.tex[1]), 7U, 0);
        ((OBS_OBJECT_WORK) main_work.tex_time_act).scale.x = 4096;
        ((OBS_OBJECT_WORK) main_work.tex_time_act).scale.y = 4096;
        ObjObjectAction2dAMALoadSetTexlist((OBS_OBJECT_WORK) main_work.tex_ring_act, main_work.tex_ring_act.obj_2d, null, null, g_gm_clear_demo_data_ama_id[gmClearDemoLanguage()], GmGameDatGetCockpitData(), AoTexGetTexList(main_work.tex[1]), 8U, 0);
        ((OBS_OBJECT_WORK) main_work.tex_ring_act).scale.x = 4096;
        ((OBS_OBJECT_WORK) main_work.tex_ring_act).scale.y = 4096;
        ObjObjectAction2dAMALoadSetTexlist((OBS_OBJECT_WORK) main_work.tex_total_act, main_work.tex_total_act.obj_2d, null, null, g_gm_clear_demo_data_ama_id[gmClearDemoLanguage()], GmGameDatGetCockpitData(), AoTexGetTexList(main_work.tex[1]), 9U, 0);
        ((OBS_OBJECT_WORK) main_work.tex_total_act).scale.x = 4096;
        ((OBS_OBJECT_WORK) main_work.tex_total_act).scale.y = 4096;
        ObjObjectAction2dAMALoadSetTexlist((OBS_OBJECT_WORK) main_work.sonic_icon_act, main_work.sonic_icon_act.obj_2d, null, null, 30, GmGameDatGetCockpitData(), AoTexGetTexList(main_work.tex[0]), 17U, 0);
        ((OBS_OBJECT_WORK) main_work.sonic_icon_act).scale.x = 4096;
        ((OBS_OBJECT_WORK) main_work.sonic_icon_act).scale.y = 4096;
        gmClearDemoCreateObjNextPromptAct(main_work);
    }

    private static void gmClearDemoCreateObjNormalTimeAtk(GMS_CLRDM_MAIN_WORK main_work)
    {
        gmClearDemoCreateObjActForStage(main_work);
        ObjObjectAction2dAMALoadSetTexlist((OBS_OBJECT_WORK) main_work.tex_big_time_act, main_work.tex_big_time_act.obj_2d, null, null, g_gm_clear_demo_data_ama_id[gmClearDemoLanguage()], GmGameDatGetCockpitData(), AoTexGetTexList(main_work.tex[1]), 15U, 0);
        ((OBS_OBJECT_WORK) main_work.tex_big_time_act).scale.x = 4096;
        ((OBS_OBJECT_WORK) main_work.tex_big_time_act).scale.y = 4096;
        ObjObjectAction2dAMALoadSetTexlist((OBS_OBJECT_WORK) main_work.time_sonic_icon_act, main_work.time_sonic_icon_act.obj_2d, null, null, 30, GmGameDatGetCockpitData(), AoTexGetTexList(main_work.tex[0]), 56U, 0);
        ((OBS_OBJECT_WORK) main_work.time_sonic_icon_act).scale.x = 4096;
        ((OBS_OBJECT_WORK) main_work.time_sonic_icon_act).scale.y = 4096;
        main_work.time_sonic_icon_act.obj_2d.frame = GMM_MAIN_USE_SUPER_SONIC() ? 1f : 0.0f;
        main_work.time_sonic_icon_act.obj_2d.speed = 0.0f;
        ObjObjectAction2dAMALoadSetTexlist((OBS_OBJECT_WORK) main_work.tex_new_record_act, main_work.tex_new_record_act.obj_2d, null, null, g_gm_clear_demo_data_ama_id[gmClearDemoLanguage()], GmGameDatGetCockpitData(), AoTexGetTexList(main_work.tex[1]), 14U, 0);
        ((OBS_OBJECT_WORK) main_work.tex_new_record_act).scale.x = 4096;
        ((OBS_OBJECT_WORK) main_work.tex_new_record_act).scale.y = 4096;
    }

    private static void gmClearDemoCreateObjSpecialScoreAtk(GMS_CLRDM_MAIN_WORK main_work)
    {
        ObjObjectAction2dAMALoadSetTexlist((OBS_OBJECT_WORK) main_work.tex_spst_up_act[0], main_work.tex_spst_up_act[0].obj_2d, null, null, g_gm_clear_demo_data_ama_id[gmClearDemoLanguage()], GmGameDatGetCockpitData(), AoTexGetTexList(main_work.tex[1]), 11U, 0);
        ((OBS_OBJECT_WORK) main_work.tex_spst_up_act[0]).scale.x = 4096;
        ((OBS_OBJECT_WORK) main_work.tex_spst_up_act[0]).scale.y = 4096;
        if (main_work.is_get_eme && main_work.is_first_spe_clear)
        {
            ObjObjectAction2dAMALoadSetTexlist((OBS_OBJECT_WORK) main_work.tex_spst_up_act[1], main_work.tex_spst_up_act[1].obj_2d, null, null, g_gm_clear_demo_data_ama_id[gmClearDemoLanguage()], GmGameDatGetCockpitData(), AoTexGetTexList(main_work.tex[1]), 12U, 0);
            ((OBS_OBJECT_WORK) main_work.tex_spst_up_act[1]).scale.x = 4096;
            ((OBS_OBJECT_WORK) main_work.tex_spst_up_act[1]).scale.y = 4096;
        }

        ObjObjectAction2dAMALoadSetTexlist((OBS_OBJECT_WORK) main_work.tex_spst_up_act[2], main_work.tex_spst_up_act[2].obj_2d, null, null, g_gm_clear_demo_data_ama_id[gmClearDemoLanguage()], GmGameDatGetCockpitData(), AoTexGetTexList(main_work.tex[1]), 13U, 0);
        ((OBS_OBJECT_WORK) main_work.tex_spst_up_act[2]).scale.x = 4096;
        ((OBS_OBJECT_WORK) main_work.tex_spst_up_act[2]).scale.y = 4096;
        gmClearDemoCreateObjSpeActForStage(main_work);
        for (int index = 0; index < main_work.has_eme_num; ++index)
        {
            if (main_work.is_get_eme && main_work.is_first_spe_clear)
            {
                ObjObjectAction2dAMALoadSetTexlist((OBS_OBJECT_WORK) main_work.icon_emer_down_act[index], main_work.icon_emer_down_act[index].obj_2d, null, null, 30, GmGameDatGetCockpitData(), AoTexGetTexList(main_work.tex[0]), (uint) (34 + index), 0);
                ((OBS_OBJECT_WORK) main_work.icon_emer_down_act[index]).scale.x = 4096;
                ((OBS_OBJECT_WORK) main_work.icon_emer_down_act[index]).scale.y = 4096;
            }
            else
            {
                ObjObjectAction2dAMALoadSetTexlist((OBS_OBJECT_WORK) main_work.icon_emer_up_act[index], main_work.icon_emer_up_act[index].obj_2d, null, null, 30, GmGameDatGetCockpitData(), AoTexGetTexList(main_work.tex[0]), (uint) (42 + index), 0);
                ((OBS_OBJECT_WORK) main_work.icon_emer_up_act[index]).scale.x = 4096;
                ((OBS_OBJECT_WORK) main_work.icon_emer_up_act[index]).scale.y = 4096;
            }
        }

        for (int index = 0; index < 5; ++index)
        {
            ObjObjectAction2dAMALoadSetTexlist((OBS_OBJECT_WORK) main_work.time_num_act[index], main_work.time_num_act[index].obj_2d, null, null, 30, GmGameDatGetCockpitData(), AoTexGetTexList(main_work.tex[0]), (uint) (57 + index), 0);
            main_work.time_num_act[index].obj_2d.ama.act_tbl[(int) main_work.time_num_act[index].obj_2d.act_id].mtn.trs_tbl[0].trs_y -= 15f;
            ((OBS_OBJECT_WORK) main_work.time_num_act[index]).scale.x = 4096;
            ((OBS_OBJECT_WORK) main_work.time_num_act[index]).scale.y = 4096;
            ObjObjectAction2dAMALoadSetTexlist((OBS_OBJECT_WORK) main_work.ring_num_act[index], main_work.ring_num_act[index].obj_2d, null, null, 30, GmGameDatGetCockpitData(), AoTexGetTexList(main_work.tex[0]), (uint) (62 + index), 0);
            ((OBS_OBJECT_WORK) main_work.ring_num_act[index]).scale.x = 4096;
            ((OBS_OBJECT_WORK) main_work.ring_num_act[index]).scale.y = 4096;
        }

        for (int index = 0; index < 9; ++index)
        {
            ObjObjectAction2dAMALoadSetTexlist((OBS_OBJECT_WORK) main_work.total_num_act[index], main_work.total_num_act[index].obj_2d, null, null, 30, GmGameDatGetCockpitData(), AoTexGetTexList(main_work.tex[0]), (uint) (72 + index), 0);
            ((OBS_OBJECT_WORK) main_work.total_num_act[index]).scale.x = 4096;
            ((OBS_OBJECT_WORK) main_work.total_num_act[index]).scale.y = 4096;
        }

        for (int index = 0; index < 3; ++index)
        {
            ObjObjectAction2dAMALoadSetTexlist((OBS_OBJECT_WORK) main_work.line_act[index], main_work.line_act[index].obj_2d, null, null, 30, GmGameDatGetCockpitData(), AoTexGetTexList(main_work.tex[0]), (uint) (67 + index), 0);
            ((OBS_OBJECT_WORK) main_work.line_act[index]).scale.x = 4096;
            ((OBS_OBJECT_WORK) main_work.line_act[index]).scale.y = 4096;
        }

        ObjObjectAction2dAMALoadSetTexlist((OBS_OBJECT_WORK) main_work.tex_time_act, main_work.tex_time_act.obj_2d, null, null, g_gm_clear_demo_data_ama_id[gmClearDemoLanguage()], GmGameDatGetCockpitData(), AoTexGetTexList(main_work.tex[1]), 18U, 0);
        ((OBS_OBJECT_WORK) main_work.tex_time_act).scale.x = 4096;
        ((OBS_OBJECT_WORK) main_work.tex_time_act).scale.y = 4096;
        main_work.tex_time_act.obj_2d.ama.act_tbl[(int) main_work.tex_time_act.obj_2d.act_id].mtn.trs_tbl[0].trs_y -= 15f;
        main_work.tex_time_act.obj_2d.ama.act_tbl[(int) main_work.tex_time_act.obj_2d.act_id].mtn.trs_tbl[1].trs_y -= 15f;
        main_work.tex_time_act.obj_2d.ama.act_tbl[(int) main_work.tex_time_act.obj_2d.act_id].mtn.trs_tbl[2].trs_y -= 15f;
        ObjObjectAction2dAMALoadSetTexlist((OBS_OBJECT_WORK) main_work.tex_ring_act, main_work.tex_ring_act.obj_2d, null, null, g_gm_clear_demo_data_ama_id[gmClearDemoLanguage()], GmGameDatGetCockpitData(), AoTexGetTexList(main_work.tex[1]), 19U, 0);
        ((OBS_OBJECT_WORK) main_work.tex_ring_act).scale.x = 4096;
        ((OBS_OBJECT_WORK) main_work.tex_ring_act).scale.y = 4096;
        ObjObjectAction2dAMALoadSetTexlist((OBS_OBJECT_WORK) main_work.tex_total_act, main_work.tex_total_act.obj_2d, null, null, g_gm_clear_demo_data_ama_id[gmClearDemoLanguage()], GmGameDatGetCockpitData(), AoTexGetTexList(main_work.tex[1]), 20U, 0);
        ((OBS_OBJECT_WORK) main_work.tex_total_act).scale.x = 4096;
        ((OBS_OBJECT_WORK) main_work.tex_total_act).scale.y = 4096;
        ObjObjectAction2dAMALoadSetTexlist((OBS_OBJECT_WORK) main_work.sonic_icon_act, main_work.sonic_icon_act.obj_2d, null, null, 30, GmGameDatGetCockpitData(), AoTexGetTexList(main_work.tex[0]), 70U, 0);
        ((OBS_OBJECT_WORK) main_work.sonic_icon_act).scale.x = 4096;
        ((OBS_OBJECT_WORK) main_work.sonic_icon_act).scale.y = 4096;
        ObjObjectAction2dAMALoadSetTexlist((OBS_OBJECT_WORK) main_work.sonic_icon_act2, main_work.sonic_icon_act2.obj_2d, null, null, 30, GmGameDatGetCockpitData(), AoTexGetTexList(main_work.tex[0]), 71U, 0);
        ((OBS_OBJECT_WORK) main_work.sonic_icon_act2).scale.x = 4096;
        ((OBS_OBJECT_WORK) main_work.sonic_icon_act2).scale.y = 4096;
        ObjObjectAction2dAMALoadSetTexlist((OBS_OBJECT_WORK) main_work.tex_extend_act, main_work.tex_extend_act.obj_2d, null, null, g_gm_clear_demo_data_ama_id[gmClearDemoLanguage()], GmGameDatGetCockpitData(), AoTexGetTexList(main_work.tex[1]), 10U, 0);
        ((OBS_OBJECT_WORK) main_work.tex_extend_act).scale.x = 4096;
        ((OBS_OBJECT_WORK) main_work.tex_extend_act).scale.y = 4096;
        gmClearDemoCreateObjNextPromptAct(main_work);
    }

    private static void gmClearDemoCreateObjSpecialTimeAtk(GMS_CLRDM_MAIN_WORK main_work)
    {
        ObjObjectAction2dAMALoadSetTexlist((OBS_OBJECT_WORK) main_work.tex_spst_up_act[0], main_work.tex_spst_up_act[0].obj_2d, null, null, g_gm_clear_demo_data_ama_id[gmClearDemoLanguage()], GmGameDatGetCockpitData(), AoTexGetTexList(main_work.tex[1]), 11U, 0);
        ((OBS_OBJECT_WORK) main_work.tex_spst_up_act[0]).scale.x = 4096;
        ((OBS_OBJECT_WORK) main_work.tex_spst_up_act[0]).scale.y = 4096;
        ObjObjectAction2dAMALoadSetTexlist((OBS_OBJECT_WORK) main_work.tex_spst_up_act[2], main_work.tex_spst_up_act[2].obj_2d, null, null, g_gm_clear_demo_data_ama_id[gmClearDemoLanguage()], GmGameDatGetCockpitData(), AoTexGetTexList(main_work.tex[1]), 13U, 0);
        ((OBS_OBJECT_WORK) main_work.tex_spst_up_act[2]).scale.x = 4096;
        ((OBS_OBJECT_WORK) main_work.tex_spst_up_act[2]).scale.y = 4096;
        gmClearDemoCreateObjSpeActForStage(main_work);
        for (int index = 0; index < main_work.has_eme_num; ++index)
        {
            ObjObjectAction2dAMALoadSetTexlist((OBS_OBJECT_WORK) main_work.icon_emer_up_act[index], main_work.icon_emer_up_act[index].obj_2d, null, null, 30, GmGameDatGetCockpitData(), AoTexGetTexList(main_work.tex[0]), (uint) (42 + index), 0);
            ((OBS_OBJECT_WORK) main_work.icon_emer_up_act[index]).scale.x = 4096;
            ((OBS_OBJECT_WORK) main_work.icon_emer_up_act[index]).scale.y = 4096;
        }

        ObjObjectAction2dAMALoadSetTexlist((OBS_OBJECT_WORK) main_work.tex_big_time_act, main_work.tex_big_time_act.obj_2d, null, null, g_gm_clear_demo_data_ama_id[gmClearDemoLanguage()], GmGameDatGetCockpitData(), AoTexGetTexList(main_work.tex[1]), 15U, 0);
        ((OBS_OBJECT_WORK) main_work.tex_big_time_act).scale.x = 4096;
        ((OBS_OBJECT_WORK) main_work.tex_big_time_act).scale.y = 4096;
        ObjObjectAction2dAMALoadSetTexlist((OBS_OBJECT_WORK) main_work.time_sonic_icon_act, main_work.time_sonic_icon_act.obj_2d, null, null, 30, GmGameDatGetCockpitData(), AoTexGetTexList(main_work.tex[0]), 56U, 0);
        ((OBS_OBJECT_WORK) main_work.time_sonic_icon_act).scale.x = 4096;
        ((OBS_OBJECT_WORK) main_work.time_sonic_icon_act).scale.y = 4096;
        main_work.time_sonic_icon_act.obj_2d.frame = GMM_MAIN_USE_SUPER_SONIC() ? 1f : 0.0f;
        main_work.time_sonic_icon_act.obj_2d.speed = 0.0f;
        ObjObjectAction2dAMALoadSetTexlist((OBS_OBJECT_WORK) main_work.tex_new_record_act, main_work.tex_new_record_act.obj_2d, null, null, g_gm_clear_demo_data_ama_id[gmClearDemoLanguage()], GmGameDatGetCockpitData(), AoTexGetTexList(main_work.tex[1]), 14U, 0);
        ((OBS_OBJECT_WORK) main_work.tex_new_record_act).scale.x = 4096;
        ((OBS_OBJECT_WORK) main_work.tex_new_record_act).scale.y = 4096;
    }

    private static void gmClearDemoCreateObjActForStage(GMS_CLRDM_MAIN_WORK main_work)
    {
        int language = GsEnvGetLanguage();
        ushort num1;
        switch (language)
        {
            case 3:
                num1 = 84;
                break;
            case 5:
                num1 = 85;
                break;
            default:
                num1 = 0;
                break;
        }

        if (language != 4)
        {
            ObjObjectAction2dAMALoadSetTexlist((OBS_OBJECT_WORK) main_work.tex_up_act[0], main_work.tex_up_act[0].obj_2d, null, null, 30, GmGameDatGetCockpitData(), AoTexGetTexList(main_work.tex[0]), num1, 0);
            ((OBS_OBJECT_WORK) main_work.tex_up_act[0]).scale.x = 4096;
            ((OBS_OBJECT_WORK) main_work.tex_up_act[0]).scale.y = 4096;
        }

        ushort num2 = language == 4 ? dm_clrdm_stage_ge_tex_act_id[main_work.stage_id] : dm_clrdm_stage_tex_act_id[main_work.stage_id];
        ObjObjectAction2dAMALoadSetTexlist((OBS_OBJECT_WORK) main_work.tex_up_act[1], main_work.tex_up_act[1].obj_2d, null, null, g_gm_clear_demo_data_ama_id[gmClearDemoLanguage()], GmGameDatGetCockpitData(), AoTexGetTexList(main_work.tex[1]), num2, 0);
        ((OBS_OBJECT_WORK) main_work.tex_up_act[1]).scale.x = 4096;
        ((OBS_OBJECT_WORK) main_work.tex_up_act[1]).scale.y = 4096;
        ushort num3;
        int index;
        if (dm_clrdm_stage_text_amb_id[main_work.stage_id] == 0)
        {
            num3 = 0;
            index = 30;
        }
        else
        {
            num3 = 1;
            index = g_gm_clear_demo_data_ama_id[gmClearDemoLanguage()];
        }

        if (num3 == 0 && (language == 0 || language == 1))
        {
            ObjObjectAction2dAMALoadSetTexlist((OBS_OBJECT_WORK) main_work.tex_up_act[2], main_work.tex_up_act[2].obj_2d, null, null, g_gm_clear_demo_data_ama_id[gmClearDemoLanguage()], GmGameDatGetCockpitData(), AoTexGetTexList(main_work.tex[1]), 3U, 0);
            ((OBS_OBJECT_WORK) main_work.tex_up_act[2]).scale.x = 4096;
            ((OBS_OBJECT_WORK) main_work.tex_up_act[2]).scale.y = 4096;
        }

        ObjObjectAction2dAMALoadSetTexlist((OBS_OBJECT_WORK) main_work.tex_up_act[3], main_work.tex_up_act[3].obj_2d, null, null, g_gm_clear_demo_data_ama_id[gmClearDemoLanguage()], GmGameDatGetCockpitData(), AoTexGetTexList(main_work.tex[1]), dm_clrdm_stage_text_act_id[main_work.stage_id], 0);
        ((OBS_OBJECT_WORK) main_work.tex_up_act[3]).scale.x = 4096;
        ((OBS_OBJECT_WORK) main_work.tex_up_act[3]).scale.y = 4096;
        ushort num4 = language != 4 ? (language != 2 ? (language != 5 ? (language != 3 ? dm_clrdm_stage_num_act_id[main_work.stage_id] : dm_clrdm_stage_sp_num_act_id[main_work.stage_id]) : dm_clrdm_stage_sp_num_act_id[main_work.stage_id]) : dm_clrdm_stage_fr_num_act_id[main_work.stage_id]) : dm_clrdm_stage_ge_num_act_id[main_work.stage_id];
        if (num4 == ushort.MaxValue)
            return;
        ObjObjectAction2dAMALoadSetTexlist((OBS_OBJECT_WORK) main_work.tex_up_act[4], main_work.tex_up_act[4].obj_2d, null, null, index, GmGameDatGetCockpitData(), AoTexGetTexList(main_work.tex[num3]), num4, 0);
        ((OBS_OBJECT_WORK) main_work.tex_up_act[4]).scale.x = 4096;
        ((OBS_OBJECT_WORK) main_work.tex_up_act[4]).scale.y = 4096;
    }

    private static void gmClearDemoCreateObjSpeActForStage(GMS_CLRDM_MAIN_WORK main_work)
    {
        int language = GsEnvGetLanguage();
        int index = main_work.stage_id - 21;
        int num = language == 3 ? 86 : 27;
        ObjObjectAction2dAMALoadSetTexlist((OBS_OBJECT_WORK) main_work.spst_num_act[index], main_work.spst_num_act[index].obj_2d, null, null, 30, GmGameDatGetCockpitData(), AoTexGetTexList(main_work.tex[0]), (uint) (num + index), 0);
        ((OBS_OBJECT_WORK) main_work.spst_num_act[index]).scale.x = 4096;
        ((OBS_OBJECT_WORK) main_work.spst_num_act[index]).scale.y = 4096;
    }
}
