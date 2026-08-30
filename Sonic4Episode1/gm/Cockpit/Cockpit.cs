using static AppMain;

// gmCockpit.cpp. Not a screen: the object-creation helper every cockpit screen calls, which is why
// the GMS_COCKPIT_*_WORK types and GMD_OBJTYPE_COCKPIT stay on AppMain with the rest of the object
// vocabulary. No platform fork; the only conditionals in the C++ are MTD_DEBUG passing a task name.
internal static partial class GmCockpit
{
    public static OBS_OBJECT_WORK GMM_COCKPIT_CREATE_WORK(
        TaskWorkFactoryDelegate work_size,
        OBS_OBJECT_WORK parent_obj,
        ushort sort_prio,
        string name)
    {
        return GmCockpitCreateWork(work_size, parent_obj, sort_prio, name);
    }

    // the C++ clamps work_size up to sizeof(OBS_OBJECT_WORK) first; there is nothing to clamp here,
    // the caller hands over a factory rather than a byte count
    private static OBS_OBJECT_WORK GmCockpitCreateWork(
        TaskWorkFactoryDelegate work_size,
        OBS_OBJECT_WORK parent_obj,
        ushort sort_prio,
        string name)
    {
        var obj_work = OBM_OBJECT_TASK_DETAIL_INIT(
            (ushort)(GMD_TASK_PRIO_FIX + sort_prio),
            GMD_TASK_GROUP_FIX,
            GMD_TASK_PAUSELEVEL_DEF,
            GMD_OBJ_OBJPAUSELEVEL_DEF,
            work_size,
            name);

        if (obj_work == null)
        {
            mppAssertNotImpl();
            return null;
        }

        obj_work.obj_type = GMD_OBJTYPE_COCKPIT;

        obj_work.ppOut = new MPP_VOID_OBS_OBJECT_WORK(ObjDrawActionSummary);
        obj_work.ppOutSub = null;
        obj_work.ppIn = null;
        obj_work.ppMove = null;
        obj_work.ppActCall = null;
        obj_work.ppRec = null;
        obj_work.ppLast = null;
        obj_work.ppViewCheck = null;

        obj_work.spd_fall = GMD_OBJ_DEF_FALL_SPD;
        obj_work.spd_fall_max = GMD_OBJ_DEF_FALL_SPDMA;

        if (parent_obj != null)
        {
            obj_work.parent_obj = parent_obj;
            obj_work.pos.x = parent_obj.pos.x;
            obj_work.pos.y = parent_obj.pos.y;
            obj_work.pos.z = parent_obj.pos.z;
        }

        obj_work.flag |= OBD_OBJECT_NOHIT | OBD_OBJECT_NOCLIP;
        obj_work.move_flag |= OBD_MOVE_NOCOL;

        return obj_work;
    }
}
