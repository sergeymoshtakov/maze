using System;
using System.Windows.Forms;
using System.Drawing;

namespace Maze
{
    class Labirint
    {
        public int height; // высота лабиринта (количество строк)
        public int width; // ширина лабиринта (количество столбцов в каждой строке)

        private int playerX;
        private int playerY;

        private int health;

        private int collectedMedals;
        private int totalMedals = 0;

        public MazeObject[,] maze;
        public PictureBox[,] images;

        public static Random r = new Random();
        public Form parent;

        public Labirint(Form parent, int width, int height)
        {
            this.width = width;
            this.height = height;
            this.parent = parent;

            maze = new MazeObject[height, width];
            images = new PictureBox[height, width];

            Generate();

            playerX = 0;
            playerY = 2;

            health = 100;
        }

        private void Generate()
        {
            int smileX = 0;
            int smileY = 2;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    MazeObject.MazeObjectType current = MazeObject.MazeObjectType.HALL;

                    // в 1 случае из 5 - ставим стену
                    if (r.Next(5) == 0)
                    {
                        current = MazeObject.MazeObjectType.WALL;
                    }

                    // в 1 случае из 250 - кладём денежку
                    if (r.Next(250) == 0)
                    {
                        current = MazeObject.MazeObjectType.MEDAL;
                        totalMedals++;
                    }

                    // в 1 случае из 250 - размещаем врага
                    if (r.Next(250) == 0)
                    {
                        current = MazeObject.MazeObjectType.ENEMY;
                    }

                    // стены по периметру обязательны
                    if (y == 0 || x == 0 || y == height - 1 | x == width - 1)
                    {
                        current = MazeObject.MazeObjectType.WALL;
                    }

                    // наш персонажик
                    if (x == smileX && y == smileY)
                    {
                        current = MazeObject.MazeObjectType.CHAR;
                    }

                    // лекарство
                    if (r.Next(100) == 0)
                    {
                        current = MazeObject.MazeObjectType.MEDICINE;
                    }

                    // есть выход, и соседняя ячейка справа всегда свободна
                    if (x == smileX + 1 && y == smileY || x == width - 1 && y == height - 3)
                    {
                        current = MazeObject.MazeObjectType.HALL;
                    }
                    
                    maze[y, x] = new MazeObject(current);
                    images[y, x] = new PictureBox();
                    images[y, x].Location = new Point(x * maze[y, x].width, y * maze[y, x].height);
                    images[y, x].Parent = parent;
                    images[y, x].Width = maze[y, x].width;
                    images[y, x].Height = maze[y, x].height;
                    images[y, x].BackgroundImage = maze[y, x].texture;
                    images[y, x].Visible = false;
                }
            }
        }

        public void Show()
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    images[y, x].Visible = true;
                }
            }
        }

        public void MovePlayer(Keys key)
        {
            int newX = playerX;
            int newY = playerY;

            switch (key)
            {
                case Keys.Left:
                    newX--;
                    break;
                case Keys.Right:
                    newX++;
                    break;
                case Keys.Up:
                    newY--;
                    break;
                case Keys.Down:
                    newY++;
                    break;
            }

            if (newX >= 0 && newX < width && newY >= 0 && newY < height && maze[newY, newX].type != MazeObject.MazeObjectType.WALL)
            {
                images[playerY, playerX].BackgroundImage = new MazeObject(MazeObject.MazeObjectType.HALL).texture;

                playerX = newX;
                playerY = newY;

                images[playerY, playerX].BackgroundImage = new MazeObject(MazeObject.MazeObjectType.CHAR).texture;

                if (playerX == width - 1 && health > 0)
                {
                    MessageBox.Show("Победа - найден выход!");
                    parent.Close();
                }
            }

            if (newX >= 0 && newX < width && newY >= 0 && newY < height && maze[newY, newX].type != MazeObject.MazeObjectType.WALL)
            {
                playerX = newX;
                playerY = newY;

                if (maze[playerY, playerX].type == MazeObject.MazeObjectType.MEDAL)
                {
                    maze[playerY, playerX].type = MazeObject.MazeObjectType.HALL;
                    collectedMedals++;
                    ShowCollectedMedals();
                }
                else if (maze[playerY, playerX].type == MazeObject.MazeObjectType.ENEMY)
                {
                    maze[playerY, playerX].type = MazeObject.MazeObjectType.HALL;
                    int damage = r.Next(20, 26);
                    health -= damage;

                    if (health <= 0)
                    {
                        MessageBox.Show("Поражение - закончилось здоровье!");
                        parent.Close();
                    }
                }
                else if (maze[playerY, playerX].type == MazeObject.MazeObjectType.MEDICINE)
                {

                    if (health < 100)
                    {
                        health += 5;
                        maze[playerY, playerX].type = MazeObject.MazeObjectType.HALL;
                        if (health > 100)
                        {
                            health = 100;
                        }
                    }
                    ShowCollectedMedals();
                }

                if (collectedMedals == totalMedals && health > 0)
                {
                    MessageBox.Show("Победа - все медали собраны!");
                    parent.Close();
                }
            }

        }

        public void ShowCollectedMedals()
        {
            parent.Text = "Собрано медалей: " + collectedMedals + " | Здоровье: " + health + "%";
        }
    }
}
