﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Interferometry.interfaces;
using rab1;
using rab1.Forms;
using Color = System.Drawing.Color;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using Image = System.Windows.Controls.Image;
using MessageBox = System.Windows.MessageBox;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace Interferometry.forms
{
    /// <summary>
    /// Логика взаимодействия для MainForm.xaml
    /// </summary>
    public partial class MainForm : ImageContainerDelegate
    {
        private List<ImageContainer> imageContainersList;
        private Pi_Class1.ZArrayDescriptor zArrayDescriptor;

        private bool needPointsCapture = false;
        private Point3D firstClick;
        private Point3D secondClick;

        //Life Cycle
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public MainForm()
        {
            InitializeComponent();

            imageContainersList = new List<ImageContainer>();
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Grid newScrollerContent = new Grid();
            newScrollerContent.HorizontalAlignment = HorizontalAlignment.Stretch;
            newScrollerContent.VerticalAlignment = VerticalAlignment.Stretch;

            for (int i = 0; i < 11; i++)
            {
                ImageContainer newImageContainer = new ImageContainer();
                newImageContainer.myDelegate = this;
                newImageContainer.HorizontalAlignment = HorizontalAlignment.Stretch;
                newImageContainer.VerticalAlignment = VerticalAlignment.Stretch;
                newImageContainer.Width = Double.NaN;
                newImageContainer.Height = Double.NaN;
                newImageContainer.setImageNumberLabel(i + 1);

                RowDefinition newRowDefinition = new RowDefinition();
                newScrollerContent.RowDefinitions.Add(newRowDefinition);
                newScrollerContent.Children.Add(newImageContainer);
                Grid.SetRow(newImageContainer, i);
                imageContainersList.Add(newImageContainer);
            }

            imageContainersScroller.Content = newScrollerContent;
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
      
        
        
        //Методы из пункта "Работа с файлами"
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void loadClicked(object sender, RoutedEventArgs e)
        {
            ImageSource newSource = FilesHelper.loadImage();

            if (newSource != null)
            {
                zArrayDescriptor = Utils.getArrayFromImage((BitmapSource) newSource);
                mainImage.Source = Utils.getImageFromArray(zArrayDescriptor);
            }
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void saveButtonClicked(object sender, RoutedEventArgs e)
        {
            if (mainImage.Source != null)
            {
                FilesHelper.saveImage(mainImage.Source);
            }
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void loadEightImages(object sender, RoutedEventArgs e)
        {

        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



        //Методы из пункта "Восстановление фазы"
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void unwrapClicked(object sender, RoutedEventArgs e)
        {
            if ((imageContainersList[8].getzArrayDescriptor() == null) || (imageContainersList[9].getzArrayDescriptor() == null))
            {
                MessageBox.Show("Изображения пустые");
                return;
            }

            Pi_Class1.ZArrayDescriptor[] imagesF = new Pi_Class1.ZArrayDescriptor[3];

            imagesF[0] = imageContainersList[8].getzArrayDescriptor();
            imagesF[1] = imageContainersList[9].getzArrayDescriptor();
            imagesF[2] = imageContainersList[10].getzArrayDescriptor();

            UnwrapForm unwrapForm = new UnwrapForm(imagesF);
            unwrapForm.imageUnwrapped += unwrapFormOnImageUnwrapped;
            unwrapForm.Show();
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void unwrapFormOnImageUnwrapped(Pi_Class1.ZArrayDescriptor unwrappedPhase)
        {
            Bitmap unwrappedPhaseImage = Pi_Class1.getUnwrappedPhaseImage(unwrappedPhase.array, unwrappedPhase.width, unwrappedPhase.height);
            imageContainersList[7].setImage(unwrappedPhaseImage);
            imageContainersList[7].setzArrayDescriptor(unwrappedPhase);
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void choosePointsClicked(object sender, RoutedEventArgs e)
        {
            if (mainImage.Source == null)
            {
                MessageBox.Show("Главное изображение пустое");
                return;
            }

            if (zArrayDescriptor == null)
            {
                MessageBox.Show("Z-массив пуст");
                return;
            }

            needPointsCapture = true;
            firstClick = null;
            secondClick = null;
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void createWrappedPhase(object sender, RoutedEventArgs e)
        {
            double[] fz = new double[4];
            fz[0] = 0;
            fz[1] = 90;
            fz[2] = 180;
            fz[3] = 270;

            Pi_Class1.ZArrayDescriptor[] source = new Pi_Class1.ZArrayDescriptor[4];
            source[0] = imageContainersList[0].getzArrayDescriptor();
            source[1] = imageContainersList[1].getzArrayDescriptor();
            source[2] = imageContainersList[2].getzArrayDescriptor();
            source[3] = imageContainersList[3].getzArrayDescriptor();

            Pi_Class1.ZArrayDescriptor result = FazaClass.ATAN_1234(source, fz);
            imageContainersList[8].setzArrayDescriptor(result);

            source = new Pi_Class1.ZArrayDescriptor[4];
            source[0] = imageContainersList[4].getzArrayDescriptor();
            source[1] = imageContainersList[5].getzArrayDescriptor();
            source[2] = imageContainersList[6].getzArrayDescriptor();
            source[3] = imageContainersList[7].getzArrayDescriptor();

            result = FazaClass.ATAN_1234(source, fz);
            imageContainersList[9].setzArrayDescriptor(result);
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void buildTableClicked(object sender, RoutedEventArgs e)
        {
            Pi_Class1.ZArrayDescriptor[] imagesForTable = new Pi_Class1.ZArrayDescriptor[3];

            imagesForTable[0] = imageContainersList[8].getzArrayDescriptor();
            imagesForTable[1] = imageContainersList[9].getzArrayDescriptor();
            imagesForTable[2] = imageContainersList[10].getzArrayDescriptor();


            TableGenerateForm tableGenerateForm = new TableGenerateForm(imagesForTable);
            tableGenerateForm.Show();
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
      

        
        //ImageContainerDelegate Methods
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void exportImage(ImageContainer imageContainer, Pi_Class1.ZArrayDescriptor arrayDescriptor)
        {
            zArrayDescriptor = arrayDescriptor;
            mainImage.Source = Utils.getImageFromArray(zArrayDescriptor);
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public Pi_Class1.ZArrayDescriptor getImageToLoad(ImageContainer imageContainer)
        {
            return zArrayDescriptor;
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



        //Mouse Events
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void mainImage_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (needPointsCapture == false)
            {
                return;
            }

            if (firstClick == null)
            {
                int x = (int)e.GetPosition(mainImage).X;
                int y = (int)e.GetPosition(mainImage).Y;
                int z = (int)zArrayDescriptor.array[x, y];

                firstClick = new Point3D(x, y, z);
                return;
            }
            else
            {
                int x = (int)e.GetPosition(mainImage).X;
                int y = (int)e.GetPosition(mainImage).Y;
                int z = (int)zArrayDescriptor.array[x, y];

                secondClick = new Point3D(x, y, z);
            }

            MessageBox.Show("Первая точка - X = " + firstClick.z + " Y = " + firstClick.y + " Z = " + firstClick.z
                + " Вторая точка -" + secondClick.x + " Y = " + secondClick.y + " Z = " + secondClick.z);

            needPointsCapture = false;
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void mainImage_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (zArrayDescriptor != null)
                {
                    int redComponent = (int) zArrayDescriptor.array[(int)e.GetPosition(mainImage).X, (int)e.GetPosition(mainImage).Y];
                    int greenComponent = (int)zArrayDescriptor.array[(int)e.GetPosition(mainImage).X, (int)e.GetPosition(mainImage).Y];
                    int blueComponent = (int)zArrayDescriptor.array[(int)e.GetPosition(mainImage).X, (int)e.GetPosition(mainImage).Y];

                    redComponentLabel.Content = Convert.ToString(redComponent);
                    greenComponentLabel.Content = Convert.ToString(greenComponent);
                    blueComponentLabel.Content = Convert.ToString(blueComponent);

                    int xPosition = (int) e.GetPosition(mainImage).X;
                    int yPositon = (int) e.GetPosition(mainImage).Y;

                    xLabel.Content = Convert.ToString(xPosition);
                    yLabel.Content = Convert.ToString(yPositon);
                }
            }catch (Exception ex){}
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
         
        

        //Additional Classes
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public class Point3D
        {
            public int x;
            public int y;
            public int z;
            
            public Point3D(int newX, int newY, int newZ)
            {
                x = newX;
                y = newY;
                z = newZ;
            }
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    }
}
