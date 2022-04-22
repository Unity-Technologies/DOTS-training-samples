using Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Profiling;
using Color = UnityEngine.Color;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    public class ComputeShaderManager : MonoBehaviour
    {
        private struct MeshProperties
        {
            public Matrix4x4 mat;
            public Vector4 color;

            public static int Size()
            {
                return
                    sizeof(float) * 4 * 4 + // matrix;
                    sizeof(float) * 4;      // color;
            }
        }

        private struct VerletPoint
        {
            //24 bytes
            public Vector3 oldPosition;
            public Vector3 currentPosition;

            //28
            public uint anchored;
            //32
            public uint neighborCount;
        };

        private struct Link
        {
            //8
            public int point1Index;
            public int point2Index;
            //12
            public float length;

            public Vector3 direction;
            public Vector2 fillvariable2;
        };

        private struct IslandDistribution
        {
            public int linkStartIndex;
            public int pointAllocator;

            public Vector3 fillvariable;
            public Vector3 fillvariable2;
        }


        public ComputeShader m_computeShader;

        ComputeBuffer m_pointsBuffer;
        ComputeBuffer m_linksBuffer;
        ComputeBuffer m_islandDistribBuffer;

        ComputeBuffer meshPropertiesBuffer;
        ComputeBuffer argsBuffer;


        VerletPoint[] points;
        Link[] links;
        IslandDistribution[] islandDistributions;
     

        public Material linkMat;
        public Mesh linkMesh;

        private int m_pointKernelIndex;
        private int m_constraintKernelIndex;


        private const int ThreadGroupSize = 64;
        public static ComputeShaderManager Instance { get; private set; }
        private const string PointKernelName = "PointDisplacement";
        private const string ConstraintKernelName = "ConstraintPass";
        private const int InstancesPerBatch = 1023;

        public float damping;
        public float friction;
        public float breakResistance;
        public float expForce;
        public int constraintIterations;
        public float gravityForce;
        //
        public Vector3 tornadoPosition;
        public float tornadoMaxForceDist;
        public float tornadoHeight;
        public float tornadoForce;
        public float tornadoFader;
        public float tornadoUpForce;
        public float tornadoInwardForce;

        private bool m_ReadyToCompute;

        private int m_pointThreadGroup;
        private int m_ConstraintThreadGroup;

        Matrix4x4[][] matrices;
        private void Awake()
        {
            Instance = this;
           
        }

        private float elapsedT;

        private Bounds bounds = new Bounds(Vector3.zero, Vector3.one * 300f);
        public void Initialize(NativeArray<Components.VerletPoint> pointsDOTS, NativeArray<Components.Link> linksDOTS, NativeArray<int> linkStartIndicesDOTS, NativeArray<int> islandAllocatorsDOTS)
        {
            //initializing arrays
            points = new VerletPoint[pointsDOTS.Length];
            links = new Link[linksDOTS.Length];     
            islandDistributions = new IslandDistribution[islandAllocatorsDOTS.Length];






            //init points & islands repartition
            for (int i = 0; i < islandAllocatorsDOTS.Length; i++)
            {
                islandDistributions[i] = new IslandDistribution()
                {
                    linkStartIndex = linkStartIndicesDOTS[i],
                    pointAllocator = islandAllocatorsDOTS[i],                    
                };
            }
            for (int i = 0; i < pointsDOTS.Length; i++)
            {
                var dotsEquivalent = pointsDOTS[i];
                points[i] = new VerletPoint()
                {
                   oldPosition = dotsEquivalent.oldPosition,
                   currentPosition = dotsEquivalent.currentPosition,
                   anchored = (uint)(dotsEquivalent.anchored > 0 ? 1 : 0),
                   neighborCount = dotsEquivalent.neighborCount,
                };
            }         
           
            //init links & matrices
            int batch = 0;
            List<List<Matrix4x4>> matricesList = new List<List<Matrix4x4>>();
            matricesList.Add(new List<Matrix4x4>());

            for (int i = 0; i < links.Length; i++)
            {
                var dotsEquivalent = linksDOTS[i];
                links[i] = new Link()
                {
                    point1Index = dotsEquivalent.point1Index,
                    point2Index = dotsEquivalent.point2Index,
                    length = dotsEquivalent.length,
                };

                matricesList[batch].Add(new Matrix4x4());
                if (matricesList[batch].Count == InstancesPerBatch)
                {
                    batch++;
                    matricesList.Add(new List<Matrix4x4>());
                }
            }            

            matrices = new Matrix4x4[matricesList.Count][];
            for (int i = 0; i < matrices.Length; i++)
            {
                matrices[i] = matricesList[i].ToArray();
            }

            //defining the number of GPU threads based on the number of data (1 point per thread - 1 thread per constraint group)
            m_pointThreadGroup = Mathf.CeilToInt((float)points.Length / ThreadGroupSize);
            m_ConstraintThreadGroup = Mathf.CeilToInt((float)islandAllocatorsDOTS.Length / ThreadGroupSize);

            InitializeComputeShader();

            InitializeRenderingBuffers();
        }

       
        void InitializeComputeShader()
        {
            m_pointKernelIndex = m_computeShader.FindKernel(PointKernelName);
            m_constraintKernelIndex = m_computeShader.FindKernel(ConstraintKernelName);

            //number of point per thread group
            m_computeShader.SetInt("pointSize", 1);           
            m_computeShader.SetInt("maxPoints", points.Length);    
            //constraint pass group repartition
            m_computeShader.SetInt("maxLinks", links.Length);
            m_computeShader.SetInt("islandNumber", islandDistributions.Length);

            //physic & tornado settings
            m_computeShader.SetFloat("gravityForce", gravityForce);
            m_computeShader.SetFloat("invDamping", 1 - damping);
            m_computeShader.SetFloat("friction", friction);
            m_computeShader.SetFloat("tornadoMaxForceDist", tornadoMaxForceDist);
            m_computeShader.SetFloat("tornadoHeight", tornadoHeight);
            m_computeShader.SetFloat("tornadoForce", tornadoForce);
            m_computeShader.SetFloat("tornadoUpForce", tornadoUpForce);
            m_computeShader.SetFloat("tornadoInwardForce", tornadoInwardForce);
            m_computeShader.SetFloat("breakResistance", breakResistance);

            //creating the buffers 
            m_linksBuffer = new ComputeBuffer(links.Length, 32);
            m_pointsBuffer = new ComputeBuffer(points.Length, 32);
            m_islandDistribBuffer = new ComputeBuffer(islandDistributions.Length, 32);

           
            m_linksBuffer.SetData(links);
            m_islandDistribBuffer.SetData(islandDistributions);

            m_pointsBuffer.SetData(points);

            m_computeShader.SetBuffer(m_constraintKernelIndex, "links", m_linksBuffer);
            m_computeShader.SetBuffer(m_constraintKernelIndex, "islandDistribution", m_islandDistribBuffer);
            m_computeShader.SetBuffer(m_pointKernelIndex, "points", m_pointsBuffer);
            m_computeShader.SetBuffer(m_constraintKernelIndex, "points", m_pointsBuffer);

            m_ReadyToCompute = true;
        }

       
        private void Update()
        {
            if (!m_ReadyToCompute) return;

            elapsedT = Time.realtimeSinceStartup;

            Profiler.BeginSample("Enter point displacement phase");
            PointPass();
            Profiler.EndSample();

            Profiler.BeginSample("Enter constraint phase");
            ConstraintPass();
           
            Profiler.EndSample();
            elapsedT = Time.realtimeSinceStartup - elapsedT;

            Graphics.DrawMeshInstancedIndirect(linkMesh, 0, linkMat, bounds, argsBuffer);
            //RenderPass();
        }


        private void PointPass()
        {
            
            tornadoFader = Mathf.Clamp01(tornadoFader + Time.deltaTime / 10f);
            m_computeShader.SetFloat("time", Time.time);
            m_computeShader.SetFloat("tornadoFader", tornadoFader);
            m_computeShader.SetVector("tornadoPosition", tornadoPosition);

           
            m_computeShader.Dispatch(m_pointKernelIndex, m_pointThreadGroup, 1, 1);
          
        }

        private void ConstraintPass()
        {
               
            m_computeShader.Dispatch(m_constraintKernelIndex, m_ConstraintThreadGroup, 1, 1);
                  
        }   

        private void OnDestroy()
        {
            m_pointsBuffer?.Release();
            m_linksBuffer?.Release();
            m_islandDistribBuffer?.Release();
        }

        private MeshProperties[] properties;
        private void InitializeRenderingBuffers()
        {
            // Argument buffer used by DrawMeshInstancedIndirect.
            uint[] args = new uint[5] { 0, 0, 0, 0, 0 };
            // Arguments for drawing mesh.
            // 0 == number of triangle indices, 1 == population, others are only relevant if drawing submeshes.
            args[0] = (uint)linkMesh.GetIndexCount(0);
            args[1] = (uint)links.Length;
            args[2] = (uint)linkMesh.GetIndexStart(0);
            args[3] = (uint)linkMesh.GetBaseVertex(0);
            argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
            argsBuffer.SetData(args);
           

            // Initialize buffer with the given population.
             properties = new MeshProperties[links.Length];
            for (int i = 0; i < links.Length; i++)
            {
                var l = links[i];
                var point1 = points[l.point1Index].currentPosition;
                var point2 = points[l.point2Index].currentPosition;

                Vector3 d = point2 - point1;

                float dist = (float)Math.Sqrt(d.x * d.x + d.y * d.y + d.z * d.z);

                var quat = d != Vector3.zero ? Quaternion.LookRotation(d / dist) : Quaternion.identity;

                MeshProperties props = new MeshProperties();
                Vector3 position = new Vector3((point1.x + point2.x) * .5f, (point1.y + point2.y) * .5f, (point1.z + point2.z) * .5f);
                Quaternion rotation = quat;
                Vector3 scale = new Vector3(0.5f, 0.5f, l.length);

                props.mat = Matrix4x4.TRS(position, rotation, scale);


                props.color = Color.Lerp(Color.red, Color.blue, Random.value);

                properties[i] = props;
            }
            

            meshPropertiesBuffer = new ComputeBuffer(links.Length, MeshProperties.Size());
            meshPropertiesBuffer.SetData(properties);
            m_computeShader.SetBuffer(m_constraintKernelIndex, "_Properties", meshPropertiesBuffer);
            linkMat.SetBuffer("_Properties", meshPropertiesBuffer);

        }

        #region rendering
        void RenderPass()
        {
            for (int i = 0; i < links.Length; i++)
            {
                var link = links[i];

                var point1 = points[link.point1Index].currentPosition;
                var point2 = points[link.point2Index].currentPosition;              
              
                var quat = link.direction != Vector3.zero ? Quaternion.LookRotation(link.direction) : Quaternion.identity;

                var matrice = Matrix4x4.TRS(new Vector3((point1.x + point2.x) * .5f, (point1.y + point2.y) * .5f, (point1.z + point2.z) * .5f),
                                           quat,
                                           new Vector3(0.5f, 0.5f, link.length));

                matrices[i / InstancesPerBatch][i % InstancesPerBatch] = matrice;             
            }

            for (int i = 0; i < matrices.Length; i++)
            {
                Graphics.DrawMeshInstanced(linkMesh, 0, linkMat, matrices[i], matrices[i].Length);
                

            }
        }
        #endregion

        #region GUI
        private Rect logRect = new Rect();
        private GUIStyle labelStyle;
        private void OnGUI()
        {
            labelStyle = new GUIStyle(GUI.skin.label);

            logRect.width = Screen.width;
            logRect.height = Screen.height / 3;
            labelStyle.fontSize = Screen.width / 50;
            string content = "";
            content += $"Point thread group : {m_pointThreadGroup} total threads : {m_pointThreadGroup * ThreadGroupSize}\n";
            content += $"Constraint thread group : {m_ConstraintThreadGroup} total threads : {m_ConstraintThreadGroup * ThreadGroupSize}\n";
            content += $"Compute time : {(elapsedT * 1000).ToString("0.000")}ms\n";
            content += $"Simulated Points  : {points.Length}\n";

            GUI.Label(logRect, content, labelStyle);
        }

        #endregion


        private void old_ConstraintPass()
        {
            m_pointsBuffer.SetData(points);
            m_linksBuffer.SetData(links);
            m_islandDistribBuffer.SetData(islandDistributions);


            m_computeShader.SetBuffer(m_constraintKernelIndex, "links", m_linksBuffer);
            m_computeShader.SetBuffer(m_constraintKernelIndex, "points", m_pointsBuffer);
            m_computeShader.SetBuffer(m_constraintKernelIndex, "islandDistribution", m_islandDistribBuffer);
            m_computeShader.Dispatch(m_constraintKernelIndex, m_ConstraintThreadGroup, 1, 1);
            m_pointsBuffer.GetData(points);
            m_linksBuffer.GetData(links);
            m_islandDistribBuffer.GetData(islandDistributions);
        }
    }
}
