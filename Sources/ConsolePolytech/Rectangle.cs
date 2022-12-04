public class Rectangle : AShapeSide
{
    public Rectangle(double longueur, double largeur)
    {
        this.Longueur = longueur;
        this.Largeur = largeur;
    }

    public override void Print()
    {
        for (int i = 1; i <= this.Largeur + 2; i++)
        {
            for (int y = 1; y <= this.Longueur; y++)
            {

                if (i == 1 || i == this.Largeur + 2)
                {
                    Console.Write("- ");
                    continue;
                }
                else if (y == 1 || y == this.Longueur)
                {
                    Console.Write("| ");
                }
                else
                {
                    Console.Write(" ");
                }
            }

            Console.WriteLine();

        }
    }
}