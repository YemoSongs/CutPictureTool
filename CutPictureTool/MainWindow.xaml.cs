using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CutPictureTool
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        private string bigTexturePath;
        private int bigWidth;
        private int bigHeight;
        private int smallWidth;
        private int smallHeight;

        /// <summary>
        /// 点击选择大图路径
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBtnClick_ChooseBigTexturePath(object sender, RoutedEventArgs e)
        {
            // 创建文件选择对话框
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "图片文件|*.jpg;*.png;*.jpeg|所有文件|*.*"
            };

            // 显示对话框并检查是否选择了文件
            if (openFileDialog.ShowDialog() == true)
            {
                string bigImagePath = openFileDialog.FileName;

                // 显示文件路径
                BigTexturePath.Content = bigImagePath;
                bigTexturePath = bigImagePath;
                // 加载图片获取宽高
                var bitmap = new BitmapImage(new Uri(bigImagePath));
                BigTextureWidth.Content = bitmap.PixelWidth.ToString();
                bigWidth = bitmap.PixelWidth;
                BigTextureHeight.Content = bitmap.PixelHeight.ToString();
                bigHeight = bitmap.PixelHeight;
            }
        }

        /// <summary>
        /// 点击裁切大图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBtnClick_CutBigTexture(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(bigTexturePath))
            {
                MessageBox.Show("请先选择大图路径");
                return;
            }

            // 获取选择的格式
            string format = GetSelectedFormat();
            if (string.IsNullOrEmpty(format))
            {
                MessageBox.Show("请选择图片格式");
                return;
            }

            // 打开保存文件对话框，让用户选择保存的文件夹
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "所有文件|*.*", // 选择所有文件类型，稍后可以根据文件扩展名保存
                DefaultExt = format // 默认扩展名
            };


            // 设置初始目录为大图所在目录
            saveFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName(bigTexturePath);

            if (saveFileDialog.ShowDialog() == true)
            {
                // 获取用户选择的保存路径
                string saveDirectory = System.IO.Path.GetDirectoryName(saveFileDialog.FileName);

                // 以大图文件名为文件夹名，在保存路径下创建文件夹
                string folderName = System.IO.Path.GetFileNameWithoutExtension(bigTexturePath);
                string targetFolder = System.IO.Path.Combine(saveDirectory, folderName);

                // 如果文件夹不存在，则创建该文件夹
                if (!System.IO.Directory.Exists(targetFolder))
                {
                    System.IO.Directory.CreateDirectory(targetFolder);
                }

                // 读取大图
                using (var img = new Bitmap(bigTexturePath))
                {
                    int rows = bigHeight / smallHeight; // 切割的行数
                    int cols = bigWidth / smallWidth;   // 切割的列数

                    for (int row = 0; row < rows; row++)
                    {
                        for (int col = 0; col < cols; col++)
                        {
                            // 定义切割区域
                            System.Drawing.Rectangle cropRect = new System.Drawing.Rectangle(col * smallWidth, row * smallHeight, smallWidth, smallHeight);

                            // 从原图中获取小图
                            using (Bitmap croppedImage = img.Clone(cropRect, img.PixelFormat))
                            {
                                // 保存小图到新创建的文件夹中
                                string savePath = System.IO.Path.Combine(targetFolder, $"{folderName}_{row}_{col}.{format}");

                                // 根据格式保存图片
                                if (format == "jpg")
                                {
                                    croppedImage.Save(savePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                                }
                                else if (format == "png")
                                {
                                    croppedImage.Save(savePath, System.Drawing.Imaging.ImageFormat.Png);
                                }
                            }
                        }
                    }

                    MessageBox.Show("切图完成！");
                }
            }
        }



        private void TextBox_SmallWidth(object sender, TextChangedEventArgs e)
        {
            // 验证用户输入的内容
            if (!int.TryParse(tbSmallWidth.Text, out int smallWidth) || smallWidth <= 0)
            {
                // 提示错误信息
                MessageBox.Show("请输入有效的小图宽度");
            }
            this.smallWidth = smallWidth;
        }

        private void TextBox_SmallHeight(object sender, TextChangedEventArgs e)
        {
            // 验证用户输入的内容
            if (!int.TryParse(tbSmallHeight.Text, out int smallHeight) || smallHeight <= 0)
            {
                // 提示错误信息
                MessageBox.Show("请输入有效的小图高度");
            }
            this.smallHeight = smallHeight;
        }


        private string GetSelectedFormat()
        {
            if (rbJPG.IsChecked == true)
                return "jpg";
            else if (rbPNG.IsChecked == true)
                return "png";
            else
                return string.Empty;
        }




    }
}
