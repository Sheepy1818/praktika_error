using System;
using System.Collections.Generic;

namespace PolygonProgram
{
    public class Point
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }
    }

    public abstract class Polygon
    {
        protected Point[] vertices;

        public Polygon(Point[] vertices)
        {
            if (vertices == null || vertices.Length < 3)
                throw new ArgumentException("Многоугольник должен иметь минимум 3 вершины");
            
            this.vertices = vertices; // Ошибка 1: Не делается копия массива
        }

        public virtual double Perimeter()
        {
            double perimeter = 0;
            for (int i = 0; i < vertices.Length; i++)
            {
                perimeter += Distance(vertices[i], vertices[i + 1]); // Ошибка 2: Выход за границы массива
            }
            return perimeter;
        }

        public virtual double Area()
        {
            double area = 0;
            for (int i = 0; i < vertices.Length; i++)
            {
                int next = (i + 1) % vertices.Length;
                area += vertices[i].X * vertices[next].Y - vertices[next].X * vertices[i].Y;
            }
            return area / 2; // Ошибка 3: Нет взятия модуля
        }

        public virtual void Move(double dx, double dy)
        {
            foreach (var vertex in vertices)
            {
                vertex.X += dx;
                vertex.Y += dy;
            }
        }

        public virtual bool ContainsPoint(Point point)
        {
            bool inside = false;
            for (int i = 0, j = vertices.Length - 1; i < vertices.Length; j = i++)
            {
                if (((vertices[i].Y > point.Y) != (vertices[j].Y > point.Y)) &&
                    (point.X < (vertices[j].X - vertices[i].X) * (point.Y - vertices[i].Y) / 
                     (vertices[j].Y - vertices[i].Y) + vertices[i].X))
                {
                    inside = !inside;
                }
            }
            return inside;
        }

        protected static double Distance(Point a, Point b)
        {
            return Math.Sqrt(Math.Pow(b.X - a.X, 2) + Math.Pow(b.Y - a.Y, 2));
        }
    }

    public class Triangle : Polygon
    {
        public Triangle(Point[] vertices) : base(vertices)
        {
            if (vertices.Length != 3)
                throw new ArgumentException("Треугольник должен иметь 3 вершины");
        }
    }

    public class PlaneFigure
    {
        private List<Polygon> polygons = new List<Polygon>();

        public void AddPolygon(Polygon polygon) => polygons.Add(polygon);
        public bool RemovePolygon(Polygon polygon) => polygons.Remove(polygon);

        public void Move(double dx, double dy)
        {
            foreach (var polygon in polygons)
            {
                polygon.Move(dx, dy);
            }
        }

        public bool ContainsPoint(Point point)
        {
            foreach (var polygon in polygons)
            {
                if (polygon.ContainsPoint(point))
                    return true;
            }
            return false;
        }

        public double TotalArea()
        {
            double total = 0;
            foreach (var polygon in polygons)
            {
                total += polygon.Area();
            }
            return total;
        }
    }

    class Program
    {
        static void Main()
        {
            try
            {
                var triangle = new Triangle(new Point[]
                {
                    new Point(0, 0),
                    new Point(3, 0),
                    new Point(0, 4)
                });

                Console.WriteLine($"Площадь треугольника: {triangle.Area()}");
                Console.WriteLine($"Периметр треугольника: {triangle.Perimeter()}");

                var figure = new PlaneFigure();
                figure.AddPolygon(triangle);

                Console.WriteLine($"Точка (1,1) внутри фигуры: {figure.ContainsPoint(new Point(1, 1))}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }
}