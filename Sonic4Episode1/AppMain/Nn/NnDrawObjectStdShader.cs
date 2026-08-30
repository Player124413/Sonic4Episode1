using Microsoft.Xna.Framework.Graphics;
using mpp;

internal partial class AppMain
{
    private static void nnDrawObjectStdShader(
        NNS_OBJECT obj,
        NNS_MATRIX[] mtxpal,
        uint[] nodestatlist,
        uint subobjtype,
        uint flag)
    {
        if (((int)flag & 1) != 0 || obj.nSubobj <= 0)
            return;

        OpenGL.glFlush();

        nngDrawCallBackVal.pObject = obj;
        nngDrawCallBackVal.pMatrixPalette = mtxpal;
        nngDrawCallBackVal.pNodeStatusList = nodestatlist;
        nngDrawCallBackVal.DrawSubobjType = subobjtype;
        nngDrawCallBackVal.DrawFlag = flag;
        nngDrawCallBackVal.iPrevMaterial = -1;
        nngDrawCallBackVal.iPrevVtxList = -1;

        nnSetWorldMatrixPalette(mtxpal);

        if (subobjtype == 2147483648U)
            subobjtype = 775U;

        for (int iSubobj = 0; iSubobj < obj.nSubobj; ++iSubobj)
        {
            NNS_SUBOBJ subobj = obj.pSubobjList[iSubobj];
            if (((int)subobjtype & (int)subobj.fType & 7) == 0
                || ((int)subobjtype & (int)subobj.fType & 768) == 0)
                continue;

            nngDrawCallBackVal.iSubobject = iSubobj;
            for (int iMeshset = 0; iMeshset < subobj.nMeshset; ++iMeshset)
            {
                NNS_MESHSET meshset = subobj.pMeshsetList[iMeshset];
                NNS_VTXLISTPTR vlistptr = obj.pVtxListPtrList[meshset.iVtxList];
                NNS_PRIMLISTPTR plistptr = obj.pPrimListPtrList[meshset.iPrimList];
                if (nodestatlist != null && ((int)nodestatlist[meshset.iNode] & 1) != 0)
                    continue;

                nngDrawCallBackVal.iMeshset = iMeshset;
                nngDrawCallBackVal.iNode = meshset.iNode;
                nngDrawCallBackVal.iMaterial = meshset.iMaterial;
                nngDrawCallBackVal.pMaterial = obj.pMatPtrList[meshset.iMaterial];
                nngDrawCallBackVal.iVtxList = meshset.iVtxList;
                nngDrawCallBackVal.pVtxListPtr = vlistptr;
                nngDrawCallBackVal.bModified = 0;
                nngDrawCallBackVal.bReDraw = 0;

                do
                {
                    nnSetViewMatrix(null);
                    nnSetShaderMatrix(nngDrawCallBackVal, meshset);
                    nnSetShaderBoneMatrix(nngDrawCallBackVal, meshset);
                    if (nnPutMaterial(nngDrawCallBackVal) == 0)
                        break;

                    if (vlistptr.pVtxList is NNS_VTXLIST_DX_DESC)
                    {
                        if (((int)flag & 768) != 0 || ((int)flag & 28672) != 0)
                        {
                            mppAssertNotImpl();
                        }
                        else
                        {
                            nnSetVertexFormat(vlistptr);
                            nnBindStdShader(nngDrawCallBackVal);
                            nnDrawMeshIndexPrimitiveList(vlistptr, plistptr);
                        }

                        nngDrawCallBackVal.iPrevMaterial = meshset.iMaterial;
                        nngDrawCallBackVal.iPrevVtxList = meshset.iVtxList;
                    }
                    else if (((int)vlistptr.fType & 16711680) != 0)
                    {
                        NNM_ASSERT(false, "not support common vertex type\n");
                    }
                }
                while (nngDrawCallBackVal.bReDraw != 0);
            }
        }
    }
}
