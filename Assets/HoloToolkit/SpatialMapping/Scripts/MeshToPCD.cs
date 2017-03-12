using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

namespace HoloToolkit.Unity.SpatialMapping
{

    // Method tog get the current meshes
    // SpatialMappingManager.Instance.GetMeshes() <-- Returns a list of meshes

    class MeshToPCD : SpatialMappingSource
    {
        List<Mesh> meshes;

        //string appdataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string appdataPath = "C:\\Users\\phil_\\AppData\\Roaming";
        public void convertMeshToPCD()
        {
            // Calculate Time
            var timer = System.Diagnostics.Stopwatch.StartNew();

            meshes = SpatialMappingManager.Instance.GetMeshes(); // Returns the meshes from the hololens.

            String path = Path.Combine(appdataPath, "Hololens");
            String fileName = "HoloPointCloud.pcd";

            Debug.Log(String.Format("Saving mesh file: {0}", Path.Combine(path,fileName)));

            LinkedList<Vector3> ls = new LinkedList<Vector3>();
            int points = 0;

            var pathcombine = Path.Combine(path, fileName);
            if (File.Exists(pathcombine))
            {
                File.Delete(pathcombine);
            }

            using (StreamWriter sw = new StreamWriter(File.OpenWrite(pathcombine)))
            {
                
                // First set the requirements for the PCD file type
                // For more info see: http://pointclouds.org/documentation/tutorials/pcd_file_format.php

                sw.WriteLine("# .PCD v.7 - Point Cloud Data file format");
                sw.WriteLine("VERSION .7");
                sw.WriteLine("FIELDS x y z");
                sw.WriteLine("SIZE 4 4 4");
                sw.WriteLine("TYPE F F F");
                sw.WriteLine("COUNT 1 1 1");
                sw.WriteLine("VIEWPOINT 0 0 0 1 0 0 0");

                foreach (Mesh m in meshes)
                {
                    var t0 = m.vertices;
                    var t1 = t0.Length;
                    for(int i = 0; i < t0.Length; i++)
                    {
                        t0[i].x = t0[i].x * (-1);
                        ls.AddLast(t0[i]);
                        points++;
                    }

                }

                sw.WriteLine("POINTS " + points);
                sw.WriteLine("DATA aciii");

                foreach (Vector3 vertex in ls)
                {
                    sw.WriteLine(vertex.x + " " + vertex.y + " " + vertex.z);
                }

            }
            timer.Stop();
            Debug.Log(String.Format("Done Saving mesh file: {0}", Path.Combine(path, fileName)) + " Time in miliseconds : " + timer.ElapsedMilliseconds);

        }

        // Called every frame by the Unity engine.
        private void Update()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
              
            if (Input.GetKeyUp(KeyCode.I))
            {
                convertMeshToPCD();
            }
#endif
        }

    }
}
