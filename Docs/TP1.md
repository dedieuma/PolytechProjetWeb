# TP 1

## Objectifs : 

- Créer un projet dotnet Console
- S'assurer que le template de base fonctionne
- Découvrir la syntaxe dotnet
- Créer un projet WebAPI
- (Bonus) Créer des nouvelles formes


## (1) Créer un projet Console

Ouvrez Vs Code, placez vous dans un nouveau dossier avec le terminal, entrez la commande

`dotnet new console -n "<nom-du-projet>"` en remplacant `<nom-du-projet>` par votre choix.

> 💡 Cela crée un nouveau projet dotnet basé sur un template prédéfini de type console.

Un seul fichier nous intéresse : Program.cs, qui ne possède qu'une seule ligne.

Allez à la racine du projet, assurez vous que le Hello World fonctionne avec la commande `dotnet run`

> Cela devrait afficher Hello, World !

> 💡 dotnet run effectue au préalable deux autres commandes, si elles n'ont pas été faites avant : `dotnet restore` pour résoudre les librairies externes (équivalent `npm install`), et `dotnet build` pour compiler le projet.

> ❗ Le projet n'a pas de méthode main ?

> 💡 La syntaxe du Program.cs est une façon moderne de créer un projet console. Beaucoup de code a été caché par les mainteneurs du Projet dotnet. Il y a bien une méthode Main() en arrière-plan, mais elle nous est cachée. Le but était d'aider les nouveaux développeurs, pour qu'ils soient moins perdus lorsqu'ils débutaient avec du code nécessaire pour faire tourner l'appli.

---

## (2) Découvrir la syntaxe dotnet

Nous allons créer des formes géométriques, sur lesquelles nous allons calculer le périmètre et la surface.

Créez un nouveau fichier `IShape.cs`, collez le code :

````csharp
public interface IShape{

    double GetPerimeter();

    double GetArea();
}
````

**Q1 : qu'est-ce qu'une interface dans un langage haut niveau ?**

Créez un fichier Rectangle.cs :

````csharp
public class Rectangle : IShape{
    
}
````

L'interface n'étant pas implémentée, assurez vous que l'éditeur signale en rouge la classe. Vous pouvez fix le problème en mettant le curseur sur le code souligné en rouge, et en faisant le raccourci `Ctrl+.` (raccourci par défaut).

Vous devriez avoir ce code :

````csharp
public class Rectangle : IShape
{
    public double GetArea()
    {
        throw new NotImplementedException();
    }

    public double GetPerimeter()
    {
        throw new NotImplementedException();
    }
}
````

Dans Program.cs, tapez : 

````csharp
Rectangle rectangle = new Rectangle();

Console.WriteLine(rectangle.GetPerimeter());
````

et faites un dotnet run.

La méthode GetPerimeter() jetant une exception, c'est ce qu'on a en sortie console...

> ❗ Mais il n'y a pas de constructeur dans Rectangle ? Comment peut-ont faire un `Rectangle rectangle = new Rectangle();` ?

> 💡 Par défaut, dotnet génère un constructeur par défaut, vide, si aucun constructeur n'est défini dans la classe.

Il est temps de rajouter les longueurs et largeurs du rectangle, avec leur constructeur...

````csharp
public class Rectangle : IShape
{
    public double Longueur { get; set; }
    public double Largeur { get; set; }

    public Rectangle(double longueur, double largeur)
    {
        this.Longueur = longueur;
        this.Largeur = largeur;
    }

    public double GetArea()
    {
        throw new NotImplementedException();
    }

    public double GetPerimeter()
    {
        throw new NotImplementedException();
    }
}
````

Mettez à jour Program.cs, qui doit s'afficher en rouge à présent...

````csharp
Rectangle rectangle = new Rectangle(10, 20);

Console.WriteLine(rectangle.GetPerimeter());
````

> 💡 `{ get; set; }` après les paramètres est du sucre syntaxique. Cela initialize des getters et setters sur ces paramètres. Essayez dans le program.cs, après `Rectangle rectangle = new Rectangle(10, 20);` de faire `rectangle.Longueur = 30;`. Essayez ensuite de supprimer le `set;` derrière Longueur dans Rectangle.cs...

**Remplissez à présent GetArea() et GetPerimeter() !**

---

Créez un Square.cs :

````csharp
public class Square : IShape
{
    public double Longueur { get; set; }
    public double Largeur { get; set; }

    public Square(double side)
    {
        this.Longueur = side;
        this.Largeur = side;
    }

    public double GetArea()
    {
        throw new NotImplementedException();
    }

    public double GetPerimeter()
    {
        throw new NotImplementedException();
    }
}
````

Remplissez à nouveau les méthodes.

> 💡 Rectangle et Square sont tous les deux des formes avec des côtés, ne pourrons-nous pas simplifier les choses ?

On pourrait donner un comportement par défaut aux formes qui sont de type 'côté'...

Nous allons créer une classe abstraite, AShapeSide.cs : 

````csharp
public abstract class AShapeSide : IShape
{
    public double Longueur { get; set; }
    public double Largeur { get; set; }

    public abstract double GetArea();

    public abstract double GetPerimeter();
}
````

**Q2 : Qu'est-ce qu'une classe abstraite ? Pourquoi l'éditeur ne devient pas rouge si on met les méthodes venant de IShape comme étant abstract ?**

Mettons à jour Rectangle et Shape...

````csharp
public class Rectangle : AShapeSide
{
    public Rectangle(double longueur, double largeur)
    {
        this.Longueur = longueur;
        this.Largeur = largeur;
    }

    public override double GetArea()
    {
        throw new NotImplementedException();
    }

    public override double GetPerimeter()
    {
        throw new NotImplementedException();
    }
}
````

````csharp
public class Square : AShapeSide
{
    public Square(double side)
    {
        this.Longueur = side;
        this.Largeur = side;
    }

    public override double GetArea()
    {
        throw new NotImplementedException();
    }

    public override double GetPerimeter()
    {
        throw new NotImplementedException();
    }
}
````

> 💡 En soi, GetArea() et GetPerimeter() sont les mêmes entre Rectangle et Square.

..Oui, on peut donc rapatrier leur comportement dans leur parent, AShapeSide.cs

````csharp
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
}
````

Rectangle et Square deviennent

````csharp
public class Rectangle : AShapeSide
{
    public Rectangle(double longueur, double largeur)
    {
        this.Longueur = longueur;
        this.Largeur = largeur;
    }
}
````

````csharp
public class Square : AShapeSide
{
    public Square(double side)
    {
        this.Longueur = side;
        this.Largeur = side;
    }
}
````

... Ils sont plutôt vides maintenant, mais ils fonctionnent toujours autant !

---

## (3) Créer un projet WebAPI

Placez votre terminal dans un nouveau dossier, et effectuez la commande

`dotnet new webapi -n "<nom-du-projet>"`

Cela crée un certain nombre de fichiers, suivant le template du type webapi.

Les fichiers les plus intéressants sont : 
- Program.cs, équivalent de la méthode main
- WeatherForcast.cs, classe générée, qui contient les informations définissant une météo à une date => On appelle cela un Model.
- Controllers/WeatherForecastController.cs : classe particulière permettant d'exposer des endpoint HTTP API Rest.

Faites un `dotnet run`

Vous devriez avoir une stack du type 
````
dotnet run
Génération...
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:XXXX
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Development
info: Microsoft.Hosting.Lifetime[0]
````

Accédez à l'url indiquée. Cela aboutit sur une page vierge, il faut rajouter `/swagger` derrière l'url. Exemple : `http://localhost:XXXX/swagger`

> Si quelque chose ne fonctionne pas, essayez de faire `dotnet dev-certs https --check --trust`

Une page particulière s'affiche : c'est une page Swagger (aussi appelée OpenAPI)

> 💡 Swagger est un format normé qui définit, via un JSON, une page permettant d'interagir avec un serveur exposant des endpoint HTTP Rest. Cela n'a pas lié à dotnet, un Json Swagger peut être exporté et utilisé par d'autres langages.

> 💡 dotnet n'expose pas par défaut un Swagger, ceci est fait via des commandes dans le Program.cs, comme par exemple `builder.Services.AddSwaggerGen();` ou `    app.UseSwagger();app.UseSwaggerUI();` Grâce à ces directives, dotnet va rechercher des endpoint HTTP dans le projet, et générer le JSON Swagger à partir de ceux-ci, et exposer le tout sous l'url /swagger.

La page est interagissable : essayez de cliquer sur le bouton bleu GET /WeatherForecast. Cliquez sur le bouton TryItOut, puis Execute : une requête HTTP GET sur http://localhost:XXXX/WeatherForecast est effectué, et envoie une réponse avec le code 200 et un body.

**Q3 : Que fait un verbe Http GET ? En existe-il d'autres ?**

**Q4 : dans le code, où est défini ce GET /WeatherForecast ? Pouvez-vous expliquer ce que fait le code ?**

Nous nous baserons sur ce projet pour le prochain TP.

## (4) BONUS : Créez une nouvelle forme

Reprenez le projet Console. Suivant le Rectangle et le carré, pouvez-vous ajouter d'autres formes ? Cercle, Triangle...

**Copiez le code des classes que vous ferez dans la feuille de réponse**

