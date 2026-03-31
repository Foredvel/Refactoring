namespace LifeSim;

public class Herbivore : Animal
{
    public Herbivore(World world, Point2 pos, Gender? gender = null, int startEnergy = 30)
        : base(world, pos, gender)
    {
        Energy = startEnergy;
    }

    protected override int Vision => 8;

    protected override int MoveCost => 2;

    protected override int BiteGain => 18;

    protected override int ReproduceThreshold => 60;

    protected override int Energy { get; set; }

    protected override char SelfGlyph => 'h';

    public override System.ConsoleColor? Color => System.ConsoleColor.Yellow;

    protected override Organism? FindPrey() => World.FindNearest<Plant>(Pos, Vision);

    protected override Animal MakeChild(Point2 p) => new Herbivore(World, p);
}
