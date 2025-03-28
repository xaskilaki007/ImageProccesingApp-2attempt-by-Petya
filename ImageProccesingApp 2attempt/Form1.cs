﻿using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ImageProccesingApp_2attempt
{
    public partial class Form1 : Form
    {
        private Bitmap originalImage; // Оригинальное изображение
        private Bitmap processedImage; // Обработанное изображение

        public Form1()
        {
            InitializeComponent();

            // Настройка начальных значений
            trk_hue.Minimum = -180;
            trk_hue.Maximum = 180;
            trk_hue.Value = 0;

            trk_contrast.Minimum = -100;
            trk_contrast.Maximum = 100;
            trk_contrast.Value = 0;

            trk_bright.Minimum = -100;
            trk_bright.Maximum = 100;
            trk_bright.Value = 0;

            // Подписка на события
            btn_open.Click += Btn_open_Click;
            btn_save.Click += Btn_save_Click;
            btn_normal.Click += Btn_normal_Click;
            btn_stretch.Click += Btn_stretch_Click;
            btn_autosize.Click += Btn_autosize_Click;
            btn_center.Click += Btn_center_Click;
            btn_zoom.Click += Btn_zoom_Click;
            btn_resize.Click += Btn_resize_Click;
            btn_reload.Click += Btn_reload_Click;
            btn_rotate.Click += Btn_rotate_Click;

            trk_hue.Scroll += TrackBar_Scroll;
            trk_contrast.Scroll += TrackBar_Scroll;
            trk_bright.Scroll += TrackBar_Scroll;
        }

        // Загрузка изображения
        private void Btn_open_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        originalImage = new Bitmap(openFileDialog.FileName);
                        processedImage = new Bitmap(originalImage);

                        pictureBox1.Image = originalImage;
                        pictureBox2.Image = originalImage;

                        txt_imgpath.Text = openFileDialog.FileName;
                        lbl_size.Text = $"{originalImage.Width} x {originalImage.Height}";
                        txt_width.Text = originalImage.Width.ToString();
                        txt_hight.Text = originalImage.Height.ToString();

                        // Установка режимов отображения
                        pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                        pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading image: {ex.Message}");
                    }
                }
            }
        }

        // Сохранение изображения
        private void Btn_save_Click(object sender, EventArgs e)
        {
            if (processedImage == null) return;

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "JPEG Image|*.jpg|PNG Image|*.png|Bitmap Image|*.bmp";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string extension = Path.GetExtension(saveFileDialog.FileName).ToLower();
                        switch (extension)
                        {
                            case ".jpg":
                            case ".jpeg":
                                processedImage.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                                break;
                            case ".png":
                                processedImage.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
                                break;
                            case ".bmp":
                                processedImage.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error saving image: {ex.Message}");
                    }
                }
            }
        }

        // Режимы отображения изображения
        private void Btn_normal_Click(object sender, EventArgs e)
        {
            pictureBox1.SizeMode = PictureBoxSizeMode.Normal;
        }

        private void Btn_stretch_Click(object sender, EventArgs e)
        {
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        private void Btn_autosize_Click(object sender, EventArgs e)
        {
            pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
        }

        private void Btn_center_Click(object sender, EventArgs e)
        {
            pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
        }

        private void Btn_zoom_Click(object sender, EventArgs e)
        {
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
        }

        // Изменение размера изображения
        private void Btn_resize_Click(object sender, EventArgs e)
        {
            if (originalImage == null) return;

            try
            {
                int width = int.Parse(txt_width.Text);
                int height = int.Parse(txt_hight.Text);

                processedImage = new Bitmap(originalImage, width, height);
                pictureBox1.Image = processedImage;
                lbl_size.Text = $"{width} x {height}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error resizing image: {ex.Message}");
            }
        }

        // Сброс изменений
        private void Btn_reload_Click(object sender, EventArgs e)
        {
            if (originalImage != null)
            {
                processedImage = new Bitmap(originalImage);
                pictureBox1.Image = processedImage;

                txt_width.Text = originalImage.Width.ToString();
                txt_hight.Text = originalImage.Height.ToString();
                lbl_size.Text = $"{originalImage.Width} x {originalImage.Height}";

                // Сброс трекбаров
                trk_hue.Value = 0;
                trk_contrast.Value = 0;
                trk_bright.Value = 0;
            }
        }

        // Поворот изображения
        private void Btn_rotate_Click(object sender, EventArgs e)
        {
            if (processedImage == null) return;

            processedImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
            pictureBox1.Image = processedImage;
        }

        // Обработка изменений трекбаров
        private void TrackBar_Scroll(object sender, EventArgs e)
        {
            if (originalImage == null) return;

            try
            {
                // Применение эффектов к изображению
                float hue = trk_hue.Value / 100f;
                float contrast = 1 + trk_contrast.Value / 100f;
                float brightness = 1 + trk_bright.Value / 100f;

                processedImage = AdjustImage(originalImage, hue, contrast, brightness);
                pictureBox1.Image = processedImage;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adjusting image: {ex.Message}");
            }
        }

        // Метод для настройки изображения (цвет, контраст, яркость)
        private Bitmap AdjustImage(Bitmap image, float hue, float contrast, float brightness)
        {
            Bitmap adjustedImage = new Bitmap(image.Width, image.Height);

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color pixel = image.GetPixel(x, y);

                    // Применение hue (оттенка)
                    float h = pixel.GetHue() + hue * 360;
                    if (h > 360) h -= 360;
                    if (h < 0) h += 360;

                    float s = pixel.GetSaturation();
                    float l = pixel.GetBrightness();

                    // Применение яркости и контраста
                    l = l * brightness;
                    l = (l - 0.5f) * contrast + 0.5f;

                    // Ограничение значений
                    l = Math.Max(0, Math.Min(1, l));
                    s = Math.Max(0, Math.Min(1, s));

                    Color newPixel = ColorFromAhsb(pixel.A, h, s, l);
                    adjustedImage.SetPixel(x, y, newPixel);
                }
            }

            return adjustedImage;
        }

        // Преобразование из HSB в Color
        private Color ColorFromAhsb(int alpha, float hue, float saturation, float brightness)
        {
            if (saturation == 0)
            {
                return Color.FromArgb(alpha,
                    Convert.ToInt32(brightness * 255),
                    Convert.ToInt32(brightness * 255),
                    Convert.ToInt32(brightness * 255));
            }

            float fMax, fMid, fMin;
            int iSextant, iMax, iMid, iMin;

            if (brightness > 0.5)
            {
                fMax = brightness - (brightness * saturation) + saturation;
                fMin = brightness + (brightness * saturation) - saturation;
            }
            else
            {
                fMax = brightness + (brightness * saturation);
                fMin = brightness - (brightness * saturation);
            }

            iSextant = (int)Math.Floor(hue / 60f);
            if (hue >= 300f)
            {
                hue -= 360f;
            }
            hue /= 60f;
            hue -= 2f * (float)Math.Floor(((iSextant + 1f) % 6f) / 2f);
            if (iSextant % 2 == 0)
            {
                fMid = hue * (fMax - fMin) + fMin;
            }
            else
            {
                fMid = fMin - hue * (fMax - fMin);
            }

            iMax = Convert.ToInt32(fMax * 255);
            iMid = Convert.ToInt32(fMid * 255);
            iMin = Convert.ToInt32(fMin * 255);

            switch (iSextant)
            {
                case 1:
                    return Color.FromArgb(alpha, iMid, iMax, iMin);
                case 2:
                    return Color.FromArgb(alpha, iMin, iMax, iMid);
                case 3:
                    return Color.FromArgb(alpha, iMin, iMid, iMax);
                case 4:
                    return Color.FromArgb(alpha, iMid, iMin, iMax);
                case 5:
                    return Color.FromArgb(alpha, iMax, iMin, iMid);
                default:
                    return Color.FromArgb(alpha, iMax, iMid, iMin);
            }
        }

        // Фильтры (заглушки - можно реализовать свои алгоритмы)
        private void Btn_f1_Click(object sender, EventArgs e)
        {
            ApplyFilter(1);
        }

        private void Btn_f2_Click(object sender, EventArgs e)
        {
            ApplyFilter(2);
        }

        private void Btn_f3_Click(object sender, EventArgs e)
        {
            ApplyFilter(3);
        }

        private void Btn_f4_Click(object sender, EventArgs e)
        {
            ApplyFilter(4);
        }

        private void Btn_f5_Click(object sender, EventArgs e)
        {
            ApplyFilter(5);
        }

        private void ApplyFilter(int filterNumber)
        {
            if (originalImage == null) return;

            // Здесь можно реализовать различные фильтры
            // Это пример - просто инвертирование цветов для демонстрации
            processedImage = new Bitmap(originalImage.Width, originalImage.Height);

            for (int y = 0; y < originalImage.Height; y++)
            {
                for (int x = 0; x < originalImage.Width; x++)
                {
                    Color pixel = originalImage.GetPixel(x, y);

                    // Пример фильтра - инверсия цветов
                    Color newPixel = Color.FromArgb(
                        255 - pixel.R,
                        255 - pixel.G,
                        255 - pixel.B);

                    processedImage.SetPixel(x, y, newPixel);
                }
            }

            pictureBox1.Image = processedImage;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Инициализация при загрузке формы
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            // Можно реализовать предпросмотр или другие функции
        }

        private void label1_Click(object sender, EventArgs e)
        {
            // Обработка клика по метке
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            // Обработка изменения текста
        }
    }
}