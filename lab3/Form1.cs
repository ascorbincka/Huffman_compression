using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace lab3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public class Node     //узел дерева Хаффмана
        {
            public char Symbol { get; set; }
            public int Frequency { get; set; }
            public Node Right { get; set; }
            public Node Left { get; set; }

            public List<bool> search(char symbol, List<bool> data)//поиск символа в дереве
            {
                // является ли узел листом
                if (Right == null && Left == null)
                {
                    if (symbol.Equals(this.Symbol))//совпадает ли
                    {
                        return data;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    List<bool> left = null;
                    List<bool> right = null;//для хранения результатов обхода левого и правого поддеревьев

                    if (Left != null)
                    {
                        List<bool> leftPath = new List<bool>();
                        leftPath.AddRange(data);
                        leftPath.Add(false);
                        left = Left.search(symbol, leftPath);// рекурсивно для левого дочернего элемента с этим новым путем
                    }

                    if (Right != null)
                    {
                        List<bool> rightPath = new List<bool>();
                        rightPath.AddRange(data);
                        rightPath.Add(true);
                        right = Right.search(symbol, rightPath);
                    }

                    if (left != null)
                    {
                        return left;//если в левом поддереве найден путь то он возвращает его
           
                    }
                    else
                    {
                        return right;
                    }
                }
            }
        }
        public class HuffmanTree
        {
            private List<Node> nodes = new List<Node>();
            public Node Root { get; set; }//корневой узел
            public Dictionary<char, int> Frequencies = new Dictionary<char, int>();
            public void Build(string file)
            {
                
                    string source = File.ReadAllText(file);
                    for (int i = 0; i < source.Length; i++)
                {
                    if (!Frequencies.ContainsKey(source[i]))
                    {
                        Frequencies.Add(source[i], 0);
                    }

                    Frequencies[source[i]]++;
                }

                foreach (KeyValuePair<char, int> symbol in Frequencies)//cоздание узлов кода для дерева
                {
                    nodes.Add(new Node() { Symbol = symbol.Key, Frequency = symbol.Value });
                }

                while (nodes.Count > 1)
                {
                    List<Node> orderedNodes = nodes.OrderBy(node => node.Frequency).ToList<Node>();//узлы сортируются по частоте

                    if (orderedNodes.Count >= 2)
                    {
                        List<Node> taken = orderedNodes.Take(2).ToList<Node>();

                       
                        Node parent = new Node()//cоздается новый  узел который объединяет частоты двух взятых 
                        {
                            Symbol = '*',//чтобы указать что он не представляет конкретный символ
                            Frequency = taken[0].Frequency + taken[1].Frequency,
                            Left = taken[0],
                            Right = taken[1]
                        };

                        nodes.Remove(taken[0]);
                        nodes.Remove(taken[1]);
                        nodes.Add(parent);
                    }

                    this.Root = nodes.FirstOrDefault();//корнем дерева становится первый узел в списке nodes

                }

            }

            public BitArray Encode(string file)
            {
                string source = File.ReadAllText(file);
                List<bool> encodedSource = new List<bool>();

                for (int i = 0; i < source.Length; i++)
                {
                    List<bool> encodedSymbol = this.Root.search(source[i], new List<bool>());//для каждого символа выполняется поиск его закодированного представления с использованием метода search
                    encodedSource.AddRange(encodedSymbol);
                }

                BitArray bits = new BitArray(encodedSource.ToArray());//gреобразовать список encodedSource в массив

                return bits;
            }

         

            public bool IsLeaf(Node node)
            {
                return (node.Left == null && node.Right == null);
            }

        }
        static void savearraytofile(BitArray bitArray, string filePath)
        {
            // получаем байты из BitArray и записываем их в файл
            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            using (BinaryWriter writer = new BinaryWriter(fs))
            {
                byte[] bytes = new byte[(bitArray.Length + 7) / 8]; // вычисляем размер байтового массива

                bitArray.CopyTo(bytes, 0); // копируем элементы BitArray в байтовый массив

                writer.Write(bytes); 
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string inputFile = "input.txt"; 
            string outputFile = "compressed.bin"; 
      
            HuffmanTree huffmanTree = new HuffmanTree();

            huffmanTree.Build(inputFile);
            BitArray encoded = huffmanTree.Encode(inputFile);

            savearraytofile(encoded, outputFile);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            long fileSize = new FileInfo("input.txt").Length;
            textBox2.Text = fileSize.ToString();
            string content = File.ReadAllText("input.txt");
            textBox1.Text= content;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            long fileSize = new FileInfo("compressed.bin").Length;
            textBox3.Text = fileSize.ToString();
        }
    }
    
}
