using Microsoft.Win32;
using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OpenCvSharp;
using System.Drawing;
using System.IO;

namespace ImageProcessing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {

        private List<System.Windows.Point> polygonPoints = new List<System.Windows.Point>();
        private bool isDrawingPolygon = false;

        public MainWindow()
        {
            InitializeComponent();
        }
        private BitmapImage originalBitmap;
        private Mat originalImageMat;


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    // Load the selected image into the Image control
                    originalBitmap = new BitmapImage(new Uri(openFileDialog.FileName));
                    imagePicture.Source = originalBitmap;
                    originalImageMat = Cv2.ImRead(openFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading image: {ex.Message}");
                }
            }
        }


        private void GrayButton_Click(object sender, RoutedEventArgs e)
        {
            if (imagePicture.Source != null && imagePicture.Source is BitmapSource)
            {
                // Convert the image to grayscale
                BitmapSource originalImage = (BitmapSource)imagePicture.Source;
                FormatConvertedBitmap grayscaleImage = new FormatConvertedBitmap(originalImage, PixelFormats.Gray32Float, null, 0);

                // Display the grayscale image
                imagePicture.Source = grayscaleImage;
            }
        }

        private void PolygonButton_Click(object sender, RoutedEventArgs e)
        {
            polygonPicture.Source = imagePicture.Source;
            isDrawingPolygon = true;
            polygonPoints.Clear();
            polygonPicture.MouseLeftButtonDown += ImageControl_MouseLeftButtonDown;
        }
        private void ImageControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (isDrawingPolygon)
            {
                System.Windows.Point mousePosition = e.GetPosition(imagePicture);
                polygonPoints.Add(mousePosition);

                if (polygonPoints.Count > 1)
                {
                    DrawPolygon();
                }
            }
        }

        private void DrawPolygon()
        {
            polygonPicture.Source = imagePicture.Source;
            if (polygonPicture.Source != null && polygonPicture.Source is BitmapSource)
            {
                DrawingVisual visual = new DrawingVisual();
                using (DrawingContext dc = visual.RenderOpen())
                {
                    Pen pen = new Pen(Brushes.Red, 1);

                    // Draw lines between each pair of consecutive points
                    for (int i = 0; i < polygonPoints.Count - 1; i++)
                    {
                        dc.DrawLine(pen, polygonPoints[i], polygonPoints[i + 1]);
                    }

                    // Draw line between the last and first point to close the polygon
                    dc.DrawLine(pen, polygonPoints[polygonPoints.Count - 1], polygonPoints[0]);
                }

                // Render the visual to the image control
                RenderTargetBitmap rtb = new RenderTargetBitmap((int)imagePicture.ActualWidth, (int)imagePicture.ActualHeight, 96, 96, PixelFormats.Pbgra32);
                rtb.Render(visual);
                polygonPicture.Source = rtb;
            }
        }
    }
}