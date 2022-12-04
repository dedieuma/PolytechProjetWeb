// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

Rectangle rectangle = new Rectangle(3, 2);

Console.WriteLine(rectangle.GetPerimeter());
rectangle.Print();

var list = new List<IShape>{

    new Rectangle(3, 2),
    new Square(5),
    new Rectangle(10, 20),
    new Rectangle(100, 20)
};