using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace graphColoring
{
    class Program
    {
        static void Main(string[] args)
        {
            Test test;
            Console.WriteLine("Задача о правильной раскраске графа в минимальное количество цветов \n");
            Console.WriteLine("[1] Автоматическая генерация данных");
            Console.WriteLine("[2] Ввод данных вручную");
            Console.WriteLine("[3] Автоматическая генерация данных - все вершины смежны");
            Console.WriteLine("[4] Автоматическая генерация данных - ни одна вершина не смежна с другой\n");
            int method = Convert.ToInt32(Console.ReadLine());
            Console.Write("Введите количество вершин: ");
            int n = Convert.ToInt32(Console.ReadLine());
            switch (method)
            {
                case 1:
                    test = new Test(100, n, 1);
                    test.getTheResults();
                    break;
                case 2:
                    test = new Test(100, n, 2);
                    test.getTheResults();
                    break;
                case 3:
                    test = new Test(100, n, 3);
                    test.getTheResults();
                    break;
                case 4:
                    test = new Test(100, n, 4);
                    test.getTheResults();
                    break;
                default:
                    Console.WriteLine("Автоматическая генерация данных...");
                    test = new Test(100, n, 1);
                    break;
            }
        }

    }

    //Класс "Вершина"
    class Vertice
    {
        public int Number;      //Номер вершины
        public int Color;       //Цвет вершины
        public bool Mark;       //Метка (true - окрашена, false - нет)

        public Vertice(int N)   //По умолчанию цвета нет(-1), метка false
        {
            this.Number = N;
            this.Color = -1;
            this.Mark = false;
        }

        public void Clean()                             //Очистить вершину (для повторного ипользования алгоритма)
        {
            this.Color = -1;
            this.Mark = false;
        }
    }

    class Graph
    {
        int N;                                          //Число вершин в графе
        int[,] Matrix;                                  //Матрица смежности
        int countColors;                                //Количество цветов в итоге, увеличивается во время "раскраски"
        List<Vertice> Vertices = new List<Vertice>();   //Список вершин
        int[] minColors;

        public Graph(int N)
        {
            this.N = N;
            minColors = new int[N];
            for (int i = 0; i < N; i++)                 //Добавляем вершины в список
            {
                Vertice V = new Vertice(i);
                Vertices.Add(V);
            }
        }

        public void getMatrix(int k)                            //Заполнение матрицы
        {
            Random rnd = new Random();                          //Интересный объект для генерации рандомных значений
            Matrix = new int[N, N];                             //Матрица смежности N x N

            switch (k)
            {
                case 1:
                    for (int i = 0; i < N; i++)
                    {
                        Matrix[i, i] = 0;                       //Все диагональные элементы равны нулю
                        for (int j = 0; j < i; j++)             //Остальные - либо 0, либо 1
                        {
                            Matrix[i, j] = rnd.Next() % 2;
                            Matrix[j, i] = Matrix[i, j];
                        }
                    }
                    break;
                case 2:
                    StreamReader reader = new StreamReader("input.txt");    //Файл, из которого читаем 
                    string line;
                    int m = 0;
                    while (!reader.EndOfStream)
                    {
                        line = reader.ReadLine();
                        string[] elements = line.Split(' ');
                        for (int j = 0; j < N; j++)
                        {
                            Matrix[m, j] = Convert.ToInt32(elements[j]);
                        }
                        m++;
                    }
                    break;
                case 3:
                    for (int i = 0; i < N; i++)
                    {
                        Matrix[i, i] = 0;                       //Все диагональные элементы равны нулю
                        for (int j = 0; j < i; j++)
                        {
                            Matrix[i, j] = 1;                   //Остальные - 1
                            Matrix[j, i] = Matrix[i, j];
                        }
                    }
                    break;
                case 4:
                    for (int i = 0; i < N; i++)
                    {
                        for (int j = 0; j < i; j++)
                        {
                            Matrix[i, j] = 0;                   //Все элементы равны нулю
                        }
                    }
                    break;
            }

        }

        public void Print()                                     //Вывод матрицы на экран
        {
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    Console.Write(Matrix[i, j]);
                    Console.Write(" ");
                }
                Console.WriteLine();
            }

        }

        public int bruteForceColoring(int curV, int currentColorCounter)        //curV - текущая рассматриваемая вершина, curColCnt - количество уже задействованных цветов. Метод будет возвращать количество цветов, задействованных для раскраски всего графа
        {
            if (curV >= N)
                return currentColorCounter;                                     //раскрасили уже весь граф
            List<int> enabledColors = new List<int>() { };                      //Формируем список доступных цветов
            for (int k = 1; k <= currentColorCounter; k++)                      //Добавляем все доступные цвета в одноимённый список
            {
                enabledColors.Add(k);
            }
            for (int u = 0; u < curV; u++)                                      //Идём по столбцам
            {
                if (Matrix[curV, u] == 1)                                       //Если есть ребро между i и j
                {
                    if (u < curV && enabledColors.Contains(Vertices[u].Color))  //И список не пуст
                        enabledColors.Remove(Vertices[u].Color);                //Удаляем из него цвет вершины j, дабы не окрасить в него i
                }
            }
            int minColorCounter = N;
            foreach (int c in enabledColors)                                    //Перебираем все доступные цвета c
            {
                Vertices[curV].Color = c;                                       //Красим текущую вершину в цвет c
                int k = bruteForceColoring(curV + 1, currentColorCounter);      //И запускаем рекурсивно раскраску остальных вершин графа
                if (k < minColorCounter)                                        //Если в итоге получилась раскраска в меньшее количество цветов, то надо запомнить эту раскраску
                {
                    minColorCounter = k;
                    //Запоминаем текущую раскраску в глобальный список
                    for (int j = 0; j < N; j++)
                    {
                        minColors[j] = Vertices[j].Color;
                    }
                }
            }
            
            //Обязательно рассматриваем возможность раскраски вершины в новый цвет. Это необходимо даже в том случае, если есть какой-то "свободный" цвет
            Vertices[curV].Color = currentColorCounter + 1;
            int t = bruteForceColoring(curV + 1, currentColorCounter + 1);
            if (t < minColorCounter)                                            //Если в итоге получилась раскраска в меньшее количество цветов, то надо запомнить эту раскраску
            {
                minColorCounter = t; 
                //Запоминаем текущую раскраску в глобальный список
                for (int j = 0; j < N; j++)
                {
                    minColors[j] = Vertices[j].Color;
                }
            }
            return minColorCounter;
        }

        public int greedyColoring()
        {
            for (int i = 0; i < N; i++)
            {
                Matrix[i, i] = 1;
            }
            
            countColors = 0;                                                    //Изначально имеем только один цвет для раскраски
            
            for (int i = 0; i < N; i++) //N
            {
            
                if (Vertices[i].Mark != true)                                   //Начинаем обход вершин с первой
                {
                    Vertices[i].Mark = true;                                    //Отмечаем вершину i раскрашеной
                    Vertices[i].Color = countColors;                            //И присваиваем ей номер цвета
                    
                    for (int j = 0; j < N; j++)       //N                       //Идём по столбцам
                    {
                    
                        if (Matrix[i, j] == 0 && Vertices[j].Mark == false) //заходим не более N раз   // если вершина i не связана с вершиной j 
                        {// 1/N
                        
                            for (int k = 0; k < N; k++)   //не превышает N                  
                            {
                            
                                Matrix[i, k] = Matrix[i, k] | Matrix[j, k]; //Логически складываем элементы строк i и j, обновляя строку i, чтобы стянуть вершины
                            }

                            Vertices[j].Mark = true;                        //Помечаем вершину j

                            Vertices[j].Color = countColors;                //И красим её в тот же цвет, что и i
                        }
                    }

                    countColors++;                                          //Добавляем новый цвет
                }
            }
            foreach (Vertice v in Vertices)                                 //Очистить вершины, для другого алгоритма
            {
                v.Clean();
            }
            return countColors;
        }
    }

    class Test
    {
        int numberOfTests;
        int Volume;
        int matrixMethod;

        double totalRunningTime_1 = 0;                       //Суммарное время работы точного алгоритма
        double totalRunningTime_2 = 0;                       //Суммарное время работы жадного алгоритма


        public Test(int numberOfTests, int Volume, int matrixMethod)
        {
            this.numberOfTests = numberOfTests;
            this.Volume = Volume;
            this.matrixMethod = matrixMethod;
        }

        void bruteForceColoringTest(Graph newGraph, out int Result)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Result = newGraph.bruteForceColoring(0, 0);
            stopwatch.Stop();
            totalRunningTime_1 += stopwatch.ElapsedTicks / 10000.0;
        }

        void greedyColoringTest(Graph newGraph, out int Result)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Result = newGraph.greedyColoring();
            stopwatch.Stop();
            totalRunningTime_2 += stopwatch.ElapsedTicks / 10000.0;
        }

        public void getTheResults()
        {
            int totalHits = 0;                                  //Количество совпадений решения точного и жадного алгоритмов
            double totalDeviation = 0;                           //Сумма отклонений

            double avgRunningTime_1;                     //Среднее время работы точного алгоритма
            double avgRunningTime_2;                     //Среднее время работы жадного алгоритма
            double avgHits;                              //Процент тестов, в которых приближённое решение совпало с точным
            double avgDeviation;                         //Среднее относительное отклонение приближенного решения от точного

            for (int i = 0; i < numberOfTests; i++)
            {
                int F1;
                int F2;
                Graph newGraph = new Graph(Volume);
                newGraph.getMatrix(matrixMethod);

                bruteForceColoringTest(newGraph, out F1);
                greedyColoringTest(newGraph, out F2);

                if (F1 == F2)
                    totalHits++;

                totalDeviation += (double)Math.Abs(F2 - F1) / (double)F1;
            }

            avgRunningTime_2 = (double)totalRunningTime_2 / (double)numberOfTests;
            avgRunningTime_1 = (double)totalRunningTime_1 / (double)numberOfTests;
            avgHits = (double)totalHits / (double)numberOfTests * 100;
            avgDeviation = (double)totalDeviation / (double)numberOfTests;

            Console.WriteLine("\nКоличество вершин в графе - " + Volume);
            Console.WriteLine("Количество тестов - " + numberOfTests);
            Console.WriteLine("Среднее время работы точного алгоритма: " + avgRunningTime_1 + "мс");
            Console.WriteLine("Среднее время работы жадного алгоритма: " + avgRunningTime_2 + "мс");
            Console.WriteLine("Процент тестов, в которых приближённое решение совпало с точным: " + Math.Round(avgHits, 2) + "%");
            Console.WriteLine("Среднее относительное отклонение приближенного решения от точного: " + Math.Round(avgDeviation, 2));
        }

    }
}
