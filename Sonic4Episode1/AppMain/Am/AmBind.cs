using System;

internal partial class AppMain
{
    public static object amBindGet(AMS_AMB_HEADER header, int index)
    {
        string sPath;
        AmbChunk buf = amBindGet(header, index, out sPath);
        if (buf == null)
            return null;
        if (header.files[index].IndexOf(".amb", StringComparison.OrdinalIgnoreCase) == -1)
            return buf;
        AMS_AMB_HEADER amsAmbHeader = readAMBFile(buf);
        amsAmbHeader.dir = sPath;
        return amsAmbHeader;
    }

    // the one place a member's bytes are located, so a per-member console override lands for every
    // reader at once. the chunk is parented to the sibling rather than copied out of it, so it
    // keeps the sibling's byte order and its preload cache
    internal static AmbChunk amBindChunk(AMS_AMB_HEADER header, int index)
    {
        var sibling = header.member_variant_amb;
        if (sibling != null)
        {
            var k = header.member_variant_index[index];
            if (k >= 0)
                return amBindChunk(sibling, k);
        }

        return new AmbChunk(
            header.data, header.data_base + header.offsets[index], header.lengths[index], header);
    }

    // An AMB can carry absent members, stored as offset 0 and size 0. On console those stay null
    // through amBindConv, whose amConvert macro only rebases a non-zero offset, so amBindGet hands
    // the caller a null and every `if (data)` guard in the game works. Ours built a zero-length
    // chunk pointing at the header instead, which no caller checks for because none has to.
    //
    // It goes unnoticed until an archive is mostly absent members: ZONE2_M_M has 4 live of 268, and
    // handing the other 264 to the NN motion parser is an EndOfStreamException on stage load
    public static AmbChunk amBindGet(
      AMS_AMB_HEADER header,
      int index,
      out string sPath)
    {
        sPath = null;

        if (index >= header.file_num)
        {
            mppAssertNotImpl();
            return null;
        }

        if (amBindIsAbsent(header, index))
            return null;

        return amBindChunk(header, index);
    }

    // Console keys absence off the offset, because its archives store 0 there for a member with no
    // data and amConvert only rebases a non-zero one. Ours cannot: the content build repacks, and
    // its packer gives every absent member the running data position with a zero length, so
    // ZONE2_M_M's 264 absent members come out at offset 4320 rather than 0. Length is the predicate
    // that holds for both, and it is the one that matters anyway - a member with no bytes has
    // nothing any reader can parse
    private static bool amBindIsAbsent(AMS_AMB_HEADER header, int index)
    {
        var sibling = header.member_variant_amb;
        if (sibling != null)
        {
            var k = header.member_variant_index[index];
            if (k >= 0)
                return amBindIsAbsent(sibling, k);
        }

        return header.lengths[index] == 0;
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