public abstract class AShapeSide : IShape
{
    public double Longueur { get; set; }
    public double Largeur { get; set; }

    public double GetArea(){
        return 0;
    }

    public double GetPerimeter(){
        return 0;
    }

    public abstract void Print();
}