using System;

internal partial class AppMain
{
    public static object amBindGet(AMS_AMB_HEADER header, int index)
    {
        string sPath;
        AmbChunk buf = amBindGet(header, index, out sPath);
        if (header.files[index].IndexOf(".amb", StringComparison.OrdinalIgnoreCase) == -1)
            return buf;
        AMS_AMB_HEADER amsAmbHeader = readAMBFile(buf);
        amsAmbHeader.dir = sPath;
        return amsAmbHeader;
    }

    // the one place a member's bytes are located, so a per-member console override lands for every
    // reader at once. the chunk is parented to the sibling rather than copied out of it, so it
    // keeps the sibling's byte order and its preload cache. see docs/per-member-variants.md
    internal static AmbChunk amBindChunk(AMS_AMB_HEADER header, int index)
    {
        var sibling = header.member_variant_amb;
        if (sibling != null)
        {
            var k = header.member_variant_index[index];
            if (k >= 0)
                return amBindChunk(sibling, k);
        }

        return new AmbChunk(header.data, header.offsets[index], header.lengths[index], header);
    }

    public static AmbChunk amBindGet(
      AMS_AMB_HEADER header,
      int index,
      out string sPath)
    {
        sPath = null;
        AmbChunk ambChunk = null;
        if (index < header.file_num)
            ambChunk = amBindChunk(header, index);
        else
            mppAssertNotImpl();
        return ambChunk;
    }

    internal static AmbChunk amBindGet(AMS_FS header, int index)
    {
        return amBindGet(header, index, out string _);
    }

    internal static AmbChunk amBindGet(
      AMS_FS header,
      int index,
      out string sPath)
    {
        sPath = null;
        byte[] array = null;
        int offset = -1;
        int length = 0;
        if (index < header.count)
        {
            var sibling = header.member_variant_amb;
            if (sibling != null && header.member_variant_index[index] >= 0)
                return amBindChunk(sibling, header.member_variant_index[index]);

            array = header.data;
            offset = header.offsets[index];
            length = header.lengths[index];
        }
        return new AmbChunk(array, offset, length, header.amb_header);
    }

    public static AmbChunk amBindSearch(AMS_AMB_HEADER header, string filename)
    {
        for (int index = 0; index < header.file_num; ++index)
        {
            if (header.files[index] == filename)
                return amBindChunk(header, index);
        }
        return null;
    }

    // File order differs between builds (the WP7 water AMB has 8 entries where PC/360 have 11),
    // so anything the C++ reaches by a hardcoded index has to be found by name here.
    public static int amBindSearchBaseName(AMS_AMB_HEADER header, string basename)
    {
        for (int index = 0; index < header.file_num; ++index)
        {
            string name = mppAmbMemberBaseName(header.files[index]);
            if (string.Compare(name, basename, StringComparison.OrdinalIgnoreCase) == 0)
                return index;
        }
        return -1;
    }

    private static AmbChunk amBindSearchEx(
      AMS_AMB_HEADER header,
      string exname)
    {
        AmbChunk ambChunk = null;
        for (int index = 0; index < header.file_num; ++index)
        {
            if (header.files[index].IndexOf(exname, 0, StringComparison.OrdinalIgnoreCase) != -1)
            {
                ambChunk = amBindChunk(header, index);
                break;
            }
        }
        return ambChunk;
    }

    private byte[] amBindSearchID(ref AMS_AMB_HEADER header, string file_id, byte[] top)
    {
        mppAssertNotImpl();
        return null;
    }



}