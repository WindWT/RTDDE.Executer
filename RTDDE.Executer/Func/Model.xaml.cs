using FreeImageAPI;
using HelixToolkit.Wpf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RTDDE.Executer
{
    /// <summary>
    /// Model.xaml 的交互逻辑
    /// </summary>
    public partial class Model : UserControl
    {
        public Model()
        {
            InitializeComponent();
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this) == false)
            {
                if (Environment.Is64BitProcess)
                {
                    System.IO.File.Copy(@"x64\FreeImage.dll", "FreeImage.dll", true);
                }
                else
                {
                    System.IO.File.Copy(@"x86\FreeImage.dll", "FreeImage.dll", true);
                }
            }
            LoadModel(0, "");
        }
        public void LoadModel(int g_id, string model)
        {            
            InitModel();
        }
        private void InitModel()
        {
            ModelImporter importer = new ModelImporter();
            Model3DGroup objModel = importer.Load(@"D:\work\RTD\RTDDE.ModelViewer\RTDDE.ModelViewer\bin\Release\e06_wz_fat_h_01.obj");

            FIBITMAP fiBitmap = FreeImage.Load(FREE_IMAGE_FORMAT.FIF_DDS, @"D:\work\RTD\RTDDE.ModelViewer\RTDDE.ModelViewer\bin\Release\330_e06_wz_fat_h_01.dds", FREE_IMAGE_LOAD_FLAGS.DEFAULT);
            Bitmap b = FreeImage.GetBitmap(fiBitmap);
            BitmapSource bitmapSource = BitmapToBitmapSource(b);

            ImageBrush textureBrush = new ImageBrush(bitmapSource) { ViewportUnits = BrushMappingMode.Absolute, TileMode = TileMode.Tile };
            var material = new DiffuseMaterial(textureBrush);

            Model3DGroup group = new Model3DGroup();
            foreach (GeometryModel3D model3d in objModel.Children)
            {
                //model3d.Material = material;
                //model3d.BackMaterial = material;
                MeshGeometry3D mesh = model3d.Geometry as MeshGeometry3D;
                //MeshGeometry3D sortedMesh = GetSortedMesh(mesh);
                List<MeshGeometry3D> splitedMeshList = SplitMesh(mesh);
                //model3d.Geometry = splitedMeshList[12];
                foreach (MeshGeometry3D splitedMesh in splitedMeshList)
                {
                    GeometryModel3D splitModel = new GeometryModel3D();
                    splitModel.Geometry = splitedMesh;
                    splitModel.Material = material;
                    splitModel.BackMaterial = material;
                    group.Children.Add(splitModel);
                }
            }

            viewer.Children.Add(new DefaultLights());
            sortingVisual.Content = group;
        }
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
        private BitmapSource BitmapToBitmapSource(System.Drawing.Bitmap bitmap)
        {
            IntPtr ptr = bitmap.GetHbitmap();
            BitmapSource result =
                System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    ptr, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            //release resource
            DeleteObject(ptr);

            return result;
        }
        private MeshGeometry3D GetSortedMesh(MeshGeometry3D mesh)
        {
            Dictionary<int, int> sortIndex = new Dictionary<int, int>();

            //sort by point-z
            Point3DCollection points = mesh.Positions;
            Point3DCollection sortedPoints = new Point3DCollection();
            Dictionary<int, Point3D> pointDict = new Dictionary<int, Point3D>();
            int i = 0;
            foreach (Point3D point in points)
            {
                pointDict.Add(i, point);
                i++;
            }
            pointDict = pointDict.OrderBy(o => o.Value.Z).ToDictionary(o => o.Key, o => o.Value);

            //get sort index & sorted Points
            i = 0;
            foreach (KeyValuePair<int, Point3D> kv in pointDict)
            {
                sortIndex.Add(i, kv.Key);
                sortedPoints.Add(kv.Value);
                i++;
            }

            //use sort index to sort texture
            PointCollection texture = mesh.TextureCoordinates;
            PointCollection sortedTexture = new PointCollection();
            foreach (KeyValuePair<int, int> kv in sortIndex)
            {
                sortedTexture.Add(texture[kv.Value]);
            }

            //sort triangle
            Int32Collection triangle = mesh.TriangleIndices;
            Int32Collection sortedTriangle = new Int32Collection();
            foreach (int tri in triangle)
            {
                sortedTriangle.Add(sortIndex.First(o => o.Value == tri).Key);
            }

            MeshGeometry3D sortedMesh = mesh.Clone();
            sortedMesh.Positions = sortedPoints;
            sortedMesh.TextureCoordinates = sortedTexture;
            sortedMesh.TriangleIndices = sortedTriangle;
            return sortedMesh;
        }
        private List<MeshGeometry3D> SplitMesh(MeshGeometry3D mesh)
        {
            Dictionary<double, List<int>> meshLayers = new Dictionary<double, List<int>>();
            int i = 0;
            foreach (Point3D point in mesh.Positions)
            {
                if (meshLayers.Keys.Contains(point.Z) == false)
                {
                    meshLayers.Add(point.Z, new List<int>());
                }
                meshLayers[point.Z].Add(i);
                i++;
            }

            meshLayers = meshLayers.OrderBy(o => o.Key).ToDictionary(o => o.Key, o => o.Value);

            List<MeshGeometry3D> splitedMeshList = new List<MeshGeometry3D>();
            int triCount = 0;
            foreach (KeyValuePair<double, List<int>> kv in meshLayers)
            {
                int j = 0;
                Dictionary<int, int> sortIndex = new Dictionary<int, int>();
                Point3DCollection sortedPoints = new Point3DCollection();
                PointCollection sortedTexture = new PointCollection();
                Int32Collection sortedTriangle = new Int32Collection();

                foreach (int index in kv.Value)
                {
                    sortIndex.Add(j, index);
                    sortedPoints.Add(mesh.Positions[index]);
                    sortedTexture.Add(mesh.TextureCoordinates[index]);
                    j++;
                }

                foreach (int tri in mesh.TriangleIndices)
                {
                    //var splitTri = sortIndex.DefaultIfEmpty(new KeyValuePair<int, int>(-1, -1)).FirstOrDefault(o => o.Value == tri);
                    if (sortIndex.Any(o => o.Value == tri))
                    {
                        sortedTriangle.Add(sortIndex.First(o => o.Value == tri).Key);
                    }
                }

                MeshGeometry3D partMesh = mesh.Clone();
                partMesh.Positions = sortedPoints;
                partMesh.TextureCoordinates = sortedTexture;
                partMesh.TriangleIndices = sortedTriangle;
                splitedMeshList.Add(partMesh);

                triCount += sortedTriangle.Count;
            }
            return splitedMeshList;
        }
    }
}
