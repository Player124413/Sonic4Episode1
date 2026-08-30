using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;

// nnstdshader's VS_INPUT layout; a rigid permutation just leaves the blend elements unread, so
// every mesh can share this one declaration
internal struct DXVertex : IVertexType
{
    public Vector3 Position;
    public Vector3 BlendWeights;
    public Byte4 BlendIndices;
    public Vector3 Normal;
    public Vector2 TexCoord;

    public static readonly VertexDeclaration Declaration = new VertexDeclaration(
        new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
        new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.BlendWeight, 0),
        new VertexElement(24, VertexElementFormat.Byte4, VertexElementUsage.BlendIndices, 0),
        new VertexElement(28, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
        new VertexElement(40, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0));

    VertexDeclaration IVertexType.VertexDeclaration => Declaration;
}
