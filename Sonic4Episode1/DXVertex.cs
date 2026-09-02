using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;

internal struct DXVertex : IVertexType
{
    public Vector3 Position;
    public Vector3 BlendWeights;
    public Byte4 BlendIndices;
    public Vector3 Normal;
    public Vector3 Tangent;
    public Vector3 Binormal;
    public Vector2 TexCoord;
    public Vector2 TexCoord2;
    public Vector2 TexCoord3;
    public Color Color;

    public static readonly VertexDeclaration Declaration = new VertexDeclaration(
        new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
        new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.BlendWeight, 0),
        new VertexElement(24, VertexElementFormat.Byte4, VertexElementUsage.BlendIndices, 0),
        new VertexElement(28, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
        new VertexElement(40, VertexElementFormat.Vector3, VertexElementUsage.Tangent, 0),
        new VertexElement(52, VertexElementFormat.Vector3, VertexElementUsage.Binormal, 0),
        new VertexElement(64, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
        new VertexElement(72, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 1),
        new VertexElement(80, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 2),
        new VertexElement(88, VertexElementFormat.Color, VertexElementUsage.Color, 0));

    VertexDeclaration IVertexType.VertexDeclaration => Declaration;
}
