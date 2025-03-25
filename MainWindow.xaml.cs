using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;

namespace TI2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        KeyGenerator _keyGenerator;
        byte[] _input;
        byte[] _output;
        public MainWindow()
        {
            InitializeComponent();
            this.KeyInput.Text = "11111111_11111111_11111111";
            _keyGenerator = KeyGenerator.ParseKeyGenerator(this.KeyInput.Text);
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog
            {
            };

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                using (FileStream stream = new FileStream(openFileDialog.FileName, FileMode.Open))
                {
                    _input = new byte[stream.Length];
                    stream.Read(_input, 0, _input.Length);
                }
                this.OriginalDataOutput.Text = IOParser.BytesToShortenedString(_input, _keyGenerator.KeyLength);
            }
        }
        private void SaveToFile(byte[] output)
        {
            System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog
            {
            };
            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                using (FileStream stream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                {
                    stream.Write(output, 0, output.Length);
                }
            }
        }
        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            SaveToFile(_output);
        }

        private void Encrypt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _keyGenerator = KeyGenerator.ParseKeyGenerator(this.KeyInput.Text);
            }
            catch (ArgumentException)
            {
                System.Windows.MessageBox.Show("Не удалось обработать ключ. Введите ключ из нулей и едениц нужной длины");
            }
            if (_input == null)
            {
                return;
            }
            byte[] keyBytes = new byte[_input.Length];
            byte[] _output = new byte[_input.Length];
            for (int i = 0; i < keyBytes.Length; i++)
            {
                keyBytes[i] = _keyGenerator.GetNextKeyByte();
                _output[i] = (byte)(_input[i] ^ keyBytes[i]);
            }
            SaveToFile(_output);
            this.EncryptedBitsOutput.Text = IOParser.BytesToShortenedString(_output, _keyGenerator.KeyLength);
            this.OriginalDataOutput.Text = IOParser.BytesToShortenedString(_input, _keyGenerator.KeyLength);
            this.KeyBitsOutput.Text = IOParser.BytesToShortenedString(keyBytes, _keyGenerator.KeyLength);
        }

        private void OriginalDataOutput_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            _input = IOParser.ParseByteArray(OriginalDataOutput.Text);
        }
    }
}
