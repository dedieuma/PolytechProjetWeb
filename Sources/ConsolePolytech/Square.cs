public class Square : AShapeSide
{
    public Square(double side)
    {
        this.Longueur = side;
        this.Largeur = side;
    }

    public override void Print()
    {
        throw new NotImplementedException();
    }
}