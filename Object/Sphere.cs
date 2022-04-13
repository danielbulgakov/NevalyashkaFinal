using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nevalyashka.Object
{
    internal class Sphere
    {
        private List<float> Vertices  = new List<float>();
        private List<float> Normals   = new List<float>();
        private List<float> TexCoords = new List<float>();
        private List <uint> Indices   = new List <uint>();

        private float X, Y, Z, R;
        private int SectorCount, StackCount;

        public Sphere(float R, float X, float Y, float Z, int SectorCount = 72, int StackCount = 36)
        {
            this.R = R;
            this.X = X;
            this.Y = Y;
            this.Z = Z;
            this.SectorCount = SectorCount;
            this.StackCount = StackCount;
            CalculateVertecies();
            CalculateIndices();
        }

        public float[] GetVertecies()
        {
            return Vertices.ToArray();
        }

        public float[] GetNormals()
        {
            return Normals.ToArray();
        }

        public uint[] GetIndices()
        {
            return Indices.ToArray();
        }

        public float[] GetAll(bool isNormals = true, bool isTexCoords = true)
        {
            List<float> result = new List<float>();

            for (int i = 0; i < Vertices.Count / 3; ++i)
            {
                    result.Add(Vertices[i * 3]);
                    result.Add(Vertices[i * 3 + 1]);
                    result.Add(Vertices[i * 3 + 2]);

                if (isNormals)
                {
                    result.Add(Normals[i * 3]);
                    result.Add(Normals[i * 3 + 1]);
                    result.Add(Normals[i * 3 + 2]);
                }

                if (isTexCoords)
                {
                    result.Add(TexCoords[i * 2]);
                    result.Add(TexCoords[i * 2 + 1]);
                }
            }


            return result.ToArray();
        }

        private void CalculateVertecies()
        {
            const float PI = 3.1415926f;

            float x, y, z, xy;                              // vertex position
            float nx, ny, nz, lengthInv = 1.0f / R;         // vertex normal
            float s, t;                                     // vertex texCoord

            float sectorStep = 2 * PI / SectorCount;
            float stackStep = PI / StackCount;
            float sectorAngle, stackAngle;

            for (int i = 0; i <= StackCount; ++i)
            {
                stackAngle = PI / 2 - i * stackStep;        // starting from pi/2 to -pi/2
                xy = R * (float)Math.Cos(stackAngle);             // r * cos(u)
                z = R * (float)Math.Sin(stackAngle) ;          // r * sin(u)
                for (int j = 0; j <= SectorCount; ++j)
                {
                    sectorAngle = j * sectorStep;           // starting from 0 to 2pi

                    // vertex position (x, y, z)
                    x = xy * (float)Math.Cos(sectorAngle) ;             // r * cos(u) * cos(v)
                    y = xy * (float)Math.Sin(sectorAngle) ;             // r * cos(u) * sin(v)
                    Vertices.Add(x + X);
                    Vertices.Add(y + Y);
                    Vertices.Add(z + Z);

                    // normalized vertex normal (nx, ny, nz)
                    nx = x * lengthInv;
                    ny = y * lengthInv;
                    nz = z * lengthInv;
                    Normals.Add(nx);
                    Normals.Add(ny);
                    Normals.Add(nz);

                    // vertex tex coord (s, t) range between [0, 1]
                    s = (float)j / SectorCount;
                    t = (float)i / StackCount;
                    TexCoords.Add(s);
                    TexCoords.Add(t);

                }
            }
        }

        private void CalculateIndices()
        {
            uint k1, k2;

            for (int i = 0; i < StackCount; ++i)
            {
                k1 = (uint)(i * (SectorCount + 1));     // beginning of current stack
                k2 = (uint)(k1 + SectorCount + 1);      // beginning of next stack

                for (int j = 0; j < SectorCount; ++j, ++k1, ++k2)
                {
                    // 2 triangles per sector excluding first and last stacks
                    // k1 => k2 => k1+1
                    if (i != 0)
                    {
                        Indices.Add(k1);
                        Indices.Add(k2);
                        Indices.Add(k1 + 1);
                    }

                    // k1+1 => k2 => k2+1
                    if (i != (StackCount - 1))
                    {
                        Indices.Add(k1 + 1);
                        Indices.Add(k2);
                        Indices.Add(k2 + 1);
                    }

                }
            }
        }

    }
}
