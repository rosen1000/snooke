namespace Snooke;

class Program
{
	// #region Game variables
	readonly static Vec UP = new(0, -1);
	readonly static Vec DOWN = new(0, 1);
	readonly static Vec RIGHT = new(1, 0);
	readonly static Vec LEFT = new(-1, 0);
	readonly static Random rand = new();

	const int WIDTH = 40;
	const int HEIGHT = 20;
	static int score = 0;
	static bool gameOver = false;

	readonly static Queue<Vec> snake = [];
	static Vec head = null!;
	static Vec direction = RIGHT;
	static readonly LinkedList<Vec> foods = [];
	// #endregion Game Variable

	static void Main()
	{
		Console.CursorVisible = false;
		StartGame();

		while (!gameOver)
		{
			if (Console.KeyAvailable)
			{
				var key = Console.ReadKey(true).Key;
				ChangeDirection(key);
			}

			MoveSnake();
			Draw();

			Thread.Sleep(100);
		}

		Console.SetCursorPosition(0, HEIGHT - 1);
		Console.WriteLine("Game Over! Score: " + score);
	}

	static void StartGame()
	{
		snake.Clear();
		head = new(WIDTH / 2, HEIGHT / 2);
		snake.Enqueue(head);
		GenerateFood();
		GenerateFood();
	}

	static void ChangeDirection(ConsoleKey key)
	{
		direction = key switch
		{
			ConsoleKey.UpArrow when direction != DOWN => UP,
			ConsoleKey.DownArrow when direction != UP => DOWN,
			ConsoleKey.LeftArrow when direction != RIGHT => LEFT,
			ConsoleKey.RightArrow when direction != LEFT => RIGHT,
			_ => direction
		};
	}

	static void MoveSnake()
	{
		head += direction;

		// Wrap around (some mathematics magic)
		head.X = (head.X + WIDTH) % WIDTH;
		head.Y = (head.Y + HEIGHT) % HEIGHT;

		if (snake.Contains(head))
		{
			gameOver = true;
			return;
		}

		snake.Enqueue(head);

		var f = foods.Find(head);
		if (f != null)
		{
			foods.Remove(f);
			score++;
			GenerateFood();
		}
		else
		{
			snake.Dequeue();
		}
	}

	static void GenerateFood()
	{
		Vec food;
		do
		{
			food = new Vec(rand.Next(0, WIDTH), rand.Next(0, HEIGHT));
		} while (snake.Contains(food) || foods.Contains(food));
		foods.AddLast(food);
	}

	static void DrawWalls()
	{
		Console.BackgroundColor = ConsoleColor.White;
		Console.ForegroundColor = ConsoleColor.Black;
		Console.Clear();

		// Top and Bottom walls
		for (int x = 0; x < WIDTH + 2; x++)
		{
			Console.SetCursorPosition(x, 0);
			Console.Write('#');

			Console.SetCursorPosition(x, HEIGHT + 1);
			Console.Write('#');
		}

		// Left and Right walls
		for (int y = 1; y < HEIGHT + 1; y++)
		{
			Console.SetCursorPosition(0, y);
			Console.Write('#');

			Console.SetCursorPosition(WIDTH + 1, y);
			Console.Write('#');
		}

		Console.BackgroundColor = ConsoleColor.Black;
	}

	static void Draw()
	{
		Console.Clear();
		DrawWalls();

		// Draw food
		Console.ForegroundColor = ConsoleColor.Red;
		foreach (var food in foods)
		{
			Console.SetCursorPosition(food.X + 1, food.Y + 1);
			Console.Write('O');
		}

		// Draw snake
		Console.ForegroundColor = ConsoleColor.Green;
		foreach (var part in snake)
		{
			Console.SetCursorPosition(part.X + 1, part.Y + 1);
			Console.Write('s');
		}
		Console.SetCursorPosition(head.X + 1, head.Y + 1);
		Console.Write('S');

		// Draw score
		Console.ForegroundColor = ConsoleColor.White;
		Console.SetCursorPosition(0, 0);
		Console.Write("Score: " + score);
	}
}

class Vec(int x, int y)
{
	public int X = x;
	public int Y = y;

	public static Vec operator +(Vec a, Vec b)
	{
		return new Vec(a.X + b.X, a.Y + b.Y);
	}

	public static Vec operator -(Vec a, Vec b)
	{
		return new Vec(a.X - b.X, a.Y - b.Y);
	}

	public static bool operator ==(Vec a, Vec b)
	{
		return a.X == b.X && a.Y == b.Y;
	}

	public static bool operator !=(Vec a, Vec b)
	{
		return !(a == b);
	}

	public override bool Equals(object? obj)
	{
		return obj is Vec v && v == this;
	}

	public override int GetHashCode()
	{
		return X * 37 + Y * 43;
	}
}
