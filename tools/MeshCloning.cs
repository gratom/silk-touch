using UnityEngine;

namespace Tools
{
    public static class MeshCloning
    {
        public static Mesh Clone(this Mesh original)
        {
            Mesh clonedMesh = new Mesh
            {
                name = "cloned" + original.name,
                vertices = original.vertices,
                triangles = original.triangles,
                uv = original.uv,
                normals = original.normals,
                colors = original.colors,
                tangents = original.tangents,
                bindposes = original.bindposes,
                indexFormat = original.indexFormat,
                boneWeights = original.boneWeights
            };

            return clonedMesh;
        }
    }
}