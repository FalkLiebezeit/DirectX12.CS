using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using SharpDX;

namespace DX12GameProgramming
{
    public static class GeometryGenerator
    {
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct Vertex
        {
            public Vector3 Position;
            public Vector3 Normal;
            public Vector3 TangentU;
            public Vector2 TexC;

            public Vertex(Vector3 p, Vector3 n, Vector3 t, Vector2 uv)
            {
                Position = p;
                Normal = n;
                TangentU = t;
                TexC = uv;
            }

            public Vertex(
                float px, float py, float pz,
                float nx, float ny, float nz,
                float tx, float ty, float tz,
                float u, float v) : this(
                    new Vector3(px, py, pz),
                    new Vector3(nx, ny, nz),
                    new Vector3(tx, ty, tz),
                    new Vector2(u, v))
            {
            }
        }

        public class MeshData
        {
            public List<Vertex> Vertices { get; } = new List<Vertex>();
            public List<int> Indices32 { get; } = new List<int>();

            public List<short> GetIndices16() => Indices32.Select(i => (short)i).ToList();
        }

        public static MeshData CreateGrid(float width, float depth, int m, int n)
        {
            var meshData = new MeshData();

            //
            // Create the vertices.
            //

            float halfWidth = 0.5f * width;
            float halfDepth = 0.5f * depth;

            float dx = width / (n - 1);
            float dz = depth / (m - 1);

            float du = 1f / (n - 1);
            float dv = 1f / (m - 1);

            for (int i = 0; i < m; i++)
            {
                float z = halfDepth - i * dz;

                for (int j = 0; j < n; j++)
                {
                    float x = -halfWidth + j * dx;

                    meshData.Vertices.Add(new Vertex(
                        new Vector3(x, 0, z),
                        new Vector3(0, 1, 0),
                        new Vector3(0, 0, 1),
                        new Vector2(j * du, i * dv))); // Stretch texture over grid.
                }
            }

            //
            // Create the indices.
            //

            // Iterate over each quad and compute indices.
            for (int i = 0; i < m - 1; i++)
            {
                for (int j = 0; j < n - 1; j++)
                {
                    meshData.Indices32.Add(i * n + j);
                    meshData.Indices32.Add(i * n + j + 1);
                    meshData.Indices32.Add((i + 1) * n + j);

                    meshData.Indices32.Add((i + 1) * n + j);
                    meshData.Indices32.Add(i * n + j + 1);
                    meshData.Indices32.Add((i + 1) * n + j + 1);
                }
            }

            return meshData;
        }


        public static MeshData CreateXZGrid(float width, float depth, int m, int n)
        {
            var meshData = new MeshData();

            //
            // Create the vertices.
            //

            float halfWidth = 0.5f * width;
            float halfDepth = 0.5f * depth;

            float dx = width / (n - 1);
            float dz = depth / (m - 1);

            float du = 1f / (n - 1);
            float dv = 1f / (m - 1);

            for (int i = 0; i < m; i++)
            {
                float z = halfDepth - i * dz;

                for (int j = 0; j < n; j++)
                {
                    float x = -halfWidth + j * dx;

                    meshData.Vertices.Add(new Vertex(
                        new Vector3(x, 0, z),
                        new Vector3(0, 1, 0),
                        new Vector3(0, 0, 1),
                        new Vector2(j * du, i * dv))); // Stretch texture over grid.
                }
            }

            //
            // Create the indices.
            //

            // Iterate over each quad and compute indices.
            for (int i = 0; i < m - 1; i++)
            {
                for (int j = 0; j < n - 1; j++)
                {
                    meshData.Indices32.Add(i * n + j);
                    meshData.Indices32.Add(i * n + j + 1);
                    meshData.Indices32.Add((i + 1) * n + j);

                    meshData.Indices32.Add((i + 1) * n + j);
                    meshData.Indices32.Add(i * n + j + 1);
                    meshData.Indices32.Add((i + 1) * n + j + 1);
                }
            }

            return meshData;
        }

        public static MeshData CreateXYGrid(float width, float depth, int m, int n)
        {
            var meshData = new MeshData();

            //
            // Create the vertices.
            //

            float halfWidth = 0.5f * width;
            float halfDepth = 0.5f * depth;

            float dx = width / (n - 1);
            float dy = depth / (m - 1);

            float du = 1f / (n - 1);
            float dv = 1f / (m - 1);

            for (int i = 0; i < m; i++)
            {
                float y = halfDepth - i * dy;

                for (int j = 0; j < n; j++)
                {
                    float x = -halfWidth + j * dx;

                    meshData.Vertices.Add(new Vertex(
                        new Vector3(x, y, 0),
                        new Vector3(0, 1, 0),
                        new Vector3(0, 0, 1),
                        new Vector2(j * du, i * dv))); // Stretch texture over grid.
                }
            }

            //
            // Create the indices.
            //

            // Iterate over each quad and compute indices.
            for (int i = 0; i < m - 1; i++)
            {
                for (int j = 0; j < n - 1; j++)
                {
                    meshData.Indices32.Add(i * n + j);
                    meshData.Indices32.Add(i * n + j + 1);
                    meshData.Indices32.Add((i + 1) * n + j);

                    meshData.Indices32.Add((i + 1) * n + j);
                    meshData.Indices32.Add(i * n + j + 1);
                    meshData.Indices32.Add((i + 1) * n + j + 1);
                }
            }

            return meshData;
        }


        public static MeshData CreateBox(float width, float height, float depth, int numSubdivisions)
        {
            var meshData = new MeshData();

            //
            // Create the vertices.
            //

            var w2 = 0.5f * width;
            var h2 = 0.5f * height;
            var d2 = 0.5f * depth;


            // Fill in the front face vertex data.
            meshData.Vertices.Add(new Vertex(-w2, -h2, -d2, 0, 0, -1, 1, 0, 0, 0, 1));
            meshData.Vertices.Add(new Vertex(-w2, +h2, -d2, 0, 0, -1, 1, 0, 0, 0, 0));
            meshData.Vertices.Add(new Vertex(+w2, +h2, -d2, 0, 0, -1, 1, 0, 0, 1, 0));
            meshData.Vertices.Add(new Vertex(+w2, -h2, -d2, 0, 0, -1, 1, 0, 0, 1, 1));

            // Fill in the back face vertex data.
            meshData.Vertices.Add(new Vertex(-w2, -h2, +d2, 0, 0, 1, -1, 0, 0, 1, 1));
            meshData.Vertices.Add(new Vertex(+w2, -h2, +d2, 0, 0, 1, -1, 0, 0, 0, 1));
            meshData.Vertices.Add(new Vertex(+w2, +h2, +d2, 0, 0, 1, -1, 0, 0, 0, 0));
            meshData.Vertices.Add(new Vertex(-w2, +h2, +d2, 0, 0, 1, -1, 0, 0, 1, 0));

            // Fill in the top face vertex data.
            meshData.Vertices.Add(new Vertex(-w2, +h2, -d2, 0, 1, 0, 1, 0, 0, 0, 1));
            meshData.Vertices.Add(new Vertex(-w2, +h2, +d2, 0, 1, 0, 1, 0, 0, 0, 0));
            meshData.Vertices.Add(new Vertex(+w2, +h2, +d2, 0, 1, 0, 1, 0, 0, 1, 0));
            meshData.Vertices.Add(new Vertex(+w2, +h2, -d2, 0, 1, 0, 1, 0, 0, 1, 1));

            // Fill in the bottom face vertex data.
            meshData.Vertices.Add(new Vertex(-w2, -h2, -d2, 0, -1, 0, -1, 0, 0, 1, 1));
            meshData.Vertices.Add(new Vertex(+w2, -h2, -d2, 0, -1, 0, -1, 0, 0, 0, 1));
            meshData.Vertices.Add(new Vertex(+w2, -h2, +d2, 0, -1, 0, -1, 0, 0, 0, 0));
            meshData.Vertices.Add(new Vertex(-w2, -h2, +d2, 0, -1, 0, -1, 0, 0, 1, 0));

            // Fill in the left face vertex data.
            meshData.Vertices.Add(new Vertex(-w2, -h2, +d2, -1, 0, 0, 0, 0, -1, 0, 1));
            meshData.Vertices.Add(new Vertex(-w2, +h2, +d2, -1, 0, 0, 0, 0, -1, 0, 0));
            meshData.Vertices.Add(new Vertex(-w2, +h2, -d2, -1, 0, 0, 0, 0, -1, 1, 0));
            meshData.Vertices.Add(new Vertex(-w2, -h2, -d2, -1, 0, 0, 0, 0, -1, 1, 1));

            // Fill in the right face vertex data.
            meshData.Vertices.Add(new Vertex(+w2, -h2, -d2, 1, 0, 0, 0, 0, 1, 0, 1));
            meshData.Vertices.Add(new Vertex(+w2, +h2, -d2, 1, 0, 0, 0, 0, 1, 0, 0));
            meshData.Vertices.Add(new Vertex(+w2, +h2, +d2, 1, 0, 0, 0, 0, 1, 1, 0));
            meshData.Vertices.Add(new Vertex(+w2, -h2, +d2, 1, 0, 0, 0, 0, 1, 1, 1));



            //
            // Create the indices.
            //

            meshData.Indices32.AddRange(new[]
            {
                // Fill in the front face index data.
                0, 1, 2, 0, 2, 3,

                // Fill in the back face index data.
                4, 5, 6, 4, 6, 7,

                // Fill in the top face index data.
                8, 9, 10, 8, 10, 11,

                // Fill in the bottom face index data.
                12, 13, 14, 12, 14, 15,

                // Fill in the left face index data
                16, 17, 18, 16, 18, 19,

                // Fill in the right face index data
                20, 21, 22, 20, 22, 23
            });

            // Put a cap on the number of subdivisions.
            numSubdivisions = Math.Min(numSubdivisions, 6);

            for (int i = 0; i < numSubdivisions; ++i)
                Subdivide(meshData);

            return meshData;
        }

        public static MeshData CreateSphere(float radius, int sliceCount, int stackCount)
        {
            var meshData = new MeshData();

            //
            // Compute the vertices stating at the top pole and moving down the stacks.
            //

            // Poles: note that there will be texture coordinate distortion as there is
            // not a unique point on the texture map to assign to the pole when mapping
            // a rectangular texture onto a sphere.

            // Top vertex.
            meshData.Vertices.Add(new Vertex(new Vector3(0, radius, 0), new Vector3(0, 1, 0), new Vector3(1, 0, 0), Vector2.Zero));

            float phiStep = MathUtil.Pi / stackCount;
            float thetaStep = 2f * MathUtil.Pi / sliceCount;

            for (int i = 1; i <= stackCount - 1; i++)
            {
                float phi = i * phiStep;

                for (int j = 0; j <= sliceCount; j++)
                {
                    float theta = j * thetaStep;

                    // Spherical to cartesian.
                    var pos = new Vector3(
                        radius * MathHelper.Sinf(phi) * MathHelper.Cosf(theta),
                        radius * MathHelper.Cosf(phi),
                        radius * MathHelper.Sinf(phi) * MathHelper.Sinf(theta));

                    // Partial derivative of P with respect to theta.
                    var tan = new Vector3(
                        -radius * MathHelper.Sinf(phi) * MathHelper.Sinf(theta),
                        0,
                        radius * MathHelper.Sinf(phi) * MathHelper.Cosf(theta));

                    tan.Normalize();

                    Vector3 norm = pos;
                    norm.Normalize();

                    var texCoord = new Vector2(theta / (MathUtil.Pi * 2), phi / MathUtil.Pi);

                    meshData.Vertices.Add(new Vertex(pos, norm, tan, texCoord));
                }
            }

            // Bottom vertex.
            meshData.Vertices.Add(new Vertex(0, -radius, 0, 0, -1, 0, 1, 0, 0, 0, 1));

            //
            // Compute indices for top stack.  The top stack was written first to the vertex buffer
            // and connects the top pole to the first ring.
            //

            for (int i = 1; i <= sliceCount; i++)
            {
                meshData.Indices32.Add(0);
                meshData.Indices32.Add(i + 1);
                meshData.Indices32.Add(i);
            }

            //
            // Compute indices for inner stacks (not connected to poles).
            //

            int baseIndex = 1;
            int ringVertexCount = sliceCount + 1;

            for (int i = 0; i < stackCount - 2; i++)
            {
                for (int j = 0; j < sliceCount; j++)
                {
                    meshData.Indices32.Add(baseIndex + i * ringVertexCount + j);
                    meshData.Indices32.Add(baseIndex + i * ringVertexCount + j + 1);
                    meshData.Indices32.Add(baseIndex + (i + 1) * ringVertexCount + j);

                    meshData.Indices32.Add(baseIndex + (i + 1) * ringVertexCount + j);
                    meshData.Indices32.Add(baseIndex + i * ringVertexCount + j + 1);
                    meshData.Indices32.Add(baseIndex + (i + 1) * ringVertexCount + j + 1);
                }
            }

            //
            // Compute indices for bottom stack.  The bottom stack was written last to the vertex buffer
            // and connects the bottom pole to the bottom ring.
            //

            // South pole vertex was added last.
            int southPoleIndex = meshData.Vertices.Count - 1;

            // Offset the indices to the index of the first vertex in the last ring.
            baseIndex = southPoleIndex - ringVertexCount;

            for (int i = 0; i < sliceCount; i++)
            {
                meshData.Indices32.Add(southPoleIndex);
                meshData.Indices32.Add(baseIndex + i);
                meshData.Indices32.Add(baseIndex + i + 1);
            }
            return meshData;
        }
       
        public static MeshData CreateCylinder(float bottomRadius, float topRadius,
            float height, int sliceCount, int stackCount)
        {
            var meshData = new MeshData();

            BuildCylinderSide(bottomRadius, topRadius, height, sliceCount, stackCount, meshData);

            BuildCylinderTopCap(topRadius, height, sliceCount, meshData);
            BuildCylinderBottomCap(bottomRadius, height, sliceCount, meshData);

            return meshData;
        }
       
        public static MeshData CreateQuad(float x, float y, float w, float h, float depth)
        {
            var meshData = new MeshData();

            // Position coordinates specified in NDC space.
            meshData.Vertices.Add(new Vertex(
                x, y - h, depth,
                0.0f, 0.0f, -1.0f,
                1.0f, 0.0f, 0.0f,
                0.0f, 1.0f));

            meshData.Vertices.Add(new Vertex(
                x, y, depth,
                0.0f, 0.0f, -1.0f,
                1.0f, 0.0f, 0.0f,
                0.0f, 0.0f));

            meshData.Vertices.Add(new Vertex(
                x + w, y, depth,
                0.0f, 0.0f, -1.0f,
                1.0f, 0.0f, 0.0f,
                1.0f, 0.0f));

            meshData.Vertices.Add(new Vertex(
                x + w, y - h, depth,
                0.0f, 0.0f, -1.0f,
                1.0f, 0.0f, 0.0f,
                1.0f, 1.0f));

            meshData.Indices32.Add(0);
            meshData.Indices32.Add(1);
            meshData.Indices32.Add(2);

            meshData.Indices32.Add(0);
            meshData.Indices32.Add(2);
            meshData.Indices32.Add(3);

            return meshData;
        }

        public static MeshData CreateEllipse(float radiusX, float radiusY, int sliceCount, int stackCount)
        {
            var meshData = new MeshData();

            //
            // Compute the vertices stating at the top pole and moving down the stacks.
            //

            // Poles: note that there will be texture coordinate distortion as there is
            // not a unique point on the texture map to assign to the pole when mapping
            // a rectangular texture onto a sphere.

            // Top vertex.
            meshData.Vertices.Add(new Vertex(new Vector3(0, radiusY, 0), new Vector3(0, 1, 0), new Vector3(1, 0, 0), Vector2.Zero));

            float phiStep = MathUtil.Pi / stackCount;
            float thetaStep = 2f * MathUtil.Pi / sliceCount;

            for (int i = 1; i <= stackCount - 1; i++)
            {
                float phi = i * phiStep;
                for (int j = 0; j <= sliceCount; j++)
                {
                    float theta = j * thetaStep;

                    // Spherical to cartesian.
                    var pos = new Vector3(
                        radiusX * MathHelper.Sinf(phi) * MathHelper.Cosf(theta),
                        radiusY * MathHelper.Cosf(phi),
                        radiusX * MathHelper.Sinf(phi) * MathHelper.Sinf(theta));

                    // Partial derivative of P with respect to theta.
                    var tan = new Vector3(
                        -radiusX * MathHelper.Sinf(phi) * MathHelper.Sinf(theta),
                        0,
                        radiusX * MathHelper.Sinf(phi) * MathHelper.Cosf(theta));

                    tan.Normalize();

                    Vector3 norm = pos;

                    norm.Normalize();

                    var texCoord = new Vector2(theta / (MathUtil.Pi * 2), phi / MathUtil.Pi);

                    meshData.Vertices.Add(new Vertex(pos, norm, tan, texCoord));
                }
            }

            // Bottom vertex.
            meshData.Vertices.Add(new Vertex(0, -radiusY, 0, 0, -1, 0, 1, 0, 0, 0, 1));

            //
            // Compute indices for top stack.  The top stack was written first to the vertex buffer
            // and connects the top pole to the first ring.
            //

            for (int i = 1; i <= sliceCount; i++)
            {
                meshData.Indices32.Add(0);
                meshData.Indices32.Add(i + 1);
                meshData.Indices32.Add(i);
            }

            //
            // Compute indices for inner stacks (not connected to poles).
            //

            int baseIndex = 1;
            int ringVertexCount = sliceCount + 1;
            for (int i = 0; i < stackCount - 2; i++)
            {
                for (int j = 0; j < sliceCount; j++)
                {
                    meshData.Indices32.Add(baseIndex + i * ringVertexCount + j);
                    meshData.Indices32.Add(baseIndex + i * ringVertexCount + j + 1);
                    meshData.Indices32.Add(baseIndex + (i + 1) * ringVertexCount + j);

                    meshData.Indices32.Add(baseIndex + (i + 1) * ringVertexCount + j);
                    meshData.Indices32.Add(baseIndex + i * ringVertexCount + j + 1);
                    meshData.Indices32.Add(baseIndex + (i + 1) * ringVertexCount + j + 1);
                }
            }

            //
            // Compute indices for bottom stack.  The bottom stack was written last to the vertex buffer
            // and connects the bottom pole to the bottom ring.
            //

            // South pole vertex was added last.
            int southPoleIndex = meshData.Vertices.Count - 1;

            // Offset the indices to the index of the first vertex in the last ring.
            baseIndex = southPoleIndex - ringVertexCount;

            for (int i = 0; i < sliceCount; i++)
            {
                meshData.Indices32.Add(southPoleIndex);
                meshData.Indices32.Add(baseIndex + i);
                meshData.Indices32.Add(baseIndex + i + 1);
            }
            return meshData;
        }

        public static MeshData CreateTorus(float innerRadius, float outerRadius, int sliceCount, int stackCount)
        {
            var meshData = new MeshData();

            float phiStep = MathUtil.Pi * 2 / stackCount;
            float thetaStep = MathUtil.Pi * 2 / sliceCount;

            for (int i = 0; i <= stackCount; i++)
            {
                float phi = i * phiStep;

                for (int j = 0; j <= sliceCount; j++)
                {
                    float theta = j * thetaStep;

                    // Torus coordinates.
                    float x = (outerRadius + innerRadius * MathHelper.Cosf(theta)) * MathHelper.Cosf(phi);
                    float y = innerRadius * MathHelper.Sinf(theta);
                    float z = (outerRadius + innerRadius * MathHelper.Cosf(theta)) * MathHelper.Sinf(phi);

                    var pos = new Vector3(x, y, z);
                    var norm = new Vector3(MathHelper.Cosf(theta) * MathHelper.Cosf(phi), MathHelper.Sinf(theta), MathHelper.Cosf(theta) * MathHelper.Sinf(phi));
                    norm.Normalize();
                    var tan = new Vector3(-MathHelper.Sinf(phi), 0, MathHelper.Cosf(phi));
                    tan.Normalize();
                    var texCoord = new Vector2((float)j / sliceCount, (float)i / stackCount);

                    meshData.Vertices.Add(new Vertex(pos, norm, tan, texCoord));
                }
            }

            // Indices.
            for (int i = 0; i < stackCount; i++)
            {
                for (int j = 0; j < sliceCount; j++)
                {
                    meshData.Indices32.Add(i * (sliceCount + 1) + j);
                    meshData.Indices32.Add((i + 1) * (sliceCount + 1) + j);
                    meshData.Indices32.Add((i + 1) * (sliceCount + 1) + j + 1);

                    meshData.Indices32.Add(i * (sliceCount + 1) + j);
                    meshData.Indices32.Add((i + 1) * (sliceCount + 1) + j + 1);
                    meshData.Indices32.Add(i * (sliceCount + 1) + j + 1);
                }
            }

            return meshData;
        }

        public static MeshData CreateCone(float bottomRadius, float height, int sliceCount, int stackCount)
        {
            var meshData = new MeshData();

            // BuildConeSide
            {
                float stackHeight = height / stackCount;
                float radiusStep = (bottomRadius) / stackCount;

                int ringCount = stackCount + 1;

                for (int i = 0; i < ringCount; i++)
                {
                    float y = -0.5f * height + i * stackHeight;
                    float r = bottomRadius - i * radiusStep;

                    float dTheta = 2.0f * MathUtil.Pi / sliceCount;
                    for (int j = 0; j <= sliceCount; j++)
                    {
                        float c = MathHelper.Cosf(j * dTheta);
                        float s = MathHelper.Sinf(j * dTheta);
                        var pos = new Vector3(r * c, y, r * s);
                        var tan = new Vector3(-s, 0.0f, c);
                        var bitangent = new Vector3(-r * c, -height, -r * s);
                        var norm = Vector3.Cross(tan, bitangent);
                        norm.Normalize();
                        var texCoord = new Vector2((float)j / sliceCount, 1.0f - (float)i / stackCount);
                        meshData.Vertices.Add(new Vertex(pos, norm, tan, texCoord));
                    }
                }

                int ringVertexCount = sliceCount + 1;

                for (int i = 0; i < stackCount; i++)
                {
                    for (int j = 0; j < sliceCount; j++)
                    {
                        meshData.Indices32.Add(i * ringVertexCount + j);
                        meshData.Indices32.Add((i + 1) * ringVertexCount + j);
                        meshData.Indices32.Add((i + 1) * ringVertexCount + j + 1);
                        meshData.Indices32.Add(i * ringVertexCount + j);
                        meshData.Indices32.Add((i + 1) * ringVertexCount + j + 1);
                        meshData.Indices32.Add(i * ringVertexCount + j + 1);
                    }
                }
            }

            // Build Bottom Cap
            {
                int baseIndex = meshData.Vertices.Count;

            float y = -0.5f * height;

            // vertices of ring
            float dTheta = 2.0f * MathUtil.Pi / sliceCount;

            for (int i = 0; i <= sliceCount; i++)
            {
                float x = bottomRadius * MathHelper.Cosf(i * dTheta);
                float z = bottomRadius * MathHelper.Sinf(i * dTheta);

                // Scale down by the height to try and make top cap texture coord area
                // proportional to base.
                float u = x / height + 0.5f;
                float v = z / height + 0.5f;

                meshData.Vertices.Add(new Vertex(new Vector3(x, y, z), new Vector3(0, -1, 0), new Vector3(1, 0, 0), new Vector2(u, v)));
            }

            // Cap center vertex.
            meshData.Vertices.Add(new Vertex(new Vector3(0, y, 0), new Vector3(0, -1, 0), new Vector3(1, 0, 0), new Vector2(0.5f, 0.5f)));

            // Cache the index of center vertex.
            int centerIndex = meshData.Vertices.Count - 1;

            for (int i = 0; i < sliceCount; i++)
            {
                meshData.Indices32.Add(centerIndex);
                meshData.Indices32.Add(baseIndex + i + 1);
                meshData.Indices32.Add(baseIndex + i);
            }
            }

            return meshData;

        }

        public static MeshData CreateDisc(float innerRadius, float outerRadius, float height, int sliceCount, int stackCount)
        {
            var meshData = new MeshData();

            // Berechne den Winkel zwischen den Scheiben
            float dTheta = 2.0f * MathUtil.Pi / sliceCount;
            float stackHeight = height / stackCount;

            for (int i = 0; i <= stackCount; i++)
            {
                float y = -0.5f * height + i * stackHeight;

                for (int j = 0; j <= sliceCount; j++)
                {
                    float c = MathHelper.Cosf(j * dTheta);
                    float s = MathHelper.Sinf(j * dTheta);
                    float radius = innerRadius + (outerRadius - innerRadius) * ((float)i / stackCount);

                    var pos = new Vector3(radius * c, y, radius * s);
                    var norm = new Vector3(0, 1, 0); // Normalenvektor nach oben
                    var tan = new Vector3(-s, 0.0f, c);
                    var texCoord = new Vector2((float)j / sliceCount, (float)i / stackCount);

                    meshData.Vertices.Add(new Vertex(pos, norm, tan, texCoord));
                }
            }

            int ringVertexCount = sliceCount + 1;
            for (int i = 0; i < stackCount; i++)
            {
                for (int j = 0; j < sliceCount; j++)
                {
                    meshData.Indices32.Add(i * ringVertexCount + j);
                    meshData.Indices32.Add((i + 1) * ringVertexCount + j);
                    meshData.Indices32.Add((i + 1) * ringVertexCount + j + 1);

                    meshData.Indices32.Add(i * ringVertexCount + j);
                    meshData.Indices32.Add((i + 1) * ringVertexCount + j + 1);
                    meshData.Indices32.Add(i * ringVertexCount + j + 1);
                }
            }

            return meshData;
        }

        public static MeshData CreateGaussGrid(float width, float depth, int m, int n)
        {
            var meshData = new MeshData();

            //
            // Create the vertices.
            //

            float halfWidth = 0.5f * width;
            float halfDepth = 0.5f * depth;

            float dx = width / (n - 1);
            float dz = depth / (m - 1);

            float du = 1f / (n - 1);
            float dv = 1f / (m - 1);

            for (int i = 0; i < m; i++)
            {
                float z = halfDepth - i * dz;

                for (int j = 0; j < n; j++)
                {
                    float x = -halfWidth + j * dx;

                     /*
                    meshData.Vertices.Add(new Vertex(
                        new Vector3(x, 0, z),
                        new Vector3(0, 1, 0),
                        new Vector3(1, 0, 0),
                        new Vector2(j * du, i * dv))); // Stretch texture over grid.
                    */
                   
                    meshData.Vertices.Add(new Vertex(
                        new Vector3(x, GetGaussFuncValue(x, z), z),
                        //new Vector3(x, x+z, z),

                        new Vector3(0,1, 0),
                        new Vector3(1, 0, 0),

                        new Vector2(j * du, i * dv))); // Stretch texture over grid.
                    

                }
            }

            //
            // Create the indices.
            //

            // Iterate over each quad and compute indices.
            for (int i = 0; i < m - 1; i++)
            {
                for (int j = 0; j < n - 1; j++)
                {
                    meshData.Indices32.Add(i * n + j);
                    meshData.Indices32.Add(i * n + j + 1);
                    meshData.Indices32.Add((i + 1) * n + j);

                    meshData.Indices32.Add((i + 1) * n + j);
                    meshData.Indices32.Add(i * n + j + 1);
                    meshData.Indices32.Add((i + 1) * n + j + 1);
                }
            }

            return meshData;
        }

        public static MeshData CreateRotSymGrid(float width, float depth, int m, int n)
        {
            var meshData = new MeshData();

            //
            // Create the vertices.
            //

            float halfWidth = 0.5f * width;
            float halfDepth = 0.5f * depth;

            float dx = width / (n - 1);
            float dz = depth / (m - 1);

            float du = 1f / (n - 1);
            float dv = 1f / (m - 1);

            for (int i = 0; i < m; i++)
            {
                float z = halfDepth - i * dz;

                for (int j = 0; j < n; j++)
                {
                    float x = -halfWidth + j * dx;

                    /*
                   meshData.Vertices.Add(new Vertex(
                       new Vector3(x, 0, z),
                       new Vector3(0, 1, 0),
                       new Vector3(1, 0, 0),
                       new Vector2(j * du, i * dv))); // Stretch texture over grid.
                   */

                    meshData.Vertices.Add(new Vertex(
                        new Vector3(x, GetRotSymFuncValue(x, z), z),
                        new Vector3(0, 1, 0),
                        new Vector3(1, 0, 0),

                        new Vector2(j * du, i * dv))); // Stretch texture over grid.


                }
            }

            //
            // Create the indices.
            //

            // Iterate over each quad and compute indices.
            for (int i = 0; i < m - 1; i++)
            {
                for (int j = 0; j < n - 1; j++)
                {
                    meshData.Indices32.Add(i * n + j);
                    meshData.Indices32.Add(i * n + j + 1);
                    meshData.Indices32.Add((i + 1) * n + j);

                    meshData.Indices32.Add((i + 1) * n + j);
                    meshData.Indices32.Add(i * n + j + 1);
                    meshData.Indices32.Add((i + 1) * n + j + 1);
                }
            }

            return meshData;
        }

        public static MeshData CreateParabolic(float width, float depth, int m, int n)
        {
            var meshData = new MeshData();

            //
            // Create the vertices.
            //

            float halfWidth = 0.5f * width;
            float halfDepth = 0.5f * depth;

            float dx = width / (n - 1);
            float dz = depth / (m - 1);

            float du = 1f / (n - 1);
            float dv = 1f / (m - 1);

            for (int i = 0; i < m; i++)
            {
                float z = halfDepth - i * dz;

                for (int j = 0; j < n; j++)
                {
                    float x = -halfWidth + j * dx;

                    meshData.Vertices.Add(new Vertex(
                        new Vector3(x, GetParabolicValue(x, z), z),
                        new Vector3(0, 1, 0),
                        new Vector3(1, 0, 0),

                        new Vector2(j * du, i * dv))); // Stretch texture over grid.


                }
            }

            //
            // Create the indices.
            //

            // Iterate over each quad and compute indices.
            for (int i = 0; i < m - 1; i++)
            {
                for (int j = 0; j < n - 1; j++)
                {
                    meshData.Indices32.Add(i * n + j);
                    meshData.Indices32.Add(i * n + j + 1);
                    meshData.Indices32.Add((i + 1) * n + j);

                    meshData.Indices32.Add((i + 1) * n + j);
                    meshData.Indices32.Add(i * n + j + 1);
                    meshData.Indices32.Add((i + 1) * n + j + 1);
                }
            }

            return meshData;
        }

        public static MeshData CreateRotParabolic(float width, float depth, int m, int n)
        {
            var meshData = new MeshData();

            //
            // Create the vertices.
            //

            float halfWidth = 0.5f * width;
            float halfDepth = 0.5f * depth;

            float dx = width / (n - 1);
            float dz = depth / (m - 1);

            float du = 1f / (n - 1);
            float dv = 1f / (m - 1);

            for (int i = 0; i < m; i++)
            {
                float z = halfDepth - i * dz;

                for (int j = 0; j < n; j++)
                {
                    float x = -halfWidth + j * dx;

                    meshData.Vertices.Add(new Vertex(
                        new Vector3(x, GetRotParabolicValue(x, z), z),
                        new Vector3(0, 1, 0),
                        new Vector3(1, 0, 0),

                        new Vector2(j * du, i * dv))); // Stretch texture over grid.


                }
            }

            //
            // Create the indices.
            //

            // Iterate over each quad and compute indices.
            for (int i = 0; i < m - 1; i++)
            {
                for (int j = 0; j < n - 1; j++)
                {
                    meshData.Indices32.Add(i * n + j);
                    meshData.Indices32.Add(i * n + j + 1);
                    meshData.Indices32.Add((i + 1) * n + j);

                    meshData.Indices32.Add((i + 1) * n + j);
                    meshData.Indices32.Add(i * n + j + 1);
                    meshData.Indices32.Add((i + 1) * n + j + 1);
                }
            }

            return meshData;
        }

        public static MeshData CreateHyperbolicParabolic(float width, float depth, int m, int n)
        {
            var meshData = new MeshData();

            //
            // Create the vertices.
            //

            float halfWidth = 0.5f * width;
            float halfDepth = 0.5f * depth;

            float dx = width / (n - 1);
            float dz = depth / (m - 1);

            float du = 1f / (n - 1);
            float dv = 1f / (m - 1);

            for (int i = 0; i < m; i++)
            {
                float z = halfDepth - i * dz;

                for (int j = 0; j < n; j++)
                {
                    float x = -halfWidth + j * dx;

                    meshData.Vertices.Add(new Vertex(
                        new Vector3(x, GetSaddleParabolicValue(x, z), z),
                        new Vector3(0, 1, 0),
                        new Vector3(1, 0, 0),

                        new Vector2(j * du, i * dv))); // Stretch texture over grid.


                }
            }

            //
            // Create the indices.
            //

            // Iterate over each quad and compute indices.
            for (int i = 0; i < m - 1; i++)
            {
                for (int j = 0; j < n - 1; j++)
                {
                    meshData.Indices32.Add(i * n + j);
                    meshData.Indices32.Add(i * n + j + 1);
                    meshData.Indices32.Add((i + 1) * n + j);

                    meshData.Indices32.Add((i + 1) * n + j);
                    meshData.Indices32.Add(i * n + j + 1);
                    meshData.Indices32.Add((i + 1) * n + j + 1);
                }
            }

            return meshData;
        }
       
        public static MeshData CreateBellyBarrel(float Radius, float height, int sliceCount, int stackCount)
        {
            var meshData = new MeshData();

           BuildBellyBarrelCylinderSide(Radius, height, sliceCount, stackCount, meshData);

            
            BuildCylinderTopCap(Radius, height, sliceCount, meshData);

            // BuildCylinderBottomCap(Radius, height, sliceCount, meshData);

            //BuildBellyBarrelCylinderSide(Radius, height, sliceCount, stackCount, meshData);


            return meshData;
        }

        public static MeshData CreateHyperBolBarrel(float Radius, float height, int sliceCount, int stackCount)
        {
            var meshData = new MeshData();

            BuildHyperBolBarrelCylinderSide(Radius, height, sliceCount, stackCount, meshData);

            BuildCylinderTopCap(Radius, height, sliceCount, meshData);

            // BuildCylinderBottomCap(Radius, height, sliceCount, meshData);

            return meshData;
        }


        /// <summary>
        ///  does not work
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="numSubdivisions"></param>
        /// <returns></returns>
        public static MeshData CreateGeosphere(float radius, int numSubdivisions)
        {
            var meshData = new MeshData();

            // Put a cap on the number of subdivisions.
            numSubdivisions = Math.Min(numSubdivisions, 6);

            // Approximate a sphere by tesselating an icosahedron.

            const float x = 0.525731f;
            const float z = 0.850651f;

            Vector3[] positions =
            {
                new Vector3(-x, 0, z), new Vector3(x, 0, z),
                new Vector3(-x, 0, -z), new Vector3(x, 0, -z),
                new Vector3(0, z, x), new Vector3(0, z, -x),
                new Vector3(0, -z, x), new Vector3(0, -z, -x),
                new Vector3(z, x, 0), new Vector3(-z, x, 0),
                new Vector3(z, -x, 0), new Vector3(-z, -x, 0)
            };

            int[] indices =
            {
                1,4,0, 4,9,0, 4,5,9, 8,5,4, 1,8,4,
                1,10,8, 10,3,8, 8,3,5, 3,2,5, 3,7,2,
                3,10,7, 10,6,7, 6,11,7, 6,0,11, 6,1,0,
                10,1,6, 11,0,9, 2,11,9, 5,2,9, 11,2,7
            };

            meshData.Vertices.AddRange(positions.Select(position => new Vertex { Position = position }));
            meshData.Indices32.AddRange(indices);

            for (int i = 0; i < numSubdivisions; i++)
                Subdivide(meshData);

            // Project vertices onto sphere and scale.
            for (int i = 0; i < positions.Length; i++)
            {
                // Project onto unit sphere.
                Vector3 normal = Vector3.Normalize(positions[i]);

                // Project onto sphere.
                Vector3 position = radius * normal;

                // Derive texture coordinates from spherical coordinates.
                float theta = MathHelper.Atan2f(positions[i].Z, positions[i].X) + MathUtil.Pi;

                float phi = MathHelper.Acosf(positions[i].Y / radius);

                Vector2 texCoord = new Vector2(
                    theta / MathUtil.TwoPi,
                    phi / MathUtil.TwoPi);

                // Partial derivative of P with respect to theta.
                Vector3 tangentU = new Vector3(
                    -radius * MathHelper.Sinf(phi) * MathHelper.Sinf(theta),
                    0.0f,
                    radius * MathHelper.Sinf(phi) * MathHelper.Cosf(theta));

                meshData.Vertices.Add(new Vertex(position, normal, tangentU, texCoord));
            }

            return meshData;
        }




        private static void Subdivide(MeshData meshData)
        {
            // Save a copy of the input geometry.
            Vertex[] verticesCopy = meshData.Vertices.ToArray();
            int[] indicesCopy = meshData.Indices32.ToArray();

            meshData.Vertices.Clear();
            meshData.Indices32.Clear();

            //       v1
            //       *
            //      / \
            //     /   \
            //  m0*-----*m1
            //   / \   / \
            //  /   \ /   \
            // *-----*-----*
            // v0    m2     v2

            int numTriangles = indicesCopy.Length / 3;

            for (int i = 0; i < numTriangles; i++)
            {
                Vertex v0 = verticesCopy[indicesCopy[i * 3 + 0]];
                Vertex v1 = verticesCopy[indicesCopy[i * 3 + 1]];
                Vertex v2 = verticesCopy[indicesCopy[i * 3 + 2]];

                //
                // Generate the midpoints.
                //

                Vertex m0 = MidPoint(v0, v1);
                Vertex m1 = MidPoint(v1, v2);
                Vertex m2 = MidPoint(v0, v2);

                //
                // Add new geometry.
                //

                meshData.Vertices.Add(v0); // 0
                meshData.Vertices.Add(v1); // 1
                meshData.Vertices.Add(v2); // 2
                meshData.Vertices.Add(m0); // 3
                meshData.Vertices.Add(m1); // 4
                meshData.Vertices.Add(m2); // 5

                meshData.Indices32.Add(i * 6 + 0);
                meshData.Indices32.Add(i * 6 + 3);
                meshData.Indices32.Add(i * 6 + 5);

                meshData.Indices32.Add(i * 6 + 3);
                meshData.Indices32.Add(i * 6 + 4);
                meshData.Indices32.Add(i * 6 + 5);

                meshData.Indices32.Add(i * 6 + 5);
                meshData.Indices32.Add(i * 6 + 4);
                meshData.Indices32.Add(i * 6 + 2);

                meshData.Indices32.Add(i * 6 + 3);
                meshData.Indices32.Add(i * 6 + 1);
                meshData.Indices32.Add(i * 6 + 4);
            }
        }

        private static Vertex MidPoint(Vertex v0, Vertex v1)
        {
            // Compute the midpoints of all the attributes. Vectors need to be normalized
            // since linear interpolating can make them not unit length.
            Vector3 pos = 0.5f * (v0.Position + v1.Position);
            Vector3 normal = Vector3.Normalize(0.5f * (v0.Normal + v1.Normal));
            Vector3 tangent = Vector3.Normalize(0.5f * (v0.TangentU + v1.TangentU));
            Vector2 tex = 0.5f * (v0.TexC + v1.TexC);

            return new Vertex(pos, normal, tangent, tex);
        }

        private static void BuildCylinderSide(float bottomRadius, float topRadius,
            float height, int sliceCount, int stackCount, MeshData meshData)
        {
            float stackHeight = height / stackCount;

            // Amount to increment radius as we move up each stack level from bottom to top.
            float radiusStep = (topRadius - bottomRadius) / stackCount;

            int ringCount = stackCount + 1;

            // Compute vertices for each stack ring starting at the bottom and moving up.
            for (int i = 0; i < ringCount; i++)
            {
                float y = -0.5f * height + i * stackHeight;

                float r = bottomRadius + i * radiusStep;

                // Vertices of ring.
                float dTheta = 2.0f * MathUtil.Pi / sliceCount;

                for (int j = 0; j <= sliceCount; j++)
                {
                    float c = MathHelper.Cosf(j * dTheta);
                    float s = MathHelper.Sinf(j * dTheta);

                    var pos = new Vector3(r * c, y, r * s);
                    var uv = new Vector2((float)j / sliceCount, 1f - (float)i / stackCount);
                    var tangent = new Vector3(-s, 0.0f, c);

                    float dr = bottomRadius - topRadius;
                    var bitangent = new Vector3(dr * c, -height, dr * s);

                    var normal = Vector3.Cross(tangent, bitangent);
                    normal.Normalize();
                    meshData.Vertices.Add(new Vertex(pos, normal, tangent, uv));
                }
            }

            // Add one because we duplicate the first and last vertex per ring
            // since the texture coordinates are different.
            int ringVertexCount = sliceCount + 1;

            // Compute indices for each stack.
            for (int i = 0; i < stackCount; i++)
            {
                for (int j = 0; j < sliceCount; j++)
                {
                    meshData.Indices32.Add(i * ringVertexCount + j);
                    meshData.Indices32.Add((i + 1) * ringVertexCount + j);
                    meshData.Indices32.Add((i + 1) * ringVertexCount + j + 1);

                    meshData.Indices32.Add(i * ringVertexCount + j);
                    meshData.Indices32.Add((i + 1) * ringVertexCount + j + 1);
                    meshData.Indices32.Add(i * ringVertexCount + j + 1);
                }
            }
        }

        private static void BuildBellyBarrelCylinderSide(float Radius, 
           float height, int sliceCount, int stackCount, MeshData meshData)
        {
            float stackHeight = height / stackCount;

            // Amount to increment radius as we move up each stack level from bottom to top.
            float radiusStep = 0.0f;// (Radius - Radius) / stackCount;

            int ringCount = stackCount + 1;

            // Compute vertices for each stack ring starting at the bottom and moving up.
            for (int i = 0; i < ringCount; i++)
            {
                float h = i * stackHeight;

                float y = - 0.5f * height + h; // i * stackHeight;

                /*
                 
                belly barrel

                r(h) = r_0 + r_max * (4h/h_max) * (1 - (h/h_max)²)

                r_0 - start radius
                h - height
                r_max - max radius
                h_max - max height

                */

                float r = Radius + Radius * (h / height) * (1 - (h / height) * (h / height));


                // Vertices of ring.
                float dTheta = 2.0f * MathUtil.Pi / sliceCount;

                for (int j = 0; j <= sliceCount; j++)
                {
                    float c = MathHelper.Cosf(j * dTheta);
                    float s = MathHelper.Sinf(j * dTheta);

                    var pos = new Vector3(r * c, y, r * s);
                    var uv = new Vector2((float)j / sliceCount, 1f - (float)i / stackCount);
                    var tangent = new Vector3(-s, 0.0f, c);

                    float dr = Radius - Radius;
                    var bitangent = new Vector3(dr * c, -height, dr * s);

                    var normal = Vector3.Cross(tangent, bitangent);
                    normal.Normalize();
                    meshData.Vertices.Add(new Vertex(pos, normal, tangent, uv));
                }
            }

            // Add one because we duplicate the first and last vertex per ring
            // since the texture coordinates are different.
            int ringVertexCount = sliceCount + 1;

            // Compute indices for each stack.
            for (int i = 0; i < stackCount; i++)
            {
                for (int j = 0; j < sliceCount; j++)
                {
                    meshData.Indices32.Add(i * ringVertexCount + j);
                    meshData.Indices32.Add((i + 1) * ringVertexCount + j);
                    meshData.Indices32.Add((i + 1) * ringVertexCount + j + 1);

                    meshData.Indices32.Add(i * ringVertexCount + j);
                    meshData.Indices32.Add((i + 1) * ringVertexCount + j + 1);
                    meshData.Indices32.Add(i * ringVertexCount + j + 1);
                }
            }
        }

        private static void BuildHyperBolBarrelCylinderSide(float Radius,
          float height, int sliceCount, int stackCount, MeshData meshData)
        {
            float stackHeight = height / stackCount;

            // Amount to increment radius as we move up each stack level from bottom to top.
            //float radiusStep = 0.0f;// (Radius - Radius) / stackCount;

            int ringCount = stackCount + 1;

            // Compute vertices for each stack ring starting at the bottom and moving up.
            for (int i = 0; i < ringCount; i++)
            {
                float h = i * stackHeight;

                float y = -0.5f * height + h; // i * stackHeight;


                float r = Radius - Radius * (h / height) * (1 - (h / height) * (h / height));


                // Vertices of ring.
                float dTheta = 2.0f * MathUtil.Pi / sliceCount;

                for (int j = 0; j <= sliceCount; j++)
                {
                    float c = MathHelper.Cosf(j * dTheta);
                    float s = MathHelper.Sinf(j * dTheta);

                    var pos = new Vector3(r * c, y, r * s);
                    var uv = new Vector2((float)j / sliceCount, 1f - (float)i / stackCount);
                    var tangent = new Vector3(-s, 0.0f, c);

                    float dr = Radius - Radius;
                    var bitangent = new Vector3(dr * c, -height, dr * s);

                    var normal = Vector3.Cross(tangent, bitangent);
                    normal.Normalize();
                    meshData.Vertices.Add(new Vertex(pos, normal, tangent, uv));
                }
            }

            // Add one because we duplicate the first and last vertex per ring
            // since the texture coordinates are different.
            int ringVertexCount = sliceCount + 1;

            // Compute indices for each stack.
            for (int i = 0; i < stackCount; i++)
            {
                for (int j = 0; j < sliceCount; j++)
                {
                    meshData.Indices32.Add(i * ringVertexCount + j);
                    meshData.Indices32.Add((i + 1) * ringVertexCount + j);
                    meshData.Indices32.Add((i + 1) * ringVertexCount + j + 1);

                    meshData.Indices32.Add(i * ringVertexCount + j);
                    meshData.Indices32.Add((i + 1) * ringVertexCount + j + 1);
                    meshData.Indices32.Add(i * ringVertexCount + j + 1);
                }
            }
        }



        private static void BuildCylinderTopCap(float topRadius, float height,
            int sliceCount, MeshData meshData)
        {
            int baseIndex = meshData.Vertices.Count;

            float y = 0.5f * height;
            float dTheta = 2.0f * MathUtil.Pi / sliceCount;

            // Duplicate cap ring vertices because the texture coordinates and normals differ.
            for (int i = 0; i <= sliceCount; i++)
            {
                float x = topRadius * MathHelper.Cosf(i * dTheta);
                float z = topRadius * MathHelper.Sinf(i * dTheta);

                // Scale down by the height to try and make top cap texture coord area
                // proportional to base.
                float u = x / height + 0.5f;
                float v = z / height + 0.5f;

                meshData.Vertices.Add(new Vertex(
                    new Vector3(x, y, z), new Vector3(0, 1, 0), new Vector3(1, 0, 0), new Vector2(u, v)));
            }

            // Cap center vertex.
            meshData.Vertices.Add(new Vertex(
                new Vector3(0, y, 0), new Vector3(0, 1, 0), new Vector3(1, 0, 0), new Vector2(0.5f, 0.5f)));

            // Index of center vertex.
            int centerIndex = meshData.Vertices.Count - 1;

            for (int i = 0; i < sliceCount; i++)
            {
                meshData.Indices32.Add(centerIndex);
                meshData.Indices32.Add(baseIndex + i + 1);
                meshData.Indices32.Add(baseIndex + i);
            }
        }

        private static void BuildCylinderBottomCap(float bottomRadius, float height,
            int sliceCount, MeshData meshData)
        {
            int baseIndex = meshData.Vertices.Count;
            float y = -0.5f * height;

            // vertices of ring
            float dTheta = 2.0f * MathUtil.Pi / sliceCount;
            for (int i = 0; i <= sliceCount; i++)
            {
                float x = bottomRadius * MathHelper.Cosf(i * dTheta);
                float z = bottomRadius * MathHelper.Sinf(i * dTheta);

                // Scale down by the height to try and make top cap texture coord area
                // proportional to base.
                float u = x / height + 0.5f;
                float v = z / height + 0.5f;

                meshData.Vertices.Add(new Vertex(new Vector3(x, y, z), new Vector3(0, -1, 0), new Vector3(1, 0, 0), new Vector2(u, v)));
            }

            // Cap center vertex.
            meshData.Vertices.Add(new Vertex(new Vector3(0, y, 0), new Vector3(0, -1, 0), new Vector3(1, 0, 0), new Vector2(0.5f, 0.5f)));

            // Cache the index of center vertex.
            int centerIndex = meshData.Vertices.Count - 1;

            for (int i = 0; i < sliceCount; i++)
            {
                meshData.Indices32.Add(centerIndex);
                meshData.Indices32.Add(baseIndex + i + 1);
                meshData.Indices32.Add(baseIndex + i);
            }
        }

        public static MeshData BuildFullscreenQuad()
        {
            var meshData = new MeshData();

            meshData.Vertices.Add(new Vertex(
                -1.0f, -1.0f, 0.0f,
                0.0f, 0.0f, -1.0f,
                1.0f, 0.0f, 0.0f,
                0.0f, 1.0f));
            meshData.Vertices.Add(new Vertex(
                -1.0f, +1.0f, 0.0f,
                0.0f, 0.0f, -1.0f,
                1.0f, 0.0f, 0.0f,
                0.0f, 0.0f));
            meshData.Vertices.Add(new Vertex(
                +1.0f, +1.0f, 0.0f,
                0.0f, 0.0f, -1.0f,
                1.0f, 0.0f, 0.0f,
                1.0f, 0.0f));
            meshData.Vertices.Add(new Vertex(
                +1.0f, -1.0f, 0.0f,
                0.0f, 0.0f, -1.0f,
                1.0f, 0.0f, 0.0f,
                1.0f, 1.0f));
            meshData.Indices32.Add(0);
            meshData.Indices32.Add(1);
            meshData.Indices32.Add(2);
            meshData.Indices32.Add(0);
            meshData.Indices32.Add(2);
            meshData.Indices32.Add(3);
            return meshData;
        }

        private static float GetGaussFuncValue(float x, float z) => 4 * (float)Math.Exp(- (Math.Pow(x, 2) + Math.Pow(z, 2)));

        private static float GetRotSymFuncValue(float x, float z)
        {

            double r = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(z, 2));

            double result = (3.0f * (Math.Sin(3 * r)) / (3 * r));

            return (float)result;

        }

        private static float GetParabolicValue(float x, float z)
        {

            double a = 0.75;

            double b = 0.75;

            double p = 1;// 0.99
                         
            double q = -5;// 0.99;

            /*
            double termx = Math.Pow(x / 2, 2) / Math.Pow(a / 2, 2); // Math.Sin
            double termz = Math.Pow(z / 2, 2) / Math.Pow(b / 2, 2);
            */


            double termx = a * Math.Pow(x, 2);  //x²

            double termz = b * Math.Pow(z, 2);  //z²


            return (float)( (termx + termz) / (2 * p) + q);    // 1/2p (x² + z²)

            // https://www.wolframalpha.com/input?i=%24%24z+%3D+%5Cfrac%7Bx2%7D%7Ba2%7D+%2B+%5Cfrac%7By2%7D%7Bb2%7D%24%24

        }
        private static float GetRotParabolicValue(float x, float z)
        {

            double a = 0.75;

            double b = 0.75;

            double p = 1;// 0.99

            double q = -1;// 0.99;

            double u = 2;// 0.99;

            /*
            double termx = Math.Pow(x / 2, 2) / Math.Pow(a / 2, 2); // Math.Sin
            double termz = Math.Pow(z / 2, 2) / Math.Pow(b / 2, 2);
            */


            double termx = Math.Pow(Math.Sqrt(u) * Math.Sin(x),2);   

            double termz = Math.Pow(Math.Sqrt(u) * Math.Cos(z),2);   


            return (float)((termx + termz) + q) ;    // 1/2p (x² + z²)

            // https://www.wolframalpha.com/input?i=%24%24z+%3D+%5Cfrac%7Bx2%7D%7Ba2%7D+%2B+%5Cfrac%7By2%7D%7Bb2%7D%24%24

        }

        private static float GetSaddleParabolicValue(float x, float z)
        {

            ///Saddle surface:
            /// z = x ^ 2 - y ^ 2

            
            double a = 1.0f;

            double b = 1.0f;


            /*
            double p = 1;// 0.99
            double u = 2;// 0.99;
            */

            /*
            double termx = Math.Pow(x / 2, 2) / Math.Pow(a / 2, 2); // Math.Sin
            double termz = Math.Pow(z / 2, 2) / Math.Pow(b / 2, 2);
            */

            double q = -1;// 0.99;

            double termx = a * Math.Pow(x * 1.25f, 2);

            double termz = b * Math.Pow(z * 1.2f, 2);


            return (float)((termx - termz) + q);   

            // https://www.wolframalpha.com/input?i=%24%24z+%3D+%5Cfrac%7Bx2%7D%7Ba2%7D+%2B+%5Cfrac%7By2%7D%7Bb2%7D%24%24

        }

        private static float GetBarrelValue(float x, float z)
        {

            ///Barrel  surface:
            /// z = r * (1 - (2z/h)²2)


            double a = 1.0f;

            double b = 1.0f;


            /*
            double p = 1;// 0.99
            double u = 2;// 0.99;
            */

            /*
            double termx = Math.Pow(x / 2, 2) / Math.Pow(a / 2, 2); // Math.Sin
            double termz = Math.Pow(z / 2, 2) / Math.Pow(b / 2, 2);
            */

            double q = -1;// 0.99;

            double termx = a * Math.Pow(x * 1.25f, 2);

            double termz = b * Math.Pow(z * 1.2f, 2);




            return (float)((termx - termz) + q);

            // https://www.wolframalpha.com/input?i=%24%24z+%3D+%5Cfrac%7Bx2%7D%7Ba2%7D+%2B+%5Cfrac%7By2%7D%7Bb2%7D%24%24

        }


    }
}
